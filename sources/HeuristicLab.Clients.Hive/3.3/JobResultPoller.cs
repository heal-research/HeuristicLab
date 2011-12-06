#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using HeuristicLab.Common;

namespace HeuristicLab.Clients.Hive {
  public class JobResultPoller {
    private bool stopRequested { get; set; }
    private AutoResetEvent waitHandle;
    private Thread thread;

    private Guid jobId;
    public Guid JobId {
      get { return jobId; }
      set { jobId = value; }
    }

    private TimeSpan interval;
    public TimeSpan Interval {
      get { return interval; }
      set { interval = value; }
    }

    private bool isPolling;
    public bool IsPolling {
      get { return isPolling; }
      set {
        if (isPolling != value) {
          isPolling = value;
          OnIsPollingChanged();
        }
      }
    }

    private bool autoResumeOnException = false;
    public bool AutoResumeOnException {
      get { return autoResumeOnException; }
      set { autoResumeOnException = value; }
    }


    public JobResultPoller(Guid jobId, TimeSpan interval) {
      this.isPolling = false;
      this.jobId = jobId;
      this.interval = interval;
    }

    public void Start() {
      IsPolling = true;
      stopRequested = false;
      thread = new Thread(RunPolling);
      thread.Start();
    }

    public void Stop() {
      stopRequested = true;
      waitHandle.Set();
      IsPolling = false;
      thread = null;
    }

    private void RunPolling() {
      while (true) {
        IsPolling = true;
        try {
          waitHandle = new AutoResetEvent(false);
          while (!stopRequested) {
            OnPollingStarted();
            FetchJobResults();
            OnPollingFinished();
            waitHandle.WaitOne(Interval);
          }
          waitHandle.Close();
        }
        catch (Exception e) {
          OnExceptionOccured(e);
        }
        finally {
          IsPolling = false;
        }
        if (stopRequested) return;
        if (!autoResumeOnException) return;
      }
    }

    public IEnumerable<LightweightTask> FetchJobResults() {
      return HiveServiceLocator.Instance.CallHiveService(service => {
        var responses = new List<LightweightTask>();
        responses.AddRange(service.GetLightweightJobTasks(jobId));
        OnJobResultsReceived(responses);
        return responses;
      });
    }

    public event EventHandler<EventArgs<IEnumerable<LightweightTask>>> JobResultsReceived;
    private void OnJobResultsReceived(IEnumerable<LightweightTask> lightweightJobs) {
      var handler = JobResultsReceived;
      if (handler != null) handler(this, new EventArgs<IEnumerable<LightweightTask>>(lightweightJobs));
    }

    public event EventHandler<EventArgs<Exception>> ExceptionOccured;
    private void OnExceptionOccured(Exception e) {
      var handler = ExceptionOccured;
      if (handler != null) handler(this, new EventArgs<Exception>(e));
    }

    public event EventHandler IsPollingChanged;
    private void OnIsPollingChanged() {
      var handler = IsPollingChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler PollingStarted;
    private void OnPollingStarted() {
      var handler = PollingStarted;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler PollingFinished;
    private void OnPollingFinished() {
      var handler = PollingFinished;
      if (handler != null) handler(this, EventArgs.Empty);
    }
  }
}
