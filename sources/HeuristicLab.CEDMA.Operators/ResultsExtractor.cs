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
  public class ResultsExtractor : OperatorBase {
    public override string Description {
      get { return "TASK."; }
    }

    public ResultsExtractor()
      : base() {
      AddVariableInfo(new VariableInfo("AgentId", "Id of the agent to extract injected variables from.", typeof(IntData), VariableKind.In));
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
        ExtractResults(database, scope, database.GetResults(agentId));
      }
      return null;
    }

    private void ExtractResults(IDatabase database, IScope scope, ICollection<ResultEntry> results) {
      foreach(ResultEntry result in results) {
        IItem item = (IItem)PersistenceManager.RestoreFromGZip(database.GetResultRawData(result.Id));
        Scope resultScope = new Scope();
        resultScope.AddVariable(new Variable(result.Summary, item));
        ExtractResults(database, resultScope, database.GetSubResults(result.Id));
        scope.AddSubScope(resultScope);
      }
    }
  }
}
