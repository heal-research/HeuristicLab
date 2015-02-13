#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  [Item("KeyedItemCollection", "Represents a keyed collection of items.")]
  [StorableClass]
  public abstract class KeyedItemCollection<TKey, TItem> : ObservableKeyedCollection<TKey, TItem>, IKeyedItemCollection<TKey, TItem> where TItem : class, IItem {
    public virtual string ItemName {
      get { return ItemAttribute.GetName(this.GetType()); }
    }
    public virtual string ItemDescription {
      get { return ItemAttribute.GetDescription(this.GetType()); }
    }
    public Version ItemVersion {
      get { return ItemAttribute.GetVersion(this.GetType()); }
    }
    public static Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Class; }
    }
    public virtual Image ItemImage {
      get { return ItemAttribute.GetImage(this.GetType()); }
    }

    [StorableConstructor]
    protected KeyedItemCollection(bool deserializing) : base(deserializing) { }
    protected KeyedItemCollection(KeyedItemCollection<TKey, TItem> original, Cloner cloner) {
      cloner.RegisterClonedObject(original, this);
      foreach (TItem item in original.dict.Values) {
        TItem clonedItem = cloner.Clone(item);
        dict.Add(GetKeyForItem(clonedItem), clonedItem);
      }
    }
    protected KeyedItemCollection() : base() { }
    protected KeyedItemCollection(int capacity) : base(capacity) { }
    protected KeyedItemCollection(IEnumerable<TItem> collection) : base(collection) { }

    public object Clone() {
      return Clone(new Cloner());
    }
    public abstract IDeepCloneable Clone(Cloner cloner);

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
  }
}
