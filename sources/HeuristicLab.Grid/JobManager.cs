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
using System.Windows.Forms;
using System.Diagnostics;

namespace HeuristicLab.Grid {
  public class JobExecutionException : ApplicationException {
    public JobExecutionException(string msg) : base(msg) { }
  }

  public class JobManager {
    private const int MAX_RESTARTS = 5;
    private const int MAX_CONNECTION_RETRIES = 10;
    private const int RETRY_TIMEOUT_SEC = 60;
    private const int RESULT_POLLING_TIMEOUT = 5;

    private class Job {
      public Guid guid;
      public ProcessingEngine engine;
      public ManualResetEvent waitHandle;
      public int restarts;
    }

    private IGridServer server;
    private string address;
    private object waitingQueueLock = new object();
    private Queue<Job> waitingJobs = new Queue<Job>();
    private object runningQueueLock = new object();
    private Queue<Job> runningJobs = new Queue<Job>();
    private Dictionary<AtomicOperation, byte[]> results = new Dictionary<AtomicOperation, byte[]>();

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
        foreach(Job j in waitingJobs) {
          j.waitHandle.Close();
        }
        waitingJobs.Clear();
        foreach(Job j in runningJobs) {
          j.waitHandle.Close();
        }
        runningJobs.Clear();
        results.Clear();
        erroredOperations.Clear();
      }
    }

    private void ResetConnection() {
      Trace.TraceInformation("Reset connection in JobManager");
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
          Job job = null;
          lock(waitingQueueLock) {
            if(waitingJobs.Count > 0) job = waitingJobs.Dequeue();
          }
          if(job==null) waitingWaitHandle.WaitOne(); // no jobs waiting
          else {
            Guid currentEngineGuid = TryStartExecuteEngine(job.engine);
            if(currentEngineGuid == Guid.Empty) {
              // couldn't start the job -> requeue
              if(job.restarts < MAX_RESTARTS) {
                job.restarts++;
                lock(waitingQueueLock) waitingJobs.Enqueue(job);
                waitingWaitHandle.Set();
              } else {
                // max restart count reached -> give up on this job and flag error
                lock(dictionaryLock) {
                  erroredOperations.Add(job.engine.InitialOperation);
                  job.waitHandle.Set();
                }
              }
            } else {
              // job started successfully
              job.guid = currentEngineGuid;
              lock(runningQueueLock) {
                runningJobs.Enqueue(job);
                runningWaitHandle.Set();
              }
            }
          }
        }
      } catch(Exception e) {
        Trace.TraceError("Exception "+e+" in JobManager.StartEngines() killed the start-engine thread\n"+e.StackTrace);
      }
    }


    public void GetResults() {
      try {
        while(true) {
          Job job = null;
          lock(runningQueueLock) {
            if(runningJobs.Count > 0) job = runningJobs.Dequeue();
          }
          if(job == null) runningWaitHandle.WaitOne(); // no jobs running
          else {
            byte[] zippedResult = TryEndExecuteEngine(server, job.guid);
            if(zippedResult != null) { // successful
              lock(dictionaryLock) {
                // store result
                results[job.engine.InitialOperation] = zippedResult;
                // notify consumer that result is ready
                job.waitHandle.Set();
              }
            } else {
              // there was a problem -> check the state of the job and restart if necessary
              JobState jobState = TryGetJobState(server, job.guid);
              if(jobState == JobState.Unknown) {
                job.restarts++;
                lock(waitingQueueLock) {
                  waitingJobs.Enqueue(job);
                  waitingWaitHandle.Set();
                }
              } else {
                // job still active at the server 
                lock(runningQueueLock) {
                  runningJobs.Enqueue(job);
                  runningWaitHandle.Set();
                }
                Thread.Sleep(TimeSpan.FromSeconds(RESULT_POLLING_TIMEOUT)); // sleep a while before trying to get the next result
              }
            }
          }
        }
      } catch(Exception e) {
        Trace.TraceError("Exception " + e + " in JobManager.GetResults() killed the results-gathering thread\n"+ e.StackTrace);
      }
    }

    public WaitHandle BeginExecuteOperation(IScope globalScope, AtomicOperation operation) {
      return BeginExecuteEngine(new ProcessingEngine(globalScope, operation));
    }

    public WaitHandle BeginExecuteEngine(ProcessingEngine engine) {
      Job job = new Job();
      job.engine = engine;
      job.waitHandle = new ManualResetEvent(false);
      job.restarts = 0;
      lock(waitingQueueLock) {
        waitingJobs.Enqueue(job);
      }
      waitingWaitHandle.Set();
      return job.waitHandle;
    }

    private byte[] ZipEngine(ProcessingEngine engine) {
      return PersistenceManager.SaveToGZip(engine);
    }

    public ProcessingEngine EndExecuteOperation(AtomicOperation operation) {
      if(erroredOperations.Contains(operation)) {
        erroredOperations.Remove(operation);
        throw new JobExecutionException("Maximal number of job restarts reached. There is a problem with the connection to the grid-server.");
      } else {
        byte[] zippedResult = null;
        lock(dictionaryLock) {
          zippedResult = results[operation];
          results.Remove(operation);
        }
        // restore the engine 
        return (ProcessingEngine)PersistenceManager.RestoreFromGZip(zippedResult);
      }
    }

    private Guid TryStartExecuteEngine(ProcessingEngine engine) {
      byte[] zippedEngine = ZipEngine(engine);
      int retries = 0;
      Guid guid = Guid.Empty;
      do {
        try {
          lock(connectionLock) {
            guid = server.BeginExecuteEngine(zippedEngine);
          }
          return guid;
        } catch(TimeoutException) {
          retries++;
          Thread.Sleep(TimeSpan.FromSeconds(RETRY_TIMEOUT_SEC));
        } catch(CommunicationException) {
          ResetConnection();
          retries++;
          Thread.Sleep(TimeSpan.FromSeconds(RETRY_TIMEOUT_SEC));
        }
      } while(retries < MAX_CONNECTION_RETRIES);
      Trace.TraceWarning("Reached max connection retries in TryStartExecuteEngine");
      return Guid.Empty;
    }

    private byte[] TryEndExecuteEngine(IGridServer server, Guid engineGuid) {
      int retries = 0;
      do {
        try {
          lock(connectionLock) {
            byte[] zippedResult = server.TryEndExecuteEngine(engineGuid);
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
      Trace.TraceWarning("Reached max connection retries in TryEndExecuteEngine");
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
      Trace.TraceWarning("Reached max connection retries in TryGetJobState");
      return JobState.Unknown;
    }
  }
}
