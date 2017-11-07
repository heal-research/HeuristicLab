using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.Packer {
  [Item("BinPackerResidualSpaceBestFit", "A class for packing bins for the 3D bin-packer problem. It uses a best fit algortihm depending on the residual space.")]
  [StorableClass]
  public class BinPackerResidualSpaceBestFit : BinPacker {
    public BinPackerResidualSpaceBestFit(Permutation permutation, PackingShape binShape, IList<PackingItem> items, bool useStackingConstraints) {
      _permutation = permutation;
      _binShape = binShape;
      _items = items;
      _useStackingConstraints = useStackingConstraints;
    }

    /// <summary>
    /// Packs the items into the bins by using a best fit residual space algorithm.
    /// The order of the chosen items depends on the merit function. 
    /// Each residual space belongs to an extreme point.
    /// </summary>
    /// <returns></returns>
    public override IList<BinPacking3D> PackItems() {
      IList<BinPacking3D> packingList = new List<BinPacking3D>();
      IList<int> remainingIds = new List<int>(_permutation);
      bool rotated = false;

      foreach (var remainingId in remainingIds) {
        PackingItem item = _items[remainingId];
        var residualSpacePoints = GetResidualSpaceForAllPoints(packingList, item);
        var sortedPoints = residualSpacePoints.OrderBy(x => x.Item3);
        var packed = false;

        foreach (var point in sortedPoints) {
          if (point.Item1.IsPositionFeasible(item, point.Item2, _useStackingConstraints)) {
            var binPacking = point.Item1;
            PackItem(ref binPacking, remainingId, item, point.Item2, _useStackingConstraints);
            packed = true;
            break;
          }
        }

        if (!packed) {
          BinPacking3D binPacking = new BinPacking3D(_binShape);
          var position = FindPackingPositionForItem(binPacking, item, _useStackingConstraints, rotated);
          if (position != null) {
            PackItem(ref binPacking, remainingId, item, position, _useStackingConstraints);
          } else {
            throw new InvalidOperationException("Item " + remainingId + " cannot be packed in an empty bin.");
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
          var rs = bp.ResidualSpace[ep];
          if (rs.Item1 < item.Width || rs.Item2 < item.Height || rs.Item3 < item.Depth) {
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
    private static int CalculateResidualMerit(Tuple<int, int, int> rs, PackingItem item, PackingPosition ep) {
      return ((rs.Item1 - item.Width) +
          (rs.Item2 - item.Height) +
          (rs.Item3 - item.Depth));
    }

  }
}
