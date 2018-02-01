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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Core;
using HeuristicLab.Common;
using HeuristicLab.Problems.BinPacking;
using HeuristicLab.Problems.BinPacking3D.Geometry;
using HeuristicLab.Collections;

namespace HeuristicLab.Problems.BinPacking3D {
  [Item("BinPacking3D", "Represents a single-bin packing for a 3D bin-packing problem.")]
  [StorableClass]
  public class BinPacking3D : BinPacking<PackingPosition, PackingShape, PackingItem> {

    [Storable]
    public IDictionary<PackingPosition, IEnumerable<ResidualSpace>> ExtremePoints { get; protected set; }
    

    public BinPacking3D(PackingShape binShape)
      : base(binShape) {
      ExtremePoints = new SortedList<PackingPosition, IEnumerable<ResidualSpace>>();

      ExtremePoints.Add(binShape.Origin, new List<ResidualSpace>() { ResidualSpace.Create(binShape.Width, binShape.Height, binShape.Depth) });
    }

    [StorableConstructor]
    protected BinPacking3D(bool deserializing) : base(deserializing) { }

    protected BinPacking3D(BinPacking3D original, Cloner cloner)
      : base(original, cloner) {
      this.ExtremePoints = new SortedList<PackingPosition, IEnumerable<ResidualSpace>>();
      foreach (var extremePoint in original.ExtremePoints) {
        var residualSpaces = new List<ResidualSpace>();
        foreach (var residualSpace in extremePoint.Value) {
          residualSpaces.Add(cloner.Clone(residualSpace));
        }
        ExtremePoints.Add(cloner.Clone(extremePoint.Key), residualSpaces);
      }      
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BinPacking3D(this, cloner);
    }



    /// <summary>
    /// Puts a given item into the bin packing at the given position.
    /// </summary>
    /// <param name="itemID">Offset in the internal item array</param>
    /// <param name="item">Item</param>
    /// <param name="position">Position of the item in the bin packing</param>
    public override void PackItem(int itemID, PackingItem item, PackingPosition position) {
      Items[itemID] = item;
      Positions[itemID] = position;
      ExtremePoints.Remove(position);
    }

    #region MassPoint

    /// <summary>
    /// This property contains the value of the mass point for the current bin packing.
    /// </summary>
    public Tuple<double, double, double> MassPoint { get { return CalculateMassPoint(); } }

    private Tuple<double, double, double> CalculateMassPoint() {
      var packingMassPoint = new { X = 0.0, Y = 0.0, Z = 0.0 };
      var totalWeight = 0.0;
      foreach (var item in Items) {
        var position = Positions[item.Key];
        var w = item.Value.Width;
        var h = item.Value.Height;
        var d = item.Value.Depth;

        var massPoint = new { X = position.X + w / 2.0, Y = position.Y + h / 2.0, Z = position.Z + d / 2.0 };
        var weight = Math.Max(item.Value.Weight, 1);
        if (packingMassPoint == null) {
          packingMassPoint = massPoint;
          totalWeight += weight;
        } else {
          var newTotalWeight = totalWeight + weight;
          packingMassPoint = new {
            X = (massPoint.X * weight + packingMassPoint.X * totalWeight) / newTotalWeight,
            Y = (massPoint.Y * weight + packingMassPoint.Y * totalWeight) / newTotalWeight,
            Z = (massPoint.Z * weight + packingMassPoint.Z * totalWeight) / newTotalWeight
          };
          totalWeight = newTotalWeight;
        }
      }

      return Tuple.Create<double, double, double>(packingMassPoint.X, packingMassPoint.Y, packingMassPoint.Z);
    }
    #endregion

    #region Position feasability

    /// <summary>
    /// In this case feasability is defined as following: 
    /// 1. the point is supported by something; 
    /// 2. the item does not collide with another already packed item
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position"></param>
    /// <param name="stackingConstraints"></param>
    /// <returns></returns>
    public override bool IsPositionFeasible(PackingItem item, PackingPosition position, bool stackingConstraints) {
      return ItemCanBePlaced(item, position) && CheckStackingConstraints(item, position, stackingConstraints);
    }



