#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("InversionMove", "Item that describes an inversion move (2-opt).")]
  [StorableType("864C1259-8419-410F-8DD1-2C2751B1AF6B")]
  public class InversionMove : TwoIndexMove {
    [StorableConstructor]
    protected InversionMove(StorableConstructorFlag _) : base(_) { }
    protected InversionMove(InversionMove original, Cloner cloner) : base(original, cloner) { }
    public InversionMove() : base() { }
    public InversionMove(int index1, int index2) : base(index1, index2, null) { }
    public InversionMove(int index1, int index2, Permutation permutation) : base(index1, index2, permutation) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new InversionMove(this, cloner);
    }
  }
}
