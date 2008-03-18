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
using HeuristicLab.Data;

namespace HeuristicLab.Operators {
  public class Sorter : OperatorBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    public Sorter()
      : base() {
      AddVariableInfo(new VariableInfo("Descending", "Sort in descending order", typeof(BoolData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Value", "Sorting value", typeof(DoubleData), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      bool descending = GetVariableValue<BoolData>("Descending", scope, true).Data;
      double[] keys = new double[scope.SubScopes.Count];
      int[] sequence = new int[keys.Length];

      for (int i = 0; i < keys.Length; i++) {
        keys[i] = scope.SubScopes[i].GetVariableValue<DoubleData>("Value", false).Data;
        sequence[i] = i;
      }

      Array.Sort<double, int>(keys, sequence);

      if (descending) {
        int temp;
        for (int i = 0; i < sequence.Length / 2; i++) {
          temp = sequence[i];
          sequence[i] = sequence[sequence.Length - 1 - i];
          sequence[sequence.Length - 1 - i] = temp;
        }
      }
      scope.ReorderSubScopes(sequence);

      return null;
    }
  }
}
