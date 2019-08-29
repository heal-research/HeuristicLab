#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
using HeuristicLab.Collections;
using HeuristicLab.Common;

namespace HeuristicLab.Core {
  [StorableType("81B63F7B-0AD4-4BAD-8F39-BAB1F6B006C8")]
  [Item("ReadOnlyCheckedItemCollection", "Represents a read-only collection of checked items.")]
  public class ReadOnlyCheckedItemCollection<T> : ReadOnlyItemCollection<T>, ICheckedItemCollection<T> where T : class, IItem {
    private CheckedItemCollection<T> CheckedItemCollection {
      get { return (CheckedItemCollection<T>)base.collection; }
    }

    [StorableConstructor]
    protected ReadOnlyCheckedItemCollection(StorableConstructorFlag _) : base(_) { }
    protected ReadOnlyCheckedItemCollection(ReadOnlyCheckedItemCollection<T> original, Cloner cloner)
      : base(original, cloner) {
      CheckedItemCollection.CheckedItemsChanged += new CollectionItemsChangedEventHandler<T>(collection_CheckedItemsChanged);
    }
    public ReadOnlyCheckedItemCollection() : base(new CheckedItemCollection<T>()) { }
    public ReadOnlyCheckedItemCollection(ICheckedItemCollection<T> collection)
      : base(collection) {
      CheckedItemCollection.CheckedItemsChanged += new CollectionItemsChangedEventHandler<T>(collection_CheckedItemsChanged);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      CheckedItemCollection.CheckedItemsChanged += new CollectionItemsChangedEventHandler<T>(collection_CheckedItemsChanged);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ReadOnlyCheckedItemCollection<T>(this, cloner);
    }

    #region ReadOnlyCheckedItemCollection<T> Members
    public event CollectionItemsChangedEventHandler<T> CheckedItemsChanged;
    protected virtual void collection_CheckedItemsChanged(object sender, CollectionItemsChangedEventArgs<T> e) {
      var handler = CheckedItemsChanged;
      if (handler != null)
        handler(this, e);
    }

    public IEnumerable<T> CheckedItems {
      get { return CheckedItemCollection.CheckedItems; }
    }

    public bool ItemChecked(T item) {
      return CheckedItemCollection.ItemChecked(item);
    }

    public void SetItemCheckedState(T item, bool checkedState) {
      CheckedItemCollection.SetItemCheckedState(item, checkedState);
    }

    public void SetItemCheckedState(IEnumerable<T> items, bool checkedState) {
      CheckedItemCollection.SetItemCheckedState(items, checkedState);
    }

    public void Add(T item, bool checkedState) {
      throw new NotSupportedException();
    }
    #endregion
  }
}
