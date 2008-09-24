using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Communication.Data;

namespace HeuristicLab.Communication.Operators {
  public class XMLConstrainedItemListDeserializer : OperatorBase {
    public override string Description {
      get {
        return @"TODO";
      }
    }

    public XMLConstrainedItemListDeserializer() {
      AddVariableInfo(new VariableInfo("CurrentState", "The current state of the protocol", typeof(ProtocolState), VariableKind.In));
      AddVariableInfo(new VariableInfo("SerializedItem", "The string serialization that is to be deserialized", typeof(StringData), VariableKind.In | VariableKind.Deleted));
      AddVariableInfo(new VariableInfo("Dictionary", "The dictionary that translates received variable names", typeof(ItemDictionary<StringData, StringData>), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      ProtocolState currentState = GetVariableValue<ProtocolState>("CurrentState", scope, true);
      ItemDictionary<StringData, StringData> dict = GetVariableValue<ItemDictionary<StringData, StringData>>("Dictionary", scope, true, false);
      StringData serializationStringData = GetVariableValue<StringData>("SerializedItem", scope, false, false);

      if (serializationStringData != null) {
        string serialization = serializationStringData.Data;

        XmlDocument document = new XmlDocument();
        document.LoadXml(serialization);
        ConstrainedItemList target = new ConstrainedItemList();
        target.Populate(document.SelectSingleNode("DATA"), new Dictionary<Guid, IStorable>());

        for (int i = 0; i < target.Count; i++) {
          if (dict != null) { // if a dictionary exists try to lookup a translation
            StringData tmp;
            if (dict.TryGetValue(new StringData(((Variable)target[i]).Name), out tmp))
              ((Variable)target[i]).Name = tmp.Data;
          }
          IVariable scopeVar = scope.GetVariable(((Variable)target[i]).Name);
          if (scopeVar == null)
            scope.AddVariable((Variable)target[i]);
          else
            scopeVar.Value = ((Variable)target[i]).Value;
        }

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
