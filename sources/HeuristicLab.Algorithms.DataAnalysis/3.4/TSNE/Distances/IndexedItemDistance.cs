#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item("IndexedItemDistance", "A distance wrapper for indexed items")]
  internal class IndexedItemDistance<T> : DistanceBase<IndexedItem<T>> {
    [Storable]
    private readonly IDistance<T> dist;

    #region HLConstructors & Cloning
    [StorableConstructor]
    protected IndexedItemDistance(bool deserializing) : base(deserializing) { }
    protected IndexedItemDistance(IndexedItemDistance<T> original, Cloner cloner) : base(original, cloner) {
      dist = cloner.Clone(original.dist);
    }
    public override IDeepCloneable Clone(Cloner cloner) { return new IndexedItemDistance<T>(this, cloner); }
    public IndexedItemDistance(IDistance<T> distance) {
      dist = distance;
    }
    #endregion

    public override double Get(IndexedItem<T> a, IndexedItem<T> b) {
      return dist.Get(a.Value, b.Value);
    }
  }
}

