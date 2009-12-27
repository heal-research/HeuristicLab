#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Collections {
  [Serializable]
  public class ObservableList<T> : IndexedCollectionChangedEventsBase<IndexedItem<T>>, IList<T> {
    [Storable]
    private List<T> list;

    #region Properties
    public int Capacity {
      get { return list.Capacity; }
      set { list.Capacity = value; }
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
        list[index] = value;
        OnItemsReplaced(new IndexedItem<T>[] { new IndexedItem<T>(index, value) }, new IndexedItem<T>[] { new IndexedItem<T>(index, item) });
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
      list.Add(item);
      OnItemsAdded(new IndexedItem<T>[] { new IndexedItem<T>(list.Count - 1, item) });
    }
    public void AddRange(IEnumerable<T> collection) {
      int index = list.Count;
      list.AddRange(collection);
      List<IndexedItem<T>> items = new List<IndexedItem<T>>();
      foreach (T item in collection) {
        items.Add(new IndexedItem<T>(index, item));
        index++;
      }
      OnItemsAdded(items);
    }

    public void Insert(int index, T item) {
      list.Insert(index, item);
      OnItemsAdded(new IndexedItem<T>[] { new IndexedItem<T>(index, item) });
    }
    public void InsertRange(int index, IEnumerable<T> collection) {
      list.InsertRange(index, collection);
      List<IndexedItem<T>> items = new List<IndexedItem<T>>();
      foreach (T item in collection) {
        items.Add(new IndexedItem<T>(index, item));
        index++;
      }
      OnItemsAdded(items);
    }

    public bool Remove(T item) {
      int index = list.IndexOf(item);
      if (index != -1) {
        list.RemoveAt(index);
        OnItemsRemoved(new IndexedItem<T>[] { new IndexedItem<T>(index, item) });
        return true;
      }
      return false;
    }
    public int RemoveAll(Predicate<T> match) {
      if (match == null) throw new ArgumentNullException();
      List<IndexedItem<T>> items = new List<IndexedItem<T>>();
      for (int i = 0; i < list.Count; i++) {
        if (match(list[i]))
          items.Add(new IndexedItem<T>(i, list[i]));
      }
      int result = list.RemoveAll(match);
      OnItemsRemoved(items);
      return result;
    }
    public void RemoveAt(int index) {
      T item = list[index];
      list.RemoveAt(index);
      OnItemsRemoved(new IndexedItem<T>[] { new IndexedItem<T>(index, item) });
    }
    public void RemoveRange(int index, int count) {
      IndexedItem<T>[] items = GetIndexedItems(index, count);
      list.RemoveRange(index, count);
      OnItemsRemoved(items);
    }

    public void Clear() {
      IndexedItem<T>[] items = GetIndexedItems();
      list.Clear();
      OnCollectionReset(new IndexedItem<T>[0], items);
    }

    public void Reverse() {
      IndexedItem<T>[] oldItems = GetIndexedItems();
      list.Reverse();
      OnItemsMoved(GetIndexedItems(), oldItems);
    }
    public void Reverse(int index, int count) {
      IndexedItem<T>[] oldItems = GetIndexedItems(index, count);
      list.Reverse(index, count);
      OnItemsMoved(GetIndexedItems(index, count), oldItems);
    }

    public void Sort() {
      IndexedItem<T>[] oldItems = GetIndexedItems();
      list.Sort();
      OnItemsMoved(GetIndexedItems(), oldItems);
    }
    public void Sort(Comparison<T> comparison) {
      IndexedItem<T>[] oldItems = GetIndexedItems();
      list.Sort(comparison);
      OnItemsMoved(GetIndexedItems(), oldItems);
    }
    public void Sort(IComparer<T> comparer) {
      IndexedItem<T>[] oldItems = GetIndexedItems();
      list.Sort(comparer);
      OnItemsMoved(GetIndexedItems(), oldItems);
    }
    public void Sort(int index, int count, IComparer<T> comparer) {
      IndexedItem<T>[] oldItems = GetIndexedItems(index, count);
      list.Sort(index, count, comparer);
      OnItemsMoved(GetIndexedItems(index, count), oldItems);
    }
    #endregion

    #region Conversion
    public ReadOnlyCollection<T> AsReadOnly() {
      return list.AsReadOnly();
    }
    public T[] ToArray() {
      return list.ToArray();
    }
    void ICollection<T>.CopyTo(T[] array, int arrayIndex) {
      list.CopyTo(array, arrayIndex);
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
    public List<T>.Enumerator GetEnumerator() {
      return list.GetEnumerator();
    }
    IEnumerator<T> IEnumerable<T>.GetEnumerator() {
      return ((IEnumerable<T>)list).GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator() {
      return ((IEnumerable)list).GetEnumerator();
    }
    #endregion

    #region Helpers
    public void TrimExcess() {
      list.TrimExcess();
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
