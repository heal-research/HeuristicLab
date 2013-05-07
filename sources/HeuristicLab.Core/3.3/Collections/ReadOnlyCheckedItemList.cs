#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  [StorableClass]
  [Item("ReadOnlyCheckedItemList", "Represents a read-only list of checked items.")]
  public class ReadOnlyCheckedItemList<T> : ReadOnlyItemList<T>, ICheckedItemList<T> where T : class, IItem {
    private CheckedItemList<T> CheckedItemList {
      get { return (CheckedItemList<T>)base.list; }
    }

    [StorableConstructor]
    protected ReadOnlyCheckedItemList(bool deserializing) : base(deserializing) { }
    protected ReadOnlyCheckedItemList(ReadOnlyCheckedItemList<T> original, Cloner cloner)
      : base(original, cloner) {
      CheckedItemList.CheckedItemsChanged += new CollectionItemsChangedEventHandler<IndexedItem<T>>(list_CheckedItemsChanged);
    }
    public ReadOnlyCheckedItemList() : base(new CheckedItemList<T>()) { }
    public ReadOnlyCheckedItemList(ICheckedItemList<T> list)
      : base(list) {
      CheckedItemList.CheckedItemsChanged += new CollectionItemsChangedEventHandler<IndexedItem<T>>(list_CheckedItemsChanged);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      CheckedItemList.CheckedItemsChanged += new CollectionItemsChangedEventHandler<IndexedItem<T>>(list_CheckedItemsChanged);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ReadOnlyCheckedItemList<T>(this, cloner);
    }


    #region ICheckedItemList<T> Members
    public event CollectionItemsChangedEventHandler<IndexedItem<T>> CheckedItemsChanged;
    protected virtual void list_CheckedItemsChanged(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      var handler = CheckedItemsChanged;
      if (handler != null)
        handler(this, e);
    }

    public IEnumerable<IndexedItem<T>> CheckedItems {
      get { return CheckedItemList.CheckedItems; }
    }

    public bool ItemChecked(T item) {
      return CheckedItemList.ItemChecked(item);
    }

    public void SetItemCheckedState(T item, bool checkedState) {
      CheckedItemList.SetItemCheckedState(item, checkedState);
    }

    public void Add(T item, bool checkedState) {
      throw new NotSupportedException();
    }

    public void Insert(int index, T item, bool checkedState) {
      throw new NotSupportedException();
    }

    #endregion
  }
}
