#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  [Item("ChangeNodeTypeManipulation", "Selects a random tree node and changes the symbol.")]
  public sealed class ChangeNodeTypeManipulation : SymbolicExpressionTreeManipulator {
    private const int MAX_TRIES = 100;

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
      List<ISymbol> allowedSymbols = new List<ISymbol>();
      ISymbolicExpressionTreeNode parent;
      int childIndex;
      ISymbolicExpressionTreeNode child;
      // repeat until a fitting parent and child are found (MAX_TRIES times)
      int tries = 0;
      do {
        parent = symbolicExpressionTree.Root.IterateNodesPrefix().Skip(1).Where(n => n.SubtreeCount > 0).SelectRandom(random);
        childIndex = random.Next(parent.SubtreeCount);

        child = parent.GetSubtree(childIndex);
        int existingSubtreeCount = child.SubtreeCount;
        allowedSymbols.Clear();
        foreach (var symbol in parent.Grammar.GetAllowedChildSymbols(parent.Symbol, childIndex)) {
          // check basic properties that the new symbol must have
          if (symbol.Name != child.Symbol.Name &&
            symbol.InitialFrequency > 0 &&
            existingSubtreeCount <= parent.Grammar.GetMinimumSubtreeCount(symbol) &&
            existingSubtreeCount >= parent.Grammar.GetMaximumSubtreeCount(symbol)) {
            // check that all existing subtrees are also allowed for the new symbol
            bool allExistingSubtreesAllowed = true;
            for (int existingSubtreeIndex = 0; existingSubtreeIndex < existingSubtreeCount && allExistingSubtreesAllowed; existingSubtreeIndex++) {
              var existingSubtree = child.GetSubtree(existingSubtreeIndex);
              allExistingSubtreesAllowed &= parent.Grammar.IsAllowedChildSymbol(symbol, existingSubtree.Symbol, existingSubtreeIndex);
            }
            if (allExistingSubtreesAllowed) {
              allowedSymbols.Add(symbol);
            }
          }
        }
        tries++;
      } while (tries < MAX_TRIES && allowedSymbols.Count == 0);

      if (tries < MAX_TRIES) {
        var weights = allowedSymbols.Select(s => s.InitialFrequency).ToList();
        var newSymbol = allowedSymbols.SelectRandom(weights, random);

        // replace the old node with the new node
        var newNode = newSymbol.CreateTreeNode();
        if (newNode.HasLocalParameters)
          newNode.ResetLocalParameters(random);
        foreach (var subtree in child.Subtrees)
          newNode.AddSubtree(subtree);
        parent.RemoveSubtree(childIndex);
        parent.InsertSubtree(childIndex, newNode);
      }
    }
  }
}
