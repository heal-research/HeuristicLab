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
  internal class BinPackerFreeVolumeBestFit : BinPacker {
    
    #region Constructors for HEAL
    [StorableConstructor]
    protected BinPackerFreeVolumeBestFit(bool deserializing) : base(deserializing) { }

    protected BinPackerFreeVolumeBestFit(BinPackerFreeVolumeBestFit original, Cloner cloner) 
      : base(original, cloner) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BinPackerFreeVolumeBestFit(this, cloner);
    }
    #endregion

    public BinPackerFreeVolumeBestFit() : base() { }

    /// <summary>
    /// Packs all items by using a free volume best fit strategy.
    /// If there is no bin packing item, a new one will be created an the current item will be packed into it.
    /// If there exists at least on bin packing item in the packing list they are being sortet by their free volume ascending. 
    /// The current item will be packed into the bin packing with the fewest free volume and enought space for placing it.
    /// If an item could not be placed in any bin packing, a new one will be created for the item.
    /// </summary>
    /// <param name="sortedItems"></param>
    /// <param name="binShape"></param>
    /// <param name="items"></param>
    /// <param name="useStackingConstraints"></param>
    /// <returns>Returns a collection of bin packing 3d objects. Each object represents a bin and the packed items</returns>
    public override IList<BinPacking3D> PackItems(Permutation sortedItems, PackingShape binShape, IList<PackingItem> items, ExtremePointCreationMethod epGenerationMethod, ExtremePointPruningMethod epPruningMethod, bool useStackingConstraints) {
      IList<BinPacking3D> packingList = new List<BinPacking3D>();
      IList<int> remainingIds = new List<int>(sortedItems);
      IExtremePointCreator extremePointCreator = ExtremePointCreatorFactory.CreateExtremePointCreator(epGenerationMethod, useStackingConstraints);
      foreach (int remainingId in remainingIds) {
        var sortedBins = packingList.OrderBy(x => x.FreeVolume);
        var z = sortedBins.ToList();

        PackingItem item = items[remainingId];
        bool positionFound = false;

        foreach (var packingBin in sortedBins) {
          PackingPosition position = FindPackingPositionForItem(packingBin, item, useStackingConstraints);
          positionFound = position != null;
          var bin = packingBin;
          if (positionFound) {
            PackItem(bin, remainingId, item, position, extremePointCreator, useStackingConstraints);
            break;
          }
        }

        if (!positionFound) {
          BinPacking3D packingBin = new BinPacking3D(binShape);
          PackingPosition position = FindPackingPositionForItem(packingBin, item, useStackingConstraints);

          if (position == null) {
            throw new InvalidOperationException("Item " + remainingId + " cannot be packed into an empty bin.");
          }

          PackItem(packingBin, remainingId, item, position, extremePointCreator, useStackingConstraints);
          packingList.Add(packingBin);
        }
      }
      ExtremePointPruningFactory.CreatePruning().PruneExtremePoints(epPruningMethod, packingList);
      return packingList.ToList();
    }

    public override void PackItemsToPackingList(IList<BinPacking3D> packingList, Permutation sortedItems, PackingShape binShape, IList<PackingItem> items, ExtremePointCreationMethod epCreationMethod, ExtremePointPruningMethod epPruningMethod, bool useStackingConstraints) {
      throw new NotImplementedException();
    }
  }
}
