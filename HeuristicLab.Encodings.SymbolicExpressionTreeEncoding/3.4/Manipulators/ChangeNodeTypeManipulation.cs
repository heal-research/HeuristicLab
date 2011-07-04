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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
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

    protected override void Manipulate(IRandom random, ISymbolicExpressionTree symbolicExpressionTree) {
      ChangeNodeType(random, symbolicExpressionTree);
    }

    public static void ChangeNodeType(IRandom random, ISymbolicExpressionTree symbolicExpressionTree) {
      // select any node as parent (except the root node)
      var manipulationPoints = (from parent in symbolicExpressionTree.Root.IterateNodesPrefix().Skip(1)
                                let subtreeCount = parent.Subtrees.Count()
                                from subtreeIndex in Enumerable.Range(0, subtreeCount)
                                let subtree = parent.GetSubtree(subtreeIndex)
                                let existingSubtreeCount = subtree.Subtrees.Count()
                                // find possible symbols for the node (also considering the existing branches below it)
                                let allowedSymbols = (from symbol in parent.Grammar.GetAllowedChildSymbols(parent.Symbol, subtreeIndex)
                                                      // do not replace the existing symbol with itself
                                                      where symbol.Name != subtree.Symbol.Name
                                                      where symbol.InitialFrequency > 0
                                                      where existingSubtreeCount <= parent.Grammar.GetMaximumSubtreeCount(symbol)
                                                      where existingSubtreeCount >= parent.Grammar.GetMinimumSubtreeCount(symbol)
                                                      // keep only symbols that are still possible considering the existing sub-trees
                                                      where (from existingSubtreeIndex in Enumerable.Range(0, existingSubtreeCount)
                                                             let existingSubtree = subtree.GetSubtree(existingSubtreeIndex)
                                                             select parent.Grammar.IsAllowedChildSymbol(symbol, existingSubtree.Symbol, existingSubtreeIndex))
                                                             .All(x => x == true)
                                                      select symbol)
                                                      .ToList()
                                where allowedSymbols.Count() > 0
                                select new { Parent = parent, Child = subtree, Index = subtreeIndex, AllowedSymbols = allowedSymbols })
                               .ToList();
      if (manipulationPoints.Count == 0) { return; }
      var selectedManipulationPoint = manipulationPoints.SelectRandom(random);

      var weights = selectedManipulationPoint.AllowedSymbols.Select(s => s.InitialFrequency).ToList();
      var newSymbol = selectedManipulationPoint.AllowedSymbols.SelectRandom(weights, random);

      // replace the old node with the new node
      var newNode = newSymbol.CreateTreeNode();
      if (newNode.HasLocalParameters)
        newNode.ResetLocalParameters(random);
      foreach (var subtree in selectedManipulationPoint.Child.Subtrees)
        newNode.AddSubtree(subtree);
      selectedManipulationPoint.Parent.RemoveSubtree(selectedManipulationPoint.Index);
      selectedManipulationPoint.Parent.InsertSubtree(selectedManipulationPoint.Index, newNode);
    }
  }
}
