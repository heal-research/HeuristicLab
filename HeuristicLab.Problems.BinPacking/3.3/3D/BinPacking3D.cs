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

      //Find ExtremePoints beginning from sourcepointX
      var sourcePointX = new PackingPosition(0, position.X + newWidth, position.Y, position.Z);
      if (sourcePointX.X < BinShape.Width && sourcePointX.Y < BinShape.Height && sourcePointX.Z < BinShape.Depth) {
        //Traversing down the y-axis  
        PackingPosition current = new PackingPosition(0, sourcePointX.X, sourcePointX.Y, sourcePointX.Z);
        while (current.Y > 0 && !IsPointOccupied(PackingPosition.MoveDown(current))) {
          current = PackingPosition.MoveDown(current);
        }
        AddExtremePoint((PackingPosition)current.Clone());
        while (current.X > 0 && !IsPointOccupied(PackingPosition.MoveLeft(current))) {
          current = PackingPosition.MoveLeft(current);
        }
        AddExtremePoint(current);

        //Traversing down the z-axis
        current = new PackingPosition(0, sourcePointX.X, sourcePointX.Y, sourcePointX.Z);
        while (current.Z > 0 && !IsPointOccupied(PackingPosition.MoveBack(current))) {
          current = PackingPosition.MoveBack(current);
        }
        AddExtremePoint((PackingPosition)current.Clone());
        while (current.Y > 0 && !IsPointOccupied(PackingPosition.MoveDown(current))) {
          current = PackingPosition.MoveDown(current);
        }
        AddExtremePoint(current);
      }

      //Find ExtremePoints beginning from sourcepointY
      var sourcePointY = new PackingPosition(0, position.X, position.Y + newItem.Height, position.Z);
      if (sourcePointY.X < BinShape.Width && sourcePointY.Y < BinShape.Height && sourcePointY.Z < BinShape.Depth) {
        //Traversing down the x-axis          
        PackingPosition current = new PackingPosition(0, sourcePointY.X, sourcePointY.Y, sourcePointY.Z);
        while (current.X > 0 && !IsPointOccupied(PackingPosition.MoveLeft(current))) {
          current = PackingPosition.MoveLeft(current);
        }
        AddExtremePoint((PackingPosition)current.Clone());
        while (current.Y > 0 && !IsPointOccupied(PackingPosition.MoveDown(current))) {
          current = PackingPosition.MoveDown(current);
        }
        AddExtremePoint(current);

        //Traversing down the z-axis
        current = new PackingPosition(0, sourcePointY.X, sourcePointY.Y, sourcePointY.Z);
        while (current.Z > 0 && !IsPointOccupied(PackingPosition.MoveBack(current))) {
          current = PackingPosition.MoveBack(current);
        }
        AddExtremePoint((PackingPosition)current.Clone());
        while (current.Y > 0 && !IsPointOccupied(PackingPosition.MoveDown(current))) {
          current = PackingPosition.MoveDown(current);
        }
        AddExtremePoint(current);
      }

      //Find ExtremePoints beginning from sourcepointZ
      var sourcePointZ = new PackingPosition(0, position.X, position.Y, position.Z + newDepth);
      if (sourcePointZ.X < BinShape.Width && sourcePointZ.Y < BinShape.Height && sourcePointZ.Z < BinShape.Depth) {
        //Traversing down the x-axis
        PackingPosition current = new PackingPosition(0, sourcePointZ.X, sourcePointZ.Y, sourcePointZ.Z);
        while (current.X > 0 && !IsPointOccupied(PackingPosition.MoveLeft(current))) {
          current = PackingPosition.MoveLeft(current);
        }
        AddExtremePoint((PackingPosition)current.Clone());
        while (current.Y > 0 && !IsPointOccupied(PackingPosition.MoveDown(current))) {
          current = PackingPosition.MoveDown(current);
        }
        AddExtremePoint(current);

        //Traversing down the y-axis
        current = new PackingPosition(0, sourcePointZ.X, sourcePointZ.Y, sourcePointZ.Z);
        while (current.Y > 0 && !IsPointOccupied(PackingPosition.MoveDown(current))) {
          current = PackingPosition.MoveDown(current);
        }
        AddExtremePoint((PackingPosition)current.Clone());
        while (current.X > 0 && !IsPointOccupied(PackingPosition.MoveLeft(current))) {
          current = PackingPosition.MoveLeft(current);
        }
        AddExtremePoint(current);
      }
    }

    private void AddExtremePoint(PackingPosition current) {
      if (ExtremePoints.Add(current)) {
        var tuple = Tuple.Create(BinShape.Width - current.X, BinShape.Height - current.Y, BinShape.Depth - current.Z);
        ResidualSpace.Add(current, tuple);
      }
    }

    public override PackingPosition FindExtremePointForItem(PackingItem item, bool rotated, bool stackingConstraints) {
      PackingItem newItem = new PackingItem(
        rotated ? item.Depth : item.Width,
        item.Height,
        rotated ? item.Width : item.Depth,
        item.TargetBin, item.Weight, item.Material);

      var ep = ExtremePoints.Where(x => IsPositionFeasible(newItem, x, stackingConstraints)).FirstOrDefault();
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
  }
}