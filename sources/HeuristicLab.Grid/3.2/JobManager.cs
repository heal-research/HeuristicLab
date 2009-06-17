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

    private IGridServer server;
    private string address;
    private object waitingQueueLock = new object();
    private Queue<AsyncGridResult> waitingJobs = new Queue<AsyncGridResult>();
    private object runningQueueLock = new object();
    private Queue<AsyncGridResult> runningJobs = new Queue<AsyncGridResult>();
    private object connectionLock = new object();

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
      lock (waitingQueueLock) {
        foreach (AsyncGridResult r in waitingJobs) {
          r.WaitHandle.Close();
        }
        waitingJobs.Clear();
      }
      lock (runningQueueLock) {
        foreach (AsyncGridResult r in runningJobs) {
          r.WaitHandle.Close();
        }
        runningJobs.Clear();
      }
    }

    private void ResetConnection() {
      Trace.TraceInformation("Reset connection in JobManager");
      lock (connectionLock) {
        // open a new channel
        NetTcpBinding binding = new NetTcpBinding();
        binding.MaxReceivedMessageSize = 100000000; // 100Mbytes
        binding.ReaderQuotas.MaxStringContentLength = 100000000; // also 100M chars
        binding.ReaderQuotas.MaxArrayLength = 100000000; // also 100M elements;

        factory = new ChannelFactory<IGridServer>(binding);
        server = factory.CreateChannel(new EndpointAddress(address));
      }
    }

    public void StartEngines() {
      try {
        while (true) {
          AsyncGridResult job = null;
          lock (waitingQueueLock) {
            if (waitingJobs.Count > 0) job = waitingJobs.Dequeue();
          }
          if (job == null) waitingWaitHandle.WaitOne(); // no jobs waiting
          else {
            Guid currentEngineGuid = TryStartExecuteEngine(job.Engine);
            if (currentEngineGuid == Guid.Empty) {
              // couldn't start the job -> requeue
              if (job.Restarts < MAX_RESTARTS) {
                job.Restarts++;
                lock (waitingQueueLock) waitingJobs.Enqueue(job);
                waitingWaitHandle.Set();
              } else {
                // max restart count reached -> give up on this job and flag error
                job.Aborted = true;
                job.SignalFinished();
              }
            } else {
              // job started successfully
              job.Guid = currentEngineGuid;
              lock (runningQueueLock) {
                runningJobs.Enqueue(job);
                runningWaitHandle.Set();
              }
            }
          }
        }
      }
      catch (Exception e) {
        Trace.TraceError("Exception " + e + " in JobManager.StartEngines() killed the start-engine thread\n" + e.StackTrace);
      }
    }


    public void GetResults() {
      try {
        while (true) {
          AsyncGridResult job = null;
          lock (runningQueueLock) {
            if (runningJobs.Count > 0) job = runningJobs.Dequeue();
          }
          if (job == null) runningWaitHandle.WaitOne(); // no jobs running
          else {
            byte[] zippedResult = TryEndExecuteEngine(server, job.Guid);
            if (zippedResult != null) { 
              // successful => store result
              job.ZippedResult = zippedResult;
              // notify consumer that result is ready
              job.SignalFinished();
            } else {
              // there was a problem -> check the state of the job and restart if necessary
              JobState jobState = TryGetJobState(server, job.Guid);
              if (jobState == JobState.Unknown) {
                job.Restarts++;
                lock (waitingQueueLock) {
                  waitingJobs.Enqueue(job);
                  waitingWaitHandle.Set();
                }
              } else {
                // job still active at the server 
                lock (runningQueueLock) {
                  runningJobs.Enqueue(job);
                  runningWaitHandle.Set();
                }
                Thread.Sleep(TimeSpan.FromSeconds(RESULT_POLLING_TIMEOUT)); // sleep a while before trying to get the next result
              }
            }
          }
        }
      }
      catch (Exception e) {
        Trace.TraceError("Exception " + e + " in JobManager.GetResults() killed the results-gathering thread\n" + e.StackTrace);
      }
    }

    public AsyncGridResult BeginExecuteEngine(ProcessingEngine engine) {
      AsyncGridResult asyncResult = new AsyncGridResult(engine);
      asyncResult.Engine = engine;
      lock (waitingQueueLock) {
        waitingJobs.Enqueue(asyncResult);
      }
      waitingWaitHandle.Set();
      return asyncResult;
    }

    private byte[] ZipEngine(IEngine engine) {
      return PersistenceManager.SaveToGZip(engine);
    }

    public IEngine EndExecuteEngine(AsyncGridResult asyncResult) {
      if (asyncResult.Aborted) {
        throw new JobExecutionException("Maximal number of job restarts reached. There is a problem with the connection to the grid-server.");
      } else {
        // restore the engine 
        return (IEngine)PersistenceManager.RestoreFromGZip(asyncResult.ZippedResult);
      }
    }

    private Guid TryStartExecuteEngine(IEngine engine) {
      byte[] zippedEngine = ZipEngine(engine);
      return SavelyExecute(() => server.BeginExecuteEngine(zippedEngine));
    }

    private byte[] TryEndExecuteEngine(IGridServer server, Guid engineGuid) {
      return SavelyExecute(() => {
        byte[] zippedResult = server.TryEndExecuteEngine(engineGuid);
        return zippedResult;
      });
    }

    private JobState TryGetJobState(IGridServer server, Guid engineGuid) {
      return SavelyExecute(() => server.JobState(engineGuid));
    }

    private TResult SavelyExecute<TResult>(Func<TResult> a) {
      int retries = 0;
      do {
        try {
          lock (connectionLock) {
            return a();
          }
        }
        catch (TimeoutException) {
          retries++;
          Thread.Sleep(TimeSpan.FromSeconds(RETRY_TIMEOUT_SEC));
        }
        catch (CommunicationException) {
          ResetConnection();
          retries++;
          Thread.Sleep(TimeSpan.FromSeconds(RETRY_TIMEOUT_SEC));
        }
      } while (retries < MAX_CONNECTION_RETRIES);
      Trace.TraceWarning("Reached max connection retries");
      return default(TResult);
    }
  }
}
