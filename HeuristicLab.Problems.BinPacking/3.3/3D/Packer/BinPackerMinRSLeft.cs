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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.BinPacking3D.ExtremePointCreation;
using HeuristicLab.Problems.BinPacking3D.ExtremePointPruning;

namespace HeuristicLab.Problems.BinPacking3D.Packer {
  internal class BinPackerMinRSLeft : BinPacker {
    #region Constructors for HEAL
    [StorableConstructor]
    protected BinPackerMinRSLeft(bool deserializing) : base(deserializing) { }

    public BinPackerMinRSLeft(BinPackerMinRSLeft original, Cloner cloner) : base(original, cloner) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BinPackerMinRSLeft(this, cloner);
    }
    #endregion



    public BinPackerMinRSLeft() : base() { }

    /// <summary>
    /// This proportion of the residual space left to the item height is used for deciding if a not stackable item should be placed.
    /// </summary>
    private const double NOT_STACKABLE_RS_LEFT_TO_ITEM_HEIGHT_PROPORTION = 1.1;

    public override IList<BinPacking3D> PackItems(Permutation sortedItems, PackingShape binShape, IList<PackingItem> items, ExtremePointCreationMethod epCreationMethod, ExtremePointPruningMethod epPruningMethod, bool useStackingConstraints) {
      var workingItems = CloneItems(items);
      IList<BinPacking3D> packingList = new List<BinPacking3D>();
      IList<int> remainingIds = new List<int>(sortedItems);

      try {
        while (remainingIds.Count > 0) {
          BinPacking3D packingBin = new BinPacking3D(binShape);
          PackRemainingItems(ref remainingIds, ref packingBin, workingItems, epCreationMethod, useStackingConstraints);
          packingList.Add(packingBin);
        }
      } catch (BinPacking3DException e) {
      }

      ExtremePointPruningFactory.CreatePruning(epPruningMethod).PruneExtremePoints(packingList);

      return packingList;
    }


    public override void PackItemsToPackingList(IList<BinPacking3D> packingList, Permutation sortedItems, PackingShape binShape, IList<PackingItem> items, ExtremePointCreationMethod epCreationMethod, ExtremePointPruningMethod epPruningMethod, bool useStackingConstraints) {
      var workingItems = CloneItems(items);
      IList<int> remainingIds = new List<int>(sortedItems);

      try {
        if (packingList.Count > 0) {
          BinPacking3D packingBin = packingList.Last();
          PackRemainingItems(ref remainingIds, ref packingBin, workingItems, epCreationMethod, useStackingConstraints);
        }

        while (remainingIds.Count > 0) {
          BinPacking3D packingBin = new BinPacking3D(binShape);
          PackRemainingItems(ref remainingIds, ref packingBin, workingItems, epCreationMethod, useStackingConstraints);
          packingList.Add(packingBin);
        }
      } catch (BinPacking3DException e) {
      }

      ExtremePointPruningFactory.CreatePruning(epPruningMethod).PruneExtremePoints(packingList);
    }



