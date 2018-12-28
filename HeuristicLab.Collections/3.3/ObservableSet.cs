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
using System.Linq;
using HEAL.Fossil;

namespace HeuristicLab.Collections {
  [StorableType("44FF4DAE-F308-4B3B-8475-0B0327619657")]
  [Serializable]
  public class ObservableSet<T> : IObservableSet<T> {
    [Storable]
    protected HashSet<T> set;

    #region Properties
    public IEqualityComparer<T> Comparer {
      get { return set.Comparer; }
    }
    public int Count {
      get { return set.Count; }
    }
    bool ICollection<T>.IsReadOnly {
      get { return ((ICollection<T>)set).IsReadOnly; }
    }
    #endregion

    #region Constructors
    public ObservableSet() {
      set = new HashSet<T>();
    }
    public ObservableSet(IEnumerable<T> collection) {
      set = new HashSet<T>(collection);
    }
    public ObservableSet(IEqualityComparer<T> comparer) {
      set = new HashSet<T>(comparer);
    }
    public ObservableSet(IEnumerable<T> collection, IEqualityComparer<T> comparer) {
      set = new HashSet<T>(collection, comparer);
    }
    [StorableConstructor]
    protected ObservableSet(StorableConstructorFlag _) { }
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
    public bool Add(T item) {
      if (set.Add(item)) {
        OnPropertyChanged("Count");
        OnItemsAdded(new T[] { item });
        return true;
      }
      return false;
    }
    void ICollection<T>.Add(T item) {
      Add(item);
    }

    public void ExceptWith(IEnumerable<T> other) {
      if (other == null) throw new ArgumentNullException();
      List<T> items = new List<T>();
      foreach (T item in other) {
        if (set.Remove(item))
          items.Add(item);
      }
      if (items.Count > 0) {
        OnPropertyChanged("Count");
        OnItemsRemoved(items);
      }
    }

    public void IntersectWith(IEnumerable<T> other) {
      if (other == null) throw new ArgumentNullException();
      HashSet<T> items = new HashSet<T>();
      foreach (T item in set) {
        if (!other.Contains(item)) items.Add(item);
      }
      if (items.Count > 0) {
        set.ExceptWith(items);
        OnPropertyChanged("Count");
        OnItemsRemoved(items);
      }
    }

    public bool Remove(T item) {
      if (set.Remove(item)) {
        OnPropertyChanged("Count");
        OnItemsRemoved(new T[] { item });
        return true;
      }
      return false;
    }
    public int RemoveWhere(Predicate<T> match) {
      if (match == null) throw new ArgumentNullException();
      HashSet<T> items = new HashSet<T>();
      foreach (T item in set) {
        if (match(item)) items.Add(item);
      }
      if (items.Count > 0) {
        set.ExceptWith(items);
        OnPropertyChanged("Count");
        OnItemsRemoved(items);
      }
      return items.Count;
    }

    public void SymmetricExceptWith(IEnumerable<T> other) {
      if (other == null) throw new ArgumentNullException();
      List<T> addedItems = new List<T>();
      List<T> removedItems = new List<T>();
      foreach (T item in other) {
        if (set.Contains(item)) {
          set.Remove(item);
          removedItems.Add(item);
        } else {
          set.Add(item);
          addedItems.Add(item);
        }
      }
      if ((addedItems.Count > 0) || (removedItems.Count > 0)) {
        OnPropertyChanged("Count");
        if (addedItems.Count > 0) OnItemsAdded(addedItems);
        if (removedItems.Count > 0) OnItemsRemoved(removedItems);
      }
    }

    public void UnionWith(IEnumerable<T> other) {
      if (other == null) throw new ArgumentNullException();
      List<T> items = new List<T>();
      foreach (T item in other) {
        if (set.Add(item)) {
          items.Add(item);
        }
      }
      if (items.Count > 0) {
        OnPropertyChanged("Count");
        OnItemsAdded(items);
      }
    }

    public void Clear() {
      if (set.Count > 0) {
        T[] items = new T[set.Count];
        set.CopyTo(items);
        set.Clear();
        OnPropertyChanged("Count");
        OnCollectionReset(new T[0], items);
      }
    }
    #endregion

    #region Conversion
    public ReadOnlyObservableSet<T> AsReadOnly() {
      return new ReadOnlyObservableSet<T>(this);
    }
    public void CopyTo(T[] array) {
      set.CopyTo(array);
    }
    public void CopyTo(T[] array, int arrayIndex) {
      set.CopyTo(array, arrayIndex);
    }
    public void CopyTo(T[] array, int arrayIndex, int count) {
      set.CopyTo(array, arrayIndex, count);
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

    #region Helpers
    public void TrimExcess() {
      set.TrimExcess();
    }
    #endregion

    #region Events
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
    #endregion
  }
}
