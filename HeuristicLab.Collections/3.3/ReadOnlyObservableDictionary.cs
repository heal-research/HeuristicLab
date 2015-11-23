#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Collections {
  [StorableClass]
  [Serializable]
  public class ReadOnlyObservableDictionary<TKey, TValue> : IObservableDictionary<TKey, TValue> {
    [Storable]
    protected IObservableDictionary<TKey, TValue> dict;

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
    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly {
      get { return true; }
    }

    public TValue this[TKey key] {
      get { return dict[key]; }
    }
    TValue IDictionary<TKey, TValue>.this[TKey key] {
      get { return dict[key]; }
      set { throw new NotSupportedException(); }
    }
    #endregion

    #region Constructors
    protected ReadOnlyObservableDictionary() { }
    public ReadOnlyObservableDictionary(IObservableDictionary<TKey, TValue> dictionary) {
      if (dictionary == null) throw new ArgumentNullException();
      dict = dictionary;
      RegisterEvents();
    }
    [StorableConstructor]
    protected ReadOnlyObservableDictionary(bool deserializing) { }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }
    #endregion

    #region Access
    public bool ContainsKey(TKey key) {
      return dict.ContainsKey(key);
    }
    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) {
      return dict.Contains(item);
    }

    public bool TryGetValue(TKey key, out TValue value) {
      return dict.TryGetValue(key, out value);
    }
    #endregion

    #region Manipulation
    void IDictionary<TKey, TValue>.Add(TKey key, TValue value) {
      throw new NotSupportedException();
    }
    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) {
      throw new NotSupportedException();
    }

    bool IDictionary<TKey, TValue>.Remove(TKey key) {
      throw new NotSupportedException();
    }
    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) {
      throw new NotSupportedException();
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Clear() {
      throw new NotSupportedException();
    }
    #endregion

    #region Conversion
    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
      ((ICollection<KeyValuePair<TKey, TValue>>)dict).CopyTo(array, arrayIndex);
    }
    #endregion

    #region Enumeration
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
      return dict.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator() {
      return ((IEnumerable)dict).GetEnumerator();
    }
    #endregion

    #region Events
    protected void RegisterEvents() {
      dict.ItemsAdded += new CollectionItemsChangedEventHandler<KeyValuePair<TKey, TValue>>(dict_ItemsAdded);
      dict.ItemsRemoved += new CollectionItemsChangedEventHandler<KeyValuePair<TKey, TValue>>(dict_ItemsRemoved);
      dict.ItemsReplaced += new CollectionItemsChangedEventHandler<KeyValuePair<TKey, TValue>>(dict_ItemsReplaced);
      dict.CollectionReset += new CollectionItemsChangedEventHandler<KeyValuePair<TKey, TValue>>(dict_CollectionReset);
      dict.PropertyChanged += new PropertyChangedEventHandler(dict_PropertyChanged);
    }

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

    private void dict_ItemsAdded(object sender, CollectionItemsChangedEventArgs<KeyValuePair<TKey, TValue>> e) {
      OnItemsAdded(e.Items);
    }
    private void dict_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<KeyValuePair<TKey, TValue>> e) {
      OnItemsRemoved(e.Items);
    }
    private void dict_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<KeyValuePair<TKey, TValue>> e) {
      OnItemsReplaced(e.Items, e.OldItems);
    }
    private void dict_CollectionReset(object sender, CollectionItemsChangedEventArgs<KeyValuePair<TKey, TValue>> e) {
      OnCollectionReset(e.Items, e.OldItems);
    }
    private void dict_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      if (e.PropertyName.Equals("Item[]") || e.PropertyName.Equals("Keys") || e.PropertyName.Equals("Values") || e.PropertyName.Equals("Count"))
        OnPropertyChanged(e.PropertyName);
    }
    #endregion
  }
}
