#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Joseph Helm and Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Encodings.IntegerVectorEncoding;

namespace HeuristicLab.Problems.BinPacking3D {
  [Item("Extreme-point IntegerVector Decoder (3d)", "Decodes the integer vector and creates a packing solution candidate")]
  [StorableClass]
  public class ExtremePointIntegerVectorDecoder : IntegerVectorDecoderBase {

    [StorableConstructor]
    protected ExtremePointIntegerVectorDecoder(bool deserializing) : base(deserializing) { }
    protected ExtremePointIntegerVectorDecoder(ExtremePointIntegerVectorDecoder original, Cloner cloner)
      : base(original, cloner) {
    }
    public ExtremePointIntegerVectorDecoder() : base() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExtremePointIntegerVectorDecoder(this, cloner);
    }

    protected override Solution CreateSolution(PackingShape binShape, bool useStackingConstraints) {
      return new Solution(binShape, useExtremePoints: true, stackingConstraints: useStackingConstraints);
    }

    protected override PackingPosition FindPositionForItem(BinPacking3D bp, PackingItem item, bool useStackingConstraints) {
      return bp.FindExtremePointForItem(item, rotated: false, stackingConstraints: useStackingConstraints);
    }

    protected override BinPacking3D CreatePacking(
      Solution partialSolution,
      ref IList<int> remainingIDs, IList<PackingItem> items, bool useStackingConstraints) {
      var bp = new BinPacking3D(partialSolution.BinShape);
      bp.ExtremePointBasedPacking(ref remainingIDs, items, useStackingConstraints);
      return bp;
    }
  }
}
