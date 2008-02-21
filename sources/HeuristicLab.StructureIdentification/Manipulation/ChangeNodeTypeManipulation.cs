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

namespace HeuristicLab.StructureIdentification {
  public class ChangeNodeTypeManipulation : OperatorBase {
    public ChangeNodeTypeManipulation()
      : base() {
      AddVariableInfo(new VariableInfo("Random", "Uniform random number generator", typeof(MersenneTwister), VariableKind.In));
      AddVariableInfo(new VariableInfo("OperatorLibrary", "The operator library containing all available operators", typeof(GPOperatorLibrary), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaxTreeHeight", "The maximal allowed height of the tree", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaxTreeSize", "The maximal allowed size (number of nodes) of the tree", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("BalancedTreesRate", "Determines how many trees should be balanced", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("OperatorTree", "The tree to mutate", typeof(IOperator), VariableKind.In));
      AddVariableInfo(new VariableInfo("TreeSize", "The size (number of nodes) of the tree", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("TreeHeight", "The height of the tree", typeof(IntData), VariableKind.In));
    }


    public override IOperation Apply(IScope scope) {
      IOperator rootOperator = GetVariableValue<IOperator>("OperatorTree", scope, false);
      MersenneTwister random = GetVariableValue<MersenneTwister>("Random", scope, true);
      GPOperatorLibrary library = GetVariableValue<GPOperatorLibrary>("OperatorLibrary", scope, true);
      double balancedTreesRate = GetVariableValue<DoubleData>("BalancedTreesRate", scope, true).Data;
      IntData treeSize = GetVariableValue<IntData>("TreeSize", scope, false);
      IntData treeHeight = GetVariableValue<IntData>("TreeHeight", scope, false);
      int maxTreeSize = GetVariableValue<IntData>("MaxTreeSize", scope, true).Data;
      int maxTreeHeight = GetVariableValue<IntData>("MaxTreeHeight", scope, true).Data;

      TreeGardener gardener = new TreeGardener(random, library);

      IOperator parent = gardener.GetRandomParentNode(rootOperator);

      IOperator selectedChild;
      int selectedChildIndex;
      if (parent == null) {
        selectedChildIndex = 0;
        selectedChild = rootOperator;
      } else {
        selectedChildIndex = random.Next(parent.SubOperators.Count);
        selectedChild = parent.SubOperators[selectedChildIndex];
      }

      if (selectedChild.SubOperators.Count == 0) {
        IOperator newTerminal = ChangeTerminalType(parent, selectedChild, selectedChildIndex, gardener, random);

        if (parent == null) {
          // no parent means the new child is the initial operator
          // and we have to update the value in the variable
          scope.GetVariable("OperatorTree").Value = newTerminal;
        } else {
          parent.RemoveSubOperator(selectedChildIndex);
          parent.AddSubOperator(newTerminal, selectedChildIndex);
          // updating the variable is not necessary because it stays the same
        }

        // size and height stays the same when changing a terminal so no need to update the variables
        // schedule an operation to initialize the new terminal
        return gardener.CreateInitializationOperation(gardener.GetAllOperators(newTerminal), scope);
      } else {
        List<IOperator> uninitializedOperators;
        IOperator newFunction = ChangeFunctionType(parent, selectedChild, selectedChildIndex, gardener, random, balancedTreesRate, out uninitializedOperators);

        if (parent == null) {
          // no parent means the new function is the initial operator
          // and we have to update the value in the variable
          scope.GetVariable("OperatorTree").Value = newFunction;
          rootOperator = newFunction;
        } else {
          // remove the old child
          parent.RemoveSubOperator(selectedChildIndex);
          // add the new child as sub-tree of parent
          parent.AddSubOperator(newFunction, selectedChildIndex);
        }

        // recalculate size and height
        treeSize.Data = gardener.GetTreeSize(rootOperator);
        treeHeight.Data = gardener.GetTreeHeight(rootOperator);

        // check if the size of the new tree is still in the allowed bounds
        if (treeHeight.Data > maxTreeHeight ||
          treeSize.Data > maxTreeSize) {
          throw new InvalidProgramException();
        }

        // check if whole tree is ok
        if (!gardener.IsValidTree(rootOperator)) {
          throw new InvalidProgramException();
        }

        // return a composite operation that initializes all created sub-trees
        return gardener.CreateInitializationOperation(uninitializedOperators, scope);
      }
    }


    private IOperator ChangeTerminalType(IOperator parent, IOperator child, int childIndex, TreeGardener gardener, MersenneTwister random) {

      IList<IOperator> allowedChildren;
      if (parent == null) {
        allowedChildren = gardener.Terminals;
      } else {
        SubOperatorsConstraintAnalyser analyser = new SubOperatorsConstraintAnalyser();
        analyser.AllPossibleOperators = gardener.Terminals;
        allowedChildren = analyser.GetAllowedOperators(parent, childIndex);
      }

      // selecting from the terminals should always work since the current child was also a terminal
      // so in the worst case we will just create a new terminal of the same type again.
      return gardener.CreateRandomTree((IOperator)allowedChildren[random.Next(allowedChildren.Count)].Clone(), 1, 1, false);
    }

    private IOperator ChangeFunctionType(IOperator parent, IOperator child, int childIndex, TreeGardener gardener, MersenneTwister random,
      double balancedTreesRate, out List<IOperator> uninitializedOperators) {
      // since there are suboperators, we have to check which 
      // and how many of the existing suboperators we can reuse

      // let's choose the operator we want to use instead of the old child. For this we have to determine the
      // pool of allowed operators based on constraints of the parent if there is one.
      IList<IOperator> allowedSubOperators;
      SubOperatorsConstraintAnalyser analyser = new SubOperatorsConstraintAnalyser();
      analyser.AllPossibleOperators = gardener.AllOperators;
      if (parent == null) {
        allowedSubOperators = gardener.AllOperators;
      } else {
        allowedSubOperators = analyser.GetAllowedOperators(parent, childIndex);
      }

      // try to make a tree with the same arity as the old child.
      int actualArity = child.SubOperators.Count;
      // arity of the selected operator
      int minArity;
      int maxArity;

      allowedSubOperators = allowedSubOperators.Where(f => {
        gardener.GetMinMaxArity(f, out minArity, out maxArity);
        return minArity <= actualArity;
      }).ToList();

      IOperator newOperator = (IOperator)allowedSubOperators[random.Next(allowedSubOperators.Count)].Clone();

      gardener.GetMinMaxArity(newOperator, out minArity, out maxArity);
      // if the old child had too many sub-operators then make the new child with the maximal arity
      if (actualArity > maxArity)
        actualArity = maxArity;

      // get the allowed size and height for new sub-trees
      // use the size of the smallest subtree as the maximal allowed size for new subtrees to
      // prevent that we ever create trees over the MaxTreeSize limit
      int maxSubTreeSize = child.SubOperators.Select(subOp => gardener.GetTreeSize(subOp)).Min();
      int maxSubTreeHeight = gardener.GetTreeHeight(child) - 1;

      // create a list that holds old sub-trees that we can reuse in the new tree
      List<IOperator> availableSubOperators = new List<IOperator>(child.SubOperators);
      List<IOperator> freshSubTrees = new List<IOperator>() { newOperator };

      // randomly select the suboperators that we keep
      for (int i = 0; i < actualArity; i++) {
        // fill all sub-operator slots of the new operator
        // if for a given slot i there are existing sub-operators that can be used in that slot
        // then use a random existing sub-operator. When there are no existing sub-operators
        // that fit in the given slot then create a new random tree and use it for the slot
        IList<IOperator> allowedOperators = analyser.GetAllowedOperators(newOperator, i);
        var matchingOperators = availableSubOperators.Where(subOp => allowedOperators.Contains(subOp, new TreeGardener.OperatorEqualityComparer()));

        if (matchingOperators.Count() > 0) {
          IOperator selectedSubOperator = matchingOperators.ElementAt(random.Next(matchingOperators.Count()));
          // we can just add it as suboperator
          newOperator.AddSubOperator(selectedSubOperator, i);
          availableSubOperators.Remove(selectedSubOperator); // the operator shouldn't be available for the following slots
        } else {
          IOperator freshOperatorTree;
          if(random.NextDouble() <= balancedTreesRate) {
            freshOperatorTree = gardener.CreateRandomTree(allowedOperators, maxSubTreeSize, maxSubTreeHeight, true);
          } else {
            freshOperatorTree = gardener.CreateRandomTree(allowedOperators, maxSubTreeSize, maxSubTreeHeight, false);
          }
          freshSubTrees.AddRange(gardener.GetAllOperators(freshOperatorTree));

          newOperator.AddSubOperator(freshOperatorTree, i);
        }
      }

      uninitializedOperators = freshSubTrees;
      return newOperator;
    }
  }
}
