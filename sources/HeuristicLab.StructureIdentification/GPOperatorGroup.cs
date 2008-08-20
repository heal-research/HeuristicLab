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
using HeuristicLab.Data;
using HeuristicLab.Random;
using HeuristicLab.Constraints;

namespace HeuristicLab.StructureIdentification {
  public class GPOperatorGroup : OperatorGroup {
    private Dictionary<IOperator, int> minTreeHeight = new Dictionary<IOperator, int>();
    private Dictionary<IOperator, int> minTreeSize = new Dictionary<IOperator, int>();
    private SubOperatorsConstraintAnalyser constraintAnalyser = new SubOperatorsConstraintAnalyser();

    public GPOperatorGroup()
      : base() {
    }

    public override void AddOperator(IOperator op) {
      base.AddOperator(op);
      var localVariableInfos = op.VariableInfos.Where(f => f.Local);

      if(op.GetVariable(GPOperatorLibrary.MIN_TREE_HEIGHT) == null) {
        op.AddVariable(new Variable(GPOperatorLibrary.MIN_TREE_HEIGHT, new IntData(-1)));
      }
      if(op.GetVariable(GPOperatorLibrary.MIN_TREE_SIZE) == null) {
        op.AddVariable(new Variable(GPOperatorLibrary.MIN_TREE_SIZE, new IntData(-1)));
      }
      if(op.GetVariable(GPOperatorLibrary.TICKETS) == null) {
        op.AddVariable(new Variable(GPOperatorLibrary.TICKETS, new DoubleData(1.0)));
      }
      foreach(IConstraint c in op.Constraints) {
        if(c is SubOperatorTypeConstraint || c is AllSubOperatorsTypeConstraint) c.Changed += new EventHandler(UpdateTreeBounds);
      }
      RecalculateMinimalTreeBounds();
      OnOperatorAdded(op);
    }

    void UpdateTreeBounds(object sender, EventArgs e) {
      RecalculateMinimalTreeBounds();
    }

    private void RecalculateMinimalTreeBounds() {
      minTreeHeight.Clear();
      minTreeSize.Clear();
      constraintAnalyser.AllPossibleOperators = Operators;

      foreach(IOperator op in Operators) {
        ((IntData)op.GetVariable(GPOperatorLibrary.MIN_TREE_HEIGHT).Value).Data = RecalculateMinimalTreeHeight(op);
        ((IntData)op.GetVariable(GPOperatorLibrary.MIN_TREE_SIZE).Value).Data = RecalculateMinimalTreeSize(op);
      }
    }

    private int RecalculateMinimalTreeSize(IOperator op) {
      // check for memoized value
      if(minTreeSize.ContainsKey(op)) {
        return minTreeSize[op];
      }

      int minArity;
      int maxArity;
      GetMinMaxArity(op, out minArity, out maxArity);
      // no suboperators possible => minimalTreeSize == 1 (the current node)
      if(minArity == 0 && maxArity == 0) {
        minTreeSize[op] = 1;
        return 1;
      }

      // when suboperators are necessary we have to find the smallest possible tree (recursively)
      // the minimal size of the parent is 1 + the sum of the minimal sizes of all subtrees
      int subTreeSizeSum = 0;

      // mark the currently processed operator to prevent infinite recursions and stack overflow
      minTreeSize[op] = 9999;
      for(int i = 0; i < minArity; i++) {
        // calculate the minTreeSize of all allowed sub-operators 
        // if the list of allowed suboperators is empty because the operator needs suboperators 
        // but there are no valid suboperators defined in the current group then we just use an impossible 
        // tree size here to indicate that there was a problem.
        // usually as more operators are added to the group the problem will be corrected (by adding the missing operator).
        // however if the missing operator is never added the high min tree size here has the effect that this operator
        // will not be included in generated subtrees because the resulting size would always be higher than a reasonably set 
        // maximal tree size.
        int minSubTreeSize = constraintAnalyser.GetAllowedOperators(op, i).Select(subOp => RecalculateMinimalTreeSize(subOp))
          .Concat(Enumerable.Repeat(9999, 1)).Min();
        subTreeSizeSum += minSubTreeSize;
      }

      minTreeSize[op] = subTreeSizeSum + 1;
      return subTreeSizeSum + 1;
    }

    private int RecalculateMinimalTreeHeight(IOperator op) {
      // check for memoized value
      if(minTreeHeight.ContainsKey(op)) {
        return minTreeHeight[op];
      }

      int minArity;
      int maxArity;
      GetMinMaxArity(op, out minArity, out maxArity);
      // no suboperators possible => minimalTreeHeight == 1
      if(minArity == 0 && maxArity == 0) {
        minTreeHeight[op] = 1;
        return 1;
      }

      // when suboperators are necessary we have to find the smallest possible tree (recursively)
      // the minimal height of the parent is 1 + the height of the largest subtree
      int maxSubTreeHeight = 0;

      // mark the currently processed operator to prevent infinite recursions leading to stack overflow
      minTreeHeight[op] = 9999;
      for(int i = 0; i < minArity; i++) {
        // calculate the minTreeHeight of all possible sub-operators.
        // use the smallest possible subTree as lower bound for the subTreeHeight.
        // if the list of allowed suboperators is empty because the operator needs suboperators 
        // but there are no valid suboperators defined in the current group then we use an impossible tree height
        // to indicate that there was a problem.
        // usually as more operators are added to the group the problem will be corrected (by adding the missing operator).
        // however if the missing operator is never added the high min tree height here has the effect that this operator
        // will not be included in generated subtrees because the resulting (virtual) height would always be higher than a reasonably set 
        // maximal tree height.
        int minSubTreeHeight = constraintAnalyser.GetAllowedOperators(op, i).Select(subOp => RecalculateMinimalTreeHeight(subOp))
          .Concat(Enumerable.Repeat(9999, 1)).Min();

        // if the smallest height of this subtree is larger than all other subtrees before we have to update the min height of the parent
        if(minSubTreeHeight > maxSubTreeHeight) {
          maxSubTreeHeight = minSubTreeHeight;
        }
      }

      minTreeHeight[op] = maxSubTreeHeight + 1;
      return maxSubTreeHeight + 1;
    }

    private void GetMinMaxArity(IOperator op, out int minArity, out int maxArity) {
      foreach(IConstraint constraint in op.Constraints) {
        NumberOfSubOperatorsConstraint theConstraint = constraint as NumberOfSubOperatorsConstraint;
        if(theConstraint != null) {
          minArity = theConstraint.MinOperators.Data;
          maxArity = theConstraint.MaxOperators.Data;
          return;
        }
      }
      // the default arity is 2
      minArity = 2;
      maxArity = 2;
    }

    public override void AddSubGroup(IOperatorGroup group) {
      throw new NotSupportedException();
    }

    public override void RemoveOperator(IOperator op) {
      base.RemoveOperator(op);
      op.RemoveVariable(GPOperatorLibrary.MIN_TREE_SIZE);
      op.RemoveVariable(GPOperatorLibrary.MIN_TREE_HEIGHT);
      op.RemoveVariable(GPOperatorLibrary.TICKETS);
      foreach(IConstraint c in op.Constraints) {
        if(c is SubOperatorTypeConstraint || c is AllSubOperatorsTypeConstraint) c.Changed -= new EventHandler(UpdateTreeBounds);
      }

      // remove the operator from the allowed sub-functions of the remaining operators
      foreach(IOperator o in Operators) {
        if(o != op) {
          foreach(IConstraint c in o.Constraints) {
            if(c is SubOperatorTypeConstraint) {
              ((SubOperatorTypeConstraint)c).RemoveOperator(op);
            } else if(c is AllSubOperatorsTypeConstraint) {
              ((AllSubOperatorsTypeConstraint)c).RemoveOperator(op);
            }
          }
        }
      }
      OnOperatorRemoved(op);
    }

    public override void RemoveSubGroup(IOperatorGroup group) {
      throw new NotSupportedException();
    }

    public event EventHandler OperatorAdded;
    public event EventHandler OperatorRemoved;

    protected virtual void OnOperatorAdded(IOperator op) {
      if(OperatorAdded != null) {
        OperatorAdded(this, new OperatorEventArgs(op));
      }
    }
    protected virtual void OnOperatorRemoved(IOperator op) {
      if(OperatorRemoved != null) {
        OperatorRemoved(this, new OperatorEventArgs(op));
      }
    }
  }

  internal class OperatorEventArgs : EventArgs {
    public IOperator op;

    public OperatorEventArgs(IOperator op) {
      this.op = op;
    }
  }
}
