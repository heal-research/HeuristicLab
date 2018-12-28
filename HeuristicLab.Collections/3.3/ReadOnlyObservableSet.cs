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
  [StorableType("CB2FD087-C3B7-499D-9188-AF5B0D4FFEDB")]
  [Serializable]
  public class ReadOnlyObservableSet<T> : IObservableSet<T> {
    [Storable]
    protected IObservableSet<T> set;

    #region Properties
    public int Count {
      get { return set.Count; }
    }
    bool ICollection<T>.IsReadOnly {
      get { return true; }
    }
    #endregion

    #region Constructors
    protected ReadOnlyObservableSet() { }
    public ReadOnlyObservableSet(IObservableSet<T> set) {
      if (set == null) throw new ArgumentNullException();
      this.set = set;
      RegisterEvents();
    }
    [StorableConstructor]
    protected ReadOnlyObservableSet(StorableConstructorFlag _) { }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }
    #endregion

    #region Access
    public bool Contains(T item) {
      return set.Contains(item);
    }

    public bool IsProperSubsetOf(IEnumerable<T> other) {
      return set.IsProperSubsetOf(other);
    }
    public bool IsProperSupersetOf(IEnumerable<T> other) {
      return set.IsProperSupersetOf(other);
    }

    public bool IsSubsetOf(IEnumerable<T> other) {
      return set.IsSubsetOf(other);
    }
    public bool IsSupersetOf(IEnumerable<T> other) {
      return set.IsSupersetOf(other);
    }

    public bool Overlaps(IEnumerable<T> other) {
      return set.Overlaps(other);
    }

    public bool SetEquals(IEnumerable<T> other) {
      return set.SetEquals(other);
    }
    #endregion

    #region Manipulation
    bool IObservableSet<T>.Add(T item) {
      throw new NotSupportedException();
    }
    void ICollection<T>.Add(T item) {
      throw new NotSupportedException();
    }

    void IObservableSet<T>.ExceptWith(IEnumerable<T> other) {
      throw new NotSupportedException();
    }

    void IObservableSet<T>.IntersectWith(IEnumerable<T> other) {
      throw new NotSupportedException();
    }

    bool ICollection<T>.Remove(T item) {
      throw new NotSupportedException();
    }

    void IObservableSet<T>.SymmetricExceptWith(IEnumerable<T> other) {
      throw new NotSupportedException();
    }

    void IObservableSet<T>.UnionWith(IEnumerable<T> other) {
      throw new NotSupportedException();
    }

    void ICollection<T>.Clear() {
      throw new NotSupportedException();
    }
    #endregion

    #region Conversion
    public void CopyTo(T[] array, int arrayIndex) {
      set.CopyTo(array, arrayIndex);
    }
    #endregion

    #region Enumeration
    public IEnumerator<T> GetEnumerator() {
      return set.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator() {
      return set.GetEnumerator();
    }
    #endregion

    #region Events
    protected void RegisterEvents() {
      set.ItemsAdded += new CollectionItemsChangedEventHandler<T>(set_ItemsAdded);
      set.ItemsRemoved += new CollectionItemsChangedEventHandler<T>(set_ItemsRemoved);
      set.CollectionReset += new CollectionItemsChangedEventHandler<T>(set_CollectionReset);
      set.PropertyChanged += new PropertyChangedEventHandler(set_PropertyChanged);
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

    private void set_ItemsAdded(object sender, CollectionItemsChangedEventArgs<T> e) {
      OnItemsAdded(e.Items);
    }
    private void set_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<T> e) {
      OnItemsRemoved(e.Items);
    }
    private void set_CollectionReset(object sender, CollectionItemsChangedEventArgs<T> e) {
      OnCollectionReset(e.Items, e.OldItems);
    }
    private void set_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      if (e.PropertyName.Equals("Count"))
        OnPropertyChanged(e.PropertyName);
    }
    #endregion
  }
}
