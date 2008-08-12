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
using System.Xml;
using HeuristicLab.CEDMA.DB.Interfaces;
using HeuristicLab.Operators;

namespace HeuristicLab.CEDMA.Core {
  public class Agent : IAgent {
    public IDatabase Database { get; set; }
    public long Id { get; set; }
    public string Name { get; set; }
    public ProcessStatus Status { get; set; }
    public bool Terminated { get; set; }
    private OperatorGraph operatorGraph;

    public IOperatorGraph OperatorGraph {
      get { return operatorGraph; }
    }

    public Agent()
      : base() {
      operatorGraph = new OperatorGraph();
    }

    public Agent(IDatabase database, long id)
      : this() {
      Database = database;
      Id = id;
    }

    public void Save() {
      Database.UpdateAgent(Id, Name);
      Database.UpdateAgent(Id, Status);
      Database.UpdateAgent(Id, PersistenceManager.SaveToGZip(OperatorGraph));
    }

    public void Start() {
      Status = ProcessStatus.Waiting;
      Save();
    }

    public ICollection<IAgent> SubAgents {
      get {
        List<IAgent> agents = new List<IAgent>();
        foreach(AgentEntry entry in Database.GetSubAgents(Id)) {
          Agent newAgent = new Agent(Database, entry.Id);
          newAgent.Name = entry.Name;
          newAgent.Status = entry.Status;
          agents.Add(newAgent);
        }
        return agents;
      }
    }

    public ICollection<IResult> Results {
      get {
        List<IResult> results = new List<IResult>();
        foreach(ResultEntry entry in Database.GetResults(Id)) {
          Result result = new Result(Database, entry.Id);
          result.Summary = entry.Summary;
          result.Description = entry.Description;
          results.Add(result);
        }
        return results;
      }
    } 

    public IView CreateView() {
      if(OperatorGraph.Operators.Count == 0) {
        byte[] rawData = Database.GetAgentRawData(Id);
        IOperatorGraph opGraph = (IOperatorGraph)PersistenceManager.RestoreFromGZip(rawData);
        DownloadOperators(opGraph, new Dictionary<long, IOperator>());
        foreach(IOperator op in opGraph.Operators) OperatorGraph.AddOperator(op);
        OperatorGraph.InitialOperator = opGraph.InitialOperator;
      }
      return new AgentView(this);
    }

    private void DownloadOperators(IOperatorGraph opGraph, Dictionary<long, IOperator> downloaded) {
      foreach(IOperator op in opGraph.Operators) {
        DownloadOperators(op, downloaded);
      }
    }

    private void DownloadOperators(IOperator op, Dictionary<long, IOperator> downloaded) {
      if(op is OperatorLink) {
        OperatorLink link = op as OperatorLink;
        if(downloaded.ContainsKey(link.Id)) {
          link.Operator = downloaded[link.Id];
        } else {
          OperatorEntry targetEntry = Database.GetOperator(link.Id);
          IOperator target = (IOperator)PersistenceManager.RestoreFromGZip(targetEntry.RawData);
          downloaded.Add(link.Id, target);
          DownloadOperators(target, downloaded);
          link.Operator = target;
        }
      } else if(op is CombinedOperator) {
        DownloadOperators(((CombinedOperator)op).OperatorGraph, downloaded);
      }
    }
  }
}
