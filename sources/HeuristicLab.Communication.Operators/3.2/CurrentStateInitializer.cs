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
using HeuristicLab.Communication.Data;

namespace HeuristicLab.Communication.Operators {
  public class CurrentStateInitializer : OperatorBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    public CurrentStateInitializer()
      : base() {
      AddVariableInfo(new VariableInfo("Protocol", "", typeof(Protocol), VariableKind.In));
      AddVariableInfo(new VariableInfo("CurrentState", "", typeof(ProtocolState), VariableKind.New | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      Protocol p = GetVariableValue<Protocol>("Protocol", scope, true);
      ProtocolState currentState = GetVariableValue<ProtocolState>("CurrentState", scope, true, false);
      IVariableInfo info = GetVariableInfo("CurrentState");
      string actualName = "";
      if (!info.Local)
        actualName = scope.TranslateName(info.FormalName);
      if (currentState == null) {
        currentState = p.InitialState;
        if (info.Local)
          AddVariable(new Variable(info.ActualName, currentState));
        else
          scope.AddVariable(new Variable(actualName, currentState));
      } else {
        currentState = p.InitialState;
        if (info.Local) {
          if (GetVariable(info.ActualName) == null) AddVariable(new Variable(info.ActualName, currentState));
          else GetVariable(info.ActualName).Value = currentState;
        } else {
          if (scope.GetVariable(actualName) == null) scope.AddVariable(new Variable(actualName, currentState));
          else scope.GetVariable(actualName).Value = currentState;
        }
      }
      return null;
    }
  }
}
