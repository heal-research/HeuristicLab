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
using HeuristicLab.Grid;
using System.Diagnostics;
using HeuristicLab.Data;
using HeuristicLab.CEDMA.Core;
using HeuristicLab.Operators;

namespace HeuristicLab.CEDMA.Server {
  public class RunScheduler {
    private class Job {
      public long AgentId;
      public WaitHandle WaitHandle;
      public AtomicOperation Operation;
    }
    private string serverUri;
    private Database database;
    private JobManager jobManager;
    private const int RELEASE_INTERVAL = 5;
    private object remoteCommLock = new object();
    private object queueLock = new object();
    private Queue<Job> jobQueue;
    private AutoResetEvent runningJobs = new AutoResetEvent(false);

    public RunScheduler(Database database, JobManager jobManager, string serverUri) {
      this.database = database;
      this.jobManager = jobManager;
      this.serverUri = serverUri;
      jobQueue = new Queue<Job>();
      Thread resultsGatheringThread = new Thread(GatherResults);
      resultsGatheringThread.Start();
    }
    public void Run() {
      while(true) {
        ReleaseWaitingRuns();
        Thread.Sleep(TimeSpan.FromSeconds(RELEASE_INTERVAL));
      }
    }
    private void ReleaseWaitingRuns() {
      ICollection<AgentEntry> agents;
      lock(remoteCommLock) {
        agents = database.GetAgents(ProcessStatus.Waiting);
      }
      foreach(AgentEntry entry in agents) {
        Scope scope = new Scope();
        // initialize CEDMA variables for the execution of the agent
        scope.AddVariable(new Variable("AgentId", new IntData((int)entry.Id)));
        scope.AddVariable(new Variable("CedmaServerUri", new StringData(serverUri)));

        byte[] rawData = database.GetAgentRawData(entry.Id);
        IOperatorGraph opGraph = (IOperatorGraph)PersistenceManager.RestoreFromGZip(rawData);

        PatchLinks(opGraph, new Dictionary<long, IOperator>());

        AtomicOperation operation = new AtomicOperation(opGraph.InitialOperator, scope);
        WaitHandle wHandle;
        lock(remoteCommLock) {
          wHandle = jobManager.BeginExecuteOperation(operation.Scope, operation);
          database.UpdateAgent(entry.Id, ProcessStatus.Active);
        }

        Job job = new Job();
        job.AgentId = entry.Id;
        job.Operation = operation;
        job.WaitHandle = wHandle;

        lock(queueLock) {
          jobQueue.Enqueue(job);
          runningJobs.Set();
        }
      }
    }

    private void PatchLinks(IOperatorGraph opGraph, Dictionary<long, IOperator> patchedOperators) {
      Dictionary<IOperator, IOperator> patchDictionary = new Dictionary<IOperator, IOperator>();
      foreach(IOperator op in opGraph.Operators) {
        IOperator patched = PatchLinks(op, patchedOperators);
        patchDictionary.Add(op, patched);
      }
      foreach(KeyValuePair<IOperator, IOperator> p in patchDictionary) {
        IOperator original = p.Key;
        IOperator patch = p.Value;
        if(original != patch) {
          foreach(IOperator subOperator in original.SubOperators) {
            patch.AddSubOperator(subOperator);
          }
          if(opGraph.InitialOperator == original)
            opGraph.InitialOperator = patch;
          opGraph.RemoveOperator(original.Guid);
          opGraph.AddOperator(patch);
        }
      }
    }

    private IOperator PatchLinks(IOperator op, Dictionary<long, IOperator> patchedOperators) {
      if(op is OperatorLink) {
        OperatorLink link = op as OperatorLink;
        if(patchedOperators.ContainsKey(link.Id)) {
          return patchedOperators[link.Id];
        } else {
          OperatorEntry targetEntry = database.GetOperator(link.Id);
          IOperator target = (IOperator)PersistenceManager.RestoreFromGZip(targetEntry.RawData);
          patchedOperators.Add(link.Id, target);
          PatchLinks(target, patchedOperators);
          return target;
        }
      } else if(op is CombinedOperator) {
        PatchLinks(((CombinedOperator)op).OperatorGraph, patchedOperators);
        return op;
      }
      return op;
    }

    private void GatherResults() {
      try {
        while(true) {
          Job job = null;
          lock(queueLock) if(jobQueue.Count > 0) job = jobQueue.Dequeue();
          if(job == null) runningJobs.WaitOne();
          else {
            job.WaitHandle.WaitOne();
            job.WaitHandle.Close();
            lock(remoteCommLock) {
              try {
                jobManager.EndExecuteOperation(job.Operation);
                database.UpdateAgent(job.AgentId, ProcessStatus.Finished);
              } catch(JobExecutionException ex) {
                database.UpdateAgent(job.AgentId, ProcessStatus.Error);
              }
            }
          }
        }
      } finally {
        Debug.Assert(false); // make sure we are notified when this thread is killed while debugging
      }
    }
  }
}
