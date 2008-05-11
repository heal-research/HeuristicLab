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
  public sealed class Variable : FunctionBase {

    public const string WEIGHT = "Weight";
    public const string OFFSET = "SampleOffset";
    public const string INDEX = "Variable";

    public override string Description {
      get {
        return @"Variable reads a value from a dataset, multiplies that value with a given factor and returns the result.
The variable 'SampleOffset' can be used to read a value from previous or following rows.
The index of the row that is actually read is SampleIndex+SampleOffset).";
      }
    }

    public Variable()
      : base() {
      AddVariableInfo(new VariableInfo(INDEX, "Index of the variable in the dataset representing this feature", typeof(ConstrainedIntData), VariableKind.None));
      GetVariableInfo(INDEX).Local = true;
      AddVariableInfo(new VariableInfo(WEIGHT, "Weight is multiplied to the feature value", typeof(ConstrainedDoubleData), VariableKind.None));
      GetVariableInfo(WEIGHT).Local = true;
      AddVariableInfo(new VariableInfo(OFFSET, "SampleOffset is added to the sample index", typeof(ConstrainedIntData), VariableKind.None));
      GetVariableInfo(OFFSET).Local = true;

      ConstrainedDoubleData weight = new ConstrainedDoubleData();
      // initialize a totally arbitrary range for the weight = [-20.0, 20.0]
      weight.AddConstraint(new DoubleBoundedConstraint(-20.0, 20.0));
      AddVariable(new HeuristicLab.Core.Variable(WEIGHT, weight));

      ConstrainedIntData variable = new ConstrainedIntData();
      AddVariable(new HeuristicLab.Core.Variable(INDEX, variable));

      ConstrainedIntData sampleOffset = new ConstrainedIntData();
      // initialize a totally arbitrary default range for sampleoffset = [-10, 10]
      sampleOffset.AddConstraint(new IntBoundedConstraint(0, 0));
      AddVariable(new HeuristicLab.Core.Variable(OFFSET, sampleOffset));

      // variable can't have suboperators
      AddConstraint(new NumberOfSubOperatorsConstraint(0, 0));
    }

    public override void Accept(IFunctionVisitor visitor) {
      visitor.Visit(this);
    }
  }
}
