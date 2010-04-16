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
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  /// <summary>
  /// Represents the base class for all basic item types.
  /// </summary>
  [StorableClass]
  [Item("Item", "Base class for all HeuristicLab items.")]
  public abstract class Item : IDeepCloneable, IItem {
    public virtual string ItemName {
      get { return ItemAttribute.GetName(this.GetType()); }
    }
    public virtual string ItemDescription {
      get { return ItemAttribute.GetDescription(this.GetType()); }
    }
    public virtual Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Class; }
    }

    [Storable]
    private bool readOnlyView;
    public virtual bool ReadOnlyView {
      get { return readOnlyView; }
      set {
        if (readOnlyView != value) {
          readOnlyView = value;
          OnReadOnlyViewChanged();
        }
      }
    }

    protected Item()
      : base() {
      readOnlyView = false;
    }
    [StorableConstructor]
    protected Item(bool deserializing) { }


    /// <summary>
    /// Creates a deep clone of this instance.
    /// </summary>
    /// <remarks>
    /// This method is the entry point for creating a deep clone of a whole object graph.
    /// </remarks>
    /// <returns>A clone of this instance.</returns>
    public object Clone() {
      return Clone(new Cloner());
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <param name="clonedObjects">Dictionary of all already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="Variable"/>.</returns>
    public virtual IDeepCloneable Clone(Cloner cloner) {
      Item clone = (Item)Activator.CreateInstance(this.GetType());
      cloner.RegisterClonedObject(this, clone);
      clone.readOnlyView = readOnlyView;
      return clone;
    }

    /// <summary>
    /// Gets the string representation of the current instance.
    /// </summary>
    /// <returns>The type name of the current instance.</returns>
    public override string ToString() {
      return ItemName;
    }

    public event EventHandler ItemImageChanged;
    protected virtual void OnItemImageChanged() {
      EventHandler handler = ItemImageChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler ReadOnlyViewChanged;
    protected virtual void OnReadOnlyViewChanged() {
      EventHandler handler = ReadOnlyViewChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler ToStringChanged;
    protected virtual void OnToStringChanged() {
      EventHandler handler = ToStringChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
  }
}