    /// <summary>
    /// Tries to pack the remainig items into a given BinPacking3D object. Each item could be packed into the BinPacking3D object will be removed from the list of remaining ids
    /// </summary>
    /// <param name="remainingIds">List of remaining ids. After the method has been executed the list has to have less items</param>
    /// <param name="packingBin">This object will be filled with some of the given items</param>
    /// <param name="items">List of packing items. Some of the items will be assigned to the BinPacking3D object</param>
    /// <param name="epCreationMethod"></param>
    /// <param name="useStackingConstraints"></param>
    protected virtual void PackRemainingItems(ref IList<int> remainingIds, ref BinPacking3D packingBin, IList<PackingItem> items, ExtremePointCreationMethod epCreationMethod, bool useStackingConstraints) {
      IExtremePointCreator extremePointCreator = ExtremePointCreatorFactory.CreateExtremePointCreator(epCreationMethod, useStackingConstraints);

      var remainingNotWeightSupportedItems = new List<int>();
      foreach (var itemId in new List<int>(remainingIds)) {
        var item = items[itemId];
        var clonedPackingBin = packingBin.Clone() as BinPacking3D;
        ExtremePointPruningFactory.CreatePruning(ExtremePointPruningMethod.PruneBehind).PruneExtremePoints(clonedPackingBin, item.SequenceGroup - 1);

        // If an item doesn't support any weight it should have a minimum waste of the residual space.
        // As long as there are weight supporting items left, put the non supporting items into a collection 
        // and try to find positions where they don't waste too much space.
        // If there are no weight supporting items left the non supporting ones have to be treated as a supporting one.
        if (item.IsStackabel && item.SupportedWeight <= 0 && useStackingConstraints && remainingIds.Any(x => items[x].SupportedWeight > 0)) {
          remainingNotWeightSupportedItems.Add(itemId);
        } else if (!item.IsStackabel) {
          PackingPosition position = FindPackingPositionForNotStackableItem(clonedPackingBin, item, useStackingConstraints);
          // if a valid packing position could be found, the current item can be added to the given bin
          if (position != null) {
            PackItem(packingBin, itemId, item, position, extremePointCreator, useStackingConstraints);
            remainingIds.Remove(itemId);
          }
        } else {
          PackingPosition position = FindPackingPositionForItem(clonedPackingBin, item, useStackingConstraints);
          // if a valid packing position could be found, the current item can be added to the given bin
          if (position != null) {
            PackItem(packingBin, itemId, item, position, extremePointCreator, useStackingConstraints);
            remainingIds.Remove(itemId);
          }
        }

        // try to find a valid position for a non weight supporting item
        var weightSupportedLeft = remainingIds.Any(x => items[x].SupportedWeight > 0);
        foreach (var saId in new List<int>(remainingNotWeightSupportedItems)) {
          item = items[saId];
          PackingPosition position = null;
          if (weightSupportedLeft) {
            position = FindPackingPositionForWeightUnsupportedItem(clonedPackingBin, item, useStackingConstraints);
          } else {
            position = FindPackingPositionForItem(clonedPackingBin, item, useStackingConstraints);
          }

          if (position != null) {
            PackItem(packingBin, saId, item, position, extremePointCreator, useStackingConstraints);
            remainingIds.Remove(saId);
            remainingNotWeightSupportedItems.Remove(saId);
          }
        }
      }
    }

    /// <summary>
    /// Finds a valid packing position for a not stackable item. 
    /// Not stackable means that an item can't be placed on another one and no other item can be placed on it.
    /// The item will be rotated and tilted so it needs the minimum area.
    /// </summary>
    /// <param name="packingBin"></param>
    /// <param name="packingItem"></param>
    /// <param name="useStackingConstraints"></param>
    /// <returns></returns>
    private PackingPosition FindPackingPositionForNotStackableItem(BinPacking3D packingBin, PackingItem packingItem, bool useStackingConstraints) {
      if (!useStackingConstraints) {
        return FindPackingPositionForItem(packingBin, packingItem, useStackingConstraints);
      }
      var areas = new List<Tuple<double, bool, bool>>();
      areas.Add(CalculateArea(packingItem, false, false));
      if (packingItem.TiltEnabled) {
        areas.Add(CalculateArea(packingItem, false, true));
      }
      if (packingItem.RotateEnabled) {
        areas.Add(CalculateArea(packingItem, true, false));
      }
      if (packingItem.RotateEnabled && packingItem.TiltEnabled) {
        areas.Add(CalculateArea(packingItem, true, true));
      }

      areas.Sort((x1, x2) => (int)(x1.Item1 - x2.Item1));
      var orientation = areas.FirstOrDefault();
      if (orientation == null) {
        return null;
      }


      PackingItem newItem = new PackingItem(packingItem) {
        Rotated = orientation.Item2,
        Tilted = orientation.Item3
      };

      var rsds = new SortedSet<ResidualSpaceDifference>();
      foreach (var ep in packingBin.ExtremePoints.Where(x => x.Key.Y == 0)) {
        var position = ep.Key;
        foreach (var rs in ep.Value) {
          var rsd = ResidualSpaceDifference.Create(position, newItem, rs);
          if (rsd != null) {
            rsds.Add(rsd);
          }
        }
      }
      var d = rsds.FirstOrDefault(x => packingBin.IsPositionFeasible(x.Item, x.Position, useStackingConstraints));

      if (d == null) {
        return null;
      }
      
      packingItem.Rotated = orientation.Item2;
      packingItem.Tilted = orientation.Item3;
      return d.Position;
    }

