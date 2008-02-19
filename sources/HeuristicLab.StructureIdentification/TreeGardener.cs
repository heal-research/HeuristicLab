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

namespace HeuristicLab.StructureIdentification {
  internal class TreeGardener {
    private IRandom random;
    private IOperatorLibrary opLibrary;
    private List<IOperator> functions;
    private List<IOperator> terminals;

    internal IList<IOperator> Terminals {
      get { return terminals.AsReadOnly(); }
    }
    private List<IOperator> allOperators;

    internal IList<IOperator> AllOperators {
      get { return allOperators.AsReadOnly(); }
    }

    internal TreeGardener(IRandom random, IOperatorLibrary opLibrary) {
      this.random = random;
      this.opLibrary = opLibrary;

      this.allOperators = new List<IOperator>();
      terminals = new List<IOperator>();
      functions = new List<IOperator>();

      // init functions and terminals based on constraints
      foreach (IOperator op in opLibrary.Group.Operators) {
        int maxA, minA;
        GetMinMaxArity(op, out minA, out maxA);
        if (maxA == 0) {
          terminals.Add(op);
        } else {
          functions.Add(op);
        }
      }

      allOperators.AddRange(functions);
      allOperators.AddRange(terminals);
    }

    #region random initialization
    internal IOperator CreateRandomTree(ICollection<IOperator> allowedOperators, int maxTreeSize, int maxTreeHeight, bool balanceTrees) {

      int minTreeHeight = allowedOperators.Select(op => ((IntData)op.GetVariable(GPOperatorLibrary.MIN_TREE_HEIGHT).Value).Data).Min();
      if (minTreeHeight > maxTreeHeight)
        maxTreeHeight = minTreeHeight;

      int minTreeSize = allowedOperators.Select(op => ((IntData)op.GetVariable(GPOperatorLibrary.MIN_TREE_SIZE).Value).Data).Min();
      if (minTreeSize > maxTreeSize)
        maxTreeSize = minTreeSize;

      int treeHeight = random.Next(minTreeHeight, maxTreeHeight + 1);
      int treeSize = random.Next(minTreeSize, maxTreeSize + 1);

      IOperator[] possibleOperators = allowedOperators.Where(op => ((IntData)op.GetVariable(GPOperatorLibrary.MIN_TREE_HEIGHT).Value).Data <= treeHeight &&
        ((IntData)op.GetVariable(GPOperatorLibrary.MIN_TREE_SIZE).Value).Data <= treeSize).ToArray();
      IOperator selectedOperator = (IOperator)possibleOperators[random.Next(possibleOperators.Length)].Clone();

      IOperator rootOperator = CreateRandomTree(selectedOperator, treeSize, treeHeight, balanceTrees);

      return rootOperator;
    }

    internal IOperator CreateRandomTree(int maxTreeSize, int maxTreeHeight, bool balanceTrees) {
      if (balanceTrees) {
        if (maxTreeHeight == 1) {
          IOperator selectedTerminal = (IOperator)terminals[random.Next(terminals.Count())].Clone();
          return selectedTerminal;
        } else {
          IOperator[] possibleFunctions = functions.Where(f => GetMinimalTreeHeight(f) <= maxTreeHeight &&
            GetMinimalTreeSize(f) <= maxTreeSize).ToArray();
          IOperator selectedFunction = (IOperator)possibleFunctions[random.Next(possibleFunctions.Length)].Clone();
          MakeBalancedTree(selectedFunction, maxTreeSize - 1, maxTreeHeight - 1);
          return selectedFunction;
        }

      } else {
        IOperator[] possibleOperators = allOperators.Where(op => GetMinimalTreeHeight(op) <= maxTreeHeight &&
          GetMinimalTreeSize(op) <= maxTreeSize).ToArray();
        IOperator selectedOperator = (IOperator)possibleOperators[random.Next(possibleOperators.Length)].Clone();
        MakeUnbalancedTree(selectedOperator, maxTreeSize - 1, maxTreeHeight - 1);
        return selectedOperator;
      }
    }

