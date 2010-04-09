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
  /// As described in Koza, Bennett, Andre, Keane, Genetic Programming III - Darwinian Invention and Problem Solving, 1999, pp. 112
  /// </summary>
  [Item("ArgumentDeleter", "Manipulates a symbolic expression by deleting an argument from an existing function defining branch.")]
  [StorableClass]
  public sealed class ArgumentDeleter : SymbolicExpressionTreeArchitectureAlteringOperator {
    public override sealed void ModifyArchitecture(
      IRandom random,
      SymbolicExpressionTree symbolicExpressionTree,
      ISymbolicExpressionGrammar grammar,
      IntValue maxTreeSize, IntValue maxTreeHeight,
      IntValue maxFunctionDefiningBranches, IntValue maxFunctionArguments,
      out bool success) {
      success = DeleteArgument(random, symbolicExpressionTree, grammar, maxTreeSize.Value, maxTreeHeight.Value, maxFunctionDefiningBranches.Value, maxFunctionArguments.Value);
    }

    public static bool DeleteArgument(
      IRandom random,
      SymbolicExpressionTree symbolicExpressionTree,
      ISymbolicExpressionGrammar grammar,
      int maxTreeSize, int maxTreeHeight,
      int maxFunctionDefiningBranches, int maxFunctionArguments) {

      var functionDefiningBranches = symbolicExpressionTree.IterateNodesPrefix().OfType<DefunTreeNode>();
      if (functionDefiningBranches.Count() == 0)
        // no function defining branch => abort
        return false;
      var selectedDefunBranch = (DefunTreeNode)SelectRandomBranch(random, functionDefiningBranches);
      if (selectedDefunBranch.NumberOfArguments <= 1)
        // argument deletion by consolidation is not possible => abort
        return false;
      var cutPoints = (from node in IterateNodesPrefix(selectedDefunBranch)
                       where node.SubTrees.Count > 0
                       from argNode in node.SubTrees.OfType<ArgumentTreeNode>()
                       select new { Parent = node, ReplacedChildIndex = node.SubTrees.IndexOf(argNode), ReplacedChild = argNode }).ToList();
      if (cutPoints.Count() == 0)
        // no cut points found => abort
        return false;
      var selectedCutPoint = cutPoints[random.Next(cutPoints.Count)];
      var removedArgument = selectedCutPoint.ReplacedChild.ArgumentIndex;
      var invocationNodes = from node in symbolicExpressionTree.IterateNodesPrefix().OfType<InvokeFunctionTreeNode>()
                            where node.InvokedFunctionName == selectedDefunBranch.Name
                            select node;
      foreach (var invokeNode in invocationNodes) {
        invokeNode.RemoveSubTree(selectedCutPoint.ReplacedChild.ArgumentIndex);
      }

      DeleteArgumentByConsolidation(random, selectedDefunBranch, removedArgument);

      selectedDefunBranch.RemoveDynamicSymbol("ARG" + removedArgument);
      // reduce arity in known functions of all root branches
      foreach (var subtree in symbolicExpressionTree.Root.SubTrees) {
        if (subtree.DynamicSymbols.Contains(selectedDefunBranch.Name)) {
          subtree.SetDynamicSymbolArgumentCount(selectedDefunBranch.Name, selectedDefunBranch.NumberOfArguments - 1);
        }
      }
      selectedDefunBranch.NumberOfArguments--;
      Debug.Assert(grammar.IsValidExpression(symbolicExpressionTree));
      return true;
    }

    private static void DeleteArgumentByConsolidation(IRandom random, DefunTreeNode branch, int removedArgumentIndex) {
      // replace references to the deleted argument with random references to existing arguments
      var possibleArgumentIndexes = (from node in IterateNodesPrefix(branch).OfType<ArgumentTreeNode>()
                                     where node.ArgumentIndex != removedArgumentIndex
                                     select node.ArgumentIndex).Distinct().ToList();
      var argNodes = from node in IterateNodesPrefix(branch).OfType<ArgumentTreeNode>()
                     where node.ArgumentIndex == removedArgumentIndex
                     select node;
      foreach (var argNode in argNodes) {
        var replacementArgument = possibleArgumentIndexes[random.Next(possibleArgumentIndexes.Count)];
        argNode.ArgumentIndex = replacementArgument;
      }
    }
    private static IEnumerable<SymbolicExpressionTreeNode> IterateNodesPrefix(SymbolicExpressionTreeNode tree) {
      yield return tree;
      foreach (var subTree in tree.SubTrees) {
        foreach (var node in IterateNodesPrefix(subTree)) {
          yield return node;
        }
      }
    }

    private static SymbolicExpressionTreeNode SelectRandomBranch(IRandom random, IEnumerable<DefunTreeNode> branches) {
      var list = branches.ToList();
      return list[random.Next(list.Count)];
    }
  }
}
