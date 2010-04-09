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
  /// Creates a new argument within one function-defining branch of a symbolic expression tree.
  /// As described in Koza, Bennett, Andre, Keane, Genetic Programming III - Darwinian Invention and Problem Solving, 1999, pp. 106
  /// </summary>
  [Item("ArgumentCreater", "Manipulates a symbolic expression by creating a new argument within one function-defining branch.")]
  [StorableClass]
  public sealed class ArgumentCreater : SymbolicExpressionTreeArchitectureAlteringOperator {
    public override sealed void ModifyArchitecture(
      IRandom random,
      SymbolicExpressionTree symbolicExpressionTree,
      ISymbolicExpressionGrammar grammar,
      IntValue maxTreeSize, IntValue maxTreeHeight,
      IntValue maxFunctionDefiningBranches, IntValue maxFunctionArguments,
      out bool success) {
      success = CreateNewArgument(random, symbolicExpressionTree, grammar, maxTreeSize.Value, maxTreeHeight.Value, maxFunctionDefiningBranches.Value, maxFunctionArguments.Value);
    }

    public static bool CreateNewArgument(
      IRandom random,
      SymbolicExpressionTree symbolicExpressionTree,
      ISymbolicExpressionGrammar grammar,
      int maxTreeSize, int maxTreeHeight,
      int maxFunctionDefiningBranches, int maxFunctionArguments) {

      var functionDefiningBranches = symbolicExpressionTree.IterateNodesPrefix().OfType<DefunTreeNode>();

      var allowedArgumentIndexes = Enumerable.Range(0, maxFunctionArguments);

      if (functionDefiningBranches.Count() == 0)
        // no function defining branch found => abort
        return false;
      // select a random function defining branch
      var selectedDefunBranch = SelectRandomBranch(random, functionDefiningBranches);
      // select a random cut point in the function defining branch
      // the branch at the cut point is to be replaced by a new argument node
      var cutPoints = (from node in IterateNodesPrefix(selectedDefunBranch)
                       where node.SubTrees.Count > 0
                       from subtree in node.SubTrees
                       select new { Parent = node, ReplacedChildIndex = node.SubTrees.IndexOf(subtree), ReplacedChild = subtree }).ToList();

      if (cutPoints.Count() == 0)
        // no cut point found => abort;
        return false;
      var selectedCutPoint = cutPoints[random.Next(cutPoints.Count)];
      var existingArguments = from node in IterateNodesPrefix(selectedDefunBranch)
                              let argNode = node as ArgumentTreeNode
                              where argNode != null
                              select argNode;
      var newArgumentIndex = allowedArgumentIndexes.Except(existingArguments.Select(x => x.ArgumentIndex)).First();
      // replace the branch at the cut point with an argument node
      var newArgNode = MakeArgumentNode(newArgumentIndex);
      var replacedBranch = selectedCutPoint.ReplacedChild;
      selectedCutPoint.Parent.RemoveSubTree(selectedCutPoint.ReplacedChildIndex);
      selectedCutPoint.Parent.InsertSubTree(selectedCutPoint.ReplacedChildIndex, newArgNode);
      // find all invokations of the selected ADF and attach a cloned version of the originally cut out branch
      var invocationNodes = from node in symbolicExpressionTree.IterateNodesPrefix().OfType<InvokeFunctionTreeNode>()
                            where node.InvokedFunctionName == selectedDefunBranch.Name
                            select node;
      // append a new argument branch after preprocessing
      foreach (var invocationNode in invocationNodes) {
        var clonedBranch = (SymbolicExpressionTreeNode)replacedBranch.Clone();
        ReplaceArgumentsInBranch(clonedBranch, invocationNode.SubTrees);
        invocationNode.InsertSubTree(newArgumentIndex, clonedBranch);
      }
      // adapt the known functions informations
      selectedDefunBranch.NumberOfArguments++;
      selectedDefunBranch.AddDynamicSymbol("ARG" + newArgumentIndex, 0);
      foreach (var subtree in symbolicExpressionTree.Root.SubTrees) {
        if (subtree.DynamicSymbols.Contains(selectedDefunBranch.Name)) {
          subtree.SetDynamicSymbolArgumentCount(selectedDefunBranch.Name, selectedDefunBranch.NumberOfArguments);
        }
      }
      Debug.Assert(grammar.IsValidExpression(symbolicExpressionTree));
      return true;
    }

    private static void ReplaceArgumentsInBranch(SymbolicExpressionTreeNode branch, IList<SymbolicExpressionTreeNode> argumentTrees) {
      // check if any subtree is an argument node 
      for (int subtreeIndex = 0; subtreeIndex < branch.SubTrees.Count; subtreeIndex++) {
        var subtree = branch.SubTrees[subtreeIndex];
        var argNode = subtree as ArgumentTreeNode;
        if (argNode != null) {
          // replace argument nodes by a clone of the original subtree that provided the result for the argument node
          branch.SubTrees[subtreeIndex] = (SymbolicExpressionTreeNode)argumentTrees[argNode.ArgumentIndex].Clone();
        } else {
          // recursively replace arguments in all branches
          ReplaceArgumentsInBranch(subtree, argumentTrees);
        }
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

    private static T SelectRandomBranch<T>(IRandom random, IEnumerable<T> branches) {
      var list = branches.ToList();
      return list[random.Next(list.Count)];
    }

    private static SymbolicExpressionTreeNode MakeArgumentNode(int argIndex) {
      var node = (ArgumentTreeNode)(new Argument()).CreateTreeNode();
      node.ArgumentIndex = argIndex;
      return node;
    }
  }
}
