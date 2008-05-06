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
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
  public class EngineStore : IEngineStore {
    private List<Guid> engineList;
    private Dictionary<Guid, byte[]> waitingEngines;
    private Dictionary<Guid, ManualResetEvent> waitHandles;
    private Dictionary<Guid, byte[]> results;
    private Dictionary<Guid, DateTime> resultDate;
    private object bigLock;
    private ChannelFactory<IClient> clientChannelFactory;
    public int WaitingJobs {
      get {
        return waitingEngines.Count;
      }
    }

    public int RunningJobs {
      get {
        return waitHandles.Count;
      }
    }

    public int WaitingResults {
      get {
        return results.Count;
      }
    }

    public EngineStore() {
      engineList = new List<Guid>();
      waitingEngines = new Dictionary<Guid, byte[]>();
      waitHandles = new Dictionary<Guid, ManualResetEvent>();
      results = new Dictionary<Guid, byte[]>();
      resultDate = new Dictionary<Guid, DateTime>();
      bigLock = new object();

      NetTcpBinding binding = new NetTcpBinding();
      binding.MaxReceivedMessageSize = 100000000; // 100Mbytes
      binding.ReaderQuotas.MaxStringContentLength = 100000000; // also 100M chars
      binding.ReaderQuotas.MaxArrayLength = 100000000; // also 100M elements;
      binding.Security.Mode = SecurityMode.None;

      clientChannelFactory = new ChannelFactory<IClient>(binding);
    }

    public bool TryTakeEngine(out Guid guid, out byte[] engine) {
      lock(bigLock) {
        if(engineList.Count == 0) {
          guid = Guid.Empty;
          engine = null;
          return false;
        } else {
          guid = engineList[0];
          engineList.RemoveAt(0);
          engine = waitingEngines[guid];
          waitingEngines.Remove(guid);
          return true;
        }
      }
    }

    public void StoreResult(Guid guid, byte[] result) {
      lock(bigLock) {
        // clear old results
        List<Guid> expiredResults = FindExpiredResults(DateTime.Now.AddHours(-1.0));
        foreach(Guid expiredGuid in expiredResults) {
          results.Remove(expiredGuid);
          waitHandles.Remove(expiredGuid);
          resultDate.Remove(expiredGuid);
        }
        // add the new result
        results[guid] = result;
        resultDate[guid] = DateTime.Now;
        waitHandles[guid].Set();
      }
    }

    private List<Guid> FindExpiredResults(DateTime expirationDate) {
      List<Guid> expiredResults = new List<Guid>();
      foreach(Guid guid in results.Keys) {
        if(resultDate[guid] < expirationDate) {
          expiredResults.Add(guid);
        }
      }
      return expiredResults;
    }

    internal void AddEngine(Guid guid, byte[] engine) {
      lock(bigLock) {
        engineList.Add(guid);
        waitingEngines.Add(guid, engine);
        waitHandles.Add(guid, new ManualResetEvent(false));
      }
    }

    internal byte[] GetResult(Guid guid) {
      return GetResult(guid, System.Threading.Timeout.Infinite);
    }

    internal byte[] GetResult(Guid guid, int timeout) {
      lock(bigLock) {
        // result already available
        if(results.ContainsKey(guid)) {
          // if the wait-handle for this result is still alive then close and remove it
          if(waitHandles.ContainsKey(guid)) {
            ManualResetEvent waitHandle = waitHandles[guid];
            waitHandle.Close();
            waitHandles.Remove(guid);
          }
          return results[guid];
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
            byte[] result = results[guid];
            return result;
          } else {
            // no result yet return without result
            return null;
          }
        }
      }
    }

    internal void AbortEngine(Guid guid) {
      lock(bigLock) {
        if(waitingEngines.ContainsKey(guid)) {
          byte[] engine = waitingEngines[guid];
          waitingEngines.Remove(guid);
          engineList.Remove(guid);
          waitHandles[guid].Set();
          results.Add(guid, engine);
        }
      }
    }

    internal JobState JobState(Guid guid) {
      lock(bigLock) {
        if(waitingEngines.ContainsKey(guid)) return HeuristicLab.Grid.JobState.Waiting;
        else if(waitHandles.ContainsKey(guid)) return HeuristicLab.Grid.JobState.Busy;
        else if(results.ContainsKey(guid)) return HeuristicLab.Grid.JobState.Finished;
        else return HeuristicLab.Grid.JobState.Unkown;
      }
    }
  }
}
