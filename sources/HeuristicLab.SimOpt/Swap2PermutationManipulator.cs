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
  public class Swap2PermutationManipulator : SimOptManipulationOperatorBase {
    public override string Description {
      get { return @"Swap two elements of a permutation or IntArray"; }
    }

    public Swap2PermutationManipulator()
      : base() {
    }

    protected override void Apply(IScope scope, IRandom random, IItem item) {
      if (item is Permutation.Permutation || item is IntArrayData) {
        IntArrayData data = (item as IntArrayData);
        int x = random.Next(data.Data.Length);
        int y;
        do {
          y = random.Next(data.Data.Length);
        } while (x == y);

        int h = data.Data[x];
        data.Data[x] = data.Data[y];
        data.Data[y] = h;
      } else throw new InvalidOperationException("ERROR: Swap2PermutationManipulator does not know how to work with " + ((item != null) ? (item.GetType().ToString()) : ("null")) + " data");
    }
  }
}
