using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Communication.Data;

namespace HeuristicLab.Communication.Operators {
  public class SimpleReceiveItemDeserializer : OperatorBase {
    public override string Description {
      get {
        return @"Deserializes data that was serialized by the SimpleSendItemSerializer";      }
    }

    public SimpleReceiveItemDeserializer() {
      AddVariableInfo(new VariableInfo("CurrentState", "The current state defines the items which are to be sent and thus need to be serialized", typeof(ProtocolState), VariableKind.In));
      AddVariableInfo(new VariableInfo("SerializedItem", "The string serialization of the item", typeof(StringData), VariableKind.Deleted));
      AddVariableInfo(new VariableInfo("Dictionary", "The dictionary that translates received variable names", typeof(ItemDictionary<StringData, StringData>), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      ProtocolState currentState = GetVariableValue<ProtocolState>("CurrentState", scope, true);
      ItemDictionary<StringData, StringData> dict = GetVariableValue<ItemDictionary<StringData, StringData>>("Dictionary", scope, true, false);
      ConstrainedItemList receivingData = currentState.ReceivingData;

      if (receivingData.Count > 0) {
        string received = GetVariableValue<StringData>("SerializedItem", scope, false).Data;
        StringReader reader = new StringReader(received);

        do {
          
          string line = reader.ReadLine();
          if (line == null) break;
          string name = line;
          line = reader.ReadLine();
          string type = line;
          line = reader.ReadLine();
          string value = line;

          int index = 0;
          while (index < receivingData.Count && !name.Equals(((Variable)receivingData[index]).Name))
            index++;

          if (index != receivingData.Count) {
            Type itemType = ((Variable)receivingData[index]).Value.GetType();
            Variable item = (Variable)receivingData[index].Clone();

            if (itemType.Equals(typeof(IntData)))
              ((IntData)item.Value).Data = int.Parse(value);
            else if (itemType.Equals(typeof(DoubleData)))
              ((DoubleData)item.Value).Data = double.Parse(value);
            else if (itemType.Equals(typeof(ConstrainedIntData)))
              ((ConstrainedIntData)item.Value).Data = int.Parse(value);
            else if (itemType.Equals(typeof(ConstrainedDoubleData)))
              ((ConstrainedDoubleData)item.Value).Data = double.Parse(value);
            else throw new NotImplementedException();

            // if a dictionary exists try to lookup a translation
            if (dict != null) {
              StringData tmp;
              if (dict.TryGetValue(new StringData(item.Name), out tmp))
                item.Name = tmp.Data;
            }

            // add/overwrite the read variable to scope
            IVariable scopeVar = scope.GetVariable(item.Name);
            if (scopeVar == null)
              scope.AddVariable(item);
            else
              scopeVar.Value = item.Value;
          }
        } while (true);
        reader.Close();

        IVariableInfo info = GetVariableInfo("SerializedItem");
        if (info.Local) {
          RemoveVariable(info.ActualName);
        } else {
          scope.RemoveVariable(scope.TranslateName(info.FormalName));
        }
      }

      return null;
    }
  }
}
