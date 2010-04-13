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
  /// Manipulates a symbolic expression by duplicating a preexisting function-defining branch.
  /// As described in Koza, Bennett, Andre, Keane, Genetic Programming III - Darwinian Invention and Problem Solving, 1999, pp. 88
  /// </summary>
  [Item("SubroutineDuplicater", "Manipulates a symbolic expression by duplicating a preexisting function-defining branch.")]
  [StorableClass]
  public sealed class SubroutineDuplicater : SymbolicExpressionTreeArchitectureAlteringOperator {
    public override sealed void ModifyArchitecture(
      IRandom random,
      SymbolicExpressionTree symbolicExpressionTree,
      ISymbolicExpressionGrammar grammar,
      IntValue maxTreeSize, IntValue maxTreeHeight,
      IntValue maxFunctionDefiningBranches, IntValue maxFunctionArguments,
      out bool success) {
      success = DuplicateRandomSubroutine(random, symbolicExpressionTree, grammar, maxTreeSize.Value, maxTreeHeight.Value, maxFunctionDefiningBranches.Value, maxFunctionArguments.Value);
    }

    public static bool DuplicateRandomSubroutine(
      IRandom random,
      SymbolicExpressionTree symbolicExpressionTree,
      ISymbolicExpressionGrammar grammar,
      int maxTreeSize, int maxTreeHeight,
      int maxFunctionDefiningBranches, int maxFunctionArguments) {
      var functionDefiningBranches = symbolicExpressionTree.IterateNodesPrefix().OfType<DefunTreeNode>();

      string formatString = new StringBuilder().Append('0', (int)Math.Log10(maxFunctionDefiningBranches) + 1).ToString(); // >= 100 functions => ###
      var allowedFunctionNames = from index in Enumerable.Range(0, maxFunctionDefiningBranches)
                                 select "ADF" + index.ToString(formatString);
      if (functionDefiningBranches.Count() == 0 || functionDefiningBranches.Count() == maxFunctionDefiningBranches)
        // no function defining branches to duplicate or already reached the max number of ADFs
        return false;
      var selectedBranch = functionDefiningBranches.SelectRandom(random);
      var clonedBranch = (DefunTreeNode)selectedBranch.Clone();
      clonedBranch.Name = allowedFunctionNames.Except(UsedFunctionNames(symbolicExpressionTree)).First();
      foreach (var node in symbolicExpressionTree.IterateNodesPrefix()) {
        var invokeFunctionNode = node as InvokeFunctionTreeNode;
        // find all invokations of the old function 
        if (invokeFunctionNode != null && invokeFunctionNode.InvokedFunctionName == selectedBranch.Name) {
          // add the new function name to the list of known functions in the branches that used the originating function
          var branch = symbolicExpressionTree.GetTopLevelBranchOf(invokeFunctionNode);
          branch.AddDynamicSymbol(clonedBranch.Name, clonedBranch.NumberOfArguments);
          // flip coin wether to replace with newly defined function
          if (random.NextDouble() < 0.5) {
            invokeFunctionNode.InvokedFunctionName = clonedBranch.Name;
          }
        }
      }
      Debug.Assert(grammar.IsValidExpression(symbolicExpressionTree));
      return true;
    }

    private static bool ContainsNode(SymbolicExpressionTreeNode branch, SymbolicExpressionTreeNode node) {
      if (branch == node) return true;
      else foreach (var subtree in branch.SubTrees) {
          if (ContainsNode(subtree, node)) return true;
        }
      return false;
    }

    private static IEnumerable<string> UsedFunctionNames(SymbolicExpressionTree symbolicExpressionTree) {
      return from node in symbolicExpressionTree.IterateNodesPrefix()
             where node.Symbol is Defun
             select ((DefunTreeNode)node).Name;
    }


  }
}
