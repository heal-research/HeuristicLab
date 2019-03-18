#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Joseph Helm and Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;
using System.Collections.Generic;
using HeuristicLab.Encodings.IntegerVectorEncoding;

namespace HeuristicLab.Problems.BinPacking2D {
  [Item("Bottom-left IntegerVector Decoder (2d)", "Decodes the integer vector and creates a packing solution candidate")]
  [StorableClass]
  public class BottomLeftIntegerVectorDecoder : IntegerVectorDecoderBase {

    [StorableConstructor]
    protected BottomLeftIntegerVectorDecoder(bool deserializing) : base(deserializing) { }
    protected BottomLeftIntegerVectorDecoder(BottomLeftIntegerVectorDecoder original, Cloner cloner)
      : base(original, cloner) {
    }
    public BottomLeftIntegerVectorDecoder() : base() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new BottomLeftIntegerVectorDecoder(this, cloner);
    }

    protected override Solution CreateSolution(PackingShape binShape) {
      return new Solution(binShape, useExtremePoints: false, stackingConstraints: false);
    }

    protected override PackingPosition FindPositionForItem(BinPacking2D bp, PackingItem item) {
      return bp.FindPositionBySliding(item, rotated: false, stackingConstraints: false);
    }

    protected override BinPacking2D CreatePacking(
      Solution partialSolution,
      ref IList<int> remainingIDs, IList<PackingItem> items) {
      var bp = new BinPacking2D(partialSolution.BinShape);
      bp.SlidingBasedPacking(ref remainingIDs, items, stackingConstraints: false);
      return bp;
    }
  }
}
