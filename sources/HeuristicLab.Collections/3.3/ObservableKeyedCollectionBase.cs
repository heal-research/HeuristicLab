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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Collections {
  [Serializable]
  public abstract class ObservableKeyedCollectionBase<TKey, TItem> : IObservableKeyedCollection<TKey, TItem> {
    [Storable]
    private Dictionary<TKey, TItem> dict;

    #region Properties
    public int Count {
      get { return dict.Count; }
    }
    public IEqualityComparer<TKey> Comparer {
      get { return dict.Comparer; }
    }
    public bool IsReadOnly {
      get { return ((ICollection<KeyValuePair<TKey, TItem>>)dict).IsReadOnly; }
    }

    public TItem this[TKey key] {
      get {
        return dict[key];
      }
    }
    #endregion

    #region Constructors
    protected ObservableKeyedCollectionBase() {
      dict = new Dictionary<TKey, TItem>();
    }
    protected ObservableKeyedCollectionBase(int capacity) {
      dict = new Dictionary<TKey, TItem>(capacity);
    }
    protected ObservableKeyedCollectionBase(IEqualityComparer<TKey> comparer) {
      dict = new Dictionary<TKey, TItem>(comparer);
    }
    protected ObservableKeyedCollectionBase(IEnumerable<TItem> collection) {
      if (collection == null) throw new ArgumentNullException();
      dict = new Dictionary<TKey, TItem>();
      foreach (TItem item in collection)
        AddItem(item);
    }
    protected ObservableKeyedCollectionBase(int capacity, IEqualityComparer<TKey> comparer) {
      dict = new Dictionary<TKey, TItem>(capacity, comparer);
    }
    protected ObservableKeyedCollectionBase(IEnumerable<TItem> collection, IEqualityComparer<TKey> comparer) {
      if (collection == null) throw new ArgumentNullException();
      dict = new Dictionary<TKey, TItem>(comparer);
      foreach (TItem item in collection)
        AddItem(item);
    }
    #endregion

    #region Destructors
    ~ObservableKeyedCollectionBase() {
      Dispose(false);
    }
    protected virtual void Dispose(bool disposing) {
      if (disposing) {
        Clear();
      }
    }
    public void Dispose() {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
    #endregion

    protected abstract TKey GetKeyForItem(TItem item);
    protected void UpdateItemKey(TItem item) {
      if (item == null) throw new ArgumentNullException();
      TKey oldKey = default(TKey);
      bool oldKeyFound = false;
      foreach (KeyValuePair<TKey, TItem> entry in dict) {
        if (entry.Value.Equals(item)) {
          oldKey = entry.Key;
          oldKeyFound = true;
          break;
        }
      }
      if (!oldKeyFound) throw new ArgumentException("item not found");
      dict.Remove(oldKey);
      dict.Add(GetKeyForItem(item), item);
      OnItemsReplaced(new TItem[] { item }, new TItem[] { item });
    }

    #region Access
    public bool Contains(TKey key) {
      return dict.ContainsKey(key);
    }
    public bool Contains(TItem item) {
      return dict.ContainsValue(item);
    }

    public bool TryGetValue(TKey key, out TItem item) {
      return dict.TryGetValue(key, out item);
    }

    public bool Exists(Predicate<TItem> match) {
      if (match == null) throw new ArgumentNullException();
      foreach (TItem item in dict.Values) {
        if (match(item)) return true;
      }
      return false;
    }

    public TItem Find(Predicate<TItem> match) {
      if (match == null) throw new ArgumentNullException();
      foreach (TItem item in dict.Values) {
        if (match(item)) return item;
      }
      return default(TItem);
    }
    public ICollection<TItem> FindAll(Predicate<TItem> match) {
      if (match == null) throw new ArgumentNullException();
      List<TItem> result = new List<TItem>();
      foreach (TItem item in dict.Values) {
        if (match(item)) result.Add(item);
      }
      return result;
    }
    #endregion

    #region Manipulation
    protected virtual void AddItem(TItem item) {
      dict.Add(GetKeyForItem(item), item);
    }
    public void Add(TItem item) {
      AddItem(item);
      OnItemsAdded(new TItem[] { item });
    }
    public void AddRange(IEnumerable<TItem> collection) {
      if (collection == null) throw new ArgumentNullException();
      foreach (TItem item in collection)
        AddItem(item);
      OnItemsAdded(collection);
    }

    protected virtual bool RemoveItem(TItem item) {
      return dict.Remove(GetKeyForItem(item));
    }
    public bool Remove(TKey key) {
      TItem item;
      if (TryGetValue(key, out item)) {
        RemoveItem(item);
        OnItemsRemoved(new TItem[] { item });
        return true;
      }
      return false;
    }
    public bool Remove(TItem item) {
      if (RemoveItem(item)) {
        OnItemsRemoved(new TItem[] { item });
        return true;
      }
      return false;
    }
    public void RemoveRange(IEnumerable<TItem> collection) {
      if (collection == null) throw new ArgumentNullException();
      List<TItem> items = new List<TItem>();
      foreach (TItem item in collection) {
        if (RemoveItem(item))
          items.Add(item);
      }
      if (items.Count > 0)
        OnItemsRemoved(items);
    }
    public int RemoveAll(Predicate<TItem> match) {
      ICollection<TItem> items = FindAll(match);
      RemoveRange(items);
      return items.Count;
    }

    protected virtual void ClearItems() {
      dict.Clear();
    }
    public void Clear() {
      TItem[] items = dict.Values.ToArray();
      ClearItems();
      OnCollectionReset(new TItem[0], items);
    }
    #endregion

    #region Conversion
    public TItem[] ToArray() {
      return dict.Values.ToArray();
    }
    public void CopyTo(TItem[] array, int arrayIndex) {
      dict.Values.CopyTo(array, arrayIndex);
    }
    public ICollection<TOutput> ConvertAll<TOutput>(Converter<TItem, TOutput> converter) {
      if (converter == null) throw new ArgumentNullException();
      List<TOutput> result = new List<TOutput>();
      foreach (TItem item in dict.Values)
        result.Add(converter(item));
      return result;
    }
    #endregion

    #region Processing
    public void ForEach(Action<TItem> action) {
      if (action == null) throw new ArgumentNullException();
      foreach (TItem item in dict.Values)
        action(item);
    }
    public bool TrueForAll(Predicate<TItem> match) {
      if (match == null) throw new ArgumentNullException();
      foreach (TItem item in dict.Values)
        if (! match(item)) return false;
      return true;
    }
    #endregion

    #region Enumeration
    public IEnumerator<TItem> GetEnumerator() {
      return dict.Values.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator() {
      return dict.Values.GetEnumerator();
    }
    #endregion

    #region Events
    [field: NonSerialized]
    public event CollectionItemsChangedEventHandler<TItem> ItemsAdded;
    protected virtual void OnItemsAdded(IEnumerable<TItem> items) {
      if (ItemsAdded != null)
        ItemsAdded(this, new CollectionItemsChangedEventArgs<TItem>(items));
    }

    [field: NonSerialized]
    public event CollectionItemsChangedEventHandler<TItem> ItemsRemoved;
    protected virtual void OnItemsRemoved(IEnumerable<TItem> items) {
      if (ItemsRemoved != null)
        ItemsRemoved(this, new CollectionItemsChangedEventArgs<TItem>(items));
    }

    [field: NonSerialized]
    public event CollectionItemsChangedEventHandler<TItem> ItemsReplaced;
    protected virtual void OnItemsReplaced(IEnumerable<TItem> items, IEnumerable<TItem> oldItems) {
      if (ItemsReplaced != null)
        ItemsReplaced(this, new CollectionItemsChangedEventArgs<TItem>(items, oldItems));
    }

    [field: NonSerialized]
    public event CollectionItemsChangedEventHandler<TItem> CollectionReset;
    protected virtual void OnCollectionReset(IEnumerable<TItem> items, IEnumerable<TItem> oldItems) {
      if (CollectionReset != null)
        CollectionReset(this, new CollectionItemsChangedEventArgs<TItem>(items, oldItems));
    }
    #endregion
  }
}
