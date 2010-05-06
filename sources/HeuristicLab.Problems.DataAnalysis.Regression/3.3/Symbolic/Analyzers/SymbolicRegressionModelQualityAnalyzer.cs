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

    private const string TrainingMeanSquaredErrorQualityParameterName = "TrainingMeanSquaredError";
    private const string MinTrainingMeanSquaredErrorQualityParameterName = "MinTrainingMeanSquaredError";
    private const string MaxTrainingMeanSquaredErrorQualityParameterName = "MaxTrainingMeanSquaredError";
    private const string AverageTrainingMeanSquaredErrorQualityParameterName = "AverageTrainingMeanSquaredError";
    private const string BestTrainingMeanSquaredErrorQualityParameterName = "BestTrainingMeanSquaredError";

    private const string TrainingAverageRelativeErrorQualityParameterName = "TrainingAverageRelativeError";
    private const string MinTrainingAverageRelativeErrorQualityParameterName = "MinTrainingAverageRelativeError";
    private const string MaxTrainingAverageRelativeErrorQualityParameterName = "MaxTrainingAverageRelativeError";
    private const string AverageTrainingAverageRelativeErrorQualityParameterName = "AverageTrainingAverageRelativeError";
    private const string BestTrainingAverageRelativeErrorQualityParameterName = "BestTrainingAverageRelativeError";

    private const string TrainingRSquaredQualityParameterName = "TrainingRSquared";
    private const string MinTrainingRSquaredQualityParameterName = "MinTrainingRSquared";
    private const string MaxTrainingRSquaredQualityParameterName = "MaxTrainingRSquared";
    private const string AverageTrainingRSquaredQualityParameterName = "AverageTrainingRSquared";
    private const string BestTrainingRSquaredQualityParameterName = "BestTrainingRSquared";

    private const string TestMeanSquaredErrorQualityParameterName = "TestMeanSquaredError";
    private const string MinTestMeanSquaredErrorQualityParameterName = "MinTestMeanSquaredError";
    private const string MaxTestMeanSquaredErrorQualityParameterName = "MaxTestMeanSquaredError";
    private const string AverageTestMeanSquaredErrorQualityParameterName = "AverageTestMeanSquaredError";
    private const string BestTestMeanSquaredErrorQualityParameterName = "BestTestMeanSquaredError";

    private const string TestAverageRelativeErrorQualityParameterName = "TestAverageRelativeError";
    private const string MinTestAverageRelativeErrorQualityParameterName = "MinTestAverageRelativeError";
    private const string MaxTestAverageRelativeErrorQualityParameterName = "MaxTestAverageRelativeError";
    private const string AverageTestAverageRelativeErrorQualityParameterName = "AverageTestAverageRelativeError";
    private const string BestTestAverageRelativeErrorQualityParameterName = "BestTestAverageRelativeError";

    private const string TestRSquaredQualityParameterName = "TestRSquared";
    private const string MinTestRSquaredQualityParameterName = "MinTestRSquared";
    private const string MaxTestRSquaredQualityParameterName = "MaxTestRSquared";
    private const string AverageTestRSquaredQualityParameterName = "AverageTestRSquared";
    private const string BestTestRSquaredQualityParameterName = "BestTestRSquared";

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
    private SymbolicRegressionModelQualityCalculator trainingQualityCalculator;
    [Storable]
    private SymbolicRegressionModelQualityCalculator testQualityCalculator;
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
      trainingQualityCalculator = new SymbolicRegressionModelQualityCalculator();
      testQualityCalculator = new SymbolicRegressionModelQualityCalculator();
      minAvgMaxTrainingMseAnalyzer = new MinAverageMaxValueAnalyzer();
      minAvgMaxTestMseAnalyzer = new MinAverageMaxValueAnalyzer();

      minAvgMaxTrainingRSquaredAnalyzer = new MinAverageMaxValueAnalyzer();
      minAvgMaxTestRSquaredAnalyzer = new MinAverageMaxValueAnalyzer();

      minAvgMaxTrainingRelErrorAnalyzer = new MinAverageMaxValueAnalyzer();
      minAvgMaxTestRelErrorAnalyzer = new MinAverageMaxValueAnalyzer();
      #endregion

      #region parameter wiring
      trainingQualityCalculator.LowerEstimationLimitParameter.ActualName = LowerEstimationLimitParameter.Name;
      trainingQualityCalculator.ProblemDataParameter.ActualName = ProblemDataParameter.Name;
      trainingQualityCalculator.SamplesStartParameter.ActualName = TrainingSamplesStartParameter.Name;
      trainingQualityCalculator.SamplesEndParameter.ActualName = TrainingSamplesEndParameter.Name;
      trainingQualityCalculator.SymbolicExpressionTreeInterpreterParameter.ActualName = SymbolicExpressionTreeInterpreterParameter.Name;
      trainingQualityCalculator.SymbolicExpressionTreeParameter.ActualName = SymbolicExpressionTreeParameter.Name;
      trainingQualityCalculator.SymbolicExpressionTreeParameter.Depth = SymbolicExpressionTreeParameter.Depth;
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
      testQualityCalculator.SymbolicExpressionTreeParameter.Depth = SymbolicExpressionTreeParameter.Depth;
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

      minAvgMaxTestMseAnalyzer.ValueParameter.ActualName = TestMeanSquaredErrorQualityParameterName;
      minAvgMaxTestMseAnalyzer.ValueParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      minAvgMaxTestMseAnalyzer.AverageValueParameter.ActualName = AverageTestMeanSquaredErrorQualityParameterName;
      minAvgMaxTestMseAnalyzer.MaxValueParameter.ActualName = MaxTestMeanSquaredErrorQualityParameterName;
      minAvgMaxTestMseAnalyzer.MinValueParameter.ActualName = MinTestMeanSquaredErrorQualityParameterName;
      minAvgMaxTestMseAnalyzer.ValuesParameter.ActualName = MeanSquaredErrorValuesParameterName;
      minAvgMaxTestMseAnalyzer.ResultsParameter.ActualName = ResultsParameter.Name;
      #endregion
      #region training/test R²
      minAvgMaxTrainingRSquaredAnalyzer.ValueParameter.ActualName = TrainingRSquaredQualityParameterName;
      minAvgMaxTrainingRSquaredAnalyzer.ValueParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      minAvgMaxTrainingRSquaredAnalyzer.AverageValueParameter.ActualName = AverageTrainingRSquaredQualityParameterName;
      minAvgMaxTrainingRSquaredAnalyzer.MaxValueParameter.ActualName = MaxTrainingRSquaredQualityParameterName;
      minAvgMaxTrainingRSquaredAnalyzer.MinValueParameter.ActualName = MinTrainingRSquaredQualityParameterName;
      minAvgMaxTrainingRSquaredAnalyzer.ValuesParameter.ActualName = RSquaredValuesParameterName;
      minAvgMaxTrainingRSquaredAnalyzer.ResultsParameter.ActualName = ResultsParameter.Name;

      minAvgMaxTestRSquaredAnalyzer.ValueParameter.ActualName = TestRSquaredQualityParameterName;
      minAvgMaxTestRSquaredAnalyzer.ValueParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      minAvgMaxTestRSquaredAnalyzer.AverageValueParameter.ActualName = AverageTestRSquaredQualityParameterName;
      minAvgMaxTestRSquaredAnalyzer.MaxValueParameter.ActualName = MaxTestRSquaredQualityParameterName;
      minAvgMaxTestRSquaredAnalyzer.MinValueParameter.ActualName = MinTestRSquaredQualityParameterName;
      minAvgMaxTestRSquaredAnalyzer.ValuesParameter.ActualName = RSquaredValuesParameterName;
      minAvgMaxTestRSquaredAnalyzer.ResultsParameter.ActualName = ResultsParameter.Name;
      #endregion
      #region training/test avg. rel. error
      minAvgMaxTrainingRelErrorAnalyzer.ValueParameter.ActualName = TrainingAverageRelativeErrorQualityParameterName;
      minAvgMaxTrainingRelErrorAnalyzer.ValueParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      minAvgMaxTrainingRelErrorAnalyzer.AverageValueParameter.ActualName = AverageTrainingAverageRelativeErrorQualityParameterName;
      minAvgMaxTrainingRelErrorAnalyzer.MaxValueParameter.ActualName = MaxTrainingAverageRelativeErrorQualityParameterName;
      minAvgMaxTrainingRelErrorAnalyzer.MinValueParameter.ActualName = MinTrainingAverageRelativeErrorQualityParameterName;
      minAvgMaxTrainingRelErrorAnalyzer.ValuesParameter.ActualName = RelativeErrorValuesParameterName;
      minAvgMaxTrainingRelErrorAnalyzer.ResultsParameter.ActualName = ResultsParameter.Name;

      minAvgMaxTestRelErrorAnalyzer.ValueParameter.ActualName = TestAverageRelativeErrorQualityParameterName;
      minAvgMaxTestRelErrorAnalyzer.ValueParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      minAvgMaxTestRelErrorAnalyzer.AverageValueParameter.ActualName = AverageTestAverageRelativeErrorQualityParameterName;
      minAvgMaxTestRelErrorAnalyzer.MaxValueParameter.ActualName = MaxTestAverageRelativeErrorQualityParameterName;
      minAvgMaxTestRelErrorAnalyzer.MinValueParameter.ActualName = MinTestAverageRelativeErrorQualityParameterName;
      minAvgMaxTestRelErrorAnalyzer.ValuesParameter.ActualName = RelativeErrorValuesParameterName;
      minAvgMaxTestRelErrorAnalyzer.ResultsParameter.ActualName = ResultsParameter.Name;
      #endregion
      #endregion

      #region operator graph
      OperatorGraph.InitialOperator = trainingQualityCalculator;
      trainingQualityCalculator.Successor = testQualityCalculator;
      testQualityCalculator.Successor = minAvgMaxTrainingMseAnalyzer;
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
      trainingQualityCalculator.SymbolicExpressionTreeParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      testQualityCalculator.SymbolicExpressionTreeParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      minAvgMaxTrainingMseAnalyzer.ValueParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      minAvgMaxTrainingRelErrorAnalyzer.ValueParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      minAvgMaxTrainingRSquaredAnalyzer.ValueParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      minAvgMaxTestMseAnalyzer.ValueParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      minAvgMaxTestRelErrorAnalyzer.ValueParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      minAvgMaxTestRSquaredAnalyzer.ValueParameter.Depth = SymbolicExpressionTreeParameter.Depth;
    }
  }
}
