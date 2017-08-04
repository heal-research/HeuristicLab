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
      ExtremePoints.Remove(position);
      ResidualSpace.Remove(position);
      UpdateResidualSpace(item, position);
      base.PackItem(itemID, item, position);
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
        if (!IsWithinResidualSpaceOfAnotherExtremePoint(below, residualSpace)) {
          // add only if the projected point's residual space is not a sub-space
          // of another extreme point's residual space
          var belowPoint = new PackingPosition(position.AssignedBin, below.X, below.Y, below.Z);
          AddExtremePoint(belowPoint);
        }
        // Projecting onto the XY-plane
        var back = ProjectBackward(sourcePoint);
        residualSpace = CalculateResidualSpace(back);
        if (!IsWithinResidualSpaceOfAnotherExtremePoint(back, residualSpace)) {
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
        if (!IsWithinResidualSpaceOfAnotherExtremePoint(left, residualSpace)) {
          // add only if the projected point's residual space is not a sub-space
          // of another extreme point's residual space
          var leftPoint = new PackingPosition(position.AssignedBin, left.X, left.Y, left.Z);
          AddExtremePoint(leftPoint);
        }
        // Projecting onto the XY-plane
        var back = ProjectBackward(sourcePoint);
        residualSpace = CalculateResidualSpace(back);
        if (!IsWithinResidualSpaceOfAnotherExtremePoint(back, residualSpace)) {
          // add only if the projected point's residual space is not a sub-space
          // of another extreme point's residual space
          var backPoint = new PackingPosition(position.AssignedBin, back.X, back.Y, back.Z);
          AddExtremePoint(backPoint);
        }
      }

      //Find ExtremePoints beginning from sourcepointZ
      sourcePoint = new Vector3D(position.X, position.Y, position.Z + newDepth);
      if (sourcePoint.X < BinShape.Width && sourcePoint.Y < BinShape.Height && sourcePoint.Z < BinShape.Depth) {
        // Projecting onto the YZ-plane
        var left = ProjectLeft(sourcePoint);
        var residualSpace = CalculateResidualSpace(left);
        if (!IsWithinResidualSpaceOfAnotherExtremePoint(left, residualSpace)) {
          // add only if the projected point's residual space is not a sub-space
          // of another extreme point's residual space
          var leftPoint = new PackingPosition(position.AssignedBin, left.X, left.Y, left.Z);
          AddExtremePoint(leftPoint);
        }
        // Projecting onto the XZ-plane
        var below = ProjectDown(sourcePoint);
        residualSpace = CalculateResidualSpace(below);
        if (!IsWithinResidualSpaceOfAnotherExtremePoint(below, residualSpace)) {
          // add only if the projected point's residual space is not a sub-space
          // of another extreme point's residual space
          var belowPoint = new PackingPosition(position.AssignedBin, below.X, below.Y, below.Z);
          AddExtremePoint(belowPoint);
        }
      }
    }

    private bool IsWithinResidualSpaceOfAnotherExtremePoint(Vector3D pos, Tuple<int, int, int> residualSpace) {
      var eps = ExtremePoints.Where(x => pos.IsInside(x, ResidualSpace[x]));
      return eps.Any(x => ResidualSpace[x].Item1 >= pos.X - x.X + residualSpace.Item1
          && ResidualSpace[x].Item2 >= pos.Y - x.Y + residualSpace.Item2
          && ResidualSpace[x].Item3 >= pos.Z - x.Z + residualSpace.Item3);
    }

    private bool AddExtremePoint(PackingPosition pos) {
      if (ExtremePoints.Add(pos)) {
        ResidualSpace.Add(pos, Tuple.Create(BinShape.Width - pos.X, BinShape.Height - pos.Y, BinShape.Depth - pos.Z));
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
        var rs = ResidualSpace[ep];
        var depth = pos.Rotated ? item.Width : item.Depth;
        var width = pos.Rotated ? item.Depth : item.Width;
        if (ep.Z >= pos.Z && ep.Z < pos.Z + depth) {
          if (ep.X <= pos.X && ep.Y >= pos.Y && ep.Y < pos.Y + item.Height) {
            var diff = pos.X - ep.X;
            var newRSX = Math.Min(rs.Item1, diff);
            ResidualSpace[ep] = Tuple.Create(newRSX, rs.Item2, rs.Item3);
          }
          if (ep.Y <= pos.Y && ep.X >= pos.X && ep.X < pos.X + width) {
            var diff = pos.Y - ep.Y;
            var newRSY = Math.Min(rs.Item2, diff);
            ResidualSpace[ep] = Tuple.Create(rs.Item1, newRSY, rs.Item3);
          }
        }
        if (ep.Z <= pos.Z && 
          ep.Y >= pos.Y && ep.Y < pos.Y + item.Height &&
          ep.X >= pos.X && ep.X < pos.X + width) {
            var diff = pos.Z - ep.Z;
            var newRSZ = Math.Min(rs.Item3, diff);
            ResidualSpace[ep] = Tuple.Create(rs.Item1, rs.Item2, newRSZ);
        }
      }
      return;
    }

    #region Helper classes for vector math
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

      public Vector3D Cross(Vector3D b) {
        return new Vector3D(
          Y * b.Z - Z * b.Y,
          -X * b.Z + Z * b.X,
          X * b.Y - Y * b.X
        );
      }

      public bool IsInside(PackingPosition pos, Tuple<int, int, int> rs) {
        return X >= pos.X && X < pos.X + rs.Item1
          && Y >= pos.Y && Y < pos.Y + rs.Item2
          && Z >= pos.Z && Z < pos.Z + rs.Item3;
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

    // A line is given as a point and a directing vector
    private class Line3D {
      public Vector3D Point;
      public Vector3D Direction;

      public Line3D(Vector3D point, Vector3D direction) {
        Point = point;
        Direction = direction;
      }

      public bool Intersects(Plane3D plane) {
        return plane.Intersects(this);
      }

      public Vector3D Intersect(Plane3D plane) {
        return plane.Intersect(this);
      }
    }

    private enum Side { Top, Left, Bottom, Right, Front, Back }
    // A plane is given as a point and two directing vectors
    private class Plane3D {
      public Vector3D Point { get; private set; }
      public Vector3D Direction1 { get; private set; }
      public Vector3D Direction2 { get; private set; }
      public Vector3D Normal { get; private set; }
      private int rhs;

      public Plane3D(PackingShape bin, Side side) {
        switch (side) {
          case Side.Top: // ZX plane
            Point = new Vector3D(0, bin.Height, 0);
            Direction1 = new Vector3D(0, 0, bin.Depth);
            Direction2 = new Vector3D(bin.Width, 0, 0);
            break;
          case Side.Left: // ZY plane
            Point = new Vector3D(0, 0, 0);
            Direction1 = new Vector3D(0, 0, bin.Depth);
            Direction2 = new Vector3D(0, bin.Height, 0);
            break;
          case Side.Bottom: // XZ plane
            Point = new Vector3D(0, 0, 0);
            Direction1 = new Vector3D(bin.Width, 0, 0);
            Direction2 = new Vector3D(0, 0, bin.Depth);
            break;
          case Side.Right: // YZ plane
            Point = new Vector3D(bin.Width, 0, 0);
            Direction1 = new Vector3D(0, bin.Height, 0);
            Direction2 = new Vector3D(0, 0, bin.Depth);
            break;
          case Side.Front: // XY plane
            Point = new Vector3D(0, 0, bin.Depth);
            Direction1 = new Vector3D(bin.Width, 0, 0);
            Direction2 = new Vector3D(0, bin.Height, 0);
            break;
          case Side.Back: // YX plane
            Point = new Vector3D(0, 0, 0);
            Direction1 = new Vector3D(0, bin.Height, 0);
            Direction2 = new Vector3D(bin.Width, 0, 0);
            break;
        }
        Normal = Direction1.Cross(Direction2);
        rhs = 0;
      }

      public Plane3D(PackingPosition pos, PackingItem item, Side side) {
        // the directing vectors are chosen such that normal always points outside the packing item
        switch (side) {
          case Side.Top: // ZX plane
            Point = new Vector3D(pos.X, pos.Y + item.Height, pos.Z);
            Direction1 = new Vector3D(0, 0, pos.Rotated ? item.Width : item.Depth);
            Direction2 = new Vector3D(pos.Rotated ? item.Depth : item.Width, 0, 0);
            break;
          case Side.Left: // ZY plane
            Point = new Vector3D(pos.X, pos.Y, pos.Z);
            Direction1 = new Vector3D(0, 0, pos.Rotated ? item.Width : item.Depth);
            Direction2 = new Vector3D(0, item.Height, 0);
            break;
          case Side.Bottom: // XZ plane
            Point = new Vector3D(pos.X, pos.Y, pos.Z);
            Direction1 = new Vector3D(pos.Rotated ? item.Depth : item.Width, 0, 0);
            Direction2 = new Vector3D(0, 0, pos.Rotated ? item.Width : item.Depth);
            break;
          case Side.Right: // YZ plane
            Point = new Vector3D(pos.X + (pos.Rotated ? item.Depth : item.Width), pos.Y, pos.Z);
            Direction1 = new Vector3D(0, item.Height, 0);
            Direction2 = new Vector3D(0, 0, pos.Rotated ? item.Width : item.Depth);
            break;
          case Side.Front: // XY plane
            Point = new Vector3D(pos.X, pos.Y, pos.Z + (pos.Rotated ? item.Width : item.Depth));
            Direction1 = new Vector3D(pos.Rotated ? item.Depth : item.Width, 0, 0);
            Direction2 = new Vector3D(0, item.Height, 0);
            break;
          case Side.Back: // YX plane
            Point = new Vector3D(pos.X, pos.Y, pos.Z);
            Direction1 = new Vector3D(0, item.Height, 0);
            Direction2 = new Vector3D(pos.Rotated ? item.Depth : item.Width, 0, 0);
            break;
        }
        Normal = Direction1.Cross(Direction2);
        rhs = Normal.X * Point.X + Normal.Y * Point.Y + Normal.Z * Point.Z;
      }

      public bool IsElementOf(Vector3D point) {
        return Normal.X * point.X + Normal.Y * point.Y + Normal.Z * point.Z == rhs;
      }

      public bool Intersects(Line3D line) {
        return Intersect(line) != null;
      }

      public Vector3D Intersect(Line3D line) {
        var denom = Normal * line.Direction;
        if (denom == 0) {
          // dir is perpendicular to the normal vector of the plane
          // this means they intersect if p1 is element of the plane
          // also the plane does not stretch infinitely, but is bound
          // to the parallelogram spanned by the point and the two
          // directing vectors
          if (IsElementOf(line.Point) && IsWithinDirectionalVectors(line.Point))
            return line.Point;
          else return null;
        }
        var intersect = line.Point + ((Normal * (Point - line.Point)) / denom) * line.Direction;
        if (IsWithinDirectionalVectors(intersect)) return intersect;
        return null;
      }

      public bool IsWithinDirectionalVectors(Vector3D point) {
        return point.X >= Point.X && (Direction1.X + Direction2.X == 0 || (point.X < Point.X + Direction1.X || point.X < Point.X + Direction2.X))
            && point.Y >= Point.Y && (Direction1.Y + Direction2.Y == 0 || (point.Y < Point.Y + Direction1.Y || point.Y < Point.Y + Direction2.Y))
            && point.Z >= Point.Z && (Direction1.Z + Direction2.Z == 0 || (point.Z < Point.Z + Direction1.Z || point.Z < Point.Z + Direction2.Z));
      }
    }
    #endregion
  }
}