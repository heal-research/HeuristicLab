#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Linq;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Collections {
  [StorableClass]
  [Serializable]
  public abstract class ObservableKeyedCollection<TKey, TItem> : IObservableKeyedCollection<TKey, TItem> {
    [Storable]
    protected Dictionary<TKey, TItem> dict;

    #region Properties
    public int Count {
      get { return dict.Count; }
    }
    public IEqualityComparer<TKey> Comparer {
      get { return dict.Comparer; }
    }
    bool ICollection<TItem>.IsReadOnly {
      get { return ((ICollection<KeyValuePair<TKey, TItem>>)dict).IsReadOnly; }
    }

    public TItem this[TKey key] {
      get {
        return dict[key];
      }
    }
    #endregion

    #region Constructors
    protected ObservableKeyedCollection() {
      dict = new Dictionary<TKey, TItem>();
    }
    protected ObservableKeyedCollection(int capacity) {
      dict = new Dictionary<TKey, TItem>(capacity);
    }
    protected ObservableKeyedCollection(IEqualityComparer<TKey> comparer) {
      dict = new Dictionary<TKey, TItem>(comparer);
    }
    protected ObservableKeyedCollection(IEnumerable<TItem> collection) {
      if (collection == null) throw new ArgumentNullException();
      dict = new Dictionary<TKey, TItem>();
      foreach (TItem item in collection)
        dict.Add(GetKeyForItem(item), item);
    }
    protected ObservableKeyedCollection(int capacity, IEqualityComparer<TKey> comparer) {
      dict = new Dictionary<TKey, TItem>(capacity, comparer);
    }
    protected ObservableKeyedCollection(IEnumerable<TItem> collection, IEqualityComparer<TKey> comparer) {
      if (collection == null) throw new ArgumentNullException();
      dict = new Dictionary<TKey, TItem>(comparer);
      foreach (TItem item in collection)
        dict.Add(GetKeyForItem(item), item);
    }
    [StorableConstructor]
    protected ObservableKeyedCollection(bool deserializing) { }
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
      if (!oldKeyFound) throw new ArgumentException("Item not found");
      dict.Remove(oldKey);
      dict.Add(GetKeyForItem(item), item);
      OnPropertyChanged("Item[]");
      OnItemsReplaced(new TItem[] { item }, new TItem[] { item });
    }

    #region Access
    public bool ContainsKey(TKey key) {
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
    public void Add(TItem item) {
      dict.Add(GetKeyForItem(item), item);
      OnPropertyChanged("Item[]");
      OnPropertyChanged("Count");
      OnItemsAdded(new TItem[] { item });
    }
    public void AddRange(IEnumerable<TItem> collection) {
      if (collection == null) throw new ArgumentNullException();
      bool empty = true;
      foreach (TItem item in collection) {
        dict.Add(GetKeyForItem(item), item);
        empty = false;
      }
      if (!empty) {
        OnPropertyChanged("Item[]");
        OnPropertyChanged("Count");
        OnItemsAdded(collection);
      }
    }

    public bool Remove(TKey key) {
      TItem item;
      if (TryGetValue(key, out item)) {
        dict.Remove(key);
        OnPropertyChanged("Item[]");
        OnPropertyChanged("Count");
        OnItemsRemoved(new TItem[] { item });
        return true;
      }
      return false;
    }
    public bool Remove(TItem item) {
      if (dict.Remove(GetKeyForItem(item))) {
        OnPropertyChanged("Item[]");
        OnPropertyChanged("Count");
        OnItemsRemoved(new TItem[] { item });
        return true;
      }
      return false;
    }
    public void RemoveRange(IEnumerable<TItem> collection) {
      if (collection == null) throw new ArgumentNullException();
      List<TItem> items = new List<TItem>();
      foreach (TItem item in collection) {
        if (dict.Remove(GetKeyForItem(item)))
          items.Add(item);
      }
      if (items.Count > 0) {
        OnPropertyChanged("Item[]");
        OnPropertyChanged("Count");
        OnItemsRemoved(items);
      }
    }
    public int RemoveAll(Predicate<TItem> match) {
      ICollection<TItem> items = FindAll(match);
      RemoveRange(items);
      return items.Count;
    }

    public void Clear() {
      if (dict.Count > 0) {
        TItem[] items = dict.Values.ToArray();
        dict.Clear();
        OnPropertyChanged("Item[]");
        OnPropertyChanged("Count");
        OnCollectionReset(new TItem[0], items);
      }
    }
    #endregion

    #region Conversion
    public ReadOnlyObservableKeyedCollection<TKey, TItem> AsReadOnly() {
      return new ReadOnlyObservableKeyedCollection<TKey, TItem>(this);
    }
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
      CollectionItemsChangedEventHandler<TItem> handler = ItemsAdded;
      if (handler != null) handler(this, new CollectionItemsChangedEventArgs<TItem>(items));
    }

    [field: NonSerialized]
    public event CollectionItemsChangedEventHandler<TItem> ItemsRemoved;
    protected virtual void OnItemsRemoved(IEnumerable<TItem> items) {
      CollectionItemsChangedEventHandler<TItem> handler = ItemsRemoved;
      if (handler != null) handler(this, new CollectionItemsChangedEventArgs<TItem>(items));
    }

    [field: NonSerialized]
    public event CollectionItemsChangedEventHandler<TItem> ItemsReplaced;
    protected virtual void OnItemsReplaced(IEnumerable<TItem> items, IEnumerable<TItem> oldItems) {
      CollectionItemsChangedEventHandler<TItem> handler = ItemsReplaced;
      if (handler != null) handler(this, new CollectionItemsChangedEventArgs<TItem>(items, oldItems));
    }

    [field: NonSerialized]
    public event CollectionItemsChangedEventHandler<TItem> CollectionReset;
    protected virtual void OnCollectionReset(IEnumerable<TItem> items, IEnumerable<TItem> oldItems) {
      CollectionItemsChangedEventHandler<TItem> handler = CollectionReset;
      if (handler != null) handler(this, new CollectionItemsChangedEventArgs<TItem>(items, oldItems));
    }

    [field: NonSerialized]
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
  }
}
