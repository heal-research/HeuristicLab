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
  [Item("InversionMoveAbsoluteAttribute", "Specifies the tabu attributes for an inversion move (2-opt) on an absolute position permutation.")]
  [StorableClass]
  public class InversionMoveAbsoluteAttribute : PermutationMoveAttribute {
    [Storable]
    public int Index1 { get; private set; }
    [Storable]
    public int Number1 { get; private set; }
    [Storable]
    public int Index2 { get; private set; }
    [Storable]
    public int Number2 { get; private set; }

    [StorableConstructor]
    private InversionMoveAbsoluteAttribute(bool deserializing)
      : base() {
    }

    public InversionMoveAbsoluteAttribute()
      : this(-1, -1, -1, -1, -1) { }

    public InversionMoveAbsoluteAttribute(int index1, int number1, int index2, int number2, double moveQuality)
      : base(moveQuality) {
      Index1 = index1;
      Number1 = number1;
      Index2 = index2;
      Number2 = number2;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      InversionMoveAbsoluteAttribute clone = (InversionMoveAbsoluteAttribute)base.Clone(cloner);
      clone.Index1 = Index1;
      clone.Number1 = Number1;
      clone.Index2 = Index2;
      clone.Number2 = Number2;
      return clone;
    }
  }
}
