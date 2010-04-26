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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.ArchitectureAlteringOperators {
  /// <summary>
  /// Manipulates a symbolic expression by duplicating an existing argument node of a function-defining branch.
  /// As described in Koza, Bennett, Andre, Keane, Genetic Programming III - Darwinian Invention and Problem Solving, 1999, pp. 94
  /// </summary>
  [Item("ArgumentDuplicater", "Manipulates a symbolic expression by duplicating an existing argument node of a function-defining branch.")]
  [StorableClass]
  public sealed class ArgumentDuplicater : SymbolicExpressionTreeArchitectureManipulator {
    public override sealed void ModifyArchitecture(
      IRandom random,
      SymbolicExpressionTree symbolicExpressionTree,
      ISymbolicExpressionGrammar grammar,
      IntValue maxTreeSize, IntValue maxTreeHeight,
      IntValue maxFunctionDefiningBranches, IntValue maxFunctionArguments,
      out bool success) {
      success = DuplicateArgument(random, symbolicExpressionTree, grammar, maxTreeSize.Value, maxTreeHeight.Value, maxFunctionDefiningBranches.Value, maxFunctionArguments.Value);
    }

    public static bool DuplicateArgument(
      IRandom random,
      SymbolicExpressionTree symbolicExpressionTree,
      ISymbolicExpressionGrammar grammar,
      int maxTreeSize, int maxTreeHeight,
      int maxFunctionDefiningBranches, int maxFunctionArguments) {
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
      var invocationNodes = from node in symbolicExpressionTree.IterateNodesPrefix().OfType<InvokeFunctionTreeNode>()
                            where node.Symbol.FunctionName == selectedDefunBranch.FunctionName
                            select node;
      foreach (var invokeNode in invocationNodes) {
        var argumentBranch = invokeNode.SubTrees[selectedArgumentSymbol.ArgumentIndex];
        var clonedArgumentBranch = (SymbolicExpressionTreeNode)argumentBranch.Clone();
        invokeNode.InsertSubTree(newArgumentIndex, clonedArgumentBranch);
      }
      // register the new argument symbol and increase the number of arguments of the ADF
      selectedDefunBranch.Grammar.AddSymbol(newArgSymbol);
      selectedDefunBranch.Grammar.SetMinSubtreeCount(newArgSymbol, 0);
      selectedDefunBranch.Grammar.SetMaxSubtreeCount(newArgSymbol, 0);
      // allow the argument as child of any other symbol
      foreach (var symb in selectedDefunBranch.Grammar.Symbols)
        for (int i = 0; i < selectedDefunBranch.Grammar.GetMaxSubtreeCount(symb); i++) {
          selectedDefunBranch.Grammar.SetAllowedChild(symb, newArgSymbol, i);
        }
      selectedDefunBranch.NumberOfArguments++;

      // increase the arity of the changed ADF in all branches that can use this ADF
      foreach (var subtree in symbolicExpressionTree.Root.SubTrees) {
        var matchingInvokeSymbol = (from symb in subtree.Grammar.Symbols.OfType<InvokeFunction>()
                                    where symb.FunctionName == selectedDefunBranch.FunctionName
                                    select symb).SingleOrDefault();
        if (matchingInvokeSymbol != null) {
          subtree.Grammar.SetMinSubtreeCount(matchingInvokeSymbol, selectedDefunBranch.NumberOfArguments);
          subtree.Grammar.SetMaxSubtreeCount(matchingInvokeSymbol, selectedDefunBranch.NumberOfArguments);
          foreach (var child in subtree.GetAllowedSymbols(0)) {
            for (int i = 0; i < subtree.Grammar.GetMaxSubtreeCount(matchingInvokeSymbol); i++) {
              subtree.Grammar.SetAllowedChild(matchingInvokeSymbol, child, i);
            }
          }
        }
      }
      return true;
    }
  }
}
