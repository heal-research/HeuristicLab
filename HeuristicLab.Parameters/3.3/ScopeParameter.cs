#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Parameters {
  /// <summary>
  /// A parameter which represents the current scope.
  /// </summary>
  [Item("ScopeParameter", "A parameter which represents the current scope.")]
  [StorableClass]
  public class ScopeParameter : LookupParameter<IScope> {
    public new IScope ActualValue {
      get { return ExecutionContext.Scope; }
    }


    [StorableConstructor]
    protected ScopeParameter(bool deserializing) : base(deserializing) { }
    protected ScopeParameter(ScopeParameter original, Cloner cloner) : base(original, cloner) { }
    public ScopeParameter()
      : base("Anonymous") {
      this.Hidden = true;
    }
    public ScopeParameter(string name)
      : base(name) {
      this.Hidden = true;
    }
    public ScopeParameter(string name, string description)
      : base(name, description) {
      this.Hidden = true;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ScopeParameter(this, cloner);
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
