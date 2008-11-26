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
  public class SizeFairCrossOver : GPCrossoverBase {
    private const int MAX_RECOMBINATION_TRIES = 20;
    public override string Description {
      get {
        return @"";
      }
    }
    public SizeFairCrossOver()
      : base() {
      AddVariableInfo(new VariableInfo("MaxTreeHeight", "The maximal allowed height of the tree", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaxTreeSize", "The maximal allowed size (number of nodes) of the tree", typeof(IntData), VariableKind.In));
    }

    internal override IFunctionTree Cross(IScope scope, TreeGardener gardener, MersenneTwister random, IFunctionTree tree0, IFunctionTree tree1) {
      int maxTreeHeight = GetVariableValue<IntData>("MaxTreeHeight", scope, true).Data;
      int maxTreeSize = GetVariableValue<IntData>("MaxTreeSize", scope, true).Data;

      // when tree0 is terminal then try to cross into tree1, when tree1 is also terminal just return tree0 unchanged.
      IFunctionTree newTree;
      if(tree0.SubTrees.Count > 0) {
        newTree = Cross(gardener, tree0, tree1, random, maxTreeSize, maxTreeHeight);
      } else if(tree1.SubTrees.Count > 0) {
        newTree = Cross(gardener, tree1, tree0, random, maxTreeSize, maxTreeHeight);
      } else newTree = tree0;

      // check if the height and size of the new tree are still in the allowed bounds
      Debug.Assert(newTree.Height <= maxTreeHeight);
      Debug.Assert(newTree.Size <= maxTreeSize);
      return newTree;
    }

    private IFunctionTree Cross(TreeGardener gardener, IFunctionTree tree0, IFunctionTree tree1, MersenneTwister random, int maxTreeSize, int maxTreeHeight) {
      int tries = 0;
      IFunctionTree insertedBranch = null;
      IFunctionTree crossoverPoint = null;
      int removedBranchIndex = 0;
      do {
        // select a random suboperator of the 'receiving' tree
        while(crossoverPoint == null) crossoverPoint = gardener.GetRandomParentNode(tree0);
        removedBranchIndex = random.Next(crossoverPoint.SubTrees.Count);
        IFunctionTree removedBranch = crossoverPoint.SubTrees[removedBranchIndex];
        IList<IFunction> allowedFunctions = gardener.GetAllowedSubFunctions(crossoverPoint.Function, removedBranchIndex);
        int removedBranchSize = removedBranch.Size;
        int maxBranchSize = maxTreeSize - (tree0.Size - removedBranchSize);
        int maxBranchHeight = maxTreeHeight - gardener.GetBranchLevel(tree0, crossoverPoint);
        insertedBranch = GetReplacementBranch(random, gardener, allowedFunctions, tree1, removedBranchSize, maxBranchSize, maxBranchHeight);
      } while(insertedBranch == null && tries++ < MAX_RECOMBINATION_TRIES);

      if(insertedBranch != null) {
        // replace the branch below the crossoverpoint with the selected branch from root1
        crossoverPoint.RemoveSubTree(removedBranchIndex);
        crossoverPoint.InsertSubTree(removedBranchIndex, insertedBranch);
      }
      return tree0;
    }

    private IFunctionTree GetReplacementBranch(IRandom random, TreeGardener gardener, IList<IFunction> allowedFunctions, IFunctionTree tree, int removedBranchSize, int maxBranchSize, int maxBranchHeight) {
      var branches = gardener.GetAllSubTrees(tree).Where(t => allowedFunctions.Contains(t.Function) && t.Size <= maxBranchSize && t.Height <= maxBranchHeight)
        .Select(t => new { Tree = t, Size = t.Size }).Where(s => s.Size < 2 * removedBranchSize + 1);

      var shorterBranches = branches.Where(t => t.Size < removedBranchSize);
      var longerBranches = branches.Where(t => t.Size > removedBranchSize);
      var equalLengthBranches = branches.Where(t => t.Size == removedBranchSize);

      if(shorterBranches.Count() == 0 || longerBranches.Count() == 0) {
        if(equalLengthBranches.Count() == 0) {
          return null;
        } else {
          return equalLengthBranches.ElementAt(random.Next(equalLengthBranches.Count())).Tree;
        }
      } else {
        // invariant: |shorterBranches| > 0  and |longerBranches| > 0
        double pEqualLength = equalLengthBranches.Count() > 0 ? 1.0 / removedBranchSize : 0.0;
        double pLonger = (1.0 - pEqualLength) / (longerBranches.Count() * (1.0 + longerBranches.Average(t => t.Size) / shorterBranches.Average(t => t.Size)));
        double pShorter = (1.0 - pEqualLength - pLonger);

        double r = random.NextDouble();
        if(r < pLonger) {
          return longerBranches.ElementAt(random.Next(longerBranches.Count())).Tree;
        } else if(r < pLonger + pShorter) {
          return shorterBranches.ElementAt(random.Next(shorterBranches.Count())).Tree;
        } else {
          return equalLengthBranches.ElementAt(random.Next(equalLengthBranches.Count())).Tree;
        }
      }
    }
  }
}
