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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.ArchitectureManipulators {
  /// <summary>
  /// Creates a new argument within one function-defining branch of a symbolic expression tree.
  /// As described in Koza, Bennett, Andre, Keane, Genetic Programming III - Darwinian Invention and Problem Solving, 1999, pp. 106
  /// </summary>
  [Item("ArgumentCreater", "Manipulates a symbolic expression by creating a new argument within one function-defining branch.")]
  [StorableClass]
  public sealed class ArgumentCreater : SymbolicExpressionTreeArchitectureManipulator {
    [StorableConstructor]
    private ArgumentCreater(bool deserializing) : base(deserializing) { }
    private ArgumentCreater(ArgumentCreater original, Cloner cloner) : base(original, cloner) { }
    public ArgumentCreater() : base() { }
    public override sealed void ModifyArchitecture(
      IRandom random,
      SymbolicExpressionTree symbolicExpressionTree,
      ISymbolicExpressionGrammar grammar,
      IntValue maxTreeSize, IntValue maxTreeHeight,
      IntValue maxFunctionDefiningBranches, IntValue maxFunctionArguments,
      out bool success) {
      success = CreateNewArgument(random, symbolicExpressionTree, grammar, maxTreeSize.Value, maxTreeHeight.Value, maxFunctionDefiningBranches.Value, maxFunctionArguments.Value);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ArgumentCreater(this, cloner);
    }

    public static bool CreateNewArgument(
      IRandom random,
      SymbolicExpressionTree symbolicExpressionTree,
      ISymbolicExpressionGrammar grammar,
      int maxTreeSize, int maxTreeHeight,
      int maxFunctionDefiningBranches, int maxFunctionArguments) {
      // work on a copy in case we find out later that the tree would be too big
      // in this case it's easiest to simply return the original tree.
      SymbolicExpressionTree clonedTree = (SymbolicExpressionTree)symbolicExpressionTree.Clone();

      var functionDefiningBranches = clonedTree.IterateNodesPrefix().OfType<DefunTreeNode>();
      if (functionDefiningBranches.Count() == 0)
        // no function defining branch found => abort
        return false;

      // select a random function defining branch
      var selectedDefunBranch = functionDefiningBranches.SelectRandom(random);
      var definedArguments = (from symbol in selectedDefunBranch.Grammar.Symbols.OfType<Argument>()
                              select symbol.ArgumentIndex).Distinct();
      if (definedArguments.Count() >= maxFunctionArguments)
        // max number of arguments reached => abort
        return false;

      var allowedArgumentIndexes = Enumerable.Range(0, maxFunctionArguments);
      var newArgumentIndex = allowedArgumentIndexes.Except(definedArguments).First();
      ArgumentTreeNode newArgumentNode = MakeArgumentNode(newArgumentIndex);

      // this operation potentially creates very big trees so the access to the size property might throw overflow exception
      try {
        if (CreateNewArgumentForDefun(random, clonedTree, selectedDefunBranch, newArgumentNode) && clonedTree.Size <= maxTreeSize && clonedTree.Height <= maxTreeHeight) {

          // size constraints are fulfilled 
          // replace root of original tree with root of manipulated tree
          symbolicExpressionTree.Root = clonedTree.Root;
          return true;
        } else {
          // keep originalTree
          return false;
        }
      }
      catch (OverflowException) {
        // keep original tree
        return false;
      }
    }

    private static bool CreateNewArgumentForDefun(IRandom random, SymbolicExpressionTree tree, DefunTreeNode defunBranch, ArgumentTreeNode newArgumentNode) {
      // select a random cut point in the function defining branch
      // the branch at the cut point is to be replaced by a new argument node
      var cutPoints = (from node in defunBranch.IterateNodesPrefix()
                       where node.SubTrees.Count > 0
                       from subtree in node.SubTrees
                       select new { Parent = node, ReplacedChildIndex = node.SubTrees.IndexOf(subtree), ReplacedChild = subtree }).ToList();

      if (cutPoints.Count() == 0)
        // no cut point found => abort;
        return false;
      var selectedCutPoint = cutPoints[random.Next(cutPoints.Count)];
      // replace the branch at the cut point with an argument node
      var replacedBranch = selectedCutPoint.ReplacedChild;
      selectedCutPoint.Parent.RemoveSubTree(selectedCutPoint.ReplacedChildIndex);
      selectedCutPoint.Parent.InsertSubTree(selectedCutPoint.ReplacedChildIndex, newArgumentNode);

      // find all old invocations of the selected ADF and attach a cloned version of the replaced branch (with all argument-nodes expanded)
      // iterate in post-fix order to make sure that the subtrees of n are already adapted when n is processed
      var invocationNodes = (from node in tree.IterateNodesPostfix().OfType<InvokeFunctionTreeNode>()
                             where node.Symbol.FunctionName == defunBranch.FunctionName
                             where node.SubTrees.Count == defunBranch.NumberOfArguments
                             select node).ToList();
      // do this repeatedly until no matching invocations are found      
      while (invocationNodes.Count > 0) {
        List<SymbolicExpressionTreeNode> newlyAddedBranches = new List<SymbolicExpressionTreeNode>();
        foreach (var invocationNode in invocationNodes) {
          // check that the invocation node really has the correct number of arguments
          if (invocationNode.SubTrees.Count != defunBranch.NumberOfArguments) throw new InvalidOperationException();
          // append a new argument branch after expanding all argument nodes
          var clonedBranch = (SymbolicExpressionTreeNode)replacedBranch.Clone();
          clonedBranch = ReplaceArgumentsInBranch(clonedBranch, invocationNode.SubTrees);
          invocationNode.InsertSubTree(newArgumentNode.Symbol.ArgumentIndex, clonedBranch);
          newlyAddedBranches.Add(clonedBranch);
        }
        // iterate in post-fix order to make sure that the subtrees of n are already adapted when n is processed
        invocationNodes = (from newlyAddedBranch in newlyAddedBranches
                           from node in newlyAddedBranch.IterateNodesPostfix().OfType<InvokeFunctionTreeNode>()
                           where node.Symbol.FunctionName == defunBranch.FunctionName
                           where node.SubTrees.Count == defunBranch.NumberOfArguments
                           select node).ToList();
      }
      // increase expected number of arguments of function defining branch
      // it's possible that the number of actually referenced arguments was reduced (all references were replaced by a single new argument)
      // but the number of expected arguments is increased anyway
      defunBranch.NumberOfArguments++;
      defunBranch.Grammar.AddSymbol(newArgumentNode.Symbol);
      defunBranch.Grammar.SetMinSubtreeCount(newArgumentNode.Symbol, 0);
      defunBranch.Grammar.SetMaxSubtreeCount(newArgumentNode.Symbol, 0);
      // allow the argument as child of any other symbol
      foreach (var symb in defunBranch.Grammar.Symbols)
        for (int i = 0; i < defunBranch.Grammar.GetMaxSubtreeCount(symb); i++) {
          defunBranch.Grammar.SetAllowedChild(symb, newArgumentNode.Symbol, i);
        }
      foreach (var subtree in tree.Root.SubTrees) {
        // when the changed function is known in the branch then update the number of arguments
        var matchingSymbol = subtree.Grammar.Symbols.OfType<InvokeFunction>().Where(s => s.FunctionName == defunBranch.FunctionName).SingleOrDefault();
        if (matchingSymbol != null) {
          subtree.Grammar.SetMinSubtreeCount(matchingSymbol, defunBranch.NumberOfArguments);
          subtree.Grammar.SetMaxSubtreeCount(matchingSymbol, defunBranch.NumberOfArguments);
          foreach (var child in subtree.GetAllowedSymbols(0)) {
            for (int i = 0; i < subtree.Grammar.GetMaxSubtreeCount(matchingSymbol); i++) {
              subtree.Grammar.SetAllowedChild(matchingSymbol, child, i);
            }
          }
        }
      }

      return true;
    }

    private static SymbolicExpressionTreeNode ReplaceArgumentsInBranch(SymbolicExpressionTreeNode branch, IList<SymbolicExpressionTreeNode> argumentTrees) {
      ArgumentTreeNode argNode = branch as ArgumentTreeNode;
      if (argNode != null) {
        // replace argument nodes by a clone of the original subtree that provided the result for the argument node
        return (SymbolicExpressionTreeNode)argumentTrees[argNode.Symbol.ArgumentIndex].Clone();
      } else {
        // call recursively for all subtree
        List<SymbolicExpressionTreeNode> subtrees = new List<SymbolicExpressionTreeNode>(branch.SubTrees);
        while (branch.SubTrees.Count > 0) branch.RemoveSubTree(0);
        foreach (var subtree in subtrees) {
          branch.AddSubTree(ReplaceArgumentsInBranch(subtree, argumentTrees));
        }
        return branch;
      }
    }

    private static ArgumentTreeNode MakeArgumentNode(int argIndex) {
      var node = (ArgumentTreeNode)(new Argument(argIndex)).CreateTreeNode();
      return node;
    }
  }
}
