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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Random;

namespace HeuristicLab.StructureIdentification {
  public class SubstituteSubTreeManipulation : OperatorBase {

    public override string Description {
      get { return "Selects a random node of the tree and replaces it with randomly initialized subtree."; }
    }

    public SubstituteSubTreeManipulation()
      : base() {
      AddVariableInfo(new VariableInfo("Random", "Uniform random number generator", typeof(MersenneTwister), VariableKind.In));
      AddVariableInfo(new VariableInfo("OperatorLibrary", "The operator library containing all available operators", typeof(GPOperatorLibrary), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaxTreeHeight", "The maximal allowed height of the tree", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaxTreeSize", "The maximal allowed size (number of nodes) of the tree", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("BalanceTrees", "Determines if the trees should be balanced", typeof(BoolData), VariableKind.In));
      AddVariableInfo(new VariableInfo("OperatorTree", "The tree to manipulate", typeof(IOperator), VariableKind.In));
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
      int treeSize = GetVariableValue<IntData>("TreeSize", scope, true).Data;
      int treeHeight = GetVariableValue<IntData>("TreeHeight", scope, true).Data;

      TreeGardener gardener = new TreeGardener(random, library);

      IOperator parent = gardener.GetRandomParentNode(rootOperator);
      if(parent == null) {
        // parent == null means we should subsitute the whole tree
        // => create a new random tree

        // create a new random operator tree
        IOperator newOperatorTree = gardener.CreateRandomTree(gardener.AllOperators, maxTreeSize, maxTreeHeight, balanceTrees);

        if(!gardener.IsValidTree(newOperatorTree)) {
          throw new InvalidProgramException();
        }

        // update the variables in the scope with the new values
        GetVariableValue<IntData>("TreeSize", scope, true).Data = gardener.GetTreeSize(newOperatorTree);
        GetVariableValue<IntData>("TreeHeight", scope, true).Data = gardener.GetTreeHeight(newOperatorTree);
        scope.GetVariable("OperatorTree").Value = newOperatorTree;
        
        // return a CompositeOperation that randomly initializes the new operator
        return gardener.CreateInitializationOperation(gardener.GetAllOperators(newOperatorTree), scope);
      } else {
        // determine a random child of the parent to be replaced
        int childIndex = random.Next(parent.SubOperators.Count);

        // get the list of allowed suboperators as the new child
        IList<IOperator> allowedOperators = gardener.GetAllowedSubOperators(parent, childIndex);

        if(allowedOperators.Count == 0) {
          // don't change anything
          // this shouldn't happen
          throw new InvalidProgramException();
        }

        // calculate the maximum size and height of the new sub-tree based on the location where
        // it will be inserted
        int parentLevel = gardener.GetNodeLevel(rootOperator, parent);

        int maxSubTreeHeight = maxTreeHeight - parentLevel;
        int maxSubTreeSize = maxTreeSize - (treeSize - gardener.GetTreeSize(parent.SubOperators[childIndex]));

        // get a random operatorTree
        IOperator newOperatorTree = gardener.CreateRandomTree(allowedOperators, maxSubTreeSize, maxSubTreeHeight, balanceTrees);

        IOperator oldChild = parent.SubOperators[childIndex];
        parent.RemoveSubOperator(childIndex);
        parent.AddSubOperator(newOperatorTree, childIndex);

        if(!gardener.IsValidTree(rootOperator)) {
          throw new InvalidProgramException();
        }

        // update the values of treeSize and treeHeight
        GetVariableValue<IntData>("TreeSize", scope, true).Data = gardener.GetTreeSize(rootOperator);
        GetVariableValue<IntData>("TreeHeight", scope, true).Data = gardener.GetTreeHeight(rootOperator);
        // the root operator hasn't changed so we don't need to update 

        // return a CompositeOperation that randomly initializes all nodes of the new subtree
        return gardener.CreateInitializationOperation(gardener.GetAllOperators(newOperatorTree), scope);
      }
    }
  }
}
