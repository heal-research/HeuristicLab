#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Regression.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using System.Collections.Generic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Analysis;
using HeuristicLab.Problems.DataAnalysis.Evaluators;
using HeuristicLab.Optimization.Operators;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Analyzers {
  /// <summary>
  /// "An operator for analyzing the quality of symbolic regression solutions symbolic expression tree encoding."
  /// </summary>
  [Item("PopulationSymbolicRegressionModelQualityAnalyzer", "An operator for analyzing the quality of symbolic regression solutions symbolic expression tree encoding.")]
  [StorableClass]
  public sealed class PopulationSymbolicRegressionModelQualityAnalyzer : AlgorithmOperator, ISymbolicRegressionSolutionPopulationAnalyzer {
    private const string SymbolicExpressionTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string ProblemDataParameterName = "ProblemData";
    private const string ResultsParameterName = "Results";

    private const string TrainingMeanSquaredErrorQualityParameterName = "TrainingMeanSquaredError";
    private const string MinTrainingMeanSquaredErrorQualityParameterName = "MinTrainingMeanSquaredError";
    private const string MaxTrainingMeanSquaredErrorQualityParameterName = "MaxTrainingMeanSquaredError";
    private const string AverageTrainingMeanSquaredErrorQualityParameterName = "AverageTrainingMeanSquaredError";

    private const string TrainingAverageRelativeErrorQualityParameterName = "TrainingAverageRelativeError";
    private const string MinTrainingAverageRelativeErrorQualityParameterName = "MinTrainingAverageRelativeError";
    private const string MaxTrainingAverageRelativeErrorQualityParameterName = "MaxTrainingAverageRelativeError";
    private const string AverageTrainingAverageRelativeErrorQualityParameterName = "AverageTrainingAverageRelativeError";

    private const string TrainingRSquaredQualityParameterName = "TrainingRSquared";
    private const string MinTrainingRSquaredQualityParameterName = "MinTrainingRSquared";
    private const string MaxTrainingRSquaredQualityParameterName = "MaxTrainingRSquared";
    private const string AverageTrainingRSquaredQualityParameterName = "AverageTrainingRSquared";

    private const string TestMeanSquaredErrorQualityParameterName = "TestMeanSquaredError";
    private const string MinTestMeanSquaredErrorQualityParameterName = "MinTestMeanSquaredError";
    private const string MaxTestMeanSquaredErrorQualityParameterName = "MaxTestMeanSquaredError";
    private const string AverageTestMeanSquaredErrorQualityParameterName = "AverageTestMeanSquaredError";

    private const string TestAverageRelativeErrorQualityParameterName = "TestAverageRelativeError";
    private const string MinTestAverageRelativeErrorQualityParameterName = "MinTestAverageRelativeError";
    private const string MaxTestAverageRelativeErrorQualityParameterName = "MaxTestAverageRelativeError";
    private const string AverageTestAverageRelativeErrorQualityParameterName = "AverageTestAverageRelativeError";

    private const string TestRSquaredQualityParameterName = "TestRSquared";
    private const string MinTestRSquaredQualityParameterName = "MinTestRSquared";
    private const string MaxTestRSquaredQualityParameterName = "MaxTestRSquared";
    private const string AverageTestRSquaredQualityParameterName = "AverageTestRSquared";

    private const string RSquaredValuesParameterName = "R-squared Values";
    private const string MeanSquaredErrorValuesParameterName = "Mean Squared Error Values";
    private const string RelativeErrorValuesParameterName = "Average Relative Error Values";

    private const string TrainingSamplesStartParameterName = "TrainingSamplesStart";
    private const string TrainingSamplesEndParameterName = "TrainingSamplesEnd";
    private const string TestSamplesStartParameterName = "TestSamplesStart";
    private const string TestSamplesEndParameterName = "TestSamplesEnd";
    private const string UpperEstimationLimitParameterName = "UpperEstimationLimit";
    private const string LowerEstimationLimitParameterName = "LowerEstimationLimit";

    #region parameter properties
    public ILookupParameter<ISymbolicExpressionTreeInterpreter> SymbolicExpressionTreeInterpreterParameter {
      get { return (ILookupParameter<ISymbolicExpressionTreeInterpreter>)Parameters[SymbolicExpressionTreeInterpreterParameterName]; }
    }
    public ILookupParameter<ItemArray<SymbolicExpressionTree>> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<ItemArray<SymbolicExpressionTree>>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public ILookupParameter<DataAnalysisProblemData> ProblemDataParameter {
      get { return (ILookupParameter<DataAnalysisProblemData>)Parameters[ProblemDataParameterName]; }
    }
    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters[ResultsParameterName]; }
    }
    public IValueLookupParameter<IntValue> TrainingSamplesStartParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[TrainingSamplesStartParameterName]; }
    }
    public IValueLookupParameter<IntValue> TrainingSamplesEndParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[TrainingSamplesEndParameterName]; }
    }
    public IValueLookupParameter<IntValue> TestSamplesStartParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[TestSamplesStartParameterName]; }
    }
    public IValueLookupParameter<IntValue> TestSamplesEndParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[TestSamplesEndParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> UpperEstimationLimitParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[UpperEstimationLimitParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> LowerEstimationLimitParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[LowerEstimationLimitParameterName]; }
    }
    #endregion

    public PopulationSymbolicRegressionModelQualityAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<ISymbolicExpressionTreeInterpreter>(SymbolicExpressionTreeInterpreterParameterName, "The interpreter that should be used to calculate the output values of the symbolic expression tree."));
      Parameters.Add(new ScopeTreeLookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees to analyze."));
      Parameters.Add(new LookupParameter<DataAnalysisProblemData>(ProblemDataParameterName, "The problem data containing the input varaibles for the symbolic regression problem."));
      Parameters.Add(new LookupParameter<ResultCollection>(ResultsParameterName, "The result collection where the best symbolic regression solution should be stored."));
      Parameters.Add(new ValueLookupParameter<IntValue>(TrainingSamplesStartParameterName, "The first index of the training data set partition on which the model quality values should be calculated."));
      Parameters.Add(new ValueLookupParameter<IntValue>(TrainingSamplesEndParameterName, "The last index of the training data set partition on which the model quality values should be calculated."));
      Parameters.Add(new ValueLookupParameter<IntValue>(TestSamplesStartParameterName, "The first index of the test data set partition on which the model quality values should be calculated."));
      Parameters.Add(new ValueLookupParameter<IntValue>(TestSamplesEndParameterName, "The last index of the test data set partition on which the model quality values should be calculated."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(UpperEstimationLimitParameterName, "The upper limit that should be used as cut off value for the output values of symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(LowerEstimationLimitParameterName, "The lower limit that should be used as cut off value for the output values of symbolic expression trees."));
      Parameters.Add(new LookupParameter<DataTable>(MeanSquaredErrorValuesParameterName, "The data table to collect mean squared error values."));
      Parameters.Add(new LookupParameter<DataTable>(RSquaredValuesParameterName, "The data table to collect R² correlation coefficient values."));
      Parameters.Add(new LookupParameter<DataTable>(RelativeErrorValuesParameterName, "The data table to collect relative error values."));

      #region operator initialization
      // should be extended to calculate MSE, rel. Error and R² on the training (validation) and test set
      UniformSubScopesProcessor subScopesProcessor = new UniformSubScopesProcessor();
      SymbolicRegressionModelQualityCalculator trainingQualityCalculator = new SymbolicRegressionModelQualityCalculator();
      SymbolicRegressionModelQualityCalculator testQualityCalculator = new SymbolicRegressionModelQualityCalculator();
      MinAverageMaxValueCalculator minAvgMaxTrainingMseCalculator = new MinAverageMaxValueCalculator();
      MinAverageMaxValueCalculator minAvgMaxTestMseCalculator = new MinAverageMaxValueCalculator();
      MinAverageMaxValueCalculator minAvgMaxTrainingR2Calculator = new MinAverageMaxValueCalculator();
      MinAverageMaxValueCalculator minAvgMaxTestR2Calculator = new MinAverageMaxValueCalculator();
      MinAverageMaxValueCalculator minAvgMaxTrainingRelErrorCalculator = new MinAverageMaxValueCalculator();
      MinAverageMaxValueCalculator minAvgMaxTestRelErrorCalculator = new MinAverageMaxValueCalculator();
      DataTableValuesCollector mseDataTableValuesCollector = new DataTableValuesCollector();
      DataTableValuesCollector r2DataTableValuesCollector = new DataTableValuesCollector();
      DataTableValuesCollector relErrorDataTableValuesCollector = new DataTableValuesCollector();
      ResultsCollector resultsCollector = new ResultsCollector();
      #endregion

      #region parameter wiring
      trainingQualityCalculator.LowerEstimationLimitParameter.ActualName = LowerEstimationLimitParameter.ActualName;
      trainingQualityCalculator.ProblemDataParameter.ActualName = ProblemDataParameter.ActualName;
      trainingQualityCalculator.SamplesStartParameter.ActualName = TrainingSamplesStartParameter.ActualName;
      trainingQualityCalculator.SamplesEndParameter.ActualName = TrainingSamplesEndParameter.ActualName;
      trainingQualityCalculator.SymbolicExpressionTreeInterpreterParameter.ActualName = SymbolicExpressionTreeInterpreterParameter.ActualName;
      trainingQualityCalculator.SymbolicExpressionTreeParameter.ActualName = SymbolicExpressionTreeParameter.ActualName;
      trainingQualityCalculator.UpperEstimationLimitParameter.ActualName = UpperEstimationLimitParameter.ActualName;
      trainingQualityCalculator.AverageRelativeErrorQualityParameter.ActualName = TrainingAverageRelativeErrorQualityParameterName;
      trainingQualityCalculator.MeanSquaredErrorQualityParameter.ActualName = TrainingMeanSquaredErrorQualityParameterName;
      trainingQualityCalculator.RSquaredQualityParameter.ActualName = TrainingRSquaredQualityParameterName;

      testQualityCalculator.LowerEstimationLimitParameter.ActualName = LowerEstimationLimitParameter.ActualName;
      testQualityCalculator.ProblemDataParameter.ActualName = ProblemDataParameter.ActualName;
      testQualityCalculator.SamplesStartParameter.ActualName = TestSamplesStartParameter.ActualName;
      testQualityCalculator.SamplesEndParameter.ActualName = TestSamplesEndParameter.ActualName;
      testQualityCalculator.SymbolicExpressionTreeInterpreterParameter.ActualName = SymbolicExpressionTreeInterpreterParameter.ActualName;
      testQualityCalculator.SymbolicExpressionTreeParameter.ActualName = SymbolicExpressionTreeParameter.ActualName;
      testQualityCalculator.UpperEstimationLimitParameter.ActualName = UpperEstimationLimitParameter.ActualName;
      testQualityCalculator.AverageRelativeErrorQualityParameter.ActualName = TestAverageRelativeErrorQualityParameterName;
      testQualityCalculator.MeanSquaredErrorQualityParameter.ActualName = TestMeanSquaredErrorQualityParameterName;
      testQualityCalculator.RSquaredQualityParameter.ActualName = TestRSquaredQualityParameterName;

      #region training min/avg/max
      minAvgMaxTrainingMseCalculator.AverageValueParameter.ActualName = AverageTrainingMeanSquaredErrorQualityParameterName;
      minAvgMaxTrainingMseCalculator.MaxValueParameter.ActualName = MaxTrainingMeanSquaredErrorQualityParameterName;
      minAvgMaxTrainingMseCalculator.MinValueParameter.ActualName = MinTrainingMeanSquaredErrorQualityParameterName;
      minAvgMaxTrainingMseCalculator.ValueParameter.ActualName = TrainingMeanSquaredErrorQualityParameterName;

      minAvgMaxTrainingR2Calculator.AverageValueParameter.ActualName = AverageTrainingRSquaredQualityParameterName;
      minAvgMaxTrainingR2Calculator.MaxValueParameter.ActualName = MaxTrainingRSquaredQualityParameterName;
      minAvgMaxTrainingR2Calculator.MinValueParameter.ActualName = MinTrainingRSquaredQualityParameterName;
      minAvgMaxTrainingR2Calculator.ValueParameter.ActualName = TrainingRSquaredQualityParameterName;

      minAvgMaxTrainingRelErrorCalculator.AverageValueParameter.ActualName = AverageTrainingAverageRelativeErrorQualityParameterName;
      minAvgMaxTrainingRelErrorCalculator.MaxValueParameter.ActualName = MaxTrainingAverageRelativeErrorQualityParameterName;
      minAvgMaxTrainingRelErrorCalculator.MinValueParameter.ActualName = MinTrainingAverageRelativeErrorQualityParameterName;
      minAvgMaxTrainingRelErrorCalculator.ValueParameter.ActualName = TrainingAverageRelativeErrorQualityParameterName;
      #endregion

      #region test min/avg/max
      minAvgMaxTestMseCalculator.AverageValueParameter.ActualName = AverageTestMeanSquaredErrorQualityParameterName;
      minAvgMaxTestMseCalculator.MaxValueParameter.ActualName = MaxTestMeanSquaredErrorQualityParameterName;
      minAvgMaxTestMseCalculator.MinValueParameter.ActualName = MinTestMeanSquaredErrorQualityParameterName;
      minAvgMaxTestMseCalculator.ValueParameter.ActualName = TestMeanSquaredErrorQualityParameterName;

      minAvgMaxTestR2Calculator.AverageValueParameter.ActualName = AverageTestRSquaredQualityParameterName;
      minAvgMaxTestR2Calculator.MaxValueParameter.ActualName = MaxTestRSquaredQualityParameterName;
      minAvgMaxTestR2Calculator.MinValueParameter.ActualName = MinTestRSquaredQualityParameterName;
      minAvgMaxTestR2Calculator.ValueParameter.ActualName = TestRSquaredQualityParameterName;

      minAvgMaxTestRelErrorCalculator.AverageValueParameter.ActualName = AverageTestAverageRelativeErrorQualityParameterName;
      minAvgMaxTestRelErrorCalculator.MaxValueParameter.ActualName = MaxTestAverageRelativeErrorQualityParameterName;
      minAvgMaxTestRelErrorCalculator.MinValueParameter.ActualName = MinTestAverageRelativeErrorQualityParameterName;
      minAvgMaxTestRelErrorCalculator.ValueParameter.ActualName = TestAverageRelativeErrorQualityParameterName;
      #endregion

      mseDataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>(AverageTrainingMeanSquaredErrorQualityParameterName, null, AverageTrainingMeanSquaredErrorQualityParameterName));
      mseDataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>(MaxTrainingMeanSquaredErrorQualityParameterName, null, MaxTrainingMeanSquaredErrorQualityParameterName));
      mseDataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>(MinTrainingMeanSquaredErrorQualityParameterName, null, MinTrainingMeanSquaredErrorQualityParameterName));
      mseDataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>(AverageTestMeanSquaredErrorQualityParameterName, null, AverageTestMeanSquaredErrorQualityParameterName));
      mseDataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>(MaxTestMeanSquaredErrorQualityParameterName, null, MaxTestMeanSquaredErrorQualityParameterName));
      mseDataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>(MinTestMeanSquaredErrorQualityParameterName, null, MinTestMeanSquaredErrorQualityParameterName));
      mseDataTableValuesCollector.DataTableParameter.ActualName = MeanSquaredErrorValuesParameterName;

      r2DataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>(AverageTrainingRSquaredQualityParameterName, null, AverageTrainingRSquaredQualityParameterName));
      r2DataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>(MaxTrainingRSquaredQualityParameterName, null, MaxTrainingRSquaredQualityParameterName));
      r2DataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>(MinTrainingRSquaredQualityParameterName, null, MinTrainingRSquaredQualityParameterName));
      r2DataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>(AverageTestRSquaredQualityParameterName, null, AverageTestRSquaredQualityParameterName));
      r2DataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>(MaxTestRSquaredQualityParameterName, null, MaxTestRSquaredQualityParameterName));
      r2DataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>(MinTestRSquaredQualityParameterName, null, MinTestRSquaredQualityParameterName));
      r2DataTableValuesCollector.DataTableParameter.ActualName = RSquaredValuesParameterName;

      relErrorDataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>(AverageTrainingAverageRelativeErrorQualityParameterName, null, AverageTrainingAverageRelativeErrorQualityParameterName));
      relErrorDataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>(MaxTrainingAverageRelativeErrorQualityParameterName, null, MaxTrainingAverageRelativeErrorQualityParameterName));
      relErrorDataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>(MinTrainingAverageRelativeErrorQualityParameterName, null, MinTrainingAverageRelativeErrorQualityParameterName));
      relErrorDataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>(AverageTestAverageRelativeErrorQualityParameterName, null, AverageTestAverageRelativeErrorQualityParameterName));
      relErrorDataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>(MaxTestAverageRelativeErrorQualityParameterName, null, MaxTestAverageRelativeErrorQualityParameterName));
      relErrorDataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>(MinTestAverageRelativeErrorQualityParameterName, null, MinTestAverageRelativeErrorQualityParameterName));
      relErrorDataTableValuesCollector.DataTableParameter.ActualName = RelativeErrorValuesParameterName;

      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>(MeanSquaredErrorValuesParameterName));
      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>(RSquaredValuesParameterName));
      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>(RelativeErrorValuesParameterName));
      resultsCollector.ResultsParameter.ActualName = ResultsParameter.Name;

      #endregion

      #region operator graph
      OperatorGraph.InitialOperator = subScopesProcessor;
      subScopesProcessor.Operator = trainingQualityCalculator;
      trainingQualityCalculator.Successor = testQualityCalculator;
      testQualityCalculator.Successor = null;
      subScopesProcessor.Successor = minAvgMaxTrainingMseCalculator;
      minAvgMaxTrainingMseCalculator.Successor = minAvgMaxTestMseCalculator;
      minAvgMaxTestMseCalculator.Successor = minAvgMaxTrainingR2Calculator;
      minAvgMaxTrainingR2Calculator.Successor = minAvgMaxTestR2Calculator;
      minAvgMaxTestR2Calculator.Successor = minAvgMaxTrainingRelErrorCalculator;
      minAvgMaxTrainingRelErrorCalculator.Successor = minAvgMaxTestRelErrorCalculator;
      minAvgMaxTestRelErrorCalculator.Successor = mseDataTableValuesCollector;
      mseDataTableValuesCollector.Successor = r2DataTableValuesCollector;
      r2DataTableValuesCollector.Successor = relErrorDataTableValuesCollector;
      relErrorDataTableValuesCollector.Successor = resultsCollector;
      #endregion

    }
  }
}
