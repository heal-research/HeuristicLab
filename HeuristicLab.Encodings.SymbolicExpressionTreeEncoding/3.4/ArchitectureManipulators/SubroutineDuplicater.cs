#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  /// <summary>
  /// Manipulates a symbolic expression by duplicating a preexisting function-defining branch.
  /// As described in Koza, Bennett, Andre, Keane, Genetic Programming III - Darwinian Invention and Problem Solving, 1999, pp. 88
  /// </summary>
  [Item("SubroutineDuplicater", "Manipulates a symbolic expression by duplicating a preexisting function-defining branch. As described in Koza, Bennett, Andre, Keane, Genetic Programming III - Darwinian Invention and Problem Solving, 1999, pp. 88")]
  [StorableClass]
  public sealed class SubroutineDuplicater : SymbolicExpressionTreeArchitectureManipulator {
    [StorableConstructor]
    private SubroutineDuplicater(bool deserializing) : base(deserializing) { }
    private SubroutineDuplicater(SubroutineDuplicater original, Cloner cloner)
      : base(original, cloner) {
    }
    public SubroutineDuplicater() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SubroutineDuplicater(this, cloner);
    }

    public override sealed void ModifyArchitecture(
      IRandom random,
      ISymbolicExpressionTree symbolicExpressionTree,
      IntValue maxFunctionDefinitions, IntValue maxFunctionArguments) {
      DuplicateSubroutine(random, symbolicExpressionTree, maxFunctionDefinitions.Value, maxFunctionArguments.Value);
    }

    public static bool DuplicateSubroutine(
      IRandom random,
      ISymbolicExpressionTree symbolicExpressionTree,
      int maxFunctionDefinitions, int maxFunctionArguments) {
      var functionDefiningBranches = symbolicExpressionTree.IterateNodesPrefix().OfType<DefunTreeNode>().ToList();
      if (!functionDefiningBranches.Any() || functionDefiningBranches.Count() == maxFunctionDefinitions)
        // no function defining branches to duplicate or already reached the max number of ADFs
        return false;

      string formatString = new StringBuilder().Append('0', (int)Math.Log10(maxFunctionDefinitions) + 1).ToString(); // >= 100 functions => ###
      var allowedFunctionNames = from index in Enumerable.Range(0, maxFunctionDefinitions)
                                 select "ADF" + index.ToString(formatString);

      var selectedBranch = functionDefiningBranches.SampleRandom(random);
      var duplicatedDefunBranch = (DefunTreeNode)selectedBranch.Clone();
      string newFunctionName = allowedFunctionNames.Except(UsedFunctionNames(symbolicExpressionTree)).First();
      duplicatedDefunBranch.FunctionName = newFunctionName;
      symbolicExpressionTree.Root.AddSubtree(duplicatedDefunBranch);
      duplicatedDefunBranch.SetGrammar((ISymbolicExpressionTreeGrammar)selectedBranch.Grammar.Clone());

      var allowedChildSymbols = selectedBranch.Grammar.GetAllowedChildSymbols(selectedBranch.Symbol);
      foreach (var allowedChildSymbol in allowedChildSymbols)
        duplicatedDefunBranch.Grammar.AddAllowedChildSymbol(duplicatedDefunBranch.Symbol, allowedChildSymbol);
      var maxSubtrees = selectedBranch.Grammar.GetMaximumSubtreeCount(selectedBranch.Symbol);
      for (int i = 0; i < maxSubtrees; i++) {
        foreach (var allowedChildSymbol in selectedBranch.Grammar.GetAllowedChildSymbols(selectedBranch.Symbol, i))
          duplicatedDefunBranch.Grammar.AddAllowedChildSymbol(duplicatedDefunBranch.Symbol, allowedChildSymbol);
      }

      // add an invoke symbol for each branch that is allowed to invoke the original function
      foreach (var subtree in symbolicExpressionTree.Root.Subtrees.OfType<SymbolicExpressionTreeTopLevelNode>()) {
        var matchingInvokeSymbol = (from symb in subtree.Grammar.Symbols.OfType<InvokeFunction>()
                                    where symb.FunctionName == selectedBranch.FunctionName
                                    select symb).SingleOrDefault();
        if (matchingInvokeSymbol != null) {
          var invokeSymbol = new InvokeFunction(duplicatedDefunBranch.FunctionName);
          subtree.Grammar.AddSymbol(invokeSymbol);
          subtree.Grammar.SetSubtreeCount(invokeSymbol, duplicatedDefunBranch.NumberOfArguments, duplicatedDefunBranch.NumberOfArguments);

          foreach (ISymbol symbol in subtree.Grammar.Symbols) {
            if (subtree.Grammar.IsAllowedChildSymbol(symbol, matchingInvokeSymbol))
              subtree.Grammar.AddAllowedChildSymbol(symbol, invokeSymbol);
            else {
              for (int i = 0; i < subtree.Grammar.GetMaximumSubtreeCount(symbol); i++)
                if (subtree.Grammar.IsAllowedChildSymbol(symbol, matchingInvokeSymbol, i))
                  subtree.Grammar.AddAllowedChildSymbol(symbol, invokeSymbol, i);
            }
          }

          foreach (ISymbol symbol in subtree.Grammar.GetAllowedChildSymbols(matchingInvokeSymbol))
            if (symbol != invokeSymbol) //avoid duplicate entry invokesymbol / invokesymbol
              subtree.Grammar.AddAllowedChildSymbol(invokeSymbol, symbol);
          for (int i = 0; i < subtree.Grammar.GetMaximumSubtreeCount(matchingInvokeSymbol); i++) {
            foreach (ISymbol symbol in subtree.Grammar.GetAllowedChildSymbols(matchingInvokeSymbol, i).Except(subtree.Grammar.GetAllowedChildSymbols(matchingInvokeSymbol)))
              subtree.Grammar.AddAllowedChildSymbol(invokeSymbol, symbol, i);
          }
        }
        // in the current subtree:
        // for all invoke nodes of the original function replace the invoke of the original function with an invoke of the new function randomly
        var originalFunctionInvocations = from node in subtree.IterateNodesPrefix().OfType<InvokeFunctionTreeNode>()
                                          where node.Symbol.FunctionName == selectedBranch.FunctionName
                                          select node;
        foreach (var originalFunctionInvokeNode in originalFunctionInvocations) {
          var newInvokeSymbol = (from symb in subtree.Grammar.Symbols.OfType<InvokeFunction>()
                                 where symb.FunctionName == duplicatedDefunBranch.FunctionName
                                 select symb).Single();
          // flip coin wether to replace with newly defined function
          if (random.NextDouble() < 0.5) {
            originalFunctionInvokeNode.Symbol = newInvokeSymbol;
          }
        }
      }
      return true;
    }

    private static IEnumerable<string> UsedFunctionNames(ISymbolicExpressionTree symbolicExpressionTree) {
      return from node in symbolicExpressionTree.IterateNodesPrefix()
             where node.Symbol is Defun
             select ((DefunTreeNode)node).FunctionName;
    }
  }
}
