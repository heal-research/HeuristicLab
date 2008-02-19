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

namespace HeuristicLab.StructureIdentification {
  public class CutOutNodeManipulation : OperatorBase {
    public override string Description {
      get {
        return @"Takes a tree, selects a random node of the tree and then tries to replace a random child 
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
      AddVariableInfo(new VariableInfo("BalanceTrees", "Determines if the trees should be balanced", typeof(BoolData), VariableKind.In));
      AddVariableInfo(new VariableInfo("OperatorTree", "The tree to mutate", typeof(IOperator), VariableKind.In));
      AddVariableInfo(new VariableInfo("TreeSize", "The size (number of nodes) of the tree", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("TreeHeight", "The height of the tree", typeof(IntData), VariableKind.In));
    }


    public override IOperation Apply(IScope scope) {
      IOperator rootOperator = GetVariableValue<IOperator>("OperatorTree", scope, true);
      MersenneTwister random = GetVariableValue<MersenneTwister>("Random", scope, true);
      GPOperatorLibrary library = GetVariableValue<GPOperatorLibrary>("OperatorLibrary", scope, true);
      int maxTreeHeight = GetVariableValue<IntData>("MaxTreeHeight", scope, true).Data;
      int maxTreeSize = GetVariableValue<IntData>("MaxTreeSize", scope, true).Data;
      bool balanceTrees = GetVariableValue<BoolData>("BalanceTrees", scope, true).Data;

      TreeGardener gardener = new TreeGardener(random, library);
      IOperator parent = gardener.GetRandomParentNode(rootOperator);
      // parent == null means we should cut out the root node
      // => return a random suboperator of the root
      if (parent == null) {
        // when there are suboperators then replace the old operator with a random suboperator
        if (rootOperator.SubOperators.Count > 0) {
          rootOperator = rootOperator.SubOperators[random.Next(rootOperator.SubOperators.Count)];

          GetVariableValue<IntData>("TreeSize", scope, true).Data = gardener.GetTreeSize(rootOperator);
          GetVariableValue<IntData>("TreeHeight", scope, true).Data = gardener.GetTreeHeight(rootOperator);

          // this is not really necessary (we can leave it in until the operator is stable)
          if (!gardener.IsValidTree(rootOperator)) {
            throw new InvalidProgramException();
          }

          // update the variable
          scope.GetVariable("OperatorTree").Value = rootOperator;
          if (!gardener.IsValidTree(rootOperator)) {
            throw new InvalidProgramException();
          }


          // the tree is already initialized so we don't have to schedule initialization operations
          return null;
        } else {
          // create a new random tree
          IOperator newOperator = gardener.CreateRandomTree(gardener.AllOperators, maxTreeSize, maxTreeHeight, balanceTrees);

          GetVariableValue<IntData>("TreeSize", scope, true).Data = gardener.GetTreeSize(newOperator);
          GetVariableValue<IntData>("TreeHeight", scope, true).Data = gardener.GetTreeHeight(newOperator);

          if (!gardener.IsValidTree(newOperator)) {
            throw new InvalidProgramException();
          }

          // update the variable
          scope.GetVariable("OperatorTree").Value = newOperator;

          if (!gardener.IsValidTree(newOperator)) {
            throw new InvalidProgramException();
          }

          // schedule an operation to initialize the whole operator graph
          return gardener.CreateInitializationOperation(gardener.GetAllOperators(newOperator), scope);
        }
      }

      int childIndex = random.Next(parent.SubOperators.Count);
      IOperator child = parent.SubOperators[childIndex];

      // match the suboperators of the child with the allowed suboperators of the parent
      IOperator[] possibleChilds = gardener.GetAllowedSubOperators(parent, childIndex).SelectMany(allowedOp => child.SubOperators
        .Where(subOp => ((StringData)subOp.GetVariable(GPOperatorLibrary.TYPE_ID).Value).Data ==
          ((StringData)allowedOp.GetVariable(GPOperatorLibrary.TYPE_ID).Value).Data)).ToArray();


      if (possibleChilds.Length > 0) {
        // replace child with a random child of the child
        // make a clone to simplify removing obsolete operators from the operator-graph
        IOperator selectedChild = (IOperator)possibleChilds[random.Next(possibleChilds.Length)].Clone();        
        parent.RemoveSubOperator(childIndex);
        parent.AddSubOperator(selectedChild, childIndex);

        if (!gardener.IsValidTree(rootOperator)) {
          throw new InvalidProgramException();
        }

        // update the size and height of our tree
        GetVariableValue<IntData>("TreeSize", scope, true).Data = gardener.GetTreeSize(rootOperator);
        GetVariableValue<IntData>("TreeHeight", scope, true).Data = gardener.GetTreeHeight(rootOperator);
        // don't need to schedule initialization operations
        return null;
      } else {
        // determine the level of the parent
        int parentLevel = gardener.GetNodeLevel(rootOperator, parent);

        // first remove the old child (first step essential!)
        parent.RemoveSubOperator(childIndex);
        // then determine the number of nodes left over after the child has been removed!
        int remainingNodes = gardener.GetTreeSize(rootOperator);

        IList<IOperator> allowedOperators = gardener.GetAllowedSubOperators(parent, childIndex);
        IOperator newOperatorTree = gardener.CreateRandomTree(allowedOperators, maxTreeSize - remainingNodes, maxTreeHeight - parentLevel, true);

        parent.AddSubOperator(newOperatorTree, childIndex);

        GetVariableValue<IntData>("TreeSize", scope, true).Data = gardener.GetTreeSize(rootOperator);
        GetVariableValue<IntData>("TreeHeight", scope, true).Data = gardener.GetTreeHeight(rootOperator);

        if (!gardener.IsValidTree(rootOperator)) {
          throw new InvalidProgramException();
        }

        // schedule an initialization operation for the new operator
        return gardener.CreateInitializationOperation(gardener.GetAllOperators(newOperatorTree), scope);
      }
    }
  }
}
