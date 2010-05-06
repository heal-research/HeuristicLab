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
using System;
using HeuristicLab.Optimization.Operators;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Analyzers {
  /// <summary>
  /// An operator that analyzes the validation best scaled symbolic regression solution.
  /// </summary>
  [Item("ValidationBestScaledSymbolicRegressionSolutionAnalyzer", "An operator that analyzes the validation best scaled symbolic regression solution.")]
  [StorableClass]
  public sealed class ValidationBestScaledSymbolicRegressionSolutionAnalyzer : AlgorithmOperator, ISymbolicRegressionAnalyzer {
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string ScaledSymbolicExpressionTreeParameterName = "ScaledSymbolicExpressionTree";
    private const string SymbolicExpressionTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string ProblemDataParameterName = "ProblemData";
    private const string SamplesStartParameterName = "SamplesStart";
    private const string SamplesEndParameterName = "SamplesEnd";
    private const string QualityParameterName = "ScaledQuality";
    private const string UpperEstimationLimitParameterName = "UpperEstimationLimit";
    private const string LowerEstimationLimitParameterName = "LowerEstimationLimit";
    private const string AlphaParameterName = "Alpha";
    private const string BetaParameterName = "Beta";
    private const string BestSolutionParameterName = "ValidationBestSolution";
    private const string BestSolutionQualityParameterName = "ValidationBestSolutionQuality";
    private const string ResultsParameterName = "Results";

    public ScopeTreeLookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ScopeTreeLookupParameter<SymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public IValueLookupParameter<ISymbolicExpressionTreeInterpreter> SymbolicExpressionTreeInterpreterParameter {
      get { return (IValueLookupParameter<ISymbolicExpressionTreeInterpreter>)Parameters[SymbolicExpressionTreeInterpreterParameterName]; }
    }
    public IValueLookupParameter<DataAnalysisProblemData> ProblemDataParameter {
      get { return (IValueLookupParameter<DataAnalysisProblemData>)Parameters[ProblemDataParameterName]; }
    }
    public IValueLookupParameter<IntValue> SamplesStartParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[SamplesStartParameterName]; }
    }
    public IValueLookupParameter<IntValue> SamplesEndParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[SamplesEndParameterName]; }
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

    [Storable]
    private BestQualityMemorizer validationQualityMemorizer;
    [Storable]
    private BestSymbolicRegressionSolutionAnalyzer bestSolutionAnalyzer;
    [Storable]
    private SymbolicRegressionSolutionLinearScaler linearScaler;
    [Storable]
    private SymbolicRegressionMeanSquaredErrorCalculator validationMseCalculator;

    public ValidationBestScaledSymbolicRegressionSolutionAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees to analyze."));
      Parameters.Add(new ValueLookupParameter<ISymbolicExpressionTreeInterpreter>(SymbolicExpressionTreeInterpreterParameterName, "The interpreter that should be used for the analysis of symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<DataAnalysisProblemData>(ProblemDataParameterName, "The problem data for which the symbolic expression tree is a solution."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesStartParameterName, "The first index of the validation partition of the data set."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesEndParameterName, "The last index of the validation partition of the data set."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(UpperEstimationLimitParameterName, "The upper estimation limit that was set for the evaluation of the symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(LowerEstimationLimitParameterName, "The lower estimation limit that was set for the evaluation of the symbolic expression trees."));
      Parameters.Add(new LookupParameter<SymbolicRegressionSolution>(BestSolutionParameterName, "The best symbolic regression solution."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestSolutionQualityParameterName, "The quality of the best symbolic regression solution."));
      Parameters.Add(new LookupParameter<ResultCollection>(ResultsParameterName, "The result collection where the best symbolic regression solution should be stored."));

      #region operator initialization
      linearScaler = new SymbolicRegressionSolutionLinearScaler();
      validationMseCalculator = new SymbolicRegressionMeanSquaredErrorCalculator();
      bestSolutionAnalyzer = new BestSymbolicRegressionSolutionAnalyzer();
      validationQualityMemorizer = new BestQualityMemorizer();
      BestAverageWorstQualityCalculator bestAvgWorstValidationQualityCalculator = new BestAverageWorstQualityCalculator();
      DataTableValuesCollector validationValuesCollector = new DataTableValuesCollector();
      ResultsCollector resultsCollector = new ResultsCollector();
      #endregion

      #region parameter wiring
      linearScaler.AlphaParameter.ActualName = AlphaParameterName;
      linearScaler.AlphaParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      linearScaler.BetaParameter.ActualName = BetaParameterName;
      linearScaler.BetaParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      linearScaler.SymbolicExpressionTreeParameter.ActualName = SymbolicExpressionTreeParameter.Name;
      linearScaler.SymbolicExpressionTreeParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      linearScaler.ScaledSymbolicExpressionTreeParameter.ActualName = ScaledSymbolicExpressionTreeParameterName;
      linearScaler.ScaledSymbolicExpressionTreeParameter.Depth = SymbolicExpressionTreeParameter.Depth;

      validationMseCalculator.LowerEstimationLimitParameter.ActualName = LowerEstimationLimitParameter.Name;
      validationMseCalculator.UpperEstimationLimitParameter.ActualName = UpperEstimationLimitParameter.Name;
      validationMseCalculator.SymbolicExpressionTreeParameter.ActualName = ScaledSymbolicExpressionTreeParameterName;
      validationMseCalculator.SymbolicExpressionTreeParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      validationMseCalculator.SymbolicExpressionTreeInterpreterParameter.ActualName = SymbolicExpressionTreeInterpreterParameter.Name;
      validationMseCalculator.QualityParameter.ActualName = QualityParameterName;
      validationMseCalculator.ProblemDataParameter.ActualName = ProblemDataParameter.Name;
      validationMseCalculator.SamplesStartParameter.ActualName = SamplesStartParameter.Name;
      validationMseCalculator.SamplesEndParameter.ActualName = SamplesEndParameter.Name;

      bestSolutionAnalyzer.BestSolutionParameter.ActualName = BestSolutionParameter.Name;
      bestSolutionAnalyzer.BestSolutionQualityParameter.ActualName = BestSolutionQualityParameter.Name;
      bestSolutionAnalyzer.LowerEstimationLimitParameter.ActualName = LowerEstimationLimitParameter.Name;
      bestSolutionAnalyzer.ProblemDataParameter.ActualName = ProblemDataParameter.Name;
      bestSolutionAnalyzer.QualityParameter.ActualName = QualityParameterName;
      bestSolutionAnalyzer.ResultsParameter.ActualName = ResultsParameter.Name;
      bestSolutionAnalyzer.SymbolicExpressionTreeInterpreterParameter.ActualName = SymbolicExpressionTreeInterpreterParameter.Name;
      bestSolutionAnalyzer.SymbolicExpressionTreeParameter.ActualName = ScaledSymbolicExpressionTreeParameterName;
      bestSolutionAnalyzer.SymbolicExpressionTreeParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      bestSolutionAnalyzer.UpperEstimationLimitParameter.ActualName = UpperEstimationLimitParameter.Name;

      bestAvgWorstValidationQualityCalculator.AverageQualityParameter.ActualName = "Current Average Validation Quality";
      bestAvgWorstValidationQualityCalculator.BestQualityParameter.ActualName = "Current Best Validation Quality";
      bestAvgWorstValidationQualityCalculator.MaximizationParameter.Value = new BoolValue(false);
      bestAvgWorstValidationQualityCalculator.QualityParameter.ActualName = QualityParameterName;
      bestAvgWorstValidationQualityCalculator.WorstQualityParameter.ActualName = "Current Worst Validation Quality";

      validationQualityMemorizer.BestQualityParameter.ActualName = "Best Validation Quality";
      validationQualityMemorizer.QualityParameter.ActualName = QualityParameterName;
      validationQualityMemorizer.QualityParameter.Depth = SymbolicExpressionTreeParameter.Depth;

      validationValuesCollector.DataTableParameter.ActualName = "Validation Qualities";
      validationValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Best Validation Quality", null, "Current Best Validation Quality"));
      validationValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Validation Quality", null, "Best Validation Quality"));

      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Best Validation Quality", null, "Current Best Validation Quality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Validation Quality", null, "Best Validation Quality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>("Validation Qualities"));
      resultsCollector.ResultsParameter.ActualName = ResultsParameter.Name;
      #endregion

      #region operator graph
      OperatorGraph.InitialOperator = linearScaler;
      linearScaler.Successor = validationMseCalculator;
      validationMseCalculator.Successor = bestSolutionAnalyzer;
      bestSolutionAnalyzer.Successor = bestAvgWorstValidationQualityCalculator;
      bestAvgWorstValidationQualityCalculator.Successor = validationQualityMemorizer;
      validationQualityMemorizer.Successor = validationValuesCollector;
      validationValuesCollector.Successor = resultsCollector;
      resultsCollector.Successor = null;
      #endregion

      Initialize();
    }

    [StorableConstructor]
    private ValidationBestScaledSymbolicRegressionSolutionAnalyzer(bool deserializing) : base() { }

    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      SymbolicExpressionTreeParameter.DepthChanged += new EventHandler(SymbolicExpressionTreeParameter_DepthChanged);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      ValidationBestScaledSymbolicRegressionSolutionAnalyzer clone = (ValidationBestScaledSymbolicRegressionSolutionAnalyzer)base.Clone(cloner);
      clone.Initialize();
      return clone;
    }

    private void SymbolicExpressionTreeParameter_DepthChanged(object sender, EventArgs e) {
      validationMseCalculator.SymbolicExpressionTreeParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      validationQualityMemorizer.QualityParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      bestSolutionAnalyzer.SymbolicExpressionTreeParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      bestSolutionAnalyzer.QualityParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      linearScaler.AlphaParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      linearScaler.BetaParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      linearScaler.SymbolicExpressionTreeParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      linearScaler.ScaledSymbolicExpressionTreeParameter.Depth = SymbolicExpressionTreeParameter.Depth;
    }
  }
}
