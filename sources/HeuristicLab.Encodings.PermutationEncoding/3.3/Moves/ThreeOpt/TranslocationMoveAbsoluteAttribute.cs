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

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("TranslocationMoveAbsoluteAttribute", "Specifies the tabu attributes for a translocation and insertion move (3-opt) on absolute permutation encodings.")]
  [StorableClass]
  public class TranslocationMoveAbsoluteAttribute : PermutationMoveAttribute {
    [Storable]
    public int[] Number { get; private set; }
    [Storable]
    public int OldPosition { get; private set; }
    [Storable]
    public int NewPosition { get; private set; }

    [StorableConstructor]
    private TranslocationMoveAbsoluteAttribute(bool deserializing)
      : base() {
    }

    public TranslocationMoveAbsoluteAttribute()
      : this(null, -1, -1, -1) { }

    public TranslocationMoveAbsoluteAttribute(int[] number, int oldPosition, int newPosition, double moveQuality)
      : base(moveQuality) {
      Number = number;
      OldPosition = oldPosition;
      NewPosition = newPosition;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      TranslocationMoveAbsoluteAttribute clone = (TranslocationMoveAbsoluteAttribute)base.Clone(cloner);
      clone.Number = Number;
      clone.OldPosition = OldPosition;
      clone.NewPosition = NewPosition;
      return clone;
    }
  }
}
