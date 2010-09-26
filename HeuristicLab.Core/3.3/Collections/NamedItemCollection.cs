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
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  [Item("NamedItemCollection", "Represents a collection of named items.")]
  [StorableClass]
  public class NamedItemCollection<T> : KeyedItemCollection<string, T> where T : class, INamedItem {
    public NamedItemCollection() : base() { }
    public NamedItemCollection(int capacity) : base(capacity) { }
    public NamedItemCollection(IEnumerable<T> collection)
      : base(collection) {
      Initialize();
    }
    [StorableConstructor]
    protected NamedItemCollection(bool deserializing) : base(deserializing) { }

    [StorableHook(HookType.AfterDeserialization)]
    protected void Initialize() {
      RegisterItemEvents(this);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      NamedItemCollection<T> clone = (NamedItemCollection<T>)base.Clone(cloner);
      clone.Initialize();
      return clone;
    }

    protected override string GetKeyForItem(T item) {
      return item.Name;
    }

    protected override void OnItemsAdded(IEnumerable<T> items) {
      RegisterItemEvents(items);
      base.OnItemsAdded(items);
    }
    protected override void OnItemsRemoved(IEnumerable<T> items) {
      DeregisterItemEvents(items);
      base.OnItemsRemoved(items);
    }
    #region NOTE
    // NOTE: OnItemsReplaced is not overridden as ItemsReplaced is only fired
    // by ObservableKeyedCollection when the key of an item has changed. The items stays
    // in the collection and therefore the NameChanging, NameChanged and Changed event handler
    // do not have to be removed and added again.
    #endregion
    protected override void OnCollectionReset(IEnumerable<T> items, IEnumerable<T> oldItems) {
      DeregisterItemEvents(oldItems);
      RegisterItemEvents(items);
      base.OnCollectionReset(items, oldItems);
    }

    private void RegisterItemEvents(IEnumerable<T> items) {
      foreach (T item in items) {
        if (item != null) {
          item.NameChanging += new EventHandler<CancelEventArgs<string>>(Item_NameChanging);
          item.NameChanged += new EventHandler(Item_NameChanged);
        }
      }
    }
    private void DeregisterItemEvents(IEnumerable<T> items) {
      foreach (T item in items) {
        if (item != null) {
          item.NameChanging -= new EventHandler<CancelEventArgs<string>>(Item_NameChanging);
          item.NameChanged -= new EventHandler(Item_NameChanged);
        }
      }
    }

    private void Item_NameChanging(object sender, CancelEventArgs<string> e) {
      e.Cancel = e.Cancel || this.ContainsKey(e.Value);
    }
    private void Item_NameChanged(object sender, EventArgs e) {
      T item = (T)sender;
      UpdateItemKey(item);
    }
  }
}
