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
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Scheduling.JSSP {
  public class CopyVariableFromSubScope : OperatorBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    public CopyVariableFromSubScope()
      : base() {
      AddVariableInfo(new VariableInfo("Variable", "Variable to move.", typeof(IItem), VariableKind.In | VariableKind.New | VariableKind.Out));
      AddVariableInfo(new VariableInfo("SubScope", "Index of the subscope.", typeof(IntData), VariableKind.In));
      GetVariableInfo("SubScope").Local = true;
      AddVariable(new Variable("SubScope", new IntData(0)));
    }

    public override IOperation Apply(IScope scope) {
      int scopeIndex = GetVariableValue<IntData>("SubScope", scope, false).Data;
      if(scope.SubScopes.Count >= scopeIndex + 1) {
        IScope s = scope.SubScopes[scopeIndex];
        IItem var = GetVariableValue<IItem>("Variable", s, false);
        if(var != null) {
          if(scope.GetVariable(scope.TranslateName("Variable")) != null) {
            scope.RemoveVariable(scope.TranslateName("Variable"));
          }
          scope.AddVariable(new Variable(scope.TranslateName("Variable"), var));
          return null;
        }
      }
      return null;
    }
  }
}
