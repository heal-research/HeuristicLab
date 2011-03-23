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

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  /// <summary>
  /// Takes two parent individuals P0 and P1 each. Selects a random node N0 of P0 and a random node N1 of P1.
  /// And replaces the branch with root0 N0 in P0 with N1 from P1 if the tree-size limits are not violated.
  /// When recombination with N0 and N1 would create a tree that is too large or invalid the operator randomly selects new N0 and N1 
  /// until a valid configuration is found.
  /// </summary>  
  [Item("SubtreeCrossover", "An operator which performs subtree swapping crossover.")]
  [StorableClass]
  public sealed class SubtreeCrossover : SymbolicExpressionTreeCrossover, ISymbolicExpressionTreeSizeConstraintOperator {
    private const string InternalCrossoverPointProbabilityParameterName = "InternalCrossoverPointProbability";
    private const string MaximumSymbolicExpressionTreeLengthParameterName = "MaximumSymbolicExpressionTreeLength";
    private const string MaximumSymbolicExpressionTreeDepthParameterName = "MaximumSymbolicExpressionTreeDepth";
    #region Parameter Properties
    public IValueLookupParameter<PercentValue> InternalCrossoverPointProbabilityParameter {
      get { return (IValueLookupParameter<PercentValue>)Parameters[InternalCrossoverPointProbabilityParameterName]; }
    }
    public IValueLookupParameter<IntValue> MaximumSymbolicExpressionTreeLengthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeLengthParameterName]; }
    }
    public IValueLookupParameter<IntValue> MaximumSymbolicExpressionTreeDepthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeDepthParameterName]; }
    }
    #endregion
    #region Properties
    public PercentValue InternalCrossoverPointProbability {
      get { return InternalCrossoverPointProbabilityParameter.ActualValue; }
    }
    public IntValue MaximumSymbolicExpressionTreeLength {
      get { return MaximumSymbolicExpressionTreeLengthParameter.ActualValue; }
    }
    public IntValue MaximumSymbolicExpressionTreeDepth {
      get { return MaximumSymbolicExpressionTreeDepthParameter.ActualValue; }
    }
    #endregion
    [StorableConstructor]
    private SubtreeCrossover(bool deserializing) : base(deserializing) { }
    private SubtreeCrossover(SubtreeCrossover original, Cloner cloner) : base(original, cloner) { }
    public SubtreeCrossover()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumSymbolicExpressionTreeLengthParameterName, "The maximal length (number of nodes) of the symbolic expression tree."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumSymbolicExpressionTreeDepthParameterName, "The maximal depth of the symbolic expression tree (a tree with one node has depth = 0)."));
      Parameters.Add(new ValueLookupParameter<PercentValue>(InternalCrossoverPointProbabilityParameterName, "The probability to select an internal crossover point (instead of a leaf node).", new PercentValue(0.9)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SubtreeCrossover(this, cloner);
    }

    protected override ISymbolicExpressionTree Cross(IRandom random,
      ISymbolicExpressionTree parent0, ISymbolicExpressionTree parent1) {
      return Cross(random, parent0, parent1, InternalCrossoverPointProbability.Value,
        MaximumSymbolicExpressionTreeLength.Value, MaximumSymbolicExpressionTreeDepth.Value);
    }

    public static ISymbolicExpressionTree Cross(IRandom random,
      ISymbolicExpressionTree parent0, ISymbolicExpressionTree parent1,
      double internalCrossoverPointProbability, int maxTreeLength, int maxTreeDepth) {
      // select a random crossover point in the first parent 
      ISymbolicExpressionTreeNode crossoverPoint0;
      int replacedSubtreeIndex;
      SelectCrossoverPoint(random, parent0, internalCrossoverPointProbability, maxTreeLength, maxTreeDepth, out crossoverPoint0, out replacedSubtreeIndex);

      // calculate the max length and depth that the inserted branch can have 
      int maxInsertedBranchLength = maxTreeLength - (parent0.Length - crossoverPoint0.GetSubtree(replacedSubtreeIndex).GetLength());
      int maxInsertedBranchDepth = maxTreeDepth - GetBranchLevel(parent0.Root, crossoverPoint0);

      List<ISymbolicExpressionTreeNode> allowedBranches = new List<ISymbolicExpressionTreeNode>();
      parent1.Root.ForEachNodePostfix((n) => {
        if (n.GetLength() <= maxInsertedBranchLength &&
          n.GetDepth() <= maxInsertedBranchDepth &&
          IsMatchingPointType(crossoverPoint0, replacedSubtreeIndex, n))
          allowedBranches.Add(n);
      });

      if (allowedBranches.Count == 0) {
        return parent0;
      } else {
        var selectedBranch = SelectRandomBranch(random, allowedBranches, internalCrossoverPointProbability);

        // manipulate the tree of parent0 in place
        // replace the branch in tree0 with the selected branch from tree1
        crossoverPoint0.RemoveSubtree(replacedSubtreeIndex);
        crossoverPoint0.InsertSubtree(replacedSubtreeIndex, selectedBranch);
        return parent0;
      }
    }

    private static bool IsMatchingPointType(ISymbolicExpressionTreeNode parent, int replacedSubtreeIndex, ISymbolicExpressionTreeNode branch) {
      // check syntax constraints of direct parent - child relation
      if (!parent.Grammar.ContainsSymbol(branch.Symbol) ||
          !parent.Grammar.IsAllowedChildSymbol(parent.Symbol, branch.Symbol, replacedSubtreeIndex)) return false;

      bool result = true;
      // check point type for the whole branch
      branch.ForEachNodePostfix((n) => {
        result =
          result &&
          parent.Grammar.ContainsSymbol(n.Symbol) &&
          n.SubtreesCount >= parent.Grammar.GetMinimumSubtreeCount(n.Symbol) &&
          n.SubtreesCount <= parent.Grammar.GetMaximumSubtreeCount(n.Symbol);
      });
      return result;
    }

    private static void SelectCrossoverPoint(IRandom random, ISymbolicExpressionTree parent0, double internalNodeProbability, int maxBranchLength, int maxBranchDepth, out ISymbolicExpressionTreeNode crossoverPoint, out int subtreeIndex) {
      if (internalNodeProbability < 0.0 || internalNodeProbability > 1.0) throw new ArgumentException("internalNodeProbability");
      List<CutPoint> internalCrossoverPoints = new List<CutPoint>();
      List<CutPoint> leafCrossoverPoints = new List<CutPoint>();
      parent0.Root.ForEachNodePostfix((n) => {
        if (n.Subtrees.Any() && n != parent0.Root) {
          foreach (var child in n.Subtrees) {
            if (child.GetLength() <= maxBranchLength &&
                child.GetDepth() <= maxBranchDepth) {
              if (child.Subtrees.Any())
                internalCrossoverPoints.Add(new CutPoint(n, child));
              else
                leafCrossoverPoints.Add(new CutPoint(n, child));
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
          subtreeIndex = selectedCrossoverPoint.ChildIndex;
        } else {
          // otherwise select external node
          var selectedCrossoverPoint = leafCrossoverPoints[random.Next(leafCrossoverPoints.Count)];
          crossoverPoint = selectedCrossoverPoint.Parent;
          subtreeIndex = selectedCrossoverPoint.ChildIndex;
        }
      } else if (leafCrossoverPoints.Count > 0) {
        // select from leaf crossover point if possible
        var selectedCrossoverPoint = leafCrossoverPoints[random.Next(leafCrossoverPoints.Count)];
        crossoverPoint = selectedCrossoverPoint.Parent;
        subtreeIndex = selectedCrossoverPoint.ChildIndex;
      } else {
        // otherwise select internal crossover point
        var selectedCrossoverPoint = internalCrossoverPoints[random.Next(internalCrossoverPoints.Count)];
        crossoverPoint = selectedCrossoverPoint.Parent;
        subtreeIndex = selectedCrossoverPoint.ChildIndex;
      }
    }

    private static ISymbolicExpressionTreeNode SelectRandomBranch(IRandom random, IEnumerable<ISymbolicExpressionTreeNode> branches, double internalNodeProbability) {
      if (internalNodeProbability < 0.0 || internalNodeProbability > 1.0) throw new ArgumentException("internalNodeProbability");
      List<ISymbolicExpressionTreeNode> allowedInternalBranches;
      List<ISymbolicExpressionTreeNode> allowedLeafBranches;
      if (random.NextDouble() < internalNodeProbability) {
        // select internal node if possible
        allowedInternalBranches = (from branch in branches
                                   where branch.Subtrees.Any()
                                   select branch).ToList();
        if (allowedInternalBranches.Count > 0) {
          return allowedInternalBranches.SelectRandom(random);
        } else {
          // no internal nodes allowed => select leaf nodes
          allowedLeafBranches = (from branch in branches
                                 where !branch.Subtrees.Any()
                                 select branch).ToList();
          return allowedLeafBranches.SelectRandom(random);
        }
      } else {
        // select leaf node if possible
        allowedLeafBranches = (from branch in branches
                               where !branch.Subtrees.Any()
                               select branch).ToList();
        if (allowedLeafBranches.Count > 0) {
          return allowedLeafBranches.SelectRandom(random);
        } else {
          allowedInternalBranches = (from branch in branches
                                     where branch.Subtrees.Any()
                                     select branch).ToList();
          return allowedInternalBranches.SelectRandom(random);
        }
      }
    }

    private static int GetBranchLevel(ISymbolicExpressionTreeNode root, ISymbolicExpressionTreeNode point) {
      if (root == point) return 0;
      foreach (var subtree in root.Subtrees) {
        int branchLevel = GetBranchLevel(subtree, point);
        if (branchLevel < int.MaxValue) return 1 + branchLevel;
      }
      return int.MaxValue;
    }
  }
}
