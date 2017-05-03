#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Algorithms.DataAnalysis.KernelRidgeRegression {
  [StorableClass]
  [Item("KernelRidgeRegressionModel", "A kernel ridge regression model")]
  public sealed class KernelRidgeRegressionModel : RegressionModel {
    public override IEnumerable<string> VariablesUsedForPrediction {
      get { return allowedInputVariables; }
    }

    [Storable]
    private readonly string[] allowedInputVariables;
    public string[] AllowedInputVariables {
      get { return allowedInputVariables; }
    }


    [Storable]
    public double LooCvRMSE { get; private set; }

    [Storable]
    private readonly double[] alpha;

    [Storable]
    private readonly double[,] trainX; // it is better to store the original training dataset completely because this is more efficient in persistence

    [Storable]
    private readonly ITransformation<double>[] scaling;

    [Storable]
    private readonly ICovarianceFunction kernel;

    [Storable]
    private readonly double lambda;

    [Storable]
    private readonly double yOffset; // implementation works for zero-mean, unit-variance target variables

    [Storable]
    private readonly double yScale;

    [StorableConstructor]
    private KernelRidgeRegressionModel(bool deserializing) : base(deserializing) { }
    private KernelRidgeRegressionModel(KernelRidgeRegressionModel original, Cloner cloner)
      : base(original, cloner) {
      // shallow copies of arrays because they cannot be modified
      allowedInputVariables = original.allowedInputVariables;
      alpha = original.alpha;
      trainX = original.trainX;
      scaling = original.scaling;
      lambda = original.lambda;
      LooCvRMSE = original.LooCvRMSE;

      yOffset = original.yOffset;
      yScale = original.yScale;
      if (original.kernel != null)
        kernel = cloner.Clone(original.kernel);
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new KernelRidgeRegressionModel(this, cloner);
    }

    public KernelRidgeRegressionModel(IDataset dataset, string targetVariable, IEnumerable<string> allowedInputVariables, IEnumerable<int> rows,
      bool scaleInputs, ICovarianceFunction kernel, double lambda = 0.1) : base(targetVariable) {
      if (kernel.GetNumberOfParameters(allowedInputVariables.Count()) > 0) throw new ArgumentException("All parameters in the kernel function must be specified.");
      name = ItemName;
      description = ItemDescription;
      this.allowedInputVariables = allowedInputVariables.ToArray();
      var trainingRows = rows.ToArray();
      this.kernel = (ICovarianceFunction)kernel.Clone();
      this.lambda = lambda;
      try {
        if (scaleInputs)
          scaling = CreateScaling(dataset, trainingRows);
        trainX = ExtractData(dataset, trainingRows, scaling);
        var y = dataset.GetDoubleValues(targetVariable, trainingRows).ToArray();
        yOffset = y.Average();
        yScale = 1.0 / y.StandardDeviation();
        for (int i = 0; i < y.Length; i++) {
          y[i] -= yOffset;
          y[i] *= yScale;
        }
        int info;
        int n = trainX.GetLength(0);
        alglib.densesolverreport denseSolveRep;
        var gram = BuildGramMatrix(trainX, lambda);
        var l = new double[n, n]; Array.Copy(gram, l, l.Length);

        double[,] invG;
        // cholesky decomposition
        var res = alglib.trfac.spdmatrixcholesky(ref l, n, false);
        if (res == false) { //throw new ArgumentException("Could not decompose matrix. Is it quadratic symmetric positive definite?");
          int[] pivots;
          var lua = new double[n, n];
          Array.Copy(gram, lua, lua.Length);
          alglib.rmatrixlu(ref lua, n, n, out pivots);
          alglib.rmatrixlusolve(lua, pivots, n, y, out info, out denseSolveRep, out alpha);
          if (info != 1) throw new ArgumentException("Could not create model.");
          alglib.matinvreport rep;
          invG = lua;  // rename
          alglib.rmatrixluinverse(ref invG, pivots, n, out info, out rep);
          if (info != 1) throw new ArgumentException("Could not invert Gram matrix.");
        } else {
          alglib.spdmatrixcholeskysolve(l, n, false, y, out info, out denseSolveRep, out alpha);
          if (info != 1) throw new ArgumentException("Could not create model.");
          // for LOO-CV we need to build the inverse of the gram matrix
          alglib.matinvreport rep;
          invG = l;   // rename 
          alglib.spdmatrixcholeskyinverse(ref invG, n, false, out info, out rep);
          if (info != 1) throw new ArgumentException("Could not invert Gram matrix.");
        }

        var ssqLooError = 0.0;
        for (int i = 0; i < n; i++) {
          var pred_i = Util.ScalarProd(Util.GetRow(gram, i).ToArray(), alpha);
          var looPred_i = pred_i - alpha[i] / invG[i, i];
          var error = (y[i] - looPred_i) / yScale;
          ssqLooError += error * error;
        }
        LooCvRMSE = Math.Sqrt(ssqLooError / n);
      } catch (alglib.alglibexception ae) {
        // wrap exception so that calling code doesn't have to know about alglib implementation
        throw new ArgumentException("There was a problem in the calculation of the kernel ridge regression model", ae);
      }
    }


    #region IRegressionModel Members
    public override IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows) {
      var newX = ExtractData(dataset, rows, scaling);
      var dim = newX.GetLength(1);
      var cov = kernel.GetParameterizedCovarianceFunction(new double[0], Enumerable.Range(0, dim).ToArray());

      var pred = new double[newX.GetLength(0)];
      for (int i = 0; i < pred.Length; i++) {
        double sum = 0.0;
        for (int j = 0; j < alpha.Length; j++) {
          sum += alpha[j] * cov.CrossCovariance(trainX, newX, j, i);
        }
        pred[i] = sum / yScale + yOffset;
      }
      return pred;
    }
    public override IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new RegressionSolution(this, new RegressionProblemData(problemData));
    }
    #endregion

    #region helpers
    private double[,] BuildGramMatrix(double[,] data, double lambda) {
      var n = data.GetLength(0);
      var dim = data.GetLength(1);
      var cov = kernel.GetParameterizedCovarianceFunction(new double[0], Enumerable.Range(0, dim).ToArray());
      var gram = new double[n, n];
      // G = (K + λ I) 
      for (var i = 0; i < n; i++) {
        for (var j = i; j < n; j++) {
          gram[i, j] = gram[j, i] = cov.Covariance(data, i, j); // symmetric matrix 
        }
        gram[i, i] += lambda;
      }
      return gram;
    }

    private ITransformation<double>[] CreateScaling(IDataset dataset, int[] rows) {
      var trans = new ITransformation<double>[allowedInputVariables.Length];
      int i = 0;
      foreach (var variable in allowedInputVariables) {
        var lin = new LinearTransformation(allowedInputVariables);
        var max = dataset.GetDoubleValues(variable, rows).Max();
        var min = dataset.GetDoubleValues(variable, rows).Min();
        lin.Multiplier = 1.0 / (max - min);
        lin.Addend = -min / (max - min);
        trans[i] = lin;
        i++;
      }
      return trans;
    }

    private double[,] ExtractData(IDataset dataset, IEnumerable<int> rows, ITransformation<double>[] scaling = null) {
      double[][] variables;
      if (scaling != null) {
        variables =
          allowedInputVariables.Select((var, i) => scaling[i].Apply(dataset.GetDoubleValues(var, rows)).ToArray())
            .ToArray();
      } else {
        variables =
        allowedInputVariables.Select(var => dataset.GetDoubleValues(var, rows).ToArray()).ToArray();
      }
      int n = variables.First().Length;
      var res = new double[n, variables.Length];
      for (int r = 0; r < n; r++)
        for (int c = 0; c < variables.Length; c++) {
          res[r, c] = variables[c][r];
        }
      return res;
    }
    #endregion
  }
}
