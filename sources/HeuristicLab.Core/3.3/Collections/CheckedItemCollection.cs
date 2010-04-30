#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  [StorableClass]
  [Item("CheckedItemCollection<T>", "Represents a collection of items that can be checked or unchecked.")]
  public class CheckedItemCollection<T> : ItemCollection<T>, ICheckedItemCollection<T> where T : class, IItem {
    [Storable]
    private Dictionary<T, bool> checkedState;

    /// <summary>
    /// gets an enumerable of checked items
    /// </summary>
    public IEnumerable<T> CheckedItems {
      get { return from pair in checkedState where pair.Value select pair.Key; }
    }

    public CheckedItemCollection()
      : base() {
      checkedState = new Dictionary<T, bool>();
    }
    public CheckedItemCollection(int capacity)
      : base(capacity) {
      checkedState = new Dictionary<T, bool>(capacity);
    }
    public CheckedItemCollection(IEnumerable<T> collection)
      : base(collection) {
      checkedState = new Dictionary<T, bool>();
      foreach (var item in collection)
        if (!checkedState.ContainsKey(item))
          checkedState.Add(item, false);
    }
    [StorableConstructor]
    protected CheckedItemCollection(bool deserializing) : base(deserializing) { }

    public bool ItemChecked(T item) {
      return checkedState[item];
    }

    public void SetItemCheckedState(T item, bool checkedState) {
      if (!this.checkedState.ContainsKey(item)) throw new ArgumentException();
      if (this.checkedState[item] != checkedState) {
        this.checkedState[item] = checkedState;
        OnCheckedItemsChanged(new T[] { item });
      }
    }

    public void Add(T item, bool checkedState) {
      Add(item);
      SetItemCheckedState(item, checkedState);
    }

    protected override void OnCollectionReset(IEnumerable<T> items, IEnumerable<T> oldItems) {
      foreach (var oldItem in oldItems)
        checkedState.Remove(oldItem);
      foreach (var item in items)
        if (!checkedState.ContainsKey(item))
          checkedState.Add(item, false);
      base.OnCollectionReset(items, oldItems);
    }

    protected override void OnItemsAdded(IEnumerable<T> items) {
      foreach (var item in items)
        if (!checkedState.ContainsKey(item))
          checkedState.Add(item, false);
      base.OnItemsAdded(items);
    }

    protected override void OnItemsRemoved(IEnumerable<T> items) {
      foreach (var item in items) {
        checkedState.Remove(item);
      }
      base.OnItemsRemoved(items);
    }

    protected virtual void OnCheckedItemsChanged(IEnumerable<T> items) {
      RaiseCheckedItemsChanged(new CollectionItemsChangedEventArgs<T>(items));
    }

    public event CollectionItemsChangedEventHandler<T> CheckedItemsChanged;
    private void RaiseCheckedItemsChanged(CollectionItemsChangedEventArgs<T> e) {
      var handler = CheckedItemsChanged;
      if (handler != null) handler(this, e);
    }

    public object Clone() {
      return Clone(new Cloner());
    }
    public virtual IDeepCloneable Clone(Cloner cloner) {
      CheckedItemCollection<T> clone = (CheckedItemCollection<T>)Activator.CreateInstance(this.GetType());
      cloner.RegisterClonedObject(this, clone);
      clone.list = new List<T>(this.Select(x => (T)cloner.Clone(x)));
      clone.checkedState = new Dictionary<T, bool>();
      foreach (var pair in checkedState)
        clone.checkedState.Add((T)cloner.Clone(pair.Key), pair.Value);
      return clone;
    }
  }
}