    /// <summary>
    /// Returns true if a given item can be placed in the current bin
    /// </summary>
    /// <param name="givenItem"></param>
    /// <param name="givenItemPosition"></param>
    /// <returns></returns>
    private bool ItemCanBePlaced(PackingItem givenItem, PackingPosition givenItemPosition) {
      // Check if the boundings of the bin would injured
      if (givenItemPosition.X + givenItem.Width > BinShape.Width ||
          givenItemPosition.Y + givenItem.Height > BinShape.Height ||
          givenItemPosition.Z + givenItem.Depth > BinShape.Depth) {
        return false;
      }

      //if the given item collides with any other item, it can not be placed
      foreach (var item in Items) {
        if (ItemsCollides(new Tuple<PackingPosition, PackingItem>(Positions[item.Key], item.Value),
                          new Tuple<PackingPosition, PackingItem>(givenItemPosition, givenItem))) {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Checks if two given items in a space collides
    /// </summary>
    /// <param name="t1"></param>
    /// <param name="t2"></param>
    /// <returns></returns>
    private bool ItemsCollides(Tuple<PackingPosition, PackingItem> t1, Tuple<PackingPosition, PackingItem> t2) {
      var position1 = t1.Item1;
      var item1 = t1.Item2;
      var position2 = t2.Item1;
      var item2 = t2.Item2;
      var cx = (position2.X == position1.X) || (position2.X < position1.X && position2.X + item2.Width > position1.X) || (position2.X > position1.X && position1.X + item1.Width > position2.X);
      var cy = (position2.Y == position1.Y) || (position2.Y < position1.Y && position2.Y + item2.Height > position1.Y) || (position2.Y > position1.Y && position1.Y + item1.Height > position2.Y);
      var cz = (position2.Z == position1.Z) || (position2.Z < position1.Z && position2.Z + item2.Depth > position1.Z) || (position2.Z > position1.Z && position1.Z + item1.Depth > position2.Z);
      return cx && cy && cz;
    }

    /// <summary>
    /// Checks the stacking constraints. This method depends that the given item can be placed at this position
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position"></param>
    /// <param name="stackingConstraints"></param>
    /// <returns></returns>
    private bool CheckStackingConstraints(PackingItem item, PackingPosition position, bool stackingConstraints) {
      if (position.Y == 0 || !stackingConstraints && HasOnePointWithAnItemBelow(item, position)) {
        return true;
      }

      return IsStaticStable(item, position) && IsWeightSupported(item, position);
    }

    /// <summary>
    /// Checks if a given item has any point lieing on another item.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    private bool HasOnePointWithAnItemBelow(PackingItem item, PackingPosition position) {
      bool p1, p2, p3, p4;
      PointsLiesOnAnStackableItem(item, position, out p1, out p2, out p3, out p4);

      return p1 || p2 || p3 || p4;
    }

    /// <summary>
    /// Checks if a given item is static stable.
    /// A item is static stable if all edges have an object below.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public override bool IsStaticStable(PackingItem item, PackingPosition position) {
      bool p1, p2, p3, p4;
      PointsLiesOnAnStackableItem(item, position, out p1, out p2, out p3, out p4);
      return p1 && p2 && p3 && p4;
    }

    /// <summary>
    /// This method sets the out parameters p1 ... p4 if the point lies on another item.
    /// p1 ... p3 represents one point on the bottom side of an given item.
    /// +---------+
    /// |p1     p2|
    /// |         |
    /// |p4     p3|
    /// +---------+
    /// </summary>
    /// <param name="item">Given item</param>
    /// <param name="position">Given item position</param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <param name="p4"></param>
    private void PointsLiesOnAnStackableItem(PackingItem item, PackingPosition position, out bool p1, out bool p2, out bool p3, out bool p4) {
      IEnumerable<Tuple<PackingPosition, PackingItem>> itemsP1;
      IEnumerable<Tuple<PackingPosition, PackingItem>> itemsP2;
      IEnumerable<Tuple<PackingPosition, PackingItem>> itemsP3;
      IEnumerable<Tuple<PackingPosition, PackingItem>> itemsP4;

      GetItemsUnderItemWithContact(item, position, out itemsP1, out itemsP2, out itemsP3, out itemsP4);

      p1 = itemsP1.Where(x => x.Item2.IsStackabel && position.X < x.Item1.X + x.Item2.Width && position.Z < x.Item1.Z + x.Item2.Depth).Any();
      p2 = itemsP2.Where(x => x.Item2.IsStackabel && position.X + item.Width > x.Item1.X && position.Z < x.Item1.Z + x.Item2.Depth).Any();
      p3 = itemsP3.Where(x => x.Item2.IsStackabel && position.X + item.Width > x.Item1.X && position.Z + item.Depth > x.Item1.Z).Any();
      p4 = itemsP4.Where(x => x.Item2.IsStackabel && position.X < x.Item1.X + x.Item2.Width && position.Z + item.Depth > x.Item1.Z).Any();
    }

    /// <summary>
    /// This method returns a collection for the out parameters itemsP1 ... itemsP4 with the items below
    /// itemsP1 ... itemsP4 represents one point on the bottom side of an given item.
    /// +---------+
    /// |p1     p2|
    /// |         |
    /// |p4     p3|
    /// +---------+
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position"></param>
    /// <param name="itemsP1"></param>
    /// <param name="itemsP2"></param>
    /// <param name="itemsP3"></param>
    /// <param name="itemsP4"></param>
    private void GetItemsUnderItemWithContact(PackingItem item, PackingPosition position,
                                   out IEnumerable<Tuple<PackingPosition, PackingItem>> itemsP1,
                                   out IEnumerable<Tuple<PackingPosition, PackingItem>> itemsP2,
                                   out IEnumerable<Tuple<PackingPosition, PackingItem>> itemsP3,
                                   out IEnumerable<Tuple<PackingPosition, PackingItem>> itemsP4) {
      itemsP1 = GetItemsBelowForPosition(new PackingPosition(0, position.X, position.Y, position.Z, false));
      itemsP2 = GetItemsBelowForPosition(new PackingPosition(0, position.X + item.Width - 1, position.Y, position.Z, false));
      itemsP3 = GetItemsBelowForPosition(new PackingPosition(0, position.X, position.Y, position.Z + item.Depth - 1, false));
      itemsP4 = GetItemsBelowForPosition(new PackingPosition(0, position.X + item.Width - 1, position.Y, position.Z + item.Depth - 1, false));

    }

    /// <summary>
    /// Returns a collection of items which are below a given point. 
    /// The top side of every item is at the same level as the Y-coordinate of the given position.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private IEnumerable<Tuple<PackingPosition, PackingItem>> GetItemsBelowForPosition(PackingPosition position) {
      return GetItemsBelow(position).Where(x => (x.Item1.Y + x.Item2.Height) == position.Y);
    }

