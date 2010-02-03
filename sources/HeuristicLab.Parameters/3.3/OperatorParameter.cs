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
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Parameters {
  /// <summary>
  /// A parameter which represents an operator.
  /// </summary>
  [Item("OperatorParameter", "A parameter which represents an operator.")]
  [Creatable("Test")]
  public class OperatorParameter : Parameter, IOperatorParameter {
    private IOperator value;
    [Storable]
    public IOperator Value {
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

    public OperatorParameter()
      : base("Anonymous", null, typeof(IOperator)) {
    }
    public OperatorParameter(string name, string description)
      : base(name, description, typeof(IOperator)) {
    }
    public OperatorParameter(string name, string description, IOperator value)
      : base(name, description, typeof(IOperator)) {
      Value = value;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      OperatorParameter clone = (OperatorParameter)base.Clone(cloner);
      clone.Value = (IOperator)cloner.Clone(value);
      return clone;
    }

    public override string ToString() {
      return string.Format("{0}: {1} ({2})", Name, Value != null ? Value.ToString() : "null", DataType.Name);
    }

    public event EventHandler ValueChanged;
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
