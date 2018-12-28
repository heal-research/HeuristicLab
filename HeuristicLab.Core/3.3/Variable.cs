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
using System.Drawing;
using HeuristicLab.Common;
using HEAL.Fossil;

namespace HeuristicLab.Core {
  /// <summary>
  /// Represents a variable which has a name and holds an IItem.
  /// </summary>
  [Item("Variable", "A variable which has a name and holds an IItem.")]
  [StorableType("FF4ED8C3-C0A0-42F9-8EFF-3DC6AAD90EE7")]
  public sealed class Variable : NamedItem, IVariable {
    public override Image ItemImage {
      get {
        if (value != null) return value.ItemImage;
        else return base.ItemImage;
      }
    }

    [Storable]
    private IItem value;
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="OnValueChanged"/> in the setter.</remarks>
    public IItem Value {
      get { return value; }
      set {
        if (this.value != value) {
          DeregisterValueEvents();
          this.value = value;
          RegisterValueEvents();
          OnValueChanged();
        }
      }
    }

    [StorableConstructor]
    private Variable(StorableConstructorFlag _) : base(_) { }
    private Variable(Variable original, Cloner cloner)
      : base(original, cloner) {
      value = cloner.Clone(original.value);
      RegisterValueEvents();
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
      RegisterValueEvents();
    }
    public Variable(string name, string description, IItem value)
      : base(name, description) {
      this.value = value;
      RegisterValueEvents();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterValueEvents();
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <param name="clonedObjects">Dictionary of all already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="Variable"/>.</returns>
    public override IDeepCloneable Clone(Cloner cloner) {
      return new Variable(this, cloner);
    }

    /// <summary>
    /// Gets the string representation of the current instance in the format: <c>Name: [null|Value]</c>.
    /// </summary>
    /// <returns>The current instance as a string.</returns>
    public override string ToString() {
      if (Value == null)
        return string.Format("{0}: null", Name);
      else
        return string.Format("{0}: {1}", Name, Value.ToString());
    }

    /// <inheritdoc/>
    public event EventHandler ValueChanged;
    /// <summary>
    /// Fires a new <c>ValueChanged</c> even.
    /// </summary>
    private void OnValueChanged() {
      var handler = ValueChanged;
      if (handler != null) handler(this, EventArgs.Empty);
      OnItemImageChanged();
      OnToStringChanged();
    }

    private void RegisterValueEvents() {
      if (value != null) {
        value.ItemImageChanged += new EventHandler(Value_ItemImageChanged);
        value.ToStringChanged += new EventHandler(Value_ToStringChanged);
      }
    }
    private void DeregisterValueEvents() {
      if (value != null) {
        value.ItemImageChanged -= new EventHandler(Value_ItemImageChanged);
        value.ToStringChanged -= new EventHandler(Value_ToStringChanged);
      }
    }
    private void Value_ItemImageChanged(object sender, EventArgs e) {
      OnItemImageChanged();
    }
    private void Value_ToStringChanged(object sender, EventArgs e) {
      OnToStringChanged();
    }
  }
}
