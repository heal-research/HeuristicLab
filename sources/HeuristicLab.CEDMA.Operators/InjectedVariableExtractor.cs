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
  public class InjectedVariableExtractor : OperatorBase {
    public override string Description {
      get { return "TASK."; }
    }

    public InjectedVariableExtractor()
      : base() {
      AddVariableInfo(new VariableInfo("AgentId", "Id of the agent to extract injected variables from.", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("CedmaServerUri", "Uri of the CEDMA server", typeof(StringData), VariableKind.In));
      AddVariableInfo(new VariableInfo("VariableName", "Name of the injected variable that should be extracted", typeof(StringData), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      string serverUrl = GetVariableValue<StringData>("CedmaServerUri", scope, true).Data;
      long agentId = GetVariableValue<IntData>("AgentId", scope, true).Data;
      string variableName = GetVariableValue<StringData>("VariableName", scope, true).Data;

      NetTcpBinding binding = new NetTcpBinding();
      binding.MaxReceivedMessageSize = 10000000; // 10Mbytes
      binding.ReaderQuotas.MaxStringContentLength = 10000000; // also 10M chars
      binding.ReaderQuotas.MaxArrayLength = 10000000; // also 10M elements;
      binding.Security.Mode = SecurityMode.None;
      using(ChannelFactory<IDatabase> factory = new ChannelFactory<IDatabase>(binding)) {
        IDatabase database = factory.CreateChannel(new EndpointAddress(serverUrl));
        IOperatorGraph opGraph = (IOperatorGraph)PersistenceManager.RestoreFromGZip(database.GetAgentRawData(agentId));        
        OperatorLinkPatcher.LinkDatabase(opGraph, database);
        IVariable var = FindInjectedVariable(database, variableName, opGraph);
        if(var != null) {
          scope.AddVariable(var);
        }
      }
      return null;
    }

    private IVariable FindInjectedVariable(IDatabase database, string variableName, IOperatorGraph opGraph) {
      foreach(IOperator op in opGraph.Operators) {
        IVariable var = FindInjectedVariable(database, variableName, op);
        if(var != null) return var;
      }
      return null;
    }

    private IVariable FindInjectedVariable(IDatabase database, string variableName, IOperator op) {
      IVariable var = op.GetVariable(variableName);
      if(var != null) return var;
      else if(op is CombinedOperator) {
        var = FindInjectedVariable(database, variableName, ((CombinedOperator)op).OperatorGraph);
      } else if(op is OperatorLink) {
        OperatorLink link = op as OperatorLink;
        OperatorEntry targetEntry = database.GetOperator(link.Id);
        IOperator target = (IOperator)PersistenceManager.RestoreFromGZip(targetEntry.RawData);
        OperatorLinkPatcher.LinkDatabase(target, database);
        link.Operator = target;

        var = FindInjectedVariable(database, variableName, link.Operator);
      }
      return var;
    }
  }
}
