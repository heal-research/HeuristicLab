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

namespace HeuristicLab.Functions {
  /// <summary>
  /// Functions are like operators except that they do not allow sub-operators and the normal form of evaluation
  /// is to evaluate all children first.
  /// </summary>
  public abstract class FunctionBase : OperatorBase, IFunction {
    
    public abstract double Apply(Dataset dataset, int sampleIndex, double[] args);

    public virtual void Accept(IFunctionVisitor visitor) {
      visitor.Visit(this);
    }

    public virtual IFunctionTree GetTreeNode() {
      return new FunctionTree(this);
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
