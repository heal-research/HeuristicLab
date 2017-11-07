using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.Packer {
  [Item("BinPackerFreeVolumeBestFit", "A class for packing bins for the 3D bin-packer problem. It uses a best fit algortihm depending on the free volume.")]
  [StorableClass]
  public class BinPackerFreeVolumeBestFit : BinPacker {
    public BinPackerFreeVolumeBestFit(Permutation permutation, PackingShape binShape, IList<PackingItem> items, bool useStackingConstraints) {
      _permutation = permutation;
      _binShape = binShape;
      _items = items;
      _useStackingConstraints = useStackingConstraints;
    }

    public override IList<BinPacking3D> PackItems() {
      IList<BinPacking3D> packingList = new List<BinPacking3D>();
      IList<int> remainingIds = new List<int>(_permutation);
      

      foreach (int remainingId in remainingIds) {
        var sortedBins = packingList.OrderBy(x => x.FreeVolume);
        PackingItem item = _items[remainingId];
        bool positionFound = false;

        foreach (var packingBin in sortedBins) {
          PackingPosition position = FindPackingPositionForItem(packingBin, item, _useStackingConstraints, false);
          positionFound = position != null;
          var bin = packingBin;
          if (positionFound) {
            PackItem(ref bin, remainingId, item, position, _useStackingConstraints);
            break;
          }            
        }

        if (!positionFound) {
          BinPacking3D packingBin = new BinPacking3D(_binShape);
          PackingPosition position = FindPackingPositionForItem(packingBin, item, _useStackingConstraints, false);

          if (position == null) {
            throw new InvalidOperationException("Item " + remainingId + " cannot be packed in empty bin.");
          }

          PackItem(ref packingBin, remainingId, item, position, _useStackingConstraints);
          packingList.Add(packingBin);
        }
      }
      return packingList.ToList();
    }
  }
}
