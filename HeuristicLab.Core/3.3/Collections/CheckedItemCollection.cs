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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HEAL.Fossil;

namespace HeuristicLab.Core {
  /// <summary>
  /// Represents a collection of checked items.
  /// </summary>
  /// <typeparam name="T">The element type (base type IItem)</typeparam>
  [StorableType("CAD59659-15B5-4CB0-A199-272E28F40832")]
  [Item("CheckedItemCollection", "Represents a collection of items that can be checked or unchecked.")]
  public class CheckedItemCollection<T> : ItemCollection<T>, ICheckedItemCollection<T> where T : class, IItem {
    [Storable]
    private Dictionary<T, bool> checkedState;

    /// <summary>
    /// Gets an enumerable of checked items
    /// </summary>
    public IEnumerable<T> CheckedItems {
      get { return from pair in checkedState where pair.Value select pair.Key; }
    }

    /// <summary>
    /// Instantiates an empty CheckedItemCollection for deserialization.
    /// </summary>
    /// <param name="deserializing"></param>
    [StorableConstructor]
    protected CheckedItemCollection(StorableConstructorFlag _) : base(_) { }
    protected CheckedItemCollection(CheckedItemCollection<T> original, Cloner cloner)
      : base(original, cloner) {
      list = new List<T>(original.Select(x => cloner.Clone(x)));
      checkedState = new Dictionary<T, bool>();
      foreach (var pair in original.checkedState)
        checkedState.Add(cloner.Clone(pair.Key), pair.Value);
    }
    /// <summary>
    /// Instantiates a new CheckedItemCollection.
    /// </summary>
    public CheckedItemCollection()
      : base() {
      checkedState = new Dictionary<T, bool>();
    }
    /// <summary>
    /// Instantiates a new CheckedItemCollection with a predefined capacity.
    /// </summary>
    /// <param name="capacity">Initial capacity.</param>
    public CheckedItemCollection(int capacity)
      : base(capacity) {
      checkedState = new Dictionary<T, bool>(capacity);
    }
    /// <summary>
    /// Instantiates a new CheckedItemCollection containing the elements of <paramref name="collection"/>.
    /// </summary>
    /// <param name="collection">Initial element collection.</param>
    public CheckedItemCollection(IEnumerable<T> collection)
      : base(collection) {
      checkedState = new Dictionary<T, bool>();
      foreach (var item in list)
        if (!checkedState.ContainsKey(item))
          checkedState.Add(item, true);
    }

    /// <summary>
    /// Gets the checked state of <paramref name="item"/>.
    /// </summary>
    /// <param name="item">The element to get the checked state for.</param>
    /// <returns>Checked state of <paramref name="item"/></returns>
    public bool ItemChecked(T item) {
      return checkedState[item];
    }

    /// <summary>
    /// Sets the checked state <paramref name="checkedState"/> of <paramref name="item"/>.
    /// </summary>
    /// <param name="item">The element to set the checked state for.</param>
    /// <param name="checkedState">The new checked state of the item</param>
    public void SetItemCheckedState(T item, bool checkedState) {
      if (!this.checkedState.ContainsKey(item)) throw new ArgumentException();
      if (this.checkedState[item] != checkedState) {
        this.checkedState[item] = checkedState;
        OnCheckedItemsChanged(new T[] { item });
      }
    }

    /// <summary>
    /// Adds a new <paramref name="item"/> with the given <paramref name="checkedState"/>.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <param name="checkedState">The checked state of the item to add.</param>
    public void Add(T item, bool checkedState) {
      Add(item);
      SetItemCheckedState(item, checkedState);
    }

    /// <summary>
    /// Creates a ReadOnlyCheckedItemCollection containing the same elements.
    /// </summary>
    /// <returns>A new ReadOnlyCheckedItemCollection containing the same elements.</returns>
    public new ReadOnlyCheckedItemCollection<T> AsReadOnly() {
      return new ReadOnlyCheckedItemCollection<T>(this);
    }

    /// <summary>
    /// Raised when the collection of items is reset.
    /// </summary>
    /// <param name="items">Empty</param>
    /// <param name="oldItems">The elements in the collection before the reset.</param>
    protected override void OnCollectionReset(IEnumerable<T> items, IEnumerable<T> oldItems) {
      foreach (var oldItem in oldItems)
        if (!list.Contains(oldItem))
          checkedState.Remove(oldItem);
      foreach (var item in items)
        if (!checkedState.ContainsKey(item))
          checkedState.Add(item, true);
      base.OnCollectionReset(items, oldItems);
    }

    /// <summary>
    /// Raised when new items are added to the collection.
    /// </summary>
    /// <param name="items">The elements that are added to the collection.</param>
    protected override void OnItemsAdded(IEnumerable<T> items) {
      foreach (var item in items)
        if (!checkedState.ContainsKey(item))
          checkedState.Add(item, true);
      base.OnItemsAdded(items);
    }

    /// <summary>
    /// Raised when items are removed from the collection.
    /// </summary>
    /// <param name="items">The items that are removed.</param>
    protected override void OnItemsRemoved(IEnumerable<T> items) {
      foreach (var item in items) {
        if (!list.Contains(item))
          checkedState.Remove(item);
      }
      base.OnItemsRemoved(items);
    }

    /// <summary>
    /// Raised when the checked state of items is changed.
    /// </summary>
    /// <param name="items">The item whose check state is changed.</param>
    protected virtual void OnCheckedItemsChanged(IEnumerable<T> items) {
      RaiseCheckedItemsChanged(new CollectionItemsChangedEventArgs<T>(items));
    }

    /// <summary>
    /// Raised after the checked state of items has been changed.
    /// </summary>
    public event CollectionItemsChangedEventHandler<T> CheckedItemsChanged;
    private void RaiseCheckedItemsChanged(CollectionItemsChangedEventArgs<T> e) {
      var handler = CheckedItemsChanged;
      if (handler != null) handler(this, e);
    }

    /// <summary>
    /// Creates a deep clone of the CheckedItemCollection.
    /// </summary>
    /// <param name="cloner"></param>
    /// <returns>A clone of the CheckedItemCollection</returns>
    public override IDeepCloneable Clone(Cloner cloner) {
      return new CheckedItemCollection<T>(this, cloner);
    }
  }
}
