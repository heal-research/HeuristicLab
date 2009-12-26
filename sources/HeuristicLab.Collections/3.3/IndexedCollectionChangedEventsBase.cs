#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System.Runtime.Serialization;

namespace HeuristicLab.Collections {
  [Serializable]
  [EmptyStorableClass]
  public abstract class IndexedCollectionChangedEventsBase<T> : CollectionChangedEventsBase<T>, IIndexedCollectionChangedEvents<T> {
    [field: NonSerialized]
    public event CollectionItemsChangedEventHandler<T> ItemsMoved;
    protected virtual void OnItemsMoved(IEnumerable<T> items, IEnumerable<T> oldItems) {
      if (ItemsMoved != null)
        ItemsMoved(this, new CollectionItemsChangedEventArgs<T>(items, oldItems));
    }
  }
}
