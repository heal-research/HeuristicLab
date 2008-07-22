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
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Data;

namespace HeuristicLab.CEDMA.DB.Interfaces {
  [ServiceContract(Namespace = "http://HeuristicLab.CEDMA.DB")]
  public interface IDatabase {
    [OperationContract]
    long InsertAgent(long? parentAgentId, string name, bool controllerAgent, byte[] rawData);

    [OperationContract(Name = "UpdateAgentName")]
    void UpdateAgent(long id, string name);

    [OperationContract(Name = "UpdateAgentStatus")]
    void UpdateAgent(long id, ProcessStatus status);

    [OperationContract(Name = "UpdateAgentData")]
    void UpdateAgent(long id, byte[] rawData);

    [OperationContract]
    long InsertResult(long agentId, string summary, string description, byte[] rawData);

    [OperationContract]
    long InsertSubResult(long resultId, string summary, string description, byte[] rawData);

    // should be replaced by more powerful querying interface (LINQ provider?)
    [OperationContract]
    ICollection<AgentEntry> GetAgents();

    [OperationContract]
    ICollection<AgentEntry> GetSubAgents(long parentAgentId);

    [OperationContract]
    ICollection<ResultEntry> GetResults(long agentId);

    [OperationContract]
    ICollection<ResultEntry> GetSubResults(long parentResultId);

  }
}
