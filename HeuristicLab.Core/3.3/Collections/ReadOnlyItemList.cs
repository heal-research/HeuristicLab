#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  [StorableClass]
  [Item("ReadOnlyItemList", "Represents a read-only list of items.")]
  public class ReadOnlyItemList<T> : ReadOnlyObservableList<T>, IItemList<T> where T : class, IItem {
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
    protected ReadOnlyItemList(bool deserializing) : base(deserializing) { }
    protected ReadOnlyItemList(ReadOnlyItemList<T> original, Cloner cloner)
      : base(cloner.Clone((IItemList<T>)original.list)) {
      cloner.RegisterClonedObject(original, this);
    }
    public ReadOnlyItemList() : base(new ItemList<T>()) { }
    public ReadOnlyItemList(IItemList<T> list) : base(list) { }

    public object Clone() {
      return Clone(new Cloner());
    }
    public virtual IDeepCloneable Clone(Cloner cloner) {
      return new ReadOnlyItemList<T>(this, cloner);
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
