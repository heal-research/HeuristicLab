#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
    protected InversionMoveAbsoluteAttribute(bool deserializing) : base(deserializing) { }
    protected InversionMoveAbsoluteAttribute(InversionMoveAbsoluteAttribute original, Cloner cloner)
      : base(original, cloner) {
      this.Index1 = original.Index1;
      this.Number1 = original.Number1;
      this.Index2 = original.Index2;
      this.Number2 = original.Number2;
    }
    public InversionMoveAbsoluteAttribute() : this(-1, -1, -1, -1, -1) { }
    public InversionMoveAbsoluteAttribute(int index1, int number1, int index2, int number2, double moveQuality)
      : base(moveQuality) {
      Index1 = index1;
      Number1 = number1;
      Index2 = index2;
      Number2 = number2;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new InversionMoveAbsoluteAttribute(this, cloner);
    }
  }
}
