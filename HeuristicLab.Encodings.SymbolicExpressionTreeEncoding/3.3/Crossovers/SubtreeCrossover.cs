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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Crossovers {
  /// <summary>
  /// Takes two parent individuals P0 and P1 each. Selects a random node N0 of P0 and a random node N1 of P1.
  /// And replaces the branch with root0 N0 in P0 with N1 from P1 if the tree-size limits are not violated.
  /// When recombination with N0 and N1 would create a tree that is too large or invalid the operator randomly selects new N0 and N1 
  /// until a valid configuration is found.
  /// </summary>  
  [Item("SubtreeCrossover", "An operator which performs subtree swapping crossover.")]
  [StorableClass]
  public sealed class SubtreeCrossover : SymbolicExpressionTreeCrossover {
    public IValueLookupParameter<PercentValue> InternalCrossoverPointProbabilityParameter {
      get { return (IValueLookupParameter<PercentValue>)Parameters["InternalCrossoverPointProbability"]; }
    }
    [StorableConstructor]
    private SubtreeCrossover(bool deserializing) : base(deserializing) { }
    private SubtreeCrossover(SubtreeCrossover original, Cloner cloner) : base(original, cloner) { }
    public SubtreeCrossover()
      : base() {
      Parameters.Add(new ValueLookupParameter<PercentValue>("InternalCrossoverPointProbability", "The probability to select an internal crossover point (instead of a leaf node).", new PercentValue(0.9)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SubtreeCrossover(this, cloner);
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
      SelectCrossoverPoint(random, parent0, internalCrossoverPointProbability, maxTreeSize, maxTreeHeight, out crossoverPoint0, out replacedSubtreeIndex);

      // calculate the max size and height that the inserted branch can have 
      int maxInsertedBranchSize = maxTreeSize - (parent0.Size - crossoverPoint0.SubTrees[replacedSubtreeIndex].GetSize());
      int maxInsertedBranchHeight = maxTreeHeight - GetBranchLevel(parent0.Root, crossoverPoint0);

      List<SymbolicExpressionTreeNode> allowedBranches = new List<SymbolicExpressionTreeNode>();
      parent1.Root.ForEachNodePostfix((n) => {
        if (n.GetSize() <= maxInsertedBranchSize &&
          n.GetHeight() <= maxInsertedBranchHeight &&
          IsMatchingPointType(crossoverPoint0, replacedSubtreeIndex, n))
          allowedBranches.Add(n);
      });

      if (allowedBranches.Count == 0) {
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
      // check syntax constraints of direct parent - child relation
      if (!parent.Grammar.ContainsSymbol(branch.Symbol) ||
          !parent.Grammar.IsAllowedChild(parent.Symbol, branch.Symbol, replacedSubtreeIndex)) return false;

      bool result = true;
      // check point type for the whole branch
      branch.ForEachNodePostfix((n) => {
        result =
          result &&
          parent.Grammar.ContainsSymbol(n.Symbol) &&
          n.SubTrees.Count >= parent.Grammar.GetMinSubtreeCount(n.Symbol) &&
          n.SubTrees.Count <= parent.Grammar.GetMaxSubtreeCount(n.Symbol);
      });
      return result;
    }

    private static void SelectCrossoverPoint(IRandom random, SymbolicExpressionTree parent0, double internalNodeProbability, int maxBranchSize, int maxBranchHeight, out SymbolicExpressionTreeNode crossoverPoint, out int subtreeIndex) {
      if (internalNodeProbability < 0.0 || internalNodeProbability > 1.0) throw new ArgumentException("internalNodeProbability");
      List<CrossoverPoint> internalCrossoverPoints = new List<CrossoverPoint>();
      List<CrossoverPoint> leafCrossoverPoints = new List<CrossoverPoint>();
      parent0.Root.ForEachNodePostfix((n) => {
        if (n.SubTrees.Count > 0 && n != parent0.Root) {
          foreach (var child in n.SubTrees) {
            if (child.GetSize() <= maxBranchSize &&
                child.GetHeight() <= maxBranchHeight) {
              if (child.SubTrees.Count > 0)
                internalCrossoverPoints.Add(new CrossoverPoint(n, child));
              else
                leafCrossoverPoints.Add(new CrossoverPoint(n, child));
            }
          }
        }
      });

      if (random.NextDouble() < internalNodeProbability) {
        // select from internal node if possible
        if (internalCrossoverPoints.Count > 0) {
          // select internal crossover point or leaf
          var selectedCrossoverPoint = internalCrossoverPoints[random.Next(internalCrossoverPoints.Count)];
          crossoverPoint = selectedCrossoverPoint.Parent;
          subtreeIndex = selectedCrossoverPoint.SubtreeIndex;
        } else {
          // otherwise select external node
          var selectedCrossoverPoint = leafCrossoverPoints[random.Next(leafCrossoverPoints.Count)];
          crossoverPoint = selectedCrossoverPoint.Parent;
          subtreeIndex = selectedCrossoverPoint.SubtreeIndex;
        }
      } else if (leafCrossoverPoints.Count > 0) {
        // select from leaf crossover point if possible
        var selectedCrossoverPoint = leafCrossoverPoints[random.Next(leafCrossoverPoints.Count)];
        crossoverPoint = selectedCrossoverPoint.Parent;
        subtreeIndex = selectedCrossoverPoint.SubtreeIndex;
      } else {
        // otherwise select internal crossover point
        var selectedCrossoverPoint = internalCrossoverPoints[random.Next(internalCrossoverPoints.Count)];
        crossoverPoint = selectedCrossoverPoint.Parent;
        subtreeIndex = selectedCrossoverPoint.SubtreeIndex;
      }
    }

    private static SymbolicExpressionTreeNode SelectRandomBranch(IRandom random, IEnumerable<SymbolicExpressionTreeNode> branches, double internalNodeProbability) {
      if (internalNodeProbability < 0.0 || internalNodeProbability > 1.0) throw new ArgumentException("internalNodeProbability");
      List<SymbolicExpressionTreeNode> allowedInternalBranches;
      List<SymbolicExpressionTreeNode> allowedLeafBranches;
      if (random.NextDouble() < internalNodeProbability) {
        // select internal node if possible
        allowedInternalBranches = (from branch in branches
                                   where branch.SubTrees.Count > 0
                                   select branch).ToList();
        if (allowedInternalBranches.Count > 0) {
          return allowedInternalBranches.SelectRandom(random);
        } else {
          // no internal nodes allowed => select leaf nodes
          allowedLeafBranches = (from branch in branches
                                 where branch.SubTrees.Count == 0
                                 select branch).ToList();
          return allowedLeafBranches.SelectRandom(random);
        }
      } else {
        // select leaf node if possible
        allowedLeafBranches = (from branch in branches
                               where branch.SubTrees.Count == 0
                               select branch).ToList();
        if (allowedLeafBranches.Count > 0) {
          return allowedLeafBranches.SelectRandom(random);
        } else {
          allowedInternalBranches = (from branch in branches
                                     where branch.SubTrees.Count > 0
                                     select branch).ToList();
          return allowedInternalBranches.SelectRandom(random);
        }
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
