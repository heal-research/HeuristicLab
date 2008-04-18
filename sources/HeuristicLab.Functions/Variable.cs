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
using System.Diagnostics;
using HeuristicLab.Data;
using HeuristicLab.Constraints;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Functions {
  public class Variable : FunctionBase {

    private ConstrainedIntData variable;
    private ConstrainedDoubleData weight;
    private ConstrainedIntData sampleOffset;

    public double SampleOffset {
      get { return sampleOffset.Data; }
    }

    public int VariableIndex {
      get { return variable.Data; }
    }

    public double Weight {
      get { return weight.Data; }
    }

    public override string Description {
      get { return @"Variable reads a value from a dataset, multiplies that value with a given factor and returns the result.
The variable 'SampleOffset' can be used to read a value from previous or following rows.
The index of the row that is actually read is SampleIndex+SampleOffset)."; }
    }

    public Variable()
      : base() {
      AddVariableInfo(new VariableInfo("Variable", "Index of the variable in the dataset representing this feature", typeof(ConstrainedIntData), VariableKind.None));
      GetVariableInfo("Variable").Local = true;
      AddVariableInfo(new VariableInfo("Weight", "Weight is multiplied to the feature value", typeof(ConstrainedDoubleData), VariableKind.None));
      GetVariableInfo("Weight").Local = true;
      AddVariableInfo(new VariableInfo("SampleOffset", "SampleOffset is added to the sample index", typeof(ConstrainedIntData), VariableKind.None));
      GetVariableInfo("SampleOffset").Local = true;

      variable = new ConstrainedIntData();
      AddLocalVariable(new HeuristicLab.Core.Variable("Variable", variable));

      weight = new ConstrainedDoubleData();
      // initialize a totally arbitrary range for the weight = [-20.0, 20.0]
      weight.AddConstraint(new DoubleBoundedConstraint(-20.0, 20.0));
      AddLocalVariable(new HeuristicLab.Core.Variable("Weight", weight));

      sampleOffset = new ConstrainedIntData();
      // initialize a totally arbitrary default range for sampleoffset = [-10, 10]
      sampleOffset.AddConstraint(new IntBoundedConstraint(0, 0));
      AddLocalVariable(new HeuristicLab.Core.Variable("SampleOffset", sampleOffset));

      // samplefeature can't have suboperators
      AddConstraint(new NumberOfSubOperatorsConstraint(0, 0));
    }

    public Variable(Variable source, IDictionary<Guid, object> clonedObjects)
      : base(source, clonedObjects) {

      variable = (ConstrainedIntData)GetVariable("Variable").Value;
      weight = (ConstrainedDoubleData)GetVariable("Weight").Value;
      sampleOffset = (ConstrainedIntData)GetVariable("SampleOffset").Value;
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      Variable clone = new Variable(this, clonedObjects);
      clonedObjects.Add(clone.Guid, clone);
      return clone;
    }

    public override void Populate(System.Xml.XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);

      variable = (ConstrainedIntData)GetVariable("Variable").Value;
      weight = (ConstrainedDoubleData)GetVariable("Weight").Value;
      sampleOffset = (ConstrainedIntData)GetVariable("SampleOffset").Value;
    }


    public override double Evaluate(Dataset dataset, int sampleIndex) {
      // local variables
      int v = variable.Data;
      double w = weight.Data;
      int offset = sampleOffset.Data;

      if(sampleIndex+offset<0 || sampleIndex+offset>=dataset.Rows) return double.NaN;
      return w * dataset.GetValue(sampleIndex + offset, v);
    }

    public override void Accept(IFunctionVisitor visitor) {
      visitor.Visit(this);
    }
  }
}
