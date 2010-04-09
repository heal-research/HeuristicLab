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

using System;
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.GeneralSymbols;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.ArchitectureAlteringOperators {
  /// <summary>
  /// Manipulates a symbolic expression by deleting a preexisting function-defining branch.
  /// As described in Koza, Bennett, Andre, Keane, Genetic Programming III - Darwinian Invention and Problem Solving, 1999, pp. 108
  /// </summary>
  [Item("SubroutineDeleter", "Manipulates a symbolic expression by deleting a preexisting function-defining branch.")]
  [StorableClass]
  public sealed class SubroutineDeleter : SymbolicExpressionTreeArchitectureAlteringOperator {
    private const int MAX_TRIES = 100;
    public override sealed void ModifyArchitecture(
      IRandom random,
      SymbolicExpressionTree symbolicExpressionTree,
      ISymbolicExpressionGrammar grammar,
      IntValue maxTreeSize, IntValue maxTreeHeight,
      IntValue maxFunctionDefiningBranches, IntValue maxFunctionArguments,
      out bool success) {
      success = DeleteSubroutine(random, symbolicExpressionTree, grammar, maxTreeSize.Value, maxTreeHeight.Value, maxFunctionDefiningBranches.Value, maxFunctionArguments.Value);
    }

    public static bool DeleteSubroutine(
      IRandom random,
      SymbolicExpressionTree symbolicExpressionTree,
      ISymbolicExpressionGrammar grammar,
      int maxTreeSize, int maxTreeHeight,
      int maxFunctionDefiningBranches, int maxFunctionArguments) {
      var functionDefiningBranches = symbolicExpressionTree.IterateNodesPrefix().OfType<DefunTreeNode>();

      if (functionDefiningBranches.Count() == 0)
        // no ADF to delete => abort
        return false;
      var selectedDefunBranch = (DefunTreeNode)SelectRandomBranch(random, functionDefiningBranches);
      // remove the selected defun
      int defunSubtreeIndex = symbolicExpressionTree.Root.SubTrees.IndexOf(selectedDefunBranch);
      symbolicExpressionTree.Root.RemoveSubTree(defunSubtreeIndex);

      // get all cut points that contain an invokation of the deleted defun
      var invocationCutPoints = from node in symbolicExpressionTree.IterateNodesPrefix()
                                where node.SubTrees.Count > 0
                                from argIndex in Enumerable.Range(0, node.SubTrees.Count)
                                let subtree = node.SubTrees[argIndex] as InvokeFunctionTreeNode
                                where subtree != null
                                where subtree.InvokedFunctionName == selectedDefunBranch.Name
                                select new { Parent = node, ReplacedChildIndex = argIndex, ReplacedChild = subtree };
      // deletion by random regeneration
      foreach (var cutPoint in invocationCutPoints) {
        SymbolicExpressionTreeNode replacementTree = null;
        int targetSize = random.Next(cutPoint.ReplacedChild.GetSize());
        int targetHeight = cutPoint.ReplacedChild.GetHeight();
        int tries = 0;
        do {
          try {
            replacementTree = ProbabilisticTreeCreator.PTC2(random, grammar, cutPoint.Parent.Symbol, targetSize, targetHeight);
          }
          catch (ArgumentException) {
            // try different size
            targetSize = random.Next(cutPoint.ReplacedChild.GetSize());
            if (tries++ > MAX_TRIES) throw;
          }
        } while (replacementTree == null);
        cutPoint.Parent.RemoveSubTree(cutPoint.ReplacedChildIndex);
        cutPoint.Parent.InsertSubTree(cutPoint.ReplacedChildIndex, replacementTree);
      }
      // remove references to deleted function
      foreach (var subtree in symbolicExpressionTree.Root.SubTrees) {
        if (subtree.DynamicSymbols.Contains(selectedDefunBranch.Name)) {
          subtree.RemoveDynamicSymbol(selectedDefunBranch.Name);
        }
      }
      Debug.Assert(grammar.IsValidExpression(symbolicExpressionTree));
      return true;
    }

    private static SymbolicExpressionTreeNode SelectRandomBranch(IRandom random, IEnumerable<DefunTreeNode> branches) {
      var list = branches.ToList();
      return list[random.Next(list.Count)];
    }
  }
}
