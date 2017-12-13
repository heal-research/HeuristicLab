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
using HeuristicLab.Problems.BinPacking3D;
using HeuristicLab.Problems.BinPacking3D.ExtremePointCreation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.Packer {
  public abstract class BinPacker : Item {

    /*
    [Storable]
    IPositionFinder PositionFinder
    
    */

    #region Constructors for HEAL

    
    [StorableConstructor]
    protected BinPacker(bool deserializing) : base(deserializing) { }

    protected BinPacker(BinPacker original, Cloner cloner) 
      : base(original, cloner) {
      //this.PositionFinder = original.PositionFinder;
    }

    #endregion

    public BinPacker() { }

    /// <summary>
    /// Packs all items of the bin packer and returns a collection of BinPacking3D objects
    /// </summary>
    /// <param name="sortedItems">Permutation of items sorted by a sorting method. The value of each permutation index references to the index of the items list</param>
    /// <param name="binShape">Bin for storing the items</param>
    /// <param name="items">A list of packing items which should be assigned to a bin</param>
    /// <param name="useStackingConstraints">Flag for using stacking constraints</param>
    /// <returns>Returns a collection of bin packing 3d objects. Each object represents a bin and the packed items</returns>
    public abstract IList<BinPacking3D> PackItems(Permutation sortedItems, PackingShape binShape, IList<PackingItem> items, ExtremePointCreationMethod epCreationMethod, bool useStackingConstraints);
    
    /// <summary>
    /// Pack a given item into a given bin and updates the residual space and the extreme points
    /// </summary>
    /// <param name="packingBin"></param>
    /// <param name="itemId"></param>
    /// <param name="packingItem"></param>
    /// <param name="position"></param>
    protected virtual void PackItem(BinPacking3D packingBin, int itemId, PackingItem packingItem, PackingPosition position, IExtremePointCreator extremePointCreator, bool useStackingConstraints) {
      if (!CheckItemDimensions(packingBin, packingItem, position)) {
        throw new BinPacking3DException($"The dimensions of the given item exceeds the bin dimensions. " +
          $"Bin: ({packingBin.BinShape.Width} {packingBin.BinShape.Depth} {packingBin.BinShape.Height})" +
          $"Item: ({packingItem.Width} {packingItem.Depth} {packingItem.Height})");
      }
      packingBin.PackItem(itemId, packingItem, position);
      extremePointCreator.UpdateBinPacking(packingBin, packingItem, position);
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

      // The extremepoints are sortet by Y / Z / X
      return packingBin.ExtremePoints.Where(x => packingBin.IsPositionFeasible(newItem, x, useStackingConstraints)).FirstOrDefault();
    }

    /// <summary>
    /// Compares the dimensions of a given item and the current bin. 
    /// </summary>
    /// <param name="item"></param>
    /// <returns>Returns true if the dimensions of an given item are less or equal to the bin.</returns>
    private bool CheckItemDimensions(BinPacking3D packingBin, PackingItem item, PackingPosition itemPosition) {
      var width = itemPosition.Rotated ? item.Depth : item.Width;
      var depth = itemPosition.Rotated ? item.Width : item.Depth;
      return packingBin.BinShape.Width >= width && packingBin.BinShape.Height >= item.Height && packingBin.BinShape.Depth >= depth;
    }
  }
}
