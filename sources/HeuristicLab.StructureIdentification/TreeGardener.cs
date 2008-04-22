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
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Constraints;
using System.Diagnostics;
using HeuristicLab.Data;
using System.Linq;
using HeuristicLab.Random;
using HeuristicLab.Operators;
using HeuristicLab.Selection;
using HeuristicLab.Functions;
using System.Collections;

namespace HeuristicLab.StructureIdentification {
  internal class TreeGardener {
    private IRandom random;
    private GPOperatorLibrary funLibrary;
    private List<IFunction> functions;
    private List<IFunction> terminals;

    internal IList<IFunction> Terminals {
      get { return terminals.AsReadOnly(); }
    }
    private List<IFunction> allFunctions;

    internal IList<IFunction> AllFunctions {
      get { return allFunctions.AsReadOnly(); }
    }

    internal TreeGardener(IRandom random, GPOperatorLibrary funLibrary) {
      this.random = random;
      this.funLibrary = funLibrary;

      this.allFunctions = new List<IFunction>();
      terminals = new List<IFunction>();
      functions = new List<IFunction>();

      // init functions and terminals based on constraints
      foreach (IFunction fun in funLibrary.Group.Operators) {
        int maxA, minA;
        GetMinMaxArity(fun, out minA, out maxA);
        if (maxA == 0) {
          terminals.Add(fun);
        } else {
          functions.Add(fun);
        }
      }

      allFunctions.AddRange(functions);
      allFunctions.AddRange(terminals);
    }

    #region random initialization
    internal IFunctionTree CreateRandomTree(ICollection<IFunction> allowedFunctions, int maxTreeSize, int maxTreeHeight) {
      // default is non-balanced trees
      return CreateRandomTree(allowedFunctions, maxTreeSize, maxTreeHeight, false); 
    }
    internal IFunctionTree CreateRandomTree(ICollection<IFunction> allowedFunctions, int maxTreeSize, int maxTreeHeight, bool balanceTrees) {

      int minTreeHeight = allowedFunctions.Select(f => ((IntData)f.GetVariable(GPOperatorLibrary.MIN_TREE_HEIGHT).Value).Data).Min();
      if (minTreeHeight > maxTreeHeight)
        maxTreeHeight = minTreeHeight;

      int minTreeSize = allowedFunctions.Select(f => ((IntData)f.GetVariable(GPOperatorLibrary.MIN_TREE_SIZE).Value).Data).Min();
      if (minTreeSize > maxTreeSize)
        maxTreeSize = minTreeSize;

      int treeHeight = random.Next(minTreeHeight, maxTreeHeight + 1);
      int treeSize = random.Next(minTreeSize, maxTreeSize + 1);

      IFunction[] possibleFunctions = allowedFunctions.Where(f => ((IntData)f.GetVariable(GPOperatorLibrary.MIN_TREE_HEIGHT).Value).Data <= treeHeight &&
        ((IntData)f.GetVariable(GPOperatorLibrary.MIN_TREE_SIZE).Value).Data <= treeSize).ToArray();
      IFunction selectedFunction = possibleFunctions[random.Next(possibleFunctions.Length)];

      return CreateRandomTree(selectedFunction, treeSize, treeHeight, balanceTrees);
    }

    internal IFunctionTree CreateRandomTree(int maxTreeSize, int maxTreeHeight, bool balanceTrees) {
      if (balanceTrees) {
        if (maxTreeHeight == 1 || maxTreeSize==1) {
          IFunction selectedTerminal = terminals[random.Next(terminals.Count())];
          return new FunctionTree(selectedTerminal);
        } else {
          IFunction[] possibleFunctions = functions.Where(f => GetMinimalTreeHeight(f) <= maxTreeHeight &&
            GetMinimalTreeSize(f) <= maxTreeSize).ToArray();
          IFunction selectedFunction = possibleFunctions[random.Next(possibleFunctions.Length)];
          FunctionTree root = new FunctionTree(selectedFunction);
          MakeBalancedTree(root, maxTreeSize - 1, maxTreeHeight - 1);
          return root;
        }

      } else {
        IFunction[] possibleFunctions = allFunctions.Where(f => GetMinimalTreeHeight(f) <= maxTreeHeight &&
          GetMinimalTreeSize(f) <= maxTreeSize).ToArray();
        IFunction selectedFunction = possibleFunctions[random.Next(possibleFunctions.Length)];
        FunctionTree root = new FunctionTree(selectedFunction);
        MakeUnbalancedTree(root, maxTreeSize - 1, maxTreeHeight - 1);
        return root;
      }
    }

