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
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.OKB.RunCreation {
  [Item("ItemWrapper", "A wrapper which wraps an IItem.")]
  [StorableClass]
  [NonDiscoverableType]
  public class ItemWrapper<T> : IItem where T : class, IItem {
    private T wrappedItem;
    protected virtual T WrappedItem {
      get { return wrappedItem; }
      set {
        if (value == null) throw new ArgumentNullException("WrappedItem", "WrappedItem cannot be null.");
        if (value != wrappedItem) {
          DeregisterWrappedItemEvents();
          wrappedItem = value;
          RegisterWrappedItemEvents();

          OnToStringChanged();
          OnItemImageChanged();
          OnWrappedItemChanged();
        }
      }
    }

    public virtual string ItemName {
      get { return wrappedItem.ItemName; }
    }
    public virtual string ItemDescription {
      get { return wrappedItem.ItemDescription; }
    }
    public Version ItemVersion {
      get { return wrappedItem.ItemVersion; }
    }
    public virtual Image ItemImage {
      get { return WrappedItem.ItemImage; }
    }

    #region Persistence Properties
    [Storable(Name = "WrappedItem")]
    private T StorableWrappedItem {
      get { return wrappedItem; }
      set {
        wrappedItem = value;
        RegisterWrappedItemEvents();
      }
    }
    #endregion

    [StorableConstructor]
    protected ItemWrapper(bool deserializing) { }
    protected ItemWrapper(ItemWrapper<T> original, Cloner cloner) {
      cloner.RegisterClonedObject(original, this);
      wrappedItem = cloner.Clone(original.wrappedItem);
      RegisterWrappedItemEvents();
    }
    public ItemWrapper(T item) {
      if (item == null) throw new ArgumentNullException("item", "ItemWrapper cannot wrap a null reference.");
      wrappedItem = item;
      RegisterWrappedItemEvents();
    }

    public object Clone() {
      return Clone(new Cloner());
    }
    public virtual IDeepCloneable Clone(Cloner cloner) {
      return new ItemWrapper<T>(this, cloner);
    }

    public override string ToString() {
      return wrappedItem.ToString();
    }

    protected virtual void OnWrappedItemChanged() { }

    #region Events
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

    protected virtual void RegisterWrappedItemEvents() {
      WrappedItem.ToStringChanged += new EventHandler(WrappedItem_ToStringChanged);
      WrappedItem.ItemImageChanged += new EventHandler(WrappedItem_ItemImageChanged);
    }
    protected virtual void DeregisterWrappedItemEvents() {
      WrappedItem.ToStringChanged -= new EventHandler(WrappedItem_ToStringChanged);
      WrappedItem.ItemImageChanged -= new EventHandler(WrappedItem_ItemImageChanged);
    }

    protected virtual void WrappedItem_ToStringChanged(object sender, EventArgs e) {
      OnToStringChanged();
    }
    protected virtual void WrappedItem_ItemImageChanged(object sender, EventArgs e) {
      OnItemImageChanged();
    }
    #endregion
  }
}
