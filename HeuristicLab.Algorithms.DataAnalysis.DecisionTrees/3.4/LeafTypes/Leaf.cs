#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2017 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Problems.DataAnalysis;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("58517042-5318-4087-B098-AC75F0208BA0")]
  [Item("Leaf", "A leaf type that uses regularized linear models with feature selection as leaf models.")]
  public sealed class Leaf : LeafBase {
    #region Constructors & Cloning
    [StorableConstructor]
    private Leaf(StorableConstructorFlag _) : base(_) { }
    private Leaf(Leaf original, Cloner cloner) : base(original, cloner) { }
    public Leaf() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new Leaf(this, cloner);
    }
    #endregion

    #region IModelType
    public override bool ProvidesConfidence {
      get { return false; }
    }
    public override IRegressionModel Build(IRegressionProblemData pd, IRandom random, CancellationToken cancellationToken, out int numberOfParameters) {
      if (pd.Dataset.Rows == 0) throw new ArgumentException("The number of training instances is too small to create a leaf model");

      if (pd.Dataset.Rows == 1)
        return new ConstantLeaf().Build(pd, random, cancellationToken, out numberOfParameters);

      var means = pd.AllowedInputVariables.ToDictionary(n => n, n => pd.Dataset.GetDoubleValues(n, pd.TrainingIndices).Average());
      var variances = pd.AllowedInputVariables.ToDictionary(n => n, n => pd.Dataset.GetDoubleValues(n, pd.TrainingIndices).Variance());
      var used = pd.AllowedInputVariables.Where(v => !variances[v].IsAlmost(0.0)).ToList();

      var targetMean = pd.TargetVariableTrainingValues.Average();
      var targetVariance = pd.TargetVariableTrainingValues.Variance();

      var model = FindBestModel(variances, means, targetMean, targetVariance, pd, used);
      numberOfParameters = 1 + model.Coefficients.Count;
      return model;
    }
    public override int MinLeafSize(IRegressionProblemData pd) {
      return 1;
    }
    #endregion

    private static PreconstructedLinearModel FindBestModel(Dictionary<string, double> variances, Dictionary<string, double> means, double yMean, double yVariance, IRegressionProblemData pd, IList<string> variables) {
      Dictionary<string, double> coeffs;
      double intercept;
      do {
        coeffs = DoRegression(pd, variables, variances, means, yMean, 1.0e-8, out intercept);
        variables = DeselectColinear(variances, coeffs, yVariance, pd, variables);
      }
      while (coeffs.Count != variables.Count);
      var numAtts = variables.Count;
      var numInst = pd.TrainingIndices.Count();
      var fullMse = CalculateSE(coeffs, intercept, pd, variables);
      var akaike = 1.0 * (numInst - numAtts) + 2 * numAtts;

      var improved = true;
      var currentNumAttributes = numAtts;

      while (improved && currentNumAttributes > 1) {
        improved = false;
        currentNumAttributes--;
        // Find attribute with smallest SC (variance-scaled coefficient)
        var candidate = variables.ToDictionary(v => v, v => Math.Abs(coeffs[v] * Math.Sqrt(variances[v] / yVariance)))
          .OrderBy(x => x.Value).Select(x => x.Key).First();

        var currVariables = variables.Where(v => !v.Equals(candidate)).ToList();
        var currentIntercept = 0.0;
        var currentCoeffs = DoRegression(pd, currVariables, variances, means, yMean, 1.0e-8, out currentIntercept);
        var currentMse = CalculateSE(currentCoeffs, currentIntercept, pd, currVariables);
        var currentAkaike = currentMse / fullMse * (numInst - numAtts) + 2 * currentNumAttributes;

        if (!(currentAkaike < akaike)) continue;
        improved = true;
        akaike = currentAkaike;
        coeffs = currentCoeffs;
        intercept = currentIntercept;
        variables = currVariables;
      }

      var pd2 = new RegressionProblemData(pd.Dataset, variables, pd.TargetVariable);
      pd2.TestPartition.End = pd.TestPartition.End;
      pd2.TestPartition.Start = pd.TestPartition.Start;
      pd2.TrainingPartition.End = pd.TrainingPartition.End;
      pd2.TrainingPartition.Start = pd.TrainingPartition.Start;

      return new PreconstructedLinearModel(coeffs, intercept, pd.TargetVariable);
    }

    private static Dictionary<string, double> DoRegression(IRegressionProblemData pd, IList<string> variables, Dictionary<string, double> variances, Dictionary<string, double> means, double yMean, double ridge, out double intercept) {

      var n = variables.Count;
      var m = pd.TrainingIndices.Count();

      var inTr = new double[n, m];
      for (var i = 0; i < n; i++) {
        var v = variables[i];
        var vdata = pd.Dataset.GetDoubleValues(v, pd.TrainingIndices).ToArray();
        var sd = Math.Sqrt(variances[v]);
        var mean = means[v];
        for (var j = 0; j < m; j++) {
          inTr[i, j] = (vdata[j] - mean) / sd;
        }
      }

      var y = new double[m, 1];
      var ydata = pd.TargetVariableTrainingValues.ToArray();
      for (var i = 0; i < m; i++)
        y[i, 0] = ydata[i]; //no scaling for targets;


      var aTy = new double[n, 1];
      alglib.rmatrixgemm(n, 1, m, 1, inTr, 0, 0, 0, y, 0, 0, 0, 0, ref aTy, 0, 0); //aTy = inTr * y;
      var aTa = new double[n, n];
      alglib.rmatrixgemm(n, n, m, 1, inTr, 0, 0, 0, inTr, 0, 0, 1, 0, ref aTa, 0, 0); //aTa = inTr * t(inTr) +aTa //

      var aTaDecomp = new double[n, n];
      bool success;
      var tries = 0;
      double[] coefficients = null;
      do {
        for (var i = 0; i < n; i++) aTa[i, i] += ridge; // add ridge to diagonal to enforce singularity
        try {
          //solve "aTa * coefficients = aTy" for coefficients;
          Array.Copy(aTa, 0, aTaDecomp, 0, aTa.Length);
          alglib.spdmatrixcholesky(ref aTaDecomp, n, true);
          int info;
          alglib.densesolverreport report;
          alglib.spdmatrixcholeskysolve(aTaDecomp, n, true, ydata, out info, out report, out coefficients);

          if (info != 1) throw new Exception();
          success = true;
        }
        catch (Exception) {
          for (var i = 0; i < n; i++) aTa[i, i] -= ridge;
          ridge *= 10; // increase ridge;
          success = false;
        }
        finally {
          tries++;
        }
      }
      while (!success && tries < 100);
      if (coefficients == null || coefficients.Length != n) throw new ArgumentException("No linear model could be built");

      intercept = yMean;
      var res = new Dictionary<string, double>();
      for (var i = 0; i < n; i++) {
        var v = variables[i];
        res.Add(v, coefficients[i] /= Math.Sqrt(variances[v]));
        intercept -= coefficients[i] * means[v];
      }

      return res;
    }

    private static IList<string> DeselectColinear(Dictionary<string, double> variances, Dictionary<string, double> coeffs, double yVariance, IRegressionProblemData pd, IList<string> variables) {
      var candidates = variables.ToDictionary(v => v, v => Math.Abs(coeffs[v] * Math.Sqrt(variances[v] / yVariance))).Where(x => x.Value > 1.5).OrderBy(x => -x.Value).ToList();
      if (candidates.Count == 0) return variables;
      var c = candidates.First().Key;
      return variables.Where(v => !v.Equals(c)).ToList();
    }

    private static double CalculateSE(Dictionary<string, double> coefficients, double intercept, IRegressionProblemData pd, IList<string> variables) {
      return pd.TrainingIndices.Select(i => RegressionPrediction(i, pd, variables, coefficients, intercept) - pd.Dataset.GetDoubleValue(pd.TargetVariable, i)).Select(error => error * error).Sum();
    }
    private static double RegressionPrediction(int i, IRegressionProblemData pd, IList<string> variables, Dictionary<string, double> coefficients, double intercept) {
      return intercept + variables.Sum(v => pd.Dataset.GetDoubleValue(v, i) * coefficients[v]);
    }
  }
}