#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;
using TS = System.Threading.Tasks;

namespace HeuristicLab.Clients.Hive {
  [Item("HiveClient", "Hive client.")]
  public sealed class HiveClient : IContent {
    private static HiveClient instance;
    public static HiveClient Instance {
      get {
        if (instance == null) instance = new HiveClient();
        return instance;
      }
    }

    #region Properties
    private HiveItemCollection<RefreshableJob> jobs;
    public HiveItemCollection<RefreshableJob> Jobs {
      get { return jobs; }
      set {
        if (value != jobs) {
          jobs = value;
          OnHiveJobsChanged();
        }
      }
    }

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

    private bool isAllowedPrivileged;
    public bool IsAllowedPrivileged {
      get { return isAllowedPrivileged; }
      set { isAllowedPrivileged = value; }
    }
    #endregion

    private HiveClient() {
      //this will never be deregistered
      TaskScheduler.UnobservedTaskException += new EventHandler<UnobservedTaskExceptionEventArgs>(TaskScheduler_UnobservedTaskException);
    }

    private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e) {
      e.SetObserved(); // avoid crash of process because task crashes. first exception found is handled in Results property
      throw new HiveException("Unobserved Exception in ConcurrentTaskDownloader", e.Exception);
    }

    public void ClearHiveClient() {
      Jobs.ClearWithoutHiveDeletion();
      foreach (var j in Jobs) {
        if (j.RefreshAutomatically) {
          j.RefreshAutomatically = false; // stop result polling
        }
        j.Dispose();
      }
      Jobs = null;

      if (onlinePlugins != null)
        onlinePlugins.Clear();
      if (alreadyUploadedPlugins != null)
        alreadyUploadedPlugins.Clear();
    }

    #region Refresh
    public void Refresh() {
      OnRefreshing();

      try {
        IsAllowedPrivileged = HiveServiceLocator.Instance.CallHiveService((s) => s.IsAllowedPrivileged());

        jobs = new HiveItemCollection<RefreshableJob>();
        var jobsLoaded = HiveServiceLocator.Instance.CallHiveService<IEnumerable<Job>>(s => s.GetJobs());

        foreach (var j in jobsLoaded) {
          jobs.Add(new RefreshableJob(j));
        }
      }
      catch {
        jobs = null;
        throw;
      }
      finally {
        OnRefreshed();
      }
    }

    public void RefreshAsync(Action<Exception> exceptionCallback) {
      var call = new Func<Exception>(delegate() {
        try {
          Refresh();
        }
        catch (Exception ex) {
          return ex;
        }
        return null;
      });
      call.BeginInvoke(delegate(IAsyncResult result) {
        Exception ex = call.EndInvoke(result);
        if (ex != null) exceptionCallback(ex);
      }, null);
    }
    #endregion

    #region Store
    public static void Store(IHiveItem item, CancellationToken cancellationToken) {
      if (item.Id == Guid.Empty) {
        if (item is RefreshableJob) {
          HiveClient.Instance.UploadJob((RefreshableJob)item, cancellationToken);
        }
        if (item is JobPermission) {
          var hep = (JobPermission)item;
          hep.GrantedUserId = HiveServiceLocator.Instance.CallHiveService((s) => s.GetUserIdByUsername(hep.GrantedUserName));
          if (hep.GrantedUserId == Guid.Empty) {
            throw new ArgumentException(string.Format("The user {0} was not found.", hep.GrantedUserName));
          }
          HiveServiceLocator.Instance.CallHiveService((s) => s.GrantPermission(hep.JobId, hep.GrantedUserId, hep.Permission));
        }
      } else {
        if (item is Job)
          HiveServiceLocator.Instance.CallHiveService(s => s.UpdateJob((Job)item));
      }
    }
    public static void StoreAsync(Action<Exception> exceptionCallback, IHiveItem item, CancellationToken cancellationToken) {
      var call = new Func<Exception>(delegate() {
        try {
          Store(item, cancellationToken);
        }
        catch (Exception ex) {
          return ex;
        }
        return null;
      });
      call.BeginInvoke(delegate(IAsyncResult result) {
        Exception ex = call.EndInvoke(result);
        if (ex != null) exceptionCallback(ex);
      }, null);
    }
    #endregion

    #region Delete
    public static void Delete(IHiveItem item) {
      if (item.Id == Guid.Empty && item.GetType() != typeof(JobPermission))
        return;

      if (item is Job)
        HiveServiceLocator.Instance.CallHiveService(s => s.DeleteJob(item.Id));
      if (item is RefreshableJob) {
        RefreshableJob job = (RefreshableJob)item;
        if (job.RefreshAutomatically) {
          job.StopResultPolling();
        }
        HiveServiceLocator.Instance.CallHiveService(s => s.DeleteJob(item.Id));
      }
      if (item is JobPermission) {
        var hep = (JobPermission)item;
        HiveServiceLocator.Instance.CallHiveService(s => s.RevokePermission(hep.JobId, hep.GrantedUserId));
      }
      item.Id = Guid.Empty;
    }
    #endregion

    #region Events
    public event EventHandler Refreshing;
    private void OnRefreshing() {
      EventHandler handler = Refreshing;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Refreshed;
    private void OnRefreshed() {
      var handler = Refreshed;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler HiveJobsChanged;
    private void OnHiveJobsChanged() {
      var handler = HiveJobsChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    #endregion

    public static void StartJob(Action<Exception> exceptionCallback, RefreshableJob refreshableJob, CancellationToken cancellationToken) {
      HiveClient.StoreAsync(
        new Action<Exception>((Exception ex) => {
          refreshableJob.ExecutionState = ExecutionState.Prepared;
          exceptionCallback(ex);
        }), refreshableJob, cancellationToken);
      refreshableJob.ExecutionState = ExecutionState.Started;
    }

    public static void PauseJob(RefreshableJob refreshableJob) {
      HiveServiceLocator.Instance.CallHiveService(service => {
        foreach (HiveTask task in refreshableJob.GetAllHiveTasks()) {
          if (task.Task.State != TaskState.Finished && task.Task.State != TaskState.Aborted && task.Task.State != TaskState.Failed)
            service.PauseTask(task.Task.Id);
        }
      });
      refreshableJob.ExecutionState = ExecutionState.Paused;
    }

    public static void StopJob(RefreshableJob refreshableJob) {
      HiveServiceLocator.Instance.CallHiveService(service => {
        foreach (HiveTask task in refreshableJob.GetAllHiveTasks()) {
          if (task.Task.State != TaskState.Finished && task.Task.State != TaskState.Aborted && task.Task.State != TaskState.Failed)
            service.StopTask(task.Task.Id);
        }
      });
      refreshableJob.ExecutionState = ExecutionState.Stopped;
    }

    public static void ResumeJob(RefreshableJob refreshableJob) {
      HiveServiceLocator.Instance.CallHiveService(service => {
        foreach (HiveTask task in refreshableJob.GetAllHiveTasks()) {
          if (task.Task.State == TaskState.Paused) {
            service.RestartTask(task.Task.Id);
          }
        }
      });
      refreshableJob.ExecutionState = ExecutionState.Started;
    }

    #region Upload Job
    private Semaphore taskUploadSemaphore = new Semaphore(Settings.Default.MaxParallelUploads, Settings.Default.MaxParallelUploads);
    private static object jobCountLocker = new object();
    private static object pluginLocker = new object();
    private void UploadJob(RefreshableJob refreshableJob, CancellationToken cancellationToken) {
      try {
        refreshableJob.IsProgressing = true;
        refreshableJob.Progress.Start("Connecting to server...");
        IEnumerable<string> resourceNames = ToResourceNameList(refreshableJob.Job.ResourceNames);
        var resourceIds = new List<Guid>();
        foreach (var resourceName in resourceNames) {
          Guid resourceId = HiveServiceLocator.Instance.CallHiveService((s) => s.GetResourceId(resourceName));
          if (resourceId == Guid.Empty) {
            throw new ResourceNotFoundException(string.Format("Could not find the resource '{0}'", resourceName));
          }
          resourceIds.Add(resourceId);
        }

        foreach (OptimizerHiveTask hiveJob in refreshableJob.HiveTasks.OfType<OptimizerHiveTask>()) {
          hiveJob.SetIndexInParentOptimizerList(null);
        }

        // upload Job
        refreshableJob.Progress.Status = "Uploading Job...";
        refreshableJob.Job.Id = HiveServiceLocator.Instance.CallHiveService((s) => s.AddJob(refreshableJob.Job));
        bool isPrivileged = refreshableJob.Job.IsPrivileged;
        refreshableJob.Job = HiveServiceLocator.Instance.CallHiveService((s) => s.GetJob(refreshableJob.Job.Id)); // update owner and permissions
        refreshableJob.Job.IsPrivileged = isPrivileged;
        cancellationToken.ThrowIfCancellationRequested();

        int totalJobCount = refreshableJob.GetAllHiveTasks().Count();
        int[] jobCount = new int[1]; // use a reference type (int-array) instead of value type (int) in order to pass the value via a delegate to task-parallel-library
        cancellationToken.ThrowIfCancellationRequested();

        // upload plugins
        refreshableJob.Progress.Status = "Uploading plugins...";
        this.OnlinePlugins = HiveServiceLocator.Instance.CallHiveService((s) => s.GetPlugins());
        this.AlreadyUploadedPlugins = new List<Plugin>();
        Plugin configFilePlugin = HiveServiceLocator.Instance.CallHiveService((s) => UploadConfigurationFile(s, onlinePlugins));
        this.alreadyUploadedPlugins.Add(configFilePlugin);
        cancellationToken.ThrowIfCancellationRequested();

        // upload tasks
        refreshableJob.Progress.Status = "Uploading tasks...";

        var tasks = new List<TS.Task>();
        foreach (HiveTask hiveTask in refreshableJob.HiveTasks) {
          var task = TS.Task.Factory.StartNew((hj) => {
            UploadTaskWithChildren(refreshableJob.Progress, (HiveTask)hj, null, resourceIds, jobCount, totalJobCount, configFilePlugin.Id, refreshableJob.Job.Id, refreshableJob.Log, refreshableJob.Job.IsPrivileged, cancellationToken);
          }, hiveTask);
          task.ContinueWith((x) => refreshableJob.Log.LogException(x.Exception), TaskContinuationOptions.OnlyOnFaulted);
          tasks.Add(task);
        }
        TS.Task.WaitAll(tasks.ToArray());
      }
      finally {
        refreshableJob.Job.Modified = false;
        refreshableJob.IsProgressing = false;
        refreshableJob.Progress.Finish();
      }
    }

    /// <summary>
    /// Uploads the local configuration file as plugin
    /// </summary>
    private static Plugin UploadConfigurationFile(IHiveService service, List<Plugin> onlinePlugins) {
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
        configPlugin.Id = service.AddPlugin(configPlugin, new List<PluginData> { configFile });
        return configPlugin;
      }
    }

    /// <summary>
    /// Uploads the given task and all its child-jobs while setting the proper parentJobId values for the childs
    /// </summary>
    /// <param name="parentHiveTask">shall be null if its the root task</param>
    private void UploadTaskWithChildren(IProgress progress, HiveTask hiveTask, HiveTask parentHiveTask, IEnumerable<Guid> groups, int[] taskCount, int totalJobCount, Guid configPluginId, Guid jobId, ILog log, bool isPrivileged, CancellationToken cancellationToken) {
      taskUploadSemaphore.WaitOne();
      bool semaphoreReleased = false;
      try {
        cancellationToken.ThrowIfCancellationRequested();
        lock (jobCountLocker) {
          taskCount[0]++;
        }
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
              HiveServiceLocator.Instance.CallHiveService((s) => hiveTask.Task.PluginsNeededIds = PluginUtil.GetPluginDependencies(s, this.onlinePlugins, this.alreadyUploadedPlugins, plugins));
            }
          }
        }, Settings.Default.MaxRepeatServiceCalls, "Failed to upload plugins");
        cancellationToken.ThrowIfCancellationRequested();
        hiveTask.Task.PluginsNeededIds.Add(configPluginId);
        hiveTask.Task.JobId = jobId;
        hiveTask.Task.IsPrivileged = isPrivileged;

