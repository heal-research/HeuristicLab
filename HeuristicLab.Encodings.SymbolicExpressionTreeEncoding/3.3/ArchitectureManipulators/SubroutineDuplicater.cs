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
  /// Manipulates a symbolic expression by duplicating a preexisting function-defining branch.
  /// As described in Koza, Bennett, Andre, Keane, Genetic Programming III - Darwinian Invention and Problem Solving, 1999, pp. 88
  /// </summary>
  [Item("SubroutineDuplicater", "Manipulates a symbolic expression by duplicating a preexisting function-defining branch.")]
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
      SymbolicExpressionTree symbolicExpressionTree,
      ISymbolicExpressionGrammar grammar,
      IntValue maxTreeSize, IntValue maxTreeHeight,
      IntValue maxFunctionDefiningBranches, IntValue maxFunctionArguments,
      out bool success) {
      success = DuplicateSubroutine(random, symbolicExpressionTree, grammar, maxTreeSize.Value, maxTreeHeight.Value, maxFunctionDefiningBranches.Value, maxFunctionArguments.Value);
    }

    public static bool DuplicateSubroutine(
      IRandom random,
      SymbolicExpressionTree symbolicExpressionTree,
      ISymbolicExpressionGrammar grammar,
      int maxTreeSize, int maxTreeHeight,
      int maxFunctionDefiningBranches, int maxFunctionArguments) {
      var functionDefiningBranches = symbolicExpressionTree.IterateNodesPrefix().OfType<DefunTreeNode>();
      if (functionDefiningBranches.Count() == 0 || functionDefiningBranches.Count() == maxFunctionDefiningBranches)
        // no function defining branches to duplicate or already reached the max number of ADFs
        return false;

      string formatString = new StringBuilder().Append('0', (int)Math.Log10(maxFunctionDefiningBranches) + 1).ToString(); // >= 100 functions => ###
      var allowedFunctionNames = from index in Enumerable.Range(0, maxFunctionDefiningBranches)
                                 select "ADF" + index.ToString(formatString);
      var selectedBranch = functionDefiningBranches.SelectRandom(random);
      var duplicatedDefunBranch = (DefunTreeNode)selectedBranch.Clone();
      string newFunctionName = allowedFunctionNames.Except(UsedFunctionNames(symbolicExpressionTree)).First();
      duplicatedDefunBranch.FunctionName = newFunctionName;
      symbolicExpressionTree.Root.AddSubTree(duplicatedDefunBranch);
      duplicatedDefunBranch.SetGrammar((ISymbolicExpressionGrammar)selectedBranch.Grammar.Clone());
      // add an invoke symbol for each branch that is allowed to invoke the original function
      foreach (var subtree in symbolicExpressionTree.Root.SubTrees.OfType<SymbolicExpressionTreeTopLevelNode>()) {
        var matchingInvokeSymbol = (from symb in subtree.Grammar.Symbols.OfType<InvokeFunction>()
                                    where symb.FunctionName == selectedBranch.FunctionName
                                    select symb).SingleOrDefault();
        if (matchingInvokeSymbol != null) {
          GrammarModifier.AddDynamicSymbol(subtree.Grammar, subtree.Symbol, duplicatedDefunBranch.FunctionName, duplicatedDefunBranch.NumberOfArguments);
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

    private static IEnumerable<string> UsedFunctionNames(SymbolicExpressionTree symbolicExpressionTree) {
      return from node in symbolicExpressionTree.IterateNodesPrefix()
             where node.Symbol is Defun
             select ((DefunTreeNode)node).FunctionName;
    }
  }
}
