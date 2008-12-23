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
using HeuristicLab.Evolutionary;

namespace HeuristicLab.IntVector {
  public abstract class IntVectorCrossoverBase : CrossoverBase {
    public IntVectorCrossoverBase()
      : base() {
      AddVariableInfo(new VariableInfo("IntVector", "Parent and child integer vector", typeof(IntArrayData), VariableKind.In | VariableKind.New));
    }

    protected sealed override void Cross(IScope scope, IRandom random, IScope parent1, IScope parent2, IScope child) {
      IVariableInfo intVectorInfo = GetVariableInfo("IntVector");
      IntArrayData vector1 = parent1.GetVariableValue<IntArrayData>(intVectorInfo.FormalName, false);
      IntArrayData vector2 = parent2.GetVariableValue<IntArrayData>(intVectorInfo.FormalName, false);

      if (vector1.Data.Length != vector2.Data.Length) throw new InvalidOperationException("Cannot apply crossover to integer vectors of different length.");

      int[] result = Cross(scope, random, vector1.Data, vector2.Data);
      child.AddVariable(new Variable(child.TranslateName(intVectorInfo.FormalName), new IntArrayData(result)));
    }

    protected abstract int[] Cross(IScope scope, IRandom random, int[] parent1, int[] parent2);
  }
}
