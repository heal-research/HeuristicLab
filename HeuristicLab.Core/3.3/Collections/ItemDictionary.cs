#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [StorableClass]
  [Item("ItemDictionary", "Represents a dictionary of items.")]
  public class ItemDictionary<TKey, TValue> : ObservableDictionary<TKey, TValue>, IItemDictionary<TKey, TValue>
    where TKey : class, IItem
    where TValue : class, IItem {

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
    protected ItemDictionary(bool deserializing) : base(deserializing) { }
    protected ItemDictionary(ItemDictionary<TKey, TValue> original, Cloner cloner) {
      cloner.RegisterClonedObject(original, this);
      foreach (TKey key in original.dict.Keys)
        dict.Add(cloner.Clone(key), cloner.Clone(original.dict[key]));
    }
    public ItemDictionary() : base() { }
    public ItemDictionary(int capacity) : base(capacity) { }
    public ItemDictionary(IEqualityComparer<TKey> comparer) : base(comparer) { }
    public ItemDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }
    public ItemDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer) { }
    public ItemDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer) { }

    public object Clone() {
      return Clone(new Cloner());
    }
    public virtual IDeepCloneable Clone(Cloner cloner) {
      return new ItemDictionary<TKey, TValue>(this, cloner);
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
