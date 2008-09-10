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

namespace HeuristicLab.CEDMA.Operators {
  public class MakeItemStatement : OperatorBase {
    private static readonly string cedmaNamespace = "http://www.heuristiclab.com/cedma/";

    public override string Description {
      get { return "TASK."; }
    }

    public MakeItemStatement()
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
          new Entity(cedmaNamespace + "Items/" + property.Guid));
        store.Add(s);
      }
      return null;
    }
  }
}
