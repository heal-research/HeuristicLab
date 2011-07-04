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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  /// <summary>
  /// Manipulates a symbolic expression by duplicating an existing argument node of a function-defining branch.
  /// As described in Koza, Bennett, Andre, Keane, Genetic Programming III - Darwinian Invention and Problem Solving, 1999, pp. 94
  /// </summary>
  [Item("ArgumentDuplicater", "Manipulates a symbolic expression by duplicating an existing argument node of a function-defining branch. As described in Koza, Bennett, Andre, Keane, Genetic Programming III - Darwinian Invention and Problem Solving, 1999, pp. 94")]
  [StorableClass]
  public sealed class ArgumentDuplicater : SymbolicExpressionTreeArchitectureManipulator {
    [StorableConstructor]
    private ArgumentDuplicater(bool deserializing) : base(deserializing) { }
    private ArgumentDuplicater(ArgumentDuplicater original, Cloner cloner) : base(original, cloner) { }
    public ArgumentDuplicater() : base() { }

    public override sealed void ModifyArchitecture(
      IRandom random,
      ISymbolicExpressionTree symbolicExpressionTree,
      IntValue maxFunctionDefinitions, IntValue maxFunctionArguments) {
      DuplicateArgument(random, symbolicExpressionTree, maxFunctionDefinitions.Value, maxFunctionArguments.Value);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ArgumentDuplicater(this, cloner);
    }

    public static bool DuplicateArgument(
      IRandom random,
      ISymbolicExpressionTree symbolicExpressionTree,
      int maxFunctionDefinitions, int maxFunctionArguments) {
      var functionDefiningBranches = symbolicExpressionTree.IterateNodesPrefix().OfType<DefunTreeNode>();

      var allowedArgumentIndexes = Enumerable.Range(0, maxFunctionArguments);
      if (functionDefiningBranches.Count() == 0)
        // no function defining branches => abort
        return false;

      var selectedDefunBranch = functionDefiningBranches.SelectRandom(random);
      var argumentSymbols = selectedDefunBranch.Grammar.Symbols.OfType<Argument>();
      if (argumentSymbols.Count() == 0 || argumentSymbols.Count() >= maxFunctionArguments)
        // when no argument or number of arguments is already at max allowed value => abort
        return false;
      var selectedArgumentSymbol = argumentSymbols.SelectRandom(random);
      var takenIndexes = argumentSymbols.Select(s => s.ArgumentIndex);
      var newArgumentIndex = allowedArgumentIndexes.Except(takenIndexes).First();

      var newArgSymbol = new Argument(newArgumentIndex);

      // replace existing references to the original argument with references to the new argument randomly in the selectedBranch
      var argumentNodes = selectedDefunBranch.IterateNodesPrefix().OfType<ArgumentTreeNode>();
      foreach (var argNode in argumentNodes) {
        if (argNode.Symbol == selectedArgumentSymbol) {
          if (random.NextDouble() < 0.5) {
            argNode.Symbol = newArgSymbol;
          }
        }
      }
      // find invocations of the functions and duplicate the matching argument branch
      var invocationNodes = (from node in symbolicExpressionTree.IterateNodesPrefix().OfType<InvokeFunctionTreeNode>()
                             where node.Symbol.FunctionName == selectedDefunBranch.FunctionName
                             where node.Subtrees.Count() == selectedDefunBranch.NumberOfArguments
                             select node).ToList();
      // do this repeatedly until no matching invocations are found      
      while (invocationNodes.Count() > 0) {
        List<ISymbolicExpressionTreeNode> newlyAddedBranches = new List<ISymbolicExpressionTreeNode>();
        foreach (var invokeNode in invocationNodes) {
          // check that the invocation node really has the correct number of arguments
          if (invokeNode.Subtrees.Count() != selectedDefunBranch.NumberOfArguments) throw new InvalidOperationException();
          var argumentBranch = invokeNode.GetSubtree(selectedArgumentSymbol.ArgumentIndex);
          var clonedArgumentBranch = (ISymbolicExpressionTreeNode)argumentBranch.Clone();
          invokeNode.InsertSubtree(newArgumentIndex, clonedArgumentBranch);
          newlyAddedBranches.Add(clonedArgumentBranch);
        }
        invocationNodes = (from newlyAddedBranch in newlyAddedBranches
                           from node in newlyAddedBranch.IterateNodesPrefix().OfType<InvokeFunctionTreeNode>()
                           where node.Symbol.FunctionName == selectedDefunBranch.FunctionName
                           where node.Subtrees.Count() == selectedDefunBranch.NumberOfArguments
                           select node).ToList();
      }
      // register the new argument symbol and increase the number of arguments of the ADF
      selectedDefunBranch.Grammar.AddSymbol(newArgSymbol);
      selectedDefunBranch.Grammar.SetSubtreeCount(newArgSymbol, 0, 0);
      // allow the duplicated argument as child of all other arguments where the orginal argument was allowed
      GrammarModifier.SetAllowedParentSymbols(selectedDefunBranch.Grammar, selectedArgumentSymbol, newArgSymbol);
      selectedDefunBranch.NumberOfArguments++;

      // increase the arity of the changed ADF in all branches that can use this ADF
      foreach (var subtree in symbolicExpressionTree.Root.Subtrees) {
        var matchingInvokeSymbol = (from symb in subtree.Grammar.Symbols.OfType<InvokeFunction>()
                                    where symb.FunctionName == selectedDefunBranch.FunctionName
                                    select symb).SingleOrDefault();
        if (matchingInvokeSymbol != null) {
          subtree.Grammar.SetSubtreeCount(matchingInvokeSymbol, selectedDefunBranch.NumberOfArguments, selectedDefunBranch.NumberOfArguments);
          foreach (var symb in subtree.Grammar.Symbols) {
            if (symb is StartSymbol || symb is ProgramRootSymbol) continue;
            if (subtree.Grammar.IsAllowedChildSymbol(matchingInvokeSymbol, symb, selectedArgumentSymbol.ArgumentIndex))
              subtree.Grammar.AddAllowedChildSymbol(matchingInvokeSymbol, symb, newArgumentIndex);
          }
        }
      }
      return true;
    }
  }
}
