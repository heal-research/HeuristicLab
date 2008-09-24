using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Communication.Data;

namespace HeuristicLab.Communication.Operators {
  public class CurrentStateVariableInjector : OperatorBase {
    public override string Description {
      get { return @"TODO\r\n add description"; }
    }

    public CurrentStateVariableInjector() {
      AddVariableInfo(new VariableInfo("CurrentState", "The current state when working up a protocol", typeof(ProtocolState), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      ProtocolState currentState = GetVariableValue<ProtocolState>("CurrentState", scope, true);
      ConstrainedItemList data = currentState.SendingData;
      for (int i = 0 ; i < data.Count ; i++) {
        scope.AddVariable((Variable)data[i].Clone(new Dictionary<Guid, object>()));
      }

      return null;
    }
  }
}
