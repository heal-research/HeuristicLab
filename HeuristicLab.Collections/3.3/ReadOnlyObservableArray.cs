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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using HEAL.Fossil;

namespace HeuristicLab.Collections {
  [StorableType("DBE0FFE7-BCAD-49C2-9A55-5360EBE0FD86")]
  [Serializable]
  public class ReadOnlyObservableArray<T> : IObservableArray<T> {
    [Storable]
    protected IObservableArray<T> array;

    #region Properties
    public int Length {
      get { return array.Length; }
    }
    int ICollection<T>.Count {
      get { return array.Count; }
    }
    bool ICollection<T>.IsReadOnly {
      get { return true; }
    }

    public T this[int index] {
      get { return array[index]; }
    }
    T IList<T>.this[int index] {
      get { return array[index]; }
      set { throw new NotSupportedException(); }
    }
    #endregion

    #region Constructors
    protected ReadOnlyObservableArray() { }
    public ReadOnlyObservableArray(IObservableArray<T> array) {
      if (array == null) throw new ArgumentNullException();
      this.array = array;
      RegisterEvents();
    }
    [StorableConstructor]
    protected ReadOnlyObservableArray(StorableConstructorFlag _) { }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }
    #endregion

    #region Access
    public bool Contains(T item) {
      return array.Contains(item);
    }

    public int IndexOf(T item) {
      return array.IndexOf(item);
    }
    #endregion

    #region Manipulation
    void ICollection<T>.Add(T item) {
      throw new NotSupportedException();
    }

    void IList<T>.Insert(int index, T item) {
      throw new NotSupportedException();
    }

    bool ICollection<T>.Remove(T item) {
      throw new NotSupportedException();
    }
    void IList<T>.RemoveAt(int index) {
      throw new NotSupportedException();
    }

    void ICollection<T>.Clear() {
      throw new NotSupportedException();
    }

    void IObservableArray<T>.Reverse() {
      throw new NotSupportedException();
    }
    void IObservableArray<T>.Reverse(int index, int count) {
      throw new NotSupportedException();
    }
    #endregion

    #region Conversion
    public void CopyTo(T[] array, int arrayIndex) {
      this.array.CopyTo(array, arrayIndex);
    }
    #endregion

    #region Enumeration
    public IEnumerator<T> GetEnumerator() {
      return array.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator() {
      return array.GetEnumerator();
    }
    #endregion

    #region Events
    protected void RegisterEvents() {
      array.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<T>>(array_ItemsReplaced);
      array.ItemsMoved += new CollectionItemsChangedEventHandler<IndexedItem<T>>(array_ItemsMoved);
      array.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<T>>(array_CollectionReset);
      array.PropertyChanged += new PropertyChangedEventHandler(array_PropertyChanged);
    }

    [field: NonSerialized]
    public event CollectionItemsChangedEventHandler<IndexedItem<T>> ItemsReplaced;
    protected virtual void OnItemsReplaced(IEnumerable<IndexedItem<T>> items, IEnumerable<IndexedItem<T>> oldItems) {
      CollectionItemsChangedEventHandler<IndexedItem<T>> handler = ItemsReplaced;
      if (handler != null) handler(this, new CollectionItemsChangedEventArgs<IndexedItem<T>>(items, oldItems));
    }

    [field: NonSerialized]
    public event CollectionItemsChangedEventHandler<IndexedItem<T>> ItemsMoved;
    protected virtual void OnItemsMoved(IEnumerable<IndexedItem<T>> items, IEnumerable<IndexedItem<T>> oldItems) {
      CollectionItemsChangedEventHandler<IndexedItem<T>> handler = ItemsMoved;
      if (handler != null) handler(this, new CollectionItemsChangedEventArgs<IndexedItem<T>>(items, oldItems));
    }

    [field: NonSerialized]
    public event CollectionItemsChangedEventHandler<IndexedItem<T>> CollectionReset;
    protected virtual void OnCollectionReset(IEnumerable<IndexedItem<T>> items, IEnumerable<IndexedItem<T>> oldItems) {
      CollectionItemsChangedEventHandler<IndexedItem<T>> handler = CollectionReset;
      if (handler != null) handler(this, new CollectionItemsChangedEventArgs<IndexedItem<T>>(items, oldItems));
    }

    [field: NonSerialized]
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }

    private void array_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      OnItemsReplaced(e.Items, e.OldItems);
    }
    private void array_ItemsMoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      OnItemsMoved(e.Items, e.OldItems);
    }
    private void array_CollectionReset(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      OnCollectionReset(e.Items, e.OldItems);
    }
    private void array_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      if (e.PropertyName.Equals("Item[]") || e.PropertyName.Equals("Length"))
        OnPropertyChanged(e.PropertyName);
    }
    #endregion
  }
}
