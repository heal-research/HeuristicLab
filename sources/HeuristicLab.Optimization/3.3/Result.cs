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
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  /// <summary>
  /// Represents a result which has a name and a data type and holds an IItem.
  /// </summary>
  [Item("Result", "A result which has a name and a data type and holds an IItem.")]
  [StorableClass]
  public sealed class Result : NamedItem, IResult {
    public override bool CanChangeName {
      get { return false; }
    }
    public override bool CanChangeDescription {
      get { return false; }
    }

    [Storable]
    private Type dataType;
    public Type DataType {
      get { return dataType; }
    }

    private IItem value;
    [Storable]
    public IItem Value {
      get { return value; }
      set {
        if (this.value != value) {
          if ((value != null) && (!dataType.IsInstanceOfType(value)))
            throw new ArgumentException(
              string.Format("Type mismatch. Value is not a \"{0}\".",
                            dataType.GetPrettyName())
            );

          if (this.value != null) this.value.ToStringChanged -= new EventHandler(Value_ToStringChanged);
          this.value = value;
          if (this.value != null) this.value.ToStringChanged += new EventHandler(Value_ToStringChanged);
          OnValueChanged();
        }
      }
    }

    public Result()
      : base("Anonymous") {
      this.dataType = typeof(IItem);
      this.value = null;
    }
    public Result(string name, Type dataType)
      : base(name) {
      this.dataType = dataType;
      this.value = null;
    }
    public Result(string name, string description, Type dataType)
      : base(name, description) {
      this.dataType = dataType;
      this.value = null;
    }
    public Result(string name, IItem value)
      : base(name) {
      this.dataType = value == null ? typeof(IItem) : value.GetType();
      this.value = value;
      if (this.value != null) this.value.ToStringChanged += new EventHandler(Value_ToStringChanged);
    }
    public Result(string name, string description, IItem value)
      : base(name, description) {
      this.dataType = value == null ? typeof(IItem) : value.GetType();
      this.value = value;
      if (this.value != null) this.value.ToStringChanged += new EventHandler(Value_ToStringChanged);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      Result clone = new Result(Name, Description, (IItem)cloner.Clone(value));
      cloner.RegisterClonedObject(this, clone);
      clone.dataType = dataType;
      return clone;
    }

    public override string ToString() {
      return string.Format("{0}: {1} ({2})", Name, Value == null ? "null" : Value.ToString(), DataType.GetPrettyName());
    }

    public event EventHandler ValueChanged;
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
