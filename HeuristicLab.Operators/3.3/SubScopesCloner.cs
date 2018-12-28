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

using HeuristicLab.Core;
using HEAL.Fossil;
using HeuristicLab.Common;
using HeuristicLab.Parameters;
using HeuristicLab.Data;

namespace HeuristicLab.Operators {
  /// <summary>
  /// An operator which creates multiple copies of the subscopes of the current scope.
  /// </summary>
  [Item("SubScopesCloner", "An operator which creates multiple copies of the subscopes of the current scope.")]
  [StorableType("1F299CDC-13E0-4F12-9E2D-7D3CAC8C651C")]
  public class SubScopesCloner : SingleSuccessorOperator {
    private ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    private ValueParameter<IntValue> NumberOfCopiesParameter {
      get { return (ValueParameter<IntValue>)Parameters["NumberOfCopies"]; }
    }

    public IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }
    public IntValue NumberOfCopies {
      get { return NumberOfCopiesParameter.Value; }
      set { NumberOfCopiesParameter.Value = value;  }
    }


    [StorableConstructor]
    protected SubScopesCloner(StorableConstructorFlag _) : base(_) { }
    protected SubScopesCloner(SubScopesCloner original, Cloner cloner)
      : base(original, cloner) {
    }
    public SubScopesCloner()
      : base() { 
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope whose sub-scopes should be cloned."));
      Parameters.Add(new ValueParameter<IntValue>("NumberOfCopies", "The number of copies that should be created.", new IntValue(1)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SubScopesCloner(this, cloner);
    }

    public override IOperation Apply() {
      ScopeList subScopes = new ScopeList(CurrentScope.SubScopes);

      for (int i = 0; i < NumberOfCopies.Value; i++) {
        foreach (Scope scope in subScopes) {
          CurrentScope.SubScopes.Add(scope.Clone() as IScope);
        }
      }

       return base.Apply();
    }
  }
}
