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
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Random;
using System;
using HeuristicLab.Functions;

namespace HeuristicLab.StructureIdentification {
  public class CutOutNodeManipulation : OperatorBase {
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
      AddVariableInfo(new VariableInfo("Random", "Uniform random number generator", typeof(MersenneTwister), VariableKind.In));
      AddVariableInfo(new VariableInfo("OperatorLibrary", "The operator library containing all available operators", typeof(GPOperatorLibrary), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaxTreeHeight", "The maximal allowed height of the tree", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaxTreeSize", "The maximal allowed size (number of nodes) of the tree", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("FunctionTree", "The tree to mutate", typeof(IFunctionTree), VariableKind.In | VariableKind.Out));
      AddVariableInfo(new VariableInfo("TreeSize", "The size (number of nodes) of the tree", typeof(IntData), VariableKind.In | VariableKind.Out));
      AddVariableInfo(new VariableInfo("TreeHeight", "The height of the tree", typeof(IntData), VariableKind.In | VariableKind.Out));
    }


    public override IOperation Apply(IScope scope) {
      IFunctionTree root = GetVariableValue<IFunctionTree>("FunctionTree", scope, true);
      MersenneTwister random = GetVariableValue<MersenneTwister>("Random", scope, true);
      GPOperatorLibrary library = GetVariableValue<GPOperatorLibrary>("OperatorLibrary", scope, true);
      int maxTreeHeight = GetVariableValue<IntData>("MaxTreeHeight", scope, true).Data;
      int maxTreeSize = GetVariableValue<IntData>("MaxTreeSize", scope, true).Data;
      TreeGardener gardener = new TreeGardener(random, library);
      IFunctionTree parent = gardener.GetRandomParentNode(root);
      // parent == null means we should cut out the root node
      // => return a random sub-tree of the root
      if (parent == null) {
        // when there are sub-trees then replace the old tree with a random sub-tree
        if (root.SubTrees.Count > 0) {
          root = root.SubTrees[random.Next(root.SubTrees.Count)];
          GetVariableValue<IntData>("TreeSize", scope, true).Data = gardener.GetTreeSize(root);
          GetVariableValue<IntData>("TreeHeight", scope, true).Data = gardener.GetTreeHeight(root);
          // update the variable
          scope.GetVariable(scope.TranslateName("FunctionTree")).Value = root;
          if (!gardener.IsValidTree(root)) {
            throw new InvalidProgramException();
          }
          // we reused a sub-tree so we don't have to schedule initialization operations
          return null;
        } else {
          // we want to cut the root node and there are no sub-trees => create a new random terminal
          IFunctionTree newTree;
          newTree = gardener.CreateRandomTree(gardener.Terminals, 1, 1);
          GetVariableValue<IntData>("TreeSize", scope, true).Data = gardener.GetTreeSize(newTree);
          GetVariableValue<IntData>("TreeHeight", scope, true).Data = gardener.GetTreeHeight(newTree);
          // update the variable
          scope.GetVariable(scope.TranslateName("FunctionTree")).Value = newTree;
          if (!gardener.IsValidTree(newTree)) {
            throw new InvalidProgramException();
          }
          // schedule an operation to initialize the whole tree
          return gardener.CreateInitializationOperation(gardener.GetAllSubTrees(newTree), scope);
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
        if (!gardener.IsValidTree(root)) {
          throw new InvalidProgramException();
        }
        // update the size and height of our tree
        GetVariableValue<IntData>("TreeSize", scope, true).Data = gardener.GetTreeSize(root);
        GetVariableValue<IntData>("TreeHeight", scope, true).Data = gardener.GetTreeHeight(root);
        // don't need to schedule initialization operations
        return null;
      } else {
        // can't reuse an existing branch => create a new tree
        // determine the level of the parent
        int parentLevel = gardener.GetBranchLevel(root, parent);
        // first remove the old child (first step essential!)
        parent.RemoveSubTree(childIndex);
        // then determine the number of nodes left over after the child has been removed!
        int remainingNodes = gardener.GetTreeSize(root);
        allowedFunctions = gardener.GetAllowedSubFunctions(parent.Function, childIndex);
        IFunctionTree newFunctionTree = gardener.CreateRandomTree(allowedFunctions, maxTreeSize - remainingNodes, maxTreeHeight - parentLevel);
        parent.InsertSubTree(childIndex, newFunctionTree);
        GetVariableValue<IntData>("TreeSize", scope, true).Data = gardener.GetTreeSize(root);
        GetVariableValue<IntData>("TreeHeight", scope, true).Data = gardener.GetTreeHeight(root);
        if (!gardener.IsValidTree(root)) {
          throw new InvalidProgramException();
        }
        // schedule an initialization operation for the new function-tree
        return gardener.CreateInitializationOperation(gardener.GetAllSubTrees(newFunctionTree), scope);
      }
    }
  }
}
