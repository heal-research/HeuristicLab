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
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Parameters {
  /// <summary>
  /// A parameter which represents the current scope.
  /// </summary>
  [Item("ScopeParameter", "A parameter which represents the current scope.")]
  [StorableClass]
  public class ScopeParameter : Parameter {
    public new IScope ActualValue {
      get { return ExecutionContext.Scope; }
    }

    public ScopeParameter()
      : base("Anonymous", typeof(IScope)) {
    }
    public ScopeParameter(string name)
      : base(name, typeof(IScope)) {
    }
    public ScopeParameter(string name, string description)
      : base(name, description, typeof(IScope)) {
    }

    public override string ToString() {
      return Name;
    }

    protected override IItem GetActualValue() {
      return ExecutionContext.Scope;
    }
    protected override void SetActualValue(IItem value) {
      throw new NotSupportedException("The actual value of a ScopeParameter cannot be set. It is always the current scope.");
    }
  }
}
