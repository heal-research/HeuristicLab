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

namespace HeuristicLab.CEDMA.Operators {
  public class SplitScopeToItemsResultWriter : OperatorBase {
    public override string Description {
      get { return "TASK."; }
    }

    public SplitScopeToItemsResultWriter()
      : base() {
      AddVariableInfo(new VariableInfo("AgentId", "Id of the agent whose results should be transformed.", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("CedmaServerUri", "Uri of the CEDMA server", typeof(StringData), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      string serverUrl = GetVariableValue<StringData>("CedmaServerUri", scope, true).Data;
      long agentId = GetVariableValue<IntData>("AgentId", scope, true).Data;

      NetTcpBinding binding = new NetTcpBinding();
      binding.MaxReceivedMessageSize = 10000000; // 10Mbytes
      binding.ReaderQuotas.MaxStringContentLength = 10000000; // also 10M chars
      binding.ReaderQuotas.MaxArrayLength = 10000000; // also 10M elements;
      binding.Security.Mode = SecurityMode.None;
      using(ChannelFactory<IDatabase> factory = new ChannelFactory<IDatabase>(binding)) {
        IDatabase database = factory.CreateChannel(new EndpointAddress(serverUrl));
        TransformResults(database, agentId);
      }
      return null;
    }

    private void TransformResults(IDatabase database, long agentId) {
      TransformResults(database, database.GetResults(agentId));
      foreach(AgentEntry subAgent in database.GetSubAgents(agentId)) {
        TransformResults(database, subAgent.Id);
      }
    }

    private void TransformResults(IDatabase database, ICollection<ResultEntry> results) {
      foreach(ResultEntry entry in results) {
        byte[] rawData = database.GetResultRawData(entry.Id);
        object resultItem = PersistenceManager.RestoreFromGZip(rawData);
        if(resultItem is IScope) {
          IScope resultScope = resultItem as IScope;
          IVariable funTreeVariable = resultScope.GetVariable("FunctionTree");
          if(funTreeVariable != null) {
            long newResultId = 0;
            if(entry.AgentId.HasValue) {
              newResultId = database.InsertResult(entry.AgentId.Value, funTreeVariable.Name, resultScope.Name, PersistenceManager.SaveToGZip(funTreeVariable.Value));
            } else if(entry.ParentResultId.HasValue) {
              newResultId = database.InsertSubResult(entry.ParentResultId.Value, funTreeVariable.Name, resultScope.Name, PersistenceManager.SaveToGZip(funTreeVariable.Value));
            }
            foreach(IVariable var in resultScope.Variables) {
              if(var != funTreeVariable) {
                database.InsertSubResult(newResultId, var.Name, "", PersistenceManager.SaveToGZip(var.Value));
              }
            }
          }
        }
        TransformResults(database, database.GetSubResults(entry.Id));
      }
    }
  }
}
