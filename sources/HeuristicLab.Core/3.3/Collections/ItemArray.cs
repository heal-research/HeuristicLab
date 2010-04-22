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
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Collections;

namespace HeuristicLab.Core {
  [StorableClass]
  [Item("ItemArray<T>", "Represents an array of items.")]
  public class ItemArray<T> : ObservableArray<T>, IItemArray<T> where T : class, IItem {
    private string filename;
    public string Filename {
      get { return filename; }
      set {
        if (!filename.Equals(value)) {
          filename = value;
          OnFilenameChanged();
        }
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

    [Storable]
    private T[] Items {
      get { return array; }
      set { array = value; }
    }

    public ItemArray() : base() { }
    public ItemArray(int length) : base(length) { }
    public ItemArray(T[] array) : base(array) { }
    public ItemArray(IEnumerable<T> collection) : base(collection) { }
    [StorableConstructor]
    protected ItemArray(bool deserializing) { }

    public object Clone() {
      return Clone(new Cloner());
    }
    public virtual IDeepCloneable Clone(Cloner cloner) {
      ItemArray<T> clone = (ItemArray<T>)Activator.CreateInstance(this.GetType());
      cloner.RegisterClonedObject(this, clone);
      clone.array = this.Select(x => (T)cloner.Clone(x)).ToArray();
      return clone;
    }

    public new ReadOnlyItemArray<T> AsReadOnly() {
      return new ReadOnlyItemArray<T>(this);
    }

    public override string ToString() {
      return ItemName;
    }

    public event EventHandler FilenameChanged;
    protected virtual void OnFilenameChanged() {
      EventHandler handler = FilenameChanged;
      if (handler != null) handler(this, EventArgs.Empty);
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
