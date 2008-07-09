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
    long InsertAgent(string name, byte[] rawData);

    [OperationContract(Name = "UpdateAgentName")]
    void UpdateAgent(long id, string name);

    [OperationContract(Name = "UpdateAgentStatus")]
    void UpdateAgent(long id, ProcessStatus status);

    [OperationContract(Name = "UpdateAgentData")]
    void UpdateAgent(long id, byte[] rawData);

    [OperationContract]
    long InsertRun(long agentId, byte[] rawData);

    [OperationContract]
    void UpdateRunStart(long runId, DateTime CreationTime);

    [OperationContract]
    void UpdateRunFinished(long runId, DateTime CreationTime);

    [OperationContract]
    void UpdateRunStatus(long runId, ProcessStatus status);

    [OperationContract]
    long InsertResult(long runId, byte[] rawData);

    [OperationContract]
    long InsertSubResult(long resultId, byte[] rawData);


    // should be replaced by more powerful querying interface (LINQ provider?)
    [OperationContract]
    ICollection<AgentEntry> GetAgents();

    [OperationContract]
    ICollection<RunEntry> GetRuns();

    [OperationContract]
    ICollection<ResultEntry> GetResults(long runId);

    [OperationContract]
    ICollection<ResultEntry> GetSubResults(long resultId);

  }
}
