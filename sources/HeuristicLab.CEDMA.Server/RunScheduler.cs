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
using HeuristicLab.Grid;
using System.Diagnostics;

namespace HeuristicLab.CEDMA.Server {
  public class RunScheduler {
    private class Job {
      public long AgentId;
      public WaitHandle WaitHandle;
      public AtomicOperation Operation;
    }
    private Database database;
    private JobManager jobManager;
    private const int RELEASE_INTERVAL = 5;
    private object remoteCommLock = new object();
    private object queueLock = new object();
    private Queue<Job> jobQueue;

    public RunScheduler(Database database, JobManager jobManager) {
      this.database = database;
      this.jobManager = jobManager;
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
      IEnumerable<AgentEntry> agents;
      lock(remoteCommLock) {
        agents = database.GetAgents(ProcessStatus.Waiting).Where(a=>!a.ControllerAgent);
      }
      foreach(AgentEntry entry in agents) {
        IOperatorGraph opGraph = (IOperatorGraph)DbPersistenceManager.Restore(entry.RawData);
        AtomicOperation op = new AtomicOperation(opGraph.InitialOperator, new Scope());
        WaitHandle wHandle;
        lock(remoteCommLock) {
          wHandle = jobManager.BeginExecuteOperation(op.Scope, op);
          database.UpdateAgent(entry.Id, ProcessStatus.Active);
        }

        Job job = new Job();
        job.AgentId = entry.Id;
        job.Operation = op;
        job.WaitHandle = wHandle;

        lock(queueLock) {
          jobQueue.Enqueue(job);
        }
      }
    }

    private void GatherResults() {
      try {
        while(true) {
          int runningJobs;
          lock(queueLock) runningJobs = jobQueue.Count;
          if(runningJobs==0) Thread.Sleep(1000); // TASK: replace with waithandle
          else {
            Job job;
            lock(queueLock) {
              job = jobQueue.Dequeue();
            }
            job.WaitHandle.WaitOne();
            job.WaitHandle.Close();
            lock(remoteCommLock) {
              jobManager.EndExecuteOperation(job.Operation);
              database.UpdateAgent(job.AgentId, ProcessStatus.Finished);
            }
          }
        }
      } finally {
        Debug.Assert(false); // make sure we are notified when this thread is killed while debugging
      }
    }
  }
}
