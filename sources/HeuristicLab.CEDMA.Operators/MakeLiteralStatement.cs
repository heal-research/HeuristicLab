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
using HeuristicLab.Core;
using HeuristicLab.Data;
using System.Threading;
using HeuristicLab.CEDMA.DB.Interfaces;
using System.ServiceModel;
using HeuristicLab.CEDMA.Core;
using HeuristicLab.Operators;
using System.Xml;
using System.IO;

namespace HeuristicLab.CEDMA.Operators {
  public class MakeLiteralStatement : OperatorBase {
    private static readonly string cedmaNamespace = "http://www.heuristiclab.com/cedma/";

    public override string Description {
      get { return "TASK."; }
    }

    public MakeLiteralStatement()
      : base() {
      AddVariableInfo(new VariableInfo("CedmaServerUri", "Uri of the CEDMA server", typeof(StringData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Subject", "", typeof(IItem), VariableKind.In));
      AddVariableInfo(new VariableInfo("Predicate", "", typeof(StringData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Property", "", typeof(IItem), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      string serverUrl = GetVariableValue<StringData>("CedmaServerUri", scope, true).Data;
      IItem subject = GetVariableValue<IItem>("Subject", scope, true);
      StringData predicate = GetVariableValue<StringData>("Predicate", scope, true);
      IItem property = GetVariableValue<IItem>("Property", scope, true);

      NetTcpBinding binding = new NetTcpBinding();
      binding.MaxReceivedMessageSize = 10000000; // 10Mbytes
      binding.ReaderQuotas.MaxStringContentLength = 10000000; // also 10M chars
      binding.ReaderQuotas.MaxArrayLength = 10000000; // also 10M elements;
      binding.Security.Mode = SecurityMode.None;
      using(ChannelFactory<IStore> factory = new ChannelFactory<IStore>(binding)) {
        IStore store = factory.CreateChannel(new EndpointAddress(serverUrl));
        Statement s = new Statement(
          new Entity(cedmaNamespace + "Items/" + subject.Guid),
          new Entity(cedmaNamespace + predicate.Data),
          TranslateItem(property));
        store.Add(s);
      }
      return null;
    }

    private Resource TranslateItem(IItem property) {
      if(property is IntData) {
        return new Literal(((IntData)property).Data);
      } else if(property is DoubleData) {
        return new Literal(((DoubleData)property).Data);
      } else if(property is BoolData) {
        return new Literal(((BoolData)property).Data);
      } else if(property is StringData) {
        return new Literal(((StringData)property).Data);
      } else {
        XmlDocument doc = PersistenceManager.CreateXmlDocument();
        XmlNode root = PersistenceManager.Persist(property, doc, new Dictionary<Guid, IStorable>());
        doc.AppendChild(root);
        StringWriter writer = new StringWriter();
        doc.Save(writer);
        writer.Flush();
        return new SerializedLiteral(writer.ToString());
      }
    }
  }
}
