#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Random;
using HeuristicLab.Data;
using HeuristicLab.Constraints;
using System.Diagnostics;

namespace HeuristicLab.GP {
  /// <summary>
  /// Implementation of a size fair crossover operator as described in: 
  /// William B. Langdon 
  /// Size Fair and Homologous Tree Genetic Programming Crossovers, 
  /// Genetic Programming and Evolvable Machines, Vol. 1, Number 1/2, pp. 95-119, April 2000
  /// </summary>
  public class SizeFairCrossOver : SizeConstrictedGPCrossoverBase {
    private const int MAX_RECOMBINATION_TRIES = 20;
    // private data structure for crossover points
    protected class CrossoverPoint {
      public IFunctionTree tree;
      public int branchSize;
      public List<int> trail;
    }

    internal override IFunctionTree Cross(TreeGardener gardener, MersenneTwister random, IFunctionTree tree0, IFunctionTree tree1, int maxTreeSize, int maxTreeHeight) {
      int tries = 0;
      IFunctionTree insertedBranch = null;
      IFunctionTree parent = null;
      int removedBranchIndex = 0;
      do {
        // select a random suboperator of the 'receiving' tree
        while (parent == null) parent = gardener.GetRandomParentNode(tree0);
        removedBranchIndex = random.Next(parent.SubTrees.Count);
        insertedBranch = GetReplacementBranch(random, gardener, tree0, parent, removedBranchIndex, tree1, maxTreeSize, maxTreeHeight);
      } while (insertedBranch == null && tries++ < MAX_RECOMBINATION_TRIES);

      if (insertedBranch != null) {
        // replace the branch below the crossoverpoint with the selected branch from root1
        parent.RemoveSubTree(removedBranchIndex);
        parent.InsertSubTree(removedBranchIndex, insertedBranch);
      }
      return tree0;
    }

    private IFunctionTree GetReplacementBranch(MersenneTwister random, TreeGardener gardener, IFunctionTree intoTree, IFunctionTree parent, int replacedBranchIndex, IFunctionTree fromTree, int maxTreeSize, int maxTreeHeight) {
      IList<IFunction> allowedFunctions = gardener.GetAllowedSubFunctions(parent.Function, replacedBranchIndex);
      int removedBranchSize = parent.SubTrees[replacedBranchIndex].Size;
      int maxBranchSize = maxTreeSize - (intoTree.Size - removedBranchSize);
      int maxBranchHeight = maxTreeHeight - gardener.GetBranchLevel(intoTree, parent);  // returns 1 if intoTree==parent and 2 if parent is a child of intoTree
      List<int> replacedTrail = GetTrail(intoTree, parent);
      replacedTrail.Add(replacedBranchIndex);

      List<CrossoverPoint> shorterBranches = new List<CrossoverPoint>();
      List<CrossoverPoint> longerBranches = new List<CrossoverPoint>();
      List<CrossoverPoint> equalLengthBranches = new List<CrossoverPoint>();

      FindPossibleBranches(fromTree, allowedFunctions, maxBranchSize, maxBranchHeight, removedBranchSize, shorterBranches, equalLengthBranches, longerBranches, new List<int>());

      if (shorterBranches.Count > 0 && longerBranches.Count > 0) {
        double pEqualLength = equalLengthBranches.Count > 0 ? 1.0 / removedBranchSize : 0.0;
        double pLonger = (1.0 - pEqualLength) / (longerBranches.Count * (1.0 + longerBranches.Average(p => p.branchSize) / shorterBranches.Average(p => p.branchSize)));
        double pShorter = (1.0 - pEqualLength - pLonger);

        double r = random.NextDouble();
        if (r < pLonger) {
          return SelectReplacement(random, replacedTrail, longerBranches);
        } else if (r < pLonger + pShorter) {
          return SelectReplacement(random, replacedTrail, shorterBranches);
        } else {
          return SelectReplacement(random, replacedTrail, equalLengthBranches);
        }
      } else if (equalLengthBranches.Count > 0) {
        return SelectReplacement(random, replacedTrail, equalLengthBranches);
      } else {
        return null;
      }
    }

    protected virtual IFunctionTree SelectReplacement(MersenneTwister random, List<int> replacedTrail, List<CrossoverPoint> crossoverPoints) {
      return crossoverPoints[random.Next(crossoverPoints.Count)].tree;
    }

    private void FindPossibleBranches(IFunctionTree tree, IList<IFunction> allowedFunctions, int maxBranchSize, int maxBranchHeight, int removedBranchSize,
      List<CrossoverPoint> shorterBranches, List<CrossoverPoint> equalLengthBranches, List<CrossoverPoint> longerBranches, List<int> trail) {
      int treeSize = tree.Size;
      if (allowedFunctions.Contains(tree.Function) && treeSize <= maxBranchSize && tree.Height <= maxBranchHeight) {
        CrossoverPoint p = new CrossoverPoint();
        p.branchSize = treeSize;
        p.tree = tree;
        p.trail = new List<int>(trail);
        if (treeSize < removedBranchSize) shorterBranches.Add(p);
        else if (treeSize > removedBranchSize) longerBranches.Add(p);
        else equalLengthBranches.Add(p);
      }
      for (int i = 0; i < tree.SubTrees.Count; i++) {
        trail.Add(i);
        FindPossibleBranches(tree.SubTrees[i], allowedFunctions, maxBranchSize, maxBranchHeight, removedBranchSize, shorterBranches, equalLengthBranches, longerBranches, trail);
        trail.RemoveAt(trail.Count - 1);
      }
    }

    private List<int> GetTrail(IFunctionTree root, IFunctionTree branch) {
      List<int> trail = new List<int>();
      GetTrail(root, branch, trail);
      trail.Reverse();
      trail.RemoveAt(trail.Count - 1);
      return trail;
    }
    private void GetTrail(IFunctionTree root, IFunctionTree branch, List<int> trail) {
      if (root == branch) {
        trail.Add(-1); // add flag that there was a match
        return;
      }

      for (int i = 0; i < root.SubTrees.Count; i++) {
        GetTrail(root.SubTrees[i], branch, trail);
        if (trail.Count>0) {
          trail.Add(i);
          return;
        }
      }
    }
  }
}
