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
using HeuristicLab.Constraints;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Functions {
  public class Constant : FunctionBase {
    public const string VALUE = "Value";

    public override string Description {
      get { return "Returns the value of local variable 'Value'."; }
    }

    public Constant()
      : base() {
      AddVariableInfo(new VariableInfo(VALUE, "The constant value", typeof(ConstrainedDoubleData), VariableKind.None));
      GetVariableInfo(VALUE).Local = true;

      ConstrainedDoubleData valueData = new ConstrainedDoubleData();
      // initialize a default range for the contant value
      valueData.AddConstraint(new DoubleBoundedConstraint(-20.0, 20.0));
      HeuristicLab.Core.Variable value = new HeuristicLab.Core.Variable(VALUE, valueData);
      AddVariable(value);

      // constant can't have suboperators
      AddConstraint(new NumberOfSubOperatorsConstraint(0, 0));
    }

    public override IFunctionTree GetTreeNode() {
      return new ConstantFunctionTree(this);
    }

    // can't apply a constant
    public override double Apply(Dataset dataset, int sampleIndex, double[] args) {
      throw new NotSupportedException();
    }

    public override void Accept(IFunctionVisitor visitor) {
      visitor.Visit(this);
    }
  }

  class ConstantFunctionTree : FunctionTree {
    private ConstrainedDoubleData value;
    public ConstantFunctionTree() : base() { }
    public ConstantFunctionTree(Constant constant) : base(constant) {
      UpdateCachedValues();
    }

    private void UpdateCachedValues() {
      value = (ConstrainedDoubleData)GetLocalVariable(Constant.VALUE).Value;
    }

    public override double Evaluate(Dataset dataset, int sampleIndex) {
      return value.Data;
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      ConstantFunctionTree clone = (ConstantFunctionTree)base.Clone(clonedObjects);
      clone.UpdateCachedValues();
      return clone;
    }

    public override void Populate(System.Xml.XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      UpdateCachedValues();
    }
  }
}
