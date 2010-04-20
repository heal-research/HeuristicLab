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
  [Item("ReadOnlyItemSet<T>", "Represents a read-only set of items.")]
  public class ReadOnlyItemSet<T> : ReadOnlyObservableSet<T>, IItemSet<T> where T : class, IItem {
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
    private IObservableSet<T> Items {
      get { return set; }
      set { set = value; }
    }

    public ReadOnlyItemSet() : base(new ItemSet<T>()) { }
    public ReadOnlyItemSet(IItemSet<T> set) : base(set) { }
    [StorableConstructor]
    protected ReadOnlyItemSet(bool deserializing) { }

    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      RegisterEvents();
    }

    public object Clone() {
      return Clone(new Cloner());
    }
    public virtual IDeepCloneable Clone(Cloner cloner) {
      ReadOnlyItemSet<T> clone = (ReadOnlyItemSet<T>)Activator.CreateInstance(this.GetType());
      cloner.RegisterClonedObject(this, clone);
      clone.set = (IItemSet<T>)((IItemSet<T>)set).Clone(cloner);
      clone.Initialize();
      return clone;
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
