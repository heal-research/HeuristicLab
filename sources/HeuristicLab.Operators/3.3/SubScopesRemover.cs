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
  /// An operator which removes one specified or (if not specified) all sub-scopes from the current scope.
  /// </summary>
  [Item("SubScopesRemover", "An operator which removes one specified or (if not specified) all sub-scopes from the current scope.")]
  [EmptyStorableClass]
  [Creatable("Test")]
  public class SubScopesRemover : SingleSuccessorOperator {
    public ValueLookupParameter<IntData> SubScopeIndexParameter {
      get { return (ValueLookupParameter<IntData>)Parameters["SubScopeIndex"]; }
    }
    protected ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    public IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }

    public SubScopesRemover()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntData>("SubScopeIndex", "The index of the sub-scope which should be removed. If this parameter has no value, all sub-scopes of the current scope are removed."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope from which one or all sub-scopes should be removed."));
    }

    public override IExecutionSequence Apply() {
      IntData index = SubScopeIndexParameter.ActualValue;
      if (index != null)
        CurrentScope.SubScopes.RemoveAt(index.Value);
      else
        CurrentScope.SubScopes.Clear();
      return base.Apply();
    }
  }
}
