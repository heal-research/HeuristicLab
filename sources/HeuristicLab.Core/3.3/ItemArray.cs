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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Drawing;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common.Resources;
using HeuristicLab.Collections;

namespace HeuristicLab.Core {
  [EmptyStorableClass]
  [Item("ItemArray<T>", "Represents an array of items.")]
  public class ItemArray<T> : ObservableArray<T>, IItem where T : class, IItem {
    [Storable(Name = "RestoreEvents")]
    private object RestoreEvents {
      get { return null; }
      set {
        foreach (T item in this)
          if (item != null) item.Changed += new ChangedEventHandler(Item_Changed);
      }
    }

    public virtual string ItemName {
      get { return ItemAttribute.GetName(this.GetType()); }
    }
    public virtual string ItemDescription {
      get { return ItemAttribute.GetDescription(this.GetType()); }
    }
    public virtual Image ItemImage {
      get { return VS2008ImageLibrary.Class; }
    }

    public ItemArray() : base() { }
    public ItemArray(int length) : base(length) { }
    public ItemArray(T[] array) : base(array) {
      foreach (T item in this)
        if (item != null) item.Changed += new ChangedEventHandler(Item_Changed);
    }
    public ItemArray(IEnumerable<T> collection) : base(collection) {
      foreach (T item in this)
        if (item != null) item.Changed += new ChangedEventHandler(Item_Changed);
    }

    public object Clone() {
      return Clone(new Cloner());
    }

    public virtual IDeepCloneable Clone(Cloner cloner) {
      ItemArray<T> clone = (ItemArray<T>)Activator.CreateInstance(this.GetType(), this.Select(x => (T)cloner.Clone(x)));
      cloner.RegisterClonedObject(this, clone);
      return clone;
    }

    public override string ToString() {
      return ItemName;
    }

    public event ChangedEventHandler Changed;
    protected void OnChanged() {
      OnChanged(new ChangedEventArgs());
    }
    protected virtual void OnChanged(ChangedEventArgs e) {
      if ((e.RegisterChangedObject(this)) && (Changed != null))
        Changed(this, e);
    }

    protected override void OnItemsReplaced(IEnumerable<IndexedItem<T>> items, IEnumerable<IndexedItem<T>> oldItems) {
      foreach (IndexedItem<T> oldItem in oldItems)
        if (oldItem.Value != null) oldItem.Value.Changed -= new ChangedEventHandler(Item_Changed);
      foreach (IndexedItem<T> item in items)
        if (item.Value != null) item.Value.Changed += new ChangedEventHandler(Item_Changed);
      base.OnItemsReplaced(items, oldItems);
    }
    protected override void OnCollectionReset(IEnumerable<IndexedItem<T>> items, IEnumerable<IndexedItem<T>> oldItems) {
      foreach (IndexedItem<T> oldItem in oldItems)
        if (oldItem.Value != null) oldItem.Value.Changed -= new ChangedEventHandler(Item_Changed);
      foreach (IndexedItem<T> item in items)
        if (item.Value != null) item.Value.Changed += new ChangedEventHandler(Item_Changed);
      base.OnCollectionReset(items, oldItems);
    }
    protected override void OnPropertyChanged(string propertyName) {
      base.OnPropertyChanged(propertyName);
      OnChanged();
    }

    private void Item_Changed(object sender, ChangedEventArgs e) {
      OnChanged(e);
    }
  }
}
