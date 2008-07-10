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
    public RunScheduler(Database database, JobManager jobManager) {
      this.database = database;
      this.jobManager = jobManager;
    }
    public void Run() {
      while(true) {
        ReleaseWaitingRuns();
        Thread.Sleep(TimeSpan.FromSeconds(RELEASE_INTERVAL));
      }
    }
    private void ReleaseWaitingRuns() {
      ICollection<RunEntry> runs = database.GetRuns(ProcessStatus.Waiting);
      jobManager.Reset();
      foreach(RunEntry entry in runs) {
        IOperatorGraph opGraph = (IOperatorGraph)DbPersistenceManager.Restore(entry.RawData);

        Scope scope = new Scope();
        AtomicOperation op = new AtomicOperation(opGraph.InitialOperator, scope);
        WaitHandle wHandle = jobManager.BeginExecuteOperation(scope, op);

        ThreadPool.QueueUserWorkItem(delegate(object state) {
          wHandle.WaitOne();
          jobManager.EndExecuteOperation(op);
          entry.Status = ProcessStatus.Finished;
          database.UpdateRunStatus(entry.Id, entry.Status);
          database.UpdateRunFinished(entry.Id, DateTime.Now);
        });

        entry.Status = ProcessStatus.Active;
        database.UpdateRunStatus(entry.Id, entry.Status);
        database.UpdateRunStart(entry.Id, DateTime.Now);
      }
    }
  }
}
