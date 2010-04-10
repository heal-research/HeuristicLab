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
  /// Manipulates a symbolic expression by adding one new function-defining branch containing
  /// a proportion of a preexisting branch and by creating a reference to the new branch.
  /// As described in Koza, Bennett, Andre, Keane, Genetic Programming III - Darwinian Invention and Problem Solving, 1999, pp. 97
  /// </summary>
  [Item("SubroutineCreater", "Manipulates a symbolic expression by adding one new function-defining branch containing a proportion of a preexisting branch and by creating a reference to the new branch.")]
  [StorableClass]
  public sealed class SubroutineCreater : SymbolicExpressionTreeArchitectureAlteringOperator {
    public override sealed void ModifyArchitecture(
      IRandom random,
      SymbolicExpressionTree symbolicExpressionTree,
      ISymbolicExpressionGrammar grammar,
      IntValue maxTreeSize, IntValue maxTreeHeight,
      IntValue maxFunctionDefiningBranches, IntValue maxFunctionArguments,
      out bool success) {
      success = CreateSubroutine(random, symbolicExpressionTree, grammar, maxTreeSize.Value, maxTreeHeight.Value, maxFunctionDefiningBranches.Value, maxFunctionArguments.Value);
    }

    public static bool CreateSubroutine(
      IRandom random,
      SymbolicExpressionTree symbolicExpressionTree,
      ISymbolicExpressionGrammar grammar,
      int maxTreeSize, int maxTreeHeight,
      int maxFunctionDefiningBranches, int maxFunctionArguments) {
      var functionDefiningBranches = symbolicExpressionTree.IterateNodesPrefix().OfType<DefunTreeNode>();
      if (functionDefiningBranches.Count() >= maxFunctionDefiningBranches)
        // allowed maximum number of ADF reached => abort
        return false;
      string formatString = new StringBuilder().Append('0', (int)Math.Log10(maxFunctionDefiningBranches) + 1).ToString(); // >= 100 functions => ###
      var allowedFunctionNames = from index in Enumerable.Range(0, maxFunctionDefiningBranches)
                                 select "ADF" + index.ToString(formatString);
      // find all body branches
      var bodies = from node in symbolicExpressionTree.IterateNodesPrefix()
                   where node.Symbol is Defun || node.Symbol is StartSymbol
                   select new { Tree = node, Size = node.GetSize() };
      var totalNumberOfBodyNodes = bodies.Select(x => x.Size).Sum();
      // select a random body
      int r = random.Next(totalNumberOfBodyNodes);
      int aggregatedNumberOfBodyNodes = 0;
      SymbolicExpressionTreeNode selectedBody = null;
      foreach (var body in bodies) {
        aggregatedNumberOfBodyNodes += body.Size;
        if (aggregatedNumberOfBodyNodes > r)
          selectedBody = body.Tree;
      }
      // sanity check
      if (selectedBody == null) throw new InvalidOperationException();
      // select a random node in the selected branch
      var allCutPoints = (from parent in IterateNodesPrefix(selectedBody)
                          from subtree in parent.SubTrees
                          select new { Parent = parent, ReplacedBranchIndex = parent.SubTrees.IndexOf(subtree), ReplacedBranch = subtree }).ToList();
      if (allCutPoints.Count == 0)
        // no cut points => abort
        return false;
      var selectedCutPoint = allCutPoints[random.Next(allCutPoints.Count)];
      // select random branches as argument cut-off points (replaced by argument terminal nodes in the function)
      List<SymbolicExpressionTreeNode> argumentBranches = SelectRandomArgumentBranches(selectedCutPoint.ReplacedBranch, random, 0.25, maxFunctionArguments);
      string functionName = allowedFunctionNames.Except(functionDefiningBranches.Select(x => x.Name)).First();
      SymbolicExpressionTreeNode functionBody = selectedCutPoint.ReplacedBranch;
      // disconnect the function body from the tree
      selectedCutPoint.Parent.RemoveSubTree(selectedCutPoint.ReplacedBranchIndex);
      // disconnect the argument branches from the function
      DisconnectBranches(functionBody, argumentBranches);
      // and insert a function invocation symbol instead
      var invokeNode = (InvokeFunctionTreeNode)(new InvokeFunction()).CreateTreeNode();
      invokeNode.InvokedFunctionName = functionName;
      selectedCutPoint.Parent.InsertSubTree(selectedCutPoint.ReplacedBranchIndex, invokeNode);
      foreach (var argumentBranch in argumentBranches)
        invokeNode.AddSubTree(argumentBranch);

      // insert a new function defining branch
      var defunNode = (DefunTreeNode)(new Defun()).CreateTreeNode();
      defunNode.Name = functionName;
      defunNode.AddSubTree(functionBody);
      symbolicExpressionTree.Root.AddSubTree(defunNode);
      // copy known symbols from originating branch into new branch
      foreach (var knownSymbol in selectedBody.DynamicSymbols) {
        defunNode.AddDynamicSymbol(knownSymbol, selectedBody.GetDynamicSymbolArgumentCount(knownSymbol));
      }
      // add function arguments as known symbols to new branch
      for (int i = 0; i < argumentBranches.Count; i++) {
        defunNode.AddDynamicSymbol("ARG" + i);
      }
      // add new function name to original branch
      selectedBody.AddDynamicSymbol(functionName, argumentBranches.Count);
      return true;
    }

    private static void DisconnectBranches(SymbolicExpressionTreeNode node, List<SymbolicExpressionTreeNode> argumentBranches) {
      // remove the subtrees so that we can clone only the root node
      List<SymbolicExpressionTreeNode> subtrees = new List<SymbolicExpressionTreeNode>(node.SubTrees);
      while (node.SubTrees.Count > 0) node.SubTrees.RemoveAt(0);
      // recursively apply function for subtrees or append a argument terminal node
      foreach (var subtree in subtrees) {
        if (argumentBranches.Contains(subtree)) {
          node.AddSubTree(MakeArgumentNode(argumentBranches.IndexOf(subtree)));
        } else {
          DisconnectBranches(subtree, argumentBranches);
          node.AddSubTree(subtree);
        }
      }
    }

    private static SymbolicExpressionTreeNode MakeArgumentNode(int argIndex) {
      var node = (ArgumentTreeNode)(new Argument()).CreateTreeNode();
      node.ArgumentIndex = argIndex;
      return node;
    }

    private static List<SymbolicExpressionTreeNode> SelectRandomArgumentBranches(SymbolicExpressionTreeNode selectedRoot,
      IRandom random,
      double argumentProbability,
      int maxArguments) {
      List<SymbolicExpressionTreeNode> argumentBranches = new List<SymbolicExpressionTreeNode>();
      foreach (var subTree in selectedRoot.SubTrees) {
        if (random.NextDouble() < argumentProbability) {
          if (argumentBranches.Count < maxArguments)
            argumentBranches.Add(subTree);
        } else {
          foreach (var argBranch in SelectRandomArgumentBranches(subTree, random, argumentProbability, maxArguments))
            if (argumentBranches.Count < maxArguments)
              argumentBranches.Add(argBranch);
        }
      }
      return argumentBranches;
    }

    private static IEnumerable<SymbolicExpressionTreeNode> IterateNodesPrefix(SymbolicExpressionTreeNode tree) {
      yield return tree;
      foreach (var subTree in tree.SubTrees) {
        foreach (var node in IterateNodesPrefix(subTree)) {
          yield return node;
        }
      }
    }

    private static IEnumerable<string> UsedFunctionNames(SymbolicExpressionTree symbolicExpressionTree) {
      return from node in symbolicExpressionTree.IterateNodesPrefix()
             where node.Symbol is Defun
             select ((DefunTreeNode)node).Name;
    }

    private static SymbolicExpressionTreeNode SelectRandomBranch(IRandom random, IEnumerable<DefunTreeNode> branches) {
      var list = branches.ToList();
      return list[random.Next(list.Count)];
    }
  }
}
