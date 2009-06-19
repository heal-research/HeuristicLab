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
    private const int RESULT_POLLING_TIMEOUT = 5;

    private IGridServer server;
    private object waitingQueueLock = new object();
    private Queue<AsyncGridResult> waitingJobs = new Queue<AsyncGridResult>();
    private object runningQueueLock = new object();
    private Queue<AsyncGridResult> runningJobs = new Queue<AsyncGridResult>();
    private AutoResetEvent runningWaitHandle = new AutoResetEvent(false);
    private AutoResetEvent waitingWaitHandle = new AutoResetEvent(false);


    public JobManager(IGridServer server) {
      this.server = server;
      Thread starterThread = new Thread(StartEngines);
      Thread resultsGatheringThread = new Thread(GetResults);
      starterThread.Start();
      resultsGatheringThread.Start();
    }

    public void Reset() {
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


    public void StartEngines() {
      try {
        while (true) {
          AsyncGridResult job = null;
          lock (waitingQueueLock) {
            if (waitingJobs.Count > 0) job = waitingJobs.Dequeue();
          }
          if (job == null) waitingWaitHandle.WaitOne(); // no jobs waiting
          else {
            Guid currentEngineGuid = server.BeginExecuteEngine(ZipEngine(job.Engine));
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
        HeuristicLab.Tracing.Logger.Error("Exception " + e + " in JobManager.StartEngines() killed the start-engine thread\n" + e.StackTrace);
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
            byte[] zippedResult = server.TryEndExecuteEngine(job.Guid);
            if (zippedResult != null) { 
              // successful => store result
              job.ZippedResult = zippedResult;
              // notify consumer that result is ready
              job.SignalFinished();
            } else {
              // there was a problem -> check the state of the job and restart if necessary
              JobState jobState = server.JobState(job.Guid);
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
        HeuristicLab.Tracing.Logger.Error("Exception " + e + " in JobManager.GetResults() killed the results-gathering thread\n" + e.StackTrace);
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
  }
}
