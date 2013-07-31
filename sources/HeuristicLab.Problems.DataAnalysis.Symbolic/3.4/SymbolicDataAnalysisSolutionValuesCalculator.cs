#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  public abstract class SymbolicDataAnalysisSolutionValuesCalculator {
    protected readonly ISymbolicExpressionTree tempTree;
    protected readonly ConstantTreeNode constantNode;

    public SymbolicDataAnalysisSolutionValuesCalculator() {
      constantNode = ((ConstantTreeNode)new Constant().CreateTreeNode());
      ISymbolicExpressionTreeNode root = new ProgramRootSymbol().CreateTreeNode();
      ISymbolicExpressionTreeNode start = new StartSymbol().CreateTreeNode();
      root.AddSubtree(start);
      tempTree = new SymbolicExpressionTree(root);
    }

    // should be moved to an interface, then un-abstract the class
    public abstract Dictionary<ISymbolicExpressionTreeNode, double> CalculateReplacementValues(ISymbolicExpressionTree tree, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, IDataAnalysisProblemData problemData);
    public abstract Dictionary<ISymbolicExpressionTreeNode, double> CalculateImpactValues(ISymbolicExpressionTree tree, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, IDataAnalysisProblemData problemData, double lowerEstimationLimit, double upperEstimationLimit);

    protected void SwitchNode(ISymbolicExpressionTreeNode root, ISymbolicExpressionTreeNode oldBranch, ISymbolicExpressionTreeNode newBranch) {
      for (int i = 0; i < root.SubtreeCount; i++) {
        if (root.GetSubtree(i) == oldBranch) {
          root.RemoveSubtree(i);
          root.InsertSubtree(i, newBranch);
          return;
        }
      }
    }

    protected double CalculateReplacementValue(ISymbolicExpressionTreeNode node, ISymbolicExpressionTree sourceTree, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, IDataAnalysisProblemData problemData) {
      // remove old ADFs
      while (tempTree.Root.SubtreeCount > 1) tempTree.Root.RemoveSubtree(1);
      // clone ADFs of source tree
      for (int i = 1; i < sourceTree.Root.SubtreeCount; i++) {
        tempTree.Root.AddSubtree((ISymbolicExpressionTreeNode)sourceTree.Root.GetSubtree(i).Clone());
      }
      var start = tempTree.Root.GetSubtree(0);
      while (start.SubtreeCount > 0) start.RemoveSubtree(0);
      start.AddSubtree((ISymbolicExpressionTreeNode)node.Clone());
      var rows = problemData.TrainingIndices;
      return interpreter.GetSymbolicExpressionTreeValues(tempTree, problemData.Dataset, rows).Median();
    }
  }
}
