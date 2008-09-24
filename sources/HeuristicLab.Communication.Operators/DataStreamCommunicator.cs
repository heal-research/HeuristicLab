using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Communication.Data;

namespace HeuristicLab.Communication.Operators {
  public class DataStreamCommunicator : OperatorBase {
    public override string Description {
      get {
        return @"TODO";
      }
    }

    public DataStreamCommunicator() {
      AddVariableInfo(new VariableInfo("CurrentState", "", typeof(ProtocolState), VariableKind.In));
      AddVariableInfo(new VariableInfo("DataStream", "", typeof(IDataStream), VariableKind.In));
      AddVariableInfo(new VariableInfo("Message", "", typeof(StringData), VariableKind.New | VariableKind.Deleted));
    }

    public override IOperation Apply(IScope scope) {
      ProtocolState currentState = GetVariableValue<ProtocolState>("CurrentState", scope, true);
      IDataStream connection = GetVariableValue<IDataStream>("DataStream", scope, true);
      IVariableInfo info = GetVariableInfo("Message");
      string actualName = "";
      if (!info.Local)
        actualName = scope.TranslateName(info.FormalName);

      if (currentState.SendingData.Count > 0) {
        string toSend = GetVariableValue<StringData>(info.FormalName, scope, false).Data;
        connection.Write(toSend);
        if (info.Local) RemoveVariable(info.ActualName);
        else scope.RemoveVariable(actualName);
      }

      if (currentState.ReceivingData.Count > 0) {
        string received = connection.Read();
        if (info.Local) AddVariable(new Variable(info.ActualName, new StringData(received)));
        else scope.AddVariable(new Variable(actualName, new StringData(received)));
      }
      return null;
    }
  }
}
