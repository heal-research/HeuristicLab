#region License Information
/* HeuristicLab
 * Copyright (C) Joseph Helm and Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
using HeuristicLab.Core;
using HeuristicLab.Common;
using HeuristicLab.Collections;

namespace HeuristicLab.Problems.BinPacking {
  [Item("BinPacking", "Represents a single-bin packing for a bin-packing problem.")]
  [StorableType("7B0C7B64-CB50-405F-9F73-15B7C86F9B86")]
  public abstract class BinPacking<TPos, TBin, TItem> : Item
    where TPos : class, IPackingPosition
    where TBin : PackingShape<TPos>
    where TItem : PackingShape<TPos> {
    #region Properties
    [Storable]
    public ObservableDictionary<int, TPos> Positions { get; private set; }

    [Storable]
    public ObservableDictionary<int, TItem> Items { get; private set; }

    [Storable]
    public TBin BinShape { get; private set; }

    [Storable]
    public SortedSet<TPos> ExtremePoints { get; protected set; }

    [Storable]
    protected Dictionary<int, List<int>> OccupationLayers { get; set; }
    
    #endregion Properties

    public int FreeVolume {
      get { return BinShape.Volume - Items.Sum(x => x.Value.Volume); }
    }

    protected BinPacking(TBin binShape)
      : base() {
      Positions = new ObservableDictionary<int, TPos>();
      Items = new ObservableDictionary<int, TItem>();
      BinShape = (TBin)binShape.Clone();
      ExtremePoints = new SortedSet<TPos>();
      OccupationLayers = new Dictionary<int, List<int>>();
    }

    [StorableConstructor]
    protected BinPacking(StorableConstructorFlag _) : base(_) { }
    protected BinPacking(BinPacking<TPos, TBin, TItem> original, Cloner cloner)
      : base(original, cloner) {
      this.Positions = new ObservableDictionary<int, TPos>();
      foreach (var kvp in original.Positions) {
        Positions.Add(kvp.Key, cloner.Clone(kvp.Value));
      }
      this.Items = new ObservableDictionary<int, TItem>();
      foreach (var kvp in original.Items) {
        Items.Add(kvp.Key, cloner.Clone(kvp.Value));
      }
      this.BinShape = (TBin)original.BinShape.Clone(cloner);
      this.ExtremePoints = new SortedSet<TPos>(original.ExtremePoints.Select(p => cloner.Clone(p)));
      this.OccupationLayers = new Dictionary<int, List<int>>();
      foreach (var kvp in original.OccupationLayers) {
        OccupationLayers.Add(kvp.Key, new List<int>(kvp.Value));
      }
    }

    protected abstract void GenerateNewExtremePointsForNewItem(TItem item, TPos position);

    public abstract TPos FindExtremePointForItem(TItem item, bool rotated, bool stackingConstraints);
    public abstract TPos FindPositionBySliding(TItem item, bool rotated, bool stackingConstraints);

    public abstract void SlidingBasedPacking(ref IList<int> sequence, IList<TItem> items, bool stackingConstraints);
    public abstract void SlidingBasedPacking(ref IList<int> sequence, IList<TItem> items, Dictionary<int, bool> rotationArray, bool stackingConstraints);
    public abstract void ExtremePointBasedPacking(ref IList<int> sequence, IList<TItem> items, bool stackingConstraints);
    public abstract void ExtremePointBasedPacking(ref IList<int> sequence, IList<TItem> items, bool stackingConstraints, Dictionary<int, bool> rotationArray);

    public virtual void PackItem(int itemID, TItem item, TPos position) {
      Items[itemID] = item;
      Positions[itemID] = position;
      ExtremePoints.Remove(position);
      foreach (int id in Items.Select(x => x.Key))
        GenerateNewExtremePointsForNewItem(Items[id], Positions[id]);
      
      AddNewItemToOccupationLayers(itemID, item, position);
    }
    public virtual bool PackItemIfFeasible(int itemID, TItem item, TPos position, bool stackingConstraints) {
      if (IsPositionFeasible(item, position, stackingConstraints)) {
        PackItem(itemID, item, position);
        return true;
      }
      return false;
    }

    public double PackingDensity {
      get {
        double result = 0;
        foreach (var entry in Items)
          result += entry.Value.Volume;
        result /= BinShape.Volume;
        return result;
      }
    }


    public int PointOccupation(TPos position) {
      foreach (var id in GetLayerItemIDs(position)) {
        if (Items[id].EnclosesPoint(Positions[id], position))
          return id;
      }
      return -1;
    }

    public bool IsPointOccupied(TPos position) {
      foreach (var id in GetLayerItemIDs(position)) {
        if (Items[id].EnclosesPoint(Positions[id], position))
          return true;
      }
      return false;
    }
    public virtual bool IsPositionFeasible(TItem item, TPos position, bool stackingConstraints) {
      //In this case feasability is defined as following: 1. the item fits into the bin-borders; 2. the point is supported by something; 3. the item does not collide with another already packed item
      if (!BinShape.Encloses(position, item))
        return false;

      foreach (var id in GetLayerItemIDs(item, position)) {
        if (Items[id].Overlaps(Positions[id], position, item))
          return false;
      }

      return true;
    }
    
    public abstract int ShortestPossibleSideFromPoint(TPos position);
    public abstract bool IsStaticStable(TItem measures, TPos position);

    protected abstract void InitializeOccupationLayers();
    protected abstract void AddNewItemToOccupationLayers(int itemID, TItem item, TPos position);
    protected abstract List<int> GetLayerItemIDs(TPos position);
    protected abstract List<int> GetLayerItemIDs(TItem item, TPos position);
  }
}
