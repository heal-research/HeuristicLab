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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Random;

namespace HeuristicLab.SimOpt {
  public class UniformIntInitializer : SimOptInitializationOperatorBase {
    public override string Description {
      get { return @"Assigns an IntData or ConstrainedIntData a value uniformly distributed in the interval [min;max["; }
    }

    public UniformIntInitializer()
      : base() {
      AddVariableInfo(new VariableInfo("Min", "", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Max", "", typeof(IntData), VariableKind.In));
    }

    protected override void Apply(IScope scope, IRandom random, IItem item) {
      int min = GetVariableValue<IntData>("Min", scope, true).Data;
      int max = GetVariableValue<IntData>("Max", scope, true).Data;

      if (item is IntData) {
        ((IntData)item).Data = random.Next(min, max);
        return;
      } else if (item is ConstrainedIntData) {
        ConstrainedIntData data = (item as ConstrainedIntData);
        for (int tries = 100; tries >= 0; tries--) {
          if (data.TrySetData(random.Next(min, max))) return;
        }
        throw new InvalidProgramException("Coudn't find a valid value in 100 tries");
      } else throw new InvalidOperationException("ERROR: UniformIntManipulator does not know how to work with " + ((item != null) ? (item.GetType().ToString()) : ("null")) + " data");
    }
  }
}
