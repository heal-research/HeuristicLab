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
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.ArchitectureManipulators {
  /// <summary>
  /// Manipulates a symbolic expression by adding one new function-defining branch containing
  /// a proportion of a preexisting branch and by creating a reference to the new branch.
  /// As described in Koza, Bennett, Andre, Keane, Genetic Programming III - Darwinian Invention and Problem Solving, 1999, pp. 97
  /// </summary>
  [Item("SubroutineCreater", "Manipulates a symbolic expression by adding one new function-defining branch containing a proportion of a preexisting branch and by creating a reference to the new branch.")]
  [StorableClass]
  public sealed class SubroutineCreater : SymbolicExpressionTreeArchitectureManipulator {
    private const double ARGUMENT_CUTOFF_PROBABILITY = 0.05;

    [StorableConstructor]
    private SubroutineCreater(bool deserializing) : base(deserializing) { }
    private SubroutineCreater(SubroutineCreater original, Cloner cloner) : base(original, cloner) { }
    public SubroutineCreater() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SubroutineCreater(this, cloner);
    }

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
      if (symbolicExpressionTree.Size + 4 > maxTreeSize)
        // defining a new function causes an size increase by 4 nodes (max) if the max tree size is reached => abort
        return false;
      string formatString = new StringBuilder().Append('0', (int)Math.Log10(maxFunctionDefiningBranches * 10 - 1)).ToString(); // >= 100 functions => ###
      var allowedFunctionNames = from index in Enumerable.Range(0, maxFunctionDefiningBranches)
                                 select "ADF" + index.ToString(formatString);

      // select a random body (either the result producing branch or an ADF branch)
      var bodies = from node in symbolicExpressionTree.Root.SubTrees
                   select new { Tree = node, Size = node.GetSize() };
      var totalNumberOfBodyNodes = bodies.Select(x => x.Size).Sum();
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

      // select a random cut point in the selected branch
      var allCutPoints = from parent in selectedBody.IterateNodesPrefix()
                         from subtree in parent.SubTrees
                         select new { Parent = parent, ReplacedBranchIndex = parent.SubTrees.IndexOf(subtree), ReplacedBranch = subtree };
      if (allCutPoints.Count() == 0)
        // no cut points => abort
        return false;
      string newFunctionName = allowedFunctionNames.Except(functionDefiningBranches.Select(x => x.FunctionName)).First();
      var selectedCutPoint = allCutPoints.SelectRandom(random);
      // select random branches as argument cut-off points (replaced by argument terminal nodes in the function)
      List<SymbolicExpressionTreeNode> argumentBranches = SelectRandomArgumentBranches(selectedCutPoint.ReplacedBranch, random, ARGUMENT_CUTOFF_PROBABILITY, maxFunctionArguments);
      SymbolicExpressionTreeNode functionBody = selectedCutPoint.ReplacedBranch;
      // disconnect the function body from the tree
      selectedCutPoint.Parent.RemoveSubTree(selectedCutPoint.ReplacedBranchIndex);
      // disconnect the argument branches from the function
      functionBody = DisconnectBranches(functionBody, argumentBranches);
      // insert a function invocation symbol instead
      var invokeNode = (InvokeFunctionTreeNode)(new InvokeFunction(newFunctionName)).CreateTreeNode();
      selectedCutPoint.Parent.InsertSubTree(selectedCutPoint.ReplacedBranchIndex, invokeNode);
      // add the branches selected as argument as subtrees of the function invocation node
      foreach (var argumentBranch in argumentBranches)
        invokeNode.AddSubTree(argumentBranch);

      // insert a new function defining branch
      var defunNode = (DefunTreeNode)(new Defun()).CreateTreeNode();
      defunNode.FunctionName = newFunctionName;
      defunNode.AddSubTree(functionBody);
      symbolicExpressionTree.Root.AddSubTree(defunNode);
      // the grammar in the newly defined function is a clone of the grammar of the originating branch
      defunNode.SetGrammar((ISymbolicExpressionGrammar)selectedBody.Grammar.Clone());
      // remove all argument symbols from grammar
      var oldArgumentSymbols = defunNode.Grammar.Symbols.OfType<Argument>().ToList();
      foreach (var oldArgSymb in oldArgumentSymbols)
        defunNode.Grammar.RemoveSymbol(oldArgSymb);
      // find unique argument indexes and matching symbols in the function defining branch 
      var newArgumentIndexes = (from node in defunNode.IterateNodesPrefix().OfType<ArgumentTreeNode>()
                                select node.Symbol.ArgumentIndex).Distinct();
      // add argument symbols to grammar of function defining branch
      GrammarModifier.AddDynamicArguments(defunNode.Grammar, defunNode.Symbol, newArgumentIndexes);
      defunNode.NumberOfArguments = newArgumentIndexes.Count();
      if (defunNode.NumberOfArguments != argumentBranches.Count) throw new InvalidOperationException();
      // add invoke symbol for newly defined function to the original branch 
      var allowedParents = from symb in selectedBody.Grammar.Symbols
                           where !(symb is ProgramRootSymbol)
                           select symb;
      var allowedChildren = from symb in selectedBody.Grammar.Symbols
                            where selectedBody.Grammar.IsAllowedChild(selectedBody.Symbol, symb, 0)
                            select symb;
      GrammarModifier.AddDynamicSymbol(selectedBody.Grammar, selectedBody.Symbol, defunNode.FunctionName, defunNode.NumberOfArguments);

      // when the new function body was taken from another function definition
      // add invoke symbol for newly defined function to all branches that are allowed to invoke the original branch
      if (selectedBody.Symbol is Defun) {
        var originalFunctionDefinition = selectedBody as DefunTreeNode;
        foreach (var subtree in symbolicExpressionTree.Root.SubTrees) {
          var originalBranchInvokeSymbol = (from symb in subtree.Grammar.Symbols.OfType<InvokeFunction>()
                                            where symb.FunctionName == originalFunctionDefinition.FunctionName
                                            select symb).SingleOrDefault();
          // when the original branch can be invoked from the subtree then also allow invocation of the function
          if (originalBranchInvokeSymbol != null) {
            allowedParents = from symb in subtree.Grammar.Symbols
                             where !(symb is ProgramRootSymbol)
                             select symb;
            allowedChildren = from symb in subtree.Grammar.Symbols
                              where subtree.Grammar.IsAllowedChild(subtree.Symbol, symb, 0)
                              select symb;
            GrammarModifier.AddDynamicSymbol(subtree.Grammar, subtree.Symbol, defunNode.FunctionName, defunNode.NumberOfArguments);
          }
        }
      }
      return true;
    }

    private static SymbolicExpressionTreeNode DisconnectBranches(SymbolicExpressionTreeNode node, List<SymbolicExpressionTreeNode> argumentBranches) {
      if (argumentBranches.Contains(node)) {
        var argumentIndex = argumentBranches.IndexOf(node);
        var argSymbol = new Argument(argumentIndex);
        return argSymbol.CreateTreeNode();
      }
      // remove the subtrees so that we can clone only the root node
      List<SymbolicExpressionTreeNode> subtrees = new List<SymbolicExpressionTreeNode>(node.SubTrees);
      while (node.SubTrees.Count > 0) node.RemoveSubTree(0);
      // recursively apply function for subtrees or append a argument terminal node
      foreach (var subtree in subtrees) {
        node.AddSubTree(DisconnectBranches(subtree, argumentBranches));
      }
      return node;
    }

    private static List<SymbolicExpressionTreeNode> SelectRandomArgumentBranches(SymbolicExpressionTreeNode selectedRoot,
      IRandom random,
      double cutProbability,
      int maxArguments) {
      // breadth first determination of argument cut-off points
      // we must make sure that we cut off all original argument nodes and that the number of new argument is smaller than the limit
      List<SymbolicExpressionTreeNode> argumentBranches = new List<SymbolicExpressionTreeNode>();
      if (selectedRoot is ArgumentTreeNode) {
        argumentBranches.Add(selectedRoot);
        return argumentBranches;
      } else {
        // get the number of argument nodes (which must be cut-off) in the sub-trees
        var numberOfArgumentsInSubtrees = (from subtree in selectedRoot.SubTrees
                                           let nArgumentsInTree = subtree.IterateNodesPrefix().OfType<ArgumentTreeNode>().Count()
                                           select nArgumentsInTree).ToList();
        // determine the minimal number of new argument nodes for each sub-tree
        var minNewArgumentsForSubtrees = numberOfArgumentsInSubtrees.Select(x => x > 0 ? 1 : 0).ToList();
        if (minNewArgumentsForSubtrees.Sum() > maxArguments) {
          argumentBranches.Add(selectedRoot);
          return argumentBranches;
        }
        // cut-off in the sub-trees in random order
        var randomIndexes = (from index in Enumerable.Range(0, selectedRoot.SubTrees.Count)
                             select new { Index = index, OrderValue = random.NextDouble() }).OrderBy(x => x.OrderValue).Select(x => x.Index);
        foreach (var subtreeIndex in randomIndexes) {
          var subtree = selectedRoot.SubTrees[subtreeIndex];
          minNewArgumentsForSubtrees[subtreeIndex] = 0;
          // => cut-off at 0..n points somewhere in the current sub-tree
          // determine the maximum number of new arguments that should be created in the branch
          // as the maximum for the whole branch minus already added arguments minus minimal number of arguments still left
          int maxArgumentsFromBranch = maxArguments - argumentBranches.Count - minNewArgumentsForSubtrees.Sum();
          // when no argument is allowed from the current branch then we have to include the whole branch into the function
          // otherwise: choose randomly wether to cut off immediately or wether to extend the function body into the branch
          if (maxArgumentsFromBranch == 0) {
            // don't cut at all => the whole sub-tree branch is included in the function body
            // (we already checked ahead of time that there are no arguments left over in the subtree)
          } else if (random.NextDouble() >= cutProbability) {
            argumentBranches.AddRange(SelectRandomArgumentBranches(subtree, random, cutProbability, maxArgumentsFromBranch));
          } else {
            // cut-off at current sub-tree
            argumentBranches.Add(subtree);
          }
        }
        return argumentBranches;
      }
    }
  }
}
