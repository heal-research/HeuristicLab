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
  /// A generic parameter representing instances of type T which are collected from the sub-scopes of the current scope.
  /// </summary>
  [Item("SubScopesItemParameter<T>", "A generic parameter representing instances of type T which are collected from the sub-scopes of the current scope.")]
  public class SubScopesItemParameter<T> : Parameter where T : class, IItem {
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

    public T[] Values {
      get { return GetValues(); }
      set { SetValues(value); }
    }

    public SubScopesItemParameter()
      : base("Anonymous", null, typeof(T)) {
      actualName = Name;
    }
    public SubScopesItemParameter(string name, string description)
      : base(name, description, typeof(T)) {
      actualName = Name;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      SubScopesItemParameter<T> clone = (SubScopesItemParameter<T>)base.Clone(cloner);
      clone.actualName = actualName;
      return clone;
    }

    public override string ToString() {
      return string.Format("{0}: {1} ({2})", Name, ActualName, DataType.Name);
    }

    protected string GetActualName() {
      string name = Name;
      ExecutionContext current = ExecutionContext;
      while (current != null) {
        if (current.Operator.Parameters.ContainsKey(name))
          name = ((SubScopesItemParameter<T>)current.Operator.Parameters[name]).ActualName;
        current = current.Parent;
      }
      return name;
    }
    protected virtual T[] GetValues() {
      string name = GetActualName();
      IScope scope = ExecutionContext.Scope;
      T[] value = new T[scope.SubScopes.Count];
      IVariable var;

      for (int i = 0; i < value.Length; i++) {
        scope.SubScopes[i].Variables.TryGetValue(name, out var);
        if (var != null) value[i] = (T)var.Value;
      }
      return value;
    }
    protected virtual void SetValues(T[] values) {
      string name = GetActualName();
      IScope scope = ExecutionContext.Scope;
      IVariable var;

      for (int i = 0; i < values.Length; i++) {
        scope.SubScopes[i].Variables.TryGetValue(name, out var);
        if (var != null) var.Value = values[i];
        else scope.SubScopes[i].Variables.Add(new Variable(name, values[i]));
      }
    }

    public event EventHandler ActualNameChanged;
    private void OnActualNameChanged() {
      if (ActualNameChanged != null)
        ActualNameChanged(this, new EventArgs());
      OnChanged();
    }
  }
}
