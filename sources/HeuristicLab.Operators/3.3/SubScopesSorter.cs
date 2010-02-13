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
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// An Operator which sorts the sub-scopes of the current scope.
  /// </summary>
  [Item("SubScopesSorter", "An operator which sorts the sub-scopes of the current scope.")]
  [EmptyStorableClass]
  [Creatable("Test")]
  public sealed class SubScopesSorter : SingleSuccessorOperator {
    private bool descending;
    private string actualName;

    public SubScopesLookupParameter<DoubleData> ValueParameter {
      get { return (SubScopesLookupParameter<DoubleData>)Parameters["Value"]; }
    }
    public ValueLookupParameter<BoolData> DescendingParameter {
      get { return (ValueLookupParameter<BoolData>)Parameters["Descending"]; }
    }
    private ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    public IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }

    public SubScopesSorter()
      : base() {
      Parameters.Add(new SubScopesLookupParameter<DoubleData>("Value", "The values contained in each sub-scope acording which the sub-scopes of the current scope are sorted."));
      Parameters.Add(new ValueLookupParameter<BoolData>("Descending", "True if the sub-scopes should be sorted in descending order, otherwise false."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope whose sub-scopes are sorted."));
    }

    public override IExecutionSequence Apply() {
      descending = DescendingParameter.ActualValue.Value;
      actualName = LookupParameter<DoubleData>.TranslateName(ValueParameter.Name, ExecutionContext);
      CurrentScope.SubScopes.Sort(SortScopes);
      return base.Apply();
    }

    private int SortScopes(IScope x, IScope y) {
      IVariable var1;
      IVariable var2;
      x.Variables.TryGetValue(actualName, out var1);
      x.Variables.TryGetValue(actualName, out var2);
      if ((var1 == null) && (var2 == null))
        return 0;
      else if ((var1 == null) && (var2 != null))
        return 1;
      else if ((var1 != null) && (var2 == null))
        return -1;
      else {
        int result = ((DoubleData)var1.Value).CompareTo((DoubleData)var2.Value);
        if (descending) result = result * -1;
        return result;
      }
    }
  }
}
