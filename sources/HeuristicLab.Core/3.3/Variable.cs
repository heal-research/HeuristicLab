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
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  /// <summary>
  /// Represents a variable which has a name and holds an IItem.
  /// </summary>
  [Item("Variable", "A variable which has a name and holds an IItem.")]
  [Creatable("Test")]
  public sealed class Variable : NamedItem, IVariable {
    private IItem value;
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="OnValueChanged"/> in the setter.</remarks>
    [Storable]
    public IItem Value {
      get { return value; }
      set {
        if (this.value != value) {
          if (this.value != null) this.value.ToStringChanged -= new EventHandler(Value_ToStringChanged);
          this.value = value;
          if (this.value != null) this.value.ToStringChanged += new EventHandler(Value_ToStringChanged);
          OnValueChanged();
        }
      }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="Variable"/> with name <c>Anonymous</c> 
    /// and value <c>null</c>.
    /// </summary>
    public Variable()
      : base("Anonymous") {
      this.value = null;
    }
    /// <summary>
    /// Initializes a new instance of <see cref="Variable"/> with the specified <paramref name="name"/>
    /// and the specified <paramref name="value"/>.
    /// </summary>
    /// <param name="name">The name of the current instance.</param>
    /// <param name="value">The value of the current instance.</param>
    public Variable(string name)
      : base(name) {
      this.value = null;
    }
    public Variable(string name, string description)
      : base(name, description) {
      this.value = null;
    }
    public Variable(string name, IItem value)
      : base(name) {
      this.value = value;
      if (this.value != null) this.value.ToStringChanged += new EventHandler(Value_ToStringChanged);
    }
    public Variable(string name, string description, IItem value)
      : base(name, description) {
      this.value = value;
      if (this.value != null) this.value.ToStringChanged += new EventHandler(Value_ToStringChanged);
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <param name="clonedObjects">Dictionary of all already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="Variable"/>.</returns>
    public override IDeepCloneable Clone(Cloner cloner) {
      Variable clone = new Variable(Name, Description, (IItem)cloner.Clone(value));
      cloner.RegisterClonedObject(this, clone);
      return clone;
    }

    /// <summary>
    /// Gets the string representation of the current instance in the format: <c>Name: [null|Value]</c>.
    /// </summary>
    /// <returns>The current instance as a string.</returns>
    public override string ToString() {
      if (Value == null)
        return string.Format("{0}: null", Name);
      else
        return string.Format("{0}: {1} ({2})", Name, Value.ToString(), Value.GetType().GetPrettyName());
    }

    /// <inheritdoc/>
    public event EventHandler ValueChanged;
    /// <summary>
    /// Fires a new <c>ValueChanged</c> even.
    /// </summary>
    private void OnValueChanged() {
      if (ValueChanged != null)
        ValueChanged(this, EventArgs.Empty);
      OnToStringChanged();
    }

    private void Value_ToStringChanged(object sender, EventArgs e) {
      OnToStringChanged();
    }
  }
}
