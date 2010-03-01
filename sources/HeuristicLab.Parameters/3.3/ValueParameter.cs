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

namespace HeuristicLab.Parameters {
  /// <summary>
  /// A parameter whose value is defined it the parameter itself.
  /// </summary>
  [Item("ValueParameter<T>", "A parameter whose value is defined it the parameter itself.")]
  public sealed class ValueParameter<T> : Parameter, IValueParameter<T> where T : class, IItem {
    private T value;
    [Storable]
    public T Value {
      get { return this.value; }
      set {
        if (value == null) throw new ArgumentNullException();
        if (value != this.value) {
          if (this.value != null) this.value.Changed -= new ChangedEventHandler(Value_Changed);
          this.value = value;
          this.value.Changed += new ChangedEventHandler(Value_Changed);
          OnValueChanged();
        }
      }
    }
    IItem IValueParameter.Value {
      get { return Value; }
      set {
        if (value == null) throw new ArgumentNullException();
        T val = value as T;
        if (val == null)
          throw new InvalidOperationException(
            string.Format("Type mismatch. Value is not a \"{0}\".",
                          typeof(T).GetPrettyName())
          );
        Value = val;
      }
    }

    private ValueParameter()
      : base("Anonymous", typeof(T)) {
    }
    public ValueParameter(string name, T value)
      : base(name, typeof(T)) {
      Value = value;
    }
    public ValueParameter(string name, string description, T value)
      : base(name, description, typeof(T)) {
      Value = value;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      ValueParameter<T> clone = new ValueParameter<T>(name, description, value);
      cloner.RegisterClonedObject(this, clone);
      clone.Value = (T)cloner.Clone(value);
      return clone;
    }

    public override string ToString() {
      return string.Format("{0}: {1} ({2})", Name, Value.ToString(), DataType.GetPrettyName());
    }

    protected override IItem GetActualValue() {
      return Value;
    }
    protected override void SetActualValue(IItem value) {
      ((IValueParameter)this).Value = value;
    }

    public event EventHandler ValueChanged;
    private void OnValueChanged() {
      if (ValueChanged != null)
        ValueChanged(this, EventArgs.Empty);
      OnChanged();
    }
    private void Value_Changed(object sender, ChangedEventArgs e) {
      OnChanged(e);
    }
  }
}
