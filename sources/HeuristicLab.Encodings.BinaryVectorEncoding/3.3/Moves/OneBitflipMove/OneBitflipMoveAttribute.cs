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

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  [Item("OneBitflipMoveAttribute", "Base class for specifying a move attribute.")]
  [StorableClass]
  public class OneBitflipMoveAttribute : Item {
    [Storable]
    public double MoveQuality { get; protected set; }
    [Storable]
    public int Index { get; protected set; }

    public OneBitflipMoveAttribute()
      : this(-1, 0) {
    }

    public OneBitflipMoveAttribute(int index, double moveQuality)
      : base() {
      Index = index;
      MoveQuality = moveQuality;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      OneBitflipMoveAttribute clone = (OneBitflipMoveAttribute)base.Clone(cloner);
      clone.MoveQuality = MoveQuality;
      clone.Index = Index;
      return clone;
    }
  }
}
