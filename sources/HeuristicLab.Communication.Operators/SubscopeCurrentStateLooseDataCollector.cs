using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Communication.Data;

namespace HeuristicLab.Communication.Operators {
  public class SubscopeCurrentStateLooseDataCollector : OperatorBase {
    public override string Description {
      get {
        return @"Browses through all subscopes and collects the data defined under ""Send"" in the current state of the protocol";
      }
    }

    public SubscopeCurrentStateLooseDataCollector() {
      AddVariableInfo(new VariableInfo("CurrentState", "The current state in the execution of the protocol", typeof(ProtocolState), VariableKind.In));
      AddVariableInfo(new VariableInfo("CombinedData", "The combinedData to be sent", typeof(StringData), VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      ProtocolState currentState = GetVariableValue<ProtocolState>("CurrentState", scope, true);

      if (currentState.SendingData.Count > 0) {
        XmlDocument document = new XmlDocument();
        XmlNode root = document.CreateNode(XmlNodeType.Element, "COMBINED_DATA", null);
        document.AppendChild(root);
        foreach (IScope subScope in scope.SubScopes) {
          ConstrainedItemList sendingData = (ConstrainedItemList)currentState.SendingData.Clone();
          for (int i = 0; i < sendingData.Count; i++) {
            Variable tmp = (Variable)sendingData[i];
            tmp.Value = subScope.GetVariableValue(tmp.Name, true);
          }

          XmlNode node = sendingData.GetXmlNode("DATA", document, new Dictionary<Guid, IStorable>());
          root.AppendChild(node);
        }

        IVariableInfo info = GetVariableInfo("CombinedData");
        if (info.Local) {
          AddVariable(new Variable(info.ActualName, new StringData(document.OuterXml)));
        } else {
          scope.AddVariable(new Variable(scope.TranslateName(info.FormalName), new StringData(document.OuterXml)));
        }
      }
      return null;
    }
  }
}