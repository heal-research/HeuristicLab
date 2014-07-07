#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Collections {
  [StorableClass]
  [Serializable]
  public class ObservableList<T> : IObservableList<T> {
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

    public T this[int index] {
      get {
        return list[index];
      }
      set {
        T item = list[index];
        if (!((item == null) && (value == null)) && ((item == null) || (!item.Equals(value)))) {
          list[index] = value;
          OnItemsReplaced(new IndexedItem<T>[] { new IndexedItem<T>(index, value) }, new IndexedItem<T>[] { new IndexedItem<T>(index, item) });
          OnPropertyChanged("Item[]");
        }
      }
    }
    #endregion

    #region Constructors
    public ObservableList() {
      list = new List<T>();
    }
    public ObservableList(int capacity) {
      list = new List<T>(capacity);
    }
    public ObservableList(IEnumerable<T> collection) {
      list = new List<T>(collection);
    }
    [StorableConstructor]
    protected ObservableList(bool deserializing) { }
    #endregion

    #region Access
    public List<T> GetRange(int index, int count) {
      return list.GetRange(index, count);
    }

    public bool Contains(T item) {
      return list.Contains(item);
    }

    public int IndexOf(T item) {
      return list.IndexOf(item);
    }
    public int IndexOf(T item, int index) {
      return list.IndexOf(item, index);
    }
    public int IndexOf(T item, int index, int count) {
      return list.IndexOf(item, index, count);
    }

    public int LastIndexOf(T item) {
      return list.LastIndexOf(item);
    }
    public int LastIndexOf(T item, int index) {
      return list.LastIndexOf(item, index);
    }
    public int LastIndexOf(T item, int index, int count) {
      return list.LastIndexOf(item, index, count);
    }

    public int BinarySearch(T item) {
      return list.BinarySearch(item);
    }
    public int BinarySearch(T item, IComparer<T> comparer) {
      return list.BinarySearch(item, comparer);
    }
    public int BinarySearch(int index, int count, T item, IComparer<T> comparer) {
      return list.BinarySearch(index, count, item, comparer);
    }

    public bool Exists(Predicate<T> match) {
      return list.Exists(match);
    }

    public T Find(Predicate<T> match) {
      return list.Find(match);
    }
    public List<T> FindAll(Predicate<T> match) {
      return list.FindAll(match);
    }
    public T FindLast(Predicate<T> match) {
      return list.FindLast(match);
    }

    public int FindIndex(Predicate<T> match) {
      return list.FindIndex(match);
    }
    public int FindIndex(int startIndex, Predicate<T> match) {
      return list.FindIndex(startIndex, match);
    }
    public int FindIndex(int startIndex, int count, Predicate<T> match) {
      return list.FindIndex(startIndex, count, match);
    }

    public int FindLastIndex(Predicate<T> match) {
      return list.FindLastIndex(match);
    }
    public int FindLastIndex(int startIndex, Predicate<T> match) {
      return list.FindLastIndex(startIndex, match);
    }
    public int FindLastIndex(int startIndex, int count, Predicate<T> match) {
      return list.FindLastIndex(startIndex, count, match);
    }
    #endregion

    #region Manipulation
    public void Add(T item) {
      int capacity = list.Capacity;
      list.Add(item);
      OnItemsAdded(new IndexedItem<T>[] { new IndexedItem<T>(list.Count - 1, item) });
      OnItemsAdded(new T[] { item });
      if (list.Capacity != capacity)
        OnPropertyChanged("Capacity");
      OnPropertyChanged("Item[]");
      OnPropertyChanged("Count");
    }
    public void AddRange(IEnumerable<T> collection) {
      int capacity = list.Capacity;
      int index = list.Count;
      list.AddRange(collection);
      List<IndexedItem<T>> items = new List<IndexedItem<T>>();
      foreach (T item in collection) {
        items.Add(new IndexedItem<T>(index, item));
        index++;
      }
      if (items.Count > 0) {
        OnItemsAdded(items);
        OnItemsAdded(collection);
        if (list.Capacity != capacity)
          OnPropertyChanged("Capacity");
        OnPropertyChanged("Item[]");
        OnPropertyChanged("Count");
      }
    }

    public void Insert(int index, T item) {
      int capacity = list.Capacity;
      list.Insert(index, item);
      OnItemsAdded(new IndexedItem<T>[] { new IndexedItem<T>(index, item) });
      OnItemsAdded(new T[] { item });
      if (list.Capacity != capacity)
        OnPropertyChanged("Capacity");
      OnPropertyChanged("Item[]");
      OnPropertyChanged("Count");
    }
    public void InsertRange(int index, IEnumerable<T> collection) {
      int capacity = list.Capacity;
      list.InsertRange(index, collection);
      List<IndexedItem<T>> items = new List<IndexedItem<T>>();
      foreach (T item in collection) {
        items.Add(new IndexedItem<T>(index, item));
        index++;
      }
      if (items.Count > 0) {
        OnItemsAdded(items);
        OnItemsAdded(collection);
        if (list.Capacity != capacity)
          OnPropertyChanged("Capacity");
        OnPropertyChanged("Item[]");
        OnPropertyChanged("Count");
      }
    }

    /// <summary>
    /// Performs a Clear and an AddRange, but does not fire separate events for those operations
    /// </summary>
    /// <param name="collection"></param>
    public void Replace(IEnumerable<T> collection) {
      List<IndexedItem<T>> oldItems = null;
      if (list.Any()) oldItems = list.Select((x, i) => new IndexedItem<T>(i, x)).ToList();
      else oldItems = new List<IndexedItem<T>>();

      int oldCapacity = list.Capacity;
      list.Clear();
      list.AddRange(collection);

      List<IndexedItem<T>> items = null;
      if (list.Any()) items = list.Select((x, i) => new IndexedItem<T>(i, x)).ToList();
      else items = new List<IndexedItem<T>>();

      OnItemsReplaced(items, oldItems);
      if (oldCapacity != list.Capacity) OnPropertyChanged("Capacity");
      OnPropertyChanged("Item[]");
      if (oldItems.Count != items.Count) OnPropertyChanged("Count");
    }

    public bool Remove(T item) {
      int index = list.IndexOf(item);
      if (index != -1) {
        list.RemoveAt(index);
        OnItemsRemoved(new IndexedItem<T>[] { new IndexedItem<T>(index, item) });
        OnItemsRemoved(new T[] { item });
        OnPropertyChanged("Item[]");
        OnPropertyChanged("Count");
        return true;
      }
      return false;
    }
    public int RemoveAll(Predicate<T> match) {
      if (match == null) throw new ArgumentNullException();
      List<IndexedItem<T>> indexedItems = new List<IndexedItem<T>>();
      List<T> items = new List<T>();
      for (int i = 0; i < list.Count; i++) {
        if (match(list[i])) {
          indexedItems.Add(new IndexedItem<T>(i, list[i]));
          items.Add(list[i]);
        }
      }
      int result = 0;
      if (indexedItems.Count > 0) {
        result = list.RemoveAll(match);
        OnItemsRemoved(indexedItems);
        OnItemsRemoved(items);
        OnPropertyChanged("Item[]");
        OnPropertyChanged("Count");
      }
      return result;
    }
    public void RemoveAt(int index) {
      T item = list[index];
      list.RemoveAt(index);
      OnItemsRemoved(new IndexedItem<T>[] { new IndexedItem<T>(index, item) });
      OnItemsRemoved(new T[] { item });
      OnPropertyChanged("Item[]");
      OnPropertyChanged("Count");
    }
    public void RemoveRange(int index, int count) {
      if (count > 0) {
        IndexedItem<T>[] indexedItems = GetIndexedItems(index, count);
        T[] items = new T[count];
        list.CopyTo(index, items, 0, count);
        list.RemoveRange(index, count);
        OnItemsRemoved(indexedItems);
        OnItemsRemoved(items);
        OnPropertyChanged("Item[]");
        OnPropertyChanged("Count");
      }
    }

    public void Clear() {
      if (list.Count > 0) {
        IndexedItem<T>[] indexedItems = GetIndexedItems();
        T[] items = list.ToArray();
        list.Clear();
        OnCollectionReset(new IndexedItem<T>[0], indexedItems);
        OnCollectionReset(new T[0], items);
        OnPropertyChanged("Item[]");
        OnPropertyChanged("Count");
      }
    }

    public void Reverse() {
      if (list.Count > 1) {
        IndexedItem<T>[] oldItems = GetIndexedItems();
        list.Reverse();
        OnItemsMoved(GetIndexedItems(), oldItems);
        OnPropertyChanged("Item[]");
      }
    }
    public void Reverse(int index, int count) {
      if (count > 1) {
        IndexedItem<T>[] oldItems = GetIndexedItems(index, count);
        list.Reverse(index, count);
        OnItemsMoved(GetIndexedItems(index, count), oldItems);
        OnPropertyChanged("Item[]");
      }
    }

    public void Sort() {
      if (list.Count > 1) {
        IndexedItem<T>[] oldItems = GetIndexedItems();
        list.StableSort();
        OnItemsMoved(GetIndexedItems(), oldItems);
        OnPropertyChanged("Item[]");
      }
    }
    public void Sort(Comparison<T> comparison) {
      if (list.Count > 1) {
        IndexedItem<T>[] oldItems = GetIndexedItems();
        list.StableSort(comparison);
        OnItemsMoved(GetIndexedItems(), oldItems);
        OnPropertyChanged("Item[]");
      }
    }
    public void Sort(IComparer<T> comparer) {
      if (list.Count > 1) {
        IndexedItem<T>[] oldItems = GetIndexedItems();
        list.StableSort(comparer);
        OnItemsMoved(GetIndexedItems(), oldItems);
        OnPropertyChanged("Item[]");
      }
    }
    public void Sort(int index, int count, IComparer<T> comparer) {
      if (count > 1) {
        IndexedItem<T>[] oldItems = GetIndexedItems(index, count);
        list.StableSort(index, count, comparer);
        OnItemsMoved(GetIndexedItems(index, count), oldItems);
        OnPropertyChanged("Item[]");
      }
    }
    #endregion

    #region Conversion
    public ReadOnlyObservableList<T> AsReadOnly() {
      return new ReadOnlyObservableList<T>(this);
    }

    public T[] ToArray() {
      return list.ToArray();
    }
    public void CopyTo(T[] array) {
      list.CopyTo(array);
    }
    public void CopyTo(T[] array, int arrayIndex) {
      list.CopyTo(array, arrayIndex);
    }
    public void CopyTo(int index, T[] array, int arrayIndex, int count) {
      list.CopyTo(index, array, arrayIndex, count);
    }
    public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) {
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
      return list.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator() {
      return list.GetEnumerator();
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
    public event CollectionItemsChangedEventHandler<IndexedItem<T>> ItemsAdded;
    protected virtual void OnItemsAdded(IEnumerable<IndexedItem<T>> items) {
      CollectionItemsChangedEventHandler<IndexedItem<T>> handler = ItemsAdded;
      if (handler != null) handler(this, new CollectionItemsChangedEventArgs<IndexedItem<T>>(items));
    }

    [field: NonSerialized]
    private event CollectionItemsChangedEventHandler<T> itemsAdded;
    event CollectionItemsChangedEventHandler<T> INotifyObservableCollectionItemsChanged<T>.ItemsAdded {
      add { itemsAdded += value; }
      remove { itemsAdded -= value; }
    }
    private void OnItemsAdded(IEnumerable<T> items) {
      CollectionItemsChangedEventHandler<T> handler = itemsAdded;
      if (handler != null) handler(this, new CollectionItemsChangedEventArgs<T>(items));
    }

    [field: NonSerialized]
    public event CollectionItemsChangedEventHandler<IndexedItem<T>> ItemsRemoved;
    protected virtual void OnItemsRemoved(IEnumerable<IndexedItem<T>> items) {
      CollectionItemsChangedEventHandler<IndexedItem<T>> handler = ItemsRemoved;
      if (handler != null) handler(this, new CollectionItemsChangedEventArgs<IndexedItem<T>>(items));
    }

    [field: NonSerialized]
    private event CollectionItemsChangedEventHandler<T> itemsRemoved;
    event CollectionItemsChangedEventHandler<T> INotifyObservableCollectionItemsChanged<T>.ItemsRemoved {
      add { itemsRemoved += value; }
      remove { itemsRemoved -= value; }
    }
    private void OnItemsRemoved(IEnumerable<T> items) {
      CollectionItemsChangedEventHandler<T> handler = itemsRemoved;
      if (handler != null) handler(this, new CollectionItemsChangedEventArgs<T>(items));
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
    private event CollectionItemsChangedEventHandler<T> collectionReset;
    event CollectionItemsChangedEventHandler<T> INotifyObservableCollectionItemsChanged<T>.CollectionReset {
      add { collectionReset += value; }
      remove { collectionReset -= value; }
    }
    private void OnCollectionReset(IEnumerable<T> items, IEnumerable<T> oldItems) {
      CollectionItemsChangedEventHandler<T> handler = collectionReset;
      if (handler != null) handler(this, new CollectionItemsChangedEventArgs<T>(items, oldItems));
    }

    [field: NonSerialized]
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion

    #region Private helpers
    private IndexedItem<T>[] GetIndexedItems() {
      IndexedItem<T>[] items = new IndexedItem<T>[list.Count];
      for (int i = 0; i < list.Count; i++)
        items[i] = new IndexedItem<T>(i, list[i]);
      return items;
    }
    private IndexedItem<T>[] GetIndexedItems(int index, int count) {
      IndexedItem<T>[] items = new IndexedItem<T>[count];
      for (int i = 0; i < count; i++)
        items[i] = new IndexedItem<T>(index + i, list[index + i]);
      return items;
    }
    #endregion
  }
}
