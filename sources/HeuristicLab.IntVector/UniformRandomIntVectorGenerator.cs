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
  public class UniformRandomIntVectorGenerator : OperatorBase {
    public override string Description {
      get { return "Operator generating a new random integer vector with each element uniformly distributed in a specified range."; }
    }

    public UniformRandomIntVectorGenerator() {
      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(IRandom), VariableKind.In));
      AddVariableInfo(new VariableInfo("Length", "Vector length", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Minimum", "Minimum of the sampling range for each vector element (included)", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Maximum", "Maximum of the sampling range for each vector element (excluded)", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("IntVector", "Created random integer vector", typeof(IntArrayData), VariableKind.New));
    }

    public static int[] Apply(IRandom random, int length, int min, int max) {
      int[] result = new int[length];
      for (int i = 0; i < length; i++)
        result[i] = random.Next(min, max);
      return result;
    }

    public override IOperation Apply(IScope scope) {
      IRandom random = GetVariableValue<IRandom>("Random", scope, true);
      int length = GetVariableValue<IntData>("Length", scope, true).Data;
      int min = GetVariableValue<IntData>("Minimum", scope, true).Data;
      int max = GetVariableValue<IntData>("Maximum", scope, true).Data;

      int[] vector = Apply(random, length, min, max);
      scope.AddVariable(new Variable(scope.TranslateName("IntVector"), new IntArrayData(vector)));

      return null;
    }
  }
}
