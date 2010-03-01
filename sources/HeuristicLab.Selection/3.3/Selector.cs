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

using System.Collections.Generic;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Selection {
  /// <summary>
  /// A base class for selection operators.
  /// </summary>
  [Item("Selector", "A base class for selection operators.")]
  [EmptyStorableClass]
  public abstract class Selector : SingleSuccessorOperator, ISelector {
    protected OptionalValueParameter<BoolData> CopySelectedParameter {
      get { return (OptionalValueParameter<BoolData>)Parameters["CopySelected"]; }
    }
    public IValueLookupParameter<IntData> NumberOfSelectedSubScopesParameter {
      get { return (ValueLookupParameter<IntData>)Parameters["NumberOfSelectedSubScopes"]; }
    }
    protected ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }

    public BoolData CopySelected {
      get { return CopySelectedParameter.Value; }
      set { CopySelectedParameter.Value = value; }
    }
    public IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }

    protected Selector()
      : base() {
      Parameters.Add(new OptionalValueParameter<BoolData>("CopySelected", "True if the selected sub-scopes should be copied, otherwise false.", new BoolData(false)));
      Parameters.Add(new ValueLookupParameter<IntData>("NumberOfSelectedSubScopes", "The number of sub-scopes which should be selected."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope from which sub-scopes should be selected."));
    }

    public sealed override IOperation Apply() {
      List<IScope> scopes = new List<IScope>(CurrentScope.SubScopes);
      IScope[] selected = Select(scopes);

      CurrentScope.SubScopes.Clear();
      IScope remainingScope = new Scope("Remaining");
      remainingScope.SubScopes.AddRange(scopes);
      CurrentScope.SubScopes.Add(remainingScope);
      IScope selectedScope = new Scope("Selected");
      selectedScope.SubScopes.AddRange(selected);
      CurrentScope.SubScopes.Add(selectedScope);

      return base.Apply();
    }

    protected abstract IScope[] Select(List<IScope> scopes);
  }
}
