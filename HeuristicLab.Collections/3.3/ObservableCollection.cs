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

using HEAL.Attic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace HeuristicLab.Collections {
  [StorableType("98A1FFF8-88D8-4A1A-BDAA-BDC3F4663C1F")]
  [Serializable]
  public class ObservableCollection<T> : IObservableCollection<T> {
    [Storable]
    protected List<T> list;

    #region Properties
    public int Capacity {
      get { return list.Capacity; }
      set {
        if (list.Capacity != value) {
          list.Capacity = value;
          OnPropertyChanged("Capacity");
        }
      }
    }
    public int Count {
      get { return list.Count; }
    }
    bool ICollection<T>.IsReadOnly {
      get { return ((ICollection<T>)list).IsReadOnly; }
    }
    #endregion

    #region Constructors
    public ObservableCollection() {
      list = new List<T>();
    }
    public ObservableCollection(int capacity) {
      list = new List<T>(capacity);
    }
    public ObservableCollection(IEnumerable<T> collection) {
      list = new List<T>(collection);
    }
    [StorableConstructor]
    protected ObservableCollection(StorableConstructorFlag _) { }
    #endregion

    #region Access
    public bool Contains(T item) {
      return list.Contains(item);
    }

    public bool Exists(Predicate<T> match) {
      return list.Exists(match);
    }

    public T Find(Predicate<T> match) {
      return list.Find(match);
    }
    public ICollection<T> FindAll(Predicate<T> match) {
      return list.FindAll(match);
    }
    public T FindLast(Predicate<T> match) {
      return list.FindLast(match);
    }
    #endregion

    #region Manipulation
    public void Add(T item) {
      int capacity = list.Capacity;
      list.Add(item);
      if (list.Capacity != capacity)
        OnPropertyChanged("Capacity");
      OnPropertyChanged("Count");
      OnItemsAdded(new T[] { item });
    }
    public void AddRange(IEnumerable<T> collection) {
      int capacity = list.Capacity;
      ICollection<T> items = collection as ICollection<T> ?? collection.ToList();
      list.AddRange(items);
      if (items.Count > 0) {
        if (list.Capacity != capacity)
          OnPropertyChanged("Capacity");
        OnPropertyChanged("Count");
        OnItemsAdded(items);
      }
    }

    public bool Remove(T item) {
      if (list.Remove(item)) {
        OnPropertyChanged("Count");
        OnItemsRemoved(new T[] { item });
        return true;
      }
      return false;
    }
    public void RemoveRange(IEnumerable<T> collection) {
      if (collection == null) throw new ArgumentNullException();
      List<T> items = new List<T>();
      foreach (T item in collection) {
        if (list.Remove(item))
          items.Add(item);
      }
      if (items.Count > 0) {
        OnPropertyChanged("Count");
        OnItemsRemoved(items);
      }
    }
    public int RemoveAll(Predicate<T> match) {
      List<T> items = list.FindAll(match);
      int result = 0;
      if (items.Count > 0) {
        result = list.RemoveAll(match);
        OnPropertyChanged("Count");
        OnItemsRemoved(items);
      }
      return result;
    }

    public void Clear() {
      if (list.Count > 0) {
        T[] items = list.ToArray();
        list.Clear();
        OnPropertyChanged("Count");
        OnCollectionReset(new T[0], items);
      }
    }
    #endregion

    #region Conversion
    public ReadOnlyObservableCollection<T> AsReadOnly() {
      return new ReadOnlyObservableCollection<T>(this);
    }
    public T[] ToArray() {
      return list.ToArray();
    }
    public void CopyTo(T[] array, int arrayIndex) {
      list.CopyTo(array, arrayIndex);
    }
    public ICollection<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) {
      return list.ConvertAll<TOutput>(converter);
    }
    #endregion

    #region Processing
    public void ForEach(Action<T> action) {
      list.ForEach(action);
    }
    public bool TrueForAll(Predicate<T> match) {
      return list.TrueForAll(match);
    }
    #endregion

    #region Enumeration
    public IEnumerator<T> GetEnumerator() {
      return ((IEnumerable<T>)list).GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator() {
      return ((IEnumerable)list).GetEnumerator();
    }
    #endregion

    #region Helpers
    public void TrimExcess() {
      int capacity = list.Capacity;
      list.TrimExcess();
      if (list.Capacity != capacity)
        OnPropertyChanged("Capacity");
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
