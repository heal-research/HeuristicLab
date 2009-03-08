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
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.SA {
  public abstract class AnnealingBase : OperatorBase {

    public override string Description {
      get { return @"Base class for annealing operators"; }
    }

    public AnnealingBase()
      : base() {
      AddVariableInfo(new VariableInfo("Temperature", "The current temperature", typeof(DoubleData), VariableKind.In | VariableKind.Out));
      AddVariableInfo(new VariableInfo("AnnealingParameter", "The parameter that is used to adjust the temperature", typeof(DoubleData), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      DoubleData temperature = GetVariableValue<DoubleData>("Temperature", scope, true);
      DoubleData parameter = GetVariableValue<DoubleData>("AnnealingParameter", scope, true);
      temperature.Data = DoAnnealing(temperature.Data, parameter.Data, scope);
      return base.Apply(scope);
    }

    protected abstract double DoAnnealing(double temperature, double parameter, IScope scope);
  }
}
