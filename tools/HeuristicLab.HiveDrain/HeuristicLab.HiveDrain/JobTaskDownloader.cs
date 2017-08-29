#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;
using System.Threading;
using HeuristicLab.Clients.Hive;
using HeuristicLab.Clients.Hive.Jobs;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.HiveDrain {
  /// <summary>
  /// downloads all finished tasks for a job
  /// </summary>
  public class JobTaskDownloader {
    public String RootLocation { get; set; }
    public Job ParentJob { get; set; }
    private ILog log;

    private static ConcurrentTaskDownloader<ItemTask> downloader =
        new ConcurrentTaskDownloader<ItemTask>(HeuristicLabHiveDrainApplication.MaxParallelDownloads, HeuristicLabHiveDrainApplication.MaxParallelDownloads);

    private static int jobCount = 0;
    private static bool endReached = false;
    private ManualResetEvent allJobsFinished = new ManualResetEvent(false);

    private Semaphore limitSemaphore = null;

    static JobTaskDownloader() {
      downloader.ExceptionOccured += new EventHandler<HeuristicLab.Common.EventArgs<Exception>>(downloader_ExceptionOccured);
    }

    static void downloader_ExceptionOccured(object sender, HeuristicLab.Common.EventArgs<Exception> e) {
      HiveDrainMainWindow.Log.LogMessage(DateTime.Now.ToShortTimeString() + " ### Exception occured: " + e.Value.ToString());
    }

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="path">root path for this job</param>
    /// <param name="parentJob">parent job</param>
    public JobTaskDownloader(string path, Job parentJob, Semaphore sem, ILog log) {
      RootLocation = path;
      ParentJob = parentJob;
      limitSemaphore = sem;
      this.log = log;
    }

    /// <summary>
    /// start downloading all finished tasks for the parentjob
    /// </summary>
    public void Start() {
      string taskPath;

      IEnumerable<LightweightTask> allTasks;
      allTasks = HiveServiceLocator.Instance.CallHiveService(s =>
          s.GetLightweightJobTasksWithoutStateLog(ParentJob.Id));

      foreach (var lightTask in allTasks) {
        if (lightTask.State == TaskState.Finished) {
          if (!CheckIfTaskDownloaded(lightTask.Id, out taskPath)) {
            AddDownloaderTask(lightTask.Id, taskPath);
            log.LogMessage(String.Format("   Getting Id {0}: {1}", lightTask.Id, DateTime.Now.ToShortTimeString()));
          } else
            log.LogMessage(String.Format("   {0} => already downloaded", lightTask.Id));
        } else
          log.LogMessage(String.Format("   {0} => ignored ({1})", lightTask.Id, lightTask.State.ToString()));
      }
      endReached = true;
      if (jobCount == 0)
        allJobsFinished.Set();

      allJobsFinished.WaitOne();

      GC.Collect();
      log.LogMessage(String.Format("All tasks for job {0} finished", ParentJob.Name));
    }

    /// <summary>
    /// adds a task with state finished to the downloader
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="taskPath"></param>
    private void AddDownloaderTask(Guid taskId, string taskPath) {
      //wait for free slot
      limitSemaphore.WaitOne();

      Interlocked.Increment(ref jobCount);
      downloader.DownloadTaskDataAndTask(taskId, (task, itemTask) => {


        log.LogMessage(String.Format("\"{0}\" - [{1}]: {2} finished", ParentJob.Name, task.Id, itemTask.Name));


        //start serialize job
        if (itemTask is OptimizerTask) {
          OptimizerTask optimizerTask = itemTask as OptimizerTask;

          //add task to serializer queue
          TaskSerializer.Serialize(new SerializerTask() {
            Content = optimizerTask.Item as IStorableContent,
            FilePath = taskPath,
            OnSaved = () => {
              log.LogMessage(String.Format("\"{0}\" - [{1}]: {2} saved", ParentJob.Name, task.Id, itemTask.Name));
              limitSemaphore.Release();
            }
          });
        } else {
          throw new InvalidOperationException(
              String.Format("Unsupported task type {0}", itemTask.GetType().Name));
        }

        //this job has finished downloading
        Interlocked.Decrement(ref jobCount);

        //if this was the last job
        if (jobCount == 0 && endReached)
          allJobsFinished.Set();
      });
    }

    /// <summary>
    /// check if there is a task directory which is not empty
    /// </summary>
    /// <param name="id"></param>
    /// <param name="taskPath"></param>
    /// <returns></returns>
    private bool CheckIfTaskDownloaded(Guid id, out string taskPath) {
      DirectoryInfo dirInfo = new DirectoryInfo(RootLocation);
      if (!dirInfo.Exists) {
        dirInfo.Create();
      }

      taskPath = Path.Combine(RootLocation, id.ToString() + ".hl");
      FileInfo fileInfo = new FileInfo(taskPath);

      if (fileInfo.Exists)
        return true;

      return false;
    }
  }
}