    internal IFunctionTree CreateRandomTree(IFunction rootFunction, int maxTreeSize, int maxTreeHeight, bool balanceTrees) {
      IFunctionTree root = new FunctionTree(rootFunction);
      if (balanceTrees) {
        MakeBalancedTree(root, maxTreeSize - 1, maxTreeHeight - 1);
      } else {
        MakeUnbalancedTree(root, maxTreeSize - 1, maxTreeHeight - 1);
      }
      if (GetTreeSize(root) > maxTreeSize ||
         GetTreeHeight(root) > maxTreeHeight) {
        throw new InvalidProgramException();
      }
      return root;
    }


    private void MakeUnbalancedTree(IFunctionTree parent, int maxTreeSize, int maxTreeHeight) {
      if (maxTreeHeight == 0 || maxTreeSize == 0) return;
      int minArity;
      int maxArity;
      GetMinMaxArity(parent.Function, out minArity, out maxArity);
      if (maxArity >= maxTreeSize) {
        maxArity = maxTreeSize;
      }
      int actualArity = random.Next(minArity, maxArity + 1);
      if (actualArity > 0) {
        int maxSubTreeSize = maxTreeSize / actualArity;
        for (int i = 0; i < actualArity; i++) {
          IFunction[] possibleFunctions = GetAllowedSubFunctions(parent.Function, i).Where(f => GetMinimalTreeHeight(f) <= maxTreeHeight &&
            GetMinimalTreeSize(f) <= maxSubTreeSize).ToArray();
          IFunction selectedFunction = possibleFunctions[random.Next(possibleFunctions.Length)];
          FunctionTree newSubTree = new FunctionTree(selectedFunction);
          MakeUnbalancedTree(newSubTree, maxSubTreeSize - 1, maxTreeHeight - 1);
          parent.InsertSubTree(i, newSubTree);
        }
      }
    }

    // NOTE: this method doesn't build fully balanced trees because we have constraints on the
    // types of possible sub-functions which can indirectly impose a limit for the depth of a given sub-tree
    private void MakeBalancedTree(IFunctionTree parent, int maxTreeSize, int maxTreeHeight) {
      if (maxTreeHeight == 0 || maxTreeSize == 0) return; // should never happen anyway
      int minArity;
      int maxArity;
      GetMinMaxArity(parent.Function, out minArity, out maxArity);
      if (maxArity >= maxTreeSize) {
        maxArity = maxTreeSize;
      }
      int actualArity = random.Next(minArity, maxArity + 1);
      if (actualArity > 0) {
        int maxSubTreeSize = maxTreeSize / actualArity;
        for (int i = 0; i < actualArity; i++) {
          if (maxTreeHeight == 1 || maxSubTreeSize == 1) {
            IFunction[] possibleTerminals = GetAllowedSubFunctions(parent.Function, i).Where(
              f => GetMinimalTreeHeight(f) <= maxTreeHeight &&
              GetMinimalTreeSize(f) <= maxSubTreeSize &&
              IsTerminal(f)).ToArray();
            IFunction selectedTerminal = possibleTerminals[random.Next(possibleTerminals.Length)];
            IFunctionTree newTree = new FunctionTree(selectedTerminal);
            parent.InsertSubTree(i, newTree);
          } else {
            IFunction[] possibleFunctions = GetAllowedSubFunctions(parent.Function, i).Where(
              f => GetMinimalTreeHeight(f) <= maxTreeHeight &&
              GetMinimalTreeSize(f) <= maxSubTreeSize &&
              !IsTerminal(f)).ToArray();
            IFunction selectedFunction = possibleFunctions[random.Next(possibleFunctions.Length)];
            FunctionTree newTree = new FunctionTree(selectedFunction);
            parent.InsertSubTree(i, newTree);
            MakeBalancedTree(newTree, maxSubTreeSize - 1, maxTreeHeight - 1);
          }
        }
      }
    }

