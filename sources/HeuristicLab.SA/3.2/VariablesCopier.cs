#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2009 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.SA {
  public class VariablesCopier : OperatorBase {
    
    public override string Description {
      get { return @"Copies all variables from a subscope to the current scope."; }
    }

    public VariablesCopier()
      : base() {
      AddVariableInfo(new VariableInfo("SubScopeIndex", "Tells the operator to copy the variables from the subscope with the given 0 based index", typeof(IntData), VariableKind.In));
      GetVariableInfo("SubScopeIndex").Local = true;
      AddVariable(new Variable("SubScopeIndex", new IntData(0)));
    }

    public override IOperation Apply(IScope scope) {
      IntData index = GetVariableValue<IntData>("SubScopeIndex", scope, true);
      ICollection<IVariable> vars = scope.SubScopes[index.Data].Variables;

      foreach (IVariable v in vars) {
        scope.AddVariable((IVariable)v.Clone());
      }

      return null;
    }
  }
}
