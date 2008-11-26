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
  public class StandardCrossOver : GPCrossoverBase {
    private const int MAX_RECOMBINATION_TRIES = 100;

    public override string Description {
      get {
        return @"Takes two parent individuals P0 and P1 each. Selects a random node N0 of P0 and a random node N1 of P1.
And replaces the branch with root0 N0 in P0 with N1 from P1 if the tree-size limits are not violated.
When recombination with N0 and N1 would create a tree that is too large or invalid the operator randomly selects new N0 and N1 
until a valid configuration is found.";
      }
    }
    public StandardCrossOver()
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

      // check if the size and height of the new tree are still within the allowed bounds
      Debug.Assert(newTree.Height <= maxTreeHeight);
      Debug.Assert(newTree.Size <= maxTreeSize);
      return newTree;
    }


    private IFunctionTree Cross(TreeGardener gardener, IFunctionTree tree0, IFunctionTree tree1, MersenneTwister random, int maxTreeSize, int maxTreeHeight) {
      int tries = 0;
      List<IFunctionTree> allowedCrossoverPoints = null;
      IFunctionTree parent0;
      int replacedChildIndex;
      do {
        // select a random crossover point in the first parent tree0
        parent0 = null;
        while(parent0 == null) parent0 = gardener.GetRandomParentNode(tree0);
        // select a random branch to replace
        replacedChildIndex = random.Next(parent0.SubTrees.Count);

        // calculate the max size and height that the inserted branch can have 
        int maxInsertedBranchSize = maxTreeSize - (tree0.Size - parent0.SubTrees[replacedChildIndex].Size);
        int maxInsertedBranchHeight = maxTreeHeight - gardener.GetBranchLevel(tree0, parent0); // branchlevel is 1 if tree0==parent0

        IList<IFunction> allowedFunctions = gardener.GetAllowedSubFunctions(parent0.Function, replacedChildIndex);
        allowedCrossoverPoints = GetPossibleCrossoverPoints(gardener, tree1, maxInsertedBranchSize, maxInsertedBranchHeight, allowedFunctions);
      } while(allowedCrossoverPoints.Count == 0 || tries++ > MAX_RECOMBINATION_TRIES);

      if(allowedCrossoverPoints.Count > 0) {
        IFunctionTree branch1 = allowedCrossoverPoints[random.Next(allowedCrossoverPoints.Count)];

        // replace the branch in tree0 with the selected branch from tree1
        parent0.RemoveSubTree(replacedChildIndex);
        parent0.InsertSubTree(replacedChildIndex, branch1);
      }
      return tree0;
    }

    private List<IFunctionTree> GetPossibleCrossoverPoints(TreeGardener gardener, IFunctionTree tree, int maxInsertedBranchSize, int maxInsertedBranchHeight, IList<IFunction> allowedFunctions) {
      List<IFunctionTree> crossoverPoints = new List<IFunctionTree>();
      foreach(IFunctionTree possiblePoint in gardener.GetAllSubTrees(tree)) {
        if(allowedFunctions.Contains(possiblePoint.Function) && possiblePoint.Size <= maxInsertedBranchSize && possiblePoint.Height <= maxInsertedBranchHeight)
          crossoverPoints.Add(possiblePoint);
      }
      return crossoverPoints;
    }
  }
}
