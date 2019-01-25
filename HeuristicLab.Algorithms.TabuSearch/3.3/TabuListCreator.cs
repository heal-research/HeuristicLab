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
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.TabuSearch {
  [Item("TabuListCreator", "An operator that creates a new empty tabu list. It can also replace an existing tabu list with a new empty one.")]
  [StorableType("F199F8AB-35BA-45EB-A7E9-F6352FC053E3")]
  public class TabuListCreator : SingleSuccessorOperator {
    public ValueLookupParameter<ItemList<IItem>> TabuListParameter {
      get { return (ValueLookupParameter<ItemList<IItem>>)Parameters["TabuList"]; }
    }
    protected ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    public IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }

    [StorableConstructor]
    protected TabuListCreator(StorableConstructorFlag _) : base(_) { }
    protected TabuListCreator(TabuListCreator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new TabuListCreator(this, cloner);
    }

    public TabuListCreator()
      : base() {
      Parameters.Add(new ValueLookupParameter<ItemList<IItem>>("TabuList", "The tabu list.", new ItemList<IItem>()));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope."));
    }

    public override IOperation Apply() {
      IVariable var;
      CurrentScope.Variables.TryGetValue(TabuListParameter.ActualName, out var);
      if (var != null)
        var.Value = (IItem)TabuListParameter.ActualValue.Clone();
      else
        CurrentScope.Variables.Add(new Variable(TabuListParameter.ActualName, (IItem)TabuListParameter.ActualValue.Clone()));
      return base.Apply();
    }
  }
}