    Tuple<double, bool, bool> CalculateArea(PackingItem packingItem, bool rotated, bool tilted) {
      var item = new PackingItem(packingItem) {
        Rotated = rotated,
        Tilted = tilted
      };
      return Tuple.Create<double, bool, bool>(item.Width * item.Depth, rotated, tilted);
    }

    /// <summary>
    /// Tries to find a valid position for a non stackable item.
    /// Positions will only be valid if the height difference of its residual space is smaller then the hight of the item.
    /// </summary>
    /// <param name="packingBin"></param>
    /// <param name="packingItem"></param>
    /// <param name="useStackingConstraints"></param>
    /// <returns></returns>
    private PackingPosition FindPackingPositionForWeightUnsupportedItem(BinPacking3D packingBin, PackingItem packingItem, bool useStackingConstraints) {
      if (!CheckItemDimensions(packingBin, packingItem)) {
        throw new BinPacking3DException($"The dimensions of the given item exceeds the bin dimensions. " +
          $"Bin: ({packingBin.BinShape.Width} {packingBin.BinShape.Depth} {packingBin.BinShape.Height})" +
          $"Item: ({packingItem.Width} {packingItem.Depth} {packingItem.Height})");
      }
      var rsds = CalculateResidalSpaceDifferences(packingBin, packingItem, useStackingConstraints).ToList();
      var rsd = rsds.Where(x => x != null && (x.Y / (double)x.Item.Height) < NOT_STACKABLE_RS_LEFT_TO_ITEM_HEIGHT_PROPORTION).OrderByDescending(x => x.Y % x.Item.Height).FirstOrDefault();

      if (rsd == null) {
        return null;
      }

      packingItem.Rotated = rsd.Item.Rotated;
      packingItem.Tilted = rsd.Item.Tilted;

      return rsd.Position;
    }

