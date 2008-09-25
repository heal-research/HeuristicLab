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
