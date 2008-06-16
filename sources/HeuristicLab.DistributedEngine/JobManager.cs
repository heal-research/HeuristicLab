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
using System.ServiceModel;
using HeuristicLab.Grid;
using System.Threading;
using HeuristicLab.Core;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;

namespace HeuristicLab.DistributedEngine {
  class JobManager {
    private IGridServer server;
    private string address;
    private Dictionary<Guid, ProcessingEngine> engines = new Dictionary<Guid, ProcessingEngine>();
    private Dictionary<Guid, ManualResetEvent> waithandles = new Dictionary<Guid, ManualResetEvent>();
    private Dictionary<AtomicOperation, byte[]> results = new Dictionary<AtomicOperation, byte[]>();
    private List<IOperation> erroredOperations = new List<IOperation>();
    private object connectionLock = new object();
    private object dictionaryLock = new object();

    private const int MAX_RESTARTS = 5;
    private const int MAX_CONNECTION_RETRIES = 10;
    private const int RETRY_TIMEOUT_SEC = 60;
    private const int CHECK_RESULTS_TIMEOUT = 10;

    private ChannelFactory<IGridServer> factory;

    public JobManager(string address) {
      this.address = address;
    }

    internal void Reset() {
      ResetConnection();
      lock(dictionaryLock) {
        foreach(WaitHandle wh in waithandles.Values) wh.Close();
        waithandles.Clear();
        engines.Clear();
        results.Clear();
        erroredOperations.Clear();
      }
    }

    private void ResetConnection() {
      lock(connectionLock) {
        // open a new channel
        NetTcpBinding binding = new NetTcpBinding();
        binding.MaxReceivedMessageSize = 100000000; // 100Mbytes
        binding.ReaderQuotas.MaxStringContentLength = 100000000; // also 100M chars
        binding.ReaderQuotas.MaxArrayLength = 100000000; // also 100M elements;
        binding.Security.Mode = SecurityMode.None;

        factory = new ChannelFactory<IGridServer>(binding);
        server = factory.CreateChannel(new EndpointAddress(address));
      }
    }

    public WaitHandle BeginExecuteOperation(IScope globalScope, AtomicOperation operation) {
      ProcessingEngine engine = new ProcessingEngine(globalScope, operation);
      byte[] zippedEngine = ZipEngine(engine);
      Guid currentEngineGuid = Guid.Empty;
      bool success = false;
      int retryCount = 0;
      do {
        try {
          lock(connectionLock) {
            currentEngineGuid = server.BeginExecuteEngine(zippedEngine);
          }
          success = true;
        } catch(TimeoutException timeoutException) {
          if(retryCount++ >= MAX_CONNECTION_RETRIES) {
            throw new ApplicationException("Maximal number of connection attempts reached. There is a problem with the connection to the grid-server.", timeoutException);
          }
          Thread.Sleep(TimeSpan.FromSeconds(RETRY_TIMEOUT_SEC));
        } catch(CommunicationException communicationException) {
          if(retryCount++ >= MAX_CONNECTION_RETRIES) {
            throw new ApplicationException("Maximal number of connection attempts reached. There is a problem with the connection to the grid-server.", communicationException);
          }
          ResetConnection();
          Thread.Sleep(TimeSpan.FromSeconds(RETRY_TIMEOUT_SEC));
        }
      } while(!success);
      lock(dictionaryLock) {
        engines[currentEngineGuid] = engine;
        waithandles[currentEngineGuid] = new ManualResetEvent(false);
      }
      ThreadPool.QueueUserWorkItem(new WaitCallback(TryGetResult), currentEngineGuid);
      return waithandles[currentEngineGuid];
    }

    private byte[] ZipEngine(ProcessingEngine engine) {
      MemoryStream memStream = new MemoryStream();
      GZipStream stream = new GZipStream(memStream, CompressionMode.Compress, true);
      PersistenceManager.Save(engine, stream);
      stream.Close();
      byte[] zippedEngine = memStream.ToArray();
      memStream.Close();
      return zippedEngine;
    }

