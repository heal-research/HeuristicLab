using System;
using System.Collections.Generic;
using HeuristicLab.Core;

namespace HeuristicLab.Clients.Hive {
  public interface IHiveClient {
    List<Plugin> AlreadyUploadedPlugins { get; set; }
    HashSet<Project> DisabledParentProjects { get; }
    HashSet<Resource> DisabledParentResources { get; }
    HiveItemCollection<RefreshableJob> Jobs { get; set; }
    List<Plugin> OnlinePlugins { get; set; }
    Dictionary<Guid, HashSet<Guid>> ProjectAncestors { get; }
    Dictionary<Guid, HashSet<Guid>> ProjectDescendants { get; }
    Dictionary<Guid, string> ProjectNames { get; }
    IItemList<Project> Projects { get; }
    Dictionary<Guid, HashSet<Guid>> ResourceAncestors { get; }
    Dictionary<Guid, HashSet<Guid>> ResourceDescendants { get; }
    Dictionary<Guid, string> ResourceNames { get; }
    IItemList<Resource> Resources { get; }

    event EventHandler HiveJobsChanged;
    event EventHandler Refreshed;
    event EventHandler Refreshing;

    void ClearHiveClient();
    IEnumerable<Resource> GetAssignedResourcesForJob(Guid jobId);
    IEnumerable<Resource> GetAssignedResourcesForProject(Guid projectId);
    IEnumerable<Project> GetAvailableProjectAncestors(Guid id);
    IEnumerable<Project> GetAvailableProjectDescendants(Guid id);
    IEnumerable<Resource> GetAvailableResourceAncestors(Guid id);
    IEnumerable<Resource> GetAvailableResourceDescendants(Guid id);
    IEnumerable<Resource> GetAvailableResourcesForProject(Guid id);
    IEnumerable<Resource> GetDisabledResourceAncestors(IEnumerable<Resource> availableResources);
    string GetProjectAncestry(Guid projectId);
    void Refresh();
    void RefreshAsync(Action<Exception> exceptionCallback);
    void RefreshProjectsAndResources();
    List<LightweightTask> GetLightweightJobTasks(Guid jobId);
    Task GetTask(Guid taskId);
    TaskData GetTaskData(Guid taskId);
  }
}