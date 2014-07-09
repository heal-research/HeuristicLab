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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Collections {
  [StorableClass]
  [Serializable]
  public class ReadOnlyObservableKeyedList<TKey, TItem> : ReadOnlyObservableList<TItem>, IObservableKeyedList<TKey, TItem> {

    protected IObservableKeyedList<TKey, TItem> KeyedList {
      get { return (IObservableKeyedList<TKey, TItem>)base.list; }
    }

    bool ICollection<TItem>.IsReadOnly {
      get { return true; }
    }
    public TItem this[TKey key] {
      get { return KeyedList[key]; }
    }

    #region Constructors
    protected ReadOnlyObservableKeyedList() { }
    public ReadOnlyObservableKeyedList(IObservableKeyedList<TKey, TItem> keyedList)
      : base(keyedList) {
      RegisterEvents();
    }
    [StorableConstructor]
    protected ReadOnlyObservableKeyedList(bool deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }
    #endregion

    public bool ContainsKey(TKey key) {
      return KeyedList.ContainsKey(key);
    }
    public bool TryGetValue(TKey key, out TItem item) {
      return KeyedList.TryGetValue(key, out item);
    }

    bool IObservableKeyedCollection<TKey, TItem>.Remove(TKey key) {
      throw new NotSupportedException();
    }

    #region events
    private void RegisterEvents() {
      ((INotifyObservableKeyedCollectionItemsChanged<TKey, TItem>)KeyedList).ItemsReplaced += keyedList_ItemsReplaced;
    }

    [field: NonSerialized]
    private event CollectionItemsChangedEventHandler<TItem> itemsReplaced;
    event CollectionItemsChangedEventHandler<TItem> INotifyObservableKeyedCollectionItemsChanged<TKey, TItem>.ItemsReplaced {
      add { itemsReplaced += value; }
      remove { itemsReplaced -= value; }
    }
    private void OnItemsReplaced(IEnumerable<TItem> items, IEnumerable<TItem> oldItems) {
      var handler = itemsReplaced;
      if (handler != null) handler(this, new CollectionItemsChangedEventArgs<TItem>(items, oldItems));
    }

    private void keyedList_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<TItem> e) {
      OnItemsReplaced(e.Items, e.OldItems);
    }

    #endregion
  }
}
