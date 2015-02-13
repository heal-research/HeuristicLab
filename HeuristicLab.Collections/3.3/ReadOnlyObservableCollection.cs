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
  public class ReadOnlyObservableCollection<T> : IObservableCollection<T> {
    [Storable]
    protected IObservableCollection<T> collection;

    #region Properties
    public int Count {
      get { return collection.Count; }
    }
    bool ICollection<T>.IsReadOnly {
      get { return true; }
    }
    #endregion

    #region Constructors
    protected ReadOnlyObservableCollection() { }
    public ReadOnlyObservableCollection(IObservableCollection<T> collection) {
      if (collection == null) throw new ArgumentNullException();
      this.collection = collection;
      RegisterEvents();
    }
    [StorableConstructor]
    protected ReadOnlyObservableCollection(bool deserializing) { }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }
    #endregion

    #region Access
    public bool Contains(T item) {
      return collection.Contains(item);
    }
    #endregion

    #region Manipulation
    void ICollection<T>.Add(T item) {
      throw new NotSupportedException();
    }

    bool ICollection<T>.Remove(T item) {
      throw new NotSupportedException();
    }

    void ICollection<T>.Clear() {
      throw new NotSupportedException();
    }
    #endregion

    #region Conversion
    public void CopyTo(T[] array, int arrayIndex) {
      collection.CopyTo(array, arrayIndex);
    }
    #endregion

    #region Enumeration
    public IEnumerator<T> GetEnumerator() {
      return ((IEnumerable<T>)collection).GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator() {
      return ((IEnumerable)collection).GetEnumerator();
    }
    #endregion

    #region Events
    protected void RegisterEvents() {
      collection.ItemsAdded += new CollectionItemsChangedEventHandler<T>(collection_ItemsAdded);
      collection.ItemsRemoved += new CollectionItemsChangedEventHandler<T>(collection_ItemsRemoved);
      collection.CollectionReset += new CollectionItemsChangedEventHandler<T>(collection_CollectionReset);
      collection.PropertyChanged += new PropertyChangedEventHandler(collection_PropertyChanged);
    }


    [field: NonSerialized]
    public event CollectionItemsChangedEventHandler<T> ItemsAdded;
    protected virtual void OnItemsAdded(IEnumerable<T> items) {
      CollectionItemsChangedEventHandler<T> handler = ItemsAdded;
      if (handler != null) handler(this, new CollectionItemsChangedEventArgs<T>(items));
    }

    [field: NonSerialized]
    public event CollectionItemsChangedEventHandler<T> ItemsRemoved;
    protected virtual void OnItemsRemoved(IEnumerable<T> items) {
      CollectionItemsChangedEventHandler<T> handler = ItemsRemoved;
      if (handler != null) handler(this, new CollectionItemsChangedEventArgs<T>(items));
    }

    [field: NonSerialized]
    public event CollectionItemsChangedEventHandler<T> CollectionReset;
    protected virtual void OnCollectionReset(IEnumerable<T> items, IEnumerable<T> oldItems) {
      CollectionItemsChangedEventHandler<T> handler = CollectionReset;
      if (handler != null) handler(this, new CollectionItemsChangedEventArgs<T>(items, oldItems));
    }

    [field: NonSerialized]
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }

    private void collection_ItemsAdded(object sender, CollectionItemsChangedEventArgs<T> e) {
      OnItemsAdded(e.Items);
    }
    private void collection_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<T> e) {
      OnItemsRemoved(e.Items);
    }
    private void collection_CollectionReset(object sender, CollectionItemsChangedEventArgs<T> e) {
      OnCollectionReset(e.Items, e.OldItems);
    }
    private void collection_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      if (e.PropertyName.Equals("Count"))
        OnPropertyChanged(e.PropertyName);
    }
    #endregion
  }
}
