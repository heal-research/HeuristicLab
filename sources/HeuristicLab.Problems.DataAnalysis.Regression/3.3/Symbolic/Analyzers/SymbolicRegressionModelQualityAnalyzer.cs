#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.DataAnalysis.Evaluators;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Analyzers {
  /// <summary>
  /// "An operator for analyzing the quality of symbolic regression solutions symbolic expression tree encoding."
  /// </summary>
  [Item("SymbolicRegressionModelQualityAnalyzer", "An operator for analyzing the quality of symbolic regression solutions symbolic expression tree encoding.")]
  [StorableClass]
  [NonDiscoverableType]
  public sealed class SymbolicRegressionModelQualityAnalyzer : SingleSuccessorOperator, ISymbolicRegressionAnalyzer {
    private const string SymbolicExpressionTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string ProblemDataParameterName = "ProblemData";
    private const string ResultsParameterName = "Results";

    private const string TrainingMeanSquaredErrorQualityParameterName = "Mean squared error (training)";
    private const string MinTrainingMeanSquaredErrorQualityParameterName = "Min mean squared error (training)";
    private const string MaxTrainingMeanSquaredErrorQualityParameterName = "Max mean squared error (training)";
    private const string AverageTrainingMeanSquaredErrorQualityParameterName = "Average mean squared error (training)";
    private const string BestTrainingMeanSquaredErrorQualityParameterName = "Best mean squared error (training)";

    private const string TrainingAverageRelativeErrorQualityParameterName = "Average relative error (training)";
    private const string MinTrainingAverageRelativeErrorQualityParameterName = "Min average relative error (training)";
    private const string MaxTrainingAverageRelativeErrorQualityParameterName = "Max average relative error (training)";
    private const string AverageTrainingAverageRelativeErrorQualityParameterName = "Average average relative error (training)";
    private const string BestTrainingAverageRelativeErrorQualityParameterName = "Best average relative error (training)";

    private const string TrainingRSquaredQualityParameterName = "R² (training)";
    private const string MinTrainingRSquaredQualityParameterName = "Min R² (training)";
    private const string MaxTrainingRSquaredQualityParameterName = "Max R² (training)";
    private const string AverageTrainingRSquaredQualityParameterName = "Average R² (training)";
    private const string BestTrainingRSquaredQualityParameterName = "Best R² (training)";

    private const string TestMeanSquaredErrorQualityParameterName = "Mean squared error (test)";
    private const string MinTestMeanSquaredErrorQualityParameterName = "Min mean squared error (test)";
    private const string MaxTestMeanSquaredErrorQualityParameterName = "Max mean squared error (test)";
    private const string AverageTestMeanSquaredErrorQualityParameterName = "Average mean squared error (test)";
    private const string BestTestMeanSquaredErrorQualityParameterName = "Best mean squared error (test)";

    private const string TestAverageRelativeErrorQualityParameterName = "Average relative error (test)";
    private const string MinTestAverageRelativeErrorQualityParameterName = "Min average relative error (test)";
    private const string MaxTestAverageRelativeErrorQualityParameterName = "Max average relative error (test)";
    private const string AverageTestAverageRelativeErrorQualityParameterName = "Average average relative error (test)";
    private const string BestTestAverageRelativeErrorQualityParameterName = "Best average relative error (test)";

    private const string TestRSquaredQualityParameterName = "R² (test)";
    private const string MinTestRSquaredQualityParameterName = "Min R² (test)";
    private const string MaxTestRSquaredQualityParameterName = "Max R² (test)";
    private const string AverageTestRSquaredQualityParameterName = "Average R² (test)";
    private const string BestTestRSquaredQualityParameterName = "Best R² (test)";

    private const string RSquaredValuesParameterName = "R²";
    private const string MeanSquaredErrorValuesParameterName = "Mean squared error";
    private const string RelativeErrorValuesParameterName = "Average relative error";

    private const string UpperEstimationLimitParameterName = "UpperEstimationLimit";
    private const string LowerEstimationLimitParameterName = "LowerEstimationLimit";

    #region parameter properties
    public ScopeTreeLookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ScopeTreeLookupParameter<SymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public IValueLookupParameter<ISymbolicExpressionTreeInterpreter> SymbolicExpressionTreeInterpreterParameter {
      get { return (IValueLookupParameter<ISymbolicExpressionTreeInterpreter>)Parameters[SymbolicExpressionTreeInterpreterParameterName]; }
    }
    public IValueLookupParameter<DataAnalysisProblemData> ProblemDataParameter {
      get { return (IValueLookupParameter<DataAnalysisProblemData>)Parameters[ProblemDataParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> UpperEstimationLimitParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[UpperEstimationLimitParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> LowerEstimationLimitParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[LowerEstimationLimitParameterName]; }
    }
    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters[ResultsParameterName]; }
    }
    #endregion
    #region properties
    public DoubleValue UpperEstimationLimit {
      get { return UpperEstimationLimitParameter.ActualValue; }
    }
    public DoubleValue LowerEstimationLimit {
      get { return LowerEstimationLimitParameter.ActualValue; }
    }
    #endregion

    [StorableConstructor]
    private SymbolicRegressionModelQualityAnalyzer(bool deserializing) : base(deserializing) { }
    private SymbolicRegressionModelQualityAnalyzer(SymbolicRegressionModelQualityAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public SymbolicRegressionModelQualityAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees to analyze."));
      Parameters.Add(new ValueLookupParameter<ISymbolicExpressionTreeInterpreter>(SymbolicExpressionTreeInterpreterParameterName, "The interpreter that should be used to calculate the output values of the symbolic expression tree."));
      Parameters.Add(new ValueLookupParameter<DataAnalysisProblemData>(ProblemDataParameterName, "The problem data containing the input varaibles for the symbolic regression problem."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(UpperEstimationLimitParameterName, "The upper limit that should be used as cut off value for the output values of symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(LowerEstimationLimitParameterName, "The lower limit that should be used as cut off value for the output values of symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<DataTable>(MeanSquaredErrorValuesParameterName, "The data table to collect mean squared error values."));
      Parameters.Add(new ValueLookupParameter<DataTable>(RSquaredValuesParameterName, "The data table to collect R² correlation coefficient values."));
      Parameters.Add(new ValueLookupParameter<DataTable>(RelativeErrorValuesParameterName, "The data table to collect relative error values."));
      Parameters.Add(new LookupParameter<ResultCollection>(ResultsParameterName, "The result collection where the best symbolic regression solution should be stored."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionModelQualityAnalyzer(this, cloner);
    }

    public override IOperation Apply() {
      double upperEstimationLimit = UpperEstimationLimit != null ? UpperEstimationLimit.Value : double.PositiveInfinity;
      double lowerEstimationLimit = LowerEstimationLimit != null ? LowerEstimationLimit.Value : double.NegativeInfinity;
      Analyze(SymbolicExpressionTreeParameter.ActualValue, SymbolicExpressionTreeInterpreterParameter.ActualValue,
        upperEstimationLimit, lowerEstimationLimit, ProblemDataParameter.ActualValue,
        ResultsParameter.ActualValue);
      return base.Apply();
    }

    public static void Analyze(IEnumerable<SymbolicExpressionTree> trees, ISymbolicExpressionTreeInterpreter interpreter,
      double upperEstimationLimit, double lowerEstimationLimit,
      DataAnalysisProblemData problemData, ResultCollection results) {
      int targetVariableIndex = problemData.Dataset.GetVariableIndex(problemData.TargetVariable.Value);
      IEnumerable<double> originalTrainingValues = problemData.Dataset.GetEnumeratedVariableValues(targetVariableIndex, problemData.TrainingIndizes);
      IEnumerable<double> originalTestValues = problemData.Dataset.GetEnumeratedVariableValues(targetVariableIndex, problemData.TestIndizes);
      List<double> trainingMse = new List<double>();
      List<double> trainingR2 = new List<double>();
      List<double> trainingRelErr = new List<double>();
      List<double> testMse = new List<double>();
      List<double> testR2 = new List<double>();
      List<double> testRelErr = new List<double>();

      OnlineMeanSquaredErrorEvaluator mseEvaluator = new OnlineMeanSquaredErrorEvaluator();
      OnlineMeanAbsolutePercentageErrorEvaluator relErrEvaluator = new OnlineMeanAbsolutePercentageErrorEvaluator();
      OnlinePearsonsRSquaredEvaluator r2Evaluator = new OnlinePearsonsRSquaredEvaluator();

      foreach (var tree in trees) {
        #region training
        var estimatedTrainingValues = interpreter.GetSymbolicExpressionTreeValues(tree, problemData.Dataset, problemData.TrainingIndizes);
        mseEvaluator.Reset();
        r2Evaluator.Reset();
        relErrEvaluator.Reset();
        var estimatedEnumerator = estimatedTrainingValues.GetEnumerator();
        var originalEnumerator = originalTrainingValues.GetEnumerator();
        while (estimatedEnumerator.MoveNext() & originalEnumerator.MoveNext()) {
          double estimated = estimatedEnumerator.Current;
          if (double.IsNaN(estimated)) estimated = upperEstimationLimit;
          else estimated = Math.Min(upperEstimationLimit, Math.Max(lowerEstimationLimit, estimated));
          mseEvaluator.Add(originalEnumerator.Current, estimated);
          r2Evaluator.Add(originalEnumerator.Current, estimated);
          relErrEvaluator.Add(originalEnumerator.Current, estimated);
        }
        if (estimatedEnumerator.MoveNext() || originalEnumerator.MoveNext()) {
          throw new InvalidOperationException("Number of elements in estimated and original enumeration doesn't match.");
        }
        trainingMse.Add(mseEvaluator.MeanSquaredError);
        trainingR2.Add(r2Evaluator.RSquared);
        trainingRelErr.Add(relErrEvaluator.MeanAbsolutePercentageError);
        #endregion
        #region test
        var estimatedTestValues = interpreter.GetSymbolicExpressionTreeValues(tree, problemData.Dataset, problemData.TestIndizes);

        mseEvaluator.Reset();
        r2Evaluator.Reset();
        relErrEvaluator.Reset();
        estimatedEnumerator = estimatedTestValues.GetEnumerator();
        originalEnumerator = originalTestValues.GetEnumerator();
        while (estimatedEnumerator.MoveNext() & originalEnumerator.MoveNext()) {
          double estimated = estimatedEnumerator.Current;
          if (double.IsNaN(estimated)) estimated = upperEstimationLimit;
          else estimated = Math.Min(upperEstimationLimit, Math.Max(lowerEstimationLimit, estimated));
          mseEvaluator.Add(originalEnumerator.Current, estimated);
          r2Evaluator.Add(originalEnumerator.Current, estimated);
          relErrEvaluator.Add(originalEnumerator.Current, estimated);
        }
        if (estimatedEnumerator.MoveNext() || originalEnumerator.MoveNext()) {
          throw new InvalidOperationException("Number of elements in estimated and original enumeration doesn't match.");
        }
        testMse.Add(mseEvaluator.MeanSquaredError);
        testR2.Add(r2Evaluator.RSquared);
        testRelErr.Add(relErrEvaluator.MeanAbsolutePercentageError);
        #endregion
      }

      AddResultTableValues(results, MeanSquaredErrorValuesParameterName, "mean squared error (training)", trainingMse.Min(), trainingMse.Average(), trainingMse.Max());
      AddResultTableValues(results, MeanSquaredErrorValuesParameterName, "mean squared error (test)", testMse.Min(), testMse.Average(), testMse.Max());
      AddResultTableValues(results, RelativeErrorValuesParameterName, "mean relative error (training)", trainingRelErr.Min(), trainingRelErr.Average(), trainingRelErr.Max());
      AddResultTableValues(results, RelativeErrorValuesParameterName, "mean relative error (test)", testRelErr.Min(), testRelErr.Average(), testRelErr.Max());
      AddResultTableValues(results, RSquaredValuesParameterName, "Pearson's R² (training)", trainingR2.Min(), trainingR2.Average(), trainingR2.Max());
      AddResultTableValues(results, RSquaredValuesParameterName, "Pearson's R² (test)", testR2.Min(), testR2.Average(), testR2.Max());
    }

    private static void AddResultTableValues(ResultCollection results, string tableName, string valueName, double minValue, double avgValue, double maxValue) {
      if (!results.ContainsKey(tableName)) {
        results.Add(new Result(tableName, new DataTable(tableName)));
      }
      DataTable table = (DataTable)results[tableName].Value;
      AddValue(table, minValue, "Min. " + valueName, string.Empty);
      AddValue(table, avgValue, "Avg. " + valueName, string.Empty);
      AddValue(table, maxValue, "Max. " + valueName, string.Empty);
    }

    private static void AddValue(DataTable table, double data, string name, string description) {
      DataRow row;
      table.Rows.TryGetValue(name, out row);
      if (row == null) {
        row = new DataRow(name, description);
        row.Values.Add(data);
        table.Rows.Add(row);
      } else {
        row.Values.Add(data);
      }
    }


    private static void SetResultValue(ResultCollection results, string name, double value) {
      if (results.ContainsKey(name))
        results[name].Value = new DoubleValue(value);
      else
        results.Add(new Result(name, new DoubleValue(value)));
    }
  }
}
