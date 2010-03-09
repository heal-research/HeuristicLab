#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.Permutation {
  [Item("Permutation2OptMove", "Item that describes a 2-Opt move.")]
  public class Permutation2OptMove : Permutation2IndexMove {
    protected Permutation2OptMove()
      : base() {
    }

    public Permutation2OptMove(int index1, int index2)
      : base(index1, index2, null) {
    }

    public Permutation2OptMove(int index1, int index2, Permutation permutation)
      : base(index1, index2, permutation) {
    }
  }
}
