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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.BinPacking3D.ExtremePointCreation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.Packer {
  public class BinPackerResidualSpaceBestFit : BinPacker {

    #region Constructors for HEAL
    [StorableConstructor]
    protected BinPackerResidualSpaceBestFit(bool deserializing) : base(deserializing) { }

    protected BinPackerResidualSpaceBestFit(BinPackerResidualSpaceBestFit original, Cloner cloner) 
      : base(original, cloner) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BinPackerResidualSpaceBestFit(this, cloner);
    }
    #endregion

    public BinPackerResidualSpaceBestFit() : base() { }

    /// <summary>
    /// Packs the items into the bins by using a best fit residual space algorithm.
    /// The order of the chosen items depends on the merit function. 
    /// Each residual space belongs to an extreme point.
    /// </summary>
    /// <returns>Returns a collection of bin packing 3d objects. Each object represents a bin and the packed items</returns>
    public override IList<BinPacking3D> PackItems(Permutation sortedItems, PackingShape binShape, IList<PackingItem> items, ExtremePointCreationMethod epGenerationMethod, bool useStackingConstraints) {
      IList<BinPacking3D> packingList = new List<BinPacking3D>();
      IList<int> remainingIds = new List<int>(sortedItems);
      IExtremePointCreator extremePointCreator = ExtremePointCreatorFactory.CreateExtremePointCreator(epGenerationMethod, useStackingConstraints);
      bool rotated = false;

      foreach (var remainingId in remainingIds) {
        PackingItem item = items[remainingId];
        var residualSpacePoints = GetResidualSpaceForAllPoints(packingList, item);
        var sortedPoints = residualSpacePoints.OrderBy(x => x.Item3);
        var packed = false;

        foreach (var point in sortedPoints) {
          if (point.Item1.IsPositionFeasible(item, point.Item2, useStackingConstraints)) {
            var binPacking = point.Item1;
            PackItem(binPacking, remainingId, item, point.Item2, extremePointCreator, useStackingConstraints);
            packed = true;
            break;
          }
        }

        if (!packed) {
          BinPacking3D binPacking = new BinPacking3D(binShape);
          var position = FindPackingPositionForItem(binPacking, item, useStackingConstraints, rotated);
          if (position != null) {
            PackItem(binPacking, remainingId, item, position, extremePointCreator, useStackingConstraints);
          } else {
            throw new InvalidOperationException("Item " + remainingId + " cannot be packed into an empty bin.");
          }
          packingList.Add(binPacking);
        }
      }
      return packingList;
    }

    /// <summary>
    /// This method returns a list with all bins and their residual space where a given item can be placed.
    /// It is nessecary to get all bins and their residaul space because this list will be sortet later.
    /// </summary>
    /// <param name="packingList"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public static IList<Tuple<BinPacking3D, PackingPosition, int>> GetResidualSpaceForAllPoints(IList<BinPacking3D> packingList, PackingItem item) {
      var residualSpacePoints = new List<Tuple<BinPacking3D, PackingPosition, int>>();
      foreach (BinPacking3D bp in packingList) {
        foreach (var ep in bp.ExtremePoints) {
          var rs = bp.ResidualSpaces[ep];
          if (rs.Width < item.Width || rs.Height < item.Height || rs.Depth < item.Depth) {
            continue;
          }
          residualSpacePoints.Add(Tuple.Create(bp, ep, CalculateResidualMerit(rs, item, ep)));
        }
      }
      return residualSpacePoints;
    }

    /// <summary>
    /// The merit function puts an item on the EP that minimizes the difference between its residual space an the item dimension
    /// </summary>
    /// <param name="rs"></param>
    /// <param name="item"></param>
    /// <param name="ep"></param>
    /// <returns></returns>
    private static int CalculateResidualMerit(ResidualSpace rs, PackingItem item, PackingPosition ep) {
      return ((rs.Width - item.Width) +
          (rs.Height - item.Height) +
          (rs.Depth - item.Depth));
    }
  }
}
