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
using HEAL.Attic;

namespace HeuristicLab.Operators {
  /// <summary>
  /// An operator which removes all sub-scopes or one specified sub-scope from the current scope.
  /// </summary>
  [Item("SubScopesRemover", "An operator which removes all sub-scopes or one specified sub-scope from the current scope.")]
  [StorableType("A9624A3A-F8E9-4B8C-AE89-D243987F3505")]
  public sealed class SubScopesRemover : SingleSuccessorOperator {
    private ValueParameter<BoolValue> RemoveAllSubScopesParameter {
      get { return (ValueParameter<BoolValue>)Parameters["RemoveAllSubScopes"]; }
    }
    public ValueLookupParameter<IntValue> SubScopeIndexParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["SubScopeIndex"]; }
    }
    private ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }

    public bool RemoveAllSubScopes {
      get { return RemoveAllSubScopesParameter.Value.Value; }
      set { RemoveAllSubScopesParameter.Value.Value = value; }
    }
    public IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }

    [StorableConstructor]
    private SubScopesRemover(StorableConstructorFlag _) : base(_) { }
    private SubScopesRemover(SubScopesRemover original, Cloner cloner)
      : base(original, cloner) {
    }
    public SubScopesRemover()
      : base() {
      Parameters.Add(new ValueParameter<BoolValue>("RemoveAllSubScopes", "True if all sub-scopes of the current scope should be removed, otherwise false.", new BoolValue(true)));
      Parameters.Add(new ValueLookupParameter<IntValue>("SubScopeIndex", "The index of the sub-scope which should be removed. This parameter is ignored, if RemoveAllSubScopes is true."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope from which one or all sub-scopes should be removed."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SubScopesRemover(this, cloner);
    }

    public override IOperation Apply() {
      if (RemoveAllSubScopes)
        CurrentScope.SubScopes.Clear();
      else {
        CurrentScope.SubScopes.RemoveAt(SubScopeIndexParameter.ActualValue.Value);
      }
      return base.Apply();
    }
  }
}
