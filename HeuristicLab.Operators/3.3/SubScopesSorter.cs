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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// An Operator which sorts the sub-scopes of the current scope.
  /// </summary>
  [Item("SubScopesSorter", "An operator which sorts the sub-scopes of the current scope.")]
  [StorableClass]
  public sealed class SubScopesSorter : SingleSuccessorOperator {
    private bool descending;
    private string actualName;

    public ScopeTreeLookupParameter<DoubleValue> ValueParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Value"]; }
    }
    public ValueLookupParameter<BoolValue> DescendingParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["Descending"]; }
    }
    private ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    public IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }

    [StorableConstructor]
    private SubScopesSorter(bool deserializing) : base(deserializing) { }
    private SubScopesSorter(SubScopesSorter original, Cloner cloner)
      : base(original, cloner) {
    }
    public SubScopesSorter()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Value", "The values contained in each sub-scope acording which the sub-scopes of the current scope are sorted."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Descending", "True if the sub-scopes should be sorted in descending order, otherwise false."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope whose sub-scopes are sorted."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SubScopesSorter(this, cloner);
    }

    public override IOperation Apply() {
      descending = DescendingParameter.ActualValue.Value;
      actualName = ValueParameter.TranslatedName;
      CurrentScope.SubScopes.Sort(SortScopes);
      return base.Apply();
    }

    private int SortScopes(IScope x, IScope y) {
      IVariable var1;
      IVariable var2;
      x.Variables.TryGetValue(actualName, out var1);
      y.Variables.TryGetValue(actualName, out var2);
      if ((var1 == null) && (var2 == null))
        return 0;
      else if ((var1 == null) && (var2 != null))
        return 1;
      else if ((var1 != null) && (var2 == null))
        return -1;
      else {
        int result = ((DoubleValue)var1.Value).CompareTo((DoubleValue)var2.Value);
        if (descending) result = result * -1;
        return result;
      }
    }
  }
}
