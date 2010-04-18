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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  [Item("KeyedItemCollection<TKey, TItem>", "Represents a keyed collection of items.")]
  [StorableClass]
  public abstract class KeyedItemCollection<TKey, TItem> : ObservableKeyedCollection<TKey, TItem>, IKeyedItemCollection<TKey, TItem> where TItem : class, IItem {
    public virtual string ItemName {
      get { return ItemAttribute.GetName(this.GetType()); }
    }
    public virtual string ItemDescription {
      get { return ItemAttribute.GetDescription(this.GetType()); }
    }
    public virtual Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Class; }
    }

    [Storable]
    private Dictionary<TKey, TItem> Items {
      get { return dict; }
      set { dict = value; }
    }

    [Storable]
    private bool readOnlyView;
    public virtual bool ReadOnlyView {
      get { return readOnlyView; }
      set {
        if ((readOnlyView != value) && !((ICollection<KeyValuePair<TKey, TItem>>)dict).IsReadOnly) {
          readOnlyView = value;
          OnReadOnlyViewChanged();
          OnPropertyChanged("ReadOnlyView");
        }
      }
    }

    protected KeyedItemCollection() : base() {
      readOnlyView = ((ICollection<KeyValuePair<TKey, TItem>>)dict).IsReadOnly;
    }
    protected KeyedItemCollection(int capacity) : base(capacity) {
      readOnlyView = ((ICollection<KeyValuePair<TKey, TItem>>)dict).IsReadOnly;
    }
    protected KeyedItemCollection(IEnumerable<TItem> collection) : base(collection) {
      readOnlyView = ((ICollection<KeyValuePair<TKey, TItem>>)dict).IsReadOnly;
    }
    [StorableConstructor]
    protected KeyedItemCollection(bool deserializing) { }

    public object Clone() {
      return Clone(new Cloner());
    }
    public virtual IDeepCloneable Clone(Cloner cloner) {
      KeyedItemCollection<TKey, TItem> clone = (KeyedItemCollection<TKey, TItem>)Activator.CreateInstance(this.GetType());
      cloner.RegisterClonedObject(this, clone);
      clone.readOnlyView = readOnlyView;
      foreach (TItem item in dict.Values) {
        TItem clonedItem = (TItem)cloner.Clone(item);
        clone.dict.Add(GetKeyForItem(clonedItem), clonedItem);
      }
      return clone;
    }

    public new ReadOnlyKeyedItemCollection<TKey, TItem> AsReadOnly() {
      return new ReadOnlyKeyedItemCollection<TKey, TItem>(this);
    }

    public override string ToString() {
      return ItemName;
    }

    public event EventHandler ItemImageChanged;
    protected virtual void OnItemImageChanged() {
      EventHandler handler = ItemImageChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler ToStringChanged;
    protected virtual void OnToStringChanged() {
      EventHandler handler = ToStringChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler ReadOnlyViewChanged;
    protected virtual void OnReadOnlyViewChanged() {
      EventHandler handler = ReadOnlyViewChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
  }
}
