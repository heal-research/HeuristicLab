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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Parameters {
  /// <summary>
  /// A parameter whose value is either defined it the parameter itself or is retrieved from the scope.
  /// </summary>
  [Item("ValueLookupParameter<T>", "A parameter whose value is either defined it the parameter itself or is retrieved from or written to a scope.")]
  public class ValueLookupParameter<T> : Parameter, IValueLookupParameter<T> where T : class, IItem {
    [Storable]
    private string actualName;
    public string ActualName {
      get { return actualName; }
      set {
        if (value == null) throw new ArgumentNullException();
        if (!actualName.Equals(value)) {
          actualName = value;
          OnActualNameChanged();
        }
      }
    }

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

    public new T ActualValue {
      get { return (T)GetActualValue(); }
      set { SetActualValue(value); }
    }

    public ValueLookupParameter()
      : base("Anonymous", typeof(T)) {
      actualName = Name;
    }
    public ValueLookupParameter(string name)
      : base(name, typeof(T)) {
      actualName = Name;
    }
    public ValueLookupParameter(string name, T value)
      : base(name, typeof(T)) {
      actualName = Name;
      Value = value;
    }
    public ValueLookupParameter(string name, string description)
      : base(name, description, typeof(T)) {
      actualName = Name;
    }
    public ValueLookupParameter(string name, string description, T value)
      : base(name, description, typeof(T)) {
      actualName = Name;
      Value = value;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      ValueLookupParameter<T> clone = (ValueLookupParameter<T>)base.Clone(cloner);
      clone.actualName = actualName;
      clone.Value = (T)cloner.Clone(value);
      return clone;
    }

    public override string ToString() {
      return string.Format("{0}: {1} ({2})", Name, Value != null ? Value.ToString() : ActualName, DataType.Name);
    }

    private IValueParameter<T> GetParameter(out string name) {
      IValueParameter<T> valueParam = this;
      ILookupParameter<T> lookupParam = this;
      ExecutionContext current = ExecutionContext;

      name = Name;
      while ((valueParam != null) && (lookupParam != null)) {
        if ((valueParam != null) && (valueParam.Value != null)) return valueParam;
        if (lookupParam != null) name = lookupParam.ActualName;

        current = current.Parent;
        while ((current != null) && !current.Operator.Parameters.ContainsKey(name))
          current = current.Parent;

        if (current != null) {
          valueParam = current.Operator.Parameters[name] as IValueParameter<T>;
          lookupParam = current.Operator.Parameters[name] as ILookupParameter<T>;
          if ((valueParam == null) && (lookupParam == null))
            throw new InvalidOperationException(
              string.Format("Parameter look-up chain broken. Parameter \"{0}\" is not an \"{1}\" or an \"{2}\".",
                            name,
                            typeof(IValueParameter<T>).GetPrettyName(),
                            typeof(ILookupParameter<T>).GetPrettyName())
            );
        } else {
          valueParam = null;
          lookupParam = null;
        }
      }
      return null;
    }
    private IVariable LookupVariable(string name) {
      IScope scope = ExecutionContext.Scope;
      while ((scope != null) && !scope.Variables.ContainsKey(name))
        scope = scope.Parent;
      return scope != null ? scope.Variables[actualName] : null;
    }
    protected override IItem GetActualValue() {
      string name;
      // try to get local value from context stack
      IValueParameter<T> param = GetParameter(out name);
      if (param != null) return param.Value;
      else {  // try to get variable from scope
        IVariable var = LookupVariable(name);
        if (var != null) {
          T value = var.Value as T;
          if (value == null)
            throw new InvalidOperationException(
              string.Format("Type mismatch. Variable \"{0}\" does not contain a \"{1}\".",
                            name,
                            typeof(T).GetPrettyName())
            );
          return value;
        }
      }
      return null;
    }
    protected override void SetActualValue(IItem value) {
      T val = value as T;
      if (val == null)
        throw new InvalidOperationException(
          string.Format("Type mismatch. Value is not a \"{0}\".",
                        typeof(T).GetPrettyName())
        );
      // try to get local value from context stack
      string name;
      IValueParameter<T> param = GetParameter(out name);
      if (param != null) param.Value = val;
      else {  // try to get variable from scope
        IVariable var = LookupVariable(name);
        if (var != null) var.Value = val;
        else ExecutionContext.Scope.Variables.Add(new Variable(name, value));
      }
    }

    public event EventHandler ActualNameChanged;
    private void OnActualNameChanged() {
      if (ActualNameChanged != null)
        ActualNameChanged(this, new EventArgs());
      OnChanged();
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
