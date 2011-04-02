#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("TwoIndexMove", "A move on a permutation that is specified by 2 indices.")]
  [StorableClass]
  public class TwoIndexMove : Item {
    [Storable]
    public int Index1 { get; protected set; }
    [Storable]
    public int Index2 { get; protected set; }
    [Storable]
    public Permutation Permutation { get; protected set; }

    [StorableConstructor]
    protected TwoIndexMove(bool deserializing) : base(deserializing) { }
    protected TwoIndexMove(TwoIndexMove original, Cloner cloner)
      : base(original, cloner) {
      this.Index1 = original.Index1;
      this.Index2 = original.Index2;
      if (original.Permutation != null)
        this.Permutation = cloner.Clone(original.Permutation);
    }
    public TwoIndexMove() : this(-1, -1, null) { }
    public TwoIndexMove(int index1, int index2, Permutation permutation)
      : base() {
      Index1 = index1;
      Index2 = index2;
      Permutation = permutation;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TwoIndexMove(this, cloner);
    }
  }
}