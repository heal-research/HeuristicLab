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
using HeuristicLab.Data;
using HeuristicLab.Core;
using System.Xml;
using HeuristicLab.DataAnalysis;
using HeuristicLab.Constraints;
using System.Diagnostics;

namespace HeuristicLab.Functions {
  /// <summary>
  /// Functions are like operators except that they do not allow sub-operators and the normal form of evaluation
  /// is to evaluate all children first.
  /// </summary>
  public abstract class FunctionBase : OperatorBase, IFunction {
    public const string INITIALIZATION = "Initialization";
    public const string MANIPULATION = "Manipulation";
    private List<IFunction>[] allowedSubFunctions;
    private int minArity = -1;
    private int maxArity = -1;

    public virtual double Apply(Dataset dataset, int sampleIndex, double[] args) {
      throw new NotImplementedException();
    }

    public virtual void Accept(IFunctionVisitor visitor) {
      visitor.Visit(this);
    }

    public virtual IFunctionTree GetTreeNode() {
      return new BakedFunctionTree(this);
    }

    public int MinArity {
      get {
        if(minArity < 0) RefreshArity();
        return minArity;
      }
    }

    public int MaxArity {
      get {
        if(maxArity < 0) RefreshArity();
        return maxArity;
      }
    }

    private void RefreshArity() {
      minArity = 2; maxArity = 2; // default arity is 2
      foreach(IConstraint constraint in Constraints) {
        NumberOfSubOperatorsConstraint theConstraint = constraint as NumberOfSubOperatorsConstraint;
        if(theConstraint != null) {
          minArity = theConstraint.MinOperators.Data;
          maxArity = theConstraint.MaxOperators.Data;
        }
      }
    }

    public IList<IFunction> AllowedSubFunctions(int index) {
      if(allowedSubFunctions == null) {
        // first time: analyze the constraint and create a cached copy of the allowed sub-functions
        allowedSubFunctions = new List<IFunction>[MaxArity];
        for(int i = 0; i < MaxArity; i++) {
          allowedSubFunctions[i] = GetAllowedSubFunctions(i);
        }
      }
      return allowedSubFunctions[index];
    }

    private List<IFunction> GetAllowedSubFunctions(int index) {
      List<IFunction> allowedSubFunctions = new List<IFunction>();
      foreach(IConstraint constraint in Constraints) {
        if(constraint is SubOperatorTypeConstraint) {
          SubOperatorTypeConstraint subOpConstraint = constraint as SubOperatorTypeConstraint;
          if(subOpConstraint.SubOperatorIndex.Data == index) {
            foreach(IFunction f in subOpConstraint.AllowedSubOperators) allowedSubFunctions.Add(f);
            subOpConstraint.Changed += new EventHandler(subOpConstraint_Changed); // register an event-handler to invalidate the cache on constraing changes
            return allowedSubFunctions;
          }
        } else if(constraint is AllSubOperatorsTypeConstraint) {
          AllSubOperatorsTypeConstraint subOpConstraint = constraint as AllSubOperatorsTypeConstraint;
          foreach(IFunction f in subOpConstraint.AllowedSubOperators) allowedSubFunctions.Add(f);
          subOpConstraint.Changed += new EventHandler(subOpConstraint_Changed); // register an event-handler to invalidate the cache on constraint changes
          return allowedSubFunctions;
        }
      }
      return allowedSubFunctions;
    }

    private void subOpConstraint_Changed(object sender, EventArgs e) {
      allowedSubFunctions = null;
    }

    // operator-tree style evaluation is not supported for functions.
    public override IOperation Apply(IScope scope) {
      throw new NotSupportedException();
    }

    private static readonly List<IOperator> emptySubOperatorList = new List<IOperator>();
    public override IList<IOperator> SubOperators {
      get { return emptySubOperatorList; }
    }

    public override void AddSubOperator(IOperator subOperator) {
      throw new NotSupportedException();
    }

    public override bool TryAddSubOperator(IOperator subOperator) {
      throw new NotSupportedException();
    }

    public override bool TryAddSubOperator(IOperator subOperator, int index) {
      throw new NotSupportedException();
    }

    public override bool TryAddSubOperator(IOperator subOperator, int index, out ICollection<IConstraint> violatedConstraints) {
      throw new NotSupportedException();
    }

    public override bool TryAddSubOperator(IOperator subOperator, out ICollection<IConstraint> violatedConstraints) {
      throw new NotSupportedException();
    }

    public override void AddSubOperator(IOperator subOperator, int index) {
      throw new NotSupportedException();
    }

    public override void RemoveSubOperator(int index) {
      throw new NotSupportedException();
    }

    public override bool TryRemoveSubOperator(int index) {
      throw new NotSupportedException();
    }

    public override bool TryRemoveSubOperator(int index, out ICollection<IConstraint> violatedConstraints) {
      throw new NotSupportedException();
    }
  }
}
