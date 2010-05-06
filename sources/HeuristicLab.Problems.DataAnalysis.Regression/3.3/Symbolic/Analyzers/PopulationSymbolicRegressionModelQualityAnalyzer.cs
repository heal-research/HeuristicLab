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
    private const string TrainingValuesParameterName = "TrainingValues";
    private const string TrainingRSQuaredParameterName = "TrainingRSquared";
    private const string MinRSQuaredParameterName = "MinRSquared";
    private const string AvgRSQuaredParameterName = "AvgRSquared";
    private const string MaxRSQuaredParameterName = "MaxRSquared";
    private const string TrainingRSquaredValuesParameterName = "TrainingRSquaredValues";
    private const string SamplesStartParameterName = "SamplesStart";
    private const string SamplesEndParameterName = "SamplesEnd";
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
    #endregion

    public PopulationSymbolicRegressionModelQualityAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<ISymbolicExpressionTreeInterpreter>(SymbolicExpressionTreeInterpreterParameterName, "The interpreter that should be used to calculate the output values of the symbolic expression tree."));
      Parameters.Add(new ScopeTreeLookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees to analyze."));
      Parameters.Add(new LookupParameter<DataAnalysisProblemData>(ProblemDataParameterName, "The problem data containing the input varaibles for the symbolic regression problem."));
      Parameters.Add(new LookupParameter<ResultCollection>(ResultsParameterName, "The result collection where the best symbolic regression solution should be stored."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesStartParameterName, "The first index of the data set partition on which the model quality values should be calculated."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesEndParameterName, "The first index of the data set partition on which the model quality values should be calculated."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(UpperEstimationLimitParameterName, "The upper limit that should be used as cut off value for the output values of symbolic expression trees."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(LowerEstimationLimitParameterName, "The lower limit that should be used as cut off value for the output values of symbolic expression trees."));

      #region operator initialization
      // should be extended to calculate MSE, rel. Error and R² on the training (validation) and test set
      UniformSubScopesProcessor subScopesProcessor = new UniformSubScopesProcessor();
      SimpleSymbolicRegressionEvaluator simpleEvaluator = new SimpleSymbolicRegressionEvaluator();
      SimpleRSquaredEvaluator simpleR2Evalator = new SimpleRSquaredEvaluator();
      MinAverageMaxValueAnalyzer minAvgMaxAnalyzer = new MinAverageMaxValueAnalyzer();
      #endregion

      #region parameter wiring
      simpleEvaluator.SymbolicExpressionTreeParameter.ActualName = SymbolicExpressionTreeParameter.Name;
      simpleEvaluator.RegressionProblemDataParameter.ActualName = ProblemDataParameter.Name;
      simpleEvaluator.SamplesStartParameter.ActualName = SamplesStartParameter.Name;
      simpleEvaluator.SamplesEndParameter.ActualName = SamplesEndParameter.Name;
      simpleEvaluator.LowerEstimationLimitParameter.ActualName = LowerEstimationLimitParameter.Name;
      simpleEvaluator.UpperEstimationLimitParameter.ActualName = UpperEstimationLimitParameter.Name;
      simpleEvaluator.SymbolicExpressionTreeInterpreterParameter.ActualName = SymbolicExpressionTreeInterpreterParameter.Name;
      simpleEvaluator.ValuesParameter.ActualName = TrainingValuesParameterName;
      simpleR2Evalator.ValuesParameter.ActualName = TrainingValuesParameterName;
      simpleR2Evalator.RSquaredParameter.ActualName = TrainingRSQuaredParameterName;
      minAvgMaxAnalyzer.ValueParameter.ActualName = TrainingRSQuaredParameterName;
      minAvgMaxAnalyzer.AverageValueParameter.ActualName = AvgRSQuaredParameterName;
      minAvgMaxAnalyzer.MaxValueParameter.ActualName = MaxRSQuaredParameterName;
      minAvgMaxAnalyzer.MinValueParameter.ActualName = MinRSQuaredParameterName;
      minAvgMaxAnalyzer.ResultsParameter.ActualName = ResultsParameter.Name;
      minAvgMaxAnalyzer.ValuesParameter.ActualName = TrainingRSquaredValuesParameterName;
      #endregion

      #region operator graph
      OperatorGraph.InitialOperator = subScopesProcessor;
      subScopesProcessor.Operator = simpleEvaluator;
      simpleEvaluator.Successor = simpleR2Evalator;
      simpleR2Evalator.Successor = null;
      subScopesProcessor.Successor = minAvgMaxAnalyzer;
      minAvgMaxAnalyzer.Successor = null;
      #endregion

    }
  }
}
