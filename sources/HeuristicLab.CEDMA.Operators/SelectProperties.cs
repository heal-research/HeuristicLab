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
using System.IO;
using System.Xml;

namespace HeuristicLab.CEDMA.Operators {
  public class SelectProperties : OperatorBase {
    private static readonly string cedmaNamespace = "http://www.heuristiclab.com/cedma/";

    public override string Description {
      get { return "TASK."; }
    }

    public SelectProperties()
      : base() {
      AddVariableInfo(new VariableInfo("CedmaServerUri", "Uri of the CEDMA server", typeof(StringData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SubjectGuid", "", typeof(StringData), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      string serverUrl = GetVariableValue<StringData>("CedmaServerUri", scope, true).Data;
      StringData subjectGuid = GetVariableValue<StringData>("SubjectGuid", scope, true);

      NetTcpBinding binding = new NetTcpBinding();
      binding.MaxReceivedMessageSize = 10000000; // 10Mbytes
      binding.ReaderQuotas.MaxStringContentLength = 10000000; // also 10M chars
      binding.ReaderQuotas.MaxArrayLength = 10000000; // also 10M elements;
      binding.Security.Mode = SecurityMode.None;
      using(ChannelFactory<IStore> factory = new ChannelFactory<IStore>(binding)) {
        IStore store = factory.CreateChannel(new EndpointAddress(serverUrl));
        Statement template = new Statement(
          new Entity(cedmaNamespace+subjectGuid.Data),
          new Entity(null),
          new Entity(null));
        IList<Statement> result = store.Select(template);

        foreach(Statement s in result) {
          if(s.Property is Literal) {
            scope.AddVariable(new Variable(s.Predicate.Uri.Replace(cedmaNamespace,""), TranslateLiteral((Literal)s.Property)));
          } else if(s.Property is SerializedLiteral) {
            scope.AddVariable(new Variable(s.Predicate.Uri.Replace(cedmaNamespace, ""), TranslateLiteral((SerializedLiteral)s.Property)));
          } else {
            scope.AddVariable(new Variable(s.Predicate.Uri, new StringData(((Entity)s.Property).Uri.Replace(cedmaNamespace, ""))));
          }
        }
      }
      return null;
    }

    private IItem TranslateLiteral(SerializedLiteral serializedLiteral) {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(serializedLiteral.RawData);      
      return (IItem)PersistenceManager.Restore(doc.ChildNodes[1], new Dictionary<Guid, IStorable>());
    }

    private IItem TranslateLiteral(Literal literal) {
      if(literal.Value is bool) {
        return new BoolData((bool)literal.Value);
      } else if(literal.Value is double) {
        return new DoubleData((double)literal.Value);
      } else if(literal.Value is int) {
        return new IntData((int)literal.Value);
      } else if(literal.Value is string) {
        return new StringData((string)literal.Value);
      } else {
        return new StringData(literal.Value.ToString());
      }
    }
  }
}
