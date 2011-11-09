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
using System.Threading;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Hive;

namespace HeuristicLab.Clients.Hive {
  /// <summary>
  /// Downloads and deserializes jobs. It avoids too many jobs beeing downloaded or deserialized at the same time to avoid memory problems
  /// </summary>
  public class ConcurrentTaskDownloader<T> where T : class, ITask {
    private bool abort = false;
    // use semaphore to ensure only few concurrenct connections and few SerializedJob objects in memory
    private Semaphore downloadSemaphore;
    private Semaphore deserializeSemaphore;

    public ConcurrentTaskDownloader(int concurrentDownloads, int concurrentDeserializations) {
      downloadSemaphore = new Semaphore(concurrentDownloads, concurrentDownloads);
      deserializeSemaphore = new Semaphore(concurrentDeserializations, concurrentDeserializations);
      TaskScheduler.UnobservedTaskException += new EventHandler<UnobservedTaskExceptionEventArgs>(TaskScheduler_UnobservedTaskException);
    }

    public void DownloadTask(Task job, Action<Task, T> onFinishedAction) {
      Task<T> task = Task<TaskData>.Factory.StartNew((x) => DownloadTask(x), job.Id)
                                     .ContinueWith((x) => DeserializeTask(x.Result));
      task.ContinueWith((x) => OnTaskFinished(job, x, onFinishedAction), TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion);
      task.ContinueWith((x) => OnTaskFailed(job, x, onFinishedAction), TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted);
    }

    private void OnTaskFinished(Task job, Task<T> task, Action<Task, T> onFinishedAction) {
      onFinishedAction(job, task.Result);
    }
    private void OnTaskFailed(Task job, Task<T> task, Action<Task, T> onFinishedAction) {
      task.Exception.Flatten().Handle((e) => { return true; });
      OnExceptionOccured(task.Exception.Flatten());
      onFinishedAction(job, null);
    }

    protected TaskData DownloadTask(object taskId) {
      downloadSemaphore.WaitOne();
      deserializeSemaphore.WaitOne();
      TaskData result;
      try {
        if (abort) return null;
        result = ServiceLocator.Instance.CallHiveService(s => s.GetTaskData((Guid)taskId));
      }
      finally {
        downloadSemaphore.Release();
      }
      return result;
    }

    protected T DeserializeTask(TaskData taskData) {
      try {
        if (abort || taskData == null) return null;
        Task task = ServiceLocator.Instance.CallHiveService(s => s.GetTask(taskData.TaskId));
        if (task == null) return null;
        var deserializedJob = PersistenceUtil.Deserialize<T>(taskData.Data);
        taskData.Data = null; // reduce memory consumption.
        return deserializedJob;
      }
      finally {
        deserializeSemaphore.Release();
      }
    }

    private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e) {
      e.SetObserved(); // avoid crash of process because task crashes. first exception found is handled in Results property
      OnExceptionOccured(new HiveException("Unobserved Exception in ConcurrentTaskDownloader", e.Exception));
    }

    public event EventHandler<EventArgs<Exception>> ExceptionOccured;
    private void OnExceptionOccured(Exception exception) {
      var handler = ExceptionOccured;
      if (handler != null) handler(this, new EventArgs<Exception>(exception));
    }
  }
}
