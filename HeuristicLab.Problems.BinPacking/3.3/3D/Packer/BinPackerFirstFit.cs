#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Problems.BinPacking3D.ExtremePointPruning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.Packer {
  internal class BinPackerFirstFit : BinPacker {
    #region Constructors for HEAL
    [StorableConstructor]
    protected BinPackerFirstFit(bool deserializing) : base(deserializing) { }

    protected BinPackerFirstFit(BinPackerFirstFit original, Cloner cloner) 
      : base(original, cloner) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BinPackerFirstFit(this, cloner);
    }
    #endregion

    public BinPackerFirstFit() : base() { }    

    /// <summary>
    /// Packs the items of the object by using a first fit algorithm into an amount of bins and returns them.
    /// </summary>
    /// <returns>Returns a collection of bin packing 3d objects. Each object represents a bin and the packed items</returns>
    public override IList<BinPacking3D> PackItems(Permutation sortedItems, PackingShape binShape, IList<PackingItem> items, ExtremePointCreationMethod epCreationMethod, ExtremePointPruningMethod epPruningMethod, bool useStackingConstraints) {
      IList<BinPacking3D> packingList = new List<BinPacking3D>();
      IList<int> remainingIds = new List<int>(sortedItems);

      while (remainingIds.Count > 0) {
        BinPacking3D packingBin = new BinPacking3D(binShape);
        PackRemainingItems(ref remainingIds, ref packingBin, items, epCreationMethod, useStackingConstraints);
        packingList.Add(packingBin);
      }

      ExtremePointPruningFactory.CreatePruning().PruneExtremePoints(epPruningMethod, packingList);
      return packingList;
    }

    /// <summary>
    /// Tries to pack the remainig items into a given BinPacking3D object. Each item could be packed into the BinPacking3D object will be removed from the list of remaining ids
    /// </summary>
    /// <param name="remainingIds">List of remaining ids. After the method has been executed the list has to have less items</param>
    /// <param name="packingBin">This object will be filled with some of the given items</param>
    /// <param name="items">List of packing items. Some of the items will be assigned to the BinPacking3D object</param>
    /// <param name="epCreationMethod"></param>
    /// <param name="useStackingConstraints"></param>
    protected void PackRemainingItems(ref IList<int> remainingIds, ref BinPacking3D packingBin, IList<PackingItem> items, ExtremePointCreationMethod epCreationMethod, bool useStackingConstraints) {
      IExtremePointCreator extremePointCreator = ExtremePointCreatorFactory.CreateExtremePointCreator(epCreationMethod, useStackingConstraints);
      foreach (var itemId in new List<int>(remainingIds)) {
        PackingPosition position = FindPackingPositionForItem(packingBin, items[itemId], useStackingConstraints);
        // if a valid packing position could be found, the current item can be added to the given bin
        if (position != null) {
          PackItem(packingBin, itemId, items[itemId], position, extremePointCreator, useStackingConstraints);
          remainingIds.Remove(itemId);
        }
      }
    }

    public override void PackItemsToPackingList(IList<BinPacking3D> packingList, Permutation sortedItems, PackingShape binShape, IList<PackingItem> items, ExtremePointCreationMethod epCreationMethod, ExtremePointPruningMethod epPruningMethod, bool useStackingConstraints) {
      throw new NotImplementedException();
    }
  }
}
