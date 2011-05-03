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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Creators;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System.Collections.Generic;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.ArchitectureManipulators {
  /// <summary>
  /// Manipulates a symbolic expression by deleting a preexisting function-defining branch.
  /// As described in Koza, Bennett, Andre, Keane, Genetic Programming III - Darwinian Invention and Problem Solving, 1999, pp. 108
  /// </summary>
  [Item("SubroutineDeleter", "Manipulates a symbolic expression by deleting a preexisting function-defining branch.")]
  [StorableClass]
  public sealed class SubroutineDeleter : SymbolicExpressionTreeArchitectureManipulator {
    [StorableConstructor]
    private SubroutineDeleter(bool deserializing) : base(deserializing) { }
    private SubroutineDeleter(SubroutineDeleter original, Cloner cloner) : base(original, cloner) { }
    public SubroutineDeleter() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SubroutineDeleter(this, cloner);
    }

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
      var selectedDefunBranch = functionDefiningBranches.SelectRandom(random);
      // remove the selected defun
      int defunSubtreeIndex = symbolicExpressionTree.Root.SubTrees.IndexOf(selectedDefunBranch);
      symbolicExpressionTree.Root.RemoveSubTree(defunSubtreeIndex);

      // remove references to deleted function
      foreach (var subtree in symbolicExpressionTree.Root.SubTrees.OfType<SymbolicExpressionTreeTopLevelNode>()) {
        var matchingInvokeSymbol = (from symb in subtree.Grammar.Symbols.OfType<InvokeFunction>()
                                    where symb.FunctionName == selectedDefunBranch.FunctionName
                                    select symb).SingleOrDefault();
        if (matchingInvokeSymbol != null) {
          subtree.Grammar.RemoveSymbol(matchingInvokeSymbol);
        }
      }

      DeletionByRandomRegeneration(random, symbolicExpressionTree, selectedDefunBranch);
      return true;
    }

    private static void DeletionByRandomRegeneration(IRandom random, SymbolicExpressionTree symbolicExpressionTree, DefunTreeNode selectedDefunBranch) {
      // find first invocation and replace it with a randomly generated tree
      // can't find all invocations in one step because once we replaced a top level invocation
      // the invocations below it are removed already
      var invocationCutPoint = (from node in symbolicExpressionTree.IterateNodesPrefix()
                                from subtree in node.SubTrees.OfType<InvokeFunctionTreeNode>()
                                where subtree.Symbol.FunctionName == selectedDefunBranch.FunctionName
                                select new { Parent = node, ReplacedChildIndex = node.SubTrees.IndexOf(subtree), ReplacedChild = subtree }).FirstOrDefault();
      while (invocationCutPoint != null) {
        // deletion by random regeneration
        SymbolicExpressionTreeNode replacementTree = null;
        var allowedSymbolsList = invocationCutPoint.Parent.GetAllowedSymbols(invocationCutPoint.ReplacedChildIndex).ToList();
        var weights = allowedSymbolsList.Select(s => s.InitialFrequency);
        var selectedSymbol = allowedSymbolsList.SelectRandom(weights, random);

        int minPossibleSize = invocationCutPoint.Parent.Grammar.GetMinExpressionLength(selectedSymbol);
        int maxSize = Math.Max(minPossibleSize, invocationCutPoint.ReplacedChild.GetSize());
        int minPossibleHeight = invocationCutPoint.Parent.Grammar.GetMinExpressionDepth(selectedSymbol);
        int maxHeight = Math.Max(minPossibleHeight, invocationCutPoint.ReplacedChild.GetHeight());
        replacementTree = selectedSymbol.CreateTreeNode();
        if (replacementTree.HasLocalParameters)
          replacementTree.ResetLocalParameters(random);
        invocationCutPoint.Parent.RemoveSubTree(invocationCutPoint.ReplacedChildIndex);
        invocationCutPoint.Parent.InsertSubTree(invocationCutPoint.ReplacedChildIndex, replacementTree);

        ProbabilisticTreeCreator.PTC2(random, replacementTree, maxSize, maxHeight, 0, 0);

        invocationCutPoint = (from node in symbolicExpressionTree.IterateNodesPrefix()
                              from subtree in node.SubTrees.OfType<InvokeFunctionTreeNode>()
                              where subtree.Symbol.FunctionName == selectedDefunBranch.FunctionName
                              select new { Parent = node, ReplacedChildIndex = node.SubTrees.IndexOf(subtree), ReplacedChild = subtree }).FirstOrDefault();
      }
    }
  }
}
