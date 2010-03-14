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

using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// An operator which collects the actual values of parameters and clones them into the current scope.
  /// </summary>
  [Item("VariableCreator", "An operator which collects the actual values of parameters and clones them into the current scope.")]
  [Creatable("Test")]
  [StorableClass]
  public class VariableCreator : ValuesCollector {
    protected ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    public IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }

    public VariableCreator()
      : base() {
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope into which the parameter values are cloned."));
    }

    public override IOperation Apply() {
      IVariable var;
      foreach (IParameter param in CollectedValues) {
        CurrentScope.Variables.TryGetValue(param.Name, out var);
        if (var != null)
          var.Value = (IItem)param.ActualValue.Clone();
        else
          CurrentScope.Variables.Add(new Variable(param.Name, (IItem)param.ActualValue.Clone()));
      }
      return base.Apply();
    }
  }
}
