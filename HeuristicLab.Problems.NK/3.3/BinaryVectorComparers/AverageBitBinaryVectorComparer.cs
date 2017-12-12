#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("Average Bit Binary Vector Comparer", "Compares two binary vectors by their average positive bit location")]
  [StorableClass]
  public sealed class AverageBitBinaryVectorComparer : Item, IBinaryVectorComparer {
    [StorableConstructor]
    private AverageBitBinaryVectorComparer(bool deserializing) : base(deserializing) { }
    private AverageBitBinaryVectorComparer(AverageBitBinaryVectorComparer original, Cloner cloner)
      : base(original, cloner) { }
    public AverageBitBinaryVectorComparer() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new AverageBitBinaryVectorComparer(this, cloner);
    }

    private static double AverageBit(BinaryVector x) {
      return x.Select((b, i) => new { b, i }).Where(v => v.b).Average(v => v.i);
    }

    public int Compare(BinaryVector x, BinaryVector y) {
      return (AverageBit(x) - AverageBit(y)).CompareTo(0);
    }
  }
}
