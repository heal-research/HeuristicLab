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
using HeuristicLab.Common.Resources;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  [StorableClass]
  [Item("ItemDictionary<TKey, TValue>", "Represents a dictionary of items.")]
  public class ItemDictionary<TKey, TValue> : ObservableDictionary<TKey, TValue>, IItemDictionary<TKey, TValue> where TKey : class, IItem where TValue : class, IItem {
    public virtual string ItemName {
      get { return ItemAttribute.GetName(this.GetType()); }
    }
    public virtual string ItemDescription {
      get { return ItemAttribute.GetDescription(this.GetType()); }
    }
    public virtual Image ItemImage {
      get { return VS2008ImageLibrary.Class; }
    }

    [Storable]
    private Dictionary<TKey, TValue> Items {
      get { return dict; }
      set { dict = value; }
    }

    public ItemDictionary() : base() { }
    public ItemDictionary(int capacity) : base(capacity) { }
    public ItemDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }
    [StorableConstructor]
    protected ItemDictionary(bool deserializing) { }

    public object Clone() {
      return Clone(new Cloner());
    }
    public virtual IDeepCloneable Clone(Cloner cloner) {
      ItemDictionary<TKey, TValue> clone = (ItemDictionary<TKey, TValue>)Activator.CreateInstance(this.GetType());
      cloner.RegisterClonedObject(this, clone);
      foreach (TKey key in dict.Keys)
        clone.dict.Add((TKey)cloner.Clone(key), (TValue)cloner.Clone(dict[key]));
      return clone;
    }

    public new ReadOnlyItemDictionary<TKey, TValue> AsReadOnly() {
      return new ReadOnlyItemDictionary<TKey, TValue>(this);
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