    internal CompositeOperation CreateInitializationOperation(ICollection<IFunctionTree> trees, IScope scope) {
      // needed for the parameter shaking operation
      CompositeOperation initializationOperation = new CompositeOperation();
      Scope tempScope = new Scope("Temp. initialization scope");

      var parametricTrees = trees.Where(t => t.Function.GetVariable(GPOperatorLibrary.INITIALIZATION) != null);

      foreach (IFunctionTree tree in parametricTrees) {
        // enqueue an initialization operation for each operator with local variables 
        IOperator initialization = (IOperator)tree.Function.GetVariable(GPOperatorLibrary.INITIALIZATION).Value;
        Scope initScope = new Scope();

        // copy the local variables into a temporary scope used for initialization
        foreach (IVariable variable in tree.LocalVariables) {
          initScope.AddVariable(variable);
        }

        tempScope.AddSubScope(initScope);
        initializationOperation.AddOperation(new AtomicOperation(initialization, initScope));
      }

      Scope backupScope = new Scope("backup");
      foreach (Scope subScope in scope.SubScopes) {
        backupScope.AddSubScope(subScope);
      }

      scope.AddSubScope(tempScope);
      scope.AddSubScope(backupScope);

      // add an operation to remove the temporary scopes        
      initializationOperation.AddOperation(new AtomicOperation(new RightReducer(), scope));
      return initializationOperation;
    }
    #endregion

    #region tree information gathering
    internal int GetTreeSize(IFunctionTree tree) {
      return 1 + tree.SubTrees.Sum(f => GetTreeSize(f));
    }

    internal int GetTreeHeight(IFunctionTree tree) {
      if (tree.SubTrees.Count == 0) return 1;
      return 1 + tree.SubTrees.Max(f => GetTreeHeight(f));
    }

    internal IFunctionTree GetRandomParentNode(IFunctionTree tree) {
      List<IFunctionTree> parentNodes = new List<IFunctionTree>();

      // add null for the parent of the root node
      parentNodes.Add(null);

      TreeForEach(tree, delegate(IFunctionTree possibleParentNode) {
        if (possibleParentNode.SubTrees.Count > 0) {
          parentNodes.Add(possibleParentNode);
        }
      });

      return parentNodes[random.Next(parentNodes.Count)];
    }

    internal IList<IFunction> GetAllowedSubFunctions(IFunction f, int index) {
      if (f == null) {
        return allFunctions;
      } else {
        ItemList slotList = (ItemList)f.GetVariable(GPOperatorLibrary.ALLOWED_SUBOPERATORS).Value;
        List<IFunction> result = new List<IFunction>();
        foreach(IFunction function in (ItemList)slotList[index]) {
          result.Add(function);
        }
        return result;
      }
    }
    internal void GetMinMaxArity(IFunction f, out int minArity, out int maxArity) {
      foreach (IConstraint constraint in f.Constraints) {
        NumberOfSubOperatorsConstraint theConstraint = constraint as NumberOfSubOperatorsConstraint;
        if (theConstraint != null) {
          minArity = theConstraint.MinOperators.Data;
          maxArity = theConstraint.MaxOperators.Data;
          return;
        }
      }
      // the default arity is 2
      minArity = 2;
      maxArity = 2;
    }
    internal bool IsTerminal(IFunction f) {
      int minArity;
      int maxArity;
      GetMinMaxArity(f, out minArity, out maxArity);
      return minArity == 0 && maxArity == 0;
    }

    internal IList<IFunction> GetAllowedParents(IFunction child, int childIndex) {
      List<IFunction> parents = new List<IFunction>();
      foreach (IFunction function in functions) {
        ICollection<IFunction> allowedSubFunctions = GetAllowedSubFunctions(function, childIndex);
        if (allowedSubFunctions.Contains(child)) {
          parents.Add(function);
        }
      }
      return parents;
    }

    internal ICollection<IFunctionTree> GetAllSubTrees(IFunctionTree root) {
      List<IFunctionTree> allTrees = new List<IFunctionTree>();
      TreeForEach(root, t => { allTrees.Add(t); });
      return allTrees;
    }

    /// <summary>
    /// returns the height level of branch in the tree
    /// if the branch == tree => 1
    /// if branch is in the sub-trees of tree => 2
    /// ...
    /// if branch is not found => -1
    /// </summary>
    /// <param name="tree">root of the function tree to process</param>
    /// <param name="branch">branch that is searched in the tree</param>
    /// <returns></returns>
    internal int GetBranchLevel(IFunctionTree tree, IFunctionTree branch) {
      return GetBranchLevelHelper(tree, branch, 1);
    }

    // 'tail-recursive' helper
    private int GetBranchLevelHelper(IFunctionTree tree, IFunctionTree branch, int level) {
      if (branch == tree) return level;

      foreach (IFunctionTree subTree in tree.SubTrees) {
        int result = GetBranchLevelHelper(subTree, branch, level + 1);
        if (result != -1) return result;
      }

      return -1;
    }

