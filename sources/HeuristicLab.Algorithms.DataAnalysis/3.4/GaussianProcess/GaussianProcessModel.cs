#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Represents a Gaussian process model.
  /// </summary>
  [StorableClass]
  [Item("GaussianProcessModel", "Represents a Gaussian process posterior.")]
  public sealed class GaussianProcessModel : NamedItem, IGaussianProcessModel {
    [Storable]
    private double negativeLogLikelihood;

    public double NegativeLogLikelihood {
      get { return negativeLogLikelihood; }
    }

    [Storable]
    private ICovarianceFunction covarianceFunction;
    public ICovarianceFunction CovarianceFunction {
      get { return covarianceFunction; }
    }
    [Storable]
    private IMeanFunction meanFunction;
    public IMeanFunction MeanFunction {
      get { return meanFunction; }
    }
    [Storable]
    private double[] hyp;
    public IEnumerable<double> Hyperparameters {
      get { return hyp; }
    }
    [Storable]
    private string targetVariable;
    public string TargetVariable {
      get { return targetVariable; }
    }
    [Storable]
    private string[] allowedInputVariables;
    public string[] AllowedInputVariables {
      get { return allowedInputVariables; }
    }

    [Storable]
    private double[] alpha;
    [Storable]
    private double sqrSigmaNoise;
    [Storable]
    private double[] meanHyp;
    [Storable]
    private double[] covHyp;

    [Storable]
    private double[,] l;

    [Storable]
    private double[,] x;
    [Storable]
    private Scaling scaling;


    [StorableConstructor]
    private GaussianProcessModel(bool deserializing) : base(deserializing) { }
    private GaussianProcessModel(GaussianProcessModel original, Cloner cloner)
      : base(original, cloner) {
      this.hyp = original.hyp;
      this.meanFunction = cloner.Clone(original.meanFunction);
      this.covarianceFunction = cloner.Clone(original.covarianceFunction);
      this.negativeLogLikelihood = original.negativeLogLikelihood;
      this.targetVariable = original.targetVariable;
      this.allowedInputVariables = original.allowedInputVariables;
      this.alpha = original.alpha;
      this.sqrSigmaNoise = original.sqrSigmaNoise;
      this.scaling = cloner.Clone(original.scaling);
      this.meanHyp = original.meanHyp;
      this.covHyp = original.covHyp;
      this.l = original.l;
      this.x = original.x;
    }
    public GaussianProcessModel(Dataset ds, string targetVariable, IEnumerable<string> allowedInputVariables, IEnumerable<int> rows,
      IEnumerable<double> hyp, IMeanFunction meanFunction, ICovarianceFunction covarianceFunction)
      : base() {
      this.name = ItemName;
      this.description = ItemDescription;
      this.hyp = hyp.ToArray();
      this.meanFunction = meanFunction;
      this.covarianceFunction = covarianceFunction;
      this.targetVariable = targetVariable;
      this.allowedInputVariables = allowedInputVariables.ToArray();
      int nAllowedVariables = allowedInputVariables.Count();

      sqrSigmaNoise = Math.Exp(2.0 * hyp.First());
      sqrSigmaNoise = Math.Max(10E-6, sqrSigmaNoise); // lower limit for the noise level
      meanHyp = hyp.Skip(1).Take(meanFunction.GetNumberOfParameters(nAllowedVariables)).ToArray();
      covHyp = hyp.Skip(1 + meanFunction.GetNumberOfParameters(nAllowedVariables)).Take(covarianceFunction.GetNumberOfParameters(nAllowedVariables)).ToArray();

      CalculateModel(ds, targetVariable, allowedInputVariables, rows);
    }

    private void CalculateModel(Dataset ds, string targetVariable, IEnumerable<string> allowedInputVariables, IEnumerable<int> rows) {
      scaling = new Scaling(ds, allowedInputVariables, rows);
      x = AlglibUtil.PrepareAndScaleInputMatrix(ds, allowedInputVariables, rows, scaling);

      var y = ds.GetDoubleValues(targetVariable, rows).ToArray();

      int n = x.GetLength(0);
      l = new double[n, n];

      meanFunction.SetParameter(meanHyp, x);
      covarianceFunction.SetParameter(covHyp, x);

      // calculate means and covariances
      double[] m = meanFunction.GetMean(x);
      for (int i = 0; i < n; i++) {

        for (int j = i; j < n; j++) {
          l[j, i] = covarianceFunction.GetCovariance(i, j) / sqrSigmaNoise;
          if (j == i) l[j, i] += 1.0;
        }
      }

      // cholesky decomposition
      int info;
      alglib.densesolverreport denseSolveRep;

      var res = alglib.trfac.spdmatrixcholesky(ref l, n, false);
      if (!res) throw new InvalidOperationException("Matrix is not positive semidefinite");

      // calculate sum of diagonal elements for likelihood
      double diagSum = Enumerable.Range(0, n).Select(i => Math.Log(l[i, i])).Sum();

      // solve for alpha
      double[] ym = y.Zip(m, (a, b) => a - b).ToArray();

      alglib.spdmatrixcholeskysolve(l, n, false, ym, out info, out denseSolveRep, out alpha);
      for (int i = 0; i < alpha.Length; i++)
        alpha[i] = alpha[i] / sqrSigmaNoise;
      negativeLogLikelihood = 0.5 * Util.ScalarProd(ym, alpha) + diagSum + (n / 2.0) * Math.Log(2.0 * Math.PI * sqrSigmaNoise);
    }

    public double[] GetHyperparameterGradients() {
      // derivatives
      int n = x.GetLength(0);
      int nAllowedVariables = x.GetLength(1);
      double[,] q = new double[n, n];
      double[,] eye = new double[n, n];
      for (int i = 0; i < n; i++) eye[i, i] = 1.0;

      int info;
      alglib.densesolverreport denseSolveRep;

      alglib.spdmatrixcholeskysolvem(l, n, false, eye, n, out info, out denseSolveRep, out q);
      // double[,] a2 = outerProd(alpha, alpha);
      for (int i = 0; i < n; i++) {
        for (int j = 0; j < n; j++)
          q[i, j] = q[i, j] / sqrSigmaNoise - alpha[i] * alpha[j]; // a2[i, j];
      }

      double noiseGradient = sqrSigmaNoise * Enumerable.Range(0, n).Select(i => q[i, i]).Sum();

      double[] meanGradients = new double[meanFunction.GetNumberOfParameters(nAllowedVariables)];
      for (int i = 0; i < meanGradients.Length; i++) {
        var meanGrad = meanFunction.GetGradients(i, x);
        meanGradients[i] = -Util.ScalarProd(meanGrad, alpha);
      }

      double[] covGradients = new double[covarianceFunction.GetNumberOfParameters(nAllowedVariables)];
      if (covGradients.Length > 0) {
        for (int i = 0; i < n; i++) {
          for (int j = 0; j < n; j++) {
            var covDeriv = covarianceFunction.GetGradient(i, j);
            for (int k = 0; k < covGradients.Length; k++) {
              covGradients[k] += q[i, j] * covDeriv[k];
            }
          }
        }
        covGradients = covGradients.Select(g => g / 2.0).ToArray();
      }

      return new double[] { noiseGradient }
        .Concat(meanGradients)
        .Concat(covGradients).ToArray();
    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new GaussianProcessModel(this, cloner);
    }

    #region IRegressionModel Members
    public IEnumerable<double> GetEstimatedValues(Dataset dataset, IEnumerable<int> rows) {
      return GetEstimatedValuesHelper(dataset, rows);
    }
    public GaussianProcessRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new GaussianProcessRegressionSolution(this, problemData);
    }
    IRegressionSolution IRegressionModel.CreateRegressionSolution(IRegressionProblemData problemData) {
      return CreateRegressionSolution(problemData);
    }
    #endregion

    private IEnumerable<double> GetEstimatedValuesHelper(Dataset dataset, IEnumerable<int> rows) {
      var newX = AlglibUtil.PrepareAndScaleInputMatrix(dataset, allowedInputVariables, rows, scaling);
      int newN = newX.GetLength(0);
      int n = x.GetLength(0);
      // var predMean = new double[newN];
      // predVar = new double[newN];



      // var kss = new double[newN];
      var Ks = new double[newN, n];
      double[,] sWKs = new double[n, newN];
      // double[,] v;


      // for stddev 
      //covarianceFunction.SetParameter(covHyp, newX);
      //kss = covarianceFunction.GetDiagonalCovariances();

      covarianceFunction.SetParameter(covHyp, x, newX);
      meanFunction.SetParameter(meanHyp, newX);
      var ms = meanFunction.GetMean(newX);
      for (int i = 0; i < newN; i++) {

        for (int j = 0; j < n; j++) {
          Ks[i, j] = covarianceFunction.GetCovariance(j, i);
          sWKs[j, i] = Ks[i, j] / Math.Sqrt(sqrSigmaNoise);
        }
      }

      // for stddev 
      // alglib.rmatrixsolvem(l, n, sWKs, newN, true, out info, out denseSolveRep, out v);


      for (int i = 0; i < newN; i++) {
        // predMean[i] = ms[i] + prod(GetRow(Ks, i), alpha);
        yield return ms[i] + Util.ScalarProd(Util.GetRow(Ks, i), alpha);
        // var sumV2 = prod(GetCol(v, i), GetCol(v, i));
        // predVar[i] = kss[i] - sumV2;
      }

    }

    #region events
    public event EventHandler Changed;
    private void OnChanged(EventArgs e) {
      var handlers = Changed;
      if (handlers != null)
        handlers(this, e);
    }
    #endregion
  }
}
