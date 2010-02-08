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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// An operator which adds new and empty sub-scopes to the current scope.
  /// </summary>
  [Item("SubScopesCreater", "An operator which adds new and empty sub-scopes to the current scope.")]
  [EmptyStorableClass]
  [Creatable("Test")]
  public class SubScopesCreater : SingleSuccessorOperator {
    public ValueLookupParameter<IntData> NumberOfSubScopesParameter {
      get { return (ValueLookupParameter<IntData>)Parameters["NumberOfSubScopes"]; }
    }
    protected ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    public IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }

    public SubScopesCreater()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntData>("NumberOfSubScopes", "The number of new and empty sub-scopes which should be added to the current scope."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope to which the new and empty sub-scopes are added."));
    }

    public override IExecutionContext Apply() {
      int n = NumberOfSubScopesParameter.ActualValue.Value;
      for (int i = 0; i < n; i++)
        CurrentScope.SubScopes.Add(new Scope(i.ToString()));
      return base.Apply();
    }
  }
}
