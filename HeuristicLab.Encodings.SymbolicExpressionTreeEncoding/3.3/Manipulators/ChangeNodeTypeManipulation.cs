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

using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Random;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Manipulators {
  [StorableClass]
  [Item("ChangeNodeTypeManipulation", "Selects a random tree node and changes the symbol size.")]
  public class ChangeNodeTypeManipulation : SymbolicExpressionTreeManipulator {

    public ChangeNodeTypeManipulation()
      : base() {
    }

    protected override void Manipulate(IRandom random, SymbolicExpressionTree symbolicExpressionTree, ISymbolicExpressionGrammar grammar, IntValue maxTreeSize, IntValue maxTreeHeight, out bool success) {

      // select any node except the with a parent where the parent is not the root node)
      var manipulationPoint = (from parent in symbolicExpressionTree.Root.IterateNodesPrefix().Skip(1)
                               from subtree in parent.SubTrees
                               select new { Parent = parent, Node = subtree, Index = parent.SubTrees.IndexOf(subtree) }).SelectRandom(random);
      // find possible symbols for the node (also considering the existing branches below it)
      var allowedSymbols = from symbol in manipulationPoint.Parent.GetAllowedSymbols(manipulationPoint.Index)
                           where manipulationPoint.Node.SubTrees.Count <= manipulationPoint.Node.Grammar.GetMaxSubtreeCount(symbol)
                           where manipulationPoint.Node.SubTrees.Count >= manipulationPoint.Node.Grammar.GetMinSubtreeCount(symbol)
                           select symbol;

      if (allowedSymbols.Count() <= 1) {
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
      if (constrainedSymbols.Count() <= 1) {
        success = false;
        return;
      }
      var newSymbol = constrainedSymbols.SelectRandom(random);

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
  }
}
