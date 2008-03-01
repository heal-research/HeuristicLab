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
    private Queue<Guid> engineQueue;
    private Dictionary<Guid, byte[]> waitingEngines;
    private Dictionary<Guid, byte[]> runningEngines;
    private Dictionary<Guid, ManualResetEvent> waitHandles;
    private Dictionary<Guid, byte[]> results;
    private Dictionary<Guid, string> runningClients;
    private object bigLock;
    private ChannelFactory<IClient> clientChannelFactory;
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
      runningClients = new Dictionary<Guid, string>();
      waitHandles = new Dictionary<Guid, ManualResetEvent>();
      results = new Dictionary<Guid, byte[]>();
      bigLock = new object();

      NetTcpBinding binding = new NetTcpBinding();
      binding.MaxReceivedMessageSize = 100000000; // 100Mbytes
      binding.ReaderQuotas.MaxStringContentLength = 100000000; // also 100M chars
      binding.ReaderQuotas.MaxArrayLength = 100000000; // also 100M elements;
      binding.Security.Mode = SecurityMode.None;

      clientChannelFactory = new ChannelFactory<IClient>(binding);
    }

    public bool TryTakeEngine(string clientUrl, out Guid guid, out byte[] engine) {
      lock(bigLock) {
        if(engineQueue.Count == 0) {
          guid = Guid.Empty;
          engine = null;
          return false;
        } else {
          guid = engineQueue.Dequeue();
          engine = waitingEngines[guid];
          waitingEngines.Remove(guid);
          runningEngines[guid] = engine;
          runningClients[guid] = clientUrl;
          return true;
        }
      }
    }

    public void StoreResult(Guid guid, byte[] result) {
      lock(bigLock) {
        if(!runningEngines.ContainsKey(guid)) return; // ignore result when the engine is not known to be running

        runningEngines.Remove(guid);
        runningClients.Remove(guid);
        results[guid] = result;
        waitHandles[guid].Set();
      }
    }

    internal void AddEngine(Guid guid, byte[] engine) {
      lock(bigLock) {
        engineQueue.Enqueue(guid);
        waitingEngines.Add(guid, engine);
        waitHandles.Add(guid, new ManualResetEvent(false));
      }
    }

    internal byte[] RemoveResult(Guid guid) {
      lock(bigLock) {
        byte[] result = results[guid];
        results.Remove(guid);
        return result;
      }
    }

    internal byte[] GetResult(Guid guid) {
      return GetResult(guid, System.Threading.Timeout.Infinite);
    }
    internal byte[] GetResult(Guid guid, int timeout) {
      lock(bigLock) {
        if(waitHandles.ContainsKey(guid)) {
          ManualResetEvent waitHandle = waitHandles[guid];
          if(waitHandle.WaitOne(timeout, false)) {
            waitHandle.Close();
            waitHandles.Remove(guid);
            byte[] result = results[guid];
            results.Remove(guid);
            return result;
          } else {
            return null;
          }
        } else {
          return null;
        }
      }
    }

    internal void AbortEngine(Guid guid) {
      string clientUrl = "";
      lock(bigLock) {
        if(runningClients.ContainsKey(guid)) {
          clientUrl = runningClients[guid];
        }

        if(clientUrl != "") {
          IClient client = clientChannelFactory.CreateChannel(new EndpointAddress(clientUrl));
          client.Abort(guid);
        }
      }
    }
  }
}
