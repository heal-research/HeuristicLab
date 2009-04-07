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
  /// Implementation of a homologous uniform crossover operator as described in: 
  /// R. Poli and W. B. Langdon.  On the Search Properties of Different Crossover Operators in Genetic Programming.
  /// In Proceedings of Genetic Programming '98, Madison, Wisconsin, 1998.
  /// </summary>
  public class UniformCrossover : SizeConstrictedGPCrossoverBase {
    // internal datastructure to represent crossover points
    private class CrossoverPoint {
      public IFunctionTree Parent0;
      public IFunctionTree Parent1;
      public int ChildIndex;
      public bool IsInternal;
    }

    public override string Description {
      get {
        return @"Uniform crossover as defined by Poli and Langdon";
      }
    }

    internal override IFunctionTree Cross(TreeGardener gardener, IRandom random, IFunctionTree tree0, IFunctionTree tree1, int maxTreeSize, int maxTreeHeight) {
      List<CrossoverPoint> allowedCrossOverPoints = new List<CrossoverPoint>();
      GetCrossOverPoints(gardener, tree0, tree1, allowedCrossOverPoints);
      // iterate through the list of crossover points and swap nodes with p=0.5
      foreach (CrossoverPoint crossoverPoint in allowedCrossOverPoints) {
        if (random.NextDouble() < 0.5) {
          if (crossoverPoint.IsInternal) {
            ExchangeNodes(crossoverPoint);
          } else {
            SwapSubtrees(crossoverPoint);
          }
        }
      }
      return tree0;
    }

    private void GetCrossOverPoints(TreeGardener gardener, IFunctionTree branch0, IFunctionTree branch1, List<CrossoverPoint> crossoverPoints) {
      if (branch0.SubTrees.Count != branch1.SubTrees.Count) return; // branches have to have same number of sub-trees to be valid crossover points
      // iterate over all sub-trees
      for (int i = 0; i < branch0.SubTrees.Count; i++) {
        IFunctionTree currentSubTree0 = branch0.SubTrees[i];
        IFunctionTree currentSubTree1 = branch1.SubTrees[i];
        // when the current sub-tree in branch1 can be attached as a child of branch0
        // and the sub-tree of branch0 can be attached as child of branch1.
        // note: we have to check both cases because either branch0 or branch1 can end up in the result tree
        if (gardener.GetAllowedSubFunctions(branch0.Function, i).Contains(currentSubTree1.Function) &&
          gardener.GetAllowedSubFunctions(branch1.Function, i).Contains(currentSubTree0.Function)) {
          // and the sub-tree is at the border of the common region 
          if (currentSubTree0.SubTrees.Count != currentSubTree1.SubTrees.Count) {
            // then we have found a valid crossover point
            CrossoverPoint p = new CrossoverPoint();
            p.ChildIndex = i;
            p.Parent0 = branch0;
            p.Parent1 = branch1;
            p.IsInternal = false;
            crossoverPoints.Add(p);
          } else {
            // when the sub-trees are not on the border of the common region
            // we also have to check if the children of the current sub-trees of branch0 and branch1 can be exchanged 
            if (CanHaveSubTrees(gardener, currentSubTree0, currentSubTree1.SubTrees) &&
              CanHaveSubTrees(gardener, currentSubTree1, currentSubTree0.SubTrees)) {
              CrossoverPoint p = new CrossoverPoint();
              p.ChildIndex = i;
              p.Parent0 = branch0;
              p.Parent1 = branch1;
              p.IsInternal = true;
              crossoverPoints.Add(p);
            }
          }
        }
        GetCrossOverPoints(gardener, currentSubTree0, currentSubTree1, crossoverPoints);
      }
    }

    private bool CanHaveSubTrees(TreeGardener gardener, IFunctionTree parent, IList<IFunctionTree> subTrees) {
      for (int i = 0; i < subTrees.Count; i++) {
        if (!gardener.GetAllowedSubFunctions(parent.Function, i).Contains(subTrees[i].Function)) return false;
      }
      return true;
    }

    private void ExchangeNodes(CrossoverPoint crossoverPoint) {
      IFunctionTree parent0 = crossoverPoint.Parent0;
      IFunctionTree parent1 = crossoverPoint.Parent1;
      int childIndex = crossoverPoint.ChildIndex;
      IFunctionTree branch0 = crossoverPoint.Parent0.SubTrees[childIndex];
      IFunctionTree branch1 = crossoverPoint.Parent1.SubTrees[childIndex];
      // exchange the branches in the parent
      parent0.RemoveSubTree(childIndex);
      parent0.InsertSubTree(childIndex, branch1);
      parent1.RemoveSubTree(childIndex);
      parent1.InsertSubTree(childIndex, branch0);

      ExchangeChildren(branch0, branch1);
    }

    private void SwapSubtrees(CrossoverPoint crossoverPoint) {
      IFunctionTree parent0 = crossoverPoint.Parent0;
      IFunctionTree parent1 = crossoverPoint.Parent1;
      int childIndex = crossoverPoint.ChildIndex;
      IFunctionTree branch0 = crossoverPoint.Parent0.SubTrees[childIndex];
      IFunctionTree branch1 = crossoverPoint.Parent1.SubTrees[childIndex];
      // insert branch1 into parent0 replacing branch0
      parent0.RemoveSubTree(childIndex);
      parent0.InsertSubTree(childIndex, branch1);
      // insert branch0 into parent1 replacing branch1
      parent1.RemoveSubTree(childIndex);
      parent1.InsertSubTree(childIndex, branch0);
    }

    private void ExchangeChildren(IFunctionTree branch0, IFunctionTree branch1) {
      List<IFunctionTree> branch0Children = new List<IFunctionTree>(branch0.SubTrees); // lists to backup subtrees
      List<IFunctionTree> branch1Children = new List<IFunctionTree>(branch1.SubTrees);

      // remove children of branch0 and branch1
      while (branch1.SubTrees.Count > 0) branch1.RemoveSubTree(0);
      while (branch0.SubTrees.Count > 0) branch0.RemoveSubTree(0);

      // add original children of branch0 to branch1
      foreach (IFunctionTree subTree in branch0Children) {
        branch1.AddSubTree(subTree);
      }
      // add original children of branch1 to branch0
      foreach (IFunctionTree subTree in branch1Children) {
        branch0.AddSubTree(subTree);
      }
    }
  }
}
