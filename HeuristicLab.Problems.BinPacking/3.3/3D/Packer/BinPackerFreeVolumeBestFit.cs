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
  [Item("BinPackerFreeVolumeBestFit", "A class for packing bins for the 3D bin-packer problem. It uses a best fit algortihm depending on the free volume.")]
  [StorableClass]
  public class BinPackerFreeVolumeBestFit : BinPacker {

    public BinPackerFreeVolumeBestFit() : base() { }

    public override IList<BinPacking3D> PackItems(Permutation sortedItems, PackingShape binShape, IList<PackingItem> items, bool useStackingConstraints) {
      IList<BinPacking3D> packingList = new List<BinPacking3D>();
      IList<int> remainingIds = new List<int>(sortedItems);


      foreach (int remainingId in remainingIds) {
        var sortedBins = packingList.OrderBy(x => x.FreeVolume);
        var z = sortedBins.ToList();

        PackingItem item = items[remainingId];
        bool positionFound = false;

        foreach (var packingBin in sortedBins) {
          PackingPosition position = FindPackingPositionForItem(packingBin, item, useStackingConstraints, false);
          positionFound = position != null;
          var bin = packingBin;
          if (positionFound) {
            PackItem(ref bin, remainingId, item, position, useStackingConstraints);
            break;
          }
        }

        if (!positionFound) {
          BinPacking3D packingBin = new BinPacking3D(binShape);
          PackingPosition position = FindPackingPositionForItem(packingBin, item, useStackingConstraints, false);

          if (position == null) {
            throw new InvalidOperationException("Item " + remainingId + " cannot be packed in empty bin.");
          }

          PackItem(ref packingBin, remainingId, item, position, useStackingConstraints);
          packingList.Add(packingBin);
        }
      }
      return packingList.ToList();
    }
  }
}
