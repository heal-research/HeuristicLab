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

    private IVariable value;

    public override string Description {
      get { return "Returns the value of local variable 'Value'."; }
    }

    public ConstrainedDoubleData Value {
      get {
        return (ConstrainedDoubleData)value.Value;
      }
    }

    public Constant()
      : base() {
      AddVariableInfo(new VariableInfo("Value", "The constant value", typeof(ConstrainedDoubleData), VariableKind.None));
      GetVariableInfo("Value").Local = true;

      ConstrainedDoubleData valueData = new ConstrainedDoubleData();
      // initialize a default range for the contant value
      valueData.AddConstraint(new DoubleBoundedConstraint(-20.0, 20.0));

      // create the local variable
      value = new HeuristicLab.Core.Variable("Value", valueData);
      AddLocalVariable(value);

      // constant can't have suboperators
      AddConstraint(new NumberOfSubOperatorsConstraint(0, 0));
    }

    public Constant(Constant source, IDictionary<Guid, object> clonedObjects)
      : base(source, clonedObjects) {
      value = GetVariable("Value");
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      Constant clone = new Constant(this, clonedObjects);
      clonedObjects.Add(clone.Guid, clone);
      return clone;
    }


    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);

      value = GetVariable("Value");
    }

    public override double Evaluate(Dataset dataset, int sampleIndex) {
      return ((ConstrainedDoubleData)value.Value).Data;
    }

    public override void Accept(IFunctionVisitor visitor) {
      visitor.Visit(this);
    }
  }
}