    #region Weight supported
    //old implementation
    /// <summary>
    /// Checks if a given the weight of an given item is supported by the items below.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    private bool IsWeightSupported(PackingItem item, PackingPosition position) {
      if (position.Y == 0) {
        return true;
      }
      IEnumerable<Tuple<PackingPosition, PackingItem>> itemsP1;
      IEnumerable<Tuple<PackingPosition, PackingItem>> itemsP2;
      IEnumerable<Tuple<PackingPosition, PackingItem>> itemsP3;
      IEnumerable<Tuple<PackingPosition, PackingItem>> itemsP4;

      GetItemsUnderItemWithContact(item, position, out itemsP1, out itemsP2, out itemsP3, out itemsP4);

      return itemsP1.Where(x => x.Item2.SupportsStacking(item)).Any() &&
        itemsP2.Where(x => x.Item2.SupportsStacking(item)).Any() &&
        itemsP3.Where(x => x.Item2.SupportsStacking(item)).Any() &&
        itemsP4.Where(x => x.Item2.SupportsStacking(item)).Any();
    }

    private double CalculateOverlay(Tuple<PackingPosition, PackingItem> item1, Tuple<PackingPosition, PackingItem> item2) {
      var left = item1.Item1.X <= item2.Item1.X ? item1 : item2;
      var right = item1.Item1.X <= item2.Item1.X ? item2 : item1;
      var behind = item1.Item1.Z <= item2.Item1.Z ? item1 : item2;
      var inFront = item1.Item1.Z <= item2.Item1.Z ? item2 : item1;

      var overlayX = right.Item2.Width;
      if (left.Item1.X + left.Item2.Width < right.Item1.X + right.Item2.Width) {
        overlayX = left.Item1.X + left.Item2.Width - right.Item1.X;
      }

      var overlayZ = inFront.Item2.Width;
      if (behind.Item1.X + behind.Item2.Width < inFront.Item1.X + inFront.Item2.Width) {
        overlayZ = behind.Item1.X + behind.Item2.Width - inFront.Item1.X;
      }

      return overlayX * overlayZ;
    }

    private IEnumerable<Tuple<PackingPosition, PackingItem>> GetItemsAboveAnItem(PackingPosition position, PackingItem item) {
      var selected = Items.Select(x => new {
        Item = x.Value,
        Position = Positions[x.Key]
      }).Where(x => x.Position.Y == position.Y + item.Height)
        .Where(x => x.Position.X <= position.X && x.Position.X + x.Item.Width > position.X ||
                    x.Position.X > position.X && x.Position.X < position.X + item.Width)
        .Where(x => x.Position.Z <= position.Z && x.Position.Z + x.Item.Height > position.Z ||
                    x.Position.Z > position.Z && x.Position.Z < position.Z + item.Height);

      return selected.Select(x => Tuple.Create(x.Position, x.Item));
    }

    private IEnumerable<Tuple<PackingPosition, PackingItem>> GetItemsUnderAnItem(PackingPosition position, PackingItem item) {
      var selected = Items.Select(x => new {
        Item = x.Value,
        Position = Positions[x.Key]
      }).Where(x => x.Position.Y + x.Item.Height == position.Y)
        .Where(x => x.Position.X <= position.X && x.Position.X + x.Item.Width > position.X ||
                    x.Position.X > position.X && x.Position.X < position.X + item.Width)
        .Where(x => x.Position.Z <= position.Z && x.Position.Z + x.Item.Height > position.Z ||
                    x.Position.Z > position.Z && x.Position.Z < position.Z + item.Height);

      return selected.Select(x => Tuple.Create(x.Position, x.Item));
    }

    #endregion


    #endregion

    protected override void GenerateNewExtremePointsForNewItem(PackingItem newItem, PackingPosition position) {
      throw new NotImplementedException();
    }

    #region Get items

    private IEnumerable<Tuple<PackingPosition, PackingItem>> GetItemsBelow(PackingPosition pos) {
      var line = new Line3D(pos, new Vector3D(0, 1, 0));
      return Items.Select(x => new {
        Item = x.Value,
        Position = Positions[x.Key],
        Intersection = line.Intersect(new Plane3D(Positions[x.Key], x.Value, Side.Top))
      }).Where(x => x.Intersection != null && x.Intersection.Y <= pos.Y)
        .Select(x => Tuple.Create(x.Position, x.Item));
    }

    #endregion
  }
}