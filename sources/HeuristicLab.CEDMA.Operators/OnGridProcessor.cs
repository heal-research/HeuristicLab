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

namespace HeuristicLab.CEDMA.Operators {
  public class OnGridProcessor : OperatorBase {
    public override string Description {
      get { return "TASK."; }
    }

    public OnGridProcessor()
      : base() {
      AddVariableInfo(new VariableInfo("AgentId", "Id of the agent that the run should be associated to.", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("OperatorGraph", "The operator graph that should be executed on the grid", typeof(IOperatorGraph), VariableKind.In));
      AddVariableInfo(new VariableInfo("CedmaServerUri", "Uri of the CEDMA server", typeof(StringData), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      IOperatorGraph operatorGraph = scope.GetVariableValue<IOperatorGraph>("OperatorGraph", false);
      string serverUrl = scope.GetVariableValue<StringData>("CedmaServerUri", true).Data;
      long agentId = scope.GetVariableValue<IntData>("AgentId", true).Data;

      Agent agent = new Agent();
      foreach(IOperator op in operatorGraph.Operators) {
        agent.OperatorGraph.AddOperator(op);
      }
      agent.OperatorGraph.InitialOperator = operatorGraph.InitialOperator;

      NetTcpBinding binding = new NetTcpBinding();
      binding.MaxReceivedMessageSize = 10000000; // 10Mbytes
      binding.ReaderQuotas.MaxStringContentLength = 10000000; // also 10M chars
      binding.ReaderQuotas.MaxArrayLength = 10000000; // also 10M elements;
      binding.Security.Mode = SecurityMode.None;
      using(ChannelFactory<IDatabase> factory = new ChannelFactory<IDatabase>(binding)) {
        IDatabase database = factory.CreateChannel(new EndpointAddress(serverUrl));
        long id = database.InsertAgent(agentId, null, DbPersistenceManager.Save(agent));
        database.UpdateAgent(id, ProcessStatus.Waiting);
      }
      return null;
    }
  }
}
