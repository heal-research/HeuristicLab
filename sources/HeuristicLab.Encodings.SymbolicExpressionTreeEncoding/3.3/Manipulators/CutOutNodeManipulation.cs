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
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Random;
using System.Diagnostics;
using HeuristicLab.GP.Interfaces;

namespace HeuristicLab.Encodings.SymbolicExpressionTree {
  public class CutOutNodeManipulation : GPManipulatorBase {
    public override string Description {
      get {
        return @"Takes a tree, selects a random node of the tree and then tries to replace a random sub-tree
of that node with one of the childs of the selected child.

               O                             O
              / \                           / \
             O   X                         O   2
                / \    2 is selected =>       / \
               1   2                         4   5
              /   / \
             3   4   5
";
      }
    }

    public CutOutNodeManipulation()
      : base() {
    }


    internal override IOperation Manipulate(MersenneTwister random, IGeneticProgrammingModel gpModel, FunctionLibrary library, int maxTreeSize, int maxTreeHeight, IScope scope) {
      TreeGardener gardener = new TreeGardener(random, library);
      IFunctionTree parent = gardener.GetRandomParentNode(gpModel.FunctionTree);
      // parent == null means we should cut out the root node
      // => return a random sub-tree of the root
      if (parent == null) {
        // when there are sub-trees then replace the old tree with a random sub-tree
        if (gpModel.FunctionTree.SubTrees.Count > 0) {
          gpModel.FunctionTree = gpModel.FunctionTree.SubTrees[random.Next(gpModel.FunctionTree.SubTrees.Count)];
          Debug.Assert(gardener.IsValidTree(gpModel.FunctionTree));
          // we reused a sub-tree so we don't have to schedule initialization operations
          return null;
        } else {
          // we want to cut the root node and there are no sub-trees => create a new random terminal
          gpModel.FunctionTree = gardener.CreateRandomTree(gardener.Terminals, 1, 1);
          Debug.Assert(gardener.IsValidTree(gpModel.FunctionTree));

          // schedule an operation to initialize the whole tree
          return Util.CreateInitializationOperation(TreeGardener.GetAllSubTrees(gpModel.FunctionTree), scope);
        }
      }

      // select a child to cut away
      int childIndex = random.Next(parent.SubTrees.Count);
      IFunctionTree child = parent.SubTrees[childIndex];
      // match the sub-trees of the child with the allowed sub-trees of the parent
      ICollection<IFunction> allowedFunctions = gardener.GetAllowedSubFunctions(parent.Function, childIndex);
      IFunctionTree[] possibleChilds = child.SubTrees.Where(t => allowedFunctions.Contains(t.Function)).ToArray();
      if (possibleChilds.Length > 0) {
        // replace child with a random child of that child
        IFunctionTree selectedChild = possibleChilds[random.Next(possibleChilds.Length)];
        parent.RemoveSubTree(childIndex);
        parent.InsertSubTree(childIndex, selectedChild);
        Debug.Assert(gardener.IsValidTree(gpModel.FunctionTree));
        // recalculate the size and height of our tree
        gpModel.Size = gpModel.FunctionTree.GetSize();
        gpModel.Height = gpModel.FunctionTree.GetHeight();
        // don't need to schedule initialization operations
        return null;
      } else {
        // can't reuse an existing branch => create a new tree
        // determine the level of the parent
        int parentLevel = gardener.GetBranchLevel(gpModel.FunctionTree, parent);
        // first remove the old child (first step essential!)
        parent.RemoveSubTree(childIndex);
        // then determine the number of nodes left over after the child has been removed!
        int remainingNodes = gpModel.FunctionTree.GetSize();
        allowedFunctions = gardener.GetAllowedSubFunctions(parent.Function, childIndex);
        IFunctionTree newFunctionTree = gardener.CreateRandomTree(allowedFunctions, maxTreeSize - remainingNodes, maxTreeHeight - parentLevel);
        parent.InsertSubTree(childIndex, newFunctionTree);
        Debug.Assert(gardener.IsValidTree(gpModel.FunctionTree));
        // recalculate size and height
        gpModel.Size = gpModel.FunctionTree.GetSize();
        gpModel.Height = gpModel.FunctionTree.GetHeight();
        // schedule an initialization operation for the new function-tree
        return Util.CreateInitializationOperation(TreeGardener.GetAllSubTrees(newFunctionTree), scope);
      }
    }
  }
}
