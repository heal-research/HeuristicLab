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

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Analyzers {
  /// <summary>
  /// An operator that analyzes the validation best scaled symbolic regression solution.
  /// </summary>
  [Item("PopulationValidationBestScaledSymbolicRegressionSolutionAnalyzer", "An operator that analyzes the validation best scaled symbolic regression solution.")]
  [StorableClass]
  public sealed class PopulationValidationBestScaledSymbolicRegressionSolutionAnalyzer : AlgorithmOperator, ISymbolicRegressionSolutionPopulationAnalyzer {
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

    public ILookupParameter<ItemArray<SymbolicExpressionTree>> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<ItemArray<SymbolicExpressionTree>>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public ILookupParameter<ISymbolicExpressionTreeInterpreter> SymbolicExpressionTreeInterpreterParameter {
      get { return (ILookupParameter<ISymbolicExpressionTreeInterpreter>)Parameters[SymbolicExpressionTreeInterpreterParameterName]; }
    }
    public ILookupParameter<DataAnalysisProblemData> ProblemDataParameter {
      get { return (ILookupParameter<DataAnalysisProblemData>)Parameters[ProblemDataParameterName]; }
    }
    public IValueLookupParameter<IntValue> SamplesStartParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[SamplesStartParameterName]; }
    }
    public IValueLookupParameter<IntValue> SamplesEndParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[SamplesEndParameterName]; }
    }
    public ILookupParameter<DoubleValue> UpperEstimationLimitParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[UpperEstimationLimitParameterName]; }
    }
    public ILookupParameter<DoubleValue> LowerEstimationLimitParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[LowerEstimationLimitParameterName]; }
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

    public PopulationValidationBestScaledSymbolicRegressionSolutionAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees to analyze."));
      Parameters.Add(new LookupParameter<ISymbolicExpressionTreeInterpreter>(SymbolicExpressionTreeInterpreterParameterName, "The interpreter that should be used for the analysis of symbolic expression trees."));
      Parameters.Add(new LookupParameter<DataAnalysisProblemData>(ProblemDataParameterName, "The problem data for which the symbolic expression tree is a solution."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesStartParameterName, "The first index of the validation partition of the data set."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesEndParameterName, "The last index of the validation partition of the data set."));
      Parameters.Add(new LookupParameter<DoubleValue>(UpperEstimationLimitParameterName, "The upper estimation limit that was set for the evaluation of the symbolic expression trees."));
      Parameters.Add(new LookupParameter<DoubleValue>(LowerEstimationLimitParameterName, "The lower estimation limit that was set for the evaluation of the symbolic expression trees."));
      Parameters.Add(new LookupParameter<SymbolicRegressionSolution>(BestSolutionParameterName, "The best symbolic regression solution."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestSolutionQualityParameterName, "The quality of the best symbolic regression solution."));
      Parameters.Add(new LookupParameter<ResultCollection>(ResultsParameterName, "The result collection where the best symbolic regression solution should be stored."));

      #region operator initialization
      UniformSubScopesProcessor subScopesProc = new UniformSubScopesProcessor();
      SymbolicRegressionSolutionLinearScaler linearScaler = new SymbolicRegressionSolutionLinearScaler();
      SymbolicRegressionMeanSquaredErrorEvaluator validationMseEvaluator = new SymbolicRegressionMeanSquaredErrorEvaluator();
      PopulationBestSymbolicRegressionSolutionAnalyzer bestSolutionAnalyzer = new PopulationBestSymbolicRegressionSolutionAnalyzer();
      #endregion

      #region parameter wiring
      linearScaler.AlphaParameter.ActualName = AlphaParameterName;
      linearScaler.BetaParameter.ActualName = BetaParameterName;
      linearScaler.SymbolicExpressionTreeParameter.ActualName = SymbolicExpressionTreeParameterName;
      linearScaler.ScaledSymbolicExpressionTreeParameter.ActualName = ScaledSymbolicExpressionTreeParameterName;

      validationMseEvaluator.LowerEstimationLimitParameter.ActualName = LowerEstimationLimitParameterName;
      validationMseEvaluator.UpperEstimationLimitParameter.ActualName = UpperEstimationLimitParameterName;
      validationMseEvaluator.SymbolicExpressionTreeParameter.ActualName = ScaledSymbolicExpressionTreeParameterName;
      validationMseEvaluator.SymbolicExpressionTreeInterpreterParameter.ActualName = SymbolicExpressionTreeInterpreterParameterName;
      validationMseEvaluator.QualityParameter.ActualName = QualityParameterName;
      validationMseEvaluator.RegressionProblemDataParameter.ActualName = ProblemDataParameterName;
      validationMseEvaluator.SamplesStartParameter.ActualName = SamplesStartParameterName;
      validationMseEvaluator.SamplesEndParameter.ActualName = SamplesEndParameterName;

      bestSolutionAnalyzer.BestSolutionParameter.ActualName = BestSolutionParameterName;
      bestSolutionAnalyzer.BestSolutionQualityParameter.ActualName = BestSolutionQualityParameterName;
      bestSolutionAnalyzer.LowerEstimationLimitParameter.ActualName = LowerEstimationLimitParameterName;
      bestSolutionAnalyzer.ProblemDataParameter.ActualName = ProblemDataParameterName;
      bestSolutionAnalyzer.QualityParameter.ActualName = QualityParameterName;
      bestSolutionAnalyzer.ResultsParameter.ActualName = ResultsParameterName;
      bestSolutionAnalyzer.SymbolicExpressionTreeInterpreterParameter.ActualName = SymbolicExpressionTreeInterpreterParameterName;
      bestSolutionAnalyzer.SymbolicExpressionTreeParameter.ActualName = ScaledSymbolicExpressionTreeParameterName;
      bestSolutionAnalyzer.UpperEstimationLimitParameter.ActualName = UpperEstimationLimitParameterName;
      #endregion

      #region operator graph
      OperatorGraph.InitialOperator = subScopesProc;
      subScopesProc.Operator = linearScaler;
      linearScaler.Successor = validationMseEvaluator;
      validationMseEvaluator.Successor = null;
      subScopesProc.Successor = bestSolutionAnalyzer;
      bestSolutionAnalyzer.Successor = null;
      #endregion
    }
  }
}