        log.LogMessage(string.Format("Uploading task ({0} kb, {1} objects)", taskData.Data.Count() / 1024, hiveTask.ItemTask.GetObjectGraphObjects().Count()));
        TryAndRepeat(() => {
          if (!cancellationToken.IsCancellationRequested) {
            if (parentHiveTask != null) {
              hiveTask.Task.Id = HiveServiceLocator.Instance.CallHiveService((s) => s.AddChildTask(parentHiveTask.Task.Id, hiveTask.Task, taskData));
            } else {
              hiveTask.Task.Id = HiveServiceLocator.Instance.CallHiveService((s) => s.AddTask(hiveTask.Task, taskData, groups.ToList()));
            }
          }
        }, Settings.Default.MaxRepeatServiceCalls, "Failed to add task", log);
        cancellationToken.ThrowIfCancellationRequested();

        lock (jobCountLocker) {
          progress.ProgressValue = (double)taskCount[0] / totalJobCount;
          progress.Status = string.Format("Uploaded task ({0} of {1})", taskCount[0], totalJobCount);
        }

        var tasks = new List<TS.Task>();
        foreach (HiveTask child in hiveTask.ChildHiveTasks) {
          var task = TS.Task.Factory.StartNew((tuple) => {
            var arguments = (Tuple<HiveTask, HiveTask>)tuple;
            UploadTaskWithChildren(progress, arguments.Item1, arguments.Item2, groups, taskCount, totalJobCount, configPluginId, jobId, log, isPrivileged, cancellationToken);
          }, new Tuple<HiveTask, HiveTask>(child, hiveTask));
          task.ContinueWith((x) => log.LogException(x.Exception), TaskContinuationOptions.OnlyOnFaulted);
          tasks.Add(task);
        }
        taskUploadSemaphore.Release(); semaphoreReleased = true; // the semaphore has to be release before waitall!
        TS.Task.WaitAll(tasks.ToArray());
      }
      finally {
        if (!semaphoreReleased) taskUploadSemaphore.Release();
      }
    }
    #endregion

    #region Download Experiment
    public static void LoadJob(RefreshableJob refreshableJob) {
      var hiveExperiment = refreshableJob.Job;
      refreshableJob.IsProgressing = true;
      TaskDownloader downloader = null;

      try {
        int totalJobCount = 0;
        IEnumerable<LightweightTask> allTasks;

        // fetch all task objects to create the full tree of tree of HiveTask objects
        refreshableJob.Progress.Start("Downloading list of tasks...");
        allTasks = HiveServiceLocator.Instance.CallHiveService(s => s.GetLightweightJobTasksWithoutStateLog(hiveExperiment.Id));
        totalJobCount = allTasks.Count();

        refreshableJob.Progress.Status = "Downloading tasks...";
        downloader = new TaskDownloader(allTasks.Select(x => x.Id));
        downloader.StartAsync();

        while (!downloader.IsFinished) {
          refreshableJob.Progress.ProgressValue = downloader.FinishedCount / (double)totalJobCount;
          refreshableJob.Progress.Status = string.Format("Downloading/deserializing tasks... ({0}/{1} finished)", downloader.FinishedCount, totalJobCount);
          Thread.Sleep(500);

          if (downloader.IsFaulted) {
            throw downloader.Exception;
          }
        }
        IDictionary<Guid, HiveTask> allHiveTasks = downloader.Results;
        var parents = allHiveTasks.Values.Where(x => !x.Task.ParentTaskId.HasValue);
        refreshableJob.Job.IsPrivileged = allHiveTasks.Any(x => x.Value.Task.IsPrivileged);

        refreshableJob.Progress.Status = "Downloading/deserializing complete. Displaying tasks...";
        // build child-task tree
        foreach (HiveTask hiveTask in parents) {
          BuildHiveJobTree(hiveTask, allTasks, allHiveTasks);
        }

        refreshableJob.HiveTasks = new ItemCollection<HiveTask>(parents);
        if (refreshableJob.IsFinished()) {
          refreshableJob.ExecutionState = Core.ExecutionState.Stopped;
        } else {
          refreshableJob.ExecutionState = Core.ExecutionState.Started;
        }
        refreshableJob.OnLoaded();
      }
      finally {
        refreshableJob.IsProgressing = false;
        refreshableJob.Progress.Finish();
        if (downloader != null) {
          downloader.Dispose();
        }
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
    #endregion

    /// <summary>
    /// Converts a string which can contain Ids separated by ';' to a enumerable
    /// </summary>
    private static IEnumerable<string> ToResourceNameList(string resourceNames) {
      if (!string.IsNullOrEmpty(resourceNames)) {
        return resourceNames.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
      } else {
        return new List<string>();
      }
    }

    public static ItemTask LoadItemJob(Guid jobId) {
      TaskData taskData = HiveServiceLocator.Instance.CallHiveService(s => s.GetTaskData(jobId));
      try {
        return PersistenceUtil.Deserialize<ItemTask>(taskData.Data);
      }
      catch {
        return null;
      }
    }

    /// <summary>
    /// Executes the action. If it throws an exception it is repeated until repetition-count is reached.
    /// If repetitions is -1, it is repeated infinitely.
    /// </summary>
    public static void TryAndRepeat(Action action, int repetitions, string errorMessage, ILog log = null) {
      while (true) {
        try { action(); return; }
        catch (Exception e) {
          if (repetitions == 0) throw new HiveException(errorMessage, e);
          if (log != null) log.LogMessage(string.Format("{0}: {1} - will try again!", errorMessage, e.ToString()));
          repetitions--;
        }
      }
    }

    public static HiveItemCollection<JobPermission> GetJobPermissions(Guid jobId) {
      return HiveServiceLocator.Instance.CallHiveService((service) => {
        IEnumerable<JobPermission> jps = service.GetJobPermissions(jobId);
        foreach (var hep in jps) {
          hep.UnmodifiedGrantedUserNameUpdate(service.GetUsernameByUserId(hep.GrantedUserId));
        }
        return new HiveItemCollection<JobPermission>(jps);
      });
    }
  }
}