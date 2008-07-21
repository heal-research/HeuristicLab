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
using System.Diagnostics;

namespace HeuristicLab.Grid {
  public class JobManager {
    private const int MAX_RESTARTS = 5;
    private const int MAX_CONNECTION_RETRIES = 10;
    private const int RETRY_TIMEOUT_SEC = 60;
    private const int CHECK_RESULTS_TIMEOUT = 3;

    private IGridServer server;
    private string address;
    private object waitingQueueLock = new object();
    private Queue<ProcessingEngine> waitingEngines = new Queue<ProcessingEngine>();
    private object runningQueueLock = new object();
    private Queue<Guid> runningEngines = new Queue<Guid>();

    private Dictionary<Guid, ProcessingEngine> engines = new Dictionary<Guid, ProcessingEngine>();
    private Dictionary<ProcessingEngine, ManualResetEvent> waithandles = new Dictionary<ProcessingEngine, ManualResetEvent>();
    private Dictionary<AtomicOperation, byte[]> results = new Dictionary<AtomicOperation, byte[]>();
    private Dictionary<ProcessingEngine, int> restarts = new Dictionary<ProcessingEngine, int>();

    private List<IOperation> erroredOperations = new List<IOperation>();
    private object connectionLock = new object();
    private object dictionaryLock = new object();

    private AutoResetEvent runningWaitHandle = new AutoResetEvent(false);
    private AutoResetEvent waitingWaitHandle = new AutoResetEvent(false);

    private ChannelFactory<IGridServer> factory;

    public JobManager(string address) {
      this.address = address;
      Thread starterThread = new Thread(StartEngines);
      Thread resultsGatheringThread = new Thread(GetResults);
      starterThread.Start();
      resultsGatheringThread.Start();
    }

