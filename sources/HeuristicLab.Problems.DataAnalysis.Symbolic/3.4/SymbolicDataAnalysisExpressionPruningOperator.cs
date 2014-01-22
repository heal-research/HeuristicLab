#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableClass]
  [Item("SymbolicExpressionTreePruningOperator", "An operator that replaces introns with constant values in a symbolic expression tree.")]
  public class SymbolicDataAnalysisExpressionPruningOperator : SingleSuccessorOperator {
    private const string NumberOfPrunedSubtreesParameterName = "PrunedSubtrees";
    private const string NumberOfPrunedTreesParameterName = "PrunedTrees";
    #region parameter properties
    public ILookupParameter<DoubleValue> NumberOfPrunedSubtreesParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[NumberOfPrunedSubtreesParameterName]; }
    }
    public ILookupParameter<DoubleValue> NumberOfPrunedTreesParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[NumberOfPrunedTreesParameterName]; }
    }
    #endregion
    #region properties
    private DoubleValue PrunedSubtrees { get { return NumberOfPrunedSubtreesParameter.ActualValue; } }
    private DoubleValue PrunedTrees { get { return NumberOfPrunedTreesParameter.ActualValue; } }
    #endregion
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicDataAnalysisExpressionPruningOperator(this, cloner);
    }
    private SymbolicDataAnalysisExpressionPruningOperator(SymbolicDataAnalysisExpressionPruningOperator original, Cloner cloner)
      : base(original, cloner) {
    }

    public SymbolicDataAnalysisExpressionPruningOperator() {
      Parameters.Add(new LookupParameter<DoubleValue>(NumberOfPrunedSubtreesParameterName));
      Parameters.Add(new LookupParameter<DoubleValue>(NumberOfPrunedTreesParameterName));
    }

    public ISymbolicDataAnalysisModel Model { get; set; }
    public IDataAnalysisProblemData ProblemData { get; set; }
    public ISymbolicDataAnalysisSolutionImpactValuesCalculator ImpactsCalculator { get; set; }
    public IRandom Random { get; set; }

    public bool PruneOnlyZeroImpactNodes { get; set; }
    public double NodeImpactThreshold { get; set; }

    public override IOperation Apply() {
      int prunedSubtrees = 0;

      var nodes = Model.SymbolicExpressionTree.Root.GetSubtree(0).GetSubtree(0).IterateNodesPrefix().ToList();

      for (int j = 0; j < nodes.Count; ++j) {
        var node = nodes[j];
        if (node is ConstantTreeNode) continue;

        var impact = ImpactsCalculator.CalculateImpactValue(Model, node, ProblemData, ProblemData.TrainingIndices);

        if (PruneOnlyZeroImpactNodes) {
          if (!impact.IsAlmost(0.0)) continue;
        } else {
          if (NodeImpactThreshold < impact) continue;
        }

        var replacementValue = ImpactsCalculator.CalculateReplacementValue(Model, node, ProblemData, ProblemData.TrainingIndices);
        var constantNode = new ConstantTreeNode(new Constant()) { Value = replacementValue };
        ReplaceWithConstant(node, constantNode);
        j += node.GetLength() - 1; // skip subtrees under the node that was folded

        prunedSubtrees++;
      }

      if (prunedSubtrees > 0) {
        lock (PrunedSubtrees) { PrunedSubtrees.Value += prunedSubtrees; }
        lock (PrunedTrees) { PrunedTrees.Value += 1; }
      }
      return base.Apply();
    }
    private static void ReplaceWithConstant(ISymbolicExpressionTreeNode original, ISymbolicExpressionTreeNode replacement) {
      var parent = original.Parent;
      var i = parent.IndexOfSubtree(original);
      parent.RemoveSubtree(i);
      parent.InsertSubtree(i, replacement);
    }
  }
}
