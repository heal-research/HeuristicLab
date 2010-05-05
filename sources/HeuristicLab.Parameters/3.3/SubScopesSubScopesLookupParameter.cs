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
  /// A generic parameter representing instances of type T which are collected from or written to the sub-scopes of the sub-scopes of the current scope.
  /// </summary>
  [Item("SubScopesSubScopesLookupParameter<T>", "A generic parameter representing instances of type T which are collected from or written to the sub-scopes of the sub-scopes of the current scope.")]
  [StorableClass]
  public class SubScopesSubScopesLookupParameter<T> : LookupParameter<ItemArray<ItemArray<T>>> where T : class, IItem {
    public SubScopesSubScopesLookupParameter() : base() { }
    public SubScopesSubScopesLookupParameter(string name) : base(name) { }
    public SubScopesSubScopesLookupParameter(string name, string description) : base(name, description) { }
    public SubScopesSubScopesLookupParameter(string name, string description, string actualName) : base(name, description, actualName) { }

    protected override IItem GetActualValue() {
      string name = LookupParameter<ItemArray<ItemArray<T>>>.TranslateName(Name, ExecutionContext);
      IScope scope = ExecutionContext.Scope;
      ItemArray<ItemArray<T>> values = new ItemArray<ItemArray<T>>(scope.SubScopes.Count);
      IVariable var;
      T value;

      for (int i = 0; i < values.Length; i++) {
        values[i] = new ItemArray<T>(scope.SubScopes[i].SubScopes.Count);
        for (int j = 0; j < values[i].Length; j++) {
          scope.SubScopes[i].SubScopes[j].Variables.TryGetValue(name, out var);
          if (var != null) {
            value = var.Value as T;
            if ((var.Value != null) && (value == null))
              throw new InvalidOperationException(
                string.Format("Type mismatch. Variable \"{0}\" does not contain a \"{1}\".",
                              name,
                              typeof(T).GetPrettyName())
              );
            values[i][j] = value;
          }
        }
      }
      return values;
    }
    protected override void SetActualValue(IItem value) {
      ItemArray<ItemArray<T>> values = value as ItemArray<ItemArray<T>>;
      if (values == null)
        throw new InvalidOperationException(
          string.Format("Type mismatch. Value is not a \"{0}\".",
                        typeof(ItemArray<T>).GetPrettyName())
        );

      string name = LookupParameter<ItemArray<ItemArray<T>>>.TranslateName(Name, ExecutionContext);
      IScope scope = ExecutionContext.Scope;
      IVariable var;

      for (int i = 0; i < values.Length; i++) {
        for (int j = 0; j < values[i].Length; j++) {
          scope.SubScopes[i].SubScopes[j].Variables.TryGetValue(name, out var);
          if (var != null) var.Value = values[i][j];
          else scope.SubScopes[i].SubScopes[j].Variables.Add(new Variable(name, values[i][j]));
        }
      }
    }
  }
}
