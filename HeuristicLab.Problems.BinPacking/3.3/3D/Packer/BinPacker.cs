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
    
    public BinPacker() { }

    /// <summary>
    /// Packs all items of the bin packer and returns a collection of BinPacking3D objects
    /// </summary>
    /// <param name="sortedItems">Permutation of items sorted by a sorting method. The value of each permutation index references to the index of the items list</param>
    /// <param name="binShape">Bin for storing the items</param>
    /// <param name="items">A list of packing items which should be assigned to a bin</param>
    /// <param name="useStackingConstraints">Flag for using stacking constraints</param>
    /// <returns></returns>
    public abstract IList<BinPacking3D> PackItems(Permutation sortedItems, PackingShape binShape, IList<PackingItem> items, bool useStackingConstraints);

    

    /// <summary>
    /// Pack a given item into a given bin and updates the residual space and the extreme points
    /// </summary>
    /// <param name="packingBin"></param>
    /// <param name="itemId"></param>
    /// <param name="packingItem"></param>
    /// <param name="position"></param>
    protected void PackItem(ref BinPacking3D packingBin, int itemId, PackingItem packingItem, PackingPosition position, bool useStackingConstraints) {

      packingBin.PackItem(itemId, packingItem, position);
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
