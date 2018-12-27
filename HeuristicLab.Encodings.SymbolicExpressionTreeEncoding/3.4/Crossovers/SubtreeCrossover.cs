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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  /// <summary>
  /// Takes two parent individuals P0 and P1 each. Selects a random node N0 of P0 and a random node N1 of P1.
  /// And replaces the branch with root0 N0 in P0 with N1 from P1 if the tree-size limits are not violated.
  /// When recombination with N0 and N1 would create a tree that is too large or invalid the operator randomly selects new N0 and N1 
  /// until a valid configuration is found.
  /// </summary>  
  [Item("SubtreeSwappingCrossover", "An operator which performs subtree swapping crossover.")]
  [StorableClass]
  public class SubtreeCrossover : SymbolicExpressionTreeCrossover, ISymbolicExpressionTreeSizeConstraintOperator {
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
    protected SubtreeCrossover(bool deserializing) : base(deserializing) { }
    protected SubtreeCrossover(SubtreeCrossover original, Cloner cloner) : base(original, cloner) { }
    public SubtreeCrossover()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumSymbolicExpressionTreeLengthParameterName, "The maximal length (number of nodes) of the symbolic expression tree."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumSymbolicExpressionTreeDepthParameterName, "The maximal depth of the symbolic expression tree (a tree with one node has depth = 0)."));
      Parameters.Add(new ValueLookupParameter<PercentValue>(InternalCrossoverPointProbabilityParameterName, "The probability to select an internal crossover point (instead of a leaf node).", new PercentValue(0.9)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SubtreeCrossover(this, cloner);
    }

    public override ISymbolicExpressionTree Crossover(IRandom random,
      ISymbolicExpressionTree parent0, ISymbolicExpressionTree parent1) {
      return Cross(random, parent0, parent1, InternalCrossoverPointProbability.Value,
        MaximumSymbolicExpressionTreeLength.Value, MaximumSymbolicExpressionTreeDepth.Value);
    }

    public static ISymbolicExpressionTree Cross(IRandom random,
      ISymbolicExpressionTree parent0, ISymbolicExpressionTree parent1,
      double internalCrossoverPointProbability, int maxTreeLength, int maxTreeDepth) {
      // select a random crossover point in the first parent 
      CutPoint crossoverPoint0;
      SelectCrossoverPoint(random, parent0, internalCrossoverPointProbability, maxTreeLength, maxTreeDepth, out crossoverPoint0);

      int childLength = crossoverPoint0.Child != null ? crossoverPoint0.Child.GetLength() : 0;
      // calculate the max length and depth that the inserted branch can have 
      int maxInsertedBranchLength = Math.Max(0, maxTreeLength - (parent0.Length - childLength));
      int maxInsertedBranchDepth = Math.Max(0, maxTreeDepth - parent0.Root.GetBranchLevel(crossoverPoint0.Parent));

      List<ISymbolicExpressionTreeNode> allowedBranches = new List<ISymbolicExpressionTreeNode>();
      parent1.Root.ForEachNodePostfix((n) => {
        if (n.GetLength() <= maxInsertedBranchLength &&
            n.GetDepth() <= maxInsertedBranchDepth && crossoverPoint0.IsMatchingPointType(n))
          allowedBranches.Add(n);
      });
      // empty branch
      if (crossoverPoint0.IsMatchingPointType(null)) allowedBranches.Add(null);

      if (allowedBranches.Count == 0) {
        return parent0;
      } else {
        var selectedBranch = SelectRandomBranch(random, allowedBranches, internalCrossoverPointProbability);
        if (selectedBranch != null)
          selectedBranch = (ISymbolicExpressionTreeNode)selectedBranch.Clone();

        if (crossoverPoint0.Child != null) {
          // manipulate the tree of parent0 in place
          // replace the branch in tree0 with the selected branch from tree1
          crossoverPoint0.Parent.RemoveSubtree(crossoverPoint0.ChildIndex);
          if (selectedBranch != null) {
            crossoverPoint0.Parent.InsertSubtree(crossoverPoint0.ChildIndex, selectedBranch);
          }
        } else {
          // child is null (additional child should be added under the parent)
          if (selectedBranch != null) {
            crossoverPoint0.Parent.AddSubtree(selectedBranch);
          }
        }
        return parent0;
      }
    }

    private static void SelectCrossoverPoint(IRandom random, ISymbolicExpressionTree parent0, double internalNodeProbability, int maxBranchLength, int maxBranchDepth, out CutPoint crossoverPoint) {
      if (internalNodeProbability < 0.0 || internalNodeProbability > 1.0) throw new ArgumentException("internalNodeProbability");
      List<CutPoint> internalCrossoverPoints = new List<CutPoint>();
      List<CutPoint> leafCrossoverPoints = new List<CutPoint>();
      parent0.Root.ForEachNodePostfix((n) => {
        if (n.SubtreeCount > 0 && n != parent0.Root) {
          //avoid linq to reduce memory pressure
          for (int i = 0; i < n.SubtreeCount; i++) {
            var child = n.GetSubtree(i);
            if (child.GetLength() <= maxBranchLength &&
                child.GetDepth() <= maxBranchDepth) {
              if (child.SubtreeCount > 0)
                internalCrossoverPoints.Add(new CutPoint(n, child));
              else
                leafCrossoverPoints.Add(new CutPoint(n, child));
            }
          }

          // add one additional extension point if the number of sub trees for the symbol is not full
          if (n.SubtreeCount < n.Grammar.GetMaximumSubtreeCount(n.Symbol)) {
            // empty extension point
            internalCrossoverPoints.Add(new CutPoint(n, n.SubtreeCount));
          }
        }
      }
    );

      if (random.NextDouble() < internalNodeProbability) {
        // select from internal node if possible
        if (internalCrossoverPoints.Count > 0) {
          // select internal crossover point or leaf
          crossoverPoint = internalCrossoverPoints[random.Next(internalCrossoverPoints.Count)];
        } else {
          // otherwise select external node
          crossoverPoint = leafCrossoverPoints[random.Next(leafCrossoverPoints.Count)];
        }
      } else if (leafCrossoverPoints.Count > 0) {
        // select from leaf crossover point if possible
        crossoverPoint = leafCrossoverPoints[random.Next(leafCrossoverPoints.Count)];
      } else {
        // otherwise select internal crossover point
        crossoverPoint = internalCrossoverPoints[random.Next(internalCrossoverPoints.Count)];
      }
    }

    private static ISymbolicExpressionTreeNode SelectRandomBranch(IRandom random, IEnumerable<ISymbolicExpressionTreeNode> branches, double internalNodeProbability) {
      if (internalNodeProbability < 0.0 || internalNodeProbability > 1.0) throw new ArgumentException("internalNodeProbability");
      List<ISymbolicExpressionTreeNode> allowedInternalBranches;
      List<ISymbolicExpressionTreeNode> allowedLeafBranches;
      if (random.NextDouble() < internalNodeProbability) {
        // select internal node if possible
        allowedInternalBranches = (from branch in branches
                                   where branch != null && branch.SubtreeCount > 0
                                   select branch).ToList();
        if (allowedInternalBranches.Count > 0) {
          return allowedInternalBranches.SampleRandom(random);

        } else {
          // no internal nodes allowed => select leaf nodes
          allowedLeafBranches = (from branch in branches
                                 where branch == null || branch.SubtreeCount == 0
                                 select branch).ToList();
          return allowedLeafBranches.SampleRandom(random);
        }
      } else {
        // select leaf node if possible
        allowedLeafBranches = (from branch in branches
                               where branch == null || branch.SubtreeCount == 0
                               select branch).ToList();
        if (allowedLeafBranches.Count > 0) {
          return allowedLeafBranches.SampleRandom(random);
        } else {
          allowedInternalBranches = (from branch in branches
                                     where branch != null && branch.SubtreeCount > 0
                                     select branch).ToList();
          return allowedInternalBranches.SampleRandom(random);

        }
      }
    }
  }
}
