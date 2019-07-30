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

    private IItemList<Project> projects;
    public IItemList<Project> Projects {
      get { return projects; }
    }

    private IItemList<Resource> resources;
    public IItemList<Resource> Resources {
      get { return resources; }
    }

    private Dictionary<Guid, HashSet<Guid>> projectAncestors;
    public Dictionary<Guid, HashSet<Guid>> ProjectAncestors {
      get { return projectAncestors; }
    }

    private Dictionary<Guid, HashSet<Guid>> projectDescendants;
    public Dictionary<Guid, HashSet<Guid>> ProjectDescendants {
      get { return projectDescendants; }
    }

    private Dictionary<Guid, HashSet<Guid>> resourceAncestors;
    public Dictionary<Guid, HashSet<Guid>> ResourceAncestors {
      get { return resourceAncestors; }
    }

    private Dictionary<Guid, HashSet<Guid>> resourceDescendants;
    public Dictionary<Guid, HashSet<Guid>> ResourceDescendants {
      get { return resourceDescendants; }
    }

    private Dictionary<Guid, string> projectNames;
    public Dictionary<Guid, string> ProjectNames {
      get { return projectNames; }
    }

    private HashSet<Project> disabledParentProjects;
    public HashSet<Project> DisabledParentProjects {
      get { return disabledParentProjects; }
    }

    private Dictionary<Guid, string> resourceNames;
    public Dictionary<Guid, string> ResourceNames {
      get { return resourceNames; }
    }

    private HashSet<Resource> disabledParentResources;
    public HashSet<Resource> DisabledParentResources {
      get { return disabledParentResources; }
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
    #endregion

    private HiveClient() { }

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
        projects = new ItemList<Project>();
        resources = new ItemList<Resource>();
        jobs = new HiveItemCollection<RefreshableJob>();
        projectNames = new Dictionary<Guid, string>();
        resourceNames = new Dictionary<Guid, string>();

        projectAncestors = new Dictionary<Guid, HashSet<Guid>>();
        projectDescendants = new Dictionary<Guid, HashSet<Guid>>();
        resourceAncestors = new Dictionary<Guid, HashSet<Guid>>();
        resourceDescendants = new Dictionary<Guid, HashSet<Guid>>();

        HiveServiceLocator.Instance.CallHiveService(service => {
          service.GetProjects().ForEach(p => projects.Add(p));
          service.GetSlaveGroups().ForEach(g => resources.Add(g));
          service.GetSlaves().ForEach(s => resources.Add(s));
          service.GetJobs().ForEach(p => jobs.Add(new RefreshableJob(p)));
          projectNames = service.GetProjectNames();
          resourceNames = service.GetResourceNames();
        });

        RefreshResourceGenealogy();
        RefreshProjectGenealogy();
        RefreshDisabledParentProjects();
        RefreshDisabledParentResources();
      } catch {
        jobs = null;
        projects = null;
        resources = null;
        throw;
      } finally {
        OnRefreshed();
      }
    }

    public void RefreshProjectsAndResources() {
      OnRefreshing();

      try {
        projects = new ItemList<Project>();
        projectNames = new Dictionary<Guid, string>();
        resources = new ItemList<Resource>();
        resourceNames = new Dictionary<Guid, string>();

        projectAncestors = new Dictionary<Guid, HashSet<Guid>>();
        projectDescendants = new Dictionary<Guid, HashSet<Guid>>();
        resourceAncestors = new Dictionary<Guid, HashSet<Guid>>();
        resourceDescendants = new Dictionary<Guid, HashSet<Guid>>();

        HiveServiceLocator.Instance.CallHiveService(service => {
          service.GetProjects().ForEach(p => projects.Add(p));
          service.GetSlaveGroups().ForEach(g => resources.Add(g));
          service.GetSlaves().ForEach(s => resources.Add(s));
          projectNames = service.GetProjectNames();
          resourceNames = service.GetResourceNames();
        });

        RefreshResourceGenealogy();
        RefreshProjectGenealogy();
        RefreshDisabledParentProjects();
        RefreshDisabledParentResources();
      } catch {
        projects = null;
        resources = null;
        throw;
      } finally {
        OnRefreshed();
      }
    }

    public void RefreshAsync(Action<Exception> exceptionCallback) {
      var call = new Func<Exception>(delegate () {
        try {
          Refresh();
        } catch (Exception ex) {
          return ex;
        }
        return null;
      });
      call.BeginInvoke(delegate (IAsyncResult result) {
        Exception ex = call.EndInvoke(result);
        if (ex != null) exceptionCallback(ex);
      }, null);
    }

    private void RefreshResourceGenealogy() {
      resourceAncestors.Clear();
      resourceDescendants.Clear();

      // fetch resource ancestor set
      HiveServiceLocator.Instance.CallHiveService(service => {
        var ra = service.GetResourceGenealogy();
        ra.Keys.ToList().ForEach(k => resourceAncestors.Add(k, new HashSet<Guid>()));
        resourceAncestors.Keys.ToList().ForEach(k => resourceAncestors[k].UnionWith(ra[k]));
      });

      // build resource descendant set
      resourceAncestors.Keys.ToList().ForEach(k => resourceDescendants.Add(k, new HashSet<Guid>()));
      foreach (var ra in resourceAncestors) {
        foreach (var ancestor in ra.Value) {
          resourceDescendants[ancestor].Add(ra.Key);
        }
      }
    }

    private void RefreshProjectGenealogy() {
      projectAncestors.Clear();
      projectDescendants.Clear();

      // fetch project ancestor list
      HiveServiceLocator.Instance.CallHiveService(service => {
        var pa = service.GetProjectGenealogy();
        pa.Keys.ToList().ForEach(k => projectAncestors.Add(k, new HashSet<Guid>()));
        projectAncestors.Keys.ToList().ForEach(k => projectAncestors[k].UnionWith(pa[k]));
      });

      // build project descendant list
      projectAncestors.Keys.ToList().ForEach(k => projectDescendants.Add(k, new HashSet<Guid>()));
      foreach (var pa in projectAncestors) {
        foreach (var ancestor in pa.Value) {
          projectDescendants[ancestor].Add(pa.Key);
        }
      }
    }

    private void RefreshDisabledParentProjects() {
      disabledParentProjects = new HashSet<Project>();

      foreach (var pid in projects
        .Where(x => x.ParentProjectId.HasValue)
        .SelectMany(x => projectAncestors[x.Id]).Distinct()
        .Where(x => !projects.Select(y => y.Id).Contains(x))) {
        var p = new Project();
        p.Id = pid;
        p.ParentProjectId = projectAncestors[pid].FirstOrDefault();
        p.Name = projectNames[pid];
        disabledParentProjects.Add(p);
      }
    }

    private void RefreshDisabledParentResources() {
      disabledParentResources = new HashSet<Resource>();

      foreach (var rid in resources
        .Where(x => x.ParentResourceId.HasValue)
        .SelectMany(x => resourceAncestors[x.Id]).Distinct()
        .Where(x => !resources.Select(y => y.Id).Contains(x))) {
        var r = new SlaveGroup();
        r.Id = rid;
        r.ParentResourceId = resourceAncestors[rid].FirstOrDefault();
        r.Name = resourceNames[rid];
        disabledParentResources.Add(r);
      }
    }

    public IEnumerable<Project> GetAvailableProjectAncestors(Guid id) {
      if (projectAncestors.ContainsKey(id)) return projects.Where(x => projectAncestors[id].Contains(x.Id));
      else return Enumerable.Empty<Project>();
    }

    public IEnumerable<Project> GetAvailableProjectDescendants(Guid id) {
      if (projectDescendants.ContainsKey(id)) return projects.Where(x => projectDescendants[id].Contains(x.Id));
      else return Enumerable.Empty<Project>();
    }

    public IEnumerable<Resource> GetAvailableResourceAncestors(Guid id) {
      if (resourceAncestors.ContainsKey(id)) return resources.Where(x => resourceAncestors[id].Contains(x.Id));
      else return Enumerable.Empty<Resource>();
    }

    public IEnumerable<Resource> GetAvailableResourceDescendants(Guid id) {
      if (resourceDescendants.ContainsKey(id)) return resources.Where(x => resourceDescendants[id].Contains(x.Id));
      else return Enumerable.Empty<Resource>();
    }

    public IEnumerable<Resource> GetAvailableResourcesForProject(Guid id) {
      var assignedProjectResources = HiveServiceLocator.Instance.CallHiveService(s => s.GetAssignedResourcesForProject(id));
      return resources.Where(x => assignedProjectResources.Select(y => y.ResourceId).Contains(x.Id));
    }

    public IEnumerable<Resource> GetDisabledResourceAncestors(IEnumerable<Resource> availableResources) {
      var missingParentIds = availableResources
        .Where(x => x.ParentResourceId.HasValue)
        .SelectMany(x => resourceAncestors[x.Id]).Distinct()
        .Where(x => !availableResources.Select(y => y.Id).Contains(x));

      return resources.OfType<SlaveGroup>().Union(disabledParentResources).Where(x => missingParentIds.Contains(x.Id));
    }
    #endregion

    #region Store
    public static void Store(IHiveItem item, CancellationToken cancellationToken) {
      if (item.Id == Guid.Empty) {
        if (item is RefreshableJob) {
          item.Id = HiveClient.Instance.UploadJob((RefreshableJob)item, cancellationToken);
        }
        if (item is JobPermission) {
          var hep = (JobPermission)item;
          hep.GrantedUserId = HiveServiceLocator.Instance.CallHiveService((s) => s.GetUserIdByUsername(hep.GrantedUserName));
          if (hep.GrantedUserId == Guid.Empty) {
            throw new ArgumentException(string.Format("The user {0} was not found.", hep.GrantedUserName));
          }
          HiveServiceLocator.Instance.CallHiveService((s) => s.GrantPermission(hep.JobId, hep.GrantedUserId, hep.Permission));
        }
        if (item is Project) {
          item.Id = HiveServiceLocator.Instance.CallHiveService(s => s.AddProject((Project)item));
        }
      } else {
        if (item is Job) {
          var job = (Job)item;
          HiveServiceLocator.Instance.CallHiveService(s => s.UpdateJob(job, job.ResourceIds));
        }
        if (item is Project)
          HiveServiceLocator.Instance.CallHiveService(s => s.UpdateProject((Project)item));
      }
    }
    public static void StoreAsync(Action<Exception> exceptionCallback, IHiveItem item, CancellationToken cancellationToken) {
      var call = new Func<Exception>(delegate () {
        try {
          Store(item, cancellationToken);
        } catch (Exception ex) {
          return ex;
        }
        return null;
      });
      call.BeginInvoke(delegate (IAsyncResult result) {
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
        HiveServiceLocator.Instance.CallHiveService(s => s.UpdateJobState(item.Id, JobState.StatisticsPending));
      if (item is RefreshableJob) {
        RefreshableJob job = (RefreshableJob)item;
        if (job.RefreshAutomatically) {
          job.StopResultPolling();
        }
        HiveServiceLocator.Instance.CallHiveService(s => s.UpdateJobState(item.Id, JobState.StatisticsPending));
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

    public static void UpdateJob(Action<Exception> exceptionCallback, RefreshableJob refreshableJob, CancellationToken cancellationToken) {
      refreshableJob.IsProgressing = true;
      refreshableJob.Progress.Message = "Saving Job...";
      HiveClient.StoreAsync(
        new Action<Exception>((Exception ex) => {
          exceptionCallback(ex);
        }), refreshableJob.Job, cancellationToken);
      refreshableJob.IsProgressing = false;
      refreshableJob.Progress.Finish();
    }

    public static void UpdateJob(RefreshableJob refreshableJob) {
      refreshableJob.IsProgressing = true;

      try {
        refreshableJob.Progress.Start("Saving Job...", ProgressMode.Indeterminate);
        HiveClient.StoreAsync(new Action<Exception>((Exception ex) => {
          throw new Exception("Update failed.", ex);
        }), refreshableJob.Job, new CancellationToken());
      } finally {
        refreshableJob.IsProgressing = false;
        refreshableJob.Progress.Finish();
      }
    }



    #region Upload Job
    private Semaphore taskUploadSemaphore = new Semaphore(Settings.Default.MaxParallelUploads, Settings.Default.MaxParallelUploads);
    private static object jobCountLocker = new object();
    private static object pluginLocker = new object();
    private Guid UploadJob(RefreshableJob refreshableJob, CancellationToken cancellationToken) {
      try {
        refreshableJob.IsProgressing = true;
        refreshableJob.Progress.Start("Connecting to server...", ProgressMode.Indeterminate);

        foreach (OptimizerHiveTask hiveJob in refreshableJob.HiveTasks.OfType<OptimizerHiveTask>()) {
          hiveJob.SetIndexInParentOptimizerList(null);
        }

        // upload Job
        refreshableJob.Progress.Message = "Uploading Job...";
        refreshableJob.Job.Id = HiveServiceLocator.Instance.CallHiveService((s) => s.AddJob(refreshableJob.Job, refreshableJob.Job.ResourceIds));
        refreshableJob.Job = HiveServiceLocator.Instance.CallHiveService((s) => s.GetJob(refreshableJob.Job.Id)); // update owner and permissions
        cancellationToken.ThrowIfCancellationRequested();

        int totalJobCount = refreshableJob.GetAllHiveTasks().Count();
        int[] jobCount = new int[1]; // use a reference type (int-array) instead of value type (int) in order to pass the value via a delegate to task-parallel-library
        cancellationToken.ThrowIfCancellationRequested();

        // upload plugins
        refreshableJob.Progress.Message = "Uploading plugins...";
        this.OnlinePlugins = HiveServiceLocator.Instance.CallHiveService((s) => s.GetPlugins());
        this.AlreadyUploadedPlugins = new List<Plugin>();
        Plugin configFilePlugin = HiveServiceLocator.Instance.CallHiveService((s) => UploadConfigurationFile(s, onlinePlugins));
        this.alreadyUploadedPlugins.Add(configFilePlugin);
        cancellationToken.ThrowIfCancellationRequested();

        // upload tasks
        refreshableJob.Progress.Message = "Uploading tasks...";
        refreshableJob.Progress.ProgressMode = ProgressMode.Determinate;
        refreshableJob.Progress.ProgressValue = 0;

        var tasks = new List<TS.Task>();
        foreach (HiveTask hiveTask in refreshableJob.HiveTasks) {
          var task = TS.Task.Factory.StartNew((hj) => {
            UploadTaskWithChildren(refreshableJob.Progress, (HiveTask)hj, null, jobCount, totalJobCount, configFilePlugin.Id, refreshableJob.Job.Id, refreshableJob.Log, cancellationToken);
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
    private void UploadTaskWithChildren(IProgress progress, HiveTask hiveTask, HiveTask parentHiveTask, int[] taskCount, int totalJobCount, Guid configPluginId, Guid jobId, ILog log, CancellationToken cancellationToken) {
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

        log.LogMessage(string.Format("Uploading task ({0} kb, {1} objects)", taskData.Data.Count() / 1024, hiveTask.ItemTask.GetObjectGraphObjects().Count()));
        TryAndRepeat(() => {
          if (!cancellationToken.IsCancellationRequested) {
            if (parentHiveTask != null) {
              hiveTask.Task.Id = HiveServiceLocator.Instance.CallHiveService((s) => s.AddChildTask(parentHiveTask.Task.Id, hiveTask.Task, taskData));
            } else {
              hiveTask.Task.Id = HiveServiceLocator.Instance.CallHiveService((s) => s.AddTask(hiveTask.Task, taskData));
            }
          }
        }, Settings.Default.MaxRepeatServiceCalls, "Failed to add task", log);
        cancellationToken.ThrowIfCancellationRequested();

        lock (jobCountLocker) {
          progress.ProgressValue = (double)taskCount[0] / totalJobCount;
          progress.Message = string.Format("Uploaded task ({0} of {1})", taskCount[0], totalJobCount);
        }

        var tasks = new List<TS.Task>();
        foreach (HiveTask child in hiveTask.ChildHiveTasks) {
          var task = TS.Task.Factory.StartNew((tuple) => {
            var arguments = (Tuple<HiveTask, HiveTask>)tuple;
            UploadTaskWithChildren(progress, arguments.Item1, arguments.Item2, taskCount, totalJobCount, configPluginId, jobId, log, cancellationToken);
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

    #region Download Experiment
    public static void LoadJob(RefreshableJob refreshableJob) {
      var hiveExperiment = refreshableJob.Job;
      refreshableJob.IsProgressing = true;
      TaskDownloader downloader = null;

      try {
        int totalJobCount = 0;
        IEnumerable<LightweightTask> allTasks;

        // fetch all task objects to create the full tree of tree of HiveTask objects
        refreshableJob.Progress.Start("Downloading list of tasks...", ProgressMode.Indeterminate);
        allTasks = HiveServiceLocator.Instance.CallHiveService(s => s.GetLightweightJobTasksWithoutStateLog(hiveExperiment.Id));
        totalJobCount = allTasks.Count();

        refreshableJob.Progress.Message = "Downloading tasks...";
        refreshableJob.Progress.ProgressMode = ProgressMode.Determinate;
        refreshableJob.Progress.ProgressValue = 0.0;
        downloader = new TaskDownloader(allTasks.Select(x => x.Id));
        downloader.StartAsync();

        while (!downloader.IsFinished) {
          refreshableJob.Progress.ProgressValue = downloader.FinishedCount / (double)totalJobCount;
          refreshableJob.Progress.Message = string.Format("Downloading/deserializing tasks... ({0}/{1} finished)", downloader.FinishedCount, totalJobCount);
          Thread.Sleep(500);

          if (downloader.IsFaulted) {
            throw downloader.Exception;
          }
        }
        IDictionary<Guid, HiveTask> allHiveTasks = downloader.Results;
        var parents = allHiveTasks.Values.Where(x => !x.Task.ParentTaskId.HasValue);

        refreshableJob.Progress.Message = "Downloading/deserializing complete. Displaying tasks...";
        refreshableJob.Progress.ProgressMode = ProgressMode.Indeterminate;

        // build child-task tree
        foreach (HiveTask hiveTask in parents) {
          BuildHiveJobTree(hiveTask, allTasks, allHiveTasks);
        }

        refreshableJob.HiveTasks = new ItemCollection<HiveTask>(parents);
        if (refreshableJob.IsFinished()) {
          refreshableJob.ExecutionState = Core.ExecutionState.Stopped;
        } else if (refreshableJob.IsPaused()) {
          refreshableJob.ExecutionState = Core.ExecutionState.Paused;
        } else {
          refreshableJob.ExecutionState = Core.ExecutionState.Started;
        }
        refreshableJob.OnLoaded();
      } finally {
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
      } catch {
        return null;
      }
    }

    /// <summary>
    /// Executes the action. If it throws an exception it is repeated until repetition-count is reached.
    /// If repetitions is -1, it is repeated infinitely.
    /// </summary>
    public static void TryAndRepeat(Action action, int repetitions, string errorMessage, ILog log = null) {
      while (true) {
        try { action(); return; } catch (Exception e) {
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

    public string GetProjectAncestry(Guid projectId) {
      if (projectId == null || projectId == Guid.Empty) return "";
      var projects = projectAncestors[projectId].Reverse().ToList();
      projects.Add(projectId);
      return string.Join(" » ", projects.Select(x => ProjectNames[x]).ToArray());
    }

    public IEnumerable<Resource> GetAssignedResourcesForJob(Guid jobId) {
      var assignedJobResource = HiveServiceLocator.Instance.CallHiveService(service => service.GetAssignedResourcesForJob(jobId));
      return Resources.Where(x => assignedJobResource.Select(y => y.ResourceId).Contains(x.Id));
    }
  }
}