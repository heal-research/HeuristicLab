#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  [Item("RemoveBranchManipulation", "Removes a random sub-tree of the input tree and fixes the tree by generating random subtrees if necessary..")]
  public sealed class RemoveBranchManipulation : SymbolicExpressionTreeManipulator, ISymbolicExpressionTreeSizeConstraintOperator {
    private const int MAX_TRIES = 100;
    private const string MaximumSymbolicExpressionTreeLengthParameterName = "MaximumSymbolicExpressionTreeLength";
    private const string MaximumSymbolicExpressionTreeDepthParameterName = "MaximumSymbolicExpressionTreeDepth";
    #region Parameter Properties
    public IValueLookupParameter<IntValue> MaximumSymbolicExpressionTreeLengthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeLengthParameterName]; }
    }
    public IValueLookupParameter<IntValue> MaximumSymbolicExpressionTreeDepthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeDepthParameterName]; }
    }
    #endregion
    #region Properties
    public IntValue MaximumSymbolicExpressionTreeLength {
      get { return MaximumSymbolicExpressionTreeLengthParameter.ActualValue; }
    }
    public IntValue MaximumSymbolicExpressionTreeDepth {
      get { return MaximumSymbolicExpressionTreeDepthParameter.ActualValue; }
    }
    #endregion

    [StorableConstructor]
    private RemoveBranchManipulation(bool deserializing) : base(deserializing) { }
    private RemoveBranchManipulation(RemoveBranchManipulation original, Cloner cloner) : base(original, cloner) { }
    public RemoveBranchManipulation()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumSymbolicExpressionTreeLengthParameterName, "The maximal length (number of nodes) of the symbolic expression tree."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumSymbolicExpressionTreeDepthParameterName, "The maximal depth of the symbolic expression tree (a tree with one node has depth = 0)."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RemoveBranchManipulation(this, cloner);
    }

    protected override void Manipulate(IRandom random, ISymbolicExpressionTree symbolicExpressionTree) {
      RemoveRandomBranch(random, symbolicExpressionTree, MaximumSymbolicExpressionTreeLength.Value, MaximumSymbolicExpressionTreeDepth.Value);
    }

    public static void RemoveRandomBranch(IRandom random, ISymbolicExpressionTree symbolicExpressionTree, int maxTreeLength, int maxTreeDepth) {
      var allowedSymbols = new List<ISymbol>();
      ISymbolicExpressionTreeNode parent;
      int childIndex;
      int maxLength;
      int maxDepth;
      // repeat until a fitting parent and child are found (MAX_TRIES times)
      int tries = 0;

      var nodes = symbolicExpressionTree.Root.IterateNodesPrefix().Skip(1).Where(n => n.SubtreeCount > 0).ToList();
      do {
        parent = nodes.SelectRandom(random);
        childIndex = random.Next(parent.SubtreeCount);
        var child = parent.GetSubtree(childIndex);
        maxLength = maxTreeLength - symbolicExpressionTree.Length + child.GetLength();
        maxDepth = maxTreeDepth - symbolicExpressionTree.Depth + child.GetDepth();

        allowedSymbols.Clear();
        foreach (var symbol in parent.Grammar.GetAllowedChildSymbols(parent.Symbol, childIndex)) {
          // check basic properties that the new symbol must have
          if (symbol.Name != child.Symbol.Name &&
            symbol.InitialFrequency > 0 &&
            parent.Grammar.GetMinimumExpressionDepth(symbol) + 1 <= maxDepth &&
            parent.Grammar.GetMinimumExpressionLength(symbol) <= maxLength) {
            allowedSymbols.Add(symbol);
          }
        }
        tries++;
      } while (tries < MAX_TRIES && allowedSymbols.Count == 0);

      if (tries >= MAX_TRIES) return;
      ReplaceWithMinimalTree(random, symbolicExpressionTree.Root, parent, childIndex);
    }

    private static void ReplaceWithMinimalTree(IRandom random, ISymbolicExpressionTreeNode root, ISymbolicExpressionTreeNode parent, int childIndex) {
      // determine possible symbols that will lead to the smallest possible tree
      var possibleSymbols = (from s in parent.Grammar.GetAllowedChildSymbols(parent.Symbol, childIndex)
                             where s.InitialFrequency > 0.0
                             group s by parent.Grammar.GetMinimumExpressionLength(s) into g
                             orderby g.Key
                             select g).First().ToList();
      var weights = possibleSymbols.Select(x => x.InitialFrequency).ToList();
      var selectedSymbol = possibleSymbols.SelectRandom(weights, random);
      var newTreeNode = selectedSymbol.CreateTreeNode();
      if (newTreeNode.HasLocalParameters) newTreeNode.ResetLocalParameters(random);
      parent.RemoveSubtree(childIndex);
      parent.InsertSubtree(childIndex, newTreeNode);

      var topLevelNode = newTreeNode as SymbolicExpressionTreeTopLevelNode;
      if (topLevelNode != null)
        topLevelNode.SetGrammar((ISymbolicExpressionTreeGrammar)root.Grammar.Clone());

      for (int i = 0; i < newTreeNode.Grammar.GetMinimumSubtreeCount(newTreeNode.Symbol); i++) {
        // insert a dummy sub-tree and add the pending extension to the list
        var dummy = new SymbolicExpressionTreeNode();
        newTreeNode.AddSubtree(dummy);
        // replace the just inserted dummy by recursive application
        ReplaceWithMinimalTree(random, root, newTreeNode, i);
      }
    }
  }
}
