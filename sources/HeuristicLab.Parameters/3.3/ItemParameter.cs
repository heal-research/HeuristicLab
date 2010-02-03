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
  /// A generic parameter which represents an instance of type T.
  /// </summary>
  [Item("ItemParameter<T>", "A generic parameter which represents an instance of type T.")]
  public class ItemParameter<T> : Parameter where T : class, IItem {
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

    private T localValue;
    [Storable]
    public T LocalValue {
      get { return this.localValue; }
      set {
        if (value != this.localValue) {
          if ((value != null) && (!DataType.IsInstanceOfType(value))) throw new ArgumentException("Static value does not match data type of parameter");
          if (this.localValue != null) this.localValue.Changed -= new ChangedEventHandler(LocalValue_Changed);
          this.localValue = value;
          if (this.localValue != null) this.localValue.Changed += new ChangedEventHandler(LocalValue_Changed);
          OnLocalValueChanged();
        }
      }
    }

    public T Value {
      get { return GetValue(); }
      set { SetValue(value); }
    }

    public ItemParameter()
      : base("Anonymous", null, typeof(T)) {
      actualName = Name;
      LocalValue = null;
    }
    public ItemParameter(string name, string description)
      : base(name, description, typeof(T)) {
      actualName = Name;
      LocalValue = null;
    }
    public ItemParameter(string name, string description, T localValue)
      : base(name, description, typeof(T)) {
      actualName = Name;
      LocalValue = localValue;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      ItemParameter<T> clone = (ItemParameter<T>)base.Clone(cloner);
      clone.actualName = actualName;
      clone.LocalValue = (T)cloner.Clone(localValue);
      return clone;
    }

    public override string ToString() {
      return string.Format("{0}: {1} ({2})", Name, LocalValue != null ? LocalValue.ToString() : ActualName, DataType.Name);
    }

    protected ItemParameter<T> GetParameter(out string name) {
      ItemParameter<T> param = this;
      ExecutionContext current = ExecutionContext;
      name = param.Name;
      while (param != null) {
        if (param.LocalValue != null) return param;
        name = param.ActualName;
        current = current.Parent;
        while ((current != null) && !current.Operator.Parameters.ContainsKey(name))
          current = current.Parent;
        if (current != null)
          param = (ItemParameter<T>)current.Operator.Parameters[actualName];
        else
          param = null;
      }
      return null;
    }
    protected IVariable GetVariable(string name) {
      IScope scope = ExecutionContext.Scope;
      while ((scope != null) && !scope.Variables.ContainsKey(name))
        scope = scope.Parent;
      return scope != null ? scope.Variables[actualName] : null;
    }
    protected virtual T GetValue() {
      string name;
      // try to get local value from context stack
      ItemParameter<T> param = GetParameter(out name);
      if (param != null) return param.Value;
      else {
        // try to get variable from scope
        IVariable var = GetVariable(name);
        if (var != null) return (T)var.Value;
      }
      return null;
    }
    protected virtual void SetValue(T value) {
      string name;
      // try to get local value from context stack
      ItemParameter<T> param = GetParameter(out name);
      if (param != null) param.Value = value;
      else {
        // try to get variable from scope
        IVariable var = GetVariable(name);
        if (var != null) var.Value = value;
        else ExecutionContext.Scope.Variables.Add(new Variable(name, value));
      }
    }

    public event EventHandler ActualNameChanged;
    private void OnActualNameChanged() {
      if (ActualNameChanged != null)
        ActualNameChanged(this, new EventArgs());
      OnChanged();
    }
    public event EventHandler LocalValueChanged;
    private void OnLocalValueChanged() {
      if (LocalValueChanged != null)
        LocalValueChanged(this, new EventArgs());
      OnChanged();
    }
    private void LocalValue_Changed(object sender, ChangedEventArgs e) {
      OnChanged(e);
    }
  }
}
