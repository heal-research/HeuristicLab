#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// An operator which removes all variables and sub-scopes from the current scope.
  /// </summary>
  [Item("ScopeCleaner", "An operator which removes all variables and sub-scopes from the current scope.")]
  [StorableClass]
  public sealed class ScopeCleaner : SingleSuccessorOperator {
    private ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    public IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }

    [StorableConstructor]
    private ScopeCleaner(bool deserializing) : base(deserializing) { }
    private ScopeCleaner(ScopeCleaner original, Cloner cloner)
      : base(original, cloner) {
    }
    public ScopeCleaner()
      : base() {
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope whose variables and sub-scopes should be removed."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ScopeCleaner(this, cloner);
    }

    public override IOperation Apply() {
      CurrentScope.Variables.Clear();
      CurrentScope.SubScopes.Clear();
      return base.Apply();
    }
  }
}
