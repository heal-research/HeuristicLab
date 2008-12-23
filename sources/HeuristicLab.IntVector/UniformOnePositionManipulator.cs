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

namespace HeuristicLab.IntVector {
  public class UniformOnePositionManipulator : IntVectorManipulatorBase {
    public override string Description {
      get { return "Uniformly distributed change of a single position of an integer vector."; }
    }

    public UniformOnePositionManipulator() {
      AddVariableInfo(new VariableInfo("Minimum", "Minimum of the sampling range for the vector element (included)", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Maximum", "Maximum of the sampling range for the vector element (excluded)", typeof(IntData), VariableKind.In));
    }

    public static int[] Apply(IRandom random, int[] vector, int min, int max) {
      int[] result = (int[])vector.Clone();
      int index = random.Next(result.Length);
      result[index] = random.Next(min, max);
      return result;
    }

    protected override int[] Manipulate(IScope scope, IRandom random, int[] vector) {
      int min = GetVariableValue<IntData>("Minimum", scope, true).Data;
      int max = GetVariableValue<IntData>("Maximum", scope, true).Data;
      return Apply(random, vector, min, max);
    }
  }
}
