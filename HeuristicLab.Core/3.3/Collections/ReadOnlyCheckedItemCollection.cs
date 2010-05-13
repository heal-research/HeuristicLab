#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  [StorableClass]
  [Item("ReadOnlyCheckedItemCollection<T>", "Represents a read-only collection of checked items.")]
  public class ReadOnlyCheckedItemCollection<T> : ReadOnlyItemCollection<T>, ICheckedItemCollection<T> where T : class, IItem {
    private CheckedItemCollection<T> CheckedItemCollection {
      get { return (CheckedItemCollection<T>)base.collection; }
    }
    
    public ReadOnlyCheckedItemCollection() : base(new CheckedItemCollection<T>()) { }
    public ReadOnlyCheckedItemCollection(ICheckedItemCollection<T> collection)
      : base(collection) {
      collection.CheckedItemsChanged += new CollectionItemsChangedEventHandler<T>(collection_CheckedItemsChanged);
    }

    [StorableConstructor]
    protected ReadOnlyCheckedItemCollection(bool deserializing) : base(deserializing) { }

    #region ReadOnlyCheckedItemCollection<T> Members
    public event CollectionItemsChangedEventHandler<T> CheckedItemsChanged;
    protected virtual void collection_CheckedItemsChanged(object sender, CollectionItemsChangedEventArgs<T> e) {
      var handler = CheckedItemsChanged;
      if (handler != null)
        handler(this, e);
    }
    
    public IEnumerable<T> CheckedItems {
      get { return CheckedItemCollection.CheckedItems; }
    }

    public bool ItemChecked(T item) {
      return CheckedItemCollection.ItemChecked(item);
    }

    public void SetItemCheckedState(T item, bool checkedState) {
      CheckedItemCollection.SetItemCheckedState(item, checkedState);
    }

    public void Add(T item, bool checkedState) {
      throw new NotSupportedException();
    }
    #endregion
  }
}
