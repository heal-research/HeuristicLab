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
using System.Threading;
using HeuristicLab.Clients.Common;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Clients.Hive {
  [Item("HiveClient", "Hive client.")]
  public sealed class HiveClient : IContent, IHiveClient {
    private static HiveClient instance;
    public static HiveClient Instance {
      get {
        if (instance == null) instance = new HiveClient();
        return instance;
      }
    }

    // 1 - old WCF client
    // 2 - new REST client
    private static int _version = 1;
    private static readonly Dictionary<int, IHiveClientImplementation> _clients = new Dictionary<int, IHiveClientImplementation>();

    private static void UpdateClientVersion() {
      _version = HiveClientUtil.GetHiveVersion();
    }

    private void SetupClients() {
      _clients.Clear();
      _clients[1] = WCFHiveClient.Instance;
      _clients[2] = RestHiveClient.Instance;
    }

    private void SetupEvents() {
      foreach(var entry in _clients) {
        entry.Value.Refreshing += Value_Refreshing;
        entry.Value.Refreshed += Value_Refreshed;
        entry.Value.HiveJobsChanged += Value_HiveJobsChanged;
      }
    }

    private void Value_Refreshing(object sender, EventArgs e) {
      // only pass events of currently used version
      if (sender == _clients[_version]) {
        OnRefreshing();
      }
    }

    private void Value_Refreshed(object sender, EventArgs e) {
      // only pass events of currently used version
      if (sender == _clients[_version]) {
        OnRefreshed();
      }
    }

    private void Value_HiveJobsChanged(object sender, EventArgs e) {
      // only pass events of currently used version
      if (sender == _clients[_version]) {
        OnHiveJobsChanged();
      }
    }

    #region Properties
    public HiveItemCollection<RefreshableJob> Jobs { 
      get {
        return _clients[_version].Jobs;
      } 
      set { 
        _clients[_version].Jobs = value; 
      } 
    }
    public IItemList<Project> Projects {
      get {
        return _clients[_version].Projects;
      }
    }

    public IItemList<Resource> Resources {
      get {
        return _clients[_version].Resources;
      }
    }

    public Dictionary<Guid, HashSet<Guid>> ProjectAncestors {
      get {
        return _clients[_version].ProjectAncestors;
      }
    }

    public Dictionary<Guid, HashSet<Guid>> ProjectDescendants {
      get {
        return _clients[_version].ProjectDescendants;
      }
    }

    public Dictionary<Guid, HashSet<Guid>> ResourceAncestors {
      get {
        return _clients[_version].ResourceAncestors;
      }
    }

    public Dictionary<Guid, HashSet<Guid>> ResourceDescendants {
      get {
        return _clients[_version].ResourceDescendants;
      }
    }

    public Dictionary<Guid, string> ProjectNames {
      get {
        return _clients[_version].ProjectNames;
      }
    }

    public HashSet<Project> DisabledParentProjects {
      get {
        return _clients[_version].DisabledParentProjects;
      }
    }

    public Dictionary<Guid, string> ResourceNames {
      get {
        return _clients[_version].ResourceNames;
      }
    }

    public HashSet<Resource> DisabledParentResources {
      get {
        return _clients[_version].DisabledParentResources;
      }
    }

    public List<Plugin> OnlinePlugins {
      get {
        return _clients[_version].OnlinePlugins;
      }
      set {
        _clients[_version].AlreadyUploadedPlugins = value;
      }
    }
    public List<Plugin> AlreadyUploadedPlugins {
      get {
        return _clients[_version].AlreadyUploadedPlugins;
      }
      set {
        _clients[_version].AlreadyUploadedPlugins = value;
      }
    }
    #endregion

    private HiveClient() {
      UpdateClientVersion();
      SetupClients();
      SetupEvents();
    }

    public void ClearHiveClient() {
      UpdateClientVersion();
      _clients[_version].ClearHiveClient();
    }

    #region Refresh
    public void Refresh() {
      UpdateClientVersion();
      _clients[_version].Refresh();
    }

    public void RefreshProjectsAndResources() {
      UpdateClientVersion();
      _clients[_version].RefreshProjectsAndResources();
    }

    public void RefreshAsync(Action<Exception> exceptionCallback) {
      UpdateClientVersion();
      _clients[_version].RefreshAsync(exceptionCallback);
    }

    public IEnumerable<Project> GetAvailableProjectAncestors(Guid id) {
      UpdateClientVersion();
      return _clients[_version].GetAvailableProjectAncestors(id);
    }

    public IEnumerable<Project> GetAvailableProjectDescendants(Guid id) {
      UpdateClientVersion();
      return _clients[_version].GetAvailableProjectDescendants(id);
    }

    public IEnumerable<Resource> GetAvailableResourceAncestors(Guid id) {
      UpdateClientVersion();
      return _clients[_version].GetAvailableResourceAncestors(id);
    }

    public IEnumerable<Resource> GetAvailableResourceDescendants(Guid id) {
      UpdateClientVersion();
      return _clients[_version].GetAvailableResourceDescendants(id);
    }

    public IEnumerable<Resource> GetAvailableResourcesForProject(Guid id) {
      UpdateClientVersion();
      return _clients[_version].GetAvailableResourcesForProject(id);
    }

    public IEnumerable<Resource> GetDisabledResourceAncestors(IEnumerable<Resource> availableResources) {
      UpdateClientVersion();
      return _clients[_version].GetDisabledResourceAncestors(availableResources);
    }
    #endregion

    #region Store
    public static void Store(IHiveItem item, CancellationToken cancellationToken) {
      _clients[_version].Store(item, cancellationToken);
    }
    public static void StoreAsync(Action<Exception> exceptionCallback, IHiveItem item, CancellationToken cancellationToken) {
      _clients[_version].StoreAsync(exceptionCallback, item, cancellationToken);
    }
    #endregion

    #region Delete
    public static void Delete(IHiveItem item) {
      _clients[_version].Delete(item);
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
      UpdateClientVersion();
      _clients[_version].StartJob(exceptionCallback, refreshableJob, cancellationToken);
    }

    public static void PauseJob(RefreshableJob refreshableJob) {
      UpdateClientVersion();
      _clients[_version].PauseJob(refreshableJob);
    }

    public static void StopJob(RefreshableJob refreshableJob) {
      UpdateClientVersion();
      _clients[_version].StopJob(refreshableJob);
    }

    public static void ResumeJob(RefreshableJob refreshableJob) {
      UpdateClientVersion();
      _clients[_version].ResumeJob(refreshableJob);
    }

    public static void UpdateJob(Action<Exception> exceptionCallback, RefreshableJob refreshableJob, CancellationToken cancellationToken) {
      UpdateClientVersion();
      _clients[_version].UpdateJob(exceptionCallback, refreshableJob, cancellationToken);
    }

    public static void UpdateJob(RefreshableJob refreshableJob) {
      UpdateClientVersion();
      _clients[_version].UpdateJob(refreshableJob);
    }

    #region Download Experiment
    public static void LoadJob(RefreshableJob refreshableJob) {
      UpdateClientVersion();
      _clients[_version].LoadJob(refreshableJob);
    }

    #endregion

    public static ItemTask LoadItemJob(Guid jobId) {
      UpdateClientVersion();
      return _clients[_version].LoadItemJob(jobId);
    }

    public static void TryAndRepeat(Action action, int repetitions, string errorMessage, ILog log = null) {
      UpdateClientVersion();
      _clients[_version].TryAndRepeat(action, repetitions, errorMessage, log);
    }

    public static HiveItemCollection<JobPermission> GetJobPermissions(Guid jobId) {
      UpdateClientVersion();
      return _clients[_version].GetJobPermissions(jobId);
    }

    public string GetProjectAncestry(Guid projectId) {
      UpdateClientVersion();
      return _clients[_version].GetProjectAncestry(projectId);
    }

    public IEnumerable<Resource> GetAssignedResourcesForJob(Guid jobId) {
      UpdateClientVersion();
      return _clients[_version].GetAssignedResourcesForJob(jobId);
    }

    public IEnumerable<Resource> GetAssignedResourcesForProject(Guid jobId) {
      UpdateClientVersion();
      return _clients[_version].GetAssignedResourcesForProject(jobId);
    }

    public List<LightweightTask> GetLightweightJobTasks(Guid jobId) {
      UpdateClientVersion();
      return _clients[_version].GetLightweightJobTasks(jobId);
    }

    public Task GetTask(Guid taskId) {
      UpdateClientVersion();
      return _clients[_version].GetTask(taskId);
    }

    public TaskData GetTaskData(Guid taskId) {
      UpdateClientVersion();
      return _clients[_version].GetTaskData(taskId);
    }
  }
}