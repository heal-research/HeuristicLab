using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.BinPacking3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.Packer {
  [Item("BinPacker", "A class for packing bins for the 3D bin-packer problem.")]
  [StorableClass]
  public abstract class BinPacker {
    protected Permutation _permutation;
    protected PackingShape _binShape;
    protected IList<PackingItem> _items;
    protected bool _useStackingConstraints;

    /// <summary>
    /// Packs all items of the bin packer and returns a collection of BinPacking3D objects
    /// </summary>
    /// <returns></returns>
    public abstract IList<BinPacking3D> PackItems();

    /// <summary>
    /// Tries to pack the remainig items into a given BinPacking3D object. Each item could be packed into the BinPacking3D object will be removed from the list of remaining ids
    /// </summary>
    /// <param name="remainingIds">List of remaining ids. After the method has been executed the list has to have less items</param>
    /// <param name="items">List of packing items. Some of the items will be assigned to the BinPacking3D object</param>
    /// <param name="packingBin">This object will be filled with some of the given items</param>
    protected void PackRemainingItems(ref IList<int> remainingIds, ref BinPacking3D packingBin, IList<PackingItem> items, bool useStackingConstraints, Dictionary<int, bool> rotationArray) {

      foreach (var itemId in new List<int>(remainingIds)) {
        bool rotated = rotationArray == null ? false : rotationArray[itemId];
        PackingPosition position = FindPackingPositionForItem(packingBin, items[itemId], useStackingConstraints, rotated);
        // if a valid packing position could be found, the current item can be added to the given bin
        if (position != null) {
          PackItem(ref packingBin, itemId, items[itemId], position, useStackingConstraints);
          remainingIds.Remove(itemId);
        }
      }
    }

    /// <summary>
    /// Pack a given item into a given bin and updates the residual space and the extreme points
    /// </summary>
    /// <param name="packingBin"></param>
    /// <param name="itemId"></param>
    /// <param name="packingItem"></param>
    /// <param name="position"></param>
    protected void PackItem(ref BinPacking3D packingBin, int itemId, PackingItem packingItem, PackingPosition position, bool useStackingConstraints) {

      packingBin.AddItem(itemId, packingItem, position);
      packingBin.UpdateResidualSpace(packingItem, position);
      packingBin.UpdateExtremePoints(packingItem, position, useStackingConstraints);
    }

    /// <summary>
    /// This method tries to find a valid packing position for an given item in a given packing bin
    /// </summary>
    /// <param name="packingBin"></param>
    /// <param name="packingItem"></param>
    /// <param name="useStackingConstraints"></param>
    /// <param name="rotated"></param>
    /// <returns>Returns the packing position for an given item. If there could not be found a valid position it returns null</returns>
    protected PackingPosition FindPackingPositionForItem(BinPacking3D packingBin, PackingItem packingItem, bool useStackingConstraints, bool rotated) {
      PackingItem newItem = new PackingItem(
        rotated ? packingItem.Depth : packingItem.Width,
        packingItem.Height,
        rotated ? packingItem.Width : packingItem.Depth,
        packingItem.TargetBin, packingItem.Weight, packingItem.Material);

      // The extremepoints are sortet by Z, X, Y

      return packingBin.ExtremePoints.Where(x => packingBin.IsPositionFeasible(newItem, x, useStackingConstraints)).FirstOrDefault();
    }
  }
}
