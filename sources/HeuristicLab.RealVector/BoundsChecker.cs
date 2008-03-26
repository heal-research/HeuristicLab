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

namespace HeuristicLab.RealVector {
  public class BoundsChecker : OperatorBase {
    public override string Description {
      get { return "Checks if all elements of a real vector are inside a given minimum and maximum value. If not, elements are corrected."; }
    }

    public BoundsChecker()
      : base() {
      AddVariableInfo(new VariableInfo("RealVector", "Real vector to check", typeof(DoubleArrayData), VariableKind.In | VariableKind.Out));
      AddVariableInfo(new VariableInfo("Minimum", "Minimum value of each vector element (included).", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Maximum", "Maximum value of each vector element (included).", typeof(DoubleData), VariableKind.In));
    }

    public static double[] Apply(double min, double max, double[] vector) {
      int length = vector.Length;
      double[] result = (double[])vector.Clone();

      for (int i = 0; i < length; i++) {
        if (result[i] < min) result[i] = min;
        if (result[i] > max) result[i] = max;
      }
      return result;
    }

    public override IOperation Apply(IScope scope) {
      DoubleArrayData vector = GetVariableValue<DoubleArrayData>("RealVector", scope, false);
      double min = GetVariableValue<DoubleData>("Minimum", scope, true).Data;
      double max = GetVariableValue<DoubleData>("Maximum", scope, true).Data;
      vector.Data = Apply(min, max, vector.Data);
      return null;
    }
  }
}
