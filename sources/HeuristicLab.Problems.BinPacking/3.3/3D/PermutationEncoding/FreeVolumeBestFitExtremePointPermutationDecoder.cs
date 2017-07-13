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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.BinPacking3D {
  [Item("Free Volume Best Fit Extreme-point Permutation Decoder (3d)", "Decodes the permutation and creates a packing solution candidate")]
  [StorableClass]
  public class FreeVolumeBestFitExtremePointPermutationDecoder : ExtremePointPermutationDecoderBase {

    [StorableConstructor]
    protected FreeVolumeBestFitExtremePointPermutationDecoder(bool deserializing) : base(deserializing) { }
    protected FreeVolumeBestFitExtremePointPermutationDecoder(FreeVolumeBestFitExtremePointPermutationDecoder original, Cloner cloner)
      : base(original, cloner) {
    }
    public FreeVolumeBestFitExtremePointPermutationDecoder() : base() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new FreeVolumeBestFitExtremePointPermutationDecoder(this, cloner);
    }

    public override Solution Decode(Permutation permutation, PackingShape binShape, IList<PackingItem> items, bool useStackingConstraints) {
      return Apply(permutation, binShape, items, useStackingConstraints);
    }

    public static Solution Apply(Permutation permutation, PackingShape binShape, IList<PackingItem> items, bool useStackingConstraints) {
      Solution result = new Solution(binShape, useExtremePoints: true, stackingConstraints: useStackingConstraints);
      IList<int> remainingIDs = new List<int>(permutation);
      var bpg = new BinPacking3D(binShape);
      bpg.ExtremePointBasedPacking(ref remainingIDs, items, stackingConstraints: useStackingConstraints);
      result.Bins.Add(bpg);
      foreach (int ID in remainingIDs) {
        var sortedBins = result.Bins.OrderBy(x => x.FreeVolume);
        var item = items[ID];
        var posFound = false;
        foreach (var bp in sortedBins) {
          var pos = bp.FindExtremePointForItem(item, false, useStackingConstraints);
          posFound = pos != null;
          if (posFound) {
            bp.PackItem(ID, item, pos);
            break;
          }
        }
        if (!posFound) {
          var bp = new BinPacking3D(binShape);
          var pos = bp.FindExtremePointForItem(item, false, useStackingConstraints);
          if (pos == null) throw new InvalidOperationException("Item " + ID + " cannot be packed in empty bin.");
          bp.PackItem(ID, item, pos);
          result.Bins.Add(bp);
        }
      }
      result.UpdateBinPackings();
      return result;
    }
  }
}
