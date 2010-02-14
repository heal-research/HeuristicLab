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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Parameters {
  /// <summary>
  /// A parameter whose value is either defined it the parameter itself or is retrieved from the scope.
  /// </summary>
  [Item("ValueLookupParameter<T>", "A parameter whose value is either defined it the parameter itself or is retrieved from or written to a scope.")]
  public class ValueLookupParameter<T> : LookupParameter<T>, IValueLookupParameter<T> where T : class, IItem {
    private T value;
    [Storable]
    public T Value {
      get { return this.value; }
      set {
        if (value != this.value) {
          if (this.value != null) this.value.Changed -= new ChangedEventHandler(Value_Changed);
          this.value = value;
          if (this.value != null) this.value.Changed += new ChangedEventHandler(Value_Changed);
          OnValueChanged();
        }
      }
    }

    public ValueLookupParameter()
      : base() {
    }
    public ValueLookupParameter(string name)
      : base(name) {
    }
    public ValueLookupParameter(string name, T value)
      : base(name) {
      Value = value;
    }
    public ValueLookupParameter(string name, string description)
      : base(name, description) {
    }
    public ValueLookupParameter(string name, string description, T value)
      : base(name, description) {
      Value = value;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      ValueLookupParameter<T> clone = (ValueLookupParameter<T>)base.Clone(cloner);
      clone.Value = (T)cloner.Clone(value);
      return clone;
    }

    public override string ToString() {
      return string.Format("{0}: {1} ({2})", Name, Value != null ? Value.ToString() : ActualName, DataType.Name);
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
