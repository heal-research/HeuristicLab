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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Core;
using HeuristicLab.Common;
using HeuristicLab.Problems.BinPacking;
using HeuristicLab.Problems.BinPacking3D.Geometry;

namespace HeuristicLab.Problems.BinPacking3D {
  [Item("BinPacking3D", "Represents a single-bin packing for a 3D bin-packing problem.")]
  [StorableClass]
  public class BinPacking3D : BinPacking<PackingPosition, PackingShape, PackingItem> {

    [Storable]
    public Dictionary<PackingPosition, Tuple<int, int, int>> ResidualSpace { get; protected set; }

    public BinPacking3D(PackingShape binShape)
      : base(binShape) {
      ResidualSpace = new Dictionary<PackingPosition, Tuple<int, int, int>>();
      AddExtremePoint(binShape.Origin);
      InitializeOccupationLayers();
    }
    [StorableConstructor]
    protected BinPacking3D(bool deserializing) : base(deserializing) { }
    protected BinPacking3D(BinPacking3D original, Cloner cloner)
      : base(original, cloner) {
      this.ResidualSpace = new Dictionary<PackingPosition, Tuple<int, int, int>>();
      foreach (var o in original.ResidualSpace)
        this.ResidualSpace.Add(cloner.Clone(o.Key), Tuple.Create(o.Value.Item1, o.Value.Item2, o.Value.Item3));
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new BinPacking3D(this, cloner);
    }


    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (ResidualSpace == null)
        ResidualSpace = new Dictionary<PackingPosition, Tuple<int, int, int>>();
      #endregion
    }

    #region New methods for bin packer class

    public void AddItem(int itemID, PackingItem item, PackingPosition position) {
      Items[itemID] = item;
      Positions[itemID] = position;
      ExtremePoints.Remove(position);
      ResidualSpace.Remove(position);
    }

    public void UpdateExtremePoints(PackingItem item, PackingPosition position, bool stackingConstraints) {
      /*if (stackingConstraints) {
        AddExtremePoint(new PackingPosition(position.AssignedBin, position.X + item.Width, position.Y, position.Z));
        AddExtremePoint(new PackingPosition(position.AssignedBin, position.X, position.Y + item.Height, position.Z));
        AddExtremePoint(new PackingPosition(position.AssignedBin, position.X, position.Y, position.Z + item.Depth));
      } else {*/
      GenerateNewExtremePointsForNewItem(item, position);

      // if an item is fit in below, before, or left of another its extreme points have to be regenerated
      foreach (var above in GetItemsAbove(position)) {
        GenerateNewExtremePointsForNewItem(above.Item2, above.Item1);
      }
      foreach (var front in GetItemsInfront(position)) {
        GenerateNewExtremePointsForNewItem(front.Item2, front.Item1);
      }
      foreach (var right in GetItemsOnRight(position)) {
        GenerateNewExtremePointsForNewItem(right.Item2, right.Item1);
      }
      //}
    }



    /// <summary>
    /// In this case feasability is defined as following: 
    /// 1. the item fits into the bin-borders; 
    /// 2. the point is supported by something; 
    /// 3. the item does not collide with another already packed item
    /// 
    /// Unfortunatelly this method does not support item rotation
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position"></param>
    /// <param name="stackingConstraints"></param>
    /// <returns></returns>
    public override bool IsPositionFeasible(PackingItem item, PackingPosition position, bool stackingConstraints) {
      var b1 = CheckItemDimensions(item);
      var b2 = ItemCanBePlaced(item, position);
      var b3 = CheckStackingConstraints(item, position, stackingConstraints);

      return b1 && b2 && b3;
    }

