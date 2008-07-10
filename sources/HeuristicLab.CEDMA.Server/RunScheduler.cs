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

namespace HeuristicLab.CEDMA.Server {
  public class RunScheduler {
    private Database database;
    private JobManager jobManager;
    private const int RELEASE_INTERVAL = 5;
    private object remoteCommLock = new object();
    private object collectionsLock = new object();
    private Queue<WaitHandle> waithandles;
    private Dictionary<WaitHandle, AtomicOperation> runningOperations;
    private Dictionary<WaitHandle, long> runningEntries;

    public RunScheduler(Database database, JobManager jobManager) {
      this.database = database;
      this.jobManager = jobManager;
      runningOperations = new Dictionary<WaitHandle, AtomicOperation>();
      runningEntries = new Dictionary<WaitHandle, long>();
      waithandles = new Queue<WaitHandle>();
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
      ICollection<RunEntry> runs;
      lock(remoteCommLock) {
        runs = database.GetRuns(ProcessStatus.Waiting);
      }
      foreach(RunEntry entry in runs) {
        IOperatorGraph opGraph = (IOperatorGraph)DbPersistenceManager.Restore(entry.RawData);
        Scope scope = new Scope();
        AtomicOperation op = new AtomicOperation(opGraph.InitialOperator, scope);
        WaitHandle wHandle;
        lock(remoteCommLock) {
          wHandle = jobManager.BeginExecuteOperation(scope, op);
          database.UpdateRunStatus(entry.Id, ProcessStatus.Active);
          database.UpdateRunStart(entry.Id, DateTime.Now);
        }

        lock(collectionsLock) {
          waithandles.Enqueue(wHandle);
          runningOperations[wHandle] = op;
          runningEntries[wHandle] = entry.Id;
        }
      }
    }

    private void GatherResults() {
      while(true) {
        if(waithandles.Count == 0) Thread.Sleep(1000);
        else {
          WaitHandle w;
          lock(collectionsLock) {
            w = waithandles.Dequeue();
          }
          w.WaitOne();
          long id;
          AtomicOperation op;
          lock(collectionsLock) {
            id = runningEntries[w];
            runningEntries.Remove(w);
            op = runningOperations[w];
            runningOperations.Remove(w);
          }
          w.Close();
          lock(remoteCommLock) {
            jobManager.EndExecuteOperation(op);
            database.UpdateRunStatus(id, ProcessStatus.Finished);
            database.UpdateRunFinished(id, DateTime.Now);
          }
        }
      }
    }
  }
}
