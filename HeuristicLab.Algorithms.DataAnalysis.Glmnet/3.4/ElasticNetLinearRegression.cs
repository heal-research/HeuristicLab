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
using System.Threading;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;

namespace HeuristicLab.Algorithms.DataAnalysis.Glmnet {
  [Item("Elastic-net Linear Regression (LR)", "Linear regression with elastic-net regularization (wrapper for glmnet)")]
  [Creatable(CreatableAttribute.Categories.DataAnalysisRegression, Priority = 110)]
  [StorableClass]
  public sealed class ElasticNetLinearRegression : FixedDataAnalysisAlgorithm<IRegressionProblem> {
    private const string PenalityParameterName = "Penality";
    private const string LambdaParameterName = "Lambda";
    #region parameters
    public IFixedValueParameter<DoubleValue> PenalityParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[PenalityParameterName]; }
    }
    public IValueParameter<DoubleValue> LambdaParameter {
      get { return (IValueParameter<DoubleValue>)Parameters[LambdaParameterName]; }
    }
    #endregion
    #region properties
    public double Penality {
      get { return PenalityParameter.Value.Value; }
      set { PenalityParameter.Value.Value = value; }
    }
    public DoubleValue Lambda {
      get { return LambdaParameter.Value; }
      set { LambdaParameter.Value = value; }
    }
    #endregion

    [StorableConstructor]
    private ElasticNetLinearRegression(bool deserializing) : base(deserializing) { }
    private ElasticNetLinearRegression(ElasticNetLinearRegression original, Cloner cloner)
      : base(original, cloner) {
    }
    public ElasticNetLinearRegression()
      : base() {
      Problem = new RegressionProblem();
      Parameters.Add(new FixedValueParameter<DoubleValue>(PenalityParameterName, "Penalty factor (alpha) for balancing between ridge (0.0) and lasso (1.0) regression", new DoubleValue(0.5)));
      Parameters.Add(new OptionalValueParameter<DoubleValue>(LambdaParameterName, "Optional: the value of lambda for which to calculate an elastic-net solution. lambda == null => calculate the whole path of all lambdas"));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ElasticNetLinearRegression(this, cloner);
    }

    protected override void Run(CancellationToken cancellationToken) {
      if (Lambda == null) {
        CreateSolutionPath();
      } else {
        CreateSolution(Lambda.Value);
      }
    }

  private void CreateSolution(double lambda) {
      double trainNMSE;
      double testNMSE;
      var coeff = CalculateModelCoefficients(Problem.ProblemData, Penality, lambda, out trainNMSE, out testNMSE);
      Results.Add(new Result("NMSE (train)", new DoubleValue(trainNMSE)));
      Results.Add(new Result("NMSE (test)", new DoubleValue(testNMSE)));

      var solution = CreateSymbolicSolution(coeff, Problem.ProblemData);
      Results.Add(new Result(solution.Name, solution.Description, solution));
    }

    public static IRegressionSolution CreateSymbolicSolution(double[] coeff, IRegressionProblemData problemData) {
      var ds = problemData.Dataset;
      var allVariables = problemData.AllowedInputVariables.ToArray();
      var doubleVariables = allVariables.Where(ds.VariableHasType<double>);
      var factorVariableNames = allVariables.Where(ds.VariableHasType<string>);
      var factorVariablesAndValues = ds.GetFactorVariableValues(factorVariableNames, Enumerable.Range(0, ds.Rows)); // must consider all factor values (in train and test set)

      List<KeyValuePair<string, IEnumerable<string>>> remainingFactorVariablesAndValues = new List<KeyValuePair<string, IEnumerable<string>>>();
      List<double> factorCoeff = new List<double>();
      List<string> remainingDoubleVariables = new List<string>();
      List<double> doubleVarCoeff = new List<double>();

      {
        int i = 0;
        // find factor varibles & value combinations with non-zero coeff
        foreach (var factorVarAndValues in factorVariablesAndValues) {
          var l = new List<string>();
          foreach (var factorValue in factorVarAndValues.Value) {
            if (!coeff[i].IsAlmost(0.0)) {
              l.Add(factorValue);
              factorCoeff.Add(coeff[i]);
            }
            i++;
          }
          if (l.Any()) remainingFactorVariablesAndValues.Add(new KeyValuePair<string, IEnumerable<string>>(factorVarAndValues.Key, l));
        }
        // find double variables with non-zero coeff
        foreach (var doubleVar in doubleVariables) {
          if (!coeff[i].IsAlmost(0.0)) {
            remainingDoubleVariables.Add(doubleVar);
            doubleVarCoeff.Add(coeff[i]);
          }
          i++;
        }
      }
      var tree = LinearModelToTreeConverter.CreateTree(
        remainingFactorVariablesAndValues, factorCoeff.ToArray(),
        remainingDoubleVariables.ToArray(), doubleVarCoeff.ToArray(),
        coeff.Last());


      SymbolicRegressionSolution solution = new SymbolicRegressionSolution(
        new SymbolicRegressionModel(problemData.TargetVariable, tree, new SymbolicDataAnalysisExpressionTreeInterpreter()),
        (IRegressionProblemData)problemData.Clone());
      solution.Model.Name = "Elastic-net Linear Regression Model";
      solution.Name = "Elastic-net Linear Regression Solution";

      return solution;
    }

    private void CreateSolutionPath() {
      double[] lambda;
      double[] trainNMSE;
      double[] testNMSE;
      double[,] coeff;
      double[] intercept;
      RunElasticNetLinearRegression(Problem.ProblemData, Penality, out lambda, out trainNMSE, out testNMSE, out coeff, out intercept);

      var coeffTable = new IndexedDataTable<double>("Coefficients", "The paths of standarized coefficient values over different lambda values");
      coeffTable.VisualProperties.YAxisMaximumAuto = false;
      coeffTable.VisualProperties.YAxisMinimumAuto = false;
      coeffTable.VisualProperties.XAxisMaximumAuto = false;
      coeffTable.VisualProperties.XAxisMinimumAuto = false;

      coeffTable.VisualProperties.XAxisLogScale = true;
      coeffTable.VisualProperties.XAxisTitle = "Lambda";
      coeffTable.VisualProperties.YAxisTitle = "Coefficients";
      coeffTable.VisualProperties.SecondYAxisTitle = "Number of variables";

      var nLambdas = lambda.Length;
      var nCoeff = coeff.GetLength(1);
      var dataRows = new IndexedDataRow<double>[nCoeff];
      var allowedVars = Problem.ProblemData.AllowedInputVariables.ToArray();
      var numNonZeroCoeffs = new int[nLambdas];

      var ds = Problem.ProblemData.Dataset;
      var doubleVariables = allowedVars.Where(ds.VariableHasType<double>);
      var factorVariableNames = allowedVars.Where(ds.VariableHasType<string>);
      var factorVariablesAndValues = ds.GetFactorVariableValues(factorVariableNames, Enumerable.Range(0, ds.Rows)); // must consider all factor values (in train and test set)
      {
        int i = 0;
        foreach (var factorVariableAndValues in factorVariablesAndValues) {
          foreach (var factorValue in factorVariableAndValues.Value) {
            double sigma = ds.GetStringValues(factorVariableAndValues.Key)
              .Select(s => s == factorValue ? 1.0 : 0.0)
              .StandardDeviation(); // calc std dev of binary indicator
            var path = Enumerable.Range(0, nLambdas).Select(r => Tuple.Create(lambda[r], coeff[r, i] * sigma)).ToArray();
            dataRows[i] = new IndexedDataRow<double>(factorVariableAndValues.Key + "=" + factorValue, factorVariableAndValues.Key + "=" + factorValue, path);
            i++;
          }
        }

        foreach (var doubleVariable in doubleVariables) {
          double sigma = ds.GetDoubleValues(doubleVariable).StandardDeviation();
          var path = Enumerable.Range(0, nLambdas).Select(r => Tuple.Create(lambda[r], coeff[r, i] * sigma)).ToArray();
          dataRows[i] = new IndexedDataRow<double>(doubleVariable, doubleVariable, path);
          i++;
        }
        // add to coeffTable by total weight (larger area under the curve => more important);
        foreach (var r in dataRows.OrderByDescending(r => r.Values.Select(t => t.Item2).Sum(x => Math.Abs(x)))) {
          coeffTable.Rows.Add(r);
        }
      }

      for (int i = 0; i < coeff.GetLength(0); i++) {
        for (int j = 0; j < coeff.GetLength(1); j++) {
          if (!coeff[i, j].IsAlmost(0.0)) {
            numNonZeroCoeffs[i]++;
          }
        }
      }
      if (lambda.Length > 2) {
        coeffTable.VisualProperties.XAxisMinimumFixedValue = Math.Pow(10, Math.Floor(Math.Log10(lambda.Last())));
        coeffTable.VisualProperties.XAxisMaximumFixedValue = Math.Pow(10, Math.Ceiling(Math.Log10(lambda.Skip(1).First())));
      }
      coeffTable.Rows.Add(new IndexedDataRow<double>("Number of variables", "The number of non-zero coefficients for each step in the path", lambda.Zip(numNonZeroCoeffs, (l, v) => Tuple.Create(l, (double)v))));
      coeffTable.Rows["Number of variables"].VisualProperties.ChartType = DataRowVisualProperties.DataRowChartType.Points;
      coeffTable.Rows["Number of variables"].VisualProperties.SecondYAxis = true;

      Results.Add(new Result(coeffTable.Name, coeffTable.Description, coeffTable));

      var errorTable = new IndexedDataTable<double>("NMSE", "Path of NMSE values over different lambda values");
      errorTable.VisualProperties.YAxisMaximumAuto = false;
      errorTable.VisualProperties.YAxisMinimumAuto = false;
      errorTable.VisualProperties.XAxisMaximumAuto = false;
      errorTable.VisualProperties.XAxisMinimumAuto = false;

      errorTable.VisualProperties.YAxisMinimumFixedValue = 0;
      errorTable.VisualProperties.YAxisMaximumFixedValue = 1.0;
      errorTable.VisualProperties.XAxisLogScale = true;
      errorTable.VisualProperties.XAxisTitle = "Lambda";
      errorTable.VisualProperties.YAxisTitle = "Normalized mean of squared errors (NMSE)";
      errorTable.VisualProperties.SecondYAxisTitle = "Number of variables";
      errorTable.Rows.Add(new IndexedDataRow<double>("NMSE (train)", "Path of NMSE values over different lambda values", lambda.Zip(trainNMSE, (l, v) => Tuple.Create(l, v))));
      errorTable.Rows.Add(new IndexedDataRow<double>("NMSE (test)", "Path of NMSE values over different lambda values", lambda.Zip(testNMSE, (l, v) => Tuple.Create(l, v))));
      errorTable.Rows.Add(new IndexedDataRow<double>("Number of variables", "The number of non-zero coefficients for each step in the path", lambda.Zip(numNonZeroCoeffs, (l, v) => Tuple.Create(l, (double)v))));
      if (lambda.Length > 2) {
        errorTable.VisualProperties.XAxisMinimumFixedValue = Math.Pow(10, Math.Floor(Math.Log10(lambda.Last())));
        errorTable.VisualProperties.XAxisMaximumFixedValue = Math.Pow(10, Math.Ceiling(Math.Log10(lambda.Skip(1).First())));
      }
      errorTable.Rows["NMSE (train)"].VisualProperties.ChartType = DataRowVisualProperties.DataRowChartType.Points;
      errorTable.Rows["NMSE (test)"].VisualProperties.ChartType = DataRowVisualProperties.DataRowChartType.Points;
      errorTable.Rows["Number of variables"].VisualProperties.ChartType = DataRowVisualProperties.DataRowChartType.Points;
      errorTable.Rows["Number of variables"].VisualProperties.SecondYAxis = true;

      Results.Add(new Result(errorTable.Name, errorTable.Description, errorTable));
    }

    public static double[] CalculateModelCoefficients(IRegressionProblemData problemData, double penalty, double lambda,
            out double trainNMSE, out double testNMSE,
            double coeffLowerBound = double.NegativeInfinity, double coeffUpperBound = double.PositiveInfinity) {
      double[] trainNMSEs;
      double[] testNMSEs;
      // run for exactly one lambda
      var coeffs = CalculateModelCoefficients(problemData, penalty, new double[] { lambda }, out trainNMSEs, out testNMSEs, coeffLowerBound, coeffUpperBound);
      trainNMSE = trainNMSEs[0];
      testNMSE = testNMSEs[0];
      return coeffs[0];
    }
    public static double[][] CalculateModelCoefficients(IRegressionProblemData problemData, double penalty, double[] lambda,
            out double[] trainNMSEs, out double[] testNMSEs,
            double coeffLowerBound = double.NegativeInfinity, double coeffUpperBound = double.PositiveInfinity,
            int maxVars = -1) {
      // run for multiple user-supplied lambdas
      double[,] coeff;
      double[] intercept;
      RunElasticNetLinearRegression(problemData, penalty, lambda.Length, 1.0, lambda, out lambda, out trainNMSEs, out testNMSEs, out coeff, out intercept, coeffLowerBound, coeffUpperBound, maxVars);

      int nRows = intercept.Length;
      int nCols = coeff.GetLength(1) + 1;
      double[][] sols = new double[nRows][];
      for (int solIdx = 0; solIdx < nRows; solIdx++) {
        sols[solIdx] = new double[nCols];
        for (int cIdx = 0; cIdx < nCols - 1; cIdx++) {
          sols[solIdx][cIdx] = coeff[solIdx, cIdx];
        }
        sols[solIdx][nCols - 1] = intercept[solIdx];
      }
      return sols;
    }

    public static void RunElasticNetLinearRegression(IRegressionProblemData problemData, double penalty,
      out double[] lambda, out double[] trainNMSE, out double[] testNMSE, out double[,] coeff, out double[] intercept,
      double coeffLowerBound = double.NegativeInfinity, double coeffUpperBound = double.PositiveInfinity,
      int maxVars = -1
      ) {
      double[] userLambda = new double[0];
      // automatically determine lambda values (maximum 100 different lambda values)
      RunElasticNetLinearRegression(problemData, penalty, 100, 0.0, userLambda, out lambda, out trainNMSE, out testNMSE, out coeff, out intercept, coeffLowerBound, coeffUpperBound, maxVars);
    }

    /// <summary>
    /// Elastic net with squared-error-loss for dense predictor matrix, runs the full path of all lambdas
    /// </summary>
    /// <param name="problemData">Predictor target matrix x and target vector y</param>
    /// <param name="penalty">Penalty for balance between ridge (0.0) and lasso (1.0) regression</param>
    /// <param name="nlam">Maximum number of lambda values (default 100)</param>
    /// <param name="flmin">User control of lambda values (&lt;1.0 => minimum lambda = flmin * (largest lambda value), >= 1.0 => use supplied lambda values</param>
    /// <param name="ulam">User supplied lambda values</param>
    /// <param name="lambda">Output lambda values</param>
    /// <param name="trainNMSE">Vector of normalized mean of squared error (NMSE = Variance(res) / Variance(y)) values on the training set for each set of coefficients along the path</param>
    /// <param name="testNMSE">Vector of normalized mean of squared error (NMSE = Variance(res) / Variance(y)) values on the test set for each set of coefficients along the path</param>
    /// <param name="coeff">Vector of coefficient vectors for each solution along the path</param>
    /// <param name="intercept">Vector of intercepts for each solution along the path</param>
    /// <param name="coeffLowerBound">Optional lower bound for all coefficients</param>
    /// <param name="coeffUpperBound">Optional upper bound for all coefficients</param>
    /// <param name="maxVars">Maximum allowed number of variables in each solution along the path (-1 => all variables are allowed)</param>
    private static void RunElasticNetLinearRegression(IRegressionProblemData problemData, double penalty,
  int nlam, double flmin, double[] ulam, out double[] lambda, out double[] trainNMSE, out double[] testNMSE, out double[,] coeff, out double[] intercept,
  double coeffLowerBound = double.NegativeInfinity, double coeffUpperBound = double.PositiveInfinity,
  int maxVars = -1
  ) {
      if (penalty < 0.0 || penalty > 1.0) throw new ArgumentException("0 <= penalty <= 1", "penalty");

      double[,] trainX;
      double[,] testX;
      double[] trainY;
      double[] testY;

      PrepareData(problemData, out trainX, out trainY, out testX, out testY);
      var numTrainObs = trainX.GetLength(1);
      var numTestObs = testX.GetLength(1);
      var numVars = trainX.GetLength(0);

      int ka = 1; // => covariance updating algorithm
      double parm = penalty;
      double[] w = Enumerable.Repeat(1.0, numTrainObs).ToArray(); // all observations have the same weight
      int[] jd = new int[1]; // do not force to use any of the variables
      double[] vp = Enumerable.Repeat(1.0, numVars).ToArray(); // all predictor variables are unpenalized
      double[,] cl = new double[numVars, 2]; // use the same bounds for all coefficients
      for (int i = 0; i < numVars; i++) {
        cl[i, 0] = coeffLowerBound;
        cl[i, 1] = coeffUpperBound;
      }

      int ne = maxVars > 0 ? maxVars : numVars;
      int nx = numVars;
      double thr = 1.0e-5; // default value as recommended in glmnet
      int isd = 1; //  => regression on standardized predictor variables
      int intr = 1;  // => do include intercept in model
      int maxit = 100000; // default value as recommended in glmnet
      // outputs
      int lmu = -1;
      double[,] ca;
      int[] ia;
      int[] nin;
      int nlp = -99;
      int jerr = -99;
      double[] trainR2;
      Glmnet.elnet(ka, parm, numTrainObs, numVars, trainX, trainY, w, jd, vp, cl, ne, nx, nlam, flmin, ulam, thr, isd, intr, maxit, out lmu, out intercept, out ca, out ia, out nin, out trainR2, out lambda, out nlp, out jerr);

      trainNMSE = new double[lmu]; // elnet returns R**2 as 1 - NMSE
      testNMSE = new double[lmu];
      coeff = new double[lmu, numVars];
      for (int solIdx = 0; solIdx < lmu; solIdx++) {
        trainNMSE[solIdx] = 1.0 - trainR2[solIdx];

        // uncompress coefficients of solution
        int selectedNin = nin[solIdx];
        double[] coefficients;
        double[] selectedCa = new double[nx];
        for (int i = 0; i < nx; i++) {
          selectedCa[i] = ca[solIdx, i];
        }

        // apply to test set to calculate test NMSE values for each lambda step
        double[] fn;
        Glmnet.modval(intercept[solIdx], selectedCa, ia, selectedNin, numTestObs, testX, out fn);
        OnlineCalculatorError error;
        var nmse = OnlineNormalizedMeanSquaredErrorCalculator.Calculate(testY, fn, out error);
        if (error != OnlineCalculatorError.None) nmse = double.NaN;
        testNMSE[solIdx] = nmse;

        // uncompress coefficients
        Glmnet.uncomp(numVars, selectedCa, ia, selectedNin, out coefficients);
        for (int i = 0; i < coefficients.Length; i++) {
          coeff[solIdx, i] = coefficients[i];
        }
      }
    }

    private static void PrepareData(IRegressionProblemData problemData, out double[,] trainX, out double[] trainY,
      out double[,] testX, out double[] testY) {
      var ds = problemData.Dataset;
      var targetVariable = problemData.TargetVariable;
      var allowedInputs = problemData.AllowedInputVariables;
      trainX = PrepareInputData(ds, allowedInputs, problemData.TrainingIndices);
      trainY = ds.GetDoubleValues(targetVariable, problemData.TrainingIndices).ToArray();

      testX = PrepareInputData(ds, allowedInputs, problemData.TestIndices);
      testY = ds.GetDoubleValues(targetVariable, problemData.TestIndices).ToArray();
    }

    private static double[,] PrepareInputData(IDataset ds, IEnumerable<string> allowedInputs, IEnumerable<int> rows) {
      var doubleVariables = allowedInputs.Where(ds.VariableHasType<double>);
      var factorVariableNames = allowedInputs.Where(ds.VariableHasType<string>);
      var factorVariables = ds.GetFactorVariableValues(factorVariableNames, Enumerable.Range(0, ds.Rows)); // must consider all factor values (in train and test set)
      double[,] binaryMatrix = ds.ToArray(factorVariables, rows);
      double[,] doubleVarMatrix = ds.ToArray(doubleVariables, rows);
      var x = binaryMatrix.HorzCat(doubleVarMatrix);
      return x.Transpose();
    }
  }
}
