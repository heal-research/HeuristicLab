#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Joseph Helm and Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Collections;

namespace HeuristicLab.Problems.BinPacking {
  [Item("BinPacking", "Represents a single-bin packing for a bin-packing problem.")]
  [StorableClass]
  public abstract class BinPacking<TPos, TBin, TItem> : Item
    where TPos : class, IPackingPosition
    where TBin : PackingShape<TPos>
    where TItem : PackingShape<TPos> {
    #region Properties
    [Storable]

    //key = item id
    public ObservableDictionary<int, TPos> Positions { get; private set; }

    [Storable]
    //key = item id
    public ObservableDictionary<int, TItem> Items { get; private set; }

    [Storable]
    public TBin BinShape { get; private set; }

    

    public double PackingDensity {
      get {
        double result = 0;
        foreach (var entry in Items)
          result += entry.Value.Volume;
        result /= BinShape.Volume;
        return result;
      }
    }

    public int FreeVolume {
      get { return BinShape.Volume - Items.Sum(x => x.Value.Volume); }
    }
    #endregion Properties
    
    protected BinPacking(TBin binShape)
      : base() {
      Positions = new ObservableDictionary<int, TPos>();
      Items = new ObservableDictionary<int, TItem>();
      BinShape = (TBin)binShape.Clone();
    }

    [StorableConstructor]
    protected BinPacking(bool deserializing) : base(deserializing) { }

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
    }
    
    /// <summary>
    /// Generate new extreme points for a given item
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position"></param>
    protected abstract void GenerateNewExtremePointsForNewItem(TItem item, TPos position);
    
    /// <summary>
    /// Packs an item into the bin packing
    /// </summary>
    /// <param name="itemID"></param>
    /// <param name="item"></param>
    /// <param name="position"></param>
    public abstract void PackItem(int itemID, TItem item, TPos position);
    
    /// <summary>
    /// Checks if the given position is feasible for the given item
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position"></param>
    /// <param name="stackingConstraints"></param>
    /// <returns>Returns true if the given position is feasible for the given item</returns>
    public abstract bool IsPositionFeasible(TItem item, TPos position, bool stackingConstraints);

    /// <summary>
    /// Checks if the given item is static stable on the given position
    /// </summary>
    /// <param name="measures">Item</param>
    /// <param name="position">Position of the item</param>
    /// <returns>Returns true if the given item is static stable on the given position</returns>
    public abstract bool IsStaticStable(TItem measures, TPos position);
    
    
  }
}
