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
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Random;

namespace HeuristicLab.SimOpt {
  public class TranslocationPermutationAdaptiveManipulator : SimOptManipulationOperatorBase {
    public override string Description {
      get { return @"Move a certain number of consecutive elements to a different part in an IntArray or Permutation.
Uses a shaking factor to provide an upper bound on the length of consecutive elments moved."; }
    }

    public TranslocationPermutationAdaptiveManipulator()
      : base() {
      AddVariableInfo(new VariableInfo("ShakingFactor", "A shaking factor that determines the maximum size of subtours which are to be translocated. The actual value is drawn from a uniform distribution between 1 and this factor (rounded)", typeof(DoubleData), VariableKind.In));
    }

    protected override void Apply(IScope scope, IRandom random, IItem item) {
      double shakingFactor = GetVariableValue<DoubleData>("ShakingFactor", scope, true).Data;
      if (item is Permutation.Permutation || item is IntArrayData) {
        IntArrayData data = (item as IntArrayData);
        int l = random.Next(1, (int)Math.Max(Math.Min((int)shakingFactor, data.Data.Length - 1), 2));
        int x = random.Next(data.Data.Length - l);
        int y;
        do {
          y = random.Next(data.Data.Length - l);
        } while (x == y);

        int[] h = new int[l];
        for (int i = 0; i < h.Length; i++)
          h[i] = data.Data[x + i];

        if (x > y) {
          while (x > y) {
            x--;
            data.Data[x + l] = data.Data[x];
          }
        } else {
          while (x < y) {
            data.Data[x] = data.Data[x + l];
            x++;
          }
        }
        for (int i = 0; i < h.Length; i++)
          data.Data[y + i] = h[i];
      } else throw new InvalidOperationException("ERROR: PermutationTranslocationManipulator does not know how to work with " + ((item != null) ? (item.GetType().ToString()) : ("null")) + " data");
    }
  }
}
