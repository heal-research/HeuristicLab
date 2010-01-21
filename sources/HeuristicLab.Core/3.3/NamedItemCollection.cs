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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Collections;
using HeuristicLab.Common;

namespace HeuristicLab.Core {
  public class NamedItemCollection<T> : ObservableKeyedCollection<string, T>, IDeepCloneable where T : class, INamedItem {
    [Storable(Name = "RestoreEvents")]
    private object RestoreEvents {
      get { return null; }
      set {
        foreach (T item in this) {
          item.NameChanging += new EventHandler<CancelEventArgs<string>>(Item_NameChanging);
          item.NameChanged += new EventHandler(Item_NameChanged);
          item.Changed += new ChangedEventHandler(Item_Changed);
        }
      }
    }

    public NamedItemCollection() : base() { }
    public NamedItemCollection(int capacity) : base(capacity) { }
    public NamedItemCollection(IEnumerable<T> collection) : base(collection) { }

    public object Clone() {
      return Clone(new Cloner());
    }

    public IDeepCloneable Clone(Cloner cloner) {
      List<T> items = new List<T>();
      foreach (T item in this)
        items.Add((T)cloner.Clone(item));
      NamedItemCollection<T> clone = (NamedItemCollection<T>)Activator.CreateInstance(this.GetType(), items);
      cloner.RegisterClonedObject(this, clone);
      return clone;
    }

    public event ChangedEventHandler Changed;
    protected void OnChanged() {
      OnChanged(new ChangedEventArgs());
    }
    protected virtual void OnChanged(ChangedEventArgs e) {
      if ((e.RegisterChangedObject(this)) && (Changed != null))
        Changed(this, e);
    }

    protected override string GetKeyForItem(T item) {
      return item.Name;
    }

    protected override void OnItemsAdded(IEnumerable<T> items) {
      foreach (T item in items) {
        item.NameChanging += new EventHandler<CancelEventArgs<string>>(Item_NameChanging);
        item.NameChanged += new EventHandler(Item_NameChanged);
        item.Changed += new ChangedEventHandler(Item_Changed);
      }
      base.OnItemsAdded(items);
    }
    protected override void OnItemsRemoved(IEnumerable<T> items) {
      foreach (T item in items) {
        item.NameChanging -= new EventHandler<CancelEventArgs<string>>(Item_NameChanging);
        item.NameChanged -= new EventHandler(Item_NameChanged);
        item.Changed -= new ChangedEventHandler(Item_Changed);
      }
      base.OnItemsRemoved(items);
    }
    #region NOTE
    // NOTE: OnItemsReplaced is not overridden as ItemsReplaced is only fired
    // by ObservableKeyedCollectionBase when the key of an item has changed. The items stays
    // in the collection and therefore the NameChanging, NameChanged and Changed event handler
    // do not have to be removed and added again.
    #endregion
    protected override void OnCollectionReset(IEnumerable<T> items, IEnumerable<T> oldItems) {
      foreach (T oldItem in oldItems) {
        oldItem.NameChanging -= new EventHandler<CancelEventArgs<string>>(Item_NameChanging);
        oldItem.NameChanged -= new EventHandler(Item_NameChanged);
        oldItem.Changed -= new ChangedEventHandler(Item_Changed);
      }
      foreach (T item in items) {
        item.NameChanging += new EventHandler<CancelEventArgs<string>>(Item_NameChanging);
        item.NameChanged += new EventHandler(Item_NameChanged);
        item.Changed += new ChangedEventHandler(Item_Changed);
      }
      base.OnCollectionReset(items, oldItems);
    }
    protected override void OnPropertyChanged(string propertyName) {
      base.OnPropertyChanged(propertyName);
      OnChanged();
    }

    private void Item_NameChanging(object sender, CancelEventArgs<string> e) {
      e.Cancel = this.ContainsKey(e.Value);
    }
    private void Item_NameChanged(object sender, EventArgs e) {
      T item = (T)sender;
      UpdateItemKey(item);
    }
    private void Item_Changed(object sender, ChangedEventArgs e) {
      OnChanged(e);
    }
  }
}
