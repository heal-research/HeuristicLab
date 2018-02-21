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
  internal class BinPackerFormClosure : BinPackerMinRSLeft {

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


    protected override void PackRemainingItems(ref IList<int> remainingIds, ref BinPacking3D packingBin, IList<PackingItem> items, ExtremePointCreationMethod epCreationMethod, bool useStackingConstraints) {
      IExtremePointCreator extremePointCreator = ExtremePointCreatorFactory.CreateExtremePointCreator(epCreationMethod, useStackingConstraints);
      // If there are any items which width equals to the bin shape width or the result of a modulo division of the width is zero, these items will be packed first
      //schauen, ob es einen gegenstand gibt, der die ganze breite ausfuellt oder bei einer division mit der breite ein rest von 0 rauskommt.
      var binShape = packingBin.BinShape;
      var placeable = remainingIds.ToList().Where(x => ItemFitsBinShapeWidth(x, items[x], binShape))
                                           .OrderByDescending(x => items[x].Width)
                                           .ThenByDescending(x => items[x].Height);
      foreach (var itemId in placeable) {
        var item = items[itemId];
        if (TryToPack(packingBin, item, itemId, extremePointCreator, useStackingConstraints)) {
          remainingIds.Remove(itemId);
        }
      }
      base.PackRemainingItems(ref remainingIds, ref packingBin, items, epCreationMethod, useStackingConstraints);
    }


      private bool ItemFitsBinShapeWidth(int itemId, PackingItem item, PackingShape binShape) {
      item.Rotated = false;
      item.Tilted = false;
      if (binShape.Width % item.Width == 0) {
        return true;
      }

      if (item.RotateEnabled && !item.TiltEnabled) {
        item.Rotated = true;
        if (binShape.Width % item.Width == 0) {
          return true;
        }
      }
      if (!item.RotateEnabled && item.TiltEnabled) {
        item.Rotated = false;
        item.Tilted = true;
        if (binShape.Width % item.Width == 0) {
          return true;
        }
      }
      if (item.RotateEnabled && item.TiltEnabled) {
        item.Rotated = true;
        item.Tilted = true;
        if (binShape.Width % item.Width == 0) {
          return true;
        }
      }
      item.Rotated = false;
      item.Tilted = false;
      return false;
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
  }
}
