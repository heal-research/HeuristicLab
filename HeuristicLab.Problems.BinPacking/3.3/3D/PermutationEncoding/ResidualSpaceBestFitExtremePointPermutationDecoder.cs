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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HEAL.Attic;

namespace HeuristicLab.Problems.BinPacking3D {
  [Item("Residual Space Best Fit Extreme-point Permutation Decoder (3d)", "Decodes the permutation and creates a packing solution candidate")]
  [StorableType("FD6679E5-CB7C-4DDA-B45F-DA6137CFA00E")]
  public class ResidualSpaceBestFitExtremePointPermutationDecoder : ExtremePointPermutationDecoderBase {

    [StorableConstructor]
    protected ResidualSpaceBestFitExtremePointPermutationDecoder(StorableConstructorFlag _) : base(_) { }
    protected ResidualSpaceBestFitExtremePointPermutationDecoder(ResidualSpaceBestFitExtremePointPermutationDecoder original, Cloner cloner)
      : base(original, cloner) {
    }
    public ResidualSpaceBestFitExtremePointPermutationDecoder() : base() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ResidualSpaceBestFitExtremePointPermutationDecoder(this, cloner);
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
        var item = items[ID];
        var points = GetResidualSpaceAllPoints(result, item);
        var sortedPoints = points.OrderBy(x => x.Item3);
        var packed = false;
        foreach (var p in sortedPoints) {
          packed = p.Item1.PackItemIfFeasible(ID, item, p.Item2, useStackingConstraints);
          if (packed) break;
        }
        if (!packed) {
          // pack item in a new bin
          var bp = new BinPacking3D(binShape);
          var positionFound = bp.FindExtremePointForItem(item, false, useStackingConstraints);
          if (positionFound != null) {
            bp.PackItem(ID, item, positionFound);
          } else throw new InvalidOperationException("Item " + ID + " cannot be packed in an empty bin.");
          result.Bins.Add(bp);
        }
      }
      result.UpdateBinPackings();
      return result;
    }

    public static IList<Tuple<BinPacking3D, PackingPosition, int>> GetResidualSpaceAllPoints(Solution solution, PackingItem item) {
      var result = new List<Tuple<BinPacking3D, PackingPosition, int>>();
      foreach (BinPacking3D bp in solution.Bins) {
        foreach (var ep in bp.ExtremePoints) {
          var rs = bp.ResidualSpace[ep];
          if (rs.Item1 < item.Width || rs.Item2 < item.Height || rs.Item3 < item.Depth) continue;
          result.Add(Tuple.Create(bp, ep, GetResidualMerit(rs, item, ep)));
        }
      }
      return result;
    }

    private static int GetResidualMerit(Tuple<int, int, int> rs, PackingItem item, PackingPosition ep) {
      return ((rs.Item1 - item.Width) +
          (rs.Item2 - item.Height) +
          (rs.Item3 - item.Depth));
    }
  }
}