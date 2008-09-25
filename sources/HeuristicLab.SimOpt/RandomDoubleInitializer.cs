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
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.SimOpt {
  public class RandomDoubleInitializer : SimOptInitializationOperatorBase {
    public override string Description {
      get { return @"Initializes a DoubleData or ConstrainedDoubleData randomly in a given interval"; }
    }

    public RandomDoubleInitializer()
      : base() {
      AddVariableInfo(new VariableInfo("Min", "Minimum of the desired value", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Max", "Maximum of the desired value", typeof(DoubleData), VariableKind.In));
    }

    protected override void Apply(IScope scope, IRandom random, IItem item) {
      double min = GetVariableValue<DoubleData>("Min", scope, true).Data;
      double max = GetVariableValue<DoubleData>("Max", scope, true).Data;
      if (item is DoubleData) {
        double r = random.NextDouble();
        ((DoubleData)item).Data = min + (r * max - r * min);
        return;
      } else if (item is ConstrainedDoubleData) {
        ConstrainedDoubleData data = (item as ConstrainedDoubleData);
        for (int tries = 100; tries >= 0; tries--) {
          double r = random.NextDouble();
          double newValue = min + (r * max - r * min);

          if (IsIntegerConstrained(data)) newValue = Math.Round(newValue);
          if (data.TrySetData(newValue)) return;
        }
        throw new InvalidProgramException("ERROR: RandomDoubleInitializer couldn't find a valid value in 100 tries");
      } else throw new InvalidOperationException("ERROR: RandomDoubleInitializer does not know how to work with " + ((item != null) ? (item.GetType().ToString()) : ("null")) + " data");
    }
  }
}
