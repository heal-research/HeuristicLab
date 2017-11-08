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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.Packer {

  [Item("BinPackerFirstFit", "A class for packing bins for the 3D bin-packer problem. It uses a first fit algorithm")]
  [StorableClass]
  public class BinPackerFirstFit : BinPacker {

    public BinPackerFirstFit() : base() { }    

    /// <summary>
    /// Packs the items of the object by using a first fit algorithm into an amount of bins and returns them
    /// </summary>
    /// <returns>Returns a collection of bin packing 3d objects. Each object represents a bin and the packed items</returns>
    public override IList<BinPacking3D> PackItems(Permutation sortedItems, PackingShape binShape, IList<PackingItem> items, bool useStackingConstraints) {
      IList<BinPacking3D> packingList = new List<BinPacking3D>();
      IList<int> remainingIds = new List<int>(sortedItems);

      while (remainingIds.Count > 0) {
        BinPacking3D packingBin = new BinPacking3D(binShape);
        PackRemainingItems(ref remainingIds, ref packingBin, items, useStackingConstraints, null);
        packingList.Add(packingBin);
      }

      return packingList;
    }

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
  }
}
