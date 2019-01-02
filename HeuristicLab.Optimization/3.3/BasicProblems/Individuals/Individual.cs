#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HEAL.Fossil;

namespace HeuristicLab.Optimization {
  [StorableType("b1388a9d-c85c-49f6-9916-ef4c93dffee0")]
  public abstract class Individual {
    [Storable]
    protected IEncoding Encoding { get; private set; }
    [Storable]
    protected IScope Scope { get; private set; }
    public string Name { get { return Encoding.Name; } }


    [StorableConstructor]
    protected Individual(StorableConstructorFlag _) {
    }

    protected Individual(IEncoding encoding, IScope scope) {
      Encoding = encoding;
      Scope = scope;
    }

    public IItem this[string name] {
      get { return ExtractScopeValue(name, Scope); }
      set { SetScopeValue(name, Scope, value); }
    }

    public IEnumerable<KeyValuePair<string, IItem>> Values {
      get { return Scope.Variables.Select(v => new KeyValuePair<string, IItem>(v.Name, v.Value)); }
    }
    public abstract TEncoding GetEncoding<TEncoding>() where TEncoding : class, IEncoding;

    public abstract Individual Copy();
    internal void CopyToScope(IScope scope) {
      foreach (var val in Values)
        SetScopeValue(val.Key, scope, val.Value);
    }

    private static IItem ExtractScopeValue(string name, IScope scope) {
      if (scope == null) throw new ArgumentNullException("scope");
      if (!scope.Variables.ContainsKey(name)) throw new ArgumentException(string.Format(" {0} cannot be found in the provided scope.", name));
      var value = scope.Variables[name].Value;
      if (value == null) throw new InvalidOperationException(string.Format("Value of {0} is null.", name));
      return value;
    }

    private static void SetScopeValue(string name, IScope scope, IItem value) {
      if (scope == null) throw new ArgumentNullException("scope");
      if (value == null) throw new ArgumentNullException("value");

      if (!scope.Variables.ContainsKey(name)) scope.Variables.Add(new Variable(name, value));
      else scope.Variables[name].Value = value;
    }
  }
}
