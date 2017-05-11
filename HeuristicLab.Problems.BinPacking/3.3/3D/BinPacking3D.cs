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

    public BinPacking3D(PackingShape binShape)
      : base(binShape) {
      ExtremePoints = new SortedSet<PackingPosition>();
      ExtremePoints.Add(binShape.Origin);
      InitializeOccupationLayers();
    }
    [StorableConstructor]
    protected BinPacking3D(bool deserializing) : base(deserializing) { }
    protected BinPacking3D(BinPacking3D original, Cloner cloner)
      : base(original, cloner) {
      this.ExtremePoints = new SortedSet<PackingPosition>(original.ExtremePoints.Select(p => cloner.Clone(p)));
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new BinPacking3D(this, cloner);
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
        ExtremePoints.Add((PackingPosition)current.Clone());
        while (current.X > 0 && !IsPointOccupied(PackingPosition.MoveLeft(current))) {
          current = PackingPosition.MoveLeft(current);
        }
        ExtremePoints.Add(current);

        //Traversing down the z-axis
        current = new PackingPosition(0, sourcePointX.X, sourcePointX.Y, sourcePointX.Z);
        while (current.Z > 0 && !IsPointOccupied(PackingPosition.MoveBack(current))) {
          current = PackingPosition.MoveBack(current);
        }
        ExtremePoints.Add((PackingPosition)current.Clone());
        while (current.Y > 0 && !IsPointOccupied(PackingPosition.MoveDown(current))) {
          current = PackingPosition.MoveDown(current);
        }
        ExtremePoints.Add(current);
      }

      //Find ExtremePoints beginning from sourcepointY
      var sourcePointY = new PackingPosition(0, position.X, position.Y + newItem.Height, position.Z);
      if (sourcePointY.X < BinShape.Width && sourcePointY.Y < BinShape.Height && sourcePointY.Z < BinShape.Depth) {
        //Traversing down the x-axis          
        PackingPosition current = new PackingPosition(0, sourcePointY.X, sourcePointY.Y, sourcePointY.Z);
        while (current.X > 0 && !IsPointOccupied(PackingPosition.MoveLeft(current))) {
          current = PackingPosition.MoveLeft(current);
        }
        ExtremePoints.Add((PackingPosition)current.Clone());
        while (current.Y > 0 && !IsPointOccupied(PackingPosition.MoveDown(current))) {
          current = PackingPosition.MoveDown(current);
        }
        ExtremePoints.Add(current);

        //Traversing down the z-axis
        current = new PackingPosition(0, sourcePointY.X, sourcePointY.Y, sourcePointY.Z);
        while (current.Z > 0 && !IsPointOccupied(PackingPosition.MoveBack(current))) {
          current = PackingPosition.MoveBack(current);
        }
        ExtremePoints.Add((PackingPosition)current.Clone());
        while (current.Y > 0 && !IsPointOccupied(PackingPosition.MoveDown(current))) {
          current = PackingPosition.MoveDown(current);
        }
        ExtremePoints.Add(current);
      }

      //Find ExtremePoints beginning from sourcepointZ
      var sourcePointZ = new PackingPosition(0, position.X, position.Y, position.Z + newDepth);
      if (sourcePointZ.X < BinShape.Width && sourcePointZ.Y < BinShape.Height && sourcePointZ.Z < BinShape.Depth) {
        //Traversing down the x-axis
        PackingPosition current = new PackingPosition(0, sourcePointZ.X, sourcePointZ.Y, sourcePointZ.Z);
        while (current.X > 0 && !IsPointOccupied(PackingPosition.MoveLeft(current))) {
          current = PackingPosition.MoveLeft(current);
        }
        ExtremePoints.Add((PackingPosition)current.Clone());
        while (current.Y > 0 && !IsPointOccupied(PackingPosition.MoveDown(current))) {
          current = PackingPosition.MoveDown(current);
        }
        ExtremePoints.Add(current);

        //Traversing down the y-axis
        current = new PackingPosition(0, sourcePointZ.X, sourcePointZ.Y, sourcePointZ.Z);
        while (current.Y > 0 && !IsPointOccupied(PackingPosition.MoveDown(current))) {
          current = PackingPosition.MoveDown(current);
        }
        ExtremePoints.Add((PackingPosition)current.Clone());
        while (current.X > 0 && !IsPointOccupied(PackingPosition.MoveLeft(current))) {
          current = PackingPosition.MoveLeft(current);
        }
        ExtremePoints.Add(current);
      }
    }

    public override PackingPosition FindExtremePointForItem(PackingItem item, bool rotated, bool stackingConstraints) {

      PackingItem newItem = new PackingItem(
        rotated ? item.Depth : item.Width,
        item.Height,
        rotated ? item.Width : item.Depth,
        item.TargetBin, item.Weight, item.Material);

      int epIndex = 0;
      while (epIndex < ExtremePoints.Count && (
        !IsPositionFeasible(newItem, ExtremePoints.ElementAt(epIndex))
        || !IsSupportedByAtLeastOnePoint(newItem, ExtremePoints.ElementAt(epIndex))
        || (stackingConstraints && !IsStaticStable(newItem, ExtremePoints.ElementAt(epIndex)))
        || (stackingConstraints && !IsWeightSupported(newItem, ExtremePoints.ElementAt(epIndex)))
      )) { epIndex++; }

      if (epIndex < ExtremePoints.Count) {
        var origPoint = ExtremePoints.ElementAt(epIndex);
        var result = new PackingPosition(origPoint.AssignedBin, origPoint.X, origPoint.Y, origPoint.Z, rotated);
        return result;
      }
      return null;
    }

    public override PackingPosition FindPositionBySliding(PackingItem item, bool rotated) {
      //TODO: does not support stacking constraints yet
      //Starting-position at upper right corner (=left bottom point of item-rectangle is at position item.width,item.height)
      PackingPosition currentPosition = new PackingPosition(0,
        BinShape.Width - (rotated ? item.Depth : item.Width),
        BinShape.Height - item.Height,
        BinShape.Depth - (rotated ? item.Width : item.Depth), rotated);
      //Slide the item as far as possible to the bottom
      while (IsPositionFeasible(item, PackingPosition.MoveDown(currentPosition))
        || IsPositionFeasible(item, PackingPosition.MoveLeft(currentPosition))
        || IsPositionFeasible(item, PackingPosition.MoveBack(currentPosition))) {
        //Slide the item as far as possible to the left
        while (IsPositionFeasible(item, PackingPosition.MoveLeft(currentPosition))
      || IsPositionFeasible(item, PackingPosition.MoveBack(currentPosition))) {
          //Slide the item as far as possible to the back
          while (IsPositionFeasible(item, PackingPosition.MoveBack(currentPosition))) {
            currentPosition = PackingPosition.MoveBack(currentPosition);
          }
          if (IsPositionFeasible(item, PackingPosition.MoveLeft(currentPosition)))
            currentPosition = PackingPosition.MoveLeft(currentPosition);
        }
        if (IsPositionFeasible(item, PackingPosition.MoveDown(currentPosition)))
          currentPosition = PackingPosition.MoveDown(currentPosition);
      }

      return IsPositionFeasible(item, currentPosition) ? currentPosition : null;
    }

    public override void SlidingBasedPacking(ref IList<int> sequence, IList<PackingItem> items) {
      var temp = new List<int>(sequence);
      for (int i = 0; i < temp.Count; i++) {
        var item = items[temp[i]];
        var position = FindPositionBySliding(item, false);
        if (position != null) {
          PackItem(temp[i], item, position);
          sequence.Remove(temp[i]);
        }
      }
    }
    public override void SlidingBasedPacking(ref IList<int> sequence, IList<PackingItem> items, Dictionary<int, bool> rotationArray) {
      var temp = new List<int>(sequence);
      for (int i = 0; i < temp.Count; i++) {
        var item = items[temp[i]];
        var position = FindPositionBySliding(item, rotationArray[i]);
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
  }

}
