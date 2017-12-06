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
using System.Linq;
using System.Threading;
using HeuristicLab.Clients.Hive;
using HeuristicLab.Clients.Hive.Jobs;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.DataAnalysis;

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

      var allTasks = HiveServiceLocator.Instance.CallHiveService(s => s.GetLightweightJobTasksWithoutStateLog(ParentJob.Id));
      var totalJobCount = allTasks.Count;
      var optimizers = new List<IOptimizer>();
      var finishedCount = -1;
      using (var downloader = new TaskDownloader(allTasks.Select(x => x.Id))) {
        downloader.StartAsync();

        while (!downloader.IsFinished || finishedCount < totalJobCount) {
          if (finishedCount != downloader.FinishedCount) {
            finishedCount = downloader.FinishedCount;
            log.LogMessage(string.Format("Downloading/deserializing tasks... ({0}/{1} finished)", finishedCount, totalJobCount));
          }

          Thread.Sleep(500);

          if (downloader.IsFaulted) {
            throw downloader.Exception;
          }
        }

        IDictionary<Guid, HiveTask> allHiveTasks = downloader.Results;
        log.LogMessage("Building hive job tree...");
        var parentTasks = allHiveTasks.Values.Where(x => !x.Task.ParentTaskId.HasValue);

        foreach (var parentTask in parentTasks) {
          BuildHiveJobTree(parentTask, allTasks, allHiveTasks);

          var optimizerTask = parentTask.ItemTask as OptimizerTask;

          if (optimizerTask != null) {
            optimizers.Add(optimizerTask.Item);
          }
        }
      }
      if (!optimizers.Any()) return;
      IStorableContent storable;
      if (optimizers.Count > 1) {
        var experiment = new Experiment();
        experiment.Optimizers.AddRange(optimizers);
        storable = experiment;
      } else {
        var optimizer = optimizers.First();
        storable = optimizer as IStorableContent;
      }
      if (storable != null) {
        // remove duplicate datasets
        log.LogMessage("Removing duplicate datasets...");
        DatasetUtil.RemoveDuplicateDatasets(storable);

        log.LogMessage(string.Format("Save job as {0}", RootLocation));
        ContentManager.Save(storable, RootLocation, true);
      } else {
        log.LogMessage(string.Format("Could not save job, content is not storable."));
      }
    }

    private static void BuildHiveJobTree(HiveTask parentHiveTask, IEnumerable<LightweightTask> allTasks, IDictionary<Guid, HiveTask> allHiveTasks) {
      IEnumerable<LightweightTask> childTasks = from job in allTasks
                                                where job.ParentTaskId.HasValue && job.ParentTaskId.Value == parentHiveTask.Task.Id
                                                orderby job.DateCreated ascending
                                                select job;
      foreach (LightweightTask task in childTasks) {
        HiveTask childHiveTask = allHiveTasks[task.Id];
        BuildHiveJobTree(childHiveTask, allTasks, allHiveTasks);
        parentHiveTask.AddChildHiveTask(childHiveTask);
      }
    }
  }
}
