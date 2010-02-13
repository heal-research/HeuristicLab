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
using System.Text;
using System.Xml;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;

namespace HeuristicLab.Core {
  [Item("NamedItem", "Base class for items which have a name and an optional description.")]
  public abstract class NamedItem : Item, INamedItem {
    [Storable]
    private string name;
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="OnNameChanging"/> and also <see cref="OnNameChanged"/> 
    /// eventually in the setter.</remarks>
    public string Name {
      get { return name; }
      set {
        if (!CanChangeName) throw new NotSupportedException("Name of NamedItem cannot be changed.");
        if (value == null) throw new ArgumentNullException();
        if (!name.Equals(value)) {
          CancelEventArgs<string> e = new CancelEventArgs<string>(value);
          OnNameChanging(e);
          if (!e.Cancel) {
            name = value;
            OnNameChanged();
          }
        }
      }
    }
    public virtual bool CanChangeName {
      get { return true; }
    }
    [Storable]
    private string description;
    public string Description {
      get { return description; }
      set {
        if (!CanChangeDescription) throw new NotSupportedException("Description of NamedItem cannot be changed.");
        if ((description == null) || (!description.Equals(value))) {
          description = value;
          OnDescriptionChanged();
        }
      }
    }
    public virtual bool CanChangeDescription {
      get { return true; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="Variable"/> with name <c>Anonymous</c> 
    /// and value <c>null</c>.
    /// </summary>
    protected NamedItem() {
      name = ItemName;
      description = ItemDescription;
    }
    /// <summary>
    /// Initializes a new instance of <see cref="Variable"/> with the specified <paramref name="name"/>
    /// and the specified <paramref name="value"/>.
    /// </summary>
    /// <param name="name">The name of the current instance.</param>
    /// <param name="value">The value of the current instance.</param>
    protected NamedItem(string name)
      : this() {
      if (name == null) throw new ArgumentNullException();
      this.name = name;
    }
    protected NamedItem(string name, string description)
      : this(name) {
      this.description = description;
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <param name="clonedObjects">Dictionary of all already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="Variable"/>.</returns>
    public override IDeepCloneable Clone(Cloner cloner) {
      NamedItem clone = (NamedItem)base.Clone(cloner);
      clone.name = name;
      clone.description = description;
      return clone;
    }

    /// <summary>
    /// Gets the string representation of the current instance in the format: <c>Name: [null|Value]</c>.
    /// </summary>
    /// <returns>The current instance as a string.</returns>
    public override string ToString() {
      return Name;
    }

    /// <inheritdoc/>
    public event EventHandler<CancelEventArgs<string>> NameChanging;
    /// <summary>
    /// Fires a new <c>NameChanging</c> event.
    /// </summary>
    /// <param name="e">The event arguments of the changing.</param>
    protected virtual void OnNameChanging(CancelEventArgs<string> e) {
      if (NameChanging != null)
        NameChanging(this, e);
    }
    /// <inheritdoc/>
    public event EventHandler NameChanged;
    /// <summary>
    /// Fires a new <c>NameChanged</c> event.
    /// </summary>
    /// <remarks>Calls <see cref="ItemBase.OnChanged"/>.</remarks>
    protected virtual void OnNameChanged() {
      if (NameChanged != null)
        NameChanged(this, new EventArgs());
      OnChanged();
    }
    /// <inheritdoc/>
    public event EventHandler DescriptionChanged;
    /// <summary>
    /// Fires a new <c>DescriptionChanged</c> event.
    /// </summary>
    /// <remarks>Calls <see cref="ItemBase.OnChanged"/>.</remarks>
    protected virtual void OnDescriptionChanged() {
      if (DescriptionChanged != null)
        DescriptionChanged(this, new EventArgs());
      OnChanged();
    }
  }
}
