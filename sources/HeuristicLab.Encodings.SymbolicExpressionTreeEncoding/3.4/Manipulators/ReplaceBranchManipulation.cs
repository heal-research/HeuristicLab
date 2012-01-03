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
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  [Item("ReplaceBranchManipulation", "Selects a branch of the tree randomly and replaces it with a newly initialized branch (using PTC2).")]
  public sealed class ReplaceBranchManipulation : SymbolicExpressionTreeManipulator, ISymbolicExpressionTreeSizeConstraintOperator {
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
    private ReplaceBranchManipulation(bool deserializing) : base(deserializing) { }
    private ReplaceBranchManipulation(ReplaceBranchManipulation original, Cloner cloner) : base(original, cloner) { }
    public ReplaceBranchManipulation()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumSymbolicExpressionTreeLengthParameterName, "The maximal length (number of nodes) of the symbolic expression tree."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumSymbolicExpressionTreeDepthParameterName, "The maximal depth of the symbolic expression tree (a tree with one node has depth = 0)."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ReplaceBranchManipulation(this, cloner);
    }

    protected override void Manipulate(IRandom random, ISymbolicExpressionTree symbolicExpressionTree) {
      ReplaceRandomBranch(random, symbolicExpressionTree, MaximumSymbolicExpressionTreeLength.Value, MaximumSymbolicExpressionTreeDepth.Value);
    }

    public static void ReplaceRandomBranch(IRandom random, ISymbolicExpressionTree symbolicExpressionTree, int maxTreeLength, int maxTreeDepth) {
      // select any node as parent (except the root node)
      var manipulationPoints = (from parent in symbolicExpressionTree.Root.IterateNodesPrefix().Skip(1)
                                from subtree in parent.Subtrees
                                let subtreeIndex = parent.IndexOfSubtree(subtree)
                                let maxLength = maxTreeLength - symbolicExpressionTree.Length + subtree.GetLength()
                                let maxDepth = maxTreeDepth - symbolicExpressionTree.Depth + subtree.GetDepth()
                                // find possible symbols for the node (also considering the existing branches below it)
                                let allowedSymbols = (from symbol in parent.Grammar.GetAllowedChildSymbols(parent.Symbol, subtreeIndex)
                                                      // do not replace symbol with the same symbol
                                                      where symbol.Name != subtree.Symbol.Name
                                                      where symbol.InitialFrequency > 0
                                                      where parent.Grammar.GetMinimumExpressionDepth(symbol) + 1 <= maxDepth
                                                      where parent.Grammar.GetMinimumExpressionLength(symbol) <= maxLength
                                                      select symbol)
                                                      .ToList()
                                where allowedSymbols.Count > 0
                                select new {
                                  Parent = parent,
                                  Child = subtree,
                                  Index = subtreeIndex,
                                  AllowedSymbols = allowedSymbols,
                                  MaxLength = maxLength,
                                  MaxDepth = maxDepth
                                })
                               .ToList();

      if (manipulationPoints.Count == 0) return;
      var selectedManipulationPoint = manipulationPoints.SelectRandom(random);

      var weights = selectedManipulationPoint.AllowedSymbols.Select(s => s.InitialFrequency).ToList();
      var seedSymbol = selectedManipulationPoint.AllowedSymbols.SelectRandom(weights, random);
      // replace the old node with the new node
      var seedNode = seedSymbol.CreateTreeNode();
      if (seedNode.HasLocalParameters)
        seedNode.ResetLocalParameters(random);

      selectedManipulationPoint.Parent.RemoveSubtree(selectedManipulationPoint.Index);
      selectedManipulationPoint.Parent.InsertSubtree(selectedManipulationPoint.Index, seedNode);
      ProbabilisticTreeCreator.PTC2(random, seedNode, selectedManipulationPoint.MaxLength, selectedManipulationPoint.MaxDepth);
    }
  }
}