    public ProcessingEngine EndExecuteOperation(AtomicOperation operation) {
      if(erroredOperations.Contains(operation)) {
        erroredOperations.Remove(operation);
        throw new ApplicationException("Maximal number of job restarts reached. There is a problem with the connection to the grid-server.");
      } else {
        byte[] zippedResult = null;
        lock(dictionaryLock) {
          zippedResult = results[operation];
          results.Remove(operation);
        }
        // restore the engine 
        using(GZipStream stream = new GZipStream(new MemoryStream(zippedResult), CompressionMode.Decompress)) {
          return (ProcessingEngine)PersistenceManager.Load(stream);
        }
      }
    }

    private void TryGetResult(object state) {
      Guid engineGuid = (Guid)state;
      int restartCounter = 0;
      do {
        Thread.Sleep(TimeSpan.FromSeconds(CHECK_RESULTS_TIMEOUT));
        byte[] zippedResult = TryEndExecuteEngine(server, engineGuid);
        if(zippedResult != null) { // successful
          lock(dictionaryLock) {
            // store result
            results[engines[engineGuid].InitialOperation] = zippedResult;
            // clean up and signal the wait handle then return
            engines.Remove(engineGuid);
            waithandles[engineGuid].Set();
            waithandles.Remove(engineGuid);
          }
          return;
        } else {
          // there was a problem -> check the state of the job and restart if necessary
          JobState jobState = TryGetJobState(server, engineGuid);
          if(jobState == JobState.Unkown) {
            TryRestartJob(engineGuid);
            restartCounter++;
          }
        }
      } while(restartCounter < MAX_RESTARTS);
      lock(dictionaryLock) {
        // the job was never finished and restarting didn't help -> stop trying to execute the job and
        // save the faulted operation in a list to throw an exception when EndExecuteEngine is called.
        erroredOperations.Add(engines[engineGuid].InitialOperation);
        // clean up and signal the wait handle
        engines.Remove(engineGuid);
        waithandles[engineGuid].Set();
        waithandles.Remove(engineGuid);
      }
    }

    private void TryRestartJob(Guid engineGuid) {
      // restart job
      ProcessingEngine engine;
      lock(dictionaryLock) {
        engine = engines[engineGuid];
      }
      byte[] zippedEngine = ZipEngine(engine);
      int retries = 0;
      do {
        try {
          lock(connectionLock) {
            server.BeginExecuteEngine(zippedEngine);
          }
          return;
        } catch(TimeoutException timeoutException) {
          retries++;
          Thread.Sleep(TimeSpan.FromSeconds(RETRY_TIMEOUT_SEC));
        } catch(CommunicationException communicationException) {
          ResetConnection();
          retries++;
          Thread.Sleep(TimeSpan.FromSeconds(RETRY_TIMEOUT_SEC));
        }
      } while(retries < MAX_CONNECTION_RETRIES);
    }

    private byte[] TryEndExecuteEngine(IGridServer server, Guid engineGuid) {
      int retries = 0;
      do {
        try {
          lock(connectionLock) {
            byte[] zippedResult = server.TryEndExecuteEngine(engineGuid, 100);
            return zippedResult;
          }
        } catch(TimeoutException timeoutException) {
          retries++;
          Thread.Sleep(TimeSpan.FromSeconds(RETRY_TIMEOUT_SEC));
        } catch(CommunicationException communicationException) {
          ResetConnection();
          retries++;
          Thread.Sleep(TimeSpan.FromSeconds(RETRY_TIMEOUT_SEC));
        }
      } while(retries < MAX_CONNECTION_RETRIES);
      return null;
    }

    private JobState TryGetJobState(IGridServer server, Guid engineGuid) {
      // check if the server is still working on the job
      int retries = 0;
      do {
        try {
          lock(connectionLock) {
            JobState jobState = server.JobState(engineGuid);
            return jobState;
          }
        } catch(TimeoutException timeoutException) {
          retries++;
          Thread.Sleep(TimeSpan.FromSeconds(RETRY_TIMEOUT_SEC));
        } catch(CommunicationException communicationException) {
          ResetConnection();
          retries++;
          Thread.Sleep(TimeSpan.FromSeconds(RETRY_TIMEOUT_SEC));
        }
      } while(retries < MAX_CONNECTION_RETRIES);
      return JobState.Unkown;
      
    }
  }
}
