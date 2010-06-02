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
using HeuristicLab.Problems.DataAnalysis.Evaluators;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Analyzers {
  [Item("BestSymbolicRegressionSolutionAnalyzer", "An operator for analyzing the best solution of symbolic regression problems given in symbolic expression tree encoding.")]
  [StorableClass]
  public sealed class BestSymbolicRegressionSolutionAnalyzer : RegressionSolutionAnalyzer, ISymbolicRegressionAnalyzer {
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string SymbolicExpressionTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string BestSolutionInputvariableCountResultName = "Variables used by best solution";
    private const string BestSolutionParameterName = "BestSolution";

    #region parameter properties
    public ScopeTreeLookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ScopeTreeLookupParameter<SymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public IValueLookupParameter<ISymbolicExpressionTreeInterpreter> SymbolicExpressionTreeInterpreterParameter {
      get { return (IValueLookupParameter<ISymbolicExpressionTreeInterpreter>)Parameters[SymbolicExpressionTreeInterpreterParameterName]; }
    }
    public ILookupParameter<SymbolicRegressionSolution> BestSolutionParameter {
      get { return (ILookupParameter<SymbolicRegressionSolution>)Parameters[BestSolutionParameterName]; }
    }
    #endregion
    #region properties
    public ISymbolicExpressionTreeInterpreter SymbolicExpressionTreeInterpreter {
      get { return SymbolicExpressionTreeInterpreterParameter.ActualValue; }
    }
    public ItemArray<SymbolicExpressionTree> SymbolicExpressionTree {
      get { return SymbolicExpressionTreeParameter.ActualValue; }
    }
    #endregion

    public BestSymbolicRegressionSolutionAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees to analyze."));
      Parameters.Add(new ValueLookupParameter<ISymbolicExpressionTreeInterpreter>(SymbolicExpressionTreeInterpreterParameterName, "The interpreter that should be used for the analysis of symbolic expression trees."));
      Parameters.Add(new LookupParameter<SymbolicRegressionSolution>(BestSolutionParameterName, "The best symbolic regression solution."));
    }

    protected override DataAnalysisSolution UpdateBestSolution() {
      double upperEstimationLimit = UpperEstimationLimit != null ? UpperEstimationLimit.Value : double.PositiveInfinity;
      double lowerEstimationLimit = LowerEstimationLimit != null ? LowerEstimationLimit.Value : double.NegativeInfinity;

      int i = Quality.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;

      if (BestSolutionQualityParameter.ActualValue == null || BestSolutionQualityParameter.ActualValue.Value > Quality[i].Value) {
        var model = new SymbolicRegressionModel((ISymbolicExpressionTreeInterpreter)SymbolicExpressionTreeInterpreter.Clone(),
          SymbolicExpressionTree[i],
          GetInputVariables(SymbolicExpressionTree[i]));
        var solution = new SymbolicRegressionSolution(ProblemData, model, lowerEstimationLimit, upperEstimationLimit);

        BestSolutionParameter.ActualValue = solution;
        BestSolutionQualityParameter.ActualValue = Quality[i];

        if (Results.ContainsKey(BestSolutionInputvariableCountResultName)) {
          Results[BestSolutionInputvariableCountResultName].Value = new IntValue(model.InputVariables.Count());
        } else {
          Results.Add(new Result(BestSolutionInputvariableCountResultName, new IntValue(model.InputVariables.Count())));
        }
      }
      return BestSolutionParameter.ActualValue;
    }

    private IEnumerable<string> GetInputVariables(SymbolicExpressionTree tree) {
      return (from varNode in tree.IterateNodesPrefix().OfType<VariableTreeNode>()
              select varNode.VariableName).Distinct();
    }
  }
}
