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
using HeuristicLab.Functions;

namespace HeuristicLab.StructureIdentification {
  public class ChangeNodeTypeManipulation : OperatorBase {

    public override string Description {
      get {
        return @"This manipulation operator selects a random tree-node and changes the function type.
If this leads to a constraint-violation (wrong number or type of sub-trees) the sub-trees are repaired
resulting in a valid tree again.";
      }
    }

    public ChangeNodeTypeManipulation()
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
      IFunctionTree root = GetVariableValue<IFunctionTree>("FunctionTree", scope, false);
      MersenneTwister random = GetVariableValue<MersenneTwister>("Random", scope, true);
      GPOperatorLibrary library = GetVariableValue<GPOperatorLibrary>("OperatorLibrary", scope, true);
      IntData treeSize = GetVariableValue<IntData>("TreeSize", scope, false);
      IntData treeHeight = GetVariableValue<IntData>("TreeHeight", scope, false);
      int maxTreeSize = GetVariableValue<IntData>("MaxTreeSize", scope, true).Data;
      int maxTreeHeight = GetVariableValue<IntData>("MaxTreeHeight", scope, true).Data;
      TreeGardener gardener = new TreeGardener(random, library);
      IFunctionTree parent = gardener.GetRandomParentNode(root);
      IFunctionTree selectedChild;
      int selectedChildIndex;
      if (parent == null) {
        selectedChildIndex = 0;
        selectedChild = root;
      } else {
        selectedChildIndex = random.Next(parent.SubTrees.Count);
        selectedChild = parent.SubTrees[selectedChildIndex];
      }

      if (selectedChild.SubTrees.Count == 0) {
        IFunctionTree newTerminal = ChangeTerminalType(parent, selectedChild, selectedChildIndex, gardener, random);
        if (parent == null) {
          // no parent means the new child is the initial operator
          // and we have to update the value in the variable
          scope.GetVariable(scope.TranslateName("FunctionTree")).Value = newTerminal;
        } else {
          parent.RemoveSubTree(selectedChildIndex);
          parent.InsertSubTree(selectedChildIndex, newTerminal);
          // updating the variable is not necessary because it stays the same
        }
        if(!gardener.IsValidTree(root)) throw new InvalidProgramException();
        // size and height stays the same when changing a terminal so no need to update the variables
        // schedule an operation to initialize the new terminal
        return gardener.CreateInitializationOperation(gardener.GetAllSubTrees(newTerminal), scope);
      } else {
        List<IFunctionTree> uninitializedBranches;
        IFunctionTree newFunction = ChangeFunctionType(parent, selectedChild, selectedChildIndex, gardener, random, out uninitializedBranches);
        if (parent == null) {
          // no parent means the new function is the initial operator
          // and we have to update the value in the variable
          scope.GetVariable(scope.TranslateName("FunctionTree")).Value = newFunction;
          root = newFunction;
        } else {
          // remove the old child
          parent.RemoveSubTree(selectedChildIndex);
          // add the new child as sub-tree of parent
          parent.InsertSubTree(selectedChildIndex, newFunction);
        }
        // recalculate size and height
        treeSize.Data = gardener.GetTreeSize(root);
        treeHeight.Data = gardener.GetTreeHeight(root);
        // check if whole tree is ok
        // check if the size of the new tree is still in the allowed bounds
        if (!gardener.IsValidTree(root) ||
          treeHeight.Data > maxTreeHeight ||
          treeSize.Data > maxTreeSize) {
          throw new InvalidProgramException();
        }
        // return a composite operation that initializes all created sub-trees
        return gardener.CreateInitializationOperation(uninitializedBranches, scope);
      }
    }

