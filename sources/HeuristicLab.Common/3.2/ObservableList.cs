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
  [Obsolete("Use collections of the HeuristicLab.Collections plugin instead", false)]
  public class ObservableList<T> : ObservableCollection<T>, IList<T> {

    public ObservableList()
      : base() {
    }

    #region IList<T> Members
    public int IndexOf(T item) {
      return list.IndexOf(item);
    }

    public void Insert(int index, T item) {
      list.Insert(index, item);
      FireItemsInserted(new List<T>() { item }, index);
    }

    public void RemoveAt(int index) {
      T itemRemoved = list[index];
      list.RemoveAt(index);
      FireItemsRemoved(new List<T>() { itemRemoved });
    }

    public T this[int index] {
      get {
        return list[index];
      }
      set {
        T oldItem = list[index];
        list[index] = value;
        FireItemsReplaced(value, oldItem, index);
      }
    }
    #endregion

    #region events
    protected virtual void FireItemsInserted(IEnumerable<T> insertedItems, int index) {
      NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, insertedItems.ToList(), index);
      FireCollectionChanged(e);
    }

    protected virtual void FireItemsReplaced(T newItem, T oldItem, int index) {
      NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem);
      FireCollectionChanged(e);
    }
    #endregion
  }
}