    public void Reset() {
      ResetConnection();
      lock(dictionaryLock) {
        foreach(WaitHandle wh in waithandles.Values) wh.Close();
        waithandles.Clear();
        engines.Clear();
        results.Clear();
        erroredOperations.Clear();
        runningEngines.Clear();
        waitingEngines.Clear();
        restarts.Clear();
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

    public void StartEngines() {
      try {
        while(true) {
          bool enginesWaiting = false;
          lock(waitingQueueLock) {
            enginesWaiting = waitingEngines.Count > 0;
          }
          if(enginesWaiting) {
            ProcessingEngine engine;
            lock(waitingQueueLock) {
              engine = waitingEngines.Dequeue();
            }
            int nRestarts = 0;
            lock(dictionaryLock) {
              if(restarts.ContainsKey(engine)) {
                nRestarts = restarts[engine];
                restarts[engine] = nRestarts + 1;
              } else {
                restarts[engine] = 0;
              }
            }
            if(nRestarts < MAX_RESTARTS) {
              byte[] zippedEngine = ZipEngine(engine);
              Guid currentEngineGuid = Guid.Empty;
              bool success = false;
              int retryCount = 0;
              do {
                try {
                  lock(connectionLock) {
                    currentEngineGuid = server.BeginExecuteEngine(zippedEngine);
                  }
                  lock(dictionaryLock) {
                    engines[currentEngineGuid] = engine;
                  }
                  lock(runningQueueLock) {
                    runningEngines.Enqueue(currentEngineGuid);
                  }

                  success = true;
                } catch(TimeoutException timeoutException) {
                  if(retryCount++ >= MAX_CONNECTION_RETRIES) {
                    //                  throw new ApplicationException("Maximal number of connection attempts reached. There is a problem with the connection to the grid-server.", timeoutException);
                    lock(waitingQueueLock) {
                      waitingEngines.Enqueue(engine);
                    }
                    success = true;
                  }
                  Thread.Sleep(TimeSpan.FromSeconds(RETRY_TIMEOUT_SEC));
                } catch(CommunicationException communicationException) {
                  if(retryCount++ >= MAX_CONNECTION_RETRIES) {
                    //                  throw new ApplicationException("Maximal number of connection attempts reached. There is a problem with the connection to the grid-server.", communicationException);
                    lock(waitingQueueLock) {
                      waitingEngines.Enqueue(engine);
                    }
                    success = true;
                  }
                  ResetConnection();
                  Thread.Sleep(TimeSpan.FromSeconds(RETRY_TIMEOUT_SEC));
                }
              } while(!success); // connection attempts
            } // restarts
            else {
              lock(dictionaryLock) {
                erroredOperations.Add(engine.InitialOperation);
                restarts.Remove(engine);
                Debug.Assert(!engines.ContainsValue(engine));
                //// clean up and signal the wait handle then return
                waithandles[engine].Set();
                waithandles.Remove(engine);
              }
            }
          } else {
            // no engines are waiting
            waitingWaitHandle.WaitOne();
          }
        }
      } finally {
        Debug.Assert(false);  // make sure that we are notified when this thread is stopped in debugging
      }
    }

    public void GetResults() {
      try {
        while(true) {
          Guid engineGuid = Guid.Empty;
          lock(runningQueueLock) {
            if(runningEngines.Count > 0) engineGuid = runningEngines.Dequeue();
          }

          if(engineGuid != Guid.Empty) {
            Thread.Sleep(TimeSpan.FromSeconds(CHECK_RESULTS_TIMEOUT));
            byte[] zippedResult = TryEndExecuteEngine(server, engineGuid);
            if(zippedResult != null) { // successful
              lock(dictionaryLock) {
                ProcessingEngine engine = engines[engineGuid];
                engines.Remove(engineGuid);
                restarts.Remove(engine);
                // store result
                results[engine.InitialOperation] = zippedResult;
                // clean up and signal the wait handle then return
                waithandles[engine].Set();
                waithandles.Remove(engine);
              }
            } else {
              // there was a problem -> check the state of the job and restart if necessary
              JobState jobState = TryGetJobState(server, engineGuid);
              if(jobState == JobState.Unkown) {
                lock(waitingQueueLock) {
                  ProcessingEngine engine = engines[engineGuid];
                  engines.Remove(engineGuid);
                  waitingEngines.Enqueue(engine);
                  waitingWaitHandle.Set();
                }
              } else {
                // job still active at the server 
                lock(runningQueueLock) {
                  runningEngines.Enqueue(engineGuid);
                }
              }
            }
          } else {
            // no running engines
            runningWaitHandle.WaitOne();
          }
        }
      } finally {
        Debug.Assert(false); // just to make sure that I get notified when debugging whenever this thread is killed somehow
      }
    }

    public WaitHandle BeginExecuteOperation(IScope globalScope, AtomicOperation operation) {
      ProcessingEngine engine = new ProcessingEngine(globalScope, operation);
      waithandles[engine] = new ManualResetEvent(false);
      lock(waitingQueueLock) {
        waitingEngines.Enqueue(engine);
      }
      waitingWaitHandle.Set();
      return waithandles[engine];
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

    private byte[] TryEndExecuteEngine(IGridServer server, Guid engineGuid) {
      int retries = 0;
      do {
        try {
          lock(connectionLock) {
            byte[] zippedResult = server.TryEndExecuteEngine(engineGuid, 100);
            return zippedResult;
          }
        } catch(TimeoutException) {
          retries++;
          Thread.Sleep(TimeSpan.FromSeconds(RETRY_TIMEOUT_SEC));
        } catch(CommunicationException) {
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
        } catch(TimeoutException) {
          retries++;
          Thread.Sleep(TimeSpan.FromSeconds(RETRY_TIMEOUT_SEC));
        } catch(CommunicationException) {
          ResetConnection();
          retries++;
          Thread.Sleep(TimeSpan.FromSeconds(RETRY_TIMEOUT_SEC));
        }
      } while(retries < MAX_CONNECTION_RETRIES);
      return JobState.Unkown;
    }
  }
}
