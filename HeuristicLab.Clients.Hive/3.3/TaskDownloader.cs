#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using HeuristicLab.Common;

namespace HeuristicLab.Clients.Hive {
  public class TaskDownloader : IDisposable {
    private IEnumerable<Guid> taskIds;
    private ConcurrentTaskDownloader<ItemTask> taskDownloader;
    private IDictionary<Guid, HiveTask> results;
    private bool exceptionOccured = false;
    private Exception currentException;
    private ReaderWriterLockSlim resultsLock = new ReaderWriterLockSlim();

    public bool IsFinished {
      get {
        try {
          resultsLock.EnterReadLock();
          return results.Count == taskIds.Count();
        }
        finally { resultsLock.ExitReadLock(); }
      }
    }

    public bool IsFaulted {
      get {
        return exceptionOccured;
      }
    }

    public Exception Exception {
      get {
        return currentException;
      }
    }

    public int FinishedCount {
      get {
        try {
          resultsLock.EnterReadLock();
          return results.Count;
        }
        finally { resultsLock.ExitReadLock(); }
      }
    }

    public IDictionary<Guid, HiveTask> Results {
      get {
        try {
          resultsLock.EnterReadLock();
          return results;
        }
        finally { resultsLock.ExitReadLock(); }
      }
    }

    public TaskDownloader(IEnumerable<Guid> jobIds) {
      taskIds = jobIds;
      taskDownloader = new ConcurrentTaskDownloader<ItemTask>(Settings.Default.MaxParallelDownloads, Settings.Default.MaxParallelDownloads);
      taskDownloader.ExceptionOccured += new EventHandler<EventArgs<Exception>>(taskDownloader_ExceptionOccured);
      results = new Dictionary<Guid, HiveTask>();
    }

    public void StartAsync() {
      foreach (Guid taskId in taskIds) {
        taskDownloader.DownloadTaskDataAndTask(taskId,
          (localTask, itemTask) => {
            if (localTask != null && itemTask != null) {
              HiveTask hiveTask = itemTask.CreateHiveTask();
              hiveTask.Task = localTask;
              try {
                resultsLock.EnterWriteLock();
                results.Add(localTask.Id, hiveTask);
              }
              finally { resultsLock.ExitWriteLock(); }
            }
          });
      }
    }

    private void taskDownloader_ExceptionOccured(object sender, EventArgs<Exception> e) {
      OnExceptionOccured(e.Value);
    }

    public event EventHandler<EventArgs<Exception>> ExceptionOccured;
    private void OnExceptionOccured(Exception exception) {
      exceptionOccured = true;
      currentException = exception;
      var handler = ExceptionOccured;
      if (handler != null) handler(this, new EventArgs<Exception>(exception));
    }

    #region IDisposable Members
    public void Dispose() {
      taskDownloader.ExceptionOccured -= new EventHandler<EventArgs<Exception>>(taskDownloader_ExceptionOccured);
      resultsLock.Dispose();
      taskDownloader.Dispose();
    }
    #endregion
  }
}
