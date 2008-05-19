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
    private object connectionLock = new object();
    private object dictionaryLock = new object();

    private const int MAX_RESTARTS = 5;
    private const int MAX_CONNECTION_RETRIES = 10;
    private const int RETRY_TIMEOUT_SEC = 10;
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
        lock(connectionLock) {
          try {
            currentEngineGuid = server.BeginExecuteEngine(zippedEngine);
            success = true;
          } catch(TimeoutException timeoutException) {
            if(retryCount < MAX_CONNECTION_RETRIES) {
              Thread.Sleep(TimeSpan.FromSeconds(RETRY_TIMEOUT_SEC));
              retryCount++;
            } else {
              throw new ApplicationException("Max retries reached.", timeoutException);
            }
          } catch(CommunicationException communicationException) {
            ResetConnection();
            // wait some time and try again (limit with maximal retries if retry count reached throw exception -> engine can decide to stop execution)
            if(retryCount < MAX_CONNECTION_RETRIES) {
              Thread.Sleep(TimeSpan.FromSeconds(RETRY_TIMEOUT_SEC));
              retryCount++;
            } else {
              throw new ApplicationException("Max retries reached.", communicationException);
            }
          }
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

    public IScope EndExecuteOperation(AtomicOperation operation) {
      byte[] zippedResult = null;
      lock(dictionaryLock) {
        zippedResult = results[operation];
        results.Remove(operation);
      }
      // restore the engine 
      using(GZipStream stream = new GZipStream(new MemoryStream(zippedResult), CompressionMode.Decompress)) {
        ProcessingEngine resultEngine = (ProcessingEngine)PersistenceManager.Load(stream);
        return resultEngine.InitialOperation.Scope;
      }      
    }

    private void TryGetResult(object state) {
      Guid engineGuid = (Guid)state;
      int restartCounter = 0;
      do {
        Thread.Sleep(TimeSpan.FromSeconds(CHECK_RESULTS_TIMEOUT));
        byte[] zippedResult = null;
        lock(connectionLock) {
          bool success = false;
          int retries = 0;
          do {
            try {
              zippedResult = server.TryEndExecuteEngine(engineGuid, 100);
              success = true;
            } catch(TimeoutException timeoutException) {
              success = false;
              retries++;
              Thread.Sleep(TimeSpan.FromSeconds(RETRY_TIMEOUT_SEC));
            } catch(CommunicationException communicationException) {
              ResetConnection();
              success = false;
              retries++;
              Thread.Sleep(TimeSpan.FromSeconds(RETRY_TIMEOUT_SEC));
            }

          } while(!success && retries < MAX_CONNECTION_RETRIES);
        }
        if(zippedResult != null) {
          lock(dictionaryLock) {
            // store result
            results[engines[engineGuid].InitialOperation] = zippedResult;

            // signal the wait handle and clean up then return
            engines.Remove(engineGuid);
            waithandles[engineGuid].Set();
            waithandles.Remove(engineGuid);
          }
          return;
        } else {
          // check if the server is still working on the job
          bool success = false;
          int retries = 0;
          JobState jobState = JobState.Unkown;
          do {
            try {
              lock(connectionLock) {
                jobState = server.JobState(engineGuid);
              }
              success = true;
            } catch(TimeoutException timeoutException) {
              retries++;
              success = false;
              Thread.Sleep(TimeSpan.FromSeconds(RETRY_TIMEOUT_SEC));
            } catch(CommunicationException communicationException) {
              ResetConnection();
              retries++;
              success = false;
              Thread.Sleep(TimeSpan.FromSeconds(RETRY_TIMEOUT_SEC));
            }
          } while(!success && retries < MAX_CONNECTION_RETRIES);
          if(jobState == JobState.Unkown) {
            // restart job
            ProcessingEngine engine;
            lock(dictionaryLock) {
              engine = engines[engineGuid];
            }
            byte[] zippedEngine = ZipEngine(engine);
            success = false;
            retries = 0;
            do {
              try {
                lock(connectionLock) {
                  server.BeginExecuteEngine(zippedEngine);
                }
                success = true;
              } catch(TimeoutException timeoutException) {
                success = false;
                retries++;
                Thread.Sleep(TimeSpan.FromSeconds(RETRY_TIMEOUT_SEC));
              } catch(CommunicationException communicationException) {
                ResetConnection();
                success = false;
                retries++;
                Thread.Sleep(TimeSpan.FromSeconds(RETRY_TIMEOUT_SEC));
              }
            } while(!success && retries < MAX_CONNECTION_RETRIES);
            restartCounter++;
          }
        }

        // when we reach a maximum amount of restarts => signal the wait-handle and set a flag that there was a problem
        if(restartCounter > MAX_RESTARTS) {
          throw new ApplicationException("Maximum number of job restarts reached.");
        }
      } while(true);
    }
  }
}
