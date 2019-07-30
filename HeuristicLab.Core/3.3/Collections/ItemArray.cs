#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;

namespace HeuristicLab.Core {
  [StorableType("CB8A238C-B8E6-47A3-A7D8-AB1212DEC672")]
  [Item("ItemArray", "Represents an array of items.")]
  public class ItemArray<T> : ObservableArray<T>, IItemArray<T> where T : class, IItem {
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
    protected ItemArray(StorableConstructorFlag _) : base(_) { }
    protected ItemArray(ItemArray<T> original, Cloner cloner) {
      cloner.RegisterClonedObject(original, this);
      array = original.Select(x => cloner.Clone(x)).ToArray();
    }
    public ItemArray() : base() { }
    public ItemArray(int length) : base(length) { }
    public ItemArray(T[] array) : base(array) { }
    public ItemArray(IEnumerable<T> collection) : base(collection) { }

    public object Clone() {
      return Clone(new Cloner());
    }
    public virtual IDeepCloneable Clone(Cloner cloner) {
      return new ItemArray<T>(this, cloner);
    }

    public new ReadOnlyItemArray<T> AsReadOnly() {
      return new ReadOnlyItemArray<T>(this);
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
