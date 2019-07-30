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
  [Item("TranslocationMove", "A move that changes three edges by performing a translocation.")]
  [StorableType("D8C65083-9375-4665-8A90-1A9EAD3EDA97")]
  public class TranslocationMove : ThreeIndexMove {
    [StorableConstructor]
    protected TranslocationMove(StorableConstructorFlag _) : base(_) { }
    protected TranslocationMove(TranslocationMove original, Cloner cloner) : base(original, cloner) { }
    public TranslocationMove() : base() { }
    public TranslocationMove(int index1, int index2, int index3) : base(index1, index2, index3, null) { }
    public TranslocationMove(int index1, int index2, int index3, Permutation permutation) : base(index1, index2, index3, permutation) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TranslocationMove(this, cloner);
    }
  }
}
