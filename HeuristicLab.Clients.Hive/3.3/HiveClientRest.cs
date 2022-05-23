#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using HEAL.Hive.RestClient.HiveRestClient;
using HeuristicLab.Clients.Hive.Util;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;
using TS = System.Threading.Tasks;

namespace HeuristicLab.Clients.Hive {
  [Item("HiveClientRest", "Hive client.")]
  public sealed class HiveClientRest : IContent {
    private static HiveClientRest instance;
    public static HiveClientRest Instance {
      get {
        if (instance == null) instance = new HiveClientRest();
        return instance;
      }
    }

    #region Properties
    private List<Plugin> onlinePlugins;
    public List<Plugin> OnlinePlugins {
      get { return onlinePlugins; }
      set { onlinePlugins = value; }
    }

    private List<Plugin> alreadyUploadedPlugins;
    public List<Plugin> AlreadyUploadedPlugins {
      get { return alreadyUploadedPlugins; }
      set { alreadyUploadedPlugins = value; }
    }
    #endregion

    private HiveClientRest() { }

    #region Upload Job
    private Semaphore taskUploadSemaphore = new Semaphore(Settings.Default.MaxParallelUploads, Settings.Default.MaxParallelUploads);
    private static object pluginLocker = new object();
    private readonly HiveRestClient client = new HiveRestClient(Settings.Default.NewHiveEndpoint, new HttpClient());
    public Guid UploadJob(RefreshableJob refreshableJob, CancellationToken cancellationToken) {
      try {
        refreshableJob.IsProgressing = true;
        refreshableJob.Progress.Start("Connecting to server...", ProgressMode.Indeterminate);

        foreach (OptimizerHiveTask hiveJob in refreshableJob.HiveTasks.OfType<OptimizerHiveTask>()) {
          hiveJob.SetIndexInParentOptimizerList(null);
        }

        // upload Job
        refreshableJob.Progress.Message = "Uploading Job...";
        refreshableJob.Job.Id = client.HiveJobPost(DtoConverterUtil.convertToNewDto(refreshableJob.Job)).Id;
        cancellationToken.ThrowIfCancellationRequested();

        // upload plugins
        refreshableJob.Progress.Message = "Uploading plugins...";
        this.OnlinePlugins = client.PluginGet().Select(x => DtoConverterUtil.convertFromNewDto(x)).ToList();
        this.AlreadyUploadedPlugins = new List<Plugin>();
        Plugin configFilePlugin = UploadConfigurationFile(onlinePlugins);
        this.alreadyUploadedPlugins.Add(configFilePlugin);
        cancellationToken.ThrowIfCancellationRequested();

        // upload tasks
        refreshableJob.Progress.Message = "Uploading tasks...";

        var tasks = new List<TS.Task>();
        foreach (HiveTask hiveTask in refreshableJob.HiveTasks) {
          var task = TS.Task.Factory.StartNew((hj) => {
            UploadTaskWithChildren((HiveTask)hj, null, configFilePlugin.Id, refreshableJob.Job.Id, refreshableJob.Log, cancellationToken);
          }, hiveTask);
          task.ContinueWith((x) => refreshableJob.Log.LogException(x.Exception), TaskContinuationOptions.OnlyOnFaulted);
          tasks.Add(task);
        }
        TS.Task.WaitAll(tasks.ToArray());
      } finally {
        refreshableJob.Job.Modified = false;
        refreshableJob.IsProgressing = false;
        refreshableJob.Progress.Finish();
      }
      return (refreshableJob.Job != null) ? refreshableJob.Job.Id : Guid.Empty;
    }

    /// <summary>
    /// Uploads the local configuration file as plugin
    /// </summary>
    private Plugin UploadConfigurationFile(List<Plugin> onlinePlugins) {
      string exeFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Settings.Default.HLBinaryName);
      string configFileName = Path.GetFileName(ConfigurationManager.OpenExeConfiguration(exeFilePath).FilePath);
      string configFilePath = ConfigurationManager.OpenExeConfiguration(exeFilePath).FilePath;
      byte[] hash;

      byte[] data = File.ReadAllBytes(configFilePath);
      using (SHA1 sha1 = SHA1.Create()) {
        hash = sha1.ComputeHash(data);
      }

      Plugin configPlugin = new Plugin() { Name = "Configuration", Version = new Version(), Hash = hash };
      PluginData configFile = new PluginData() { FileName = configFileName, Data = data };

      IEnumerable<Plugin> onlineConfig = onlinePlugins.Where(p => p.Hash.SequenceEqual(hash));

