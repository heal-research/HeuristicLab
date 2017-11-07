using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.Packer {

  [Item("BinPackerFirstFit", "A class for packing bins for the 3D bin-packer problem. It uses a first fit algorithm")]
  [StorableClass]
  public class BinPackerFirstFit : BinPacker {

    public BinPackerFirstFit(Permutation permutation, PackingShape binShape, IList<PackingItem> items, bool useStackingConstraints) {
      _permutation = permutation;
      _binShape = binShape;
      _items = items;
      _useStackingConstraints = useStackingConstraints;
    }


    /// <summary>
    /// Packs the items of the object by using a first fit algorithm into an amount of bins and returns them
    /// </summary>
    /// <returns>Returns a collection of bin packing 3d objects. Each object represents a bin and the packed items</returns>
    public override IList<BinPacking3D> PackItems() {
      IList<BinPacking3D> packingList = new List<BinPacking3D>();
      IList<int> remainingIds = new List<int>(_permutation);

      while (remainingIds.Count > 0) {
        BinPacking3D packingBin = new BinPacking3D(_binShape);
        PackRemainingItems(ref remainingIds, ref packingBin, _items, _useStackingConstraints, null);
        packingList.Add(packingBin);
      }

      return packingList;
    }
  }
}
