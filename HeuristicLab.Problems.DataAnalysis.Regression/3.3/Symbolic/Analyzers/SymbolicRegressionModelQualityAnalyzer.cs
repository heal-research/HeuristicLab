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
using System;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Analyzers {
  /// <summary>
  /// "An operator for analyzing the quality of symbolic regression solutions symbolic expression tree encoding."
  /// </summary>
  [Item("SymbolicRegressionModelQualityAnalyzer", "An operator for analyzing the quality of symbolic regression solutions symbolic expression tree encoding.")]
  [StorableClass]
  public sealed class SymbolicRegressionModelQualityAnalyzer : AlgorithmOperator, ISymbolicRegressionAnalyzer {
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

    private const string TrainingSamplesStartParameterName = "TrainingSamplesStart";
    private const string TrainingSamplesEndParameterName = "TrainingSamplesEnd";
    private const string TestSamplesStartParameterName = "TestSamplesStart";
    private const string TestSamplesEndParameterName = "TestSamplesEnd";
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
    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters[ResultsParameterName]; }
    }
    #endregion

    [Storable]
    private UniformSubScopesProcessor subScopesProcessor;
    [Storable]
    private MinAverageMaxValueAnalyzer minAvgMaxTrainingMseAnalyzer;
    [Storable]
    private MinAverageMaxValueAnalyzer minAvgMaxTestMseAnalyzer;
    [Storable]
    private MinAverageMaxValueAnalyzer minAvgMaxTrainingRSquaredAnalyzer;
    [Storable]
    private MinAverageMaxValueAnalyzer minAvgMaxTestRSquaredAnalyzer;
    [Storable]
    private MinAverageMaxValueAnalyzer minAvgMaxTrainingRelErrorAnalyzer;
    [Storable]
    private MinAverageMaxValueAnalyzer minAvgMaxTestRelErrorAnalyzer;

    public SymbolicRegressionModelQualityAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees to analyze."));
      Parameters.Add(new ValueLookupParameter<ISymbolicExpressionTreeInterpreter>(SymbolicExpressionTreeInterpreterParameterName, "The interpreter that should be used to calculate the output values of the symbolic expression tree."));
      Parameters.Add(new ValueLookupParameter<DataAnalysisProblemData>(ProblemDataParameterName, "The problem data containing the input varaibles for the symbolic regression problem."));
      Parameters.Add(new ValueLookupParameter<IntValue>(TrainingSamplesStartParameterName, "The first index of the training data set partition on which the model quality values should be calculated."));
      Parameters.Add(new ValueLookupParameter<IntValue>(TrainingSamplesEndParameterName, "The last index of the training data set partition on which the model quality values should be calculated."));
      Parameters.Add(new ValueLookupParameter<IntValue>(TestSamplesStartParameterName, "The first index of the test data set partition on which the model quality values should be calculated."));
      Parameters.Add(new ValueLookupParameter<IntValue>(TestSamplesEndParameterName, "The last index of the test data set partition on which the model quality values should be calculated."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(UpperEstimationLimitParameterName, "The upper limit that should be used as cut off value for the output values of symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(LowerEstimationLimitParameterName, "The lower limit that should be used as cut off value for the output values of symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<DataTable>(MeanSquaredErrorValuesParameterName, "The data table to collect mean squared error values."));
      Parameters.Add(new ValueLookupParameter<DataTable>(RSquaredValuesParameterName, "The data table to collect R² correlation coefficient values."));
      Parameters.Add(new ValueLookupParameter<DataTable>(RelativeErrorValuesParameterName, "The data table to collect relative error values."));
      Parameters.Add(new LookupParameter<ResultCollection>(ResultsParameterName, "The result collection where the best symbolic regression solution should be stored."));

      #region operator initialization
      subScopesProcessor = new UniformSubScopesProcessor();
      SymbolicRegressionModelQualityCalculator trainingQualityCalculator = new SymbolicRegressionModelQualityCalculator();
      SymbolicRegressionModelQualityCalculator testQualityCalculator = new SymbolicRegressionModelQualityCalculator();
      minAvgMaxTrainingMseAnalyzer = new MinAverageMaxValueAnalyzer();
      minAvgMaxTestMseAnalyzer = new MinAverageMaxValueAnalyzer();

      minAvgMaxTrainingRSquaredAnalyzer = new MinAverageMaxValueAnalyzer();
      minAvgMaxTestRSquaredAnalyzer = new MinAverageMaxValueAnalyzer();

      minAvgMaxTrainingRelErrorAnalyzer = new MinAverageMaxValueAnalyzer();
      minAvgMaxTestRelErrorAnalyzer = new MinAverageMaxValueAnalyzer();
      #endregion

      #region parameter wiring
      subScopesProcessor.Depth.Value = SymbolicExpressionTreeParameter.Depth;
      trainingQualityCalculator.LowerEstimationLimitParameter.ActualName = LowerEstimationLimitParameter.Name;
      trainingQualityCalculator.ProblemDataParameter.ActualName = ProblemDataParameter.Name;
      trainingQualityCalculator.SamplesStartParameter.ActualName = TrainingSamplesStartParameter.Name;
      trainingQualityCalculator.SamplesEndParameter.ActualName = TrainingSamplesEndParameter.Name;
      trainingQualityCalculator.SymbolicExpressionTreeInterpreterParameter.ActualName = SymbolicExpressionTreeInterpreterParameter.Name;
      trainingQualityCalculator.SymbolicExpressionTreeParameter.ActualName = SymbolicExpressionTreeParameter.Name;
      trainingQualityCalculator.UpperEstimationLimitParameter.ActualName = UpperEstimationLimitParameter.Name;
      trainingQualityCalculator.AverageRelativeErrorQualityParameter.ActualName = TrainingAverageRelativeErrorQualityParameterName;
      trainingQualityCalculator.MeanSquaredErrorQualityParameter.ActualName = TrainingMeanSquaredErrorQualityParameterName;
      trainingQualityCalculator.RSquaredQualityParameter.ActualName = TrainingRSquaredQualityParameterName;

      testQualityCalculator.LowerEstimationLimitParameter.ActualName = LowerEstimationLimitParameter.Name;
      testQualityCalculator.ProblemDataParameter.ActualName = ProblemDataParameter.Name;
      testQualityCalculator.SamplesStartParameter.ActualName = TestSamplesStartParameter.Name;
      testQualityCalculator.SamplesEndParameter.ActualName = TestSamplesEndParameter.Name;
      testQualityCalculator.SymbolicExpressionTreeInterpreterParameter.ActualName = SymbolicExpressionTreeInterpreterParameter.Name;
      testQualityCalculator.SymbolicExpressionTreeParameter.ActualName = SymbolicExpressionTreeParameter.Name;
      testQualityCalculator.UpperEstimationLimitParameter.ActualName = UpperEstimationLimitParameter.Name;
      testQualityCalculator.AverageRelativeErrorQualityParameter.ActualName = TestAverageRelativeErrorQualityParameterName;
      testQualityCalculator.MeanSquaredErrorQualityParameter.ActualName = TestMeanSquaredErrorQualityParameterName;
      testQualityCalculator.RSquaredQualityParameter.ActualName = TestRSquaredQualityParameterName;
      #region training/test MSE
      minAvgMaxTrainingMseAnalyzer.ValueParameter.ActualName = TrainingMeanSquaredErrorQualityParameterName;
      minAvgMaxTrainingMseAnalyzer.ValueParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      minAvgMaxTrainingMseAnalyzer.AverageValueParameter.ActualName = AverageTrainingMeanSquaredErrorQualityParameterName;
      minAvgMaxTrainingMseAnalyzer.MaxValueParameter.ActualName = MaxTrainingMeanSquaredErrorQualityParameterName;
      minAvgMaxTrainingMseAnalyzer.MinValueParameter.ActualName = MinTrainingMeanSquaredErrorQualityParameterName;
      minAvgMaxTrainingMseAnalyzer.ValuesParameter.ActualName = MeanSquaredErrorValuesParameterName;
      minAvgMaxTrainingMseAnalyzer.ResultsParameter.ActualName = ResultsParameter.Name;
      minAvgMaxTrainingMseAnalyzer.CollectMinValueInResultsParameter.Value = new BoolValue(false);
      minAvgMaxTrainingMseAnalyzer.CollectAverageValueInResultsParameter.Value = new BoolValue(false);
      minAvgMaxTrainingMseAnalyzer.CollectMaxValueInResultsParameter.Value = new BoolValue(false);

      minAvgMaxTestMseAnalyzer.ValueParameter.ActualName = TestMeanSquaredErrorQualityParameterName;
      minAvgMaxTestMseAnalyzer.ValueParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      minAvgMaxTestMseAnalyzer.AverageValueParameter.ActualName = AverageTestMeanSquaredErrorQualityParameterName;
      minAvgMaxTestMseAnalyzer.MaxValueParameter.ActualName = MaxTestMeanSquaredErrorQualityParameterName;
      minAvgMaxTestMseAnalyzer.MinValueParameter.ActualName = MinTestMeanSquaredErrorQualityParameterName;
      minAvgMaxTestMseAnalyzer.ValuesParameter.ActualName = MeanSquaredErrorValuesParameterName;
      minAvgMaxTestMseAnalyzer.ResultsParameter.ActualName = ResultsParameter.Name;
      minAvgMaxTestMseAnalyzer.CollectMinValueInResultsParameter.Value = new BoolValue(false);
      minAvgMaxTestMseAnalyzer.CollectAverageValueInResultsParameter.Value = new BoolValue(false);
      minAvgMaxTestMseAnalyzer.CollectMaxValueInResultsParameter.Value = new BoolValue(false);

      #endregion
      #region training/test R²
      minAvgMaxTrainingRSquaredAnalyzer.ValueParameter.ActualName = TrainingRSquaredQualityParameterName;
      minAvgMaxTrainingRSquaredAnalyzer.ValueParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      minAvgMaxTrainingRSquaredAnalyzer.AverageValueParameter.ActualName = AverageTrainingRSquaredQualityParameterName;
      minAvgMaxTrainingRSquaredAnalyzer.MaxValueParameter.ActualName = MaxTrainingRSquaredQualityParameterName;
      minAvgMaxTrainingRSquaredAnalyzer.MinValueParameter.ActualName = MinTrainingRSquaredQualityParameterName;
      minAvgMaxTrainingRSquaredAnalyzer.ValuesParameter.ActualName = RSquaredValuesParameterName;
      minAvgMaxTrainingRSquaredAnalyzer.ResultsParameter.ActualName = ResultsParameter.Name;
      minAvgMaxTrainingRSquaredAnalyzer.CollectMinValueInResultsParameter.Value = new BoolValue(false);
      minAvgMaxTrainingRSquaredAnalyzer.CollectAverageValueInResultsParameter.Value = new BoolValue(false);
      minAvgMaxTrainingRSquaredAnalyzer.CollectMaxValueInResultsParameter.Value = new BoolValue(false);


      minAvgMaxTestRSquaredAnalyzer.ValueParameter.ActualName = TestRSquaredQualityParameterName;
      minAvgMaxTestRSquaredAnalyzer.ValueParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      minAvgMaxTestRSquaredAnalyzer.AverageValueParameter.ActualName = AverageTestRSquaredQualityParameterName;
      minAvgMaxTestRSquaredAnalyzer.MaxValueParameter.ActualName = MaxTestRSquaredQualityParameterName;
      minAvgMaxTestRSquaredAnalyzer.MinValueParameter.ActualName = MinTestRSquaredQualityParameterName;
      minAvgMaxTestRSquaredAnalyzer.ValuesParameter.ActualName = RSquaredValuesParameterName;
      minAvgMaxTestRSquaredAnalyzer.ResultsParameter.ActualName = ResultsParameter.Name;
      minAvgMaxTestRSquaredAnalyzer.CollectMinValueInResultsParameter.Value = new BoolValue(false);
      minAvgMaxTestRSquaredAnalyzer.CollectAverageValueInResultsParameter.Value = new BoolValue(false);
      minAvgMaxTestRSquaredAnalyzer.CollectMaxValueInResultsParameter.Value = new BoolValue(false);

      #endregion
      #region training/test avg. rel. error
      minAvgMaxTrainingRelErrorAnalyzer.ValueParameter.ActualName = TrainingAverageRelativeErrorQualityParameterName;
      minAvgMaxTrainingRelErrorAnalyzer.ValueParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      minAvgMaxTrainingRelErrorAnalyzer.AverageValueParameter.ActualName = AverageTrainingAverageRelativeErrorQualityParameterName;
      minAvgMaxTrainingRelErrorAnalyzer.MaxValueParameter.ActualName = MaxTrainingAverageRelativeErrorQualityParameterName;
      minAvgMaxTrainingRelErrorAnalyzer.MinValueParameter.ActualName = MinTrainingAverageRelativeErrorQualityParameterName;
      minAvgMaxTrainingRelErrorAnalyzer.ValuesParameter.ActualName = RelativeErrorValuesParameterName;
      minAvgMaxTrainingRelErrorAnalyzer.ResultsParameter.ActualName = ResultsParameter.Name;
      minAvgMaxTrainingRelErrorAnalyzer.CollectMinValueInResultsParameter.Value = new BoolValue(false);
      minAvgMaxTrainingRelErrorAnalyzer.CollectAverageValueInResultsParameter.Value = new BoolValue(false);
      minAvgMaxTrainingRelErrorAnalyzer.CollectMaxValueInResultsParameter.Value = new BoolValue(false);

      minAvgMaxTestRelErrorAnalyzer.ValueParameter.ActualName = TestAverageRelativeErrorQualityParameterName;
      minAvgMaxTestRelErrorAnalyzer.ValueParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      minAvgMaxTestRelErrorAnalyzer.AverageValueParameter.ActualName = AverageTestAverageRelativeErrorQualityParameterName;
      minAvgMaxTestRelErrorAnalyzer.MaxValueParameter.ActualName = MaxTestAverageRelativeErrorQualityParameterName;
      minAvgMaxTestRelErrorAnalyzer.MinValueParameter.ActualName = MinTestAverageRelativeErrorQualityParameterName;
      minAvgMaxTestRelErrorAnalyzer.ValuesParameter.ActualName = RelativeErrorValuesParameterName;
      minAvgMaxTestRelErrorAnalyzer.ResultsParameter.ActualName = ResultsParameter.Name;
      minAvgMaxTestRelErrorAnalyzer.CollectMinValueInResultsParameter.Value = new BoolValue(false);
      minAvgMaxTestRelErrorAnalyzer.CollectAverageValueInResultsParameter.Value = new BoolValue(false);
      minAvgMaxTestRelErrorAnalyzer.CollectMaxValueInResultsParameter.Value = new BoolValue(false);
      #endregion
      #endregion

      #region operator graph
      OperatorGraph.InitialOperator = subScopesProcessor;
      subScopesProcessor.Operator = trainingQualityCalculator;
      trainingQualityCalculator.Successor = testQualityCalculator;
      testQualityCalculator.Successor = null;
      subScopesProcessor.Successor = minAvgMaxTrainingMseAnalyzer;
      minAvgMaxTrainingMseAnalyzer.Successor = minAvgMaxTestMseAnalyzer;
      minAvgMaxTestMseAnalyzer.Successor = minAvgMaxTrainingRSquaredAnalyzer;
      minAvgMaxTrainingRSquaredAnalyzer.Successor = minAvgMaxTestRSquaredAnalyzer;
      minAvgMaxTestRSquaredAnalyzer.Successor = minAvgMaxTrainingRelErrorAnalyzer;
      minAvgMaxTrainingRelErrorAnalyzer.Successor = minAvgMaxTestRelErrorAnalyzer;
      minAvgMaxTestRelErrorAnalyzer.Successor = null;
      #endregion

      Initialize();
    }

    [StorableConstructor]
    private SymbolicRegressionModelQualityAnalyzer(bool deserializing) : base() { }

    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      SymbolicExpressionTreeParameter.DepthChanged += new EventHandler(SymbolicExpressionTreeParameter_DepthChanged);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      SymbolicRegressionModelQualityAnalyzer clone = (SymbolicRegressionModelQualityAnalyzer)base.Clone(cloner);
      clone.Initialize();
      return clone;
    }

    private void SymbolicExpressionTreeParameter_DepthChanged(object sender, EventArgs e) {
      subScopesProcessor.Depth.Value = SymbolicExpressionTreeParameter.Depth;
      minAvgMaxTrainingMseAnalyzer.ValueParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      minAvgMaxTrainingRelErrorAnalyzer.ValueParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      minAvgMaxTrainingRSquaredAnalyzer.ValueParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      minAvgMaxTestMseAnalyzer.ValueParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      minAvgMaxTestRelErrorAnalyzer.ValueParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      minAvgMaxTestRSquaredAnalyzer.ValueParameter.Depth = SymbolicExpressionTreeParameter.Depth;
    }
  }
}
