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
      ResidualSpace.Remove(position);
    }

    /// <summary>
    /// Updates the extreme points of the bin
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position"></param>
    /// <param name="stackingConstraints"></param>
    public void UpdateExtremePoints(PackingItem item, PackingPosition position, bool stackingConstraints) {
      if (stackingConstraints) {
        UpdateExtremePointsWithStackingConstriants(item, position);
      } else {
        UpdateExtremePointsWithoutStackingConstriants(item, position);
      }
    }

    /// <summary>
    /// Updates the extreme points of the bin. 
    /// As there is no need to project the extreme points to the next side of an item, the extreme points are only generated for the current item.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position"></param>
    private void UpdateExtremePointsWithStackingConstriants(PackingItem newItem, PackingPosition position) {

      //UpdateExtremePointsWithoutStackingConstriants(newItem, position);
      //return;

      int newWidth = position.Rotated ? newItem.Depth : newItem.Width;
      int newDepth = position.Rotated ? newItem.Width : newItem.Depth;
      var ep1 = new PackingPosition(position.AssignedBin, position.X + newWidth, position.Y, position.Z);
      var ep2 = new PackingPosition(position.AssignedBin, position.X, position.Y + newItem.Height, position.Z);
      var ep3 = new PackingPosition(position.AssignedBin, position.X, position.Y, position.Z + newDepth);
      AddExtremePoint(ep1);
      AddExtremePoint(ep2);
      AddExtremePoint(ep3);
    }
        
    private Tuple<int, int, int> CalculateResidualSpace(Vector3D pos) {
      var itemPos = Items.Select(x => new { Item = x.Value, Position = Positions[x.Key] });
      Vector3D limit = new Vector3D() { X = BinShape.Width, Y = BinShape.Height, Z = BinShape.Depth };
      if (pos.X < limit.X && pos.Y < limit.Y && pos.Z < limit.Z) {
        var rightLimit = ProjectRight(pos);
        var upLimit = ProjectUp(pos);
        var forwardLimit = ProjectForward(pos);
        if (rightLimit.X > 0) {
          limit.X = rightLimit.X;
        }
        if (upLimit.Y > 0) {
          limit.Y = upLimit.Y;
        }
        if (forwardLimit.Z > 0) {
          limit.Z = forwardLimit.Z;
        }
      }
      
      if (limit.X - pos.X <= 0 || limit.Y - pos.Y <= 0  || limit.Z - pos.Z <= 0) {
        return Tuple.Create(0, 0, 0);
      }      
      return Tuple.Create(limit.X - pos.X, limit.Y - pos.Y, limit.Z - pos.Z);
    }


    private Vector3D CreateRs(PackingPosition position) {
      return new Vector3D() { X = position.X, Y = position.Y, Z = position.Z };
    }

    private void UpdateExtremePointsWithoutStackingConstriants(PackingItem item, PackingPosition position) {
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
      PointsLiesOnAnItem(item, position, out p1, out p2, out p3, out p4);

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
      PointsLiesOnAnItem(item, position, out p1, out p2, out p3, out p4);
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
    private void PointsLiesOnAnItem(PackingItem item, PackingPosition position, out bool p1, out bool p2, out bool p3, out bool p4) {
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


    /// <summary>
    /// Checks if a given the weight of an given item is supportet by the items below.
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



    #endregion

    /// <summary>
    /// Generates new extreme points for a given item and its position.
    /// It also recalcualtes the residual space and removes the extreme points which are not needed any more.
    /// </summary>
    /// <param name="newItem"></param>
    /// <param name="position"></param>
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

    /// <summary>
    /// Returns true if all values of a given tuple are not zero
    /// </summary>
    /// <param name="rs">Tuple with three integer values which represents a residual space</param>
    /// <returns></returns>
    private bool IsNonZero(Tuple<int, int, int> rs) {
      return rs.Item1 > 0 && rs.Item2 > 0 && rs.Item3 > 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="residualSpace"></param>
    /// <returns></returns>
    private bool IsWithinResidualSpaceOfAnotherExtremePoint(Vector3D pos, Tuple<int, int, int> residualSpace) {
      var eps = ExtremePoints.Where(x => !pos.Equals(x) && pos.IsInside(x, ResidualSpace[x]));
      return eps.Any(x => IsWithinResidualSpaceOfAnotherExtremePoint(pos, residualSpace, x, ResidualSpace[x]));
    }
    private bool IsWithinResidualSpaceOfAnotherExtremePoint(Vector3D pos, Tuple<int, int, int> rsPos, PackingPosition ep, Tuple<int, int, int> rsEp) {
      return rsEp.Item1 >= pos.X - ep.X + rsPos.Item1
          && rsEp.Item2 >= pos.Y - ep.Y + rsPos.Item2
          && rsEp.Item3 >= pos.Z - ep.Z + rsPos.Item3;
    }

    /// <summary>
    /// Adds an extrem point to the extreme point collection of the bin packing.
    /// </summary>
    /// <param name="pos">Position of the new extreme point</param>
    /// <returns>Retruns true if the extreme point could be added</returns>
    private bool AddExtremePoint(PackingPosition pos) {
      if (ExtremePoints.Add(pos)) {
        var rs = CalculateResidualSpace(new Vector3D(pos));
        ResidualSpace.Add(pos, rs);
        // Check if the existing extreme points are shadowed by the new point
        // This is, their residual space fit entirely into the residual space of the new point
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


    #region Sliding based packing  and obsolet methods 
    public override PackingPosition FindExtremePointForItem(PackingItem item, bool rotated, bool stackingConstraints) {
      throw new NotSupportedException();
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




    public override PackingPosition FindPositionBySliding(PackingItem item, bool rotated, bool stackingConstraints) {
      throw new NotSupportedException();
    }

    public override void SlidingBasedPacking(ref IList<int> sequence, IList<PackingItem> items, bool stackingConstraints) {
      throw new NotSupportedException();
    }
    public override void SlidingBasedPacking(ref IList<int> sequence, IList<PackingItem> items, Dictionary<int, bool> rotationArray, bool stackingConstraints) {
      throw new NotSupportedException();
    }


    public override void ExtremePointBasedPacking(ref IList<int> sequence, IList<PackingItem> items, bool stackingConstraints) {
      throw new NotSupportedException();
    }

    public override void ExtremePointBasedPacking(ref IList<int> sequence, IList<PackingItem> items, bool stackingConstraints, Dictionary<int, bool> rotationArray) {
      throw new NotSupportedException();
    }

    public override int ShortestPossibleSideFromPoint(PackingPosition position) {
      throw new NotSupportedException();
    }


    protected override void InitializeOccupationLayers() {
    }
    protected override void AddNewItemToOccupationLayers(int itemID, PackingItem item, PackingPosition position) {
    }


    protected override List<int> GetLayerItemIDs(PackingPosition position) {
      return null;
    }
    protected override List<int> GetLayerItemIDs(PackingItem item, PackingPosition position) {
      return null;
    }
    #endregion


    /// <summary>
    /// Updates the resiual space for a packing item.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="pos"></param>
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