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
  public class NormalIntManipulator : SimOptManipulationOperatorBase {
    public override string Description {
      get { return @"Perturbs an IntData or ConstrainedIntData by a value normally distributed around 0"; }
    }

    public NormalIntManipulator()
      : base() {
      AddVariableInfo(new VariableInfo("ShakingFactor", "Strength of the perturbation", typeof(DoubleData), VariableKind.In));
    }

    protected override void Apply(IScope scope, IRandom random, IItem item) {
      double shakingFactor = GetVariableValue<DoubleData>("ShakingFactor", scope, true).Data;
      NormalDistributedRandom normal = new NormalDistributedRandom(random, 0.0, shakingFactor);
      if (item is IntData) {
        ((IntData)item).Data += (int)normal.NextDouble();
        return;
      } else if (item is ConstrainedIntData) {
        ConstrainedIntData data = (item as ConstrainedIntData);
        for (int tries = 100; tries >= 0; tries--) {
          int newValue = data.Data + (int)normal.NextDouble();
          if (data.TrySetData(newValue)) return;
        }
        throw new InvalidProgramException("Coudn't find a valid value in 100 tries");
      } else throw new InvalidOperationException("ERROR: UniformIntManipulator does not know how to work with " + ((item != null) ? (item.GetType().ToString()) : ("null")) + " data");
    }
  }
}
