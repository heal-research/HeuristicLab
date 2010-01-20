#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  /// <summary>
  /// Represents a variable which has a name and holds an IItem.
  /// </summary>
  [Item("Variable", "A variable which has a name and holds an IItem.")]
  [Creatable("Test")]
  public sealed class Variable : NamedItemBase {
    private IItem value;
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="OnValueChanged"/> in the setter.</remarks>
    [Storable]
    public IItem Value {
      get { return value; }
      set {
        if (this.value != value) {
          if (this.value != null) this.value.Changed -= new ChangedEventHandler(Value_Changed);
          this.value = value;
          if (this.value != null) this.value.Changed += new ChangedEventHandler(Value_Changed);
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
      Value = null;
    }
    /// <summary>
    /// Initializes a new instance of <see cref="Variable"/> with the specified <paramref name="name"/>
    /// and the specified <paramref name="value"/>.
    /// </summary>
    /// <param name="name">The name of the current instance.</param>
    /// <param name="value">The value of the current instance.</param>
    public Variable(string name, IItem value)
      : base(name) {
      Value = value;
    }

    /// <inheritdoc cref="IVariable.GetValue&lt;T&gt;"/>
    public T GetValue<T>() where T : class, IItem {
      return (T)Value;
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <param name="clonedObjects">Dictionary of all already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="Variable"/>.</returns>
    public override IDeepCloneable Clone(Cloner cloner) {
      Variable clone = new Variable();
      cloner.RegisterClonedObject(this, clone);
      clone.name = name;
      clone.description = description;
      clone.Value = (IItem)cloner.Clone(value);
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
        return string.Format("{0}: {1} ({2})", Name, Value.ToString(), Value.GetType().Name);
    }

    /// <inheritdoc/>
    public event EventHandler ValueChanged;
    /// <summary>
    /// Fires a new <c>ValueChanged</c> even.
    /// </summary>
    private void OnValueChanged() {
      if (ValueChanged != null)
        ValueChanged(this, new EventArgs());
      OnChanged();
    }

    private void Value_Changed(object sender, ChangedEventArgs e) {
      OnChanged(e);
    }
  }
}