      if (onlineConfig.Count() > 0) {
        return onlineConfig.First();
      } else {
        var pluginInfo = client.PluginPost(DtoConverterUtil.convertToNewDto(configPlugin));
        configPlugin.Id = pluginInfo.Id;
        configFile.PluginId = pluginInfo.Id;
        client.PluginDataPost(DtoConverterUtil.convertToNewDto(configFile));
        return configPlugin;
      }
    }

    /// <summary>
    /// Uploads the given task and all its child-jobs while setting the proper parentJobId values for the childs
    /// </summary>
    /// <param name="parentHiveTask">shall be null if its the root task</param>
    private void UploadTaskWithChildren(HiveTask hiveTask, HiveTask parentHiveTask, Guid configPluginId, Guid jobId, ILog log, CancellationToken cancellationToken) {
      taskUploadSemaphore.WaitOne();
      bool semaphoreReleased = false;
      try {
        cancellationToken.ThrowIfCancellationRequested();
        TaskData taskData;
        List<IPluginDescription> plugins;

        if (hiveTask.ItemTask.ComputeInParallel) {
          hiveTask.Task.IsParentTask = true;
          hiveTask.Task.FinishWhenChildJobsFinished = true;
          taskData = hiveTask.GetAsTaskData(true, out plugins);
        } else {
          hiveTask.Task.IsParentTask = false;
          hiveTask.Task.FinishWhenChildJobsFinished = false;
          taskData = hiveTask.GetAsTaskData(false, out plugins);
        }
        cancellationToken.ThrowIfCancellationRequested();

        TryAndRepeat(() => {
          if (!cancellationToken.IsCancellationRequested) {
            lock (pluginLocker) {
              hiveTask.Task.PluginsNeededIds = PluginUtilRest.GetPluginDependencies(client, OnlinePlugins, AlreadyUploadedPlugins, plugins);
             }
          }
        }, Settings.Default.MaxRepeatServiceCalls, "Failed to upload plugins");
        cancellationToken.ThrowIfCancellationRequested();
        hiveTask.Task.PluginsNeededIds.Add(configPluginId);
        hiveTask.Task.JobId = jobId;

        log.LogMessage(string.Format("Uploading task ({0} kb, {1} objects)", taskData.Data.Count() / 1024, hiveTask.ItemTask.GetObjectGraphObjects().Count()));
        TryAndRepeat(() => {
          if (!cancellationToken.IsCancellationRequested) {
            if (parentHiveTask != null) {
              hiveTask.Task.ParentTaskId = parentHiveTask.Task.Id;
              var taskInfo = client.HiveTaskPost(DtoConverterUtil.convertToNewDto(hiveTask.Task));
              client.HiveTaskPlugins(taskInfo.Id, hiveTask.Task.PluginsNeededIds);
              taskData.TaskId = taskInfo.Id;
              client.HiveTaskDataPost(DtoConverterUtil.convertToNewDto(taskData));
            } else {
              var taskInfo = client.HiveTaskPost(DtoConverterUtil.convertToNewDto(hiveTask.Task));
              client.HiveTaskPlugins(taskInfo.Id, hiveTask.Task.PluginsNeededIds);
              taskData.TaskId = taskInfo.Id;
              client.HiveTaskDataPost(DtoConverterUtil.convertToNewDto(taskData));
            }
          }
        }, Settings.Default.MaxRepeatServiceCalls, "Failed to add task", log);
        cancellationToken.ThrowIfCancellationRequested();

        var tasks = new List<TS.Task>();
        foreach (HiveTask child in hiveTask.ChildHiveTasks) {
          var task = TS.Task.Factory.StartNew((tuple) => {
            var arguments = (Tuple<HiveTask, HiveTask>)tuple;
            UploadTaskWithChildren(arguments.Item1, arguments.Item2, configPluginId, jobId, log, cancellationToken);
          }, new Tuple<HiveTask, HiveTask>(child, hiveTask));
          task.ContinueWith((x) => log.LogException(x.Exception), TaskContinuationOptions.OnlyOnFaulted);
          tasks.Add(task);
        }
        taskUploadSemaphore.Release(); semaphoreReleased = true; // the semaphore has to be release before waitall!
        TS.Task.WaitAll(tasks.ToArray());
      } finally {
        if (!semaphoreReleased) taskUploadSemaphore.Release();
      }
    }
    #endregion

    public static void TryAndRepeat(Action action, int repetitions, string errorMessage, ILog log = null) {
      while (true) {
        try { action(); return; } catch (Exception e) {
          if (repetitions == 0) throw new HiveException(errorMessage, e);
          if (log != null) log.LogMessage(string.Format("{0}: {1} - will try again!", errorMessage, e.ToString()));
          repetitions--;
        }
      }
    }

  }
}