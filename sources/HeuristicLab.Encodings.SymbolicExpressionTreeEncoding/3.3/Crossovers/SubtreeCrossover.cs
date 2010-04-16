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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Data;
using System.Linq;
using System;
using HeuristicLab.Parameters;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.GeneralSymbols;
namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {


  /// <summary>
  /// Takes two parent individuals P0 and P1 each. Selects a random node N0 of P0 and a random node N1 of P1.
  /// And replaces the branch with root0 N0 in P0 with N1 from P1 if the tree-size limits are not violated.
  /// When recombination with N0 and N1 would create a tree that is too large or invalid the operator randomly selects new N0 and N1 
  /// until a valid configuration is found.
  /// </summary>  
  [Item("SubtreeCrossover", "An operator which performs subtree swapping crossover.")]
  [StorableClass]
  public class SubtreeCrossover : SymbolicExpressionTreeCrossover {
    public IValueLookupParameter<PercentValue> InternalCrossoverPointProbabilityParameter {
      get { return (IValueLookupParameter<PercentValue>)Parameters["InternalCrossoverPointProbability"]; }
    }

    public SubtreeCrossover()
      : base() {
      Parameters.Add(new ValueLookupParameter<PercentValue>("InternalCrossoverPointProbability", "The probability to select an internal crossover point (instead of a leaf node).", new PercentValue(0.9)));
    }

    protected override SymbolicExpressionTree Cross(IRandom random,
      SymbolicExpressionTree parent0, SymbolicExpressionTree parent1,
      IntValue maxTreeSize, IntValue maxTreeHeight, out bool success) {
      return Cross(random, parent0, parent1, InternalCrossoverPointProbabilityParameter.ActualValue.Value, maxTreeSize.Value, maxTreeHeight.Value, out success);
    }

    public static SymbolicExpressionTree Cross(IRandom random,
      SymbolicExpressionTree parent0, SymbolicExpressionTree parent1,
      double internalCrossoverPointProbability, int maxTreeSize, int maxTreeHeight, out bool success) {
      // select a random crossover point in the first parent 
      SymbolicExpressionTreeNode crossoverPoint0;
      int replacedSubtreeIndex;
      SelectCrossoverPoint(random, parent0, internalCrossoverPointProbability, maxTreeSize - 1, maxTreeHeight - 1, out crossoverPoint0, out replacedSubtreeIndex);

      // calculate the max size and height that the inserted branch can have 
      int maxInsertedBranchSize = maxTreeSize - (parent0.Size - crossoverPoint0.SubTrees[replacedSubtreeIndex].GetSize());
      int maxInsertedBranchHeight = maxTreeHeight - GetBranchLevel(parent0.Root, crossoverPoint0);

      var allowedBranches = (from branch in parent1.Root.IterateNodesPrefix()
                             where branch.GetSize() < maxInsertedBranchSize
                             where branch.GetHeight() < maxInsertedBranchHeight
                             where IsMatchingPointType(crossoverPoint0, replacedSubtreeIndex, branch)
                             select branch).ToList();

      if (allowedBranches.Count() == 0) {
        success = false;
        return parent0;
      } else {
        var selectedBranch = SelectRandomBranch(random, allowedBranches, internalCrossoverPointProbability);

        // manipulate the tree of parent0 in place
        // replace the branch in tree0 with the selected branch from tree1
        crossoverPoint0.RemoveSubTree(replacedSubtreeIndex);
        crossoverPoint0.InsertSubTree(replacedSubtreeIndex, selectedBranch);
        success = true;
        return parent0;
      }
    }

    private static bool IsMatchingPointType(SymbolicExpressionTreeNode parent, int replacedSubtreeIndex, SymbolicExpressionTreeNode branch) {
      // check point type for the whole branch
      foreach (var node in branch.IterateNodesPostfix()) {
        if (!parent.Grammar.ContainsSymbol(node.Symbol)) return false;
        else if (node.SubTrees.Count < parent.Grammar.GetMinSubtreeCount(node.Symbol)) return false;
        else if (node.SubTrees.Count > parent.Grammar.GetMaxSubtreeCount(node.Symbol)) return false;
      }

      // check syntax constraints of direct parent - child relation
      if (!parent.Grammar.IsAllowedChild(parent.Symbol, branch.Symbol, replacedSubtreeIndex)) return false;

      return true;
    }

    private static void SelectCrossoverPoint(IRandom random, SymbolicExpressionTree parent0, double internalNodeProbability, int maxBranchSize, int maxBranchHeight, out SymbolicExpressionTreeNode crossoverPoint, out int subtreeIndex) {
      var crossoverPoints = from branch in parent0.Root.IterateNodesPrefix()
                            where branch.SubTrees.Count > 0
                            where branch != parent0.Root
                            where branch.GetSize() < maxBranchSize
                            where branch.GetHeight() < maxBranchHeight
                            from index in Enumerable.Range(0, branch.SubTrees.Count)
                            let p = new { CrossoverPoint = branch, SubtreeIndex = index, IsLeaf = branch.SubTrees[index].SubTrees.Count == 0 }
                            select p;
      var internalCrossoverPoints = (from p in crossoverPoints
                                     where !p.IsLeaf
                                     select p).ToList();
      var leafCrossoverPoints = (from p in crossoverPoints
                                 where p.IsLeaf
                                 select p).ToList();
      if (internalCrossoverPoints.Count == 0) {
        var selectedCrossoverPoint = leafCrossoverPoints[random.Next(leafCrossoverPoints.Count)];
        crossoverPoint = selectedCrossoverPoint.CrossoverPoint;
        subtreeIndex = selectedCrossoverPoint.SubtreeIndex;
      } else if (leafCrossoverPoints.Count == 0) {
        var selectedCrossoverPoint = internalCrossoverPoints[random.Next(internalCrossoverPoints.Count)];
        crossoverPoint = selectedCrossoverPoint.CrossoverPoint;
        subtreeIndex = selectedCrossoverPoint.SubtreeIndex;
      } else if (random.NextDouble() < internalNodeProbability && internalCrossoverPoints.Count > 0) {
        // select internal crossover point or leaf
        var selectedCrossoverPoint = internalCrossoverPoints[random.Next(internalCrossoverPoints.Count)];
        crossoverPoint = selectedCrossoverPoint.CrossoverPoint;
        subtreeIndex = selectedCrossoverPoint.SubtreeIndex;
      } else {
        var selectedCrossoverPoint = leafCrossoverPoints[random.Next(leafCrossoverPoints.Count)];
        crossoverPoint = selectedCrossoverPoint.CrossoverPoint;
        subtreeIndex = selectedCrossoverPoint.SubtreeIndex;
      }
    }

    private static SymbolicExpressionTreeNode SelectRandomBranch(IRandom random, IEnumerable<SymbolicExpressionTreeNode> branches, double internalNodeProbability) {
      if (internalNodeProbability < 0.0 || internalNodeProbability > 1.0) throw new ArgumentException("internalNodeProbability");
      var groupedBranches = (from branch in branches
                             group branch by branch.SubTrees.Count into g
                             select g).ToList();
      var allowedInternalBranches = (from g in groupedBranches
                                     where g.Key > 0
                                     from branch in g
                                     select branch).ToList();
      var allowedLeafBranches = (from g in groupedBranches
                                 where g.Key == 0
                                 from leaf in g
                                 select leaf).ToList();
      if (allowedInternalBranches.Count == 0) {
        return allowedLeafBranches.SelectRandom(random);
      } else if (allowedLeafBranches.Count == 0) {
        return allowedInternalBranches.SelectRandom(random);
      } else if (random.NextDouble() < internalNodeProbability) {
        // when leaf and internal nodes are possible then choose either a leaf or internal node with internalNodeProbability
        return allowedInternalBranches.SelectRandom(random);
      } else {
        return allowedLeafBranches.SelectRandom(random);
      }
    }

    private static int GetBranchLevel(SymbolicExpressionTreeNode root, SymbolicExpressionTreeNode point) {
      if (root == point) return 0;
      foreach (var subtree in root.SubTrees) {
        int branchLevel = GetBranchLevel(subtree, point);
        if (branchLevel < int.MaxValue) return 1 + branchLevel;
      }
      return int.MaxValue;
    }
  }
}
