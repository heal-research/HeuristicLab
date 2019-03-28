#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Net.Security;
using System.ServiceModel;
using HeuristicLab.Services.Hive.DataTransfer;

namespace HeuristicLab.Services.Hive.ServiceContracts {

  [ServiceContract(ProtectionLevel = ProtectionLevel.EncryptAndSign)]
  public interface IHiveService {
    #region Task Methods

    [OperationContract]
    Guid AddTask(Task task, TaskData taskData);

    [OperationContract]
    Guid AddChildTask(Guid parentTaskId, Task task, TaskData taskData);

    [OperationContract]
    Task GetTask(Guid taskId);

    [OperationContract]
    IEnumerable<LightweightTask> GetLightweightJobTasks(Guid jobId);

    [OperationContract]
    IEnumerable<LightweightTask> GetLightweightJobTasksWithoutStateLog(Guid jobId);

    [OperationContract]
    TaskData GetTaskData(Guid taskId);

    [OperationContract]
    void UpdateTask(Task taskDto);

    [OperationContract]
    void UpdateTaskData(Task taskDto, TaskData taskDataDto);

    [OperationContract]
    Task UpdateTaskState(Guid taskId, TaskState taskState, Guid? slaveId, Guid? userId, string exception);
    #endregion

    #region Task Control Methods
    [OperationContract]
    void StopTask(Guid taskId);

    [OperationContract]
    void PauseTask(Guid taskId);

    [OperationContract]
    void RestartTask(Guid taskId);
    #endregion

    #region Job Methods
    [OperationContract]
    Job GetJob(Guid id);

    [OperationContract]
    IEnumerable<Job> GetJobs();

    [OperationContract]
    IEnumerable<Job> GetJobsByProjectId(Guid projectId);

    [OperationContract]
    IEnumerable<Job> GetJobsByProjectIds(IEnumerable<Guid> projectIds);

    [OperationContract]
    Guid AddJob(Job jobDto, IEnumerable<Guid> resourceIds);

    [OperationContract]
    void UpdateJob(Job jobDto, IEnumerable<Guid> resourceIds);

    [OperationContract]
    void UpdateJobState(Guid JobId, JobState jobState);

    [OperationContract]
    void UpdateJobStates(IEnumerable<Guid> jobIds, JobState jobState);

    [OperationContract]
    IEnumerable<AssignedJobResource> GetAssignedResourcesForJob(Guid jobId);
    #endregion

    #region JobPermission Methods
    [OperationContract]
    void GrantPermission(Guid jobId, Guid grantedUserId, Permission permission);

    [OperationContract]
    void RevokePermission(Guid hiveExperimentId, Guid grantedUserId);

    [OperationContract]
    IEnumerable<JobPermission> GetJobPermissions(Guid jobId);

    // BackwardsCompatibility3.3
    #region Backwards compatible code, remove with 3.4
    [OperationContract]
    bool IsAllowedPrivileged(); // current user may execute privileged task
    #endregion
    #endregion

    #region Login Methods
    [OperationContract]
    void Hello(Slave slave);

    [OperationContract]
    void GoodBye(Guid slaveId);
    #endregion

    #region Heartbeat Methods
    [OperationContract]
    List<MessageContainer> Heartbeat(Heartbeat heartbeat);
    #endregion

    #region Plugin Methods
    [OperationContract]
    Plugin GetPlugin(Guid pluginId);

    [OperationContract]
    [FaultContract(typeof(PluginAlreadyExistsFault))]
    Guid AddPlugin(Plugin plugin, List<PluginData> pluginData);

    [OperationContract]
    IEnumerable<Plugin> GetPlugins();

    [OperationContract]
    IEnumerable<PluginData> GetPluginDatas(List<Guid> pluginIds);
    #endregion

    #region Project Methods
    [OperationContract]
    Guid AddProject(Project projectDto);

    [OperationContract]
    void UpdateProject(Project projectDto);

    [OperationContract]
    void DeleteProject(Guid projectId);

    [OperationContract]
    Project GetProject(Guid projectId);

    [OperationContract]
    IEnumerable<Project> GetProjects();

    [OperationContract]
    IEnumerable<Project> GetProjectsForAdministration();

