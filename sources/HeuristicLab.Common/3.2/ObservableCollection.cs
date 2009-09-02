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

namespace HeuristicLab.Common {
  public class ObservableCollection<T> : ICollection<T> {
    private List<T> list;

    public ObservableCollection() {
      list = new List<T>();
    }

    #region Events
    public event EventHandler CollectionCleared;
    protected void FireCollectionCleared() {
      OnCollectionCleared();
    } 
    protected virtual void OnCollectionCleared() {
      if (CollectionCleared != null)
        CollectionCleared(this, new EventArgs());
    }

    public event EventHandler<EnumerableEventArgs<T>> ItemsAdded;
    protected void FireItemsAdded(IEnumerable<T> addedItems) {
      OnItemsAdded(addedItems);
    }
    protected virtual void OnItemsAdded(IEnumerable<T> addedItems) {
      if (ItemsAdded != null)
        ItemsAdded(this, new EnumerableEventArgs<T>(addedItems));
    }

    public event EventHandler<EnumerableEventArgs<T>> ItemsRemoved;
    protected void FireItemsRemoved(IEnumerable<T> removedItems) {
      OnItemsRemoved(removedItems);
    }
    protected virtual void OnItemsRemoved(IEnumerable<T> removedItems) {
      if (ItemsRemoved != null)
        ItemsRemoved(this, new EnumerableEventArgs<T>(removedItems));
    }
    #endregion

    #region ICollection<T> Members
    public void Add(T item) {
      this.list.Add(item);
      FireItemsAdded(new List<T> { item });
    }

    public void AddRange(IEnumerable<T> items) {
      this.list.AddRange(items);
      FireItemsAdded(items);
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
