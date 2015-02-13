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
  public class ReadOnlyObservableKeyedCollection<TKey, TItem> : IObservableKeyedCollection<TKey, TItem> {
    [Storable]
    protected IObservableKeyedCollection<TKey, TItem> collection;

    #region Properties
    public int Count {
      get { return collection.Count; }
    }
    bool ICollection<TItem>.IsReadOnly {
      get { return true; }
    }

    public TItem this[TKey key] {
      get {
        return collection[key];
      }
    }
    #endregion

    #region Constructors
    protected ReadOnlyObservableKeyedCollection() { }
    public ReadOnlyObservableKeyedCollection(IObservableKeyedCollection<TKey, TItem> collection) {
      if (collection == null) throw new ArgumentNullException();
      this.collection = collection;
      RegisterEvents();
    }
    [StorableConstructor]
    protected ReadOnlyObservableKeyedCollection(bool deserializing) { }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }
    #endregion

    #region Access
    public bool ContainsKey(TKey key) {
      return collection.ContainsKey(key);
    }
    public bool Contains(TItem item) {
      return collection.Contains(item);
    }

    public bool TryGetValue(TKey key, out TItem item) {
      return collection.TryGetValue(key, out item);
    }
    #endregion

    #region Manipulation
    void ICollection<TItem>.Add(TItem item) {
      throw new NotSupportedException();
    }

    bool IObservableKeyedCollection<TKey, TItem>.Remove(TKey key) {
      throw new NotSupportedException();
    }
    bool ICollection<TItem>.Remove(TItem item) {
      throw new NotSupportedException();
    }

    void ICollection<TItem>.Clear() {
      throw new NotSupportedException();
    }
    #endregion

    #region Conversion
    public void CopyTo(TItem[] array, int arrayIndex) {
      collection.CopyTo(array, arrayIndex);
    }
    #endregion

    #region Enumeration
    public IEnumerator<TItem> GetEnumerator() {
      return ((IEnumerable<TItem>)collection).GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator() {
      return ((IEnumerable)collection).GetEnumerator();
    }
    #endregion

    #region Events
    protected void RegisterEvents() {
      collection.ItemsAdded += new CollectionItemsChangedEventHandler<TItem>(collection_ItemsAdded);
      collection.ItemsRemoved += new CollectionItemsChangedEventHandler<TItem>(collection_ItemsRemoved);
      collection.ItemsReplaced += new CollectionItemsChangedEventHandler<TItem>(collection_ItemsReplaced);
      collection.CollectionReset += new CollectionItemsChangedEventHandler<TItem>(collection_CollectionReset);
      collection.PropertyChanged += new PropertyChangedEventHandler(collection_PropertyChanged);
    }

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

    private void collection_ItemsAdded(object sender, CollectionItemsChangedEventArgs<TItem> e) {
      OnItemsAdded(e.Items);
    }
    private void collection_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<TItem> e) {
      OnItemsRemoved(e.Items);
    }
    private void collection_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<TItem> e) {
      OnItemsReplaced(e.Items, e.OldItems);
    }
    private void collection_CollectionReset(object sender, CollectionItemsChangedEventArgs<TItem> e) {
      OnCollectionReset(e.Items, e.OldItems);
    }
    private void collection_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      if (e.PropertyName.Equals("Item[]") || e.PropertyName.Equals("Count"))
        OnPropertyChanged(e.PropertyName);
    }
    #endregion
  }
}
