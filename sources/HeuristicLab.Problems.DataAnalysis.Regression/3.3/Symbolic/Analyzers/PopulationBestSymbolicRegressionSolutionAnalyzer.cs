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
  [Item("PopulationBestSymbolicRegressionSolutionAnalyzer", "An operator for analyzing the best solution of symbolic regression problems given in symbolic expression tree encoding.")]
  [StorableClass]
  public sealed class PopulationBestSymbolicRegressionSolutionAnalyzer : SingleSuccessorOperator, ISymbolicRegressionSolutionPopulationAnalyzer {
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string SymbolicExpressionTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string ProblemDataParameterName = "ProblemData";
    private const string QualityParameterName = "Quality";
    private const string UpperEstimationLimitParameterName = "UpperEstimationLimit";
    private const string LowerEstimationLimitParameterName = "LowerEstimationLimit";
    private const string BestSolutionParameterName = "BestSolution";
    private const string BestSolutionQualityParameterName = "BestSolutionQuality";
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
    public ILookupParameter<ItemArray<DoubleValue>> QualityParameter {
      get { return (ILookupParameter<ItemArray<DoubleValue>>)Parameters[QualityParameterName]; }
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

    public PopulationBestSymbolicRegressionSolutionAnalyzer()
      : base() {
      Parameters.Add(new SubScopesLookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees to analyze."));
      Parameters.Add(new LookupParameter<ISymbolicExpressionTreeInterpreter>(SymbolicExpressionTreeInterpreterParameterName, "The interpreter that should be used for the analysis of symbolic expression trees."));
      Parameters.Add(new LookupParameter<DataAnalysisProblemData>(ProblemDataParameterName, "The problem data for which the symbolic expression tree is a solution."));
      Parameters.Add(new LookupParameter<DoubleValue>(UpperEstimationLimitParameterName, "The upper estimation limit that was set for the evaluation of the symbolic expression trees."));
      Parameters.Add(new LookupParameter<DoubleValue>(LowerEstimationLimitParameterName, "The lower estimation limit that was set for the evaluation of the symbolic expression trees."));
      Parameters.Add(new SubScopesLookupParameter<DoubleValue>(QualityParameterName, "The qualities of the symbolic regression trees which should be analyzed."));
      Parameters.Add(new LookupParameter<SymbolicRegressionSolution>(BestSolutionParameterName, "The best symbolic regression solution."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestSolutionQualityParameterName, "The quality of the best symbolic regression solution."));
      Parameters.Add(new LookupParameter<ResultCollection>(ResultsParameterName, "The result collection where the best symbolic regression solution should be stored."));
    }

    public override IOperation Apply() {
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      ResultCollection results = ResultsParameter.ActualValue;
      ISymbolicExpressionTreeInterpreter interpreter = SymbolicExpressionTreeInterpreterParameter.ActualValue;
      ItemArray<SymbolicExpressionTree> expressions = SymbolicExpressionTreeParameter.ActualValue;
      DataAnalysisProblemData problemData = ProblemDataParameter.ActualValue;
      DoubleValue upperEstimationLimit = UpperEstimationLimitParameter.ActualValue;
      DoubleValue lowerEstimationLimit = LowerEstimationLimitParameter.ActualValue;

      int i = qualities.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;

      SymbolicRegressionSolution solution = BestSolutionParameter.ActualValue;
      if (solution == null) {
        var model = new SymbolicRegressionModel(interpreter, expressions[i], GetInputVariables(expressions[i]));
        solution = new SymbolicRegressionSolution(problemData, model, lowerEstimationLimit.Value, upperEstimationLimit.Value);
        BestSolutionParameter.ActualValue = solution;
        BestSolutionQualityParameter.ActualValue = qualities[i];
        results.Add(new Result("Best Symbolic Regression Solution", solution));
      } else {
        if (BestSolutionQualityParameter.ActualValue.Value > qualities[i].Value) {
          var model = new SymbolicRegressionModel(interpreter, expressions[i], GetInputVariables(expressions[i]));
          solution = new SymbolicRegressionSolution(problemData, model, lowerEstimationLimit.Value, upperEstimationLimit.Value);
          BestSolutionParameter.ActualValue = solution;
          BestSolutionQualityParameter.ActualValue = qualities[i];
          results["Best Symbolic Regression Solution"].Value = solution;
        }
      }

      return base.Apply();
    }

    private IEnumerable<string> GetInputVariables(SymbolicExpressionTree tree) {
      return (from varNode in tree.IterateNodesPrefix().OfType<VariableTreeNode>()
              select varNode.VariableName).Distinct();
    }
  }
}
