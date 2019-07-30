#region License Information
/* HeuristicLab
 * Copyright (C) Joseph Helm and Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HEAL.Attic;
using HeuristicLab.Common;
using System.Collections.Generic;
using HeuristicLab.Encodings.IntegerVectorEncoding;

namespace HeuristicLab.Problems.BinPacking3D {
  [Item("Bottom-left IntegerVector Decoder (3d)", "Decodes the integer vector and creates a packing solution candidate")]
  [StorableType("3216A482-05F0-4E4C-B74E-E5C81A24DFC2")]
  public class BottomLeftIntegerVectorDecoder : IntegerVectorDecoderBase {

    [StorableConstructor]
    protected BottomLeftIntegerVectorDecoder(StorableConstructorFlag _) : base(_) { }
    protected BottomLeftIntegerVectorDecoder(BottomLeftIntegerVectorDecoder original, Cloner cloner)
      : base(original, cloner) {
    }
    public BottomLeftIntegerVectorDecoder() : base() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new BottomLeftIntegerVectorDecoder(this, cloner);
    }

    public override Solution Decode(IntegerVector intVec, PackingShape binShape, IList<PackingItem> items, bool useStackingConstraints) {
      // TODO
      if (useStackingConstraints) throw new NotSupportedException("Stacking constraints are not supported by the Bottom-Left IntegerVector Decoder");
      return base.Decode(intVec, binShape, items, useStackingConstraints: false);
    }

    protected override Solution CreateSolution(PackingShape binShape, bool useStackingConstraints) {
      return new Solution(binShape, useExtremePoints: false, stackingConstraints: useStackingConstraints);
    }

    protected override PackingPosition FindPositionForItem(BinPacking3D bp, PackingItem item, bool useStackingConstraints) {
      return bp.FindPositionBySliding(item, rotated: false, stackingConstraints: useStackingConstraints);
    }

    protected override BinPacking3D CreatePacking(
      Solution partialSolution,
      ref IList<int> remainingIDs, IList<PackingItem> items, bool useStackingConstraints) {
      var bp = new BinPacking3D(partialSolution.BinShape);
      bp.SlidingBasedPacking(ref remainingIDs, items, useStackingConstraints);
      return bp;
    }
  }
}
