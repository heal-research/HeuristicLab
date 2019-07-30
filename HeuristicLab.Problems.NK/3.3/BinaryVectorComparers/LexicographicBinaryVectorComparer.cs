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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HEAL.Attic;

namespace HeuristicLab.Problems.NK {
  [Item("Lexicographic Binary Vector Comparer", "Compares two binary vectors lexicographically")]
  [StorableType("E107BAC1-B863-4704-9129-F258B0974285")]
  public sealed class LexicographicBinaryVectorComparer : Item, IBinaryVectorComparer {
    [StorableConstructor]
    private LexicographicBinaryVectorComparer(StorableConstructorFlag _) : base(_) { }
    private LexicographicBinaryVectorComparer(LexicographicBinaryVectorComparer original, Cloner cloner)
      : base(original, cloner) { }
    public LexicographicBinaryVectorComparer() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new LexicographicBinaryVectorComparer(this, cloner);
    }

    public int Compare(BinaryVector x, BinaryVector y) {
      for (int i = 0; i < Math.Min(x.Length, y.Length); i++) {
        if (!x[i] && y[i])
          return -1;
        if (x[i] && !y[i])
          return 1;
      }
      if (x.Length > y.Length)
        return 1;
      if (x.Length < y.Length)
        return -1;
      return 0;
    }
  }
}
