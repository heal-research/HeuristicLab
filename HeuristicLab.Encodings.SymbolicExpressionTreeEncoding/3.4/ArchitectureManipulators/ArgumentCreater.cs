#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.Random;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  /// <summary>
  /// Creates a new argument within one function-defining branch of a symbolic expression tree.
  /// As described in Koza, Bennett, Andre, Keane, Genetic Programming III - Darwinian Invention and Problem Solving, 1999, pp. 106
  /// </summary>
  [Item("ArgumentCreater", "Manipulates a symbolic expression by creating a new argument within one function-defining branch. As described in Koza, Bennett, Andre, Keane, Genetic Programming III - Darwinian Invention and Problem Solving, 1999, pp. 106")]
  [StorableType("2AC1EE1A-B2B4-47E7-97AF-BFC9FB2D7BA2")]
  public sealed class ArgumentCreater : SymbolicExpressionTreeArchitectureManipulator, ISymbolicExpressionTreeSizeConstraintOperator {
    private const string MaximumSymbolicExpressionTreeLengthParameterName = "MaximumSymbolicExpressionTreeLength";
    private const string MaximumSymbolicExpressionTreeDepthParameterName = "MaximumSymbolicExpressionTreeDepth";
    #region Parameter Properties
    public IValueLookupParameter<IntValue> MaximumSymbolicExpressionTreeLengthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeLengthParameterName]; }
    }
    public IValueLookupParameter<IntValue> MaximumSymbolicExpressionTreeDepthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeDepthParameterName]; }
    }
    #endregion
    #region Properties
    public IntValue MaximumSymbolicExpressionTreeLength {
      get { return MaximumSymbolicExpressionTreeLengthParameter.ActualValue; }
    }
    public IntValue MaximumSymbolicExpressionTreeDepth {
      get { return MaximumSymbolicExpressionTreeDepthParameter.ActualValue; }
    }
    #endregion
    [StorableConstructor]
    private ArgumentCreater(StorableConstructorFlag _) : base(_) { }
    private ArgumentCreater(ArgumentCreater original, Cloner cloner) : base(original, cloner) { }
    public ArgumentCreater()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumSymbolicExpressionTreeLengthParameterName, "The maximal length (number of nodes) of the symbolic expression tree."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumSymbolicExpressionTreeDepthParameterName, "The maximal depth of the symbolic expression tree (a tree with one node has depth = 0)."));
    }

    public override sealed void ModifyArchitecture(
      IRandom random,
      ISymbolicExpressionTree symbolicExpressionTree,
      IntValue maxFunctionDefinitions, IntValue maxFunctionArguments) {
      CreateNewArgument(random, symbolicExpressionTree, MaximumSymbolicExpressionTreeLength.Value, MaximumSymbolicExpressionTreeDepth.Value, maxFunctionDefinitions.Value, maxFunctionArguments.Value);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ArgumentCreater(this, cloner);
    }

    public static bool CreateNewArgument(
      IRandom random,
      ISymbolicExpressionTree symbolicExpressionTree,
      int maxTreeLength, int maxTreeDepth,
      int maxFunctionDefinitions, int maxFunctionArguments) {
      // work on a copy in case we find out later that the tree would be too big
      // in this case it's easiest to simply return the original tree.
      ISymbolicExpressionTree clonedTree = (ISymbolicExpressionTree)symbolicExpressionTree.Clone();

      var functionDefiningBranches = clonedTree.IterateNodesPrefix().OfType<DefunTreeNode>().ToList();
      if (!functionDefiningBranches.Any())
        // no function defining branch found => abort
        return false;

      // select a random function defining branch
      var selectedDefunBranch = functionDefiningBranches.SampleRandom(random);

      var definedArguments = (from symbol in selectedDefunBranch.Grammar.Symbols.OfType<Argument>()
                              select symbol.ArgumentIndex).Distinct();
      if (definedArguments.Count() >= maxFunctionArguments)
        // max number of arguments reached => abort
        return false;

      var allowedArgumentIndexes = Enumerable.Range(0, maxFunctionArguments);
      var newArgumentIndex = allowedArgumentIndexes.Except(definedArguments).First();
      ArgumentTreeNode newArgumentNode = MakeArgumentNode(newArgumentIndex);

      // this operation potentially creates very big trees so the access to the length property might throw overflow exception
      try {
        if (CreateNewArgumentForDefun(random, clonedTree, selectedDefunBranch, newArgumentNode) && clonedTree.Length <= maxTreeLength && clonedTree.Depth <= maxTreeDepth) {

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

    private static bool CreateNewArgumentForDefun(IRandom random, ISymbolicExpressionTree tree, DefunTreeNode defunBranch, ArgumentTreeNode newArgumentNode) {
      // select a random cut point in the function defining branch
      // the branch at the cut point is to be replaced by a new argument node
      var cutPoints = (from node in defunBranch.IterateNodesPrefix()
                       where node.Subtrees.Count() > 0 &&
                             !node.IterateNodesPrefix().OfType<ArgumentTreeNode>().Any() &&
                             !node.IterateNodesPrefix().OfType<InvokeFunctionTreeNode>().Any()
                       from subtree in node.Subtrees
                       select new CutPoint(node, subtree)).ToList();

      if (cutPoints.Count() == 0)
        // no cut point found => abort;
        return false;
      var selectedCutPoint = cutPoints[random.Next(cutPoints.Count)];
      // replace the branch at the cut point with an argument node
      var replacedBranch = selectedCutPoint.Child;
      selectedCutPoint.Parent.RemoveSubtree(selectedCutPoint.ChildIndex);
      selectedCutPoint.Parent.InsertSubtree(selectedCutPoint.ChildIndex, newArgumentNode);

      // find all old invocations of the selected ADF and attach a cloned version of the replaced branch (with all argument-nodes expanded)
      // iterate in post-fix order to make sure that the subtrees of n are already adapted when n is processed
      var invocationNodes = (from node in tree.IterateNodesPostfix().OfType<InvokeFunctionTreeNode>()
                             where node.Symbol.FunctionName == defunBranch.FunctionName
                             where node.Subtrees.Count() == defunBranch.NumberOfArguments
                             select node).ToList();
      // do this repeatedly until no matching invocations are found      
      while (invocationNodes.Count > 0) {
        List<ISymbolicExpressionTreeNode> newlyAddedBranches = new List<ISymbolicExpressionTreeNode>();
        foreach (var invocationNode in invocationNodes) {
          // check that the invocation node really has the correct number of arguments
          if (invocationNode.Subtrees.Count() != defunBranch.NumberOfArguments) throw new InvalidOperationException();
          // append a new argument branch after expanding all argument nodes
          var clonedBranch = (ISymbolicExpressionTreeNode)replacedBranch.Clone();
          clonedBranch = ReplaceArgumentsInBranch(clonedBranch, invocationNode.Subtrees);
          invocationNode.InsertSubtree(newArgumentNode.Symbol.ArgumentIndex, clonedBranch);
          newlyAddedBranches.Add(clonedBranch);
        }
        // iterate in post-fix order to make sure that the subtrees of n are already adapted when n is processed
        invocationNodes = (from newlyAddedBranch in newlyAddedBranches
                           from node in newlyAddedBranch.IterateNodesPostfix().OfType<InvokeFunctionTreeNode>()
                           where node.Symbol.FunctionName == defunBranch.FunctionName
                           where node.Subtrees.Count() == defunBranch.NumberOfArguments
                           select node).ToList();
      }
      // increase expected number of arguments of function defining branch
      // it's possible that the number of actually referenced arguments was reduced (all references were replaced by a single new argument)
      // but the number of expected arguments is increased anyway
      defunBranch.NumberOfArguments++;
      defunBranch.Grammar.AddSymbol(newArgumentNode.Symbol);
      defunBranch.Grammar.SetSubtreeCount(newArgumentNode.Symbol, 0, 0);
      // allow the argument as child of any other symbol
      GrammarModifier.SetAllowedParentSymbols(defunBranch.Grammar, selectedCutPoint.Child.Symbol, newArgumentNode.Symbol);

      foreach (var subtree in tree.Root.Subtrees) {
        // when the changed function is known in the branch then update the number of arguments
        var matchingSymbol = subtree.Grammar.Symbols.OfType<InvokeFunction>().Where(s => s.FunctionName == defunBranch.FunctionName).SingleOrDefault();
        if (matchingSymbol != null) {
          subtree.Grammar.SetSubtreeCount(matchingSymbol, defunBranch.NumberOfArguments, defunBranch.NumberOfArguments);
          foreach (var symb in subtree.Grammar.Symbols) {
            if (symb is StartSymbol || symb is ProgramRootSymbol) continue;
            if (symb.Name == matchingSymbol.Name) continue; //don't allow invoke as child of invoke
            if (subtree.Grammar.IsAllowedChildSymbol(selectedCutPoint.Parent.Symbol, symb, selectedCutPoint.ChildIndex))
              subtree.Grammar.AddAllowedChildSymbol(matchingSymbol, symb, newArgumentNode.Symbol.ArgumentIndex);
          }
        }
      }

      return true;
    }

    private static ISymbolicExpressionTreeNode ReplaceArgumentsInBranch(ISymbolicExpressionTreeNode branch, IEnumerable<ISymbolicExpressionTreeNode> argumentTrees) {
      ArgumentTreeNode argNode = branch as ArgumentTreeNode;
      if (argNode != null) {
        // replace argument nodes by a clone of the original subtree that provided the result for the argument node
        return (SymbolicExpressionTreeNode)argumentTrees.ElementAt(argNode.Symbol.ArgumentIndex).Clone();
      } else {
        // call recursively for all subtree
        List<ISymbolicExpressionTreeNode> subtrees = new List<ISymbolicExpressionTreeNode>(branch.Subtrees);
        while (branch.Subtrees.Count() > 0) branch.RemoveSubtree(0);
        foreach (var subtree in subtrees) {
          branch.AddSubtree(ReplaceArgumentsInBranch(subtree, argumentTrees));
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