    /// <summary>
    /// Compares the dimensions of a given item and the current bin. 
    /// </summary>
    /// <param name="item"></param>
    /// <returns>Returns true if the dimensions of an given item are less or equal to the bin.</returns>
    private bool CheckItemDimensions(PackingItem item) {
      return BinShape.Width >= item.Width && BinShape.Height >= item.Height && BinShape.Depth >= item.Depth;
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
    bool ItemsCollides(Tuple<PackingPosition, PackingItem> t1, Tuple<PackingPosition, PackingItem> t2) {
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
    public bool HasOnePointWithAnItemBelow(PackingItem item, PackingPosition position) {
      bool p1, p2, p3, p4;
      PointsLiesOnAnItems(item, position, out p1, out p2, out p3, out p4);

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
      PointsLiesOnAnItems(item, position, out p1, out p2, out p3, out p4);
      return p1 && p2 && p3 && p4;
    }

    /// <summary>
    /// This method sets the out parameters p1 ... p3 if the point lies on another item.
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
    public void PointsLiesOnAnItems(PackingItem item, PackingPosition position, out bool p1, out bool p2, out bool p3, out bool p4) {
      IEnumerable<Tuple<PackingPosition, PackingItem>> itemsP1;
      IEnumerable<Tuple<PackingPosition, PackingItem>> itemsP2;
      IEnumerable<Tuple<PackingPosition, PackingItem>> itemsP3;
      IEnumerable<Tuple<PackingPosition, PackingItem>> itemsP4;

      GetItemsUnderItemWithContact(item, position, out itemsP1, out itemsP2, out itemsP3, out itemsP4);

      p1 = itemsP1.Where(x => position.X < x.Item1.X + x.Item2.Width && position.Z < x.Item1.Z + x.Item2.Depth).Any();
      p2 = itemsP2.Where(x => position.X + item.Width > x.Item1.X && position.Z < x.Item1.Z + x.Item2.Depth).Any();
      p3 = itemsP3.Where(x => position.X + item.Width > x.Item1.X && position.Z + item.Depth > x.Item1.Z).Any();
      p4 = itemsP4.Where(x => position.X < x.Item1.X + x.Item2.Width && position.Z + item.Depth > x.Item1.Z).Any();
    }


    /// <summary>
    /// 
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


    /// <summary>
    /// Checks if a given the weight of an given item is supportet by the items below.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool IsWeightSupported(PackingItem item, PackingPosition position) {
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



    #endregion

    public override void PackItem(int itemID, PackingItem item, PackingPosition position) {
      // base call is deliberately omitted, because UpdateResidualSpace needs to be fitted in before
      // generating new extreme points
      Items[itemID] = item;
      Positions[itemID] = position;
      ExtremePoints.Remove(position);
      ResidualSpace.Remove(position);
      UpdateResidualSpace(item, position);
      GenerateNewExtremePointsForNewItem(item, position);
      // if an item is fit in below, before, or left of another its extreme points have to be regenerated
      foreach (var above in GetItemsAbove(position))
        GenerateNewExtremePointsForNewItem(above.Item2, above.Item1);
      foreach (var front in GetItemsInfront(position))
        GenerateNewExtremePointsForNewItem(front.Item2, front.Item1);
      foreach (var right in GetItemsOnRight(position))
        GenerateNewExtremePointsForNewItem(right.Item2, right.Item1);
      AddNewItemToOccupationLayers(itemID, item, position);
    }



    protected override void GenerateNewExtremePointsForNewItem(PackingItem newItem, PackingPosition position) {
      int newWidth = position.Rotated ? newItem.Depth : newItem.Width;
      int newDepth = position.Rotated ? newItem.Width : newItem.Depth;

      var itemPlacement = Items.Select(x => new { Item = x.Value, Position = Positions[x.Key] }).ToList();
      // Find ExtremePoints beginning from sourcepointX
      var sourcePoint = new Vector3D() { X = position.X + newWidth, Y = position.Y, Z = position.Z };
      if (sourcePoint.X < BinShape.Width && sourcePoint.Y < BinShape.Height && sourcePoint.Z < BinShape.Depth) {
        // Projecting onto the XZ-plane
        var below = ProjectDown(sourcePoint);
        var residualSpace = CalculateResidualSpace(below);
        if (IsNonZero(residualSpace)
          && !IsWithinResidualSpaceOfAnotherExtremePoint(below, residualSpace)) {
          // add only if the projected point's residual space is not a sub-space
          // of another extreme point's residual space
          var belowPoint = new PackingPosition(position.AssignedBin, below.X, below.Y, below.Z);
          AddExtremePoint(belowPoint);
        }
        // Projecting onto the XY-plane
        var back = ProjectBackward(sourcePoint);
        residualSpace = CalculateResidualSpace(back);
        if (IsNonZero(residualSpace)
          && !IsWithinResidualSpaceOfAnotherExtremePoint(back, residualSpace)) {
          // add only if the projected point's residual space is not a sub-space
          // of another extreme point's residual space
          var backPoint = new PackingPosition(position.AssignedBin, back.X, back.Y, back.Z);
          AddExtremePoint(backPoint);
        }
      }

      //Find ExtremePoints beginning from sourcepointY
      sourcePoint = new Vector3D(position.X, position.Y + newItem.Height, position.Z);
      if (sourcePoint.X < BinShape.Width && sourcePoint.Y < BinShape.Height && sourcePoint.Z < BinShape.Depth) {
        // Projecting onto the YZ-plane
        var left = ProjectLeft(sourcePoint);
        var residualSpace = CalculateResidualSpace(left);
        if (IsNonZero(residualSpace)
          && !IsWithinResidualSpaceOfAnotherExtremePoint(left, residualSpace)) {
          // add only if the projected point's residual space is not a sub-space
          // of another extreme point's residual space
          var leftPoint = new PackingPosition(position.AssignedBin, left.X, left.Y, left.Z);
          AddExtremePoint(leftPoint);
        }
        // Projecting onto the XY-plane
        var back = ProjectBackward(sourcePoint);
        residualSpace = CalculateResidualSpace(back);
        if (IsNonZero(residualSpace)
          && !IsWithinResidualSpaceOfAnotherExtremePoint(back, residualSpace)) {
          // add only if the projected point's residual space is not a sub-space
          // of another extreme point's residual space
          var backPoint = new PackingPosition(position.AssignedBin, back.X, back.Y, back.Z);
          AddExtremePoint(backPoint);
        }
      }

      //Find ExtremePoints beginning from sourcepointZ
      sourcePoint = new Vector3D(position.X, position.Y, position.Z + newDepth);
      if (sourcePoint.X < BinShape.Width && sourcePoint.Y < BinShape.Height && sourcePoint.Z < BinShape.Depth) {
        // Projecting onto the XZ-plane
        var below = ProjectDown(sourcePoint);
        var residualSpace = CalculateResidualSpace(below);
        if (IsNonZero(residualSpace)
          && !IsWithinResidualSpaceOfAnotherExtremePoint(below, residualSpace)) {
          // add only if the projected point's residual space is not a sub-space
          // of another extreme point's residual space
          var belowPoint = new PackingPosition(position.AssignedBin, below.X, below.Y, below.Z);
          AddExtremePoint(belowPoint);
        }
        // Projecting onto the YZ-plane
        var left = ProjectLeft(sourcePoint);
        residualSpace = CalculateResidualSpace(left);
        if (IsNonZero(residualSpace)
          && !IsWithinResidualSpaceOfAnotherExtremePoint(left, residualSpace)) {
          // add only if the projected point's residual space is not a sub-space
          // of another extreme point's residual space
          var leftPoint = new PackingPosition(position.AssignedBin, left.X, left.Y, left.Z);
          AddExtremePoint(leftPoint);
        }
      }
    }

    private bool IsNonZero(Tuple<int, int, int> rs) {
      return rs.Item1 > 0 && rs.Item2 > 0 && rs.Item3 > 0;
    }


    private bool IsWithinResidualSpaceOfAnotherExtremePoint(Vector3D pos, Tuple<int, int, int> residualSpace) {
      var eps = ExtremePoints.Where(x => !pos.Equals(x) && pos.IsInside(x, ResidualSpace[x]));
      return eps.Any(x => IsWithinResidualSpaceOfAnotherExtremePoint(pos, residualSpace, x, ResidualSpace[x]));
    }
    private bool IsWithinResidualSpaceOfAnotherExtremePoint(Vector3D pos, Tuple<int, int, int> rsPos, PackingPosition ep, Tuple<int, int, int> rsEp) {
      return rsEp.Item1 >= pos.X - ep.X + rsPos.Item1
          && rsEp.Item2 >= pos.Y - ep.Y + rsPos.Item2
          && rsEp.Item3 >= pos.Z - ep.Z + rsPos.Item3;
    }

    private bool AddExtremePoint(PackingPosition pos) {
      if (ExtremePoints.Add(pos)) {
        var rs = CalculateResidualSpace(new Vector3D(pos));
        ResidualSpace.Add(pos, rs);
        // Check if existing extreme points are shadowed by the new point
        // That is, their residual space fit entirely into the residual space of the new point
        foreach (var ep in ExtremePoints.Where(x => x != pos && new Vector3D(x).IsInside(pos, rs)).ToList()) {
          if (IsWithinResidualSpaceOfAnotherExtremePoint(new Vector3D(ep), ResidualSpace[ep], pos, rs)) {
            ExtremePoints.Remove(ep);
            ResidualSpace.Remove(ep);
          }
        }
        return true;
      }
      return false;
    }

    private Tuple<int, int, int> CalculateResidualSpace(Vector3D pos) {
      var itemPos = Items.Select(x => new { Item = x.Value, Position = Positions[x.Key] });
      var rightLimit = ProjectRight(pos);
      var upLimit = ProjectUp(pos);
      var forwardLimit = ProjectForward(pos);
      return Tuple.Create(rightLimit.X - pos.X, upLimit.Y - pos.Y, forwardLimit.Z - pos.Z);
    }

    #region Projections

    private Vector3D ProjectBackward(Vector3D pos) {
      var line = new Line3D(pos, new Vector3D(0, 0, -1));
      return Items.Select(x => new { Item = x.Value, Position = Positions[x.Key] })
                  .Select(x => new Plane3D(x.Position, x.Item, Side.Front))
                  .Concat(new[] { new Plane3D(BinShape, Side.Back) })
                  .Select(x => x.Intersect(line))
                  .Where(x => x != null && x.Z <= pos.Z)
                  .MaxItems(x => x.Z).First();
    }

    private Vector3D ProjectLeft(Vector3D pos) {
      var line = new Line3D(pos, new Vector3D(-1, 0, 0));
      return Items.Select(x => new { Item = x.Value, Position = Positions[x.Key] })
                  .Select(x => new Plane3D(x.Position, x.Item, Side.Right))
                  .Concat(new[] { new Plane3D(BinShape, Side.Left) })
                  .Select(x => x.Intersect(line))
                  .Where(x => x != null && x.X <= pos.X)
                  .MaxItems(x => x.X).First();
    }

    private Vector3D ProjectDown(Vector3D pos) {
      var line = new Line3D(pos, new Vector3D(0, -1, 0));
      return Items.Select(x => new { Item = x.Value, Position = Positions[x.Key] })
                  .Select(x => new Plane3D(x.Position, x.Item, Side.Top))
                  .Concat(new[] { new Plane3D(BinShape, Side.Bottom) })
                  .Select(x => x.Intersect(line))
                  .Where(x => x != null && x.Y <= pos.Y)
                  .MaxItems(x => x.Y).First();
    }

    private Vector3D ProjectForward(Vector3D pos) {
      var line = new Line3D(pos, new Vector3D(0, 0, 1));
      return Items.Select(x => new { Item = x.Value, Position = Positions[x.Key] })
                  .Select(x => new Plane3D(x.Position, x.Item, Side.Back))
                  .Concat(new[] { new Plane3D(BinShape, Side.Front) })
                  .Select(x => x.Intersect(line))
                  .Where(x => x != null && x.Z >= pos.Z)
                  .MinItems(x => x.Z).First();
    }

    private Vector3D ProjectRight(Vector3D pos) {
      var line = new Line3D(pos, new Vector3D(1, 0, 0));
      return Items.Select(x => new { Item = x.Value, Position = Positions[x.Key] })
                  .Select(x => new Plane3D(x.Position, x.Item, Side.Left))
                  .Concat(new[] { new Plane3D(BinShape, Side.Right) })
                  .Select(x => x.Intersect(line))
                  .Where(x => x != null && x.X >= pos.X)
                  .MinItems(x => x.X).First();
    }

    private Vector3D ProjectUp(Vector3D pos) {
      var line = new Line3D(pos, new Vector3D(0, 1, 0));
      return Items.Select(x => new { Item = x.Value, Position = Positions[x.Key] })
                  .Select(x => new Plane3D(x.Position, x.Item, Side.Bottom))
                  .Concat(new[] { new Plane3D(BinShape, Side.Top) })
                  .Select(x => x.Intersect(line))
                  .Where(x => x != null && x.Y >= pos.Y)
                  .MinItems(x => x.Y).First();
    }
    #endregion

    #region Get items

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private IEnumerable<Tuple<PackingPosition, PackingItem>> GetItemsAbove(PackingPosition pos) {
      var line = new Line3D(pos, new Vector3D(0, 1, 0));
      return Items.Select(x => new {
        Item = x.Value,
        Position = Positions[x.Key],
        Intersection = line.Intersect(new Plane3D(Positions[x.Key], x.Value, Side.Bottom))
      }).Where(x => x.Intersection != null && x.Intersection.Y >= pos.Y)
        .Select(x => Tuple.Create(x.Position, x.Item));
    }

    private IEnumerable<Tuple<PackingPosition, PackingItem>> GetItemsInfront(PackingPosition pos) {
      var line = new Line3D(pos, new Vector3D(0, 0, 1));
      return Items.Select(x => new {
        Item = x.Value,
        Position = Positions[x.Key],
        Intersection = line.Intersect(new Plane3D(Positions[x.Key], x.Value, Side.Back))
      }).Where(x => x.Intersection != null && x.Intersection.Y >= pos.Y)
        .Select(x => Tuple.Create(x.Position, x.Item));
    }

    private IEnumerable<Tuple<PackingPosition, PackingItem>> GetItemsOnRight(PackingPosition pos) {
      var line = new Line3D(pos, new Vector3D(1, 0, 0));
      return Items.Select(x => new {
        Item = x.Value,
        Position = Positions[x.Key],
        Intersection = line.Intersect(new Plane3D(Positions[x.Key], x.Value, Side.Left))
      }).Where(x => x.Intersection != null && x.Intersection.Y >= pos.Y)
        .Select(x => Tuple.Create(x.Position, x.Item));
    }

    private IEnumerable<Tuple<PackingPosition, PackingItem>> GetItemsBelow(PackingPosition pos) {
      var line = new Line3D(pos, new Vector3D(0, 1, 0));
      return Items.Select(x => new {
        Item = x.Value,
        Position = Positions[x.Key],
        Intersection = line.Intersect(new Plane3D(Positions[x.Key], x.Value, Side.Top))
      }).Where(x => x.Intersection != null && x.Intersection.Y <= pos.Y)
        .Select(x => Tuple.Create(x.Position, x.Item));
    }

    private IEnumerable<Tuple<PackingPosition, PackingItem>> GetItemsBehind(PackingPosition pos) {
      var line = new Line3D(pos, new Vector3D(0, 0, 1));
      return Items.Select(x => new {
        Item = x.Value,
        Position = Positions[x.Key],
        Intersection = line.Intersect(new Plane3D(Positions[x.Key], x.Value, Side.Front))
      }).Where(x => x.Intersection != null && x.Intersection.Y >= pos.Y)
        .Select(x => Tuple.Create(x.Position, x.Item));
    }

    private IEnumerable<Tuple<PackingPosition, PackingItem>> GetItemsOnLeft(PackingPosition pos) {
      var line = new Line3D(pos, new Vector3D(1, 0, 0));
      return Items.Select(x => new {
        Item = x.Value,
        Position = Positions[x.Key],
        Intersection = line.Intersect(new Plane3D(Positions[x.Key], x.Value, Side.Right))
      }).Where(x => x.Intersection != null && x.Intersection.Y >= pos.Y)
        .Select(x => Tuple.Create(x.Position, x.Item));
    }

    #endregion



    public override PackingPosition FindExtremePointForItem(PackingItem item, bool rotated, bool stackingConstraints) {
      PackingItem newItem = new PackingItem(
        rotated ? item.Depth : item.Width,
        item.Height,
        rotated ? item.Width : item.Depth,
        item.TargetBin, item.Weight, item.Material);

      var ep = ExtremePoints.Where(x => IsPositionFeasible(newItem, x, stackingConstraints))
                            .FirstOrDefault();
      if (ep != null) {
        var result = new PackingPosition(ep.AssignedBin, ep.X, ep.Y, ep.Z, rotated);
        return result;
      }
      return null;
    }

  

    #region Sliding based packing    
    public override PackingPosition FindPositionBySliding(PackingItem item, bool rotated, bool stackingConstraints) {
      //Starting-position at upper right corner (=left bottom point of item-rectangle is at position item.width,item.height)
      PackingPosition currentPosition = new PackingPosition(0,
        BinShape.Width - (rotated ? item.Depth : item.Width),
        BinShape.Height - item.Height,
        BinShape.Depth - (rotated ? item.Width : item.Depth), rotated);
      //Slide the item as far as possible to the bottom
      while (IsPositionFeasible(item, PackingPosition.MoveDown(currentPosition), stackingConstraints)
        || IsPositionFeasible(item, PackingPosition.MoveLeft(currentPosition), stackingConstraints)
        || IsPositionFeasible(item, PackingPosition.MoveBack(currentPosition), stackingConstraints)) {
        //Slide the item as far as possible to the left
        while (IsPositionFeasible(item, PackingPosition.MoveLeft(currentPosition), stackingConstraints)
      || IsPositionFeasible(item, PackingPosition.MoveBack(currentPosition), stackingConstraints)) {
          //Slide the item as far as possible to the back
          while (IsPositionFeasible(item, PackingPosition.MoveBack(currentPosition), stackingConstraints)) {
            currentPosition = PackingPosition.MoveBack(currentPosition);
          }
          if (IsPositionFeasible(item, PackingPosition.MoveLeft(currentPosition), stackingConstraints))
            currentPosition = PackingPosition.MoveLeft(currentPosition);
        }
        if (IsPositionFeasible(item, PackingPosition.MoveDown(currentPosition), stackingConstraints))
          currentPosition = PackingPosition.MoveDown(currentPosition);
      }

      return IsPositionFeasible(item, currentPosition, stackingConstraints) ? currentPosition : null;
    }

    public override void SlidingBasedPacking(ref IList<int> sequence, IList<PackingItem> items, bool stackingConstraints) {
      var temp = new List<int>(sequence);
      for (int i = 0; i < temp.Count; i++) {
        var item = items[temp[i]];
        var position = FindPositionBySliding(item, false, stackingConstraints);
        if (position != null) {
          PackItem(temp[i], item, position);
          sequence.Remove(temp[i]);
        }
      }
    }
    public override void SlidingBasedPacking(ref IList<int> sequence, IList<PackingItem> items, Dictionary<int, bool> rotationArray, bool stackingConstraints) {
      var temp = new List<int>(sequence);
      for (int i = 0; i < temp.Count; i++) {
        var item = items[temp[i]];
        var position = FindPositionBySliding(item, rotationArray[i], stackingConstraints);
        if (position != null) {
          PackItem(temp[i], item, position);
          sequence.Remove(temp[i]);
        }
      }
    }
    #endregion
    public override void ExtremePointBasedPacking(ref IList<int> sequence, IList<PackingItem> items, bool stackingConstraints) {
      var temp = new List<int>(sequence);
      foreach (int itemID in temp) {
        var item = items[itemID];
        var positionFound = FindExtremePointForItem(item, false, stackingConstraints);
        if (positionFound != null) {
          PackItem(itemID, item, positionFound);
          sequence.Remove(itemID);
        }
      }
    }
    public override void ExtremePointBasedPacking(ref IList<int> sequence, IList<PackingItem> items, bool stackingConstraints, Dictionary<int, bool> rotationArray) {
      var temp = new List<int>(sequence);
      foreach (int itemID in temp) {
        var item = items[itemID];
        var positionFound = FindExtremePointForItem(item, rotationArray[itemID], stackingConstraints);
        if (positionFound != null) {
          PackItem(itemID, item, positionFound);
          sequence.Remove(itemID);
        }
      }
    }
    public override int ShortestPossibleSideFromPoint(PackingPosition position) {

      int shortestSide = int.MaxValue;
      int width = BinShape.Width;
      int height = BinShape.Height;
      int depth = BinShape.Depth;

      if (position.X >= width || position.Y >= height || position.Z >= depth)
        return shortestSide;

      PackingPosition current = new PackingPosition(0, position.X, position.Y, position.Z);
      while (current.X < width && IsPointOccupied(current)) { current = PackingPosition.MoveRight(current); }
      if (current.X - position.X < shortestSide)
        shortestSide = current.X - position.X;


      current = new PackingPosition(0, position.X, position.Y, position.Z);
      while (current.Y < height && IsPointOccupied(current)) { current = PackingPosition.MoveUp(current); }
      if (current.Y - position.Y < shortestSide)
        shortestSide = current.Y - position.Y;


      current = new PackingPosition(0, position.X, position.Y, position.Z);
      while (current.Z < depth && IsPointOccupied(current)) { current = PackingPosition.MoveFront(current); }
      if (current.Z - position.Z < shortestSide)
        shortestSide = current.Z - position.Z;

      return shortestSide;
    }
    

    protected override void InitializeOccupationLayers() {
      for (int i = 0; i * 10 <= BinShape.Depth; i += 1) {
        OccupationLayers[i] = new List<int>();
      }
    }
    protected override void AddNewItemToOccupationLayers(int itemID, PackingItem item, PackingPosition position) {
      int z1 = position.Z / 10;
      int z2 = (position.Z + (position.Rotated ? item.Width : item.Depth)) / 10;

      for (int i = z1; i <= z2; i++)
        OccupationLayers[i].Add(itemID);
    }
    protected override List<int> GetLayerItemIDs(PackingPosition position) {
      return OccupationLayers[position.Z / 10];
    }
    protected override List<int> GetLayerItemIDs(PackingItem item, PackingPosition position) {
      List<int> result = new List<int>();
      int z1 = position.Z / 10;
      int z2 = (position.Z + (position.Rotated ? item.Width : item.Depth)) / 10;

      for (int i = z1; i <= z2; i++)
        result.AddRange(OccupationLayers[i]);
      return result;
    }

    public void UpdateResidualSpace(PackingItem item, PackingPosition pos) {
      foreach (var ep in ExtremePoints.ToList()) {
        var rs = ResidualSpace[ep];
        var depth = pos.Rotated ? item.Width : item.Depth;
        var width = pos.Rotated ? item.Depth : item.Width;
        var changed = false;
        if (ep.Z >= pos.Z && ep.Z < pos.Z + depth) {
          if (ep.X <= pos.X && ep.Y >= pos.Y && ep.Y < pos.Y + item.Height) {
            var diff = pos.X - ep.X;
            var newRSX = Math.Min(rs.Item1, diff);
            rs = Tuple.Create(newRSX, rs.Item2, rs.Item3);
            changed = true;
          }
          if (ep.Y <= pos.Y && ep.X >= pos.X && ep.X < pos.X + width) {
            var diff = pos.Y - ep.Y;
            var newRSY = Math.Min(rs.Item2, diff);
            rs = Tuple.Create(rs.Item1, newRSY, rs.Item3);
            changed = true;
          }
        }

        if (ep.Z <= pos.Z &&
            ep.Y >= pos.Y && ep.Y < pos.Y + item.Height &&
            ep.X >= pos.X && ep.X < pos.X + width) {
          var diff = pos.Z - ep.Z;
          var newRSZ = Math.Min(rs.Item3, diff);
          rs = Tuple.Create(rs.Item1, rs.Item2, newRSZ);
          changed = true;
        }

        if (changed) {
          if (IsNonZero(rs) && !IsWithinResidualSpaceOfAnotherExtremePoint(new Vector3D(ep), rs)) {
            ResidualSpace[ep] = rs;
          } else {
            ExtremePoints.Remove(ep);
            ResidualSpace.Remove(ep);
          }
        }
      }
      return;
    }


  }
}