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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Selection {
  /// <summary>
  /// A base class for selection operators.
  /// </summary>
  [Item("Selector", "A base class for selection operators.")]
  [StorableClass]
  public abstract class Selector : InstrumentedOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    protected ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }

    public IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }

    [StorableConstructor]
    protected Selector(bool deserializing) : base(deserializing) { }
    protected Selector(Selector original, Cloner cloner) : base(original, cloner) { }

    protected Selector()
      : base() {
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope from which sub-scopes should be selected."));
    }

    public sealed override IOperation InstrumentedApply() {
      List<IScope> scopes = new List<IScope>(CurrentScope.SubScopes);
      IScope[] selected = Select(scopes);

      CurrentScope.SubScopes.Clear();
      IScope remainingScope = new Scope("Remaining");
      remainingScope.SubScopes.AddRange(scopes);
      CurrentScope.SubScopes.Add(remainingScope);
      IScope selectedScope = new Scope("Selected");
      selectedScope.SubScopes.AddRange(selected);
      CurrentScope.SubScopes.Add(selectedScope);

      return base.InstrumentedApply();
    }

    protected abstract IScope[] Select(List<IScope> scopes);
  }
}
