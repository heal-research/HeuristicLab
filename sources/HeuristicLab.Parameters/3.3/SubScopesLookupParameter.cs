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
  /// A generic parameter representing instances of type T which are collected from the sub-scopes of the current scope.
  /// </summary>
  [Item("SubScopesLookupParameter<T>", "A generic parameter representing instances of type T which are collected from or written to the sub-scopes of the current scope.")]
  public class SubScopesLookupParameter<T> : Parameter, ILookupParameter<T> where T : class, IItem {
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

    public new ItemArray<T> ActualValue {
      get { return (ItemArray<T>)GetActualValue(); }
      set { SetActualValue(value); }
    }

    public SubScopesLookupParameter()
      : base("Anonymous", typeof(ItemArray<T>)) {
      actualName = Name;
    }
    public SubScopesLookupParameter(string name)
      : base(name, typeof(ItemArray<T>)) {
      actualName = Name;
    }
    public SubScopesLookupParameter(string name, string description)
      : base(name, description, typeof(ItemArray<T>)) {
      actualName = Name;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      SubScopesLookupParameter<T> clone = (SubScopesLookupParameter<T>)base.Clone(cloner);
      clone.actualName = actualName;
      return clone;
    }

    public override string ToString() {
      return string.Format("{0}: {1} ({2})", Name, ActualName, DataType.Name);
    }

    protected override IItem GetActualValue() {
      string name = LookupParameter<T>.TranslateName(Name, ExecutionContext);
      IScope scope = ExecutionContext.Scope;
      ItemArray<T> values = new ItemArray<T>(scope.SubScopes.Count);
      IVariable var;
      T value;

      for (int i = 0; i < values.Length; i++) {
        scope.SubScopes[i].Variables.TryGetValue(name, out var);
        if (var != null) {
          value = var.Value as T;
          if (value == null)
            throw new InvalidOperationException(
              string.Format("Type mismatch. Variable \"{0}\" does not contain a \"{1}\".",
                            name,
                            typeof(T).GetPrettyName())
            );
          values[i] = value;
        }
      }
      return values;
    }
    protected override void SetActualValue(IItem value) {
      ItemArray<T> values = value as ItemArray<T>;
      if (values == null)
        throw new InvalidOperationException(
          string.Format("Type mismatch. Value is not a \"{0}\".",
                        typeof(ItemArray<T>).GetPrettyName())
        );

      string name = LookupParameter<T>.TranslateName(Name, ExecutionContext);
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
