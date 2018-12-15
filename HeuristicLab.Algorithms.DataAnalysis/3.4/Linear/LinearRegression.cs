#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Linear regression data analysis algorithm.
  /// </summary>
  [Item("Linear Regression (LR)", "Linear regression data analysis algorithm (wrapper for ALGLIB).")]
  [Creatable(CreatableAttribute.Categories.DataAnalysisRegression, Priority = 100)]
  [StorableClass]
  public sealed class LinearRegression : FixedDataAnalysisAlgorithm<IRegressionProblem> {
    private const string LinearRegressionModelResultName = "Linear regression solution";

    [StorableConstructor]
    private LinearRegression(bool deserializing) : base(deserializing) { }
    private LinearRegression(LinearRegression original, Cloner cloner)
      : base(original, cloner) {
    }
    public LinearRegression()
      : base() {
      Problem = new RegressionProblem();
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LinearRegression(this, cloner);
    }

    #region linear regression
    protected override void Run(CancellationToken cancellationToken) {
      double rmsError, cvRmsError;
      var solution = CreateSolution(Problem.ProblemData, out rmsError, out cvRmsError);
      Results.Add(new Result(LinearRegressionModelResultName, "The linear regression solution.", solution));
      Results.Add(new Result("Root mean square error", "The root of the mean of squared errors of the linear regression solution on the training set.", new DoubleValue(rmsError)));
      Results.Add(new Result("Estimated root mean square error (cross-validation)", "The estimated root of the mean of squared errors of the linear regression solution via cross validation.", new DoubleValue(cvRmsError)));
    }

    [Obsolete("Use CreateSolution() instead")]
    public static ISymbolicRegressionSolution CreateLinearRegressionSolution(IRegressionProblemData problemData, out double rmsError, out double cvRmsError) {
      IEnumerable<string> doubleVariables;
      IEnumerable<KeyValuePair<string, IEnumerable<string>>> factorVariables;
      double[,] inputMatrix;
      PrepareData(problemData, out inputMatrix, out doubleVariables, out factorVariables);

      alglib.linearmodel lm = new alglib.linearmodel();
      alglib.lrreport ar = new alglib.lrreport();
      int nRows = inputMatrix.GetLength(0);
      int nFeatures = inputMatrix.GetLength(1) - 1;

      int retVal = 1;
      alglib.lrbuild(inputMatrix, nRows, nFeatures, out retVal, out lm, out ar);
      if (retVal != 1) throw new ArgumentException("Error in calculation of linear regression solution");
      rmsError = ar.rmserror;
      cvRmsError = ar.cvrmserror;

      double[] coefficients = new double[nFeatures + 1]; // last coefficient is for the constant
      alglib.lrunpack(lm, out coefficients, out nFeatures);
      
      int nFactorCoeff = factorVariables.Sum(kvp=>kvp.Value.Count());
      int nVarCoeff = doubleVariables.Count();
      var tree = LinearModelToTreeConverter.CreateTree(factorVariables, coefficients.Take(nFactorCoeff).ToArray(),
        doubleVariables.ToArray(), coefficients.Skip(nFactorCoeff).Take(nVarCoeff).ToArray(),
        @const: coefficients[nFeatures]);

      SymbolicRegressionSolution solution = new SymbolicRegressionSolution(new SymbolicRegressionModel(problemData.TargetVariable, tree, new SymbolicDataAnalysisExpressionTreeLinearInterpreter()), (IRegressionProblemData)problemData.Clone());
      solution.Model.Name = "Linear Regression Model";
      solution.Name = "Linear Regression Solution";
      return solution;
    }

    public static IRegressionSolution CreateSolution(IRegressionProblemData problemData, out double rmsError, out double cvRmsError) {
      IEnumerable<string> doubleVariables;
      IEnumerable<KeyValuePair<string, IEnumerable<string>>> factorVariables;
      double[,] inputMatrix;
      PrepareData(problemData, out inputMatrix, out doubleVariables, out factorVariables);

      alglib.linearmodel lm = new alglib.linearmodel();
      alglib.lrreport ar = new alglib.lrreport();
      int nRows = inputMatrix.GetLength(0);
      int nFeatures = inputMatrix.GetLength(1) - 1;

      int retVal = 1;
      alglib.lrbuild(inputMatrix, nRows, nFeatures, out retVal, out lm, out ar);
      if (retVal != 1) throw new ArgumentException("Error in calculation of linear regression solution");
      rmsError = ar.rmserror;
      cvRmsError = ar.cvrmserror;

      // get parameters of the model
      double[] w;
      int nVars;
      alglib.lrunpack(lm, out w, out nVars);

      // ar.c is the covariation matrix,  array[0..NVars,0..NVars].
      // C[i, j] = Cov(A[i], A[j])

      var solution = new LinearRegressionModel(w, ar.c, cvRmsError, problemData.TargetVariable, doubleVariables, factorVariables)
        .CreateRegressionSolution((IRegressionProblemData)problemData.Clone());
      solution.Name = "Linear Regression Solution";
      return solution;
    }

    private static void PrepareData(IRegressionProblemData problemData, 
      out double[,] inputMatrix, 
      out IEnumerable<string> doubleVariables, 
      out IEnumerable<KeyValuePair<string, IEnumerable<string>>> factorVariables) {
      var dataset = problemData.Dataset;
      string targetVariable = problemData.TargetVariable;
      IEnumerable<string> allowedInputVariables = problemData.AllowedInputVariables;
      IEnumerable<int> rows = problemData.TrainingIndices;
      doubleVariables = allowedInputVariables.Where(dataset.VariableHasType<double>);
      var factorVariableNames = allowedInputVariables.Where(dataset.VariableHasType<string>);
      factorVariables = dataset.GetFactorVariableValues(factorVariableNames, rows);
      double[,] binaryMatrix = dataset.ToArray(factorVariables, rows);
      double[,] doubleVarMatrix = dataset.ToArray(doubleVariables.Concat(new string[] { targetVariable }), rows);
      inputMatrix = binaryMatrix.HorzCat(doubleVarMatrix);

      if (inputMatrix.ContainsNanOrInfinity())
        throw new NotSupportedException("Linear regression does not support NaN or infinity values in the input dataset.");
    }
    #endregion
  }
}
