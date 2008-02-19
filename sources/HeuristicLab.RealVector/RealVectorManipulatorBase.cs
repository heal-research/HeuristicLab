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
  public abstract class RealVectorManipulatorBase : OperatorBase {
    public RealVectorManipulatorBase() {
      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(IRandom), VariableKind.In));
      AddVariableInfo(new VariableInfo("RealVector", "Real vector to manipulate", typeof(DoubleArrayData), VariableKind.In | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      IRandom random = GetVariableValue<IRandom>("Random", scope, true);
      DoubleArrayData vector = GetVariableValue<DoubleArrayData>("RealVector", scope, false);
      vector.Data = Manipulate(scope, random, vector.Data);
      return null;
    }

    protected abstract double[] Manipulate(IScope scope, IRandom random, double[] vector);
  }
}
