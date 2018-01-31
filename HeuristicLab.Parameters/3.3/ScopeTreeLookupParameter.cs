#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Parameters {
  /// <summary>
  /// A generic parameter representing instances of type T which are collected from or written to scope tree.
  /// </summary>
  [Item("ScopeTreeLookupParameter", "A generic parameter representing instances of type T which are collected from or written to scope tree.")]
  [StorableClass]
  public class ScopeTreeLookupParameter<T> : LookupParameter<ItemArray<T>>, IScopeTreeLookupParameter<T> where T : class, IItem {
    [Storable]
    private int depth;
    public int Depth {
      get { return depth; }
      set {
        if (value < 0) throw new ArgumentException("Depth must be larger than or equal to 0.");
        if (depth != value) {
          depth = value;
          OnDepthChanged();
        }
      }
    }

    [StorableConstructor]
    protected ScopeTreeLookupParameter(bool deserializing) : base(deserializing) { }
    protected ScopeTreeLookupParameter(ScopeTreeLookupParameter<T> original, Cloner cloner)
      : base(original, cloner) {
      depth = original.depth;
    }
    public ScopeTreeLookupParameter()
      : base() {
      depth = 1;
    }
    public ScopeTreeLookupParameter(string name)
      : base(name) {
      depth = 1;
    }
    public ScopeTreeLookupParameter(string name, int depth)
      : base(name) {
      this.depth = depth;
    }
    public ScopeTreeLookupParameter(string name, string description)
      : base(name, description) {
      depth = 1;
    }
    public ScopeTreeLookupParameter(string name, string description, int depth)
      : base(name, description) {
      this.depth = depth;
    }
    public ScopeTreeLookupParameter(string name, string description, string actualName)
      : base(name, description, actualName) {
      depth = 1;
    }
    public ScopeTreeLookupParameter(string name, string description, string actualName, int depth)
      : base(name, description, actualName) {
      this.depth = depth;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ScopeTreeLookupParameter<T>(this, cloner);
    }

    protected override IItem GetActualValue() {
      IEnumerable<IScope> scopes = new IScope[] { ExecutionContext.Scope };
      for (int i = 0; i < depth; i++)
        scopes = scopes.Select(x => (IEnumerable<IScope>)x.SubScopes).Aggregate((a, b) => a.Concat(b));

      string name = TranslatedName;
      List<T> values = new List<T>();
      IVariable var;
      T value;
      foreach (IScope scope in scopes) {
        scope.Variables.TryGetValue(name, out var);
        if (var != null) {
          value = var.Value as T;
          if ((var.Value != null) && (value == null))
            throw new InvalidOperationException(
              string.Format("Type mismatch. Variable \"{0}\" does not contain a \"{1}\".",
                            name,
                            typeof(T).GetPrettyName())
            );
          values.Add(value);
        }
      }
      return new ItemArray<T>(values);
    }
    protected override void SetActualValue(IItem value) {
      ItemArray<T> values = value as ItemArray<T>;
      if (values == null)
        throw new InvalidOperationException(
          string.Format("Type mismatch. Value is not a \"{0}\".",
                        typeof(ItemArray<T>).GetPrettyName())
        );

      IEnumerable<IScope> scopes = new IScope[] { ExecutionContext.Scope };
      for (int i = 0; i < depth; i++)
        scopes = scopes.Select(x => (IEnumerable<IScope>)x.SubScopes).Aggregate((a, b) => a.Concat(b));

      if (scopes.Count() != values.Length) throw new InvalidOperationException("Number of values is not equal to number of scopes.");

      string name = TranslatedName;
      int j = 0;
      IVariable var;
      foreach (IScope scope in scopes) {
        scope.Variables.TryGetValue(name, out var);
        if (var != null) var.Value = values[j];
        else scope.Variables.Add(new Variable(name, values[j]));
        j++;
      }
    }

    public event EventHandler DepthChanged;
    protected virtual void OnDepthChanged() {
      EventHandler handler = DepthChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
  }
}
