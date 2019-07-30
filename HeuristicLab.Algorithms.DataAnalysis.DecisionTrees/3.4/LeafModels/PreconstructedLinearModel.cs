#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Problems.DataAnalysis;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  // multidimensional extension of http://www2.stat.duke.edu/~tjl13/s101/slides/unit6lec3H.pdf
  [StorableType("15F2295C-28C1-48C3-8DCB-9470823C6734")]
  internal sealed class PreconstructedLinearModel : RegressionModel {
    [Storable]
    public Dictionary<string, double> Coefficients { get; private set; }
    [Storable]
    public double Intercept { get; private set; }

    public override IEnumerable<string> VariablesUsedForPrediction {
      get { return Coefficients.Keys; }
    }

    #region HLConstructors
    [StorableConstructor]
    private PreconstructedLinearModel(StorableConstructorFlag _) : base(_) { }
    private PreconstructedLinearModel(PreconstructedLinearModel original, Cloner cloner) : base(original, cloner) {
      if (original.Coefficients != null) Coefficients = original.Coefficients.ToDictionary(x => x.Key, x => x.Value);
      Intercept = original.Intercept;
    }
    public PreconstructedLinearModel(Dictionary<string, double> coefficients, double intercept, string targetvariable) : base(targetvariable) {
      Coefficients = new Dictionary<string, double>(coefficients);
      Intercept = intercept;
    }
    public PreconstructedLinearModel(double intercept, string targetvariable) : base(targetvariable) {
      Coefficients = new Dictionary<string, double>();
      Intercept = intercept;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PreconstructedLinearModel(this, cloner);
    }
    #endregion

    public static PreconstructedLinearModel CreateLinearModel(IRegressionProblemData pd, out double rmse) {
      return AlternativeCalculation(pd, out rmse);
    }

    private static PreconstructedLinearModel ClassicCalculation(IRegressionProblemData pd) {
      var inputMatrix = pd.Dataset.ToArray(pd.AllowedInputVariables.Concat(new[] {
        pd.TargetVariable
      }), pd.AllIndices);

      var nFeatures = inputMatrix.GetLength(1) - 1;
      double[] coefficients;

      alglib.linearmodel lm;
      alglib.lrreport ar;
      int retVal;
      alglib.lrbuild(inputMatrix, inputMatrix.GetLength(0), nFeatures, out retVal, out lm, out ar);
      if (retVal != 1) throw new ArgumentException("Error in calculation of linear regression solution");

      alglib.lrunpack(lm, out coefficients, out nFeatures);
      var coeffs = pd.AllowedInputVariables.Zip(coefficients, (s, d) => new {s, d}).ToDictionary(x => x.s, x => x.d);
      var res = new PreconstructedLinearModel(coeffs, coefficients[nFeatures], pd.TargetVariable);
      return res;
    }

    private static PreconstructedLinearModel AlternativeCalculation(IRegressionProblemData pd, out double rmse) {
      var variables = pd.AllowedInputVariables.ToList();
      var n = variables.Count;
      var m = pd.TrainingIndices.Count();

      //Set up X^T
      var inTr = new double[n + 1, m];
      for (var i = 0; i < n; i++) {
        var vdata = pd.Dataset.GetDoubleValues(variables[i], pd.TrainingIndices).ToArray();
        for (var j = 0; j < m; j++) inTr[i, j] = vdata[j];
      }
      for (var i = 0; i < m; i++) inTr[n, i] = 1;

      //Set up y
      var y = new double[m, 1];
      var ydata = pd.TargetVariableTrainingValues.ToArray();
      for (var i = 0; i < m; i++) y[i, 0] = ydata[i];

      //Perform linear regression
      var aTy = new double[n + 1, 1];
      var aTa = new double[n + 1, n + 1];
      var aTyVector = new double[n + 1];
      int info;
      alglib.densesolverreport report;
      double[] coefficients;

      //Perform linear regression
      alglib.rmatrixgemm(n + 1, 1, m, 1, inTr, 0, 0, 0, y, 0, 0, 0, 0, ref aTy, 0, 0); //aTy = inTr * y;
      alglib.rmatrixgemm(n + 1, n + 1, m, 1, inTr, 0, 0, 0, inTr, 0, 0, 1, 0, ref aTa, 0, 0); //aTa = inTr * t(inTr) +aTa //
      alglib.spdmatrixcholesky(ref aTa, n + 1, true);
      for (var i = 0; i < n + 1; i++) aTyVector[i] = aTy[i, 0];
      alglib.spdmatrixcholeskysolve(aTa, n + 1, true, aTyVector, out info, out report, out coefficients);

      //if Cholesky calculation fails fall back to classic linear regresseion 
      if (info != 1) {
        alglib.linearmodel lm;
        alglib.lrreport ar;
        int retVal;
        var inputMatrix = pd.Dataset.ToArray(pd.AllowedInputVariables.Concat(new[] {
          pd.TargetVariable
        }), pd.AllIndices);
        alglib.lrbuild(inputMatrix, inputMatrix.GetLength(0), n, out retVal, out lm, out ar);
        if (retVal != 1) throw new ArgumentException("Error in calculation of linear regression solution");
        alglib.lrunpack(lm, out coefficients, out n);
      }

      var coeffs = Enumerable.Range(0, n).ToDictionary(i => variables[i], i => coefficients[i]);
      var model = new PreconstructedLinearModel(coeffs, coefficients[n], pd.TargetVariable);
      rmse = pd.TrainingIndices.Select(i => pd.Dataset.GetDoubleValue(pd.TargetVariable, i) - model.GetEstimatedValue(pd.Dataset, i)).Sum(r => r * r) / m;
      rmse = Math.Sqrt(rmse);
      return model;
    }

    public override IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows) {
      return rows.Select(row => GetEstimatedValue(dataset, row));
    }

    public override IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new RegressionSolution(this, problemData);
    }

    #region helpers
    private double GetEstimatedValue(IDataset dataset, int row) {
      return Intercept + (Coefficients.Count == 0 ? 0 : Coefficients.Sum(s => s.Value * dataset.GetDoubleValue(s.Key, row)));
    }
    #endregion
  }
}