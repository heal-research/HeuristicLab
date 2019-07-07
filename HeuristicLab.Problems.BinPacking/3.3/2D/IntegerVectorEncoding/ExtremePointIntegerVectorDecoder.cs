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

using HeuristicLab.Core;
using HEAL.Attic;
using HeuristicLab.Common;
using System.Collections.Generic;

namespace HeuristicLab.Problems.BinPacking2D {
  [Item("Extreme-point IntegerVector Decoder (2d)", "Decodes the integer vector and creates a packing solution candidate")]
  [StorableType("D40DEE7A-4AAC-47DA-9FA8-12D3C09177A7")]
  public class ExtremePointIntegerVectorDecoder : IntegerVectorDecoderBase {

    [StorableConstructor]
    protected ExtremePointIntegerVectorDecoder(StorableConstructorFlag _) : base(_) { }
    protected ExtremePointIntegerVectorDecoder(ExtremePointIntegerVectorDecoder original, Cloner cloner)
      : base(original, cloner) {
    }
    public ExtremePointIntegerVectorDecoder() : base() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExtremePointIntegerVectorDecoder(this, cloner);
    }

    protected override Solution CreateSolution(PackingShape binShape) {
      return new Solution(binShape, useExtremePoints: true, stackingConstraints: false);
    }

    protected override PackingPosition FindPositionForItem(BinPacking2D bp, PackingItem item) {
      return bp.FindExtremePointForItem(item, false, stackingConstraints: false);
    }

    protected override BinPacking2D CreatePacking(
      Solution partialSolution,
      ref IList<int> remainingIDs, IList<PackingItem> items) {
      var bp = new BinPacking2D(partialSolution.BinShape);
      bp.ExtremePointBasedPacking(ref remainingIDs, items, stackingConstraints: false);
      return bp;
    }
  }
}
