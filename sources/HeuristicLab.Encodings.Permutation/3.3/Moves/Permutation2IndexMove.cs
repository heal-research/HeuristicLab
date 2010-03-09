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
  [Item("Permutation2IndexMove", "A move on a permutation that is specified by 2 indices")]
  public class Permutation2IndexMove : Item {
    [Storable]
    public int Index1 { get; protected set; }
    [Storable]
    public int Index2 { get; protected set; }
    [Storable]
    public Permutation Permutation { get; protected set; }

    protected Permutation2IndexMove() {
      Index1 = -1;
      Index2 = -1;
      Permutation = null;
    }

    public Permutation2IndexMove(int index1, int index2, Permutation permutation)
      : base() {
      Index1 = index1;
      Index2 = index2;
      Permutation = permutation;
    }
  }
}