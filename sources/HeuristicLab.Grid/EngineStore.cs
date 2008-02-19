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
using System.Threading;
using System.ServiceModel;

namespace HeuristicLab.Grid {
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext=false)]
  public class EngineStore : IEngineStore {
    private Queue<Guid> engineQueue;
    private Dictionary<Guid, byte[]> waitingEngines;
    private Dictionary<Guid, byte[]> runningEngines;
    private Dictionary<Guid, byte[]> results;
    private object bigLock;

    private event EventHandler ResultRecieved;

    public int WaitingJobs {
      get {
        return waitingEngines.Count;
      }
    }

    public int RunningJobs {
      get {
        return runningEngines.Count;
      }
    }

    public int WaitingResults {
      get {
        return results.Count;
      }
    }

    public EngineStore() {
      engineQueue = new Queue<Guid>();
      waitingEngines = new Dictionary<Guid, byte[]>();
      runningEngines = new Dictionary<Guid, byte[]>();
      results = new Dictionary<Guid, byte[]>();
      bigLock = new object();
    }

    public bool TryTakeEngine(out Guid guid, out byte[] engine) {
      lock (bigLock) {
        if (engineQueue.Count == 0) {
          guid = Guid.Empty;
          engine = null;
          return false;
        } else {
          guid = engineQueue.Dequeue();
          engine = waitingEngines[guid];
          waitingEngines.Remove(guid);
          runningEngines[guid] = engine;
          return true;
        }
      }
    }

    public void StoreResult(Guid guid, byte[] result) {
      lock (bigLock) {
        if (!runningEngines.ContainsKey(guid)) return; // ignore result when the engine is not known to be running

        runningEngines.Remove(guid);
        results[guid] = result;
        OnResultRecieved(guid);
      }
    }

    internal void AddEngine(Guid guid, byte[] engine) {
      lock (bigLock) {
        engineQueue.Enqueue(guid);
        waitingEngines.Add(guid, engine);
      }
    }

    internal byte[] RemoveResult(Guid guid) {
      lock (bigLock) {
        byte[] result = results[guid];
        results.Remove(guid);
        return result;
      }
    }

    internal byte[] GetResult(Guid guid) {
      ManualResetEvent waitHandle = new ManualResetEvent(false);
      lock (bigLock) {
        if (results.ContainsKey(guid)) {
          byte[] result = results[guid];
          results.Remove(guid);
          return result;
        } else {
          ResultRecieved += delegate(object source, EventArgs args) {
            ResultRecievedEventArgs resultArgs = (ResultRecievedEventArgs)args;
            if (resultArgs.resultGuid == guid) {
              waitHandle.Set();
            }
          };
        }
      }

      waitHandle.WaitOne();
      waitHandle.Close();

      lock (bigLock) {
        byte[] result = results[guid];
        results.Remove(guid);
        return result;
      }
    }

    private void OnResultRecieved(Guid guid) {
      ResultRecievedEventArgs args = new ResultRecievedEventArgs();
      args.resultGuid = guid;
      if (ResultRecieved != null) {
        ResultRecieved(this, args);
      }
    }

    private class ResultRecievedEventArgs : EventArgs {
      public Guid resultGuid;
    }
  }
}
