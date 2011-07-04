#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class ObservableArray<T> : IObservableArray<T> {
    [Storable]
    protected T[] array;

    #region Properties
    public int Length {
      get { return array.Length; }
    }
    int ICollection<T>.Count {
      get { return array.Length; }
    }
    bool ICollection<T>.IsReadOnly {
      get { return array.IsReadOnly; }
    }

    public T this[int index] {
      get {
        return array[index];
      }
      set {
        T item = array[index];
        if (!((item == null) && (value == null)) && ((item == null) || (!item.Equals(value)))) {
          array[index] = value;
          OnItemsReplaced(new IndexedItem<T>[] { new IndexedItem<T>(index, value) }, new IndexedItem<T>[] { new IndexedItem<T>(index, item) });
          OnPropertyChanged("Item[]");
        }
      }
    }
    #endregion

    #region Constructors
    public ObservableArray() {
      array = new T[0];
    }
    public ObservableArray(int length) {
      array = new T[length];
    }
    public ObservableArray(T[] array) {
      this.array = (T[])array.Clone();
    }
    public ObservableArray(IEnumerable<T> collection) {
      array = collection.ToArray();
    }
    [StorableConstructor]
    protected ObservableArray(bool deserializing) { }
    #endregion

    #region Access
    public bool Contains(T item) {
      return IndexOf(item) != -1;
    }
    public int IndexOf(T item) {
      return Array.IndexOf<T>(array, item);
    }
    public int IndexOf(T item, int startIndex) {
      return Array.IndexOf<T>(array, item, startIndex);
    }
    public int IndexOf(T item, int startIndex, int count) {
      return Array.IndexOf<T>(array, item, startIndex, count);
    }

    public int LastIndexOf(T item) {
      return Array.LastIndexOf<T>(array, item);
    }
    public int LastIndexOf(T item, int startIndex) {
      return Array.LastIndexOf<T>(array, item, startIndex);
    }
    public int LastIndexOf(T item, int startIndex, int count) {
      return Array.LastIndexOf<T>(array, item, startIndex, count);
    }

    public int BinarySearch(T item) {
      return Array.BinarySearch<T>(array, item);
    }
    public int BinarySearch(T item, IComparer<T> comparer) {
      return Array.BinarySearch<T>(array, item, comparer);
    }
    public int BinarySearch(int index, int count, T item) {
      return Array.BinarySearch<T>(array, index, count, item);
    }
    public int BinarySearch(int index, int count, T item, IComparer<T> comparer) {
      return Array.BinarySearch<T>(array, index, count, item, comparer);
    }

    public bool Exists(Predicate<T> match) {
      return Array.Exists<T>(array, match);
    }

    public T Find(Predicate<T> match) {
      return Array.Find<T>(array, match);
    }
    public T[] FindAll(Predicate<T> match) {
      return Array.FindAll<T>(array, match);
    }
    public T FindLast(Predicate<T> match) {
      return Array.FindLast<T>(array, match);
    }

    public int FindIndex(Predicate<T> match) {
      return Array.FindIndex<T>(array, match);
    }
    public int FindIndex(int startIndex, Predicate<T> match) {
      return Array.FindIndex<T>(array, startIndex, match);
    }
    public int FindIndex(int startIndex, int count, Predicate<T> match) {
      return Array.FindIndex<T>(array, startIndex, count, match);
    }

    public int FindLastIndex(Predicate<T> match) {
      return Array.FindLastIndex<T>(array, match);
    }
    public int FindLastIndex(int startIndex, Predicate<T> match) {
      return Array.FindLastIndex<T>(array, startIndex, match);
    }
    public int FindLastIndex(int startIndex, int count, Predicate<T> match) {
      return Array.FindLastIndex<T>(array, startIndex, count, match);
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

    public void Clear(int index, int length) {
      if (length > 0) {
        IndexedItem<T>[] oldItems = GetIndexedItems(index, length);
        Array.Clear(array, index, length);
        OnPropertyChanged("Item[]");
        OnItemsReplaced(GetIndexedItems(index, length), oldItems); 
      }
    }
    void ICollection<T>.Clear() {
      Clear(0, array.Length);
    }

    public void Resize(int newSize) {
      if (newSize != array.Length) {
        IndexedItem<T>[] oldItems = GetIndexedItems();
        Array.Resize<T>(ref array, newSize);
        OnPropertyChanged("Length");
        OnPropertyChanged("Item[]");
        OnCollectionReset(GetIndexedItems(), oldItems);
      }
    }

    public void Reverse() {
      if (array.Length > 1) {
        IndexedItem<T>[] oldItems = GetIndexedItems();
        Array.Reverse(array);
        OnPropertyChanged("Item[]");
        OnItemsMoved(GetIndexedItems(), oldItems);
      }
    }
    public void Reverse(int index, int length) {
      if (length > 1) {
        IndexedItem<T>[] oldItems = GetIndexedItems(index, length);
        Array.Reverse(array, index, length);
        OnPropertyChanged("Item[]");
        OnItemsMoved(GetIndexedItems(index, length), oldItems);
      }
    }

    public void Sort() {
      if (array.Length > 1) {
        IndexedItem<T>[] oldItems = GetIndexedItems();
        Array.Sort<T>(array);
        OnPropertyChanged("Item[]");
        OnItemsMoved(GetIndexedItems(), oldItems);
      }
    }
    public void Sort(Comparison<T> comparison) {
      if (array.Length > 1) {
        IndexedItem<T>[] oldItems = GetIndexedItems();
        Array.Sort<T>(array, comparison);
        OnPropertyChanged("Item[]");
        OnItemsMoved(GetIndexedItems(), oldItems);
      }
    }
    public void Sort(IComparer<T> comparer) {
      if (array.Length > 1) {
        IndexedItem<T>[] oldItems = GetIndexedItems();
        Array.Sort<T>(array, comparer);
        OnPropertyChanged("Item[]");
        OnItemsMoved(GetIndexedItems(), oldItems);
      }
    }
    public void Sort(int index, int length) {
      if (length > 1) {
        IndexedItem<T>[] oldItems = GetIndexedItems(index, length);
        Array.Sort<T>(array, index, length);
        OnPropertyChanged("Item[]");
        OnItemsMoved(GetIndexedItems(index, length), oldItems);
      }
    }
    public void Sort(int index, int length, IComparer<T> comparer) {
      if (length > 1) {
        IndexedItem<T>[] oldItems = GetIndexedItems(index, length);
        Array.Sort<T>(array, index, length, comparer);
        OnPropertyChanged("Item[]");
        OnItemsMoved(GetIndexedItems(index, length), oldItems);
      }
    }
    #endregion

    #region Conversion
    public ReadOnlyObservableArray<T> AsReadOnly() {
      return new ReadOnlyObservableArray<T>(this);
    }
    public void CopyTo(T[] array) {
      Array.Copy(this.array, array, this.array.Length);
    }
    public void CopyTo(T[] array, int arrayIndex) {
      Array.Copy(this.array, 0, array, arrayIndex, this.array.Length);
    }
    public void CopyTo(int index, T[] array, int arrayIndex, int count) {
      Array.Copy(this.array, index, array, arrayIndex, count);
    }
    public TOutput[] ConvertAll<TOutput>(Converter<T, TOutput> converter) {
      return Array.ConvertAll<T, TOutput>(array, converter);
    }
    #endregion

    #region Processing
    public void ForEach(Action<T> action) {
      Array.ForEach<T>(array, action);
    }
    public bool TrueForAll(Predicate<T> match) {
      return Array.TrueForAll<T>(array, match);
    }
    #endregion

    #region Enumeration
    public IEnumerator<T> GetEnumerator() {
      foreach (object o in ((IEnumerable)this))
        yield return (T)o;
    }
    IEnumerator IEnumerable.GetEnumerator() {
      return array.GetEnumerator();
    }
    #endregion

    #region Events
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
    #endregion

    #region Private helpers
    private IndexedItem<T>[] GetIndexedItems() {
      IndexedItem<T>[] items = new IndexedItem<T>[array.Length];
      for (int i = 0; i < array.Length; i++)
        items[i] = new IndexedItem<T>(i, array[i]);
      return items;
    }
    private IndexedItem<T>[] GetIndexedItems(int index, int count) {
      IndexedItem<T>[] items = new IndexedItem<T>[count];
      for (int i = 0; i < count; i++)
        items[i] = new IndexedItem<T>(index + i, array[index + i]);
      return items;
    }
    #endregion
  }
}