    [OperationContract]
    IDictionary<Guid, HashSet<Guid>> GetProjectGenealogy();

    [OperationContract]
    IDictionary<Guid, string> GetProjectNames();
    #endregion

    #region ProjectPermission Methods
    [OperationContract]
    void SaveProjectPermissions(Guid projectId, List<Guid> grantedUserIds, bool reassign, bool cascading, bool reassignCascading);

    //[OperationContract]
    //void GrantProjectPermissions(Guid projectId, List<Guid> grantedUserIds, bool cascading);

    //[OperationContract]
    //void RevokeProjectPermissions(Guid projectId, List<Guid> grantedUserIds, bool cascading);

    [OperationContract]
    IEnumerable<ProjectPermission> GetProjectPermissions(Guid projectId);
    #endregion

    #region AssignedProjectResource Methods
    [OperationContract]
    void SaveProjectResourceAssignments(Guid projectId, List<Guid> resourceIds, bool reassign, bool cascading, bool reassignCascading);

    //[OperationContract]
    //void AssignProjectResources(Guid projectId, List<Guid> resourceIds, bool cascading);

    //[OperationContract]
    //void UnassignProjectResources(Guid projectId, List<Guid> resourceIds, bool cascading);

    [OperationContract]
    IEnumerable<AssignedProjectResource> GetAssignedResourcesForProject(Guid projectId);

    [OperationContract]
    IEnumerable<AssignedProjectResource> GetAssignedResourcesForProjectAdministration(Guid projectId);

    [OperationContract]
    IEnumerable<AssignedProjectResource> GetAssignedResourcesForProjectsAdministration(IEnumerable<Guid> projectIds);
    #endregion

    #region Slave Methods
    [OperationContract]
    Guid AddSlave(Slave slave);

    [OperationContract]
    Guid AddSlaveGroup(SlaveGroup slaveGroup);

    [OperationContract]
    Slave GetSlave(Guid slaveId);

    [OperationContract]
    IEnumerable<Slave> GetSlaves();

    [OperationContract]
    IEnumerable<SlaveGroup> GetSlaveGroups();

    [OperationContract]
    IEnumerable<Slave> GetSlavesForAdministration();

    [OperationContract]
    IEnumerable<SlaveGroup> GetSlaveGroupsForAdministration();

    [OperationContract]
    IDictionary<Guid, HashSet<Guid>> GetResourceGenealogy();

    [OperationContract]
    IDictionary<Guid, string> GetResourceNames();

    [OperationContract]
    void UpdateSlave(Slave slave);

    [OperationContract]
    void UpdateSlaveGroup(SlaveGroup slaveGroup);

    [OperationContract]
    void DeleteSlave(Guid slaveId);

    [OperationContract]
    void DeleteSlaveGroup(Guid slaveGroupId);

    [OperationContract]
    void AddResourceToGroup(Guid slaveGroupId, Guid resourceId);

    [OperationContract]
    void RemoveResourceFromGroup(Guid slaveGroupId, Guid resourceId);

    [OperationContract]
    Guid GetResourceId(string resourceName);

    [OperationContract]
    void TriggerEventManager(bool force);

    [OperationContract]
    int GetNewHeartbeatInterval(Guid slaveId);
    #endregion

    #region Downtime Methods
    [OperationContract]
    Guid AddDowntime(Downtime downtime);

    [OperationContract]
    void DeleteDowntime(Guid downtimeId);

    [OperationContract]
    void UpdateDowntime(Downtime downtimeDto);

    [OperationContract]
    IEnumerable<Downtime> GetDowntimesForResource(Guid resourceId);
    #endregion

    #region User Methods
    [OperationContract]
    string GetUsernameByUserId(Guid userId);

    [OperationContract]
    Guid GetUserIdByUsername(string username);

    [OperationContract]
    Dictionary<Guid, HashSet<Guid>> GetUserGroupTree();

    [OperationContract]
    bool CheckAccessToAdminAreaGranted();
    #endregion

    #region UserPriorities Methods
    [OperationContract]
    IEnumerable<UserPriority> GetUserPriorities();
    #endregion
  }
}
