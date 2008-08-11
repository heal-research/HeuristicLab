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
    private Dictionary<Guid, ManualResetEvent> waitHandles;
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

    public EngineStore() {
      waitHandles = new Dictionary<Guid, ManualResetEvent>();
      DbProviderFactory fact;
      fact = DbProviderFactories.GetFactory("System.Data.SQLite");
      if(!System.IO.File.Exists(dbFile)) {
        database = new Database(connectionString);
        database.CreateNew();
      } else {
        database = new Database(connectionString);
      }
      database = new Database(connectionString);
    }

    private object t = new object();
    public bool TryTakeEngine(out Guid guid, out byte[] engine) {
      lock(t) {
        List<JobEntry> waitingJobs = database.GetWaitingJobs();
        if(waitingJobs.Count == 0) {
          guid = Guid.Empty; engine = null;
          return false;
        } else {
          JobEntry oldestEntry = waitingJobs.OrderBy(a => a.CreationTime).First();
          guid = oldestEntry.Guid;
          engine = oldestEntry.RawData;
          database.UpdateJobState(guid, HeuristicLab.Grid.JobState.Busy);
          return true;
        }
      }
    }

    public void StoreResult(Guid guid, byte[] result) {
      database.DeleteExpiredResults();
      // add the new result
      database.SetJobResult(guid, result);
      waitHandles[guid].Set();
    }

    internal void AddEngine(Guid guid, byte[] engine) {
      database.InsertJob(guid, HeuristicLab.Grid.JobState.Waiting, engine);
      waitHandles.Add(guid, new ManualResetEvent(false));
    }

    internal byte[] GetResult(Guid guid) {
      return GetResult(guid, System.Threading.Timeout.Infinite);
    }

    internal byte[] GetResult(Guid guid, int timeout) {
      if(JobState(guid) == HeuristicLab.Grid.JobState.Finished) {
        ManualResetEvent waitHandle = waitHandles[guid];
        waitHandle.Close();
        waitHandles.Remove(guid);
        JobEntry entry = database.GetJob(guid);
        return entry.RawData;
      } else {
        // result not yet available, if there is also no wait-handle for that result then we will never have a result and can return null
        if(!waitHandles.ContainsKey(guid)) return null;
        // otherwise we have a wait-handle and can wait for the result
        ManualResetEvent waitHandle = waitHandles[guid];
        // wait
        if(waitHandle.WaitOne(timeout, true)) {
          // ok got the result in within the wait time => close and remove the wait-hande and return the result
          waitHandle.Close();
          waitHandles.Remove(guid);
          JobEntry entry = database.GetJob(guid);
          return entry.RawData;
        } else {
          // no result yet, check for which jobs we waited too long and requeue those jobs
          database.RestartExpiredActiveJobs();
          return null;
        }
      }
    }

    internal JobState JobState(Guid guid) {
      return database.GetJob(guid).Status;
    }
  }
}
