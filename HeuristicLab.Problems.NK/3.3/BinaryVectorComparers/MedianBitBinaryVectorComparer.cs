#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.NK {
  [Item("Median Bit Binary Vector Comparer", "Compares two binary vectors by the median positive bit location")]
  [StorableClass]
  public sealed class MedianBitBinaryVectorComparer : Item, IBinaryVectorComparer {
    [StorableConstructor]
    private MedianBitBinaryVectorComparer(bool deserializing) : base(deserializing) { }
    private MedianBitBinaryVectorComparer(MedianBitBinaryVectorComparer original, Cloner cloner)
      : base(original, cloner) { }
    public MedianBitBinaryVectorComparer() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new MedianBitBinaryVectorComparer(this, cloner);
    }

    private static int MedianBit(BinaryVector x) {
      var activeIndices = x.Select((b, i) => new { b, i }).Where(v => v.b).ToList();
      if (activeIndices.Count > 0)
        return activeIndices[activeIndices.Count / 2].i;
      else
        return 0;
    }

    public int Compare(BinaryVector x, BinaryVector y) {
      return MedianBit(x) - MedianBit(y);
    }
  }
}
