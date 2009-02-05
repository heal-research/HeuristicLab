using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Communication.Data;

namespace HeuristicLab.Communication.Operators {
  public abstract class CommunicatorBase : OperatorBase {
    public CommunicatorBase()
      : base() {
      AddVariableInfo(new VariableInfo("PeerName", "", typeof(StringData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Protocol", "", typeof(Protocol), VariableKind.In));
      AddVariableInfo(new VariableInfo("CurrentState", "", typeof(ProtocolState), VariableKind.In));
      AddVariableInfo(new VariableInfo("Message", "The message to be sent and/or received. If it is not present the operator will listen for incomming messages.", typeof(Message), VariableKind.New | VariableKind.In | VariableKind.Out | VariableKind.Deleted));
    }

    public override IOperation Apply(IScope scope) {
      Protocol protocol = GetVariableValue<Protocol>("Protocol", scope, true);
      ProtocolState currentState = GetVariableValue<ProtocolState>("CurrentState", scope, true);
      
      IVariableInfo info = GetVariableInfo("Message");
      string actualName = info.ActualName;
      if (!info.Local)
        actualName = scope.TranslateName(info.FormalName);

      string peerName = GetVariableValue<StringData>("PeerName", scope, true).Data;
      Message message = GetVariableValue<Message>("Message", scope, false, false);

      if (message == null) { // RECEIVE MODE (no message present)
        message = Receive(scope, protocol, currentState);
        if (message == null) throw new InvalidOperationException("ERROR in Communicator: Message could not be received");
        if (!info.Local) scope.AddVariable(new Variable(actualName, message));
        else AddVariable(new Variable(actualName, message));
      } else { // SEND MODE (message present)
        if (message.Source.Equals(peerName)) {
          // originating here, send it
          Send(scope, protocol, currentState, message);
          if (!info.Local) scope.RemoveVariable(actualName);
          else RemoveVariable(actualName);
          // after sending the message if there is something to be received, apply this operator again
          if (currentState.Expect.Count > 0) return new AtomicOperation(this, scope);
        } else throw new ArgumentException("ERROR in Communicator: Message to be sent does not originate from this peer");
      }
      return null;
    }

    protected abstract void Send(IScope scope, Protocol protocol, ProtocolState currentState, Message message);
    protected abstract Message Receive(IScope scope, Protocol protocol, ProtocolState currentState);
  }
}
