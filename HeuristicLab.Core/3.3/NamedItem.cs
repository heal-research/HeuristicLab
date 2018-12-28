#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HEAL.Fossil;

namespace HeuristicLab.Core {
  [Item("NamedItem", "Base class for items which have a name and an optional description.")]
  [StorableType("7492EDE5-CA7C-45D5-B5CE-8709BFAC9239")]
  public abstract class NamedItem : Item, INamedItem {
    [Storable]
    protected string name;
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="OnNameChanging"/> and also <see cref="OnNameChanged"/> 
    /// eventually in the setter.</remarks>
    public string Name {
      get { return name; }
      set {
        if (!CanChangeName) throw new NotSupportedException("Name cannot be changed.");
        if (!(name.Equals(value) || (value == null) && (name == string.Empty))) {
          CancelEventArgs<string> e = value == null ? new CancelEventArgs<string>(string.Empty) : new CancelEventArgs<string>(value);
          OnNameChanging(e);
          if (!e.Cancel) {
            name = value == null ? string.Empty : value;
            OnNameChanged();
          }
        }
      }
    }
    public virtual bool CanChangeName {
      get { return true; }
    }
    [Storable]
    protected string description;
    public string Description {
      get { return description; }
      set {
        if (!CanChangeDescription) throw new NotSupportedException("Description cannot be changed.");
        if (!(description.Equals(value) || (value == null) && (description == string.Empty))) {
          description = value == null ? string.Empty : value;
          OnDescriptionChanged();
        }
      }
    }
    public virtual bool CanChangeDescription {
      get { return true; }
    }

    [StorableConstructor]
    protected NamedItem(StorableConstructorFlag _) : base(_) { }
    protected NamedItem(NamedItem original, Cloner cloner)
      : base(original, cloner) {
      name = original.name;
      description = original.description;
    }
    /// <summary>
    /// Initializes a new instance of <see cref="Variable"/> with name <c>Anonymous</c> 
    /// and value <c>null</c>.
    /// </summary>
    protected NamedItem() {
      name = string.Empty;
      description = string.Empty;
    }
    /// <summary>
    /// Initializes a new instance of <see cref="Variable"/> with the specified <paramref name="name"/>
    /// and the specified <paramref name="value"/>.
    /// </summary>
    /// <param name="name">The name of the current instance.</param>
    /// <param name="value">The value of the current instance.</param>
    protected NamedItem(string name) {
      if (name == null) this.name = string.Empty;
      else this.name = name;
      description = string.Empty;
    }
    protected NamedItem(string name, string description) {
      if (name == null) this.name = string.Empty;
      else this.name = name;
      if (description == null) this.description = string.Empty;
      else this.description = description;
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
      var handler = NameChanging;
      if (handler != null) handler(this, e);
    }
    /// <inheritdoc/>
    public event EventHandler NameChanged;
    /// <summary>
    /// Fires a new <c>NameChanged</c> event.
    /// </summary>
    /// <remarks>Calls <see cref="ItemBase.OnChanged"/>.</remarks>
    protected virtual void OnNameChanged() {
      var handler = NameChanged;
      if (handler != null) handler(this, EventArgs.Empty);
      OnToStringChanged();
    }
    /// <inheritdoc/>
    public event EventHandler DescriptionChanged;
    /// <summary>
    /// Fires a new <c>DescriptionChanged</c> event.
    /// </summary>
    /// <remarks>Calls <see cref="ItemBase.OnChanged"/>.</remarks>
    protected virtual void OnDescriptionChanged() {
      var handler = DescriptionChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
  }
}
