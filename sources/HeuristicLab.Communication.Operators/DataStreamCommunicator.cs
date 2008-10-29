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
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Communication.Data;

namespace HeuristicLab.Communication.Operators {
  public class DataStreamCommunicator : CommunicatorBase {
    public override string Description {
      get {
        return @"Sends a message if present and otherwise listens for incoming messages";
      }
    }

    public DataStreamCommunicator() {
      AddVariableInfo(new VariableInfo("DataStream", "", typeof(IDataStream), VariableKind.In));
    }

    private string Encode(Message message) {
      XmlDocument document = new XmlDocument();
      return message.GetXmlNode("Message", document, new Dictionary<Guid, IStorable>()).ToString();
    }

    private Message Decode(string m) {
      XmlDocument document = new XmlDocument();
      document.LoadXml(m);
      Message message = new Message();
      message.Populate(document.SelectSingleNode("Message"), new Dictionary<Guid, IStorable>());
      return message;
    }

    protected override void Send(IScope scope, Protocol protocol, ProtocolState currentState, Message message) {
      IDataStream connection = scope.GetVariableValue<IDataStream>("DataStream", true);
      connection.Write("PROTOCOL_ID " + protocol.Name);
      if (connection.Read().Equals("ACK")) {
        connection.Write("STATE_ID " + currentState.Name);
        if (connection.Read().Equals("ACK")) {
          connection.Write(Encode(message));
        }
      }
    }

    protected override Message Receive(IScope scope, Protocol protocol, ProtocolState currentState) {
      IDataStream connection = scope.GetVariableValue<IDataStream>("DataStream", true);
      Message message = new Message();
      string rcvd = connection.Read();
      if (rcvd.StartsWith("PROTOCOL_ID ")) {
        if (rcvd.Substring(12).Equals(protocol.Name)) {
          connection.Write("ACK");
          rcvd = connection.Read();
          if (rcvd.StartsWith("STATE_ID ")) {
            if (rcvd.Substring(9).Equals(currentState.Name)) {
              connection.Write("ACK");
              message = Decode(connection.Read());
              return message;
            } else {
              connection.Write("SYNCERROR STATE_ID");
              return null;
            }
          } else {
            connection.Write("ERROR");
            return null;
          }
        } else {
          connection.Write("SYNCERROR PROTOCOL_ID");
          return null;
        }
      } else {
        connection.Write("ERROR");
        return null;
      }
    }
  }
}
