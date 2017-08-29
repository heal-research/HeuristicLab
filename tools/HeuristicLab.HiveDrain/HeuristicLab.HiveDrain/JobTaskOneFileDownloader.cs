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
using System.Threading;
using HeuristicLab.Clients.Hive;
using HeuristicLab.Clients.Hive.Jobs;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;

namespace HeuristicLab.HiveDrain {
  public class JobTaskOneFileDownloader {
    public String RootLocation { get; set; }

    public Job ParentJob { get; set; }

    private ILog log;

    private RunCollection results = new RunCollection();

    private static ConcurrentTaskDownloader<ItemTask> downloader =
        new ConcurrentTaskDownloader<ItemTask>(HeuristicLabHiveDrainApplication.MaxParallelDownloads, HeuristicLabHiveDrainApplication.MaxParallelDownloads);

    private static int jobCount = 0;

    private static bool endReached = false;

    private ManualResetEvent allJobsFinished = new ManualResetEvent(false);

    private Semaphore limitSemaphore = null;

    static JobTaskOneFileDownloader() {
      downloader.ExceptionOccured += downloader_ExceptionOccured;
    }

    static void downloader_ExceptionOccured(object sender, HeuristicLab.Common.EventArgs<Exception> e) {
      HiveDrainMainWindow.Log.LogMessage(DateTime.Now.ToShortTimeString() + " ### Exception occured: " + e.Value);
    }

    public JobTaskOneFileDownloader(string path, Job parentJob, Semaphore sem, ILog log) {
      RootLocation = path + ".hl";
      ParentJob = parentJob;
      limitSemaphore = sem;
      this.log = log;
    }

    public void Start() {
      results = new RunCollection();

      IEnumerable<LightweightTask> allTasks;
      allTasks = HiveServiceLocator.Instance.CallHiveService(s =>
          s.GetLightweightJobTasksWithoutStateLog(ParentJob.Id));

      foreach (var lightTask in allTasks) {
        if (lightTask.State == TaskState.Finished) {
          AddDownloaderTask(lightTask.Id);
          log.LogMessage(String.Format("   Getting Id {0}: {1}", lightTask.Id, DateTime.Now.ToShortTimeString()));
        } else
          log.LogMessage(String.Format("   {0} => ignored ({1})", lightTask.Id, lightTask.State.ToString()));
      }
      endReached = true;
      if (jobCount == 0)
        allJobsFinished.Set();

      allJobsFinished.WaitOne();
      log.LogMessage("Saving data to file...");
      ContentManager.Save(results, RootLocation, true);

      GC.Collect();
      log.LogMessage(String.Format("All tasks for job {0} finished", ParentJob.Name));
    }

    /// <summary>
    /// adds a task with state finished to the downloader
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="taskPath"></param>
    private void AddDownloaderTask(Guid taskId) {
      //wait for free slot
      limitSemaphore.WaitOne();

      Interlocked.Increment(ref jobCount);
      downloader.DownloadTaskDataAndTask(taskId, (task, itemTask) => {

        log.LogMessage(String.Format("\"{0}\" - [{1}]: {2} finished", ParentJob.Name, task.Id, itemTask.Name));
        if (itemTask is OptimizerTask) {
          OptimizerTask optimizerTask = itemTask as OptimizerTask;
          IOptimizer opt = (IOptimizer)optimizerTask.Item;

          lock (results) {
            results.AddRange(opt.Runs);
          }

          log.LogMessage(String.Format("\"{0}\" - [{1}]: {2} added to result collection", ParentJob.Name, task.Id, itemTask.Name));
        } else {
          log.LogMessage(String.Format("Unsupported task type {0}", itemTask.GetType().Name));
        }

        limitSemaphore.Release();
        Interlocked.Decrement(ref jobCount);

        //if this was the last job
        if (jobCount == 0 && endReached)
          allJobsFinished.Set();
      });
    }
  }
}
