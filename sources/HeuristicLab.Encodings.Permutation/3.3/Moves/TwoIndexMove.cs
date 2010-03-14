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
  [Item("TwoIndexMove", "A move on a permutation that is specified by 2 indices")]
  [StorableClass]
  public class TwoIndexMove : Item {
    [Storable]
    public int Index1 { get; protected set; }
    [Storable]
    public int Index2 { get; protected set; }
    [Storable]
    public Permutation Permutation { get; protected set; }

    public TwoIndexMove()
      : base() {
      Index1 = -1;
      Index2 = -1;
      Permutation = null;
    }

    public TwoIndexMove(int index1, int index2, Permutation permutation)
      : base() {
      Index1 = index1;
      Index2 = index2;
      Permutation = permutation;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      TwoIndexMove clone = (TwoIndexMove)base.Clone(cloner);
      clone.Index1 = Index1;
      clone.Index2 = Index2;
      if (Permutation != null)
        clone.Permutation = (Permutation)Permutation.Clone(cloner);
      return clone;
    }
  }
}