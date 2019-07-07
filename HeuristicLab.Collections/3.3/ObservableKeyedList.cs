#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;

namespace HeuristicLab.Collections {
  [Serializable]
  [StorableType("47F1D1D1-DF32-4C23-8D4F-0B36A6E42AF7")]
  public abstract class ObservableKeyedList<TKey, TItem> : ObservableList<TItem>, IObservableKeyedList<TKey, TItem> {

    private Dictionary<TKey, TItem> dict;
    [Storable]
    protected Dictionary<TKey, TItem> Dictionary {
      get { return dict; }
      private set { dict = value; }
    }
    protected IEqualityComparer<TKey> Comparer {
      get { return dict.Comparer; }
    }

    public TItem this[TKey key] {
      get { return dict[key]; }
    }

    #region constructors
    protected ObservableKeyedList()
      : base() {
      dict = new Dictionary<TKey, TItem>();
    }
    protected ObservableKeyedList(int capacity)
      : base(capacity) {
      dict = new Dictionary<TKey, TItem>(capacity);
    }
    protected ObservableKeyedList(IEqualityComparer<TKey> comparer)
      : base() {
      dict = new Dictionary<TKey, TItem>(comparer);
    }
    protected ObservableKeyedList(int capacity, IEqualityComparer<TKey> comparer)
      : base(capacity) {
      dict = new Dictionary<TKey, TItem>(capacity, comparer);
    }
    [StorableConstructor]
    protected ObservableKeyedList(StorableConstructorFlag _) : base(_) { }
    #endregion

    protected abstract TKey GetKeyForItem(TItem item);
    protected void UpdateItemKey(TItem item) {
      if (item == null) throw new ArgumentNullException();
      var dictionaryItem = dict.FirstOrDefault(i => i.Value.Equals(item));
      if (dictionaryItem.Equals(default(KeyValuePair<TKey, TItem>))) throw new ArgumentException("Item not found");

      dict.Remove(dictionaryItem.Key);
      dict.Add(GetKeyForItem(item), item);
      int index = IndexOf(item);
      OnItemsReplaced(new[] { new IndexedItem<TItem>(index, item) }, new[] { new IndexedItem<TItem>(index, item) });
      OnItemsReplaced(new[] { item }, new[] { item });
      OnPropertyChanged("Item[]");
    }

    #region methods
    public new ReadOnlyObservableKeyedList<TKey, TItem> AsReadOnly() {
      return new ReadOnlyObservableKeyedList<TKey, TItem>(this);
    }

    public bool ContainsKey(TKey key) {
      return dict.ContainsKey(key);
    }
    public new bool Contains(TItem item) {
      TKey key = GetKeyForItem(item);
      return ContainsKey(key);
    }

    public bool TryGetValue(TKey key, out TItem item) {
      return dict.TryGetValue(key, out item);
    }

    public bool Remove(TKey key) {
      TItem item;
      if (!TryGetValue(key, out item)) return false;
      return base.Remove(item);
    }
    #endregion


    #region Events
    [field: NonSerialized]
    private event CollectionItemsChangedEventHandler<TItem> itemsReplaced;
    event CollectionItemsChangedEventHandler<TItem> INotifyObservableKeyedCollectionItemsChanged<TKey, TItem>.ItemsReplaced {
      add { itemsReplaced += value; }
      remove { itemsReplaced -= value; }
    }
    private void OnItemsReplaced(IEnumerable<TItem> items, IEnumerable<TItem> oldItems) {
      var handler = itemsReplaced;
      if (handler != null) handler(this, new CollectionItemsChangedEventArgs<TItem>(items, oldItems));
    }

    protected override void OnItemsAdded(IEnumerable<IndexedItem<TItem>> items) {
      UpdateDictionary(items, Enumerable.Empty<IndexedItem<TItem>>());
      base.OnItemsAdded(items);
    }

    protected override void OnItemsRemoved(IEnumerable<IndexedItem<TItem>> items) {
      UpdateDictionary(Enumerable.Empty<IndexedItem<TItem>>(), items);
      base.OnItemsRemoved(items);
    }

    protected override void OnItemsReplaced(IEnumerable<IndexedItem<TItem>> items, IEnumerable<IndexedItem<TItem>> oldItems) {
      UpdateDictionary(items, oldItems);
      base.OnItemsReplaced(items, oldItems);
    }

    protected override void OnCollectionReset(IEnumerable<IndexedItem<TItem>> items, IEnumerable<IndexedItem<TItem>> oldItems) {
      UpdateDictionary(items, oldItems);
      base.OnCollectionReset(items, oldItems);
    }
    #endregion

    #region helper methods
    private void UpdateDictionary(IEnumerable<IndexedItem<TItem>> items, IEnumerable<IndexedItem<TItem>> oldItems) {
      foreach (var item in oldItems)
        dict.Remove(GetKeyForItem(item.Value));

      bool duplicateKeyFound = false;
      foreach (var item in items) {
        TKey key = GetKeyForItem(item.Value);
        if (dict.ContainsKey(key)) {
          duplicateKeyFound = true;
          break;
        }
        dict.Add(key, item.Value);
      }

      //restore old state of list and dictionary
      if (duplicateKeyFound) {
        foreach (var item in items.OrderByDescending(i => i.Index)) {
          list.RemoveAt(item.Index);
        }
        foreach (var item in items.Select(i => i.Value).Except(list)) {
          TKey key = GetKeyForItem(item);
          dict.Remove(key);
        }
        foreach (var item in oldItems.OrderByDescending(i => i.Index)) {
          list.Insert(item.Index, item.Value);
          TKey key = GetKeyForItem(item.Value);
          dict.Add(key, item.Value);
        }
        throw new ArgumentException("An element with the same key already exists.");
      }
    }
    #endregion
  }
}
