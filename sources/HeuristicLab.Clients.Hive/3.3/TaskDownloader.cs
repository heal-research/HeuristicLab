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
using System.Linq;
using HeuristicLab.Clients.Hive.Jobs;
using HeuristicLab.Common;

namespace HeuristicLab.Clients.Hive {
  public class TaskDownloader {
    private IEnumerable<Guid> taskIds;
    private ConcurrentTaskDownloader<ItemTask> taskDownloader;
    private IDictionary<Guid, HiveTask> results;
    private bool exceptionOccured = false;
    private Exception currentException;

    public bool IsFinished {
      get {
        return results.Count == taskIds.Count();
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
        return results.Count;
      }
    }

    public IDictionary<Guid, HiveTask> Results {
      get {
        return results;
      }
    }

    public TaskDownloader(IEnumerable<Guid> jobIds) {
      this.taskIds = jobIds;
      this.taskDownloader = new ConcurrentTaskDownloader<ItemTask>(Settings.Default.MaxParallelDownloads, Settings.Default.MaxParallelDownloads);
      this.taskDownloader.ExceptionOccured += new EventHandler<EventArgs<Exception>>(taskDownloader_ExceptionOccured);
      this.results = new Dictionary<Guid, HiveTask>();
    }

    public void StartAsync() {
      foreach (Guid taskId in taskIds) {
        Task task = ServiceLocator.Instance.CallHiveService(s => s.GetTask(taskId));

        taskDownloader.DownloadTask(task,
          (localJob, itemJob) => {
            if (localJob != null && itemJob != null) {
              HiveTask hiveTask;
              if (itemJob is OptimizerTask) {
                hiveTask = new OptimizerHiveTask((OptimizerTask)itemJob);
              } else {
                hiveTask = new HiveTask(itemJob, true);
              }
              hiveTask.Task = localJob;
              this.results.Add(localJob.Id, hiveTask);
            }
          });
      }
    }

    private void taskDownloader_ExceptionOccured(object sender, EventArgs<Exception> e) {
      OnExceptionOccured(e.Value);
    }

    public event EventHandler<EventArgs<Exception>> ExceptionOccured;
    private void OnExceptionOccured(Exception exception) {
      this.exceptionOccured = true;
      this.currentException = exception;
      var handler = ExceptionOccured;
      if (handler != null) handler(this, new EventArgs<Exception>(exception));
    }
  }
}
