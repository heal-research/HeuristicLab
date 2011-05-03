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
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Analyzers {
  /// <summary>
  /// An operator that analyzes the validation best scaled symbolic regression solution.
  /// </summary>
  [Item("ValidationBestScaledSymbolicRegressionSolutionAnalyzer", "An operator that analyzes the validation best scaled symbolic regression solution.")]
  [StorableClass]
  [NonDiscoverableType]
  public sealed class ValidationBestScaledSymbolicRegressionSolutionAnalyzer : AlgorithmOperator, ISymbolicRegressionAnalyzer {
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string ScaledSymbolicExpressionTreeParameterName = "ScaledSymbolicExpressionTree";
    private const string SymbolicExpressionTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string ProblemDataParameterName = "ProblemData";
    private const string TrainingSamplesStartParameterName = "TrainingSamplesStart";
    private const string TrainingSamplesEndParameterName = "TrainingSamplesEnd";
    private const string ValidationSamplesStartParameterName = "ValidationSamplesStart";
    private const string ValidationSamplesEndParameterName = "ValidationSamplesEnd";
    private const string TestSamplesStartParameterName = "TestSamplesStart";
    private const string TestSamplesEndParameterName = "TestSamplesEnd";
    private const string QualityParameterName = "Quality";
    private const string ScaledQualityParameterName = "ScaledQuality";
    private const string UpperEstimationLimitParameterName = "UpperEstimationLimit";
    private const string LowerEstimationLimitParameterName = "LowerEstimationLimit";
    private const string AlphaParameterName = "Alpha";
    private const string BetaParameterName = "Beta";
    private const string BestSolutionParameterName = "Best solution (validation)";
    private const string BestSolutionQualityParameterName = "Best solution quality (validation)";
    private const string CurrentBestValidationQualityParameterName = "Current best validation quality";
    private const string ResultsParameterName = "Results";
    private const string BestKnownQualityParameterName = "BestKnownQuality";

    public ScopeTreeLookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ScopeTreeLookupParameter<SymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters[QualityParameterName]; }
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
    public IValueLookupParameter<IntValue> ValidationSamplesStartParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[ValidationSamplesStartParameterName]; }
    }
    public IValueLookupParameter<IntValue> ValidationSamplesEndParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[ValidationSamplesEndParameterName]; }
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
    public ILookupParameter<SymbolicRegressionSolution> BestSolutionParameter {
      get { return (ILookupParameter<SymbolicRegressionSolution>)Parameters[BestSolutionParameterName]; }
    }
    public ILookupParameter<DoubleValue> BestSolutionQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[BestSolutionQualityParameterName]; }
    }
    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters[ResultsParameterName]; }
    }
    public ILookupParameter<DoubleValue> BestKnownQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[BestKnownQualityParameterName]; }
    }

    [Storable]
    private UniformSubScopesProcessor subScopesProcessor;
    [Storable]
    private SymbolicRegressionSolutionLinearScaler linearScaler;
    [Storable]
    private SymbolicRegressionModelQualityAnalyzer modelQualityAnalyzer;
    [Storable]
    private SymbolicRegressionMeanSquaredErrorEvaluator validationMseEvaluator;
    [Storable]
    private BestSymbolicRegressionSolutionAnalyzer bestSolutionAnalyzer;
    [Storable]
    private UniformSubScopesProcessor cleaningSubScopesProcessor;
    [Storable]
    private Assigner removeScaledExpressionTreeAssigner;
    [Storable]
    private BestQualityMemorizer bestKnownQualityMemorizer;
    [Storable]
    private BestAverageWorstQualityCalculator bestAvgWorstValidationQualityCalculator;
    [Storable]
    private DataTableValuesCollector validationValuesCollector;
    [Storable]
    private ResultsCollector resultsCollector;

    [StorableConstructor]
    private ValidationBestScaledSymbolicRegressionSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    private ValidationBestScaledSymbolicRegressionSolutionAnalyzer(ValidationBestScaledSymbolicRegressionSolutionAnalyzer original, Cloner cloner)
      : base(original, cloner) {
      Initialize();
    }
    public ValidationBestScaledSymbolicRegressionSolutionAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees to analyze."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>(QualityParameterName, "The quality of the symbolic expression trees to analyze."));
      Parameters.Add(new ValueLookupParameter<ISymbolicExpressionTreeInterpreter>(SymbolicExpressionTreeInterpreterParameterName, "The interpreter that should be used for the analysis of symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<DataAnalysisProblemData>(ProblemDataParameterName, "The problem data for which the symbolic expression tree is a solution."));
      Parameters.Add(new ValueLookupParameter<IntValue>(TrainingSamplesStartParameterName, "The first index of the training partition of the data set."));
      Parameters.Add(new ValueLookupParameter<IntValue>(TrainingSamplesEndParameterName, "The last index of the training partition of the data set."));
      Parameters.Add(new ValueLookupParameter<IntValue>(ValidationSamplesStartParameterName, "The first index of the validation partition of the data set."));
      Parameters.Add(new ValueLookupParameter<IntValue>(ValidationSamplesEndParameterName, "The last index of the validation partition of the data set."));
      Parameters.Add(new ValueLookupParameter<IntValue>(TestSamplesStartParameterName, "The first index of the test partition of the data set."));
      Parameters.Add(new ValueLookupParameter<IntValue>(TestSamplesEndParameterName, "The last index of the test partition of the data set."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(UpperEstimationLimitParameterName, "The upper estimation limit that was set for the evaluation of the symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(LowerEstimationLimitParameterName, "The lower estimation limit that was set for the evaluation of the symbolic expression trees."));
      Parameters.Add(new LookupParameter<SymbolicRegressionSolution>(BestSolutionParameterName, "The best symbolic regression solution."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestSolutionQualityParameterName, "The quality of the best symbolic regression solution."));
      Parameters.Add(new LookupParameter<ResultCollection>(ResultsParameterName, "The result collection where the best symbolic regression solution should be stored."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestKnownQualityParameterName, "The best known (validation) quality achieved on the data set."));

      #region operator initialization
      subScopesProcessor = new UniformSubScopesProcessor();
      linearScaler = new SymbolicRegressionSolutionLinearScaler();
      modelQualityAnalyzer = new SymbolicRegressionModelQualityAnalyzer();
      validationMseEvaluator = new SymbolicRegressionMeanSquaredErrorEvaluator();
      bestSolutionAnalyzer = new BestSymbolicRegressionSolutionAnalyzer();
      cleaningSubScopesProcessor = new UniformSubScopesProcessor();
      removeScaledExpressionTreeAssigner = new Assigner();
      bestKnownQualityMemorizer = new BestQualityMemorizer();
      bestAvgWorstValidationQualityCalculator = new BestAverageWorstQualityCalculator();
      validationValuesCollector = new DataTableValuesCollector();
      resultsCollector = new ResultsCollector();
      #endregion

      #region parameter wiring
      subScopesProcessor.Depth.Value = SymbolicExpressionTreeParameter.Depth;

      linearScaler.AlphaParameter.ActualName = AlphaParameterName;
      linearScaler.BetaParameter.ActualName = BetaParameterName;
      linearScaler.SymbolicExpressionTreeParameter.ActualName = SymbolicExpressionTreeParameter.Name;
      linearScaler.ScaledSymbolicExpressionTreeParameter.ActualName = ScaledSymbolicExpressionTreeParameterName;

      modelQualityAnalyzer.ProblemDataParameter.ActualName = ProblemDataParameter.Name;
      modelQualityAnalyzer.SymbolicExpressionTreeParameter.ActualName = ScaledSymbolicExpressionTreeParameterName;
      modelQualityAnalyzer.SymbolicExpressionTreeParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      modelQualityAnalyzer.UpperEstimationLimitParameter.ActualName = UpperEstimationLimitParameter.Name;
      modelQualityAnalyzer.LowerEstimationLimitParameter.ActualName = LowerEstimationLimitParameter.Name;
      modelQualityAnalyzer.SymbolicExpressionTreeInterpreterParameter.ActualName = SymbolicExpressionTreeInterpreterParameter.Name;

      validationMseEvaluator.LowerEstimationLimitParameter.ActualName = LowerEstimationLimitParameter.Name;
      validationMseEvaluator.UpperEstimationLimitParameter.ActualName = UpperEstimationLimitParameter.Name;
      validationMseEvaluator.SymbolicExpressionTreeParameter.ActualName = ScaledSymbolicExpressionTreeParameterName;
      validationMseEvaluator.SymbolicExpressionTreeInterpreterParameter.ActualName = SymbolicExpressionTreeInterpreterParameter.Name;
      validationMseEvaluator.QualityParameter.ActualName = ScaledQualityParameterName;
      validationMseEvaluator.RegressionProblemDataParameter.ActualName = ProblemDataParameter.Name;
      validationMseEvaluator.SamplesStartParameter.ActualName = ValidationSamplesStartParameter.Name;
      validationMseEvaluator.SamplesEndParameter.ActualName = ValidationSamplesEndParameter.Name;

      bestSolutionAnalyzer.BestSolutionParameter.ActualName = BestSolutionParameter.Name;
      bestSolutionAnalyzer.BestSolutionQualityParameter.ActualName = BestSolutionQualityParameter.Name;
      bestSolutionAnalyzer.LowerEstimationLimitParameter.ActualName = LowerEstimationLimitParameter.Name;
      bestSolutionAnalyzer.ProblemDataParameter.ActualName = ProblemDataParameter.Name;
      bestSolutionAnalyzer.QualityParameter.ActualName = ScaledQualityParameterName;
      bestSolutionAnalyzer.ResultsParameter.ActualName = ResultsParameter.Name;
      bestSolutionAnalyzer.SymbolicExpressionTreeInterpreterParameter.ActualName = SymbolicExpressionTreeInterpreterParameter.Name;
      bestSolutionAnalyzer.SymbolicExpressionTreeParameter.ActualName = ScaledSymbolicExpressionTreeParameterName;
      bestSolutionAnalyzer.SymbolicExpressionTreeParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      bestSolutionAnalyzer.UpperEstimationLimitParameter.ActualName = UpperEstimationLimitParameter.Name;

      cleaningSubScopesProcessor.Depth.Value = SymbolicExpressionTreeParameter.Depth;

      removeScaledExpressionTreeAssigner.LeftSideParameter.ActualName = ScaledSymbolicExpressionTreeParameterName;
      removeScaledExpressionTreeAssigner.RightSideParameter.Value = new SymbolicExpressionTree();

      bestAvgWorstValidationQualityCalculator.AverageQualityParameter.ActualName = "Current average validation quality";
      bestAvgWorstValidationQualityCalculator.BestQualityParameter.ActualName = CurrentBestValidationQualityParameterName;
      bestAvgWorstValidationQualityCalculator.MaximizationParameter.Value = new BoolValue(false);
      bestAvgWorstValidationQualityCalculator.QualityParameter.ActualName = ScaledQualityParameterName;
      bestAvgWorstValidationQualityCalculator.QualityParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      bestAvgWorstValidationQualityCalculator.WorstQualityParameter.ActualName = "Current worst validation quality";

      bestKnownQualityMemorizer.BestQualityParameter.ActualName = BestKnownQualityParameterName;
      bestKnownQualityMemorizer.MaximizationParameter.Value = new BoolValue(false);
      bestKnownQualityMemorizer.QualityParameter.ActualName = QualityParameter.Name;
      bestKnownQualityMemorizer.QualityParameter.Depth = QualityParameter.Depth;

      validationValuesCollector.DataTableParameter.ActualName = "Validation quality";
      validationValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>(CurrentBestValidationQualityParameterName, null, CurrentBestValidationQualityParameterName));
      validationValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>(BestSolutionQualityParameter.Name, null, BestSolutionQualityParameter.Name));

      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>(CurrentBestValidationQualityParameterName, null, CurrentBestValidationQualityParameterName));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>(BestSolutionQualityParameter.Name, null, BestSolutionQualityParameter.Name));
      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>("Validation quality"));
      resultsCollector.ResultsParameter.ActualName = ResultsParameter.Name;
      #endregion

      #region operator graph
      OperatorGraph.InitialOperator = subScopesProcessor;
      subScopesProcessor.Operator = linearScaler;
      linearScaler.Successor = validationMseEvaluator;
      validationMseEvaluator.Successor = null;
      subScopesProcessor.Successor = modelQualityAnalyzer;
      modelQualityAnalyzer.Successor = bestSolutionAnalyzer;
      bestSolutionAnalyzer.Successor = cleaningSubScopesProcessor;
      cleaningSubScopesProcessor.Operator = removeScaledExpressionTreeAssigner;
      cleaningSubScopesProcessor.Successor = bestAvgWorstValidationQualityCalculator;
      bestAvgWorstValidationQualityCalculator.Successor = bestKnownQualityMemorizer;
      bestKnownQualityMemorizer.Successor = validationValuesCollector;
      validationValuesCollector.Successor = resultsCollector;
      resultsCollector.Successor = null;
      #endregion

      Initialize();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ValidationBestScaledSymbolicRegressionSolutionAnalyzer(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }
    private void Initialize() {
      SymbolicExpressionTreeParameter.DepthChanged += new EventHandler(SymbolicExpressionTreeParameter_DepthChanged);
    }


    private void SymbolicExpressionTreeParameter_DepthChanged(object sender, EventArgs e) {
      subScopesProcessor.Depth.Value = SymbolicExpressionTreeParameter.Depth;
      cleaningSubScopesProcessor.Depth.Value = SymbolicExpressionTreeParameter.Depth;
      bestSolutionAnalyzer.SymbolicExpressionTreeParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      bestSolutionAnalyzer.QualityParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      bestAvgWorstValidationQualityCalculator.QualityParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      bestKnownQualityMemorizer.QualityParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      modelQualityAnalyzer.SymbolicExpressionTreeParameter.Depth = SymbolicExpressionTreeParameter.Depth;
    }
  }
}
