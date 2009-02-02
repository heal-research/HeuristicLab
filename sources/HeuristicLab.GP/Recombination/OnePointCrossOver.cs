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
  /// Implementation of a homologous one point crossover operator as described in: 
  /// W. B. Langdon and R. Poli.  Foundations of Genetic Programming. Springer-Verlag, 2002.
  /// </summary>
  public class OnePointCrossOver : SizeConstrictedGPCrossoverBase {
    // internal data structure to represent crossover points
    private class CrossoverPoint {
      public IFunctionTree parent0;
      public IFunctionTree parent1;
      public int childIndex;
    }
    public override string Description {
      get {
        return @"One point crossover for trees as described in W. B. Langdon and R. Poli. Foundations of Genetic Programming. Springer-Verlag, 2002.";
      }
    }

    internal override IFunctionTree Cross(TreeGardener gardener, MersenneTwister random, IFunctionTree tree0, IFunctionTree tree1, int maxTreeSize, int maxTreeHeight) {
      List<CrossoverPoint> allowedCrossOverPoints = new List<CrossoverPoint>();
      GetCrossOverPoints(gardener, tree0, tree1, maxTreeSize - tree0.Size, allowedCrossOverPoints);
      if (allowedCrossOverPoints.Count > 0) {
        CrossoverPoint crossOverPoint = allowedCrossOverPoints[random.Next(allowedCrossOverPoints.Count)];
        IFunctionTree parent0 = crossOverPoint.parent0;
        IFunctionTree branch1 = crossOverPoint.parent1.SubTrees[crossOverPoint.childIndex];
        parent0.RemoveSubTree(crossOverPoint.childIndex);
        parent0.InsertSubTree(crossOverPoint.childIndex, branch1);
      }
      return tree0;
    }

    private void GetCrossOverPoints(TreeGardener gardener, IFunctionTree branch0, IFunctionTree branch1, int maxNewNodes, List<CrossoverPoint> crossoverPoints) {
      if (branch0.SubTrees.Count != branch1.SubTrees.Count) return;

      for (int i = 0; i < branch0.SubTrees.Count; i++) {
        // if the current branch can be attached as a sub-tree to branch0
        if (gardener.GetAllowedSubFunctions(branch0.Function, i).Contains(branch1.SubTrees[i].Function) &&
           branch1.SubTrees[i].Size - branch0.SubTrees[i].Size <= maxNewNodes) {
          CrossoverPoint p = new CrossoverPoint();
          p.childIndex = i;
          p.parent0 = branch0;
          p.parent1 = branch1;
          crossoverPoints.Add(p);
        }
        GetCrossOverPoints(gardener, branch0.SubTrees[i], branch1.SubTrees[i], maxNewNodes, crossoverPoints);
      }
    }
  }
}
