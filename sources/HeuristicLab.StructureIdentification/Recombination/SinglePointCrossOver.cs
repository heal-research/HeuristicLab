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
  public class SinglePointCrossOver : OperatorBase {

    public override string Description {
      get {
        return "TASK";
      }
    }
    public SinglePointCrossOver()
      : base() {
      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(MersenneTwister), VariableKind.In));
      AddVariableInfo(new VariableInfo("OperatorLibrary", "The operator library containing all available operators", typeof(GPOperatorLibrary), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaxTreeHeight", "The maximal allowed height of the tree", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaxTreeSize", "The maximal allowed size (number of nodes) of the tree", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("BalanceTrees", "Determines if the trees should be balanced", typeof(BoolData), VariableKind.In));
      AddVariableInfo(new VariableInfo("OperatorTree", "The tree to mutate", typeof(IOperator), VariableKind.In));
      AddVariableInfo(new VariableInfo("TreeSize", "The size (number of nodes) of the tree", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("TreeHeight", "The height of the tree", typeof(IntData), VariableKind.In));
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
      List<IOperator> newOperators;
      IOperator newTree = Cross(gardener, parent1, parent2,
        random, maxTreeSize, maxTreeHeight, out newOperators);

      if(!gardener.IsValidTree(newTree)) {
        throw new InvalidProgramException();
      }

      int newTreeSize = gardener.GetTreeSize(newTree);
      int newTreeHeight = gardener.GetTreeHeight(newTree);
      child.AddVariable(new Variable("OperatorTree", newTree));
      child.AddVariable(new Variable("TreeSize", new IntData(newTreeSize)));
      child.AddVariable(new Variable("TreeHeight", new IntData(newTreeHeight)));

      // check if the size of the new tree is still in the allowed bounds
      if (newTreeHeight > maxTreeHeight ||
        newTreeSize > maxTreeSize) {
        throw new InvalidProgramException();
      }


      return gardener.CreateInitializationOperation(newOperators, child);
    }


    private IOperator Cross(TreeGardener gardener, IScope f, IScope g, MersenneTwister random, int maxTreeSize, int maxTreeHeight, out List<IOperator> newOperators) {
      IOperator tree0 = f.GetVariableValue<IOperator>("OperatorTree", false);
      int tree0Height = f.GetVariableValue<IntData>("TreeHeight", false).Data;
      int tree0Size = f.GetVariableValue<IntData>("TreeSize", false).Data;

      IOperator tree1 = g.GetVariableValue<IOperator>("OperatorTree", false);
      int tree1Height = g.GetVariableValue<IntData>("TreeHeight", false).Data;
      int tree1Size = g.GetVariableValue<IntData>("TreeSize", false).Data;

      if(tree0Size == 1 && tree1Size == 1) {
        return CombineTerminals(gardener, tree0, tree1, random, maxTreeHeight, out newOperators);
      } else {
        // we are going to insert tree1 into tree0 at a random place so we have to make sure that tree0 is not a terminal
        // in case both trees are higher than 1 we swap the trees with probability 50%
        if(tree0Height == 1 || (tree1Height > 1 && random.Next(2) == 0)) {
          IOperator tmp = tree0; tree0 = tree1; tree1 = tmp;
          int tmpHeight = tree0Height; tree0Height = tree1Height; tree1Height = tmpHeight;
          int tmpSize = tree0Size; tree0Size = tree1Size; tree1Size = tmpSize;
        }

        // save the root because later on we change tree0 and tree1 while searching a valid tree configuration
        IOperator root = tree0;
        int rootSize = tree0Size;

        // select a random suboperators of the two trees at a random level
        int tree0Level = random.Next(tree0Height - 1); // since we checked before that the height of tree0 is > 1 this is OK
        int tree1Level = random.Next(tree1Height);
        tree0 = gardener.GetRandomNode(tree0, tree0Level);
        tree1 = gardener.GetRandomNode(tree1, tree1Level);

        // recalculate the size and height of tree1 (the one that we want to insert) because we need to check constraints later on
        tree1Size = gardener.GetTreeSize(tree1);
        tree1Height = gardener.GetTreeHeight(tree1);

        List<int> possibleChildIndices = new List<int>();
        TreeGardener.OperatorEqualityComparer comparer = new TreeGardener.OperatorEqualityComparer();

        // Now tree0 is supposed to take tree1 as one if its children. If this is not possible,
        // then go down in either of the two trees as far as possible. If even then it is not possible
        // to merge the trees then throw an exception
        // find the list of allowed indices (regarding allowed sub-operators, maxTreeSize and maxTreeHeight)
        for(int i = 0; i < tree0.SubOperators.Count; i++) {
          int subOperatorSize = gardener.GetTreeSize(tree0.SubOperators[i]);

          // the index is ok when the operator is allowed as sub-operator and we don't violate the maxSize and maxHeight constraints
          if(GetAllowedOperators(tree0, i).Contains(tree1, comparer) &&
            rootSize - subOperatorSize + tree1Size < maxTreeSize &&
            tree0Level + tree1Height < maxTreeHeight) {
            possibleChildIndices.Add(i);
          }
        }

        while(possibleChildIndices.Count == 0) {
          // ok we couln't find a possible configuration given the current tree0 and tree1
          // possible reasons for this are: 
          //  - tree1 is not allowed as sub-operator of tree0
          //  - appending tree1 as child of tree0 would create a tree that exceedes the maxTreeHeight
          //  - replacing any child of tree0 with tree1 woulde create a tree that exceedes the maxTeeSize
          // thus we have to either:
          //  - go up in tree0 => the insert position allows larger trees
          //  - go down in tree1 => the tree that is inserted becomes smaller
          //  - however we have to get lucky to solve the 'allowed sub-operators' problem
          if(tree1Height == 1 || (tree0Level>0 && random.Next(2) == 0)) {
            // go up in tree0 
            tree0Level--;
            tree0 = gardener.GetRandomNode(root, tree0Level);
          } else if(tree1.SubOperators.Count > 0) {
            // go down in node2:
            tree1 = tree1.SubOperators[random.Next(tree1.SubOperators.Count)];
            tree1Size = gardener.GetTreeSize(tree1);
            tree1Height = gardener.GetTreeHeight(tree1);
          } else {
            // could neither go up or down ... don't know what to do ... give up
            throw new InvalidProgramException();
          }

          // recalculate the list of possible indices 
          possibleChildIndices.Clear();
          for(int i = 0; i < tree0.SubOperators.Count; i++) {
            int subOperatorSize = gardener.GetTreeSize(tree0.SubOperators[i]);

            // when the operator is allowed as sub-operator and we don't violate the maxSize and maxHeight constraints
            // the index is ok
            if(GetAllowedOperators(tree0, i).Contains(tree1, comparer) &&
              rootSize - subOperatorSize + tree1Size < maxTreeSize &&
              tree0Level + tree1Height < maxTreeHeight) {
              possibleChildIndices.Add(i);
            }
          }
        }

        // no possible configuration found this indicates that there is a bigger problem
        if(possibleChildIndices.Count == 0) {
          throw new InvalidProgramException();
        }

        // replace the existing sub-tree at a random index in tree0 with tree1
        int selectedIndex = possibleChildIndices[random.Next(possibleChildIndices.Count)];
        tree0.RemoveSubOperator(selectedIndex);
        tree0.AddSubOperator(tree1, selectedIndex);

        // no new operators where needed
        newOperators = new List<IOperator>();
        return root;
      }
    }

    private ICollection<IOperator> GetAllowedOperators(IOperator tree0, int i) {
      ItemList slotList = (ItemList)tree0.GetVariable(GPOperatorLibrary.ALLOWED_SUBOPERATORS).Value;
      return ((ItemList)slotList[i]).OfType<IOperator>().ToArray();
    }

    private IOperator CombineTerminals(TreeGardener gardener, IOperator f, IOperator g, MersenneTwister random, int maxTreeHeight, out List<IOperator> newOperators) {
      newOperators = new List<IOperator>();
      ICollection<IOperator> possibleParents = gardener.GetPossibleParents(new List<IOperator>() { f, g });
      if(possibleParents.Count == 0) throw new InvalidProgramException();

      IOperator parent = (IOperator)possibleParents.ElementAt(random.Next(possibleParents.Count())).Clone();

      int minArity;
      int maxArity;
      gardener.GetMinMaxArity(parent, out minArity, out maxArity);

      int nSlots = Math.Max(2, minArity);

      HashSet<IOperator>[] slotSets = new HashSet<IOperator>[nSlots];

      SubOperatorsConstraintAnalyser analyser = new SubOperatorsConstraintAnalyser();
      analyser.AllPossibleOperators = new List<IOperator>() { g, f };
      for(int slot = 0; slot < nSlots; slot++) {
        HashSet<IOperator> slotSet = new HashSet<IOperator>(analyser.GetAllowedOperators(parent, slot));
        slotSets[slot] = slotSet;
      }

      int[] slotSequence = Enumerable.Range(0, slotSets.Count()).OrderBy(slot => slotSets[slot].Count()).ToArray();

      IOperator[] selectedOperators = new IOperator[nSlots];
      for(int i = 0; i < slotSequence.Length; i++) {
        int slot = slotSequence[i];
        HashSet<IOperator> slotSet = slotSets[slot];
        if(slotSet.Count() == 0) {
          var allowedOperators = GetAllowedOperators(parent, slot);
          selectedOperators[slot] = gardener.CreateRandomTree(allowedOperators, 1, 1, true);
          newOperators.AddRange(gardener.GetAllOperators(selectedOperators[slot]));
        } else {
          IOperator selectedOperator = slotSet.ElementAt(random.Next(slotSet.Count()));
          selectedOperators[slot] = selectedOperator;
          for(int j = i + 1; j < slotSequence.Length; j++) {
            int otherSlot = slotSequence[j];
            slotSets[otherSlot].Remove(selectedOperator);
          }
        }
      }

      for(int i = 0; i < selectedOperators.Length; i++) {
        parent.AddSubOperator(selectedOperators[i], i);
      }

      return parent;
    }
  }
}
