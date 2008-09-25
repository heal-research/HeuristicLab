#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Communication.Data;

namespace HeuristicLab.Communication.Operators {
  public class XMLConstrainedItemListSerializer : OperatorBase {
    public override string Description {
      get {
        return @"TODO";
      }
    }

    public XMLConstrainedItemListSerializer() {
      AddVariableInfo(new VariableInfo("CurrentState", "The current state defines the items which are to be sent and thus need to be serialized", typeof(ProtocolState), VariableKind.In));
      AddVariableInfo(new VariableInfo("SerializedItem", "The string serialization of the items in the current state", typeof(StringData), VariableKind.New | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      ProtocolState currentState = GetVariableValue<ProtocolState>("CurrentState", scope, true);

      ConstrainedItemList sendingData = (ConstrainedItemList)currentState.SendingData.Clone();
      if (sendingData.Count > 0) {
        for (int i = 0; i < sendingData.Count; i++) {
          Variable tmp = (Variable)sendingData[i];
          tmp.Value = scope.GetVariableValue(tmp.Name, false);
        }
        XmlDocument document = new XmlDocument();
        string serialized = sendingData.GetXmlNode("DATA", document, new Dictionary<Guid, IStorable>()).OuterXml;

        IVariableInfo info = GetVariableInfo("SerializedItem");
        if (info.Local) {
          AddVariable(new Variable(info.ActualName, new StringData(serialized)));
        } else {
          IVariable scopeVar = scope.GetVariable(scope.TranslateName(info.FormalName));
          if (scopeVar == null) {
            scope.AddVariable(new Variable(scope.TranslateName(info.FormalName), new StringData(serialized)));
          } else {
            scopeVar.Value = new StringData(serialized);
          }
        }
      }

      return null;
    }
  }
}
