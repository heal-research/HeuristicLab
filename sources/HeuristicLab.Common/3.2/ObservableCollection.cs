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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace HeuristicLab.Common {
  public class ObservableCollection<T> : ICollection<T>, INotifyCollectionChanged {
    protected List<T> list;

    public ObservableCollection() {
      list = new List<T>();
    }

    #region Events
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    protected void FireCollectionChanged(NotifyCollectionChangedEventArgs e) {
      if (CollectionChanged != null) {
        CollectionChanged(this, e);
      }
    }

    protected virtual void FireCollectionCleared() {
        NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
        FireCollectionChanged( e);
    }

    protected virtual void FireItemsAdded(IEnumerable<T> addedItems, int index) {
        NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, addedItems.ToList(), index);
        FireCollectionChanged(e);
    }

    protected virtual void FireItemsRemoved(IEnumerable<T> removedItems) {
        NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItems.ToList());
        FireCollectionChanged(e);
    }
    #endregion

    #region ICollection<T> Members
    public void Add(T item) {
      int index = this.list.Count - 1;
      this.list.Add(item);
      FireItemsAdded(new List<T> { item }, index);
    }

    public void AddRange(IEnumerable<T> items) {
      int index = this.list.Count - 1;
      this.list.AddRange(items);
      FireItemsAdded(items, index);
    }

    public void Clear() {
      this.list.Clear();
      FireCollectionCleared();
    }

    public bool Contains(T item) {
      return this.list.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex) {
      this.list.CopyTo(array, arrayIndex);
    }

    public int Count {
      get { return this.list.Count; }
    }

    public bool IsReadOnly {
      get { return false; }
    }

    public bool Remove(T item) {
      bool ret = this.list.Remove(item);
      if (ret)
        FireItemsRemoved(new List<T> { item });
      return ret;
    }

    public void RemoveRange(IEnumerable<T> items) {
      bool ret = false;
      foreach (T item in items)
        ret |= list.Remove(item);
      if (ret)
        FireItemsRemoved(items);
    }

    #endregion

    #region IEnumerable<T> Members

    public IEnumerator<T> GetEnumerator() {
      return list.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return list.GetEnumerator();
    }

    #endregion


  }
}
