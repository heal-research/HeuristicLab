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
  public class ObservableCollection<T> : CollectionChangedEventsBase<T>, ICollection<T> {
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
    public bool IsReadOnly {
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
      list.Add(item);
      OnItemsAdded(new T[] { item });
    }
    public void AddRange(IEnumerable<T> collection) {
      list.AddRange(collection);
      OnItemsAdded(collection);
    }

    public bool Remove(T item) {
      if (list.Remove(item)) {
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
      if (items.Count > 0)
        OnItemsRemoved(items);
    }
    public int RemoveAll(Predicate<T> match) {
      List<T> items = list.FindAll(match);
      int result = list.RemoveAll(match);
      OnItemsRemoved(items);
      return result;
    }

    public void Clear() {
      T[] items = list.ToArray();
      list.Clear();
      OnCollectionReset(new T[0], items);
    }
    #endregion

    #region Conversion
    public ReadOnlyCollection<T> AsReadOnly() {
      return list.AsReadOnly();
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
      list.TrimExcess();
    }
    #endregion
  }
}
