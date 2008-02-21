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

namespace HeuristicLab.StructureIdentification {
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
      AddVariableInfo(new VariableInfo("OperatorTree", "The tree to mutate", typeof(IOperator), VariableKind.In | VariableKind.Out));
      AddVariableInfo(new VariableInfo("TreeSize", "The size (number of nodes) of the tree", typeof(IntData), VariableKind.In | VariableKind.Out));
      AddVariableInfo(new VariableInfo("TreeHeight", "The height of the tree", typeof(IntData), VariableKind.In | VariableKind.Out));
    }


    public override IOperation Apply(IScope scope) {
      IOperator rootOperator = GetVariableValue<IOperator>("OperatorTree", scope, true);
      MersenneTwister random = GetVariableValue<MersenneTwister>("Random", scope, true);
      GPOperatorLibrary library = GetVariableValue<GPOperatorLibrary>("OperatorLibrary", scope, true);

      TreeGardener gardener = new TreeGardener(random, library);

      IOperator parent = gardener.GetRandomParentNode(rootOperator);

      // parent==null means the whole tree should be deleted.
      // => return a new minimal random tree
      if(parent == null) {
        IOperator newTree = gardener.CreateRandomTree(1, 1, true);

        // check if the tree is ok
        if(!gardener.IsValidTree(newTree)) {
          throw new InvalidOperationException();
        }

        // update sizes to match the new tree
        GetVariableValue<IntData>("TreeSize", scope, true).Data = gardener.GetTreeSize(newTree);
        GetVariableValue<IntData>("TreeHeight", scope, true).Data = gardener.GetTreeHeight(newTree);
        scope.GetVariable("OperatorTree").Value = newTree;

        // schedule an operation to initialize the newly created operator
        return gardener.CreateInitializationOperation(gardener.GetAllOperators(newTree), scope);
      }

      int childIndex = random.Next(parent.SubOperators.Count);

      int min;
      int max;
      gardener.GetMinMaxArity(parent, out min, out max);
      if(parent.SubOperators.Count > min) {
        parent.RemoveSubOperator(childIndex);
        // actually since the next suboperators are shifted in the place of the removed operator
        // it might be possible that these suboperators are not allowed in the place of the old operator
        // we ignore this problem for now.
        // when this starts to become a problem a possible solution is to go through the shifted operators from the place of the shifted
        // and find the first one that doesn't fit. At this position we insert a new randomly initialized subtree of matching type (gkronber 25.12.07)

        if(!gardener.IsValidTree(rootOperator)) {
          throw new InvalidOperationException();
        }

        GetVariableValue<IntData>("TreeSize", scope, true).Data = gardener.GetTreeSize(rootOperator);
        GetVariableValue<IntData>("TreeHeight", scope, true).Data = gardener.GetTreeHeight(rootOperator);
        // root hasn't changed so don't need to update 'OperatorTree' variable

        return null;
      } else {
        // replace with a minimal random seedling
        parent.RemoveSubOperator(childIndex);

        IList<IOperator> allowedOperators = gardener.GetAllowedSubOperators(parent, childIndex);
        IOperator newOperatorTree = gardener.CreateRandomTree(allowedOperators, 1, 1, true);

        parent.AddSubOperator(newOperatorTree, childIndex);

        if(!gardener.IsValidTree(rootOperator)) {
          throw new InvalidProgramException();
        }

        GetVariableValue<IntData>("TreeSize", scope, true).Data = gardener.GetTreeSize(rootOperator);
        GetVariableValue<IntData>("TreeHeight", scope, true).Data = gardener.GetTreeHeight(rootOperator);
        // again the root hasn't changed so we don't need to update the 'OperatorTree' variable

        return gardener.CreateInitializationOperation(gardener.GetAllOperators(newOperatorTree), scope);
      }
    }
  }
}
