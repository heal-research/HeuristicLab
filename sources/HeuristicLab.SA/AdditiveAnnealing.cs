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
  public class AdditiveAnnealing : AnnealingBase {

    public override string Description {
      get { return @"Adjusts the temperature by a constant value: T := T - parameter"; }
    }

    public AdditiveAnnealing()
      : base() {
    }

    public static double Apply(double temperature, double parameter) {
      return temperature - parameter;
    }

    protected override double DoAnnealing(double temperature, double parameter, IScope scope) {
      return Apply(temperature, parameter);
    }
  }
}
