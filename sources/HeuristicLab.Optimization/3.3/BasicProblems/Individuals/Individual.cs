#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;

namespace HeuristicLab.Optimization {
  public abstract class Individual {
    protected IEncoding Encoding { get; private set; }
    protected IScope Scope { get; private set; }

    public string Name { get { return Encoding.Name; } }

    protected Individual(IEncoding encoding, IScope scope) {
      Encoding = encoding;
      Scope = scope;
    }

    public abstract IItem this[string name] { get; set; }
    public abstract TEncoding GetEncoding<TEncoding>() where TEncoding : class, IEncoding;

    public Individual Copy() {
      return CopyToScope(new Scope());
    }

    public abstract Individual CopyToScope(IScope scope);

    protected static IItem ExtractScopeValue(string name, IScope scope) {
      if (!scope.Variables.ContainsKey(name)) throw new ArgumentException(string.Format(" {0} cannot be found in the provided scope.", name));
      var value = scope.Variables[name].Value;
      if (value == null) throw new InvalidOperationException(string.Format("Value of {0} is null.", name));
      return value;
    }

    protected static void SetScopeValue(string name, IScope scope, IItem value) {
      if (value == null) throw new ArgumentNullException("value");
      if (!scope.Variables.ContainsKey(name)) scope.Variables.Add(new Variable(name, value));
      else scope.Variables[name].Value = value;
    }
  }
}