    internal bool IsValidTree(IFunctionTree tree) {
      foreach(IConstraint constraint in tree.Function.Constraints) {
        if(constraint is NumberOfSubOperatorsConstraint) {
          int max = ((NumberOfSubOperatorsConstraint)constraint).MaxOperators.Data;
          int min = ((NumberOfSubOperatorsConstraint)constraint).MinOperators.Data;
          if(tree.SubTrees.Count < min || tree.SubTrees.Count > max) 
            return false;
        }
      }
      foreach(IFunctionTree subTree in tree.SubTrees) {
        if(!IsValidTree(subTree)) return false;
      }
      return true;
    }

    // returns a random branch from the specified level in the tree
    internal IFunctionTree GetRandomBranch(IFunctionTree tree, int level) {
      if (level == 0) return tree;
      List<IFunctionTree> branches = GetBranchesAtLevel(tree, level);
      return branches[random.Next(branches.Count)];
    }
    #endregion

    #region private utility methods

    private int GetMinimalTreeHeight(IOperator op) {
      return ((IntData)op.GetVariable(GPOperatorLibrary.MIN_TREE_HEIGHT).Value).Data;
    }

    private int GetMinimalTreeSize(IOperator op) {
      return ((IntData)op.GetVariable(GPOperatorLibrary.MIN_TREE_SIZE).Value).Data;
    }

    private void TreeForEach(IFunctionTree tree, Action<IFunctionTree> action) {
      action(tree);
      foreach (IFunctionTree subTree in tree.SubTrees) {
        TreeForEach(subTree, action);
      }
    }

    private List<IFunctionTree> GetBranchesAtLevel(IFunctionTree tree, int level) {
      if (level == 1) return new List<IFunctionTree>(tree.SubTrees);

      List<IFunctionTree> branches = new List<IFunctionTree>();
      foreach (IFunctionTree subTree in tree.SubTrees) {
        branches.AddRange(GetBranchesAtLevel(subTree, level - 1));
      }
      return branches;
    }


    #endregion

    internal ICollection<IFunction> GetPossibleParents(List<IFunction> list) {
      List<IFunction> result = new List<IFunction>();
      foreach (IFunction f in functions) {
        if (IsPossibleParent(f, list)) {
          result.Add(f);
        }
      }
      return result;
    }

    private bool IsPossibleParent(IFunction f, List<IFunction> children) {
      int minArity;
      int maxArity;
      GetMinMaxArity(f, out minArity, out maxArity);

      // note: we can't assume that the operators in the children list have different types!

      // when the maxArity of this function is smaller than the list of operators that
      // should be included as sub-operators then it can't be a parent
      if (maxArity < children.Count()) {
        return false;
      }
      int nSlots = Math.Max(minArity, children.Count);

      SubOperatorsConstraintAnalyser analyzer = new SubOperatorsConstraintAnalyser();
      analyzer.AllPossibleOperators = children.Cast<IOperator>().ToArray<IOperator>();

      List<HashSet<IFunction>> slotSets = new List<HashSet<IFunction>>();

      // we iterate through all slots for sub-trees and calculate the set of 
      // allowed functions for this slot.
      // we only count those slots that can hold at least one of the children that we should combine
      for (int slot = 0; slot < nSlots; slot++) {
        HashSet<IFunction> functionSet = new HashSet<IFunction>(analyzer.GetAllowedOperators(f, slot).Cast<IFunction>());
        if (functionSet.Count() > 0) {
          slotSets.Add(functionSet);
        }
      }

      // ok at the end of this operation we know how many slots of the parent can actually
      // hold one of our children.
      // if the number of slots is smaller than the number of children we can be sure that
      // we can never combine all children as sub-trees of the function and thus the function
      // can't be a parent.
      if (slotSets.Count() < children.Count()) {
        return false;
      }

      // finally we sort the sets by size and beginning from the first set select one
      // function for the slot and thus remove it as possible sub-tree from the remaining sets.
      // when we can successfully assign all available children to a slot the function is a valid parent
      // when only a subset of all children can be assigned to slots the function is no valid parent
      slotSets.Sort((p, q) => p.Count() - q.Count());

      int assignments = 0;
      for (int i = 0; i < slotSets.Count() - 1; i++) {
        if (slotSets[i].Count > 0) {
          IFunction selected = slotSets[i].ElementAt(0);
          assignments++;
          for (int j = i + 1; j < slotSets.Count(); j++) {
            slotSets[j].Remove(selected);
          }
        }
      }

      // sanity check 
      if (assignments > children.Count) throw new InvalidProgramException();
      return assignments == children.Count - 1;
    }
  }
}
