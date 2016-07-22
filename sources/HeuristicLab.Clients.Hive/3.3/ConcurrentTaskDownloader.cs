#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class ConcurrentTaskDownloader<T> : IDisposable where T : class, ITask {
    private bool abort = false;
    // use semaphore to ensure only few concurrenct connections and few SerializedJob objects in memory
    private Semaphore downloadSemaphore;
    private Semaphore deserializeSemaphore;

    public ConcurrentTaskDownloader(int concurrentDownloads, int concurrentDeserializations) {
      downloadSemaphore = new Semaphore(concurrentDownloads, concurrentDownloads);
      deserializeSemaphore = new Semaphore(concurrentDeserializations, concurrentDeserializations);
    }

    public void DownloadTaskData(Task t, Action<Task, T> onFinishedAction) {
      Task<Tuple<Task, T>> task = Task<Tuple<Task, TaskData>>.Factory.StartNew(DownloadTaskData, t)
                                     .ContinueWith((y) => DeserializeTask(y.Result));

      task.ContinueWith((x) => OnTaskFinished(x, onFinishedAction), TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion);
      task.ContinueWith((x) => OnTaskFailed(x), TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted);
    }

    public void DownloadTaskDataAndTask(Guid taskId, Action<Task, T> onFinishedAction) {
      Task<Tuple<Task, T>> task = Task<Task>.Factory.StartNew(DownloadTask, taskId)
                                     .ContinueWith((x) => DownloadTaskData(x.Result))
                                     .ContinueWith((y) => DeserializeTask(y.Result));

      task.ContinueWith((x) => OnTaskFinished(x, onFinishedAction), TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion);
      task.ContinueWith((x) => OnTaskFailed(x), TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted);
    }

    private void OnTaskFinished(Task<Tuple<Task, T>> task, Action<Task, T> onFinishedAction) {
      onFinishedAction(task.Result.Item1, task.Result.Item2);
    }
    private void OnTaskFailed(Task<Tuple<Task, T>> task) {
      task.Exception.Flatten().Handle((e) => { return true; });
      OnExceptionOccured(task.Exception.Flatten());
    }

    private Task DownloadTask(object taskId) {
      Task t = null;
      HiveClient.TryAndRepeat(() => {
        t = HiveServiceLocator.Instance.CallHiveService(s => s.GetTask((Guid)taskId));
      }, Settings.Default.MaxRepeatServiceCalls, "Failed to download task.");
      return t;
    }

    protected Tuple<Task, TaskData> DownloadTaskData(object taskId) {
      return DownloadTaskData((Task)taskId);
    }

    protected Tuple<Task, TaskData> DownloadTaskData(Task task) {
      downloadSemaphore.WaitOne();
      TaskData result = null;
      try {
        if (abort) return null;
        HiveClient.TryAndRepeat(() => {
          result = HiveServiceLocator.Instance.CallHiveService(s => s.GetTaskData(task.Id));
        }, Settings.Default.MaxRepeatServiceCalls, "Failed to download task data.");
      }
      finally {
        downloadSemaphore.Release();
      }
      return new Tuple<Task, TaskData>(task, result);
    }

    protected Tuple<Task, T> DeserializeTask(Tuple<Task, TaskData> taskData) {
      deserializeSemaphore.WaitOne();
      try {
        if (abort || taskData.Item2 == null || taskData.Item1 == null) return null;
        var deserializedJob = PersistenceUtil.Deserialize<T>(taskData.Item2.Data);
        taskData.Item2.Data = null; // reduce memory consumption.
        return new Tuple<Task, T>(taskData.Item1, deserializedJob);
      }
      finally {
        deserializeSemaphore.Release();
      }
    }

    public event EventHandler<EventArgs<Exception>> ExceptionOccured;
    private void OnExceptionOccured(Exception exception) {
      var handler = ExceptionOccured;
      if (handler != null) handler(this, new EventArgs<Exception>(exception));
    }

    #region IDisposable Members
    public void Dispose() {
      deserializeSemaphore.Dispose();
      downloadSemaphore.Dispose();
    }
    #endregion
  }
}
