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
  [Item("ReadOnlyKeyedItemCollection<TKey, TItem>", "Represents a read-only keyed collection of items.")]
  public class ReadOnlyKeyedItemCollection<TKey, TItem> : ReadOnlyObservableKeyedCollection<TKey, TItem>, IKeyedItemCollection<TKey, TItem> where TItem : class, IItem {
    private string filename;
    public string Filename {
      get { return filename; }
      set {
        if (value == null) throw new ArgumentNullException();
        if ((filename == null) || !filename.Equals(value)) {
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
    private IObservableKeyedCollection<TKey, TItem> Items {
      get { return collection; }
      set { collection = value; }
    }

    protected ReadOnlyKeyedItemCollection() : base() { }
    public ReadOnlyKeyedItemCollection(IKeyedItemCollection<TKey, TItem> collection) : base(collection) { }
    [StorableConstructor]
    protected ReadOnlyKeyedItemCollection(bool deserializing) { }

    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      RegisterEvents();
    }

    public object Clone() {
      return Clone(new Cloner());
    }
    public virtual IDeepCloneable Clone(Cloner cloner) {
      ReadOnlyKeyedItemCollection<TKey, TItem> clone = (ReadOnlyKeyedItemCollection<TKey, TItem>)Activator.CreateInstance(this.GetType());
      cloner.RegisterClonedObject(this, clone);
      clone.collection = (IKeyedItemCollection<TKey, TItem>)((IKeyedItemCollection<TKey, TItem>)collection).Clone(cloner);
      clone.Initialize();
      return clone;
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
