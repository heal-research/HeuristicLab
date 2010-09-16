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
  [Item("ItemCollection", "Represents a collection of items.")]
  public class ItemCollection<T> : ObservableCollection<T>, IItemCollection<T> where T : class, IItem {
    public virtual string ItemName {
      get { return ItemAttribute.GetName(this.GetType()); }
    }
    public virtual string ItemDescription {
      get { return ItemAttribute.GetDescription(this.GetType()); }
    }
    public Version ItemVersion {
      get { return ItemAttribute.GetVersion(this.GetType()); }
    }
    public virtual Image ItemImage {
      get { return VS2008ImageLibrary.Class; }
    }

    public ItemCollection() : base() { }
    public ItemCollection(int capacity) : base(capacity) { }
    public ItemCollection(IEnumerable<T> collection) : base(collection) { }
    [StorableConstructor]
    protected ItemCollection(bool deserializing) : base(deserializing) { }

    public object Clone() {
      return Clone(new Cloner());
    }
    public virtual IDeepCloneable Clone(Cloner cloner) {
      ItemCollection<T> clone = (ItemCollection<T>)Activator.CreateInstance(this.GetType());
      cloner.RegisterClonedObject(this, clone);
      clone.list = new List<T>(this.Select(x => (T)cloner.Clone(x)));
      return clone;
    }

    public new ReadOnlyItemCollection<T> AsReadOnly() {
      return new ReadOnlyItemCollection<T>(this);
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
