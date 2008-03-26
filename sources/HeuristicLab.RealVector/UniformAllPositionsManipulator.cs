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
  public class UniformAllPositionsManipulator : RealVectorManipulatorBase {
    public override string Description {
      get { return "Uniformly distributed change of all positions of a real vector."; }
    }

    public UniformAllPositionsManipulator() {
      AddVariableInfo(new VariableInfo("Minimum", "Minimum of the sampling range for the vector element (included)", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Maximum", "Maximum of the sampling range for the vector element (excluded)", typeof(DoubleData), VariableKind.In));
    }

    public static double[] Apply(IRandom random, double[] vector, double min, double max) {
      double[] result = (double[])vector.Clone();

      for (int i = 0; i < result.Length; i++) {
        result[i] = min + random.NextDouble() * (max - min);
      }

      return result;
    }

    protected override double[] Manipulate(IScope scope, IRandom random, double[] vector) {
      double min = GetVariableValue<DoubleData>("Minimum", scope, true).Data;
      double max = GetVariableValue<DoubleData>("Maximum", scope, true).Data;
      return Apply(random, vector, min, max);
    }
  }
}
