using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Communication.Data;

namespace HeuristicLab.Communication.Operators {
  public class MessageProcessor : OperatorBase {
    public override string Description {
      get {
        return @"If no ""Message"" is present it will call its first suboperator (communicator) to receive a ""Message"".
If a locally created ""Message"" is present it will call its first suboperator (communicator) to send it and if a response is expected call itself again to check for it.
If a received ""Message"" is present it will output it as ""Received"" and create a response ""Message"" if necessary.";
      }
    }

    public MessageProcessor()
      : base() {
      AddVariableInfo(new VariableInfo("Protocol", "The protocol that is used", typeof(Protocol), VariableKind.In));
      AddVariableInfo(new VariableInfo("PeerName", "The name of this peer", typeof(StringData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Message", "The message that is to be processed", typeof(Message), VariableKind.New | VariableKind.Out | VariableKind.Deleted));
      AddVariableInfo(new VariableInfo("Received", "A message that has been received is saved for further processing", typeof(Message), VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      Message message = GetVariableValue<Message>("Message", scope, false, false);
      if (message == null) { // Case 1: No message present -> call first SubOperator to create one (listen for one)
        if (SubOperators.Count > 0) {
          CompositeOperation co = new CompositeOperation();
          co.AddOperation(new AtomicOperation(SubOperators[0], scope));
          co.AddOperation(new AtomicOperation(this, scope));
          return co;
        } else throw new InvalidOperationException("ERROR in MessageProcessor: No message present and no suboperator defined which could listen and create one");
      } else {
        Protocol protocol = GetVariableValue<Protocol>("Protocol", scope, true);
        if (message.Protocol.Equals(protocol.Name)) {
          string peerName = GetVariableValue<StringData>("PeerName", scope, true).Data;
          if (message.Source.Equals(peerName)) { // Case 2: A locally created message is present
            if (message.Expect.Count > 0) { // Subcase 2.1: Expects response, so send it and call self afterwards to retrieve the response
              CompositeOperation co = new CompositeOperation();
              co.AddOperation(new AtomicOperation(SubOperators[0], scope));
              co.AddOperation(new AtomicOperation(this, scope));
              return co;
            } else { // Subcase 2.2: No response expected, just send it
              return new AtomicOperation(SubOperators[0], scope);
            }
          } else { // Case 3: A remotely created message received -> save message, and if necessary create response message
            if (message.Give.Count > 0) { // if data has been added for this
              IVariableInfo receivedInfo = GetVariableInfo("Received");
              if (receivedInfo.Local) {
                AddVariable(new Variable(receivedInfo.ActualName, (IItem)message.Clone()));
              } else {
                scope.AddVariable(new Variable(scope.TranslateName(receivedInfo.FormalName), (IItem)message.Clone()));
              }
            }
            if (message.Expect.Count > 0) { // if data has been requested, create a response
              message.Give = message.Expect;
              message.Expect = new List<IVariable>();
              message.Destination = message.Source;
              message.Source = peerName;
              message.Timestamp = DateTime.Now;
            } else { // otherwise delete the message
              IVariableInfo messageInfo = GetVariableInfo("Message");
              if (messageInfo.Local) {
                RemoveVariable(messageInfo.ActualName);
              } else {
                scope.RemoveVariable(scope.TranslateName(messageInfo.ActualName));
              }
            }
          }
        } else throw new InvalidOperationException("ERROR in MessageProcessor: Remote peer is using different protocol");
      }
      return null;
    }
  }
}