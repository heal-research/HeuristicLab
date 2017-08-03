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

namespace HeuristicLab.Problems.BinPacking3D {
  [Item("BinPacking3D", "Represents a single-bin packing for a 3D bin-packing problem.")]
  [StorableClass]
  public class BinPacking3D : BinPacking<PackingPosition, PackingShape, PackingItem> {

    [Storable]
    public Dictionary<PackingPosition, Tuple<int, int, int>> ResidualSpace { get; protected set; }

    public BinPacking3D(PackingShape binShape)
      : base(binShape) {
      ResidualSpace = new Dictionary<PackingPosition, Tuple<int,int,int>>();
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

    public override void PackItem(int itemID, PackingItem item, PackingPosition position) {
      ResidualSpace.Remove(position);
      base.PackItem(itemID, item, position);
    }

    protected override void GenerateNewExtremePointsForNewItem(PackingItem newItem, PackingPosition position) {
      int newWidth = position.Rotated ? newItem.Depth : newItem.Width;
      int newDepth = position.Rotated ? newItem.Width : newItem.Depth;

      var itemPlacement = Items.Select(x => new { Item = x.Value, Position = Positions[x.Key] }).ToList();
      //Find ExtremePoints beginning from sourcepointX
      var sourcePoint = new Vector3D() { X = position.X + newWidth, Y = position.Y, Z = position.Z };
      if (sourcePoint.X < BinShape.Width && sourcePoint.Y < BinShape.Height && sourcePoint.Z < BinShape.Depth) {
        //Traversing down the y-axis  
        var below = itemPlacement.Where(x => IsBelow(x.Position, x.Item, sourcePoint))
                                 .MaxItems(x => x.Position.Y + x.Item.Height).FirstOrDefault();
        var projY = false;
        if (below != null) {
          var belowPoint = new PackingPosition(position.AssignedBin, sourcePoint.X, below.Position.Y + below.Item.Height, sourcePoint.Z);
          projY = AddExtremePoint(belowPoint);
        }
        //Traversing down the z-axis
        var back = itemPlacement.Where(x => IsBack(x.Position, x.Item, sourcePoint))
                                .MaxItems(x => x.Position.Z + x.Item.Depth).FirstOrDefault();
        var projZ = false;
        if (back != null) {
          var backPoint = new PackingPosition(position.AssignedBin, sourcePoint.X, sourcePoint.Y, back.Position.Z + back.Item.Depth);
          projZ = AddExtremePoint(backPoint);
        }

        // only add the source point if both projections added new points
        // or if neither of the projections added a point
        // if a projection tried to add a point that already exists, then this
        // extreme point would be covered by the existing extreme point
        if ((below == null && back == null) || (below == null || projY) && (back == null || projZ))
          AddExtremePoint(new PackingPosition(position.AssignedBin, sourcePoint.X, sourcePoint.Y, sourcePoint.Z));
      }

      //Find ExtremePoints beginning from sourcepointY
      sourcePoint = new Vector3D(position.X, position.Y + newItem.Height, position.Z);
      if (sourcePoint.X < BinShape.Width && sourcePoint.Y < BinShape.Height && sourcePoint.Z < BinShape.Depth) {
        //Traversing down the x-axis   
        var left = itemPlacement.Where(x => IsLeft(x.Position, x.Item, sourcePoint))
                                .MaxItems(x => x.Position.X + x.Item.Width).FirstOrDefault();
        var projX = false;
        if (left != null) {
          var leftPoint = new PackingPosition(position.AssignedBin, left.Position.X + left.Item.Width, sourcePoint.Y, sourcePoint.Z);
          projX = AddExtremePoint(leftPoint);
        }

        //Traversing down the z-axis
        var back = itemPlacement.Where(x => IsBack(x.Position, x.Item, sourcePoint))
                                .MaxItems(x => x.Position.Z + x.Item.Depth).FirstOrDefault();
        var projZ = false;
        if (back != null) {
          var backPoint = new PackingPosition(position.AssignedBin, sourcePoint.X, sourcePoint.Y, back.Position.Z + back.Item.Depth);
          projZ = AddExtremePoint(backPoint);
        }

        // only add the source point if both projections added new points
        // or if neither of the projections added a point
        // if a projection tried to add a point that already exists, then this
        // extreme point would be covered by the existing extreme point
        if ((left == null || projX) && (back == null || projZ))
          AddExtremePoint(new PackingPosition(position.AssignedBin, sourcePoint.X, sourcePoint.Y, sourcePoint.Z));
      }

      //Find ExtremePoints beginning from sourcepointZ
      sourcePoint = new Vector3D(position.X, position.Y, position.Z + newDepth);
      if (sourcePoint.X < BinShape.Width && sourcePoint.Y < BinShape.Height && sourcePoint.Z < BinShape.Depth) {
        //Traversing down the x-axis
        var left = itemPlacement.Where(x => IsLeft(x.Position, x.Item, sourcePoint))
                                .MaxItems(x => x.Position.X + x.Item.Width).FirstOrDefault();
        var projX = false;
        if (left != null) {
          var leftPoint = new PackingPosition(position.AssignedBin, left.Position.X + left.Item.Width, sourcePoint.Y, sourcePoint.Z);
          projX = AddExtremePoint(leftPoint);
        }

        //Traversing down the y-axis
        var below = itemPlacement.Where(x => IsBelow(x.Position, x.Item, sourcePoint))
                                 .MaxItems(x => x.Position.Y + x.Item.Height).FirstOrDefault();
        var projY = false;
        if (below != null) {
          var belowPoint = new PackingPosition(position.AssignedBin, sourcePoint.X, below.Position.Y + below.Item.Height, sourcePoint.Z);
          projY = AddExtremePoint(belowPoint);
        }

        // only add the source point if both projections added new points
        // or if neither of the projections added a point
        // if a projection tried to add a point that already exists, then this
        // extreme point would be covered by the existing extreme point
        if ((left == null || projX) && (below == null || projY))
          AddExtremePoint(new PackingPosition(position.AssignedBin, sourcePoint.X, sourcePoint.Y, sourcePoint.Z));
      }
    }

    private bool IsBelow(PackingPosition lowerPos, PackingItem lowerItem, Vector3D upper) {
      return lowerPos.X <= upper.X && lowerPos.X + lowerItem.Width > upper.X
        && lowerPos.Z <= upper.Z && lowerPos.Z + lowerItem.Depth > upper.Z
        && lowerPos.Y + lowerItem.Height < upper.Y;
    }

    private bool IsLeft(PackingPosition leftPos, PackingItem leftItem, Vector3D right) {
      return leftPos.Y <= right.Y && leftPos.Y + leftItem.Height > right.Y
        && leftPos.Z <= right.Z && leftPos.Z + leftItem.Depth > right.Z
        && leftPos.X + leftItem.Width < right.X;
    }

    private bool IsBack(PackingPosition backPos, PackingItem backItem, Vector3D front) {
      return backPos.Y <= front.Y && backPos.Y + backItem.Height > front.Y
        && backPos.X <= front.X && backPos.X + backItem.Width > front.X
        && backPos.Z + backItem.Depth < front.Z;
    }

    private bool AddExtremePoint(PackingPosition current) {
      if (ExtremePoints.Add(current)) {
        var tuple = Tuple.Create(BinShape.Width - current.X, BinShape.Height - current.Y, BinShape.Depth - current.Z);
        ResidualSpace.Add(current, tuple);
        return true;
      }
      return false;
    }

    public override PackingPosition FindExtremePointForItem(PackingItem item, bool rotated, bool stackingConstraints) {
      PackingItem newItem = new PackingItem(
        rotated ? item.Depth : item.Width,
        item.Height,
        rotated ? item.Width : item.Depth,
        item.TargetBin, item.Weight, item.Material);

      var ep = ExtremePoints.Where(x => IsPositionFeasible(newItem, x, stackingConstraints))
                            .OrderBy(x => x.X + x.Y + x.Z) // order by manhattan distance to try those closest to origin first
                            .FirstOrDefault();
      if (ep != null) {
        var result = new PackingPosition(ep.AssignedBin, ep.X, ep.Y, ep.Z, rotated);
        return result;
      }
      return null;
    }

    public override bool IsPositionFeasible(PackingItem item, PackingPosition position, bool stackingConstraints) {
      var feasible = base.IsPositionFeasible(item, position, stackingConstraints);
      return feasible
        && IsSupportedByAtLeastOnePoint(item, position)
        && (!stackingConstraints || (IsStaticStable(item, position) && IsWeightSupported(item, position)));
    }

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
    public override void ExtremePointBasedPacking(ref IList<int> sequence, IList<PackingItem> items, bool stackingConstraints) {
      var temp = new List<int>(sequence);
      foreach (int itemID in temp) {
        var item = items[itemID];
        var positionFound = FindExtremePointForItem(item, false, stackingConstraints);
        if (positionFound != null) {
          PackItem(itemID, item, positionFound);
          if (Items.Count > 1)
            UpdateResidualSpace(item, positionFound);
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
    public override bool IsStaticStable(PackingItem item, PackingPosition position) {
      //Static stability is given, if item is placed on the ground
      if (position.Y == 0)
        return true;

      if (IsPointOccupied(new PackingPosition(0, position.X, position.Y - 1, position.Z))
        && IsPointOccupied(new PackingPosition(0, position.X + item.Width - 1, position.Y - 1, position.Z))
        && IsPointOccupied(new PackingPosition(0, position.X, position.Y - 1, position.Z + item.Depth - 1))
        && IsPointOccupied(new PackingPosition(0, position.X + item.Width - 1, position.Y - 1, position.Z + item.Depth - 1)))
        return true;

      return false;
    }


    public bool IsSupportedByAtLeastOnePoint(PackingItem item, PackingPosition position) {
      if (position.Y == 0)
        return true;

      int y = position.Y - 1;
      for (int x = position.X; x < position.X + item.Width; x++)
        for (int z = position.Z; z < position.Z + item.Depth; z++)
          if (IsPointOccupied(new PackingPosition(0, x, y, z)))
            return true;

      return false;
    }
    public bool IsWeightSupported(PackingItem item, PackingPosition ep) {
      if (ep.Y == 0)
        return true;

      if (Items[PointOccupation(new PackingPosition(0, ep.X, ep.Y - 1, ep.Z))].SupportsStacking(item)
        && Items[PointOccupation(new PackingPosition(0, ep.X + item.Width - 1, ep.Y - 1, ep.Z))].SupportsStacking(item)
        && Items[PointOccupation(new PackingPosition(0, ep.X, ep.Y - 1, ep.Z + item.Depth - 1))].SupportsStacking(item)
        && Items[PointOccupation(new PackingPosition(0, ep.X + item.Width - 1, ep.Y - 1, ep.Z + item.Depth - 1))].SupportsStacking(item))
        return true;

      return false;
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
      foreach (var ep in ExtremePoints) {
        if (ep.Z >= pos.Z && ep.Z <= pos.Z + item.Depth) {
          if (ep.X <= pos.X && ep.Y > pos.Y && ep.Y < pos.Y + item.Height) {
            var diff = pos.X - ep.X;
            var newRSX = ResidualSpace[ep].Item1 < diff ? ResidualSpace[ep].Item1 : diff;
            ResidualSpace[ep] = Tuple.Create(newRSX, ResidualSpace[ep].Item2, ResidualSpace[ep].Item3);
          }
          if (ep.Y <= pos.Y && ep.X > pos.X && ep.X < pos.X + item.Width) {
            var diff = pos.Y - ep.Y;
            var newRSY = ResidualSpace[ep].Item2 < diff ? ResidualSpace[ep].Item2 : diff;
            ResidualSpace[ep] = Tuple.Create(ResidualSpace[ep].Item1, newRSY, ResidualSpace[ep].Item3);
          }
        }
        if (ep.Z <= pos.Z && 
          ep.Y > pos.Y && ep.Y < pos.Y + item.Height &&
          ep.X > pos.X && ep.X < pos.X + item.Width) {
            var diff = pos.Z - ep.Z;
            var newRSZ = ResidualSpace[ep].Item3 < diff ? ResidualSpace[ep].Item3 : diff;
            ResidualSpace[ep] = Tuple.Create(ResidualSpace[ep].Item1, ResidualSpace[ep].Item2, newRSZ);
        }
      }
      return;
    }

    private class Vector3D {
      public int X;
      public int Y;
      public int Z;
      public Vector3D() { }
      public Vector3D(int x, int y, int z) {
        X = x;
        Y = y;
        Z = z;
      }
      public Vector3D(PackingPosition pos) {
        X = pos.X;
        Y = pos.Y;
        Z = pos.Z;
      }
      public static Vector3D AlongX(Vector3D pos, PackingItem item) {
        return new Vector3D(
          pos.X + item.Width,
          pos.Y,
          pos.Z
        );
      }
      public static Vector3D AlongY(Vector3D pos, PackingItem item) {
        return new Vector3D(
          pos.X,
          pos.Y + item.Height,
          pos.Z
        );
      }
      public static Vector3D AlongZ(Vector3D pos, PackingItem item) {
        return new Vector3D(
          pos.X,
          pos.Y,
          pos.Z + item.Depth
        );
      }
      public static Vector3D AlongX(PackingPosition pos, PackingItem item) {
        return new Vector3D(
          pos.X + item.Width,
          pos.Y,
          pos.Z
        );
      }
      public static Vector3D AlongY(PackingPosition pos, PackingItem item) {
        return new Vector3D(
          pos.X,
          pos.Y + item.Height,
          pos.Z
        );
      }
      public static Vector3D AlongZ(PackingPosition pos, PackingItem item) {
        return new Vector3D(
          pos.X,
          pos.Y,
          pos.Z + item.Depth
        );
      }

      public static bool Intersects(Vector3D p1, Vector3D dir, Vector3D p2, Vector3D dir2, Vector3D dir3) {
        var normal = dir2.Cross(dir3);
        var denom = normal * dir;
        if (denom == 0) {
          // dir is perpendicular to the normal vector of the plane
          // this means they intersect if p1 is element of the plane
          return p1.X * normal.X + p1.Y * normal.Y + p1.Z * normal.Z == p2.X * normal.X + p2.Y * normal.Y + p2.Z * normal.Z
            && IsInPlane(p1, p2, dir2, dir3);
        }
        var intersect = p1 + ((normal * (p2 - p1)) / denom) * dir;
        return IsInPlane(intersect, p2, dir2, dir3);
      }

      private static bool IsInPlane(Vector3D p1, Vector3D p2, Vector3D dir2, Vector3D dir3) {
        return p1.X >= p2.X && (p1.X <= p2.X + dir2.X || p1.X <= p2.X + dir3.X)
            && p1.Y >= p2.Y && (p1.Y <= p2.Y + dir2.Y || p1.Y <= p2.Y + dir3.Y)
            && p1.Z >= p2.Z && (p1.Z <= p2.Z + dir2.Z || p1.Z <= p2.Z + dir3.Z);
      }

      public Vector3D Cross(Vector3D b) {
        return new Vector3D(
          Y * b.Z - Z * b.Y,
          -X * b.Z + Z * b.X,
          X * b.Y - Y * b.X
        );
      }
      public static int operator *(Vector3D a, Vector3D b) {
        return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
      }
      public static Vector3D operator *(int a, Vector3D b) {
        return new Vector3D(a * b.X, a * b.Y, a * b.Z);
      }
      public static Vector3D operator *(Vector3D a, int b) {
        return new Vector3D(a.X * b, a.Y * b, a.Z * b);
      }
      public static Vector3D operator +(Vector3D a, Vector3D b) {
        return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
      }
      public static Vector3D operator -(Vector3D a, Vector3D b) {
        return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
      }
    }
  }
}