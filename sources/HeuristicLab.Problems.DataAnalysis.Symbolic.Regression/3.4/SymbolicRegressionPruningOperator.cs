#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [StorableClass]
  [Item("SymbolicRegressionPruningOperator", "An operator which prunes symbolic regression trees.")]
  public class SymbolicRegressionPruningOperator : SymbolicDataAnalysisExpressionPruningOperator {
    protected SymbolicRegressionPruningOperator(SymbolicRegressionPruningOperator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionPruningOperator(this, cloner);
    }

    [StorableConstructor]
    protected SymbolicRegressionPruningOperator(bool deserializing) : base(deserializing) { }

    public SymbolicRegressionPruningOperator(ISymbolicDataAnalysisSolutionImpactValuesCalculator impactValuesCalculator)
      : base(impactValuesCalculator) {
    }

    protected override ISymbolicDataAnalysisModel CreateModel(ISymbolicExpressionTree tree, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, IDataAnalysisProblemData problemData, DoubleLimit estimationLimits) {
      return new SymbolicRegressionModel(tree, interpreter, estimationLimits.Lower, estimationLimits.Upper);
    }

    protected override double Evaluate(IDataAnalysisModel model) {
      var regressionModel = (IRegressionModel)model;
      var regressionProblemData = (IRegressionProblemData)ProblemData;
      var rows = Enumerable.Range(FitnessCalculationPartition.Start, FitnessCalculationPartition.Size);
      return Evaluate(regressionModel, regressionProblemData, rows);
    }

    private static double Evaluate(IRegressionModel model, IRegressionProblemData problemData,
      IEnumerable<int> rows) {
      var estimatedValues = model.GetEstimatedValues(problemData.Dataset, rows); // also bounds the values
      var targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows);
      OnlineCalculatorError errorState;
      var quality = OnlinePearsonsRSquaredCalculator.Calculate(targetValues, estimatedValues, out errorState);
      if (errorState != OnlineCalculatorError.None) return double.NaN;
      return quality;
    }

    public static ISymbolicExpressionTree Prune(ISymbolicExpressionTree tree, SymbolicRegressionSolutionImpactValuesCalculator impactValuesCalculator, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, IRegressionProblemData problemData, DoubleLimit estimationLimits, IEnumerable<int> rows, double nodeImpactThreshold = 0.0, bool pruneOnlyZeroImpactNodes = false) {
      var clonedTree = (ISymbolicExpressionTree)tree.Clone();
      var model = new SymbolicRegressionModel(clonedTree, interpreter, estimationLimits.Lower, estimationLimits.Upper);
      var nodes = clonedTree.IterateNodesPrefix().ToList();
      double quality = Evaluate(model, problemData, rows);

      for (int i = 0; i < nodes.Count; ++i) {
        var node = nodes[i];
        if (node is ConstantTreeNode) continue;

        double impactValue, replacementValue;
        impactValuesCalculator.CalculateImpactAndReplacementValues(model, node, problemData, rows, out impactValue, out replacementValue, quality);

        if (pruneOnlyZeroImpactNodes) {
          if (!impactValue.IsAlmost(0.0)) continue;
        } else if (nodeImpactThreshold < impactValue) {
          continue;
        }

        var constantNode = (ConstantTreeNode)node.Grammar.GetSymbol("Constant").CreateTreeNode();
        constantNode.Value = replacementValue;

        ReplaceWithConstant(node, constantNode);
        i += node.GetLength() - 1; // skip subtrees under the node that was folded

        quality -= impactValue;
      }
      return model.SymbolicExpressionTree;
    }
  }
}
