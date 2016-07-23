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

namespace HeuristicLab.Problems.BinPacking2D {
  [Item("BinPacking2D", "Represents a single-bin packing for a 2D bin-packing problem.")]
  [StorableClass]
  public class BinPacking2D : BinPacking.BinPacking<PackingPosition, PackingShape, PackingItem> {

    public BinPacking2D(PackingShape binShape)
      : base(binShape) {
      ExtremePoints = new SortedSet<PackingPosition>();
      ExtremePoints.Add(binShape.Origin);
      InitializeOccupationLayers();
    }

    [StorableConstructor]
    protected BinPacking2D(bool deserializing) : base(deserializing) { }
    protected BinPacking2D(BinPacking2D original, Cloner cloner)
      : base(original, cloner) {
      this.ExtremePoints = new SortedSet<PackingPosition>(original.ExtremePoints.Select(p => cloner.Clone(p)));
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new BinPacking2D(this, cloner);
    }

    protected override void GenerateNewExtremePointsForNewItem(PackingItem newItem, PackingPosition position) {

      int newWidth = position.Rotated ? newItem.Height : newItem.Width;
      int newHeight = position.Rotated ? newItem.Width : newItem.Height;

      //Find ExtremePoints beginning from sourcepointX
      var sourcePointX = new PackingPosition(0, position.X + newWidth, position.Y);
      if (sourcePointX.X < BinShape.Width && sourcePointX.Y < BinShape.Height) {
        //Traversing down the y-axis        
        var newPoint = new PackingPosition(0, sourcePointX.X, sourcePointX.Y - 1);
        while (sourcePointX.Y > 0 && !IsPointOccupied(newPoint)) {
          sourcePointX = newPoint;
          newPoint = new PackingPosition(0, sourcePointX.X, sourcePointX.Y - 1);
        }
        ExtremePoints.Add(new PackingPosition(0, sourcePointX.X, sourcePointX.Y));
      }

      //Find ExtremePoints beginning from sourcepointY
      var sourcePointY = new PackingPosition(0, position.X, position.Y + newHeight);
      if (sourcePointY.X < BinShape.Width && sourcePointY.Y < BinShape.Height) {
        //Traversing down the x-axis  
        var newPoint = new PackingPosition(0, sourcePointY.X - 1, sourcePointY.Y);
        while (sourcePointY.X > 0 && !IsPointOccupied(newPoint)) {
          sourcePointY = newPoint;
          newPoint = new PackingPosition(0, sourcePointY.X - 1, sourcePointY.Y);
        }
        ExtremePoints.Add(new PackingPosition(0, sourcePointY.X, sourcePointY.Y));
      }
    }

    public override PackingPosition FindExtremePointForItem(PackingItem item, bool rotated, bool stackingConstraints) {
      PackingItem rotatedItem = new PackingItem(
        rotated ? item.Height : item.Width,
        rotated ? item.Width : item.Height,
        item.TargetBin);

      int epIndex = 0;
      while (epIndex < ExtremePoints.Count && (!IsPositionFeasible(rotatedItem, ExtremePoints.ElementAt(epIndex)))) { epIndex++; }

      if (epIndex < ExtremePoints.Count) {
        var currentPoint = ExtremePoints.ElementAt(epIndex);

        var result = new PackingPosition(currentPoint.AssignedBin, currentPoint.X, currentPoint.Y, rotated);
        return result;
      }
      return null;
    }
    public override PackingPosition FindPositionBySliding(PackingItem item, bool rotated) {
      PackingPosition currentPosition = new PackingPosition(0,
        BinShape.Width - (rotated ? item.Height : item.Width),
        BinShape.Height - (rotated ? item.Width : item.Height), rotated);
      //Slide the item as far as possible to the left
      while (IsPositionFeasible(item, PackingPosition.MoveLeft(currentPosition))
        || IsPositionFeasible(item, PackingPosition.MoveDown(currentPosition))) {
        //Slide the item as far as possible to the bottom
        while (IsPositionFeasible(item, PackingPosition.MoveDown(currentPosition))) {
          currentPosition = PackingPosition.MoveDown(currentPosition);
        }
        if (IsPositionFeasible(item, PackingPosition.MoveLeft(currentPosition)))
          currentPosition = PackingPosition.MoveLeft(currentPosition);
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
        var position = FindPositionBySliding(item, rotationArray[temp[i]]);
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
        var positionFound = FindExtremePointForItem(item, false, false);
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
        var positionFound = FindExtremePointForItem(item, rotationArray[itemID], false);
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

      if (position.X >= width || position.Y >= height)
        return shortestSide;

      PackingPosition current = new PackingPosition(0, position.X, position.Y);
      while (current.X < width && IsPointOccupied(current)) { current = PackingPosition.MoveRight(current); }
      if (current.X - position.X < shortestSide)
        shortestSide = current.X - position.X;


      current = new PackingPosition(0, position.X, position.Y);
      while (current.Y < height && IsPointOccupied(current)) { current = PackingPosition.MoveUp(current); }
      if (current.Y - position.Y < shortestSide)
        shortestSide = current.Y - position.Y;

      return shortestSide;
    }
    public override bool IsStaticStable(PackingItem item, PackingPosition position) {
      throw new NotSupportedException();
    }

    protected override void InitializeOccupationLayers() {
      for (int i = 0; i * 10 <= BinShape.Width; i += 1) {
        OccupationLayers[i] = new List<int>();
      }
    }

    protected override void AddNewItemToOccupationLayers(int itemID, PackingItem item, PackingPosition position) {
      int x1 = position.X / 10;
      int x2 = (position.X + (position.Rotated ? item.Height : item.Width)) / 10;

      for (int i = x1; i <= x2; i++)
        OccupationLayers[i].Add(itemID);
    }
    protected override List<int> GetLayerItemIDs(PackingPosition position) {
      return OccupationLayers[position.X / 10];
    }
    protected override List<int> GetLayerItemIDs(PackingItem item, PackingPosition position) {
      List<int> result = new List<int>();
      int x1 = position.X / 10;
      int x2 = (position.X + (position.Rotated ? item.Height : item.Width)) / 10;

      for (int i = x1; i <= x2; i++)
        result.AddRange(OccupationLayers[i]);

      return result;
    }
  }
}
