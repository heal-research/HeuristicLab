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
using System.Diagnostics;

namespace HeuristicLab.StructureIdentification {
  public class SizeFairCrossOver : OperatorBase {
    private const int MAX_RECOMBINATION_TRIES = 20;
    public override string Description {
      get {
        return @"Takes two parent individuals P0 and P1 each. Selects a random node N0 of P0 and a random node N1 of P1.
And replaces the branch with root0 N0 in P0 with N1 from P1 if the tree-size limits are not violated.
When recombination with N0 and N1 would create a tree that is too large or invalid the operator randomly selects new N0 and N1 
until a valid configuration is found.";
      }
    }
    public SizeFairCrossOver()
      : base() {
      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(MersenneTwister), VariableKind.In));
      AddVariableInfo(new VariableInfo("OperatorLibrary", "The operator library containing all available operators", typeof(GPOperatorLibrary), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaxTreeHeight", "The maximal allowed height of the tree", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaxTreeSize", "The maximal allowed size (number of nodes) of the tree", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("FunctionTree", "The tree to mutate", typeof(IFunctionTree), VariableKind.In | VariableKind.New));
      AddVariableInfo(new VariableInfo("TreeSize", "The size (number of nodes) of the tree", typeof(IntData), VariableKind.New));
      AddVariableInfo(new VariableInfo("TreeHeight", "The height of the tree", typeof(IntData), VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      MersenneTwister random = GetVariableValue<MersenneTwister>("Random", scope, true);
      GPOperatorLibrary opLibrary = GetVariableValue<GPOperatorLibrary>("OperatorLibrary", scope, true);
      int maxTreeHeight = GetVariableValue<IntData>("MaxTreeHeight", scope, true).Data;
      int maxTreeSize = GetVariableValue<IntData>("MaxTreeSize", scope, true).Data;

      TreeGardener gardener = new TreeGardener(random, opLibrary);

      if((scope.SubScopes.Count % 2) != 0)
        throw new InvalidOperationException("Number of parents is not even");

      CompositeOperation initOperations = new CompositeOperation();

      int children = scope.SubScopes.Count / 2;
      for(int i = 0; i < children; i++) {
        IScope parent1 = scope.SubScopes[0];
        scope.RemoveSubScope(parent1);
        IScope parent2 = scope.SubScopes[0];
        scope.RemoveSubScope(parent2);
        IScope child = new Scope(i.ToString());
        IOperation childInitOperation = Cross(gardener, maxTreeSize, maxTreeHeight, scope, random, parent1, parent2, child);
        initOperations.AddOperation(childInitOperation);
        scope.AddSubScope(child);
      }

      return initOperations;
    }

    private IOperation Cross(TreeGardener gardener, int maxTreeSize, int maxTreeHeight,
      IScope scope, MersenneTwister random, IScope parent1, IScope parent2, IScope child) {
      List<IFunctionTree> newBranches;
      IFunctionTree newTree = Cross(gardener, parent1, parent2,
        random, maxTreeSize, maxTreeHeight, out newBranches);


      int newTreeSize = newTree.Size;
      int newTreeHeight = newTree.Height;
      child.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("FunctionTree"), newTree));
      child.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("TreeSize"), new IntData(newTreeSize)));
      child.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("TreeHeight"), new IntData(newTreeHeight)));

      // check if the new tree is valid and if the height of is still in the allowed bounds (we are not so strict for the max-size)
      Debug.Assert(gardener.IsValidTree(newTree) && newTreeHeight <= maxTreeHeight);
      return gardener.CreateInitializationOperation(newBranches, child);
    }


    private IFunctionTree Cross(TreeGardener gardener, IScope f, IScope g, MersenneTwister random, int maxTreeSize, int maxTreeHeight, out List<IFunctionTree> newBranches) {
      IFunctionTree tree0 = f.GetVariableValue<IFunctionTree>("FunctionTree", false);
      int tree0Height = f.GetVariableValue<IntData>("TreeHeight", false).Data;
      int tree0Size = f.GetVariableValue<IntData>("TreeSize", false).Data;

      IFunctionTree tree1 = g.GetVariableValue<IFunctionTree>("FunctionTree", false);
      int tree1Height = g.GetVariableValue<IntData>("TreeHeight", false).Data;
      int tree1Size = g.GetVariableValue<IntData>("TreeSize", false).Data;

      if(tree0Size == 1 && tree1Size == 1) {
        return CombineTerminals(gardener, tree0, tree1, random, maxTreeHeight, out newBranches);
      } else {
        newBranches = new List<IFunctionTree>();

        // we are going to insert tree1 into tree0 at a random place so we have to make sure that tree0 is not a terminal
        // in case both trees are higher than 1 we swap the trees with probability 50%
        if(tree0Height == 1 || (tree1Height > 1 && random.Next(2) == 0)) {
          IFunctionTree tmp = tree0; tree0 = tree1; tree1 = tmp;
          int tmpHeight = tree0Height; tree0Height = tree1Height; tree1Height = tmpHeight;
          int tmpSize = tree0Size; tree0Size = tree1Size; tree1Size = tmpSize;
        }

        // save the roots because later on we change tree0 and tree1 while searching a valid tree configuration
        IFunctionTree root0 = tree0;
        IFunctionTree root1 = tree1;
        int root0Height = tree0Height;
        int root1Height = tree1Height;
        int rootSize = tree0Size;

        // select a random suboperators of the two trees at a random level
        int tree0Level = random.Next(root0Height - 1); // since we checked before that the height of tree0 is > 1 this is OK
        int tree1Level = random.Next(root1Height);
        tree0 = gardener.GetRandomBranch(tree0, tree0Level);
        tree1 = gardener.GetRandomBranch(tree1, tree1Level);

        // recalculate the size and height of tree1 (the one that we want to insert) because we need to check constraints later on
        tree1Size = tree1.Size;
        tree1Height = tree1.Height;

        List<int> possibleChildIndices = new List<int>();

        // Now tree0 is supposed to take tree1 as one if its children. If this is not possible,
        // then go down in either of the two trees as far as possible. If even then it is not possible
        // to merge the trees then throw an exception
        // find the list of allowed indices (regarding allowed sub-trees, maxTreeSize and maxTreeHeight)
        for(int i = 0; i < tree0.SubTrees.Count; i++) {
          int subTreeSize = tree0.SubTrees[i].Size;

          // the index is ok when the function is allowed as sub-tree and we don't violate the maxSize and maxHeight constraints
          if(gardener.GetAllowedSubFunctions(tree0.Function, i).Contains(tree1.Function) &&
            rootSize - subTreeSize + tree1Size < maxTreeSize &&
            tree0Level + tree1Height < maxTreeHeight) {
            possibleChildIndices.Add(i);
          }
        }
        int tries = 0;
        while(possibleChildIndices.Count == 0) {
          if(tries++ > MAX_RECOMBINATION_TRIES) {
            if(random.Next() > 0.5) return root1;
            else return root0;
          }
          // we couln't find a possible configuration given the current tree0 and tree1
          // possible reasons for this are: 
          //  - tree1 is not allowed as sub-tree of tree0
          //  - appending tree1 as child of tree0 would create a tree that exceedes the maxTreeHeight
          //  - replacing any child of tree0 with tree1 woulde create a tree that exceedes the maxTeeSize
          // thus we just try until we find a valid configuration

          tree0Level = random.Next(root0Height - 1);
          tree1Level = random.Next(root1Height);
          tree0 = gardener.GetRandomBranch(root0, tree0Level);
          tree1 = gardener.GetRandomBranch(root1, tree1Level);

          // recalculate the size and height of tree1 (the one that we want to insert) because we need to check constraints later on
          tree1Size = tree1.Size;
          tree1Height = tree1.Height;
          // recalculate the list of possible indices 
          possibleChildIndices.Clear();
          for(int i = 0; i < tree0.SubTrees.Count; i++) {
            int subTreeSize = tree0.SubTrees[i].Size;

            // when the function is allowed as sub-tree and we don't violate the maxSize and maxHeight constraints
            // the index is ok
            if(gardener.GetAllowedSubFunctions(tree0.Function, i).Contains(tree1.Function) &&
              rootSize - subTreeSize + tree1Size < maxTreeSize &&
              tree0Level + tree1Height < maxTreeHeight) {
              possibleChildIndices.Add(i);
            }
          }
        }
        // replace the existing sub-tree at a random index in tree0 with tree1
        int selectedIndex = possibleChildIndices[random.Next(possibleChildIndices.Count)];
        tree0.RemoveSubTree(selectedIndex);
        tree0.InsertSubTree(selectedIndex, tree1);
        return root0;
      }
    }


    // take f and g and create a tree that has f and g as sub-trees 
    // example
    //       O
    //      /|\
    //     g 2 f 
    //
    private IFunctionTree CombineTerminals(TreeGardener gardener, IFunctionTree f, IFunctionTree g, MersenneTwister random, int maxTreeHeight, out List<IFunctionTree> newBranches) {
      newBranches = new List<IFunctionTree>();
      // determine the set of possible parent functions
      ICollection<IFunction> possibleParents = gardener.GetPossibleParents(new List<IFunction>() { f.Function, g.Function });
      if(possibleParents.Count == 0) throw new InvalidProgramException();
      // and select a random one
      IFunctionTree parent = possibleParents.ElementAt(random.Next(possibleParents.Count())).GetTreeNode();

      int minArity;
      int maxArity;
      gardener.GetMinMaxArity(parent.Function, out minArity, out maxArity);
      int nSlots = Math.Max(2, minArity);
      // determine which slot can take which sub-trees
      List<IFunctionTree>[] slots = new List<IFunctionTree>[nSlots];
      for(int slot = 0; slot < nSlots; slot++) {
        ICollection<IFunction> allowedSubFunctions = gardener.GetAllowedSubFunctions(parent.Function, slot);
        List<IFunctionTree> allowedTrees = new List<IFunctionTree>();
        if(allowedSubFunctions.Contains(f.Function)) allowedTrees.Add(f);
        if(allowedSubFunctions.Contains(g.Function)) allowedTrees.Add(g);
        slots[slot] = allowedTrees;
      }
      // fill the slots in the order of degrees of freedom
      int[] slotSequence = Enumerable.Range(0, slots.Count()).OrderBy(slot => slots[slot].Count()).ToArray();

      // tmp arry to store the tree for each sub-tree slot of the parent
      IFunctionTree[] selectedFunctionTrees = new IFunctionTree[nSlots];

      // fill the sub-tree slots of the parent starting with the slots that can take potentially both functions (f and g)
      for(int i = 0; i < slotSequence.Length; i++) {
        int slot = slotSequence[i];
        List<IFunctionTree> allowedTrees = slots[slot];
        // when neither f nor g fit into the slot => create a new random tree
        if(allowedTrees.Count() == 0) {
          var allowedFunctions = gardener.GetAllowedSubFunctions(parent.Function, slot);
          selectedFunctionTrees[slot] = gardener.CreateRandomTree(allowedFunctions, 1, 1, true);
          newBranches.AddRange(gardener.GetAllSubTrees(selectedFunctionTrees[slot]));
        } else {
          // select randomly which tree to insert into this slot
          IFunctionTree selectedTree = allowedTrees[random.Next(allowedTrees.Count())];
          selectedFunctionTrees[slot] = selectedTree;
          // remove the tree that we used in this slot from following function-sets
          for(int j = i + 1; j < slotSequence.Length; j++) {
            int otherSlot = slotSequence[j];
            slots[otherSlot].Remove(selectedTree);
          }
        }
      }
      // actually append the sub-trees to the parent tree
      for(int i = 0; i < selectedFunctionTrees.Length; i++) {
        parent.InsertSubTree(i, selectedFunctionTrees[i]);
      }

      return parent;
    }
  }
}
