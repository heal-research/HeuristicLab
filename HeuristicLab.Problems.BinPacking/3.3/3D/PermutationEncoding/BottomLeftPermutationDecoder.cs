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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;
using System.Collections.Generic;
using HeuristicLab.Encodings.PermutationEncoding;

namespace HeuristicLab.Problems.BinPacking3D {
  [Item("Bottom-left Permutation Decoder (3d)", "Decodes the permutation and creates a packing solution candidate")]
  [StorableClass]
  public class BottomLeftPermutationDecoder : Item, IDecoder<Permutation> {

    [StorableConstructor]
    protected BottomLeftPermutationDecoder(bool deserializing) : base(deserializing) { }
    protected BottomLeftPermutationDecoder(BottomLeftPermutationDecoder original, Cloner cloner)
      : base(original, cloner) {
    }
    public BottomLeftPermutationDecoder() : base() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new BottomLeftPermutationDecoder(this, cloner);
    }

    public Solution Decode(Permutation permutation, PackingShape binShape, IList<PackingItem> items, bool useStackingConstraints) {
      Solution result = new Solution(binShape, useExtremePoints: false, stackingConstraints: useStackingConstraints);
      IList<int> remainingIDs = new List<int>(permutation);
      while (remainingIDs.Count > 0) {
        var bp = new BinPacking3D(binShape);
        bp.SlidingBasedPacking(ref remainingIDs, items, useStackingConstraints);
        result.Bins.Add(bp);
      }
      result.UpdateBinPackings();
      return result;
    }
  }
}
