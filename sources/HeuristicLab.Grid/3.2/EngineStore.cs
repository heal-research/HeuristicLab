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
using System.Linq;
using System.Threading;
using System.ServiceModel;
using System.Data.Common;

namespace HeuristicLab.Grid {
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
  public class EngineStore : IEngineStore {
    private Database database;

    private static readonly string dbFile = AppDomain.CurrentDomain.BaseDirectory + "/enginestore.db3";
    private static readonly string connectionString = "Data Source=\"" + dbFile + "\";Pooling=False";

    private int waitingJobs;
    public int WaitingJobs {
      get {
        return waitingJobs;
      }
    }
    private int runningJobs;
    public int RunningJobs {
      get {
        return runningJobs;
      }
    }
    private int results;
    public int WaitingResults {
      get {
        return results;
      }
    }
    private object locker = new object();
    public EngineStore() {
      DbProviderFactory fact;
      fact = DbProviderFactories.GetFactory("System.Data.SQLite");
      if(!System.IO.File.Exists(dbFile)) {
        database = new Database(connectionString);
        database.CreateNew();
      } else {
        database = new Database(connectionString);
      }

      // init counters
      waitingJobs = (int)database.GetJobCount(JobState.Waiting);
      runningJobs = (int)database.GetJobCount(JobState.Busy);
      results = (int)database.GetJobCount(JobState.Finished);
    }

    public bool TryTakeEngine(out Guid guid, out byte[] engine) {
      JobEntry nextWaitingJob = database.GetNextWaitingJob();
      if(nextWaitingJob == null) {
        guid = Guid.Empty; engine = null;
        return false;
      } else {
        guid = nextWaitingJob.Guid;
        engine = nextWaitingJob.RawData;
        lock(locker) {
          runningJobs++;
          waitingJobs--;
        }
        return true;
      }
    }

    public void StoreResult(Guid guid, byte[] result) {
      database.DeleteExpiredResults();
      // add the new result
      database.SetJobResult(guid, result);
      lock(locker) {
        results++;
        runningJobs--;
      }
    }

    internal void AddEngine(Guid guid, byte[] engine) {
      database.InsertJob(guid, JobState.Waiting, engine);
      lock(locker) {
        waitingJobs++;
      }
    }

    internal byte[] GetResult(Guid guid) {
      if(GetJobState(guid) == JobState.Finished) {
        JobEntry entry = database.GetJob(guid);
        return entry.RawData;
      } else {
        // JobState is Busy, Waiting or Unknown
        // no result yet, check for which jobs we waited too long and requeue those jobs
        database.RestartExpiredActiveJobs();
        return null;
      }
    }

    internal JobState GetJobState(Guid guid) {
      return database.GetJobState(guid);
    }
  }
}