    private IFunctionTree ChangeTerminalType(IFunctionTree parent, IFunctionTree child, int childIndex, TreeGardener gardener, MersenneTwister random) {
      IList<IFunction> allowedChildren;
      if (parent == null) {
        allowedChildren = gardener.Terminals;
      } else {
        allowedChildren = new List<IFunction>();
        var allAllowedChildren = gardener.GetAllowedSubFunctions(parent.Function, childIndex);
        foreach(IFunction c in allAllowedChildren) {
          if(gardener.IsTerminal(c)) allowedChildren.Add(c);
        }
      }
      // selecting from the terminals should always work since the current child was also a terminal
      // so in the worst case we will just create a new terminal of the same type again.
      return gardener.CreateRandomTree(allowedChildren[random.Next(allowedChildren.Count)], 1, 1, false);
    }

    private IFunctionTree ChangeFunctionType(IFunctionTree parent, IFunctionTree child, int childIndex, TreeGardener gardener, MersenneTwister random,
      out List<IFunctionTree> uninitializedBranches) {
      // since there are subtrees, we have to check which 
      // and how many of the existing subtrees we can reuse.
      // first let's choose the function we want to use instead of the old child. For this we have to determine the
      // pool of allowed functions based on constraints of the parent if there is one.
      IList<IFunction> allowedFunctions = gardener.GetAllowedSubFunctions(parent!=null?parent.Function:null, childIndex);
      // try to make a tree with the same arity as the old child.
      int actualArity = child.SubTrees.Count;
      // arity of the selected operator
      int minArity;
      int maxArity;
      // only allow functions where we can keep all existing sub-trees
      // we don't want to create new sub-trees here 
      // this restriction can be removed if we add code that creates sub-trees where necessary (gkronber 22.04.08)
      allowedFunctions = allowedFunctions.Where(f => {
        gardener.GetMinMaxArity(f, out minArity, out maxArity);
        return minArity <= actualArity;
      }).ToList();
      // create a new tree-node for a randomly selected function
      IFunctionTree newTree = new FunctionTree(allowedFunctions[random.Next(allowedFunctions.Count)]);
      gardener.GetMinMaxArity(newTree.Function, out minArity, out maxArity);
      // if the old child had too many sub-trees then the new child should keep as many sub-trees as possible
      if (actualArity > maxArity)
        actualArity = maxArity;
      // get the allowed size and height for new sub-trees
      // use the size of the smallest subtree as the maximal allowed size for new subtrees to
      // prevent that we ever create trees over the MaxTreeSize limit
      int maxSubTreeSize = child.SubTrees.Select(subTree => gardener.GetTreeSize(subTree)).Min();
      int maxSubTreeHeight = gardener.GetTreeHeight(child) - 1;
      // create a list that holds old sub-trees that we can reuse in the new tree
      List<IFunctionTree> availableSubTrees = new List<IFunctionTree>(child.SubTrees);
      List<IFunctionTree> freshSubTrees = new List<IFunctionTree>() { newTree };
      // randomly select the sub-trees that we keep
      for (int i = 0; i < actualArity; i++) {
        // fill all sub-tree slots of the new tree
        // if for a given slot i there are multiple existing sub-trees that can be used in that slot
        // then use a random existing sub-tree. When there are no existing sub-trees
        // that fit in the given slot then create a new random tree and use it for the slot
        ICollection<IFunction> allowedSubFunctions = gardener.GetAllowedSubFunctions(newTree.Function, i);
        var matchingSubTrees = availableSubTrees.Where(subTree => allowedSubFunctions.Contains(subTree.Function));
        if (matchingSubTrees.Count() > 0) {
          IFunctionTree selectedSubTree = matchingSubTrees.ElementAt(random.Next(matchingSubTrees.Count()));
          // we can just add it as subtree
          newTree.InsertSubTree(i, selectedSubTree);
          availableSubTrees.Remove(selectedSubTree); // the branch shouldn't be available for the following slots
        } else {
          // no existing matching tree found => create a new one
          IFunctionTree freshTree = gardener.CreateRandomTree(allowedSubFunctions, maxSubTreeSize, maxSubTreeHeight);
          freshSubTrees.AddRange(gardener.GetAllSubTrees(freshTree));
          newTree.InsertSubTree(i, freshTree);
        }
      }
      uninitializedBranches = freshSubTrees;
      return newTree;
    }
  }
}
