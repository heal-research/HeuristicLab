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

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Manipulators {
  [StorableClass]
  [Item("ChangeNodeTypeManipulation", "Selects a random tree node and changes the symbol.")]
  public sealed class ChangeNodeTypeManipulation : SymbolicExpressionTreeManipulator {

    [StorableConstructor]
    private ChangeNodeTypeManipulation(bool deserializing) : base(deserializing) { }
    private ChangeNodeTypeManipulation(ChangeNodeTypeManipulation original, Cloner cloner) : base(original, cloner) { }
    public ChangeNodeTypeManipulation() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ChangeNodeTypeManipulation(this, cloner);
    }

    protected override void Manipulate(IRandom random, SymbolicExpressionTree symbolicExpressionTree, ISymbolicExpressionGrammar grammar, IntValue maxTreeSize, IntValue maxTreeHeight, out bool success) {
      ChangeNodeType(random, symbolicExpressionTree, grammar, maxTreeSize.Value, maxTreeHeight.Value, out success);
    }

    public static void ChangeNodeType(IRandom random, SymbolicExpressionTree symbolicExpressionTree, ISymbolicExpressionGrammar grammar, int maxTreeSize, int maxTreeHeight, out bool success) {

      // select any node as parent (except the root node)
      var manipulationPoint = (from parent in symbolicExpressionTree.Root.IterateNodesPrefix().Skip(1)
                               from subtree in parent.SubTrees
                               select new { Parent = parent, Node = subtree, Index = parent.SubTrees.IndexOf(subtree) }).SelectRandom(random);
      // find possible symbols for the node (also considering the existing branches below it)
      var allowedSymbols = from symbol in manipulationPoint.Parent.GetAllowedSymbols(manipulationPoint.Index)
                           where manipulationPoint.Node.SubTrees.Count <= manipulationPoint.Node.Grammar.GetMaxSubtreeCount(symbol)
                           where manipulationPoint.Node.SubTrees.Count >= manipulationPoint.Node.Grammar.GetMinSubtreeCount(symbol)
                           select symbol;

      if (allowedSymbols.Count() == 0) {
        success = false;
        return;
      }
      var node = manipulationPoint.Node;
      // keep only symbols that are still possible considering the existing sub-trees
      var constrainedSymbols = from symbol in allowedSymbols
                               let disallowedSubtrees =
                                     from subtree in node.SubTrees
                                     where !node.Grammar.IsAllowedChild(symbol, subtree.Symbol, node.SubTrees.IndexOf(subtree))
                                     select subtree
                               where disallowedSubtrees.Count() == 0
                               select symbol;
      if (constrainedSymbols.Count() == 0) {
        success = false;
        return;
      }
      var newSymbol = SelectRandomSymbol(random, constrainedSymbols);

      // replace the old node with the new node
      var newNode = newSymbol.CreateTreeNode();
      if (newNode.HasLocalParameters)
        newNode.ResetLocalParameters(random);
      foreach (var subtree in node.SubTrees)
        newNode.AddSubTree(subtree);
      manipulationPoint.Parent.RemoveSubTree(manipulationPoint.Index);
      manipulationPoint.Parent.InsertSubTree(manipulationPoint.Index, newNode);
      success = true;
    }

    private static Symbol SelectRandomSymbol(IRandom random, IEnumerable<Symbol> symbols) {
      var symbolList = symbols.ToList();
      var ticketsSum = symbolList.Select(x => x.InitialFrequency).Sum();
      if (ticketsSum == 0.0) throw new ArgumentException("The initial frequency of all allowed symbols is zero.");
      var r = random.NextDouble() * ticketsSum;
      double aggregatedTickets = 0;
      for (int i = 0; i < symbolList.Count; i++) {
        aggregatedTickets += symbolList[i].InitialFrequency;
        if (aggregatedTickets > r) {
          return symbolList[i];
        }
      }
      // this should never happen
      throw new ArgumentException("There is a problem with the initial frequency setting of allowed symbols.");
    }
  }
}
