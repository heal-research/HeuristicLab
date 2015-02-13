#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  /// <summary>
  /// Represents a list of checked items.
  /// </summary>
  /// <typeparam name="T">The element type (base type is IItem)</typeparam>
  [StorableClass]
  [Item("CheckedItemList", "Represents a list of items that can be checked or unchecked.")]
  public class CheckedItemList<T> : ItemList<T>, ICheckedItemList<T> where T : class, IItem {
    [Storable]
    private Dictionary<T, bool> checkedState;

    /// <summary>
    /// Gets an enumerable of checked items.
    /// </summary>
    public IEnumerable<IndexedItem<T>> CheckedItems {
      get {
        return from i in Enumerable.Range(0, list.Count)
               where ItemChecked(i)
               select new IndexedItem<T>(i, list[i]);
      }
    }

    /// <summary>
    /// Instantiates a new CheckedItemList for deserialization.
    /// </summary>
    /// <param name="deserializing"></param>
    [StorableConstructor]
    protected CheckedItemList(bool deserializing) : base(deserializing) { }
    protected CheckedItemList(CheckedItemList<T> original, Cloner cloner)
      : base(original, cloner) {
      list = new List<T>(original.Select(x => (T)cloner.Clone(x)));
      checkedState = new Dictionary<T, bool>();
      foreach (var pair in original.checkedState)
        checkedState.Add(cloner.Clone(pair.Key), pair.Value);
    }
    /// <summary>
    /// Instantiates an empty CheckedItemList.
    /// </summary>
    public CheckedItemList()
      : base() {
      checkedState = new Dictionary<T, bool>();
    }
    /// <summary>
    /// Instantiates an empty CheckedItemList with a given initial <paramref name="capacity"/>.
    /// </summary>
    /// <param name="capacity">The initial capacity.</param>
    public CheckedItemList(int capacity)
      : base(capacity) {
      checkedState = new Dictionary<T, bool>();
    }
    /// <summary>
    /// Instantiates an CheckedItemList initially filled with the elements of <paramref name="collection"/>.
    /// </summary>
    /// <param name="collection">Collection of elements.</param>
    public CheckedItemList(IEnumerable<T> collection)
      : base(collection) {
      checkedState = new Dictionary<T, bool>();
      foreach (var item in list) {
        if (!checkedState.ContainsKey(item))
          checkedState.Add(item, true);
      }
    }

    /// <summary>
    /// Gets the checked state of <paramref name="item"/>.
    /// </summary>
    /// <param name="item">The element to get the checked state for.</param>
    /// <returns>The checked state of <paramref name="item"/></returns>
    public bool ItemChecked(T item) {
      return checkedState[item];
    }

    /// <summary>
    /// Gets the checked state of item with <paramref name="index"/>.
    /// </summary>
    /// <param name="itemIndex">The index of the element to get the checked state for.</param>
    /// <returns>The checked state of the element at <paramref name="itemIndex"/>.</returns>
    public bool ItemChecked(int itemIndex) {
      return ItemChecked(this[itemIndex]);
    }

    /// <summary>
    /// Sets the checked state of <paramref name="item"/> to <paramref name="checkedState"/>.
    /// </summary>
    /// <param name="item">The item to set the checked state for.</param>
    /// <param name="checkedState">The new checked state of <paramref name="item"/></param>
    public void SetItemCheckedState(T item, bool checkedState) {
      if (!this.checkedState.ContainsKey(item)) throw new ArgumentException();
      if (this.checkedState[item] != checkedState) {
        this.checkedState[item] = checkedState;
        OnCheckedItemsChanged(new IndexedItem<T>[] { new IndexedItem<T>(IndexOf(item), item) });
      }
    }

    /// <summary>
    /// Sets the checked state of the element with <paramref name="itemIndex"/> to <paramref name="checkedState"/>.
    /// </summary>
    /// <param name="itemIndex">The index of the item to set the checked state for.</param>
    /// <param name="checkedState">The new checked state of the item.</param>
    public void SetItemCheckedState(int itemIndex, bool checkedState) {
      SetItemCheckedState(this[itemIndex], checkedState);
    }

    /// <summary>
    /// Adds a new <paramref name="item"/> with <paramref name="checkedState"/> to the list.
    /// </summary>
    /// <param name="item">The item to add to the list.</param>
    /// <param name="checkedState">The checked state of the item added to the list.</param>
    public void Add(T item, bool checkedState) {
      Add(item);
      SetItemCheckedState(item, checkedState);
    }

    /// <summary>
    /// Inserts a new <paramref name="item"/> at <paramref name="index"/> with <paramref name="checkedState"/> into the list.
    /// </summary>
    /// <param name="index">The insertion index of the new element.</param>
    /// <param name="item">The element that is inserted into the list.</param>
    /// <param name="checkedState">The checked state of the inserted element.</param>
    public void Insert(int index, T item, bool checkedState) {
      Insert(index, item);
      SetItemCheckedState(item, checkedState);
    }

    /// <summary>
    /// Creates a ReadOnlyCheckedItemList containing the same elements.
    /// </summary>
    /// <returns>A new ReadOnlyCheckedItemList containing the same elements.</returns>
    public new ReadOnlyCheckedItemList<T> AsReadOnly() {
      return new ReadOnlyCheckedItemList<T>(this);
    }

    /// <summary>
    /// Raised after the list has been reset.
    /// </summary>
    /// <param name="items">Empty</param>
    /// <param name="oldItems">The elements of the list before it has been reset.</param>
    protected override void OnCollectionReset(IEnumerable<IndexedItem<T>> items, IEnumerable<IndexedItem<T>> oldItems) {
      foreach (var oldIndexedItem in oldItems) {
        if (!list.Contains(oldIndexedItem.Value))
          checkedState.Remove(oldIndexedItem.Value);
      }
      foreach (var indexedItem in items) {
        if (!checkedState.ContainsKey(indexedItem.Value))
          checkedState.Add(indexedItem.Value, true);
      }
      base.OnCollectionReset(items, oldItems);
    }

    /// <summary>
    /// Raised when new items are added to the list.
    /// </summary>
    /// <param name="items">The items that are added.</param>
    protected override void OnItemsAdded(IEnumerable<IndexedItem<T>> items) {
      foreach (var indexedItem in items)
        if (!checkedState.ContainsKey(indexedItem.Value))
          checkedState.Add(indexedItem.Value, true);
      base.OnItemsAdded(items);
    }

    /// <summary>
    /// Raised when items are removed from the list.
    /// </summary>
    /// <param name="items">Items that are removed.</param>
    protected override void OnItemsRemoved(IEnumerable<IndexedItem<T>> items) {
      foreach (var indexedItem in items)
        if (!list.Contains(indexedItem.Value))
          checkedState.Remove(indexedItem.Value);
      base.OnItemsRemoved(items);
    }

    /// <summary>
    /// Raised when items are replaced.
    /// </summary>
    /// <param name="items">The items which replace <paramref name="oldItems"/></param>
    /// <param name="oldItems">The items that are replaced by <paramref name="items"/></param>
    protected override void OnItemsReplaced(IEnumerable<IndexedItem<T>> items, IEnumerable<IndexedItem<T>> oldItems) {
      foreach (var oldIndexedItem in oldItems)
        if (!list.Contains(oldIndexedItem.Value))
          checkedState.Remove(oldIndexedItem.Value);
      foreach (var indexedItem in items)
        if (!checkedState.ContainsKey(indexedItem.Value))
          checkedState.Add(indexedItem.Value, true);
      base.OnItemsReplaced(items, oldItems);
    }

    /// <summary>
    /// Raised after the checked state of items has been changed.
    /// </summary>
    /// <param name="items">The items whose check state has been changed.</param>
    protected virtual void OnCheckedItemsChanged(IEnumerable<IndexedItem<T>> items) {
      RaiseCheckedItemsChanged(new CollectionItemsChangedEventArgs<IndexedItem<T>>(items));
    }

    /// <summary>
    /// Raised after the checked state of items has been changed.
    /// </summary>
    public event CollectionItemsChangedEventHandler<IndexedItem<T>> CheckedItemsChanged;
    private void RaiseCheckedItemsChanged(CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      var handler = CheckedItemsChanged;
      if (handler != null) handler(this, e);
    }

    /// <summary>
    /// Creates a new deep clone of the CheckedItemList.
    /// </summary>
    /// <param name="cloner"></param>
    /// <returns>A deep clone of the CheckedItemList</returns>
    public override IDeepCloneable Clone(Cloner cloner) {
      return new CheckedItemList<T>(this, cloner);
    }
  }
}