    internal IOperator CreateRandomTree(IOperator root, int maxTreeSize, int maxTreeHeight, bool balanceTrees) {
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


    private void MakeUnbalancedTree(IOperator parent, int maxTreeSize, int maxTreeHeight) {
      if (maxTreeHeight == 0 || maxTreeSize == 0) return;
      int minArity;
      int maxArity;
      GetMinMaxArity(parent, out minArity, out maxArity);
      if (maxArity >= maxTreeSize) {
        maxArity = maxTreeSize;
      }
      int actualArity = random.Next(minArity, maxArity + 1);
      if (actualArity > 0) {
        int maxSubTreeSize = maxTreeSize / actualArity;
        for (int i = 0; i < actualArity; i++) {
          IOperator[] possibleOperators = GetAllowedSubOperators(parent, i).Where(op => GetMinimalTreeHeight(op) <= maxTreeHeight &&
            GetMinimalTreeSize(op) <= maxSubTreeSize).ToArray();
          IOperator selectedOperator = (IOperator)possibleOperators[random.Next(possibleOperators.Length)].Clone();
          parent.AddSubOperator(selectedOperator, i);
          MakeUnbalancedTree(selectedOperator, maxSubTreeSize - 1, maxTreeHeight - 1);
        }
      }
    }

    // NOTE: this method doesn't build fully balanced trees because we have constraints on the
    // types of possible suboperators which can indirectly impose a limit for the depth of a given suboperator
    private void MakeBalancedTree(IOperator parent, int maxTreeSize, int maxTreeHeight) {
      if (maxTreeHeight == 0 || maxTreeSize == 0) return; // should never happen anyway
      int minArity;
      int maxArity;
      GetMinMaxArity(parent, out minArity, out maxArity);
      if (maxArity >= maxTreeSize) {
        maxArity = maxTreeSize;
      }
      int actualArity = random.Next(minArity, maxArity + 1);
      if (actualArity > 0) {
        int maxSubTreeSize = maxTreeSize / actualArity;
        for (int i = 0; i < actualArity; i++) {
          if (maxTreeHeight == 1 || maxSubTreeSize == 1) {
            IOperator[] possibleTerminals = GetAllowedSubOperators(parent, i).Where(
              op => GetMinimalTreeHeight(op) <= maxTreeHeight &&
              GetMinimalTreeSize(op) <= maxSubTreeSize &&
              IsTerminal(op)).ToArray();
            IOperator selectedTerminal = (IOperator)possibleTerminals[random.Next(possibleTerminals.Length)].Clone();
            parent.AddSubOperator(selectedTerminal, i);
          } else {
            IOperator[] possibleFunctions = GetAllowedSubOperators(parent, i).Where(
              op => GetMinimalTreeHeight(op) <= maxTreeHeight &&
              GetMinimalTreeSize(op) <= maxSubTreeSize &&
              !IsTerminal(op)).ToArray();
            IOperator selectedFunction = (IOperator)possibleFunctions[random.Next(possibleFunctions.Length)].Clone();
            parent.AddSubOperator(selectedFunction, i);
            MakeBalancedTree(selectedFunction, maxSubTreeSize - 1, maxTreeHeight - 1);
          }
        }
      }
    }

    internal CompositeOperation CreateInitializationOperation(ICollection<IOperator> operators, IScope scope) {
      // needed for the parameter shaking operation
      CompositeOperation initializationOperation = new CompositeOperation();
      Scope tempScope = new Scope("Temp. initialization scope");

      var parametricOperators = operators.Where(o => o.GetVariable(GPOperatorLibrary.INITIALIZATION) != null);

      foreach (IOperator op in parametricOperators) {
        // enqueue an initialization operation for each operator with local variables 
        IOperator initialization = (IOperator)op.GetVariable(GPOperatorLibrary.INITIALIZATION).Value;
        Scope initScope = new Scope();

        // copy the local variables into a temporary scope used for initialization
        foreach (VariableInfo info in op.VariableInfos) {
          if (info.Local) {
            initScope.AddVariable(op.GetVariable(info.FormalName));
          }
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
    internal int GetTreeSize(IOperator tree) {
      return 1 + tree.SubOperators.Sum(f => GetTreeSize(f));
    }

    internal int GetTreeHeight(IOperator tree) {
      if (tree.SubOperators.Count == 0) return 1;
      return 1 + tree.SubOperators.Max(f => GetTreeHeight(f));
    }

    internal IOperator GetRandomParentNode(IOperator tree) {
      List<IOperator> parentNodes = new List<IOperator>();

      // add null for the parent of the root node
      parentNodes.Add(null);

      TreeForEach(tree, delegate(IOperator op) {
        if (op.SubOperators.Count > 0) {
          parentNodes.Add(op);
        }
      });

      return parentNodes[random.Next(parentNodes.Count)];
    }

    internal IList<IOperator> GetAllowedSubOperators(IOperator op, int index) {
      if (op == null) {
        return allOperators;
      } else {

        SubOperatorsConstraintAnalyser analyser = new SubOperatorsConstraintAnalyser();
        analyser.AllPossibleOperators = allOperators;

        return analyser.GetAllowedOperators(op, index);
      }
    }
    internal void GetMinMaxArity(IOperator root, out int minArity, out int maxArity) {
      foreach (IConstraint constraint in root.Constraints) {
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
    internal bool IsTerminal(IOperator f) {
      int minArity;
      int maxArity;
      GetMinMaxArity(f, out minArity, out maxArity);
      return minArity == 0 && maxArity == 0;
    }

    internal IList<IOperator> GetAllowedParents(IOperator child, int childIndex) {
      List<IOperator> parents = new List<IOperator>();
      foreach (IOperator function in functions) {
        IList<IOperator> allowedSubOperators = GetAllowedSubOperators(function, childIndex);
        if (allowedSubOperators.Contains(child, new OperatorEqualityComparer())) {
          parents.Add(function);
        }
      }
      return parents;
    }

    internal ICollection<IOperator> GetAllOperators(IOperator root) {
      List<IOperator> allOps = new List<IOperator>();
      TreeForEach(root, t => { allOps.Add(t); });
      return allOps;
    }

    /// <summary>
    /// returns the height level of op in the tree
    /// if the op == tree => 1
    /// if op is in the suboperators of tree => 2
    /// ...
    /// if op is not found => -1
    /// </summary>
    /// <param name="tree">operator tree to process</param>
    /// <param name="op">operater that is searched in the tree</param>
    /// <returns></returns>
    internal int GetNodeLevel(IOperator tree, IOperator op) {
      return GetNodeLevelHelper(tree, op, 1);
    }

    private int GetNodeLevelHelper(IOperator tree, IOperator op, int level) {
      if (op == tree) return level;

      foreach (IOperator subTree in tree.SubOperators) {
        int result = GetNodeLevelHelper(subTree, op, level + 1);
        if (result != -1) return result;
      }

      return -1;
    }

    internal bool IsValidTree(IOperator tree) {
      if (!tree.IsValid())
        return false;
      foreach (IOperator subTree in tree.SubOperators) {
        if (!subTree.IsValid())
          return false;
      }

      return true;
    }

    // returns a random node from the specified level in the tree
    internal IOperator GetRandomNode(IOperator tree, int level) {
      if (level == 0) return tree;
      List<IOperator> nodes = GetOperatorsAtLevel(tree, level);
      return nodes[random.Next(nodes.Count)];
    }
    #endregion

    #region private utility methods

    private int GetMinimalTreeHeight(IOperator op) {
      return ((IntData)op.GetVariable(GPOperatorLibrary.MIN_TREE_HEIGHT).Value).Data;
    }

    private int GetMinimalTreeSize(IOperator op) {
      return ((IntData)op.GetVariable(GPOperatorLibrary.MIN_TREE_SIZE).Value).Data;
    }

    private void TreeForEach(IOperator tree, Action<IOperator> action) {
      action(tree);
      foreach (IOperator child in tree.SubOperators) {
        TreeForEach(child, action);
      }
    }

    private List<IOperator> GetOperatorsAtLevel(IOperator tree, int level) {
      if (level == 1) return new List<IOperator>(tree.SubOperators);

      List<IOperator> result = new List<IOperator>();
      foreach (IOperator subOperator in tree.SubOperators) {
        result.AddRange(GetOperatorsAtLevel(subOperator, level - 1));
      }
      return result;
    }


    #endregion

    internal class OperatorEqualityComparer : IEqualityComparer<IOperator> {
      #region IEqualityComparer<IOperator> Members

      public bool Equals(IOperator x, IOperator y) {
        return ((StringData)x.GetVariable(GPOperatorLibrary.TYPE_ID).Value).Data ==
          ((StringData)y.GetVariable(GPOperatorLibrary.TYPE_ID).Value).Data;
      }

      public int GetHashCode(IOperator obj) {
        return ((StringData)obj.GetVariable(GPOperatorLibrary.TYPE_ID).Value).Data.GetHashCode();
      }

      #endregion
    }

    internal ICollection<IOperator> GetPossibleParents(List<IOperator> list) {
      List<IOperator> result = new List<IOperator>();
      foreach (IOperator op in functions) {
        if (IsPossibleParent(op, list)) {
          result.Add(op);
        }
      }
      return result;
    }

    private bool IsPossibleParent(IOperator op, List<IOperator> children) {
      int minArity;
      int maxArity;
      GetMinMaxArity(op, out minArity, out maxArity);

      // note: we can't assume that the operators in the children list have different types!

      // when the maxArity of this function is smaller than the list of operators that
      // should be included as sub-operators then it can't be a parent
      if (maxArity < children.Count()) {
        return false;
      }
      int nSlots = Math.Max(minArity, children.Count);

      SubOperatorsConstraintAnalyser analyzer = new SubOperatorsConstraintAnalyser();
      analyzer.AllPossibleOperators = children;

      List<HashSet<IOperator>> slotSets = new List<HashSet<IOperator>>();

      // we iterate through all slots for sub-operators and calculate the set of 
      // allowed sub-operators for this slot.
      // we only count those slots that can hold at least one of the children that we should combine
      for (int slot = 0; slot < nSlots; slot++) {
        HashSet<IOperator> operatorSet = new HashSet<IOperator>(analyzer.GetAllowedOperators(op, slot));
        if (operatorSet.Count() > 0) {
          slotSets.Add(operatorSet);
        }
      }

      // ok at the end of this operation we know how many slots of the parent can actually
      // hold one of our children.
      // if the number of slots is smaller than the number of children we can be sure that
      // we can never combine all children as sub-operators of the operator and thus the operator
      // can't be a parent.
      if (slotSets.Count() < children.Count()) {
        return false;
      }

      // finally we sort the sets by size and beginning from the first set select one
      // operator for the slot and thus remove it as possible sub-operator from the remaining sets.
      // when we can successfully assign all available children to a slot the operator is a valid parent
      // when only a subset of all children can be assigned to slots the operator is no valid parent
      slotSets.Sort((p, q) => p.Count() - q.Count());

      int assignments = 0;
      for (int i = 0; i < slotSets.Count() - 1; i++) {
        if (slotSets[i].Count > 0) {
          IOperator selected = slotSets[i].ElementAt(0);
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
