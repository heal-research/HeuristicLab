using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Communication.Data;

namespace HeuristicLab.Communication.Operators {
  public class SimpleSendItemSerializer : OperatorBase {
    public override string Description {
      get {
        return @"Serializes the data defined under ""Send"" in the current state of the protocol. It uses a simple scheme that prints in each line: the name of the variable, the type of the containing value and the value itself.";
      }
    }

    public SimpleSendItemSerializer() {
      AddVariableInfo(new VariableInfo("CurrentState", "The current state defines the items which are to be sent and thus need to be serialized", typeof(ProtocolState), VariableKind.In));
      AddVariableInfo(new VariableInfo("SerializedItem", "The string serialization of the item", typeof(StringData), VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      ProtocolState currentState = GetVariableValue<ProtocolState>("CurrentState", scope, true);
      ConstrainedItemList sendingData = currentState.SendingData;

      if (sendingData.Count > 0) {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < sendingData.Count; i++) {
          sb.AppendLine(((Variable)sendingData[i]).Name);
          IVariable tmp = scope.GetVariable(((Variable)sendingData[i]).Name);
          if (tmp == null) throw new InvalidOperationException("ERROR in SimpleSendItemSerializer: variable " + ((Variable)sendingData[i]).Name + " not found!");
          sb.AppendLine(tmp.Value.GetType().ToString());
          sb.AppendLine(tmp.Value.ToString());
        }

        IVariableInfo info = GetVariableInfo("SerializedItem");
        if (info.Local) {
          AddVariable(new Variable(info.ActualName, new StringData(sb.ToString())));
        } else {
          scope.AddVariable(new Variable(scope.TranslateName(info.FormalName), new StringData(sb.ToString())));
        }
      }

      return null;
    }
  }
}
