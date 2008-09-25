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
  public class TranslocationPermutationManipulator : SimOptManipulationOperatorBase {
    public override string Description {
      get { return @"Move a certain number of consecutive elements to a different part in an IntArray or Permutation."; }
    }

    public TranslocationPermutationManipulator()
      : base() {
    }

    protected override void Apply(IScope scope, IRandom random, IItem item) {
      if (item is Permutation.Permutation || item is IntArrayData) {
        IntArrayData data = (item as IntArrayData);
        int l = random.Next(1, data.Data.Length - 2);
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
