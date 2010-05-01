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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Drawing;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Collections;

namespace HeuristicLab.Core {
  [StorableClass]
  [Item("CheckedItemList<T>", "Represents a list of items that can be checked or unchecked.")]
  public class CheckedItemList<T> : ItemList<T>, ICheckedItemList<T> where T : class, IItem {
    [Storable]
    private Dictionary<T, bool> checkedState;

    public IEnumerable<IndexedItem<T>> CheckedItems {
      get {
        return from i in Enumerable.Range(0, list.Count)
               where ItemChecked(i)
               select new IndexedItem<T>(i, list[i]);
      }
    }
    public CheckedItemList()
      : base() {
      checkedState = new Dictionary<T, bool>();
    }
    public CheckedItemList(int capacity)
      : base(capacity) {
      checkedState = new Dictionary<T, bool>();
    }
    public CheckedItemList(IEnumerable<T> collection)
      : base(collection) {
      checkedState = new Dictionary<T, bool>();
      foreach (var item in collection) {
        if (!checkedState.ContainsKey(item))
          checkedState.Add(item, true);
      }
    }
    [StorableConstructor]
    protected CheckedItemList(bool deserializing) : base(deserializing) { }

    public bool ItemChecked(T item) {
      return checkedState[item];
    }

    public bool ItemChecked(int itemIndex) {
      return ItemChecked(this[itemIndex]);
    }

    public void SetItemCheckedState(T item, bool checkedState) {
      if (!this.checkedState.ContainsKey(item)) throw new ArgumentException();
      if (this.checkedState[item] != checkedState) {
        this.checkedState[item] = checkedState;
        OnCheckedItemsChanged(new IndexedItem<T>[] { new IndexedItem<T>(IndexOf(item), item) });
      }
    }

    public void SetItemCheckedState(int itemIndex, bool checkedState) {
      SetItemCheckedState(this[itemIndex], checkedState);
    }

    public void Add(T item, bool checkedState) {
      Add(item);
      SetItemCheckedState(item, checkedState);
    }

    public void Insert(int index, T item, bool checkedState) {
      Insert(index, item);
      SetItemCheckedState(item, checkedState);
    }

    protected override void OnCollectionReset(IEnumerable<IndexedItem<T>> items, IEnumerable<IndexedItem<T>> oldItems) {
      foreach (var oldIndexedItem in oldItems) {
        checkedState.Remove(oldIndexedItem.Value);
      }
      foreach (var indexedItem in items) {
        if (!checkedState.ContainsKey(indexedItem.Value))
          checkedState.Add(indexedItem.Value, true);
      }
      base.OnCollectionReset(items, oldItems);
    }

    protected override void OnItemsAdded(IEnumerable<IndexedItem<T>> items) {
      foreach (var indexedItem in items)
        if (!checkedState.ContainsKey(indexedItem.Value))
          checkedState.Add(indexedItem.Value, true);
      base.OnItemsAdded(items);
    }

    protected override void OnItemsRemoved(IEnumerable<IndexedItem<T>> items) {
      foreach (var indexedItem in items)
        checkedState.Remove(indexedItem.Value);
      base.OnItemsRemoved(items);
    }

    protected override void OnItemsReplaced(IEnumerable<IndexedItem<T>> items, IEnumerable<IndexedItem<T>> oldItems) {
      foreach (var oldIndexedItem in oldItems)
        checkedState.Remove(oldIndexedItem.Value);
      foreach (var indexedItem in items)
        if (!checkedState.ContainsKey(indexedItem.Value))
          checkedState.Add(indexedItem.Value, true);
      base.OnItemsReplaced(items, oldItems);
    }

    protected virtual void OnCheckedItemsChanged(IEnumerable<IndexedItem<T>> items) {
      RaiseCheckedItemsChanged(new CollectionItemsChangedEventArgs<IndexedItem<T>>(items));
    }

    public event CollectionItemsChangedEventHandler<IndexedItem<T>> CheckedItemsChanged;
    private void RaiseCheckedItemsChanged(CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      var handler = CheckedItemsChanged;
      if (handler != null) handler(this, e);
    }

    public object Clone() {
      return Clone(new Cloner());
    }
    public virtual IDeepCloneable Clone(Cloner cloner) {
      CheckedItemList<T> clone = (CheckedItemList<T>)Activator.CreateInstance(this.GetType());
      cloner.RegisterClonedObject(this, clone);
      clone.list = new List<T>(this.Select(x => (T)cloner.Clone(x)));
      clone.checkedState = new Dictionary<T, bool>();
      foreach (var pair in checkedState)
        clone.checkedState.Add((T)cloner.Clone(pair.Key), pair.Value);
      return clone;
    }
  }
}
