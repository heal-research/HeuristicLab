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
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HEAL.Fossil;

namespace HeuristicLab.Optimization.Operators {
  /// <summary>
  /// A base class for operators that perform path relinking between single objective solutions.
  /// </summary>
  [Item("SingleObjectivePathRelinker", "A base class for operators that perform path relinking between single objective solutions.")]
  [StorableType("364CE8A7-0258-4EC1-BBDB-E40C944BD196")]
  public abstract class SingleObjectivePathRelinker : SingleSuccessorOperator, ISingleObjectivePathRelinker {
    #region Parameter properties
    public ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    public ILookupParameter<ItemArray<IItem>> ParentsParameter {
      get { return (IScopeTreeLookupParameter<IItem>)Parameters["Parents"]; }
    }
    public IValueParameter<PercentValue> RelinkingAccuracyParameter {
      get { return (IValueParameter<PercentValue>)Parameters["RelinkingAccuracy"]; }
    }
    #endregion

    #region Properties
    private IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }
    private ItemArray<IItem> Parents {
      get { return ParentsParameter.ActualValue; }
    }
    private PercentValue RelinkingAccuracy {
      get { return RelinkingAccuracyParameter.Value; }
    }
    #endregion

    [StorableConstructor]
    protected SingleObjectivePathRelinker(StorableConstructorFlag _) : base(_) { }
    protected SingleObjectivePathRelinker(SingleObjectivePathRelinker original, Cloner cloner) : base(original, cloner) { }
    protected SingleObjectivePathRelinker()
      : base() {
      #region Create parameters
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope that contains the parents."));
      Parameters.Add(new ScopeTreeLookupParameter<IItem>("Parents", "The parents used for path relinking."));
      Parameters.Add(new ValueParameter<PercentValue>("RelinkingAccuracy", "The percentage of relinked offspring that should be yielded.", new PercentValue(0.25)));
      #endregion
    }

    public sealed override IOperation Apply() {
      ItemArray<IItem> relinkedSolutions = Relink(Parents, RelinkingAccuracy);
      var offspringScope = new Scope("Offspring");
      foreach (var solution in relinkedSolutions) {
        var scope = new Scope();
        scope.Variables.Add(new Variable(ParentsParameter.ActualName, solution));
        offspringScope.SubScopes.Add(scope);
      }
      CurrentScope.SubScopes.Add(offspringScope);
      return base.Apply();
    }

    protected abstract ItemArray<IItem> Relink(ItemArray<IItem> parents, PercentValue n);
  }
}
