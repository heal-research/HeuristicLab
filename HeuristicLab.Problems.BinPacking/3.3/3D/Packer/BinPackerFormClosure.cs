using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.BinPacking3D.ExtremePointCreation;

namespace HeuristicLab.Problems.BinPacking3D.Packer {
  internal class BinPackerFormClosure : BinPacker {

    #region Constructors for HEAL
    [StorableConstructor]
    protected BinPackerFormClosure(bool deserializing) : base(deserializing) { }

    public BinPackerFormClosure(BinPackerMinRSLeft original, Cloner cloner) : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      throw new NotImplementedException();
    }
    #endregion

    public BinPackerFormClosure() : base() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sortedItems"></param>
    /// <param name="binShape"></param>
    /// <param name="items"></param>
    /// <param name="epCreationMethod"></param>
    /// <param name="epPruningMethod"></param>
    /// <param name="useStackingConstraints"></param>
    /// <returns></returns>
    public override IList<BinPacking3D> PackItems(Permutation sortedItems, PackingShape binShape, IList<PackingItem> items, ExtremePointCreationMethod epCreationMethod, ExtremePointPruningMethod epPruningMethod, bool useStackingConstraints) {
      var workingItems = CloneItems(items);
      IList<BinPacking3D> packingList = new List<BinPacking3D>();
      IList<int> remainingIds = new List<int>(sortedItems);
      IExtremePointCreator extremePointCreator = ExtremePointCreatorFactory.CreateExtremePointCreator(epCreationMethod, useStackingConstraints);

      try {
        var groupedItems = items.Select((x, index) => new { Item = x, Id = index })
                                .GroupBy(x => x.Item.Layer)
                                .OrderBy(x => x.Key);

        BinPacking3D packingBin = new BinPacking3D(binShape);
        packingList.Add(packingBin);
        foreach (var grouping in groupedItems) {
          var notPacked = new List<KeyValuePair<int, PackingItem>>();
          var gr = grouping.ToList();
          var selectedItems = gr.Where(x => ItemBinWidth(x.Item, binShape))
                                .OrderByDescending(x => x.Item.Width)
                                .ThenByDescending(x => x.Item.Height)
                                .ThenByDescending(x => x.Item.Depth);
          var otherItems = gr.Except(selectedItems).OrderByDescending(x => x.Item.Width * x.Item.Height);
          foreach (var item in selectedItems) {
            if (TryToPack(packingBin, item.Item, item.Id, extremePointCreator, useStackingConstraints)) {
              remainingIds.Remove(item.Id);
            } else {
              notPacked.Add(new KeyValuePair<int, PackingItem>(item.Id, item.Item));
            }
          }
          foreach (var item in otherItems) {
            if (TryToPack(packingBin, item.Item, item.Id, extremePointCreator, useStackingConstraints)) {
              remainingIds.Remove(item.Id);
            } else {
              notPacked.Add(new KeyValuePair<int, PackingItem>(item.Id, item.Item));
            }
          }

          while (notPacked.Count > 0) {
            packingBin = new BinPacking3D(binShape);
            packingList.Add(packingBin);

            foreach (var item in notPacked.ToList()) {
              if (TryToPack(packingBin, item.Value, item.Key, extremePointCreator, useStackingConstraints)) {
                remainingIds.Remove(item.Key);
                notPacked.Remove(item);
              }
            }
          }
        }
      } catch { }


      return packingList;
    }

    private bool TryToPack(BinPacking3D packingBin, PackingItem item, int itemId, IExtremePointCreator extremePointCreator, bool useStackingConstraints) {
      var position = FindPackingPositionForItem(packingBin, item, useStackingConstraints);
      if (position != null) {
        PackItem(packingBin, itemId, item, position, extremePointCreator, useStackingConstraints);
        packingBin.PackItem(itemId, item, position);
        return true;
      }
      return false;
    }

    protected override PackingPosition FindPackingPositionForItem(BinPacking3D packingBin, PackingItem packingItem, bool useStackingConstraints) {
      if (!CheckItemDimensions(packingBin, packingItem)) {
        throw new BinPacking3DException($"The dimensions of the given item exceeds the bin dimensions. " +
          $"Bin: ({packingBin.BinShape.Width} {packingBin.BinShape.Depth} {packingBin.BinShape.Height})" +
          $"Item: ({packingItem.Width} {packingItem.Depth} {packingItem.Height})");
      }

      var newPosition = packingBin.ExtremePoints.Where(x => packingBin.IsPositionFeasible(packingItem, x.Key, useStackingConstraints)).FirstOrDefault();

      return newPosition.Key;
    }


    private static bool ItemBinWidth(PackingItem item, PackingShape bin) {
      var itemCopy = item.Clone() as PackingItem;
      int binWith = bin.Width;
      if (itemCopy.Width <= binWith && binWith % itemCopy.Width == 0) {
        return true;
      }

      itemCopy.Rotated = true;
      if (itemCopy.Width <= binWith && binWith % itemCopy.Width == 0) {
        item.Rotated = true;
        return true;
      }

      itemCopy.Rotated = false;
      itemCopy.Tilted = true;
      if (itemCopy.Width <= binWith && binWith % itemCopy.Width == 0) {
        item.Tilted = true;
        return true;
      }

      itemCopy.Rotated = true;
      itemCopy.Tilted = true;
      if (itemCopy.Width <= binWith && binWith % itemCopy.Width == 0) {
        item.Rotated = true;
        item.Tilted = true;
        return true;
      }
      return false;
    }

    public override void PackItemsToPackingList(IList<BinPacking3D> packingList, Permutation sortedItems, PackingShape binShape, IList<PackingItem> items, ExtremePointCreationMethod epCreationMethod, ExtremePointPruningMethod epPruningMethod, bool useStackingConstraints) {
      throw new NotImplementedException();
    }
  }
}
