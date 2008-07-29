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
using HeuristicLab.Core;
using System.Collections;
using HeuristicLab.CEDMA.DB.Interfaces;
using System.Xml;
using System.Runtime.Serialization;
using System.IO;

namespace HeuristicLab.CEDMA.Core {
  public class AgentList : ItemBase, IAgentList {
    private List<IAgent> agentList;
    private IDatabase database;
    public IDatabase Database {
      get { return database; }
      set {
        database = value;
        ReloadList();
      }
    }

    private void ReloadList() {
      agentList.Clear();
      foreach(AgentEntry a in database.GetAgents()) {
        Agent newAgent = new Agent(Database, a.Id);
        newAgent.Name = a.Name;
        newAgent.Status = a.Status;
        IOperatorGraph opGraph = (IOperatorGraph)PersistenceManager.RestoreFromGZip(a.RawData);
        foreach(IOperator op in opGraph.Operators) newAgent.OperatorGraph.AddOperator(op);
        newAgent.OperatorGraph.InitialOperator = opGraph.InitialOperator;
        agentList.Add(newAgent);
      }
      FireChanged();
    }

    public AgentList()
      : base() {
      agentList = new List<IAgent>();
    }

    public void CreateAgent() {
      Agent agent = new Agent();
      agent.Name = DateTime.Now.ToString();
      agent.Status = ProcessStatus.Unknown;
      agent.Database = database;
      long id = database.InsertAgent(null, agent.Name, PersistenceManager.SaveToGZip(agent.OperatorGraph));
      agent.Id = id;
      agentList.Add(agent);
    }

    public IEnumerator<IAgent> GetEnumerator() {
      return agentList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    public override IView CreateView() {
      return new AgentListView(this);
    }
  }
}
