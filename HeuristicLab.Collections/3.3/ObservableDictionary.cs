#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;

namespace HeuristicLab.Collections {
  [StorableType("EFAF5129-39F0-4477-834E-E22B85D63BCF")]
  [Serializable]
  public class ObservableDictionary<TKey, TValue> : IObservableDictionary<TKey, TValue> {
    [Storable]
    protected Dictionary<TKey, TValue> dict;

    #region Properties
    public ICollection<TKey> Keys {
      get { return dict.Keys; }
    }
    public ICollection<TValue> Values {
      get { return dict.Values; }
    }
    public int Count {
      get { return dict.Count; }
    }
    public IEqualityComparer<TKey> Comparer {
      get { return dict.Comparer; }
    }
    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly {
      get { return ((ICollection<KeyValuePair<TKey, TValue>>)dict).IsReadOnly; }
    }

    public TValue this[TKey key] {
      get {
        return dict[key];
      }
      set {
        if (dict.ContainsKey(key)) {
          KeyValuePair<TKey, TValue> item = new KeyValuePair<TKey, TValue>(key, dict[key]);
          dict[key] = value;
          OnItemsReplaced(new KeyValuePair<TKey, TValue>[] { new KeyValuePair<TKey, TValue>(key, value) }, new KeyValuePair<TKey, TValue>[] { item });
        } else {
          dict[key] = value;
          OnItemsAdded(new KeyValuePair<TKey, TValue>[] { new KeyValuePair<TKey, TValue>(key, value) });
        }
      }
    }
    #endregion

    #region Constructors
    public ObservableDictionary() {
      dict = new Dictionary<TKey, TValue>();
    }
    public ObservableDictionary(int capacity) {
      dict = new Dictionary<TKey, TValue>(capacity);
    }
    public ObservableDictionary(IEqualityComparer<TKey> comparer) {
      dict = new Dictionary<TKey, TValue>(comparer);
    }
    public ObservableDictionary(IDictionary<TKey, TValue> dictionary) {
      dict = new Dictionary<TKey, TValue>(dictionary);
    }
    public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer) {
      dict = new Dictionary<TKey, TValue>(capacity, comparer);
    }
    public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) {
      dict = new Dictionary<TKey, TValue>(dictionary, comparer);
    }
    [StorableConstructor]
    protected ObservableDictionary(StorableConstructorFlag _) { }
    #endregion

    #region Access
    public bool ContainsKey(TKey key) {
      return dict.ContainsKey(key);
    }
    public bool ContainsValue(TValue value) {
      return dict.ContainsValue(value);
    }
    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) {
      return dict.Contains(item);
    }

    public bool TryGetValue(TKey key, out TValue value) {
      return dict.TryGetValue(key, out value);
    }
    #endregion

    #region Manipulation
    public void Add(TKey key, TValue value) {
      dict.Add(key, value);
      OnPropertyChanged("Item[]");
      OnPropertyChanged("Keys");
      OnPropertyChanged("Values");
      OnPropertyChanged("Count");
      OnItemsAdded(new KeyValuePair<TKey, TValue>[] { new KeyValuePair<TKey, TValue>(key, value) });
    }
    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) {
      ((ICollection<KeyValuePair<TKey, TValue>>)dict).Add(item);
      OnPropertyChanged("Item[]");
      OnPropertyChanged("Keys");
      OnPropertyChanged("Values");
      OnPropertyChanged("Count");
      OnItemsAdded(new KeyValuePair<TKey, TValue>[] { item });
    }

    public bool Remove(TKey key) {
      TValue value;
      if (dict.TryGetValue(key, out value)) {
        dict.Remove(key);
        OnPropertyChanged("Item[]");
        OnPropertyChanged("Keys");
        OnPropertyChanged("Values");
        OnPropertyChanged("Count");
        OnItemsRemoved(new KeyValuePair<TKey, TValue>[] { new KeyValuePair<TKey, TValue>(key, value) });
        return true;
      }
      return false;
    }
    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) {
      if (((ICollection<KeyValuePair<TKey, TValue>>)dict).Remove(item)) {
        OnPropertyChanged("Item[]");
        OnPropertyChanged("Keys");
        OnPropertyChanged("Values");
        OnPropertyChanged("Count");
        OnItemsRemoved(new KeyValuePair<TKey, TValue>[] { item });
        return true;
      }
      return false;
    }

    public void Clear() {
      if (dict.Count > 0) {
        KeyValuePair<TKey, TValue>[] items = dict.ToArray();
        dict.Clear();
        OnPropertyChanged("Item[]");
        OnPropertyChanged("Keys");
        OnPropertyChanged("Values");
        OnPropertyChanged("Count");
        OnCollectionReset(new KeyValuePair<TKey, TValue>[0], items);
      }
    }
    #endregion

    #region Conversion
    public ReadOnlyObservableDictionary<TKey, TValue> AsReadOnly() {
      return new ReadOnlyObservableDictionary<TKey, TValue>(this);
    }
    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
      ((ICollection<KeyValuePair<TKey, TValue>>)dict).CopyTo(array, arrayIndex);
    }
    #endregion

    #region Enumeration
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
      return dict.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator() {
      return dict.GetEnumerator();
    }
    #endregion

    #region Events
    [field: NonSerialized]
    public event CollectionItemsChangedEventHandler<KeyValuePair<TKey, TValue>> ItemsAdded;
    protected virtual void OnItemsAdded(IEnumerable<KeyValuePair<TKey, TValue>> items) {
      CollectionItemsChangedEventHandler<KeyValuePair<TKey, TValue>> handler = ItemsAdded;
      if (handler != null) handler(this, new CollectionItemsChangedEventArgs<KeyValuePair<TKey, TValue>>(items));
    }

    [field: NonSerialized]
    public event CollectionItemsChangedEventHandler<KeyValuePair<TKey, TValue>> ItemsRemoved;
    protected virtual void OnItemsRemoved(IEnumerable<KeyValuePair<TKey, TValue>> items) {
      CollectionItemsChangedEventHandler<KeyValuePair<TKey, TValue>> handler = ItemsRemoved;
      if (handler != null) handler(this, new CollectionItemsChangedEventArgs<KeyValuePair<TKey, TValue>>(items));
    }

    [field: NonSerialized]
    public event CollectionItemsChangedEventHandler<KeyValuePair<TKey, TValue>> ItemsReplaced;
    protected virtual void OnItemsReplaced(IEnumerable<KeyValuePair<TKey, TValue>> items, IEnumerable<KeyValuePair<TKey, TValue>> oldItems) {
      CollectionItemsChangedEventHandler<KeyValuePair<TKey, TValue>> handler = ItemsReplaced;
      if (handler != null) handler(this, new CollectionItemsChangedEventArgs<KeyValuePair<TKey, TValue>>(items, oldItems));
    }

    [field: NonSerialized]
    public event CollectionItemsChangedEventHandler<KeyValuePair<TKey, TValue>> CollectionReset;
    protected virtual void OnCollectionReset(IEnumerable<KeyValuePair<TKey, TValue>> items, IEnumerable<KeyValuePair<TKey, TValue>> oldItems) {
      CollectionItemsChangedEventHandler<KeyValuePair<TKey, TValue>> handler = CollectionReset;
      if (handler != null) handler(this, new CollectionItemsChangedEventArgs<KeyValuePair<TKey, TValue>>(items, oldItems));
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
