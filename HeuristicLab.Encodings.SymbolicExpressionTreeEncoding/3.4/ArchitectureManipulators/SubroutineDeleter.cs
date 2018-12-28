#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Fossil;
using HeuristicLab.Random;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  /// <summary>
  /// Manipulates a symbolic expression by deleting a preexisting function-defining branch.
  /// As described in Koza, Bennett, Andre, Keane, Genetic Programming III - Darwinian Invention and Problem Solving, 1999, pp. 108
  /// </summary>
  [Item("SubroutineDeleter", "Manipulates a symbolic expression by deleting a preexisting function-defining branch. As described in Koza, Bennett, Andre, Keane, Genetic Programming III - Darwinian Invention and Problem Solving, 1999, pp. 108")]
  [StorableType("A4AB4A79-F013-4DF4-847D-5F2C171859A7")]
  public sealed class SubroutineDeleter : SymbolicExpressionTreeArchitectureManipulator {
    [StorableConstructor]
    private SubroutineDeleter(StorableConstructorFlag _) : base(_) { }
    private SubroutineDeleter(SubroutineDeleter original, Cloner cloner) : base(original, cloner) { }
    public SubroutineDeleter() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SubroutineDeleter(this, cloner);
    }

    public override sealed void ModifyArchitecture(
      IRandom random,
      ISymbolicExpressionTree symbolicExpressionTree,
      IntValue maxFunctionDefinitions, IntValue maxFunctionArguments) {
      DeleteSubroutine(random, symbolicExpressionTree, maxFunctionDefinitions.Value, maxFunctionArguments.Value);
    }

    public static bool DeleteSubroutine(
      IRandom random,
      ISymbolicExpressionTree symbolicExpressionTree,
      int maxFunctionDefinitions, int maxFunctionArguments) {
      var functionDefiningBranches = symbolicExpressionTree.IterateNodesPrefix().OfType<DefunTreeNode>().ToList();

      if (!functionDefiningBranches.Any())
        // no ADF to delete => abort
        return false;

      var selectedDefunBranch = functionDefiningBranches.SampleRandom(random);
      // remove the selected defun
      int defunSubtreeIndex = symbolicExpressionTree.Root.IndexOfSubtree(selectedDefunBranch);
      symbolicExpressionTree.Root.RemoveSubtree(defunSubtreeIndex);

      // remove references to deleted function
      foreach (var subtree in symbolicExpressionTree.Root.Subtrees.OfType<SymbolicExpressionTreeTopLevelNode>()) {
        var matchingInvokeSymbol = (from symb in subtree.Grammar.Symbols.OfType<InvokeFunction>()
                                    where symb.FunctionName == selectedDefunBranch.FunctionName
                                    select symb).SingleOrDefault();
        if (matchingInvokeSymbol != null) {
          subtree.Grammar.RemoveSymbol(matchingInvokeSymbol);
        }
      }

      DeletionByRandomRegeneration(random, symbolicExpressionTree, selectedDefunBranch);
      return true;
    }

    private static void DeletionByRandomRegeneration(IRandom random, ISymbolicExpressionTree symbolicExpressionTree, DefunTreeNode selectedDefunBranch) {
      // find first invocation and replace it with a randomly generated tree
      // can't find all invocations in one step because once we replaced a top level invocation
      // the invocations below it are removed already
      var invocationCutPoint = (from node in symbolicExpressionTree.IterateNodesPrefix()
                                from subtree in node.Subtrees.OfType<InvokeFunctionTreeNode>()
                                where subtree.Symbol.FunctionName == selectedDefunBranch.FunctionName
                                select new CutPoint(node, subtree)).FirstOrDefault();
      while (invocationCutPoint != null) {
        // deletion by random regeneration
        ISymbolicExpressionTreeNode replacementTree = null;
        var allowedSymbolsList = invocationCutPoint.Parent.Grammar.GetAllowedChildSymbols(invocationCutPoint.Parent.Symbol, invocationCutPoint.ChildIndex).ToList();
        var weights = allowedSymbolsList.Select(s => s.InitialFrequency);

#pragma warning disable 612, 618
        var selectedSymbol = allowedSymbolsList.SelectRandom(weights, random);
#pragma warning restore 612, 618


        int minPossibleLength = invocationCutPoint.Parent.Grammar.GetMinimumExpressionLength(selectedSymbol);
        int maxLength = Math.Max(minPossibleLength, invocationCutPoint.Child.GetLength());
        int minPossibleDepth = invocationCutPoint.Parent.Grammar.GetMinimumExpressionDepth(selectedSymbol);
        int maxDepth = Math.Max(minPossibleDepth, invocationCutPoint.Child.GetDepth());
        replacementTree = selectedSymbol.CreateTreeNode();
        if (replacementTree.HasLocalParameters)
          replacementTree.ResetLocalParameters(random);
        invocationCutPoint.Parent.RemoveSubtree(invocationCutPoint.ChildIndex);
        invocationCutPoint.Parent.InsertSubtree(invocationCutPoint.ChildIndex, replacementTree);

        ProbabilisticTreeCreator.PTC2(random, replacementTree, maxLength, maxDepth);

        invocationCutPoint = (from node in symbolicExpressionTree.IterateNodesPrefix()
                              from subtree in node.Subtrees.OfType<InvokeFunctionTreeNode>()
                              where subtree.Symbol.FunctionName == selectedDefunBranch.FunctionName
                              select new CutPoint(node, subtree)).FirstOrDefault();
      }
    }
  }
}
