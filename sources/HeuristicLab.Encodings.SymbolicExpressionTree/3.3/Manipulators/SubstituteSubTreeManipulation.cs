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
using HeuristicLab.Core;
using HeuristicLab.Random;
using System.Diagnostics;
using HeuristicLab.GP.Interfaces;

namespace HeuristicLab.GP.Operators {
  public class SubstituteSubTreeManipulation : GPManipulatorBase {

    public override string Description {
      get { return "Selects a random node of the tree and replaces it with randomly initialized subtree."; }
    }

    public SubstituteSubTreeManipulation()
      : base() {
    }

    internal override IOperation Manipulate(MersenneTwister random, IGeneticProgrammingModel gpModel, FunctionLibrary library, int maxTreeSize, int maxTreeHeight, IScope scope) {
      TreeGardener gardener = new TreeGardener(random, library);
      IFunctionTree parent = gardener.GetRandomParentNode(gpModel.FunctionTree);
      if (parent == null) {
        // parent == null means we should subsitute the whole tree
        // => create a new random tree
        IFunctionTree newTree = gardener.CreateRandomTree(gardener.AllFunctions, maxTreeSize, maxTreeHeight);
        Debug.Assert(gardener.IsValidTree(newTree));

        gpModel.FunctionTree = newTree;
        // return a CompositeOperation that randomly initializes the new tree
        return Util.CreateInitializationOperation(TreeGardener.GetAllSubTrees(newTree), scope);
      } else {
        // determine a random child of the parent to be replaced
        int childIndex = random.Next(parent.SubTrees.Count);
        // get the list of allowed functions for the new sub-tree
        ICollection<IFunction> allowedFunctions = gardener.GetAllowedSubFunctions(parent.Function, childIndex);
        if (allowedFunctions.Count == 0) {
          // don't change anything
          // this shouldn't happen
          throw new InvalidProgramException();
        }

        // calculate the maximum size and height of the new sub-tree based on the location where
        // it will be inserted
        int parentLevel = gardener.GetBranchLevel(gpModel.FunctionTree, parent);
        int maxSubTreeHeight = maxTreeHeight - parentLevel;
        int maxSubTreeSize = maxTreeSize - (gpModel.Size - parent.SubTrees[childIndex].GetSize());

        // create a random function tree
        IFunctionTree newTree = gardener.CreateRandomTree(allowedFunctions, maxSubTreeSize, maxSubTreeHeight);
        parent.RemoveSubTree(childIndex);
        parent.InsertSubTree(childIndex, newTree);

        Debug.Assert(gardener.IsValidTree(gpModel.FunctionTree));
        // update the values of treeSize and treeHeight
        gpModel.Size = gpModel.FunctionTree.GetSize();
        gpModel.Height = gpModel.FunctionTree.GetHeight();
        // the root hasn't changed so we don't need to update 
        // return a CompositeOperation that randomly initializes all nodes of the new subtree
        return Util.CreateInitializationOperation(TreeGardener.GetAllSubTrees(newTree), scope);
      }
    }
  }
}