    protected override PackingPosition FindPackingPositionForItem(BinPacking3D packingBin, PackingItem packingItem, bool useStackingConstraints) {
      if (!CheckItemDimensions(packingBin, packingItem)) {
        throw new BinPacking3DException($"The dimensions of the given item exceeds the bin dimensions. " +
          $"Bin: ({packingBin.BinShape.Width} {packingBin.BinShape.Depth} {packingBin.BinShape.Height})" +
          $"Item: ({packingItem.Width} {packingItem.Depth} {packingItem.Height})");
      }

      var rsds = CalculateResidalSpaceDifferences(packingBin, packingItem, useStackingConstraints).Where(x => x != null);
      var rsd = rsds.FirstOrDefault();


      if (rsd == null) {
        return null;
      }

      packingItem.Rotated = rsd.Item.Rotated;
      packingItem.Tilted = rsd.Item.Tilted;

      return rsd.Position;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="packingBin"></param>
    /// <param name="packingItem"></param>
    /// <param name="useStackingConstraints"></param>
    /// <returns></returns>
    private SortedSet<ResidualSpaceDifference> CalculateResidalSpaceDifferences(BinPacking3D packingBin, PackingItem packingItem, bool useStackingConstraints) {
      var rsds = new SortedSet<ResidualSpaceDifference>();

      rsds.Add(FindResidualSpaceDifferenceForItem(packingBin, packingItem, useStackingConstraints, rotated: false, tilted: false));

      if (packingItem.TiltEnabled) {
        rsds.Add(FindResidualSpaceDifferenceForItem(packingBin, packingItem, useStackingConstraints, rotated: false, tilted: true));
      }
      if (packingItem.RotateEnabled) {
        rsds.Add(FindResidualSpaceDifferenceForItem(packingBin, packingItem, useStackingConstraints, rotated: true, tilted: false));
      }
      if (packingItem.RotateEnabled && packingItem.TiltEnabled) {
        rsds.Add(FindResidualSpaceDifferenceForItem(packingBin, packingItem, useStackingConstraints, rotated: true, tilted: true));
      }
      return rsds;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="packingBin"></param>
    /// <param name="packingItem"></param>
    /// <param name="useStackingConstraints"></param>
    /// <param name="rotated"></param>
    /// <param name="tilted"></param>
    /// <returns></returns>
    protected ResidualSpaceDifference FindResidualSpaceDifferenceForItem(BinPacking3D packingBin, PackingItem packingItem, bool useStackingConstraints, bool rotated, bool tilted) {
      PackingItem newItem = new PackingItem(packingItem) {
        Rotated = rotated,
        Tilted = tilted
      };

      var rsds = new SortedSet<ResidualSpaceDifference>();
      foreach (var ep in packingBin.ExtremePoints) {
        var position = ep.Key;
        foreach (var rs in ep.Value) {
          var rsd = ResidualSpaceDifference.Create(position, newItem, rs);
          if (rsd != null) {
            rsds.Add(rsd);
          }
        }
      }

      // If the packer uses line projection it migth happen, that an extreme point right of the disired on will be chosen.
      // To avoid this, if there is an extreme point on the rigth side available this point will be returned.
      var possible = rsds.Where(rsd => packingBin.IsPositionFeasible(rsd.Item, rsd.Position, useStackingConstraints));
      var prevered = possible.FirstOrDefault();

      return possible.Where(x => x.Position.Z == prevered.Position.Z && x.Position.Y == prevered.Position.Y).OrderBy(x => x.Position.X).FirstOrDefault();
    }

    protected override bool CheckItemDimensions(BinPacking3D packingBin, PackingItem item) {
      bool ok = false;
      int width = item.OriginalWidth;
      int height = item.OriginalHeight;
      int depth = item.OriginalDepth;

      ok |= CheckItemDimensions(packingBin, width, height, depth);

      if (item.RotateEnabled && item.TiltEnabled) {
        ok |= CheckItemDimensions(packingBin, depth, height, width);//rotated
        ok |= CheckItemDimensions(packingBin, height, width, depth);//tilted
        ok |= CheckItemDimensions(packingBin, depth, width, height);//rotated & tilted
      } else if (item.RotateEnabled) {
        ok |= CheckItemDimensions(packingBin, depth, height, width);
      } else if (item.TiltEnabled) {
        ok |= CheckItemDimensions(packingBin, height, width, depth);
      }

      return ok;
    }

    private bool CheckItemDimensions(BinPacking3D packingBin, int width, int height, int depth) {
      return base.CheckItemDimensions(packingBin, new PackingItem() {
        OriginalWidth = width,
        OriginalHeight = height,
        OriginalDepth = depth
      });
    }


    protected class ResidualSpaceDifference : IComparable {
      public static ResidualSpaceDifference Create(PackingPosition position, PackingItem item, ResidualSpace rs) {
        var x = rs.Width - item.Width;
        var y = rs.Height - item.Height;
        var z = rs.Depth - item.Depth;
        // the item can't be places in the residual space
        if (rs.IsZero() || x < 0 || y < 0 || z < 0) {
          return null;
        }

        return new ResidualSpaceDifference() {
          Position = position,
          Item = item,
          X = x,
          Y = y,
          Z = z
        };
      }

      public ResidualSpaceDifference() { }

      public PackingItem Item { get; set; }

      public PackingPosition Position { get; set; }
      public int X { get; set; }
      public int Y { get; set; }
      public int Z { get; set; }


      public int CompareTo(object obj) {
        if (!(obj is ResidualSpaceDifference)) {
          return 0;
        }
        var rsd = obj as ResidualSpaceDifference;

        var x = this.X - rsd.X;
        var y = this.Y - rsd.Y;
        var z = this.Z - rsd.Z;

        if (x != 0) {
          return x;
        } else if (y != 0) {
          return y;
        } else if (z != 0) {
          return z;
        }

        return 0;
      }

      public override string ToString() {
        return string.Format("({1},{2},{3})", X, Y, Z);
      }
    }

  }
}
