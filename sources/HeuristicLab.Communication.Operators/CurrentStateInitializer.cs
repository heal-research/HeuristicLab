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
