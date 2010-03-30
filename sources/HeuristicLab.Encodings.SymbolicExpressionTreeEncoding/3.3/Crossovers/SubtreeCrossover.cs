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
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Data;
using System.Linq;
using System;
using HeuristicLab.Parameters;
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
    private const int MAX_TRIES = 100;

    public IValueLookupParameter<PercentValue> InternalCrossoverPointProbabilityParameter {
      get { return (IValueLookupParameter<PercentValue>)Parameters["InternalCrossoverPointProbability"]; }
    }

    public SubtreeCrossover()
      : base() {
      Parameters.Add(new ValueLookupParameter<PercentValue>("InternalCrossoverPointProbability", "The probability to select an internal crossover point (instead of a leaf node).", new PercentValue(0.9)));
    }

    protected override SymbolicExpressionTree Cross(IRandom random, ISymbolicExpressionGrammar grammar,
      SymbolicExpressionTree parent0, SymbolicExpressionTree parent1,
      IntValue maxTreeSize, IntValue maxTreeHeight) {
      return Apply(random, grammar, parent0, parent1, InternalCrossoverPointProbabilityParameter.ActualValue.Value, maxTreeSize.Value, maxTreeHeight.Value);
    }

    public static SymbolicExpressionTree Apply(IRandom random, ISymbolicExpressionGrammar grammar,
      SymbolicExpressionTree parent0, SymbolicExpressionTree parent1,
      double internalCrossoverPointProbability, int maxTreeSize, int maxTreeHeight) {
      int tries = 0;
      while (tries++ < MAX_TRIES) {
        // select a random crossover point in the first parent 
        SymbolicExpressionTreeNode crossoverPoint0;
        int replacedSubtreeIndex;
        SelectCrossoverPoint(random, parent0, internalCrossoverPointProbability, out crossoverPoint0, out replacedSubtreeIndex);

        // calculate the max size and height that the inserted branch can have 
        int maxInsertedBranchSize = maxTreeSize - (parent0.Size - crossoverPoint0.SubTrees[replacedSubtreeIndex].GetSize());
        int maxInsertedBranchHeight = maxTreeHeight - GetBranchLevel(parent0.Root, crossoverPoint0);

        var allowedBranches = from branch in IterateNodes(parent1.Root)
                              where branch.GetSize() < maxInsertedBranchSize
                              where branch.GetHeight() < maxInsertedBranchHeight
                              where grammar.AllowedSymbols(crossoverPoint0.Symbol, replacedSubtreeIndex).Contains(branch.Symbol)
                              select branch;

        if (allowedBranches.Count() > 0) {
          var selectedBranch = SelectRandomBranch(random, allowedBranches, internalCrossoverPointProbability);

          // manipulate the tree of parent0 in place
          // replace the branch in tree0 with the selected branch from tree1
          crossoverPoint0.RemoveSubTree(replacedSubtreeIndex);
          crossoverPoint0.InsertSubTree(replacedSubtreeIndex, selectedBranch);
          return parent0;
        }
      }

      // TODO: we should have a way to track the number of failed crossover attempts
      // for now just return the first parent unchanged
      return parent0;
    }

    private static void SelectCrossoverPoint(IRandom random, SymbolicExpressionTree parent0, double internalNodeProbability, out SymbolicExpressionTreeNode crossoverPoint, out int subtreeIndex) {
      var crossoverPoints = from branch in IterateNodes(parent0.Root)
                            where branch.SubTrees.Count > 0
                            from index in Enumerable.Range(0, branch.SubTrees.Count)
                            let p = new { CrossoverPoint = branch, SubtreeIndex = index, IsLeaf = branch.SubTrees[index].SubTrees.Count == 0 }
                            select p;
      var internalCrossoverPoints = (from p in crossoverPoints
                                     where !p.IsLeaf
                                     select p).ToList();
      // select internal crossover point or leaf
      if (random.NextDouble() < internalNodeProbability && internalCrossoverPoints.Count > 0) {
        var selectedCrossoverPoint = internalCrossoverPoints[random.Next(internalCrossoverPoints.Count)];
        crossoverPoint = selectedCrossoverPoint.CrossoverPoint;
        subtreeIndex = selectedCrossoverPoint.SubtreeIndex;
      } else {
        var leafCrossoverPoints = (from p in crossoverPoints
                                   where p.IsLeaf
                                   select p).ToList();
        var selectedCrossoverPoint = leafCrossoverPoints[random.Next(leafCrossoverPoints.Count)];
        crossoverPoint = selectedCrossoverPoint.CrossoverPoint;
        subtreeIndex = selectedCrossoverPoint.SubtreeIndex;
      }
    }

    private static SymbolicExpressionTreeNode SelectRandomBranch(IRandom random, IEnumerable<SymbolicExpressionTreeNode> branches, double internalNodeProbability) {
      if (internalNodeProbability < 0.0 || internalNodeProbability > 1.0) throw new ArgumentException("internalNodeProbability");
      var groupedBranches = from branch in branches
                            group branch by branch.SubTrees.Count into g
                            select g;
      var allowedInternalBranches = (from g in groupedBranches
                                     where g.Key > 0
                                     from branch in g
                                     select branch).ToList();
      if (random.NextDouble() < internalNodeProbability && allowedInternalBranches.Count > 0) {
        return allowedInternalBranches[random.Next(allowedInternalBranches.Count)];
      } else {
        var allowedLeafBranches = (from g in groupedBranches
                                   where g.Key == 0
                                   from leaf in g
                                   select leaf).ToList();
        return allowedLeafBranches[random.Next(allowedLeafBranches.Count)];
      }
    }

    private static IEnumerable<SymbolicExpressionTreeNode> IterateNodes(SymbolicExpressionTreeNode root) {
      foreach (var subTree in root.SubTrees) {
        foreach (var branch in IterateNodes(subTree)) {
          yield return branch;
        }
      }
      yield return root;
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
