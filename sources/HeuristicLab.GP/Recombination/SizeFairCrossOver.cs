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
using System.Diagnostics;

namespace HeuristicLab.GP {
  /// <summary>
  /// Implementation of a size fair crossover operator as described in: 
  /// William B. Langdon 
  /// Size Fair and Homologous Tree Genetic Programming Crossovers, 
  /// Genetic Programming and Evolvable Machines, Vol. 1, Number 1/2, pp. 95-119, April 2000
  /// </summary>
  public class SizeFairCrossOver : OperatorBase {
    private const int MAX_RECOMBINATION_TRIES = 20;
    public override string Description {
      get {
        return @"";
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
      Debug.Assert(gardener.IsValidTree(newTree) && newTreeHeight <= maxTreeHeight && newTreeSize <= maxTreeSize);
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

        // select a random suboperator of the 'receiving' tree
        IFunctionTree crossoverPoint = gardener.GetRandomParentNode(tree0);
        int removedBranchIndex;
        IFunctionTree removedBranch;
        IList<IFunction> allowedFunctions;
        if(crossoverPoint == null) {
          removedBranchIndex = 0;
          removedBranch = tree0;
          allowedFunctions = gardener.GetAllowedSubFunctions(null, 0);
        } else {
          removedBranchIndex = random.Next(crossoverPoint.SubTrees.Count);
          removedBranch = crossoverPoint.SubTrees[removedBranchIndex];
          allowedFunctions = gardener.GetAllowedSubFunctions(crossoverPoint.Function, removedBranchIndex);
        }
        int removedBranchSize = removedBranch.Size;
        int maxBranchSize = maxTreeSize - (tree0.Size - removedBranchSize);
        int maxBranchHeight = maxTreeHeight - gardener.GetBranchLevel(tree0, removedBranch);
        IFunctionTree insertedBranch = GetReplacementBranch(random, gardener, allowedFunctions, tree1, removedBranchSize, maxBranchSize, maxBranchHeight);

        int tries = 0;
        while(insertedBranch == null) {
          if(tries++ > MAX_RECOMBINATION_TRIES) {
            if(random.Next() > 0.5) return tree1;
            else return tree0;
          }

          // retry with a different crossoverPoint        
          crossoverPoint = gardener.GetRandomParentNode(tree0);
          if(crossoverPoint == null) {
            removedBranchIndex = 0;
            removedBranch = tree0;
            allowedFunctions = gardener.GetAllowedSubFunctions(null, 0);
          } else {
            removedBranchIndex = random.Next(crossoverPoint.SubTrees.Count);
            removedBranch = crossoverPoint.SubTrees[removedBranchIndex];
            allowedFunctions = gardener.GetAllowedSubFunctions(crossoverPoint.Function, removedBranchIndex);
          }
          removedBranchSize = removedBranch.Size;
          maxBranchSize = maxTreeSize - (tree0.Size - removedBranchSize);
          maxBranchHeight = maxTreeHeight - gardener.GetBranchLevel(tree0, removedBranch) + 1;
          insertedBranch = GetReplacementBranch(random, gardener, allowedFunctions, tree1, removedBranchSize, maxBranchSize, maxBranchHeight);
        }
        if(crossoverPoint != null) {
          // replace the branch below the crossoverpoint with the selected branch from root1
          crossoverPoint.RemoveSubTree(removedBranchIndex);
          crossoverPoint.InsertSubTree(removedBranchIndex, insertedBranch);
          return tree0;
        } else {
          return insertedBranch;
        }
      }
    }

    private IFunctionTree GetReplacementBranch(IRandom random, TreeGardener gardener, IList<IFunction> allowedFunctions, IFunctionTree tree, int removedBranchSize, int maxBranchSize, int maxBranchHeight) {
      var branches = gardener.GetAllSubTrees(tree).Where(t => allowedFunctions.Contains(t.Function) && t.Size < maxBranchSize && t.Height < maxBranchHeight)
        .Select(t => new { Tree = t, Size = t.Size }).Where(s => s.Size < 2 * removedBranchSize + 1);

      var shorterBranches = branches.Where(t => t.Size < removedBranchSize);
      var longerBranches = branches.Where(t => t.Size > removedBranchSize);
      var equalLengthBranches = branches.Where(t => t.Size == removedBranchSize);

      if(shorterBranches.Count() == 0 || longerBranches.Count() == 0) {
        if(equalLengthBranches.Count() == 0) {
          return null;
        } else {
          return equalLengthBranches.ElementAt(random.Next(equalLengthBranches.Count())).Tree;
        }
      } else {
        // invariant: |shorterBranches| > 0  and |longerBranches| > 0
        double pEqualLength = equalLengthBranches.Count() > 0 ? 1.0 / removedBranchSize : 0.0;
        double pLonger = (1.0 - pEqualLength) / (longerBranches.Count() * (1.0 + longerBranches.Average(t => t.Size) / shorterBranches.Average(t => t.Size)));
        double pShorter = (1.0 - pEqualLength - pLonger);

        double r = random.NextDouble();
        if(r < pLonger) {
          return longerBranches.ElementAt(random.Next(longerBranches.Count())).Tree;
        } else if(r < pLonger + pShorter) {
          return shorterBranches.ElementAt(random.Next(shorterBranches.Count())).Tree;
        } else {
          return equalLengthBranches.ElementAt(random.Next(equalLengthBranches.Count())).Tree;
        }
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

      int nSlots = Math.Max(2, parent.Function.MinArity);
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
          selectedFunctionTrees[slot] = gardener.CreateRandomTree(allowedFunctions, 1, 1);
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
