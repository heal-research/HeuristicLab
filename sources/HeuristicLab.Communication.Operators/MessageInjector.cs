using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Communication.Data;

namespace HeuristicLab.Communication.Operators {
  public class MessageInjector : OperatorBase {
    public override string Description {
      get {
        return @"Creates a message template for a communicator to send.";
      }
    }

    public MessageInjector()
      : base() {
      AddVariableInfo(new VariableInfo("Protocol", "", typeof(Protocol), VariableKind.In));
      AddVariableInfo(new VariableInfo("PeerName", "", typeof(StringData), VariableKind.In));
      AddVariableInfo(new VariableInfo("CurrentState", "The current state which to create a message", typeof(ProtocolState), VariableKind.In));
      AddVariableInfo(new VariableInfo("Message", "The message to inject", typeof(Message), VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      Protocol protocol = GetVariableValue<Protocol>("Protocol", scope, true);
      ProtocolState currentState = GetVariableValue<ProtocolState>("CurrentState", scope, true);
      string iPeer = GetVariableValue<StringData>("PeerName", scope, true).Data;
      Message message = new Message();
      message.Protocol = protocol.Name;
      message.Source = iPeer;
      message.Destination = "";
      for (int i = 0; i < currentState.Give.Count; i++)
        message.Give.Add((IVariable)currentState.Give[i].Clone());
      for (int i = 0; i < currentState.Expect.Count; i++)
        message.Expect.Add((IVariable)currentState.Expect[i].Clone());

      scope.AddVariable(new Variable(scope.TranslateName("Message"),message));
      return null;
    }
  }
}