#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2009 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Data;
using HeuristicLab.Evolutionary;

namespace HeuristicLab.SimOpt {
  public class NumericDataAverageMultiCrossover : CrossoverBase {

    public override string Description {
      get { return @"Takes two DoubleData, or ConstrainedDoubleData, and averages them."; }
    }

    public NumericDataAverageMultiCrossover()
      : base() {
      AddVariableInfo(new VariableInfo("VariableName", "The name of the variable to cross", typeof(IObjectData), VariableKind.In | VariableKind.Out));
    }

    protected override void Cross(IScope scope, IRandom random) {
      IScope[] parents = scope.SubScopes.ToArray<IScope>();
      if (parents.Length == 0) throw new ArgumentException("ERROR in NumericDataAverageMultiCrossover: No parents are given");
      double sum = 0.0;
      IVariable var = null;
      string name = scope.TranslateName("VariableName");
      for (int i = 0; i < parents.Length; i++) {
        var = parents[i].GetVariable(name);
        if (var.Value is DoubleData) {
          sum += ((DoubleData)var.Value).Data;
        } else if (var.Value is ConstrainedDoubleData) {
          sum += ((ConstrainedDoubleData)var.Value).Data;
        } else throw new InvalidOperationException("ERROR in NumericDataAverageMultiCrossover: Encountered unknown type: " + ((var.Value != null) ? (var.Value.GetType().ToString()) : ("null")));
      }
      double average = sum / (double)parents.Length;
      IVariable varChild = (IVariable)var.Clone();
      if (varChild.Value is DoubleData) {
        (varChild.Value as DoubleData).Data = average;
      } else if (var.Value is ConstrainedDoubleData) {
        (varChild.Value as ConstrainedDoubleData).TrySetData(average);
      }
      IVariable varScope = scope.GetVariable(name);
      if (varScope == null)
        scope.AddVariable(varChild);
      else varScope.Value = varChild.Value;
    }
  }
}
