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
using HeuristicLab.CEDMA.DB;
using HeuristicLab.CEDMA.DB.Interfaces;
using HeuristicLab.Core;
using System.Threading;
using HeuristicLab.CEDMA.Core;

namespace HeuristicLab.CEDMA.Server {
  public class AgentScheduler {
    private Database database;
    private List<IEngine> engines;
    private Dictionary<IEngine, AgentEntry> agent;

    public AgentScheduler(Database database) {
      this.database = database;
      engines = new List<IEngine>();
      agent = new Dictionary<IEngine, AgentEntry>();
    }

    public void Run() {
      while(true) {
        ClearFinishedEngines();
        CreateNewEngines();
        if(engines.Count == 0) Thread.Sleep(10000);
        else StepAllEngines();
      }
    }
    private void ClearFinishedEngines() {
      List<IEngine> finishedEngines = new List<IEngine>();
      foreach(IEngine e in engines) {
        if(e.Terminated) {
          finishedEngines.Add(e);
        }
      }
      foreach(IEngine e in finishedEngines) {
        engines.Remove(e);
        AgentEntry entry = agent[e];
        entry.Status = ProcessStatus.Finished;
        database.UpdateAgent(entry.Id, entry.Status);
      }
    }
    private void CreateNewEngines() {
      ICollection<AgentEntry> agents = database.GetAgents(ProcessStatus.Waiting);
      foreach(AgentEntry a in agents) {
        SequentialEngine.SequentialEngine engine = new HeuristicLab.SequentialEngine.SequentialEngine();
        Agent newAgent = (Agent)DbPersistenceManager.Restore(a.RawData);
        engine.OperatorGraph.Clear();
        foreach(IOperator op in newAgent.OperatorGraph.Operators) engine.OperatorGraph.AddOperator(op);
        engine.OperatorGraph.InitialOperator = newAgent.OperatorGraph.InitialOperator;
        engine.Reset();

        agent[engine] = a;
        engines.Add(engine);
        a.Status = ProcessStatus.Active;
        database.UpdateAgent(a.Id, a.Status);
      }
    }
    private void StepAllEngines() {
      for(int steps = 0; steps < 100; steps++) {
        foreach(IEngine engine in engines) {
          engine.ExecuteStep();
        }
        Thread.Sleep(100); // prevent overload
      }
    }
  }
}
