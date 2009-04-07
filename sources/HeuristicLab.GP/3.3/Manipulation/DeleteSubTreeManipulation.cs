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
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Random;
using System.Diagnostics;

namespace HeuristicLab.GP {
  public class DeleteSubTreeManipulation : OperatorBase {
    public override string Description {
      get {
        return @"Deletes a random sub-tree of the input tree. If the remaining tree is not valid 
the operator tries to fix the tree by generating random subtrees where necessary.";
      }
    }

    public DeleteSubTreeManipulation()
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
      TreeGardener gardener = new TreeGardener(random, library);
      IFunctionTree parent = gardener.GetRandomParentNode(root);

      // parent==null means the whole tree should be deleted.
      // => return a new minimal random tree
      if(parent == null) {
        IFunctionTree newTree = gardener.CreateBalancedRandomTree(1, 1);
        // check if the tree is ok
        Debug.Assert(gardener.IsValidTree(newTree));
        // update sizes to match the new tree
        GetVariableValue<IntData>("TreeSize", scope, true).Data = newTree.Size;
        GetVariableValue<IntData>("TreeHeight", scope, true).Data = newTree.Height;
        scope.GetVariable(scope.TranslateName("FunctionTree")).Value = newTree;

        // schedule an operation to initialize the newly created operator
        return gardener.CreateInitializationOperation(gardener.GetAllSubTrees(newTree), scope);
      }

      // select a branch to prune
      int childIndex = random.Next(parent.SubTrees.Count);
      if(parent.SubTrees.Count > parent.Function.MinArity) {
        parent.RemoveSubTree(childIndex);
        // actually since the next sub-trees are shifted in the place of the removed branch
        // it might be possible that these sub-trees are not allowed in the place of the old branch
        // we ignore this problem for now.
        // when this starts to become a problem a possible solution is to go through the shifted branches from the place of the shifted
        // and find the first one that doesn't fit. At this position we insert a new randomly initialized subtree of matching type (gkronber 25.12.07)

        Debug.Assert(gardener.IsValidTree(root));
        GetVariableValue<IntData>("TreeSize", scope, true).Data = root.Size;
        GetVariableValue<IntData>("TreeHeight", scope, true).Data = root.Height;
        // root hasn't changed so don't need to update 'FunctionTree' variable
        return null;
      } else {
        // replace with a minimal random seedling
        parent.RemoveSubTree(childIndex);
        ICollection<IFunction> allowedFunctions = gardener.GetAllowedSubFunctions(parent.Function, childIndex);
        IFunctionTree newFunctionTree = gardener.CreateRandomTree(allowedFunctions, 1, 1);
        parent.InsertSubTree(childIndex, newFunctionTree);
        Debug.Assert(gardener.IsValidTree(root));
        GetVariableValue<IntData>("TreeSize", scope, true).Data = root.Size;
        GetVariableValue<IntData>("TreeHeight", scope, true).Data = root.Height;
        // again the root hasn't changed so we don't need to update the 'FunctionTree' variable
        // but we have to return an initialization operation for the newly created tree
        return gardener.CreateInitializationOperation(gardener.GetAllSubTrees(newFunctionTree), scope);
      }
    }
  }
}
