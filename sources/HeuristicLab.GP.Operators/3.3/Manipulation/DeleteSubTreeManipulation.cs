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
using HeuristicLab.Random;
using System.Diagnostics;
using HeuristicLab.GP.Interfaces;

namespace HeuristicLab.GP.Operators {
  public class DeleteSubTreeManipulation : GPManipulatorBase {
    public override string Description {
      get {
        return @"Deletes a random sub-tree of the input tree. If the remaining tree is not valid 
the operator tries to fix the tree by generating random subtrees where necessary.";
      }
    }

    public DeleteSubTreeManipulation()
      : base() {
    }

    internal override IOperation Manipulate(MersenneTwister random, IGeneticProgrammingModel gpModel, FunctionLibrary library, int maxTreeSize, int maxTreeHeight, IScope scope) {
      TreeGardener gardener = new TreeGardener(random, library);
      IFunctionTree parent = gardener.GetRandomParentNode(gpModel.FunctionTree);

      // parent==null means the whole tree should be deleted.
      // => return a new minimal random tree
      if (parent == null) {
        IFunctionTree newTree = gardener.CreateBalancedRandomTree(1, 1);
        // check if the tree is ok
        Debug.Assert(gardener.IsValidTree(newTree));
        gpModel.FunctionTree = newTree;
        // schedule an operation to initialize the newly created operator
        return Util.CreateInitializationOperation(TreeGardener.GetAllSubTrees(newTree), scope);
      }

      // select a branch to prune
      int childIndex = random.Next(parent.SubTrees.Count);
      if (parent.SubTrees.Count > parent.Function.MinSubTrees) {
        parent.RemoveSubTree(childIndex);
        // actually since the next sub-trees are shifted in the place of the removed branch
        // it might be possible that these sub-trees are not allowed in the place of the old branch
        // we ignore this problem for now.
        // when this starts to become a problem a possible solution is to go through the shifted branches from the place of the shifted
        // and find the first one that doesn't fit. At this position we insert a new randomly initialized subtree of matching type (gkronber 25.12.07)

        Debug.Assert(gardener.IsValidTree(gpModel.FunctionTree));
        // recalculate size and height
        gpModel.Size = gpModel.FunctionTree.GetSize();
        gpModel.Height = gpModel.FunctionTree.GetHeight();
        // root hasn't changed so don't need to update 'FunctionTree' variable
        return null;
      } else {
        // replace with a minimal random seedling
        parent.RemoveSubTree(childIndex);
        ICollection<IFunction> allowedFunctions = gardener.GetAllowedSubFunctions(parent.Function, childIndex);
        IFunctionTree newFunctionTree = gardener.CreateRandomTree(allowedFunctions, 1, 1);
        parent.InsertSubTree(childIndex, newFunctionTree);
        Debug.Assert(gardener.IsValidTree(gpModel.FunctionTree));
        // recalculate size and height
        gpModel.Size = gpModel.FunctionTree.GetSize();
        gpModel.Height = gpModel.FunctionTree.GetHeight();
        // return an initialization operation for the newly created tree
        return Util.CreateInitializationOperation(TreeGardener.GetAllSubTrees(newFunctionTree), scope);
      }
    }
  }
}
