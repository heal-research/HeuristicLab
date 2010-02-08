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
using System.Linq;
using System.Text;
using System.Xml;
using HeuristicLab.Collections;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// An operator which collects the actual values of parameters and clones them into the current scope.
  /// </summary>
  [Item("VariableInjector", "An operator which collects the actual values of parameters and clones them into the current scope.")]
  [Creatable("Test")]
  [EmptyStorableClass]
  public class VariableInjector : ValueCollector {
    protected ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    public IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }

    public VariableInjector()
      : base() {
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope into which the parameter values are cloned."));
    }

    public override IExecutionContext Apply() {
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
