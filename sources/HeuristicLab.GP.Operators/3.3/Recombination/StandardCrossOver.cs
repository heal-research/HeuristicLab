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

using System.Collections.Generic;
using HeuristicLab.Core;
using HeuristicLab.GP.Interfaces;

namespace HeuristicLab.GP.Operators {
  public class StandardCrossOver : SizeConstrictedGPCrossoverBase {
    private int MaxRecombinationTries { get { return 20; } }

    public override string Description {
      get {
        return @"Takes two parent individuals P0 and P1 each. Selects a random node N0 of P0 and a random node N1 of P1.
And replaces the branch with root0 N0 in P0 with N1 from P1 if the tree-size limits are not violated.
When recombination with N0 and N1 would create a tree that is too large or invalid the operator randomly selects new N0 and N1 
until a valid configuration is found.";
      }
    }

    internal override IFunctionTree Cross(TreeGardener gardener, IRandom random, IFunctionTree tree0, IFunctionTree tree1, int maxTreeSize, int maxTreeHeight) {
      int tries = 0;
      List<IFunctionTree> allowedCrossoverPoints = null;
      IFunctionTree parent0;
      int replacedChildIndex;
      do {
        // select a random crossover point in the first parent tree0
        parent0 = null;
        while (parent0 == null) parent0 = gardener.GetRandomParentNode(tree0);
        // select a random branch to replace
        replacedChildIndex = random.Next(parent0.SubTrees.Count);

        // calculate the max size and height that the inserted branch can have 
        int maxInsertedBranchSize = maxTreeSize - (tree0.GetSize() - parent0.SubTrees[replacedChildIndex].GetSize());
        int maxInsertedBranchHeight = maxTreeHeight - gardener.GetBranchLevel(tree0, parent0); // branchlevel is 1 if tree0==parent0

        IList<IFunction> allowedFunctions = new List<IFunction>(gardener.GetAllowedSubFunctions(parent0.Function, replacedChildIndex));
        allowedCrossoverPoints = GetPossibleCrossoverPoints(tree1, maxInsertedBranchSize, maxInsertedBranchHeight, allowedFunctions);
      } while (allowedCrossoverPoints.Count == 0 && tries++ < MaxRecombinationTries);

      if (allowedCrossoverPoints.Count > 0) {
        IFunctionTree branch1 = allowedCrossoverPoints[random.Next(allowedCrossoverPoints.Count)];

        // replace the branch in tree0 with the selected branch from tree1
        parent0.RemoveSubTree(replacedChildIndex);
        parent0.InsertSubTree(replacedChildIndex, branch1);
      }
      return tree0;
    }

    private List<IFunctionTree> GetPossibleCrossoverPoints(IFunctionTree tree, int maxInsertedBranchSize, int maxInsertedBranchHeight, IList<IFunction> allowedFunctions) {
      List<IFunctionTree> crossoverPoints = new List<IFunctionTree>();
      foreach (IFunctionTree possiblePoint in TreeGardener.GetAllSubTrees(tree)) {
        if (allowedFunctions.Contains(possiblePoint.Function) && possiblePoint.GetSize() <= maxInsertedBranchSize && possiblePoint.GetHeight() <= maxInsertedBranchHeight)
          crossoverPoints.Add(possiblePoint);
      }
      return crossoverPoints;
    }
  }
}
