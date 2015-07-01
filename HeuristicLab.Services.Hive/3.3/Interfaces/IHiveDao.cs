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
using System.Linq.Expressions;
using HeuristicLab.Services.Hive.DataAccess;
using DT = HeuristicLab.Services.Hive.DataTransfer;

namespace HeuristicLab.Services.Hive {
  public interface IHiveDao {
    #region Task Methods
    DT.Task GetTask(Guid id);
    IEnumerable<DT.Task> GetTasks(Expression<Func<Task, bool>> predicate);
    IEnumerable<DT.LightweightTask> GetLightweightTasks(Expression<Func<Task, bool>> predicate);
    IEnumerable<DT.LightweightTask> GetLightweightTasksWithoutStateLog(Expression<Func<Task, bool>> predicate);
    Guid AddTask(DT.Task dto);
    void UpdateTaskAndPlugins(DT.Task dto);
    void UpdateTaskAndStateLogs(DT.Task dto);
    void UpdateTask(DT.Task dto);
    void DeleteTask(Guid id);
    IEnumerable<TaskInfoForScheduler> GetWaitingTasks(DT.Slave slave);
    IEnumerable<DT.Task> GetParentTasks(IEnumerable<Guid> resourceIds, int count, bool finished);
    DT.Task UpdateTaskState(Guid taskId, TaskState taskState, Guid? slaveId, Guid? userId, string exception);
    #endregion

    #region TaskData Methods
    DT.TaskData GetTaskData(Guid id);
    IEnumerable<DT.TaskData> GetTaskDatas(Expression<Func<TaskData, bool>> predicate);
    Guid AddTaskData(DT.TaskData dto);
    void UpdateTaskData(DT.TaskData dto);
    void DeleteTaskData(Guid id);
    #endregion

    #region StateLog Methods
    DT.StateLog GetStateLog(Guid id);
    IEnumerable<DT.StateLog> GetStateLogs(Expression<Func<StateLog, bool>> predicate);
    Guid AddStateLog(DT.StateLog dto);
    void UpdateStateLog(DT.StateLog dto);
    void DeleteStateLog(Guid id);
    #endregion

    #region Job Methods
    DT.Job GetJob(Guid id);
    IEnumerable<DT.Job> GetJobs(Expression<Func<Job, bool>> predicate);
    IEnumerable<JobInfoForScheduler> GetJobInfoForScheduler(Expression<Func<Job, bool>> predicate);
    Guid AddJob(DT.Job dto);
    void UpdateJob(DT.Job dto);
    void DeleteJob(Guid id);
    #endregion

    #region JobPermission Methods
    DT.JobPermission GetJobPermission(Guid jobId, Guid grantedUserId);
    IEnumerable<DT.JobPermission> GetJobPermissions(Expression<Func<JobPermission, bool>> predicate);
    void AddJobPermission(DT.JobPermission dto);
    void UpdateJobPermission(DT.JobPermission dto);
    void DeleteJobPermission(Guid jobId, Guid grantedUserId);
    void SetJobPermission(Guid jobId, Guid grantedByUserId, Guid grantedUserId, Permission permission);
    #endregion

    #region Plugin Methods
    DT.Plugin GetPlugin(Guid id);
    IEnumerable<DT.Plugin> GetPlugins(Expression<Func<Plugin, bool>> predicate);
    Guid AddPlugin(DT.Plugin dto);
    void UpdatePlugin(DT.Plugin dto);
    void DeletePlugin(Guid id);
    #endregion

    #region PluginData Methods
    DT.PluginData GetPluginData(Guid id);
    IEnumerable<DT.PluginData> GetPluginDatas(Expression<Func<PluginData, bool>> predicate);
    Guid AddPluginData(DT.PluginData dto);
    void UpdatePluginData(DT.PluginData dto);
    void DeletePluginData(Guid id);
    #endregion

    #region Slave Methods
    DT.Slave GetSlave(Guid id);
    IEnumerable<DT.Slave> GetSlaves(Expression<Func<Slave, bool>> predicate);
    Guid AddSlave(DT.Slave dto);
    void UpdateSlave(DT.Slave dto);
    void DeleteSlave(Guid id);
    #endregion

    #region SlaveGroup Methods
    DT.SlaveGroup GetSlaveGroup(Guid id);
    IEnumerable<DT.SlaveGroup> GetSlaveGroups(Expression<Func<SlaveGroup, bool>> predicate);
    Guid AddSlaveGroup(DT.SlaveGroup dto);
    void UpdateSlaveGroup(DT.SlaveGroup dto);
    void DeleteSlaveGroup(Guid id);
    #endregion

    #region Resource Methods
    DT.Resource GetResource(Guid id);
    IEnumerable<DT.Resource> GetResources(Expression<Func<Resource, bool>> predicate);
    Guid AddResource(DT.Resource dto);
    void UpdateResource(DT.Resource dto);
    void DeleteResource(Guid id);
    void AssignJobToResource(Guid taskId, IEnumerable<Guid> resourceIds);
    IEnumerable<DT.Resource> GetAssignedResources(Guid jobId);
    IEnumerable<DT.Resource> GetParentResources(Guid resourceId);
    IEnumerable<DT.Resource> GetChildResources(Guid resourceId);
    IEnumerable<DT.Task> GetJobsByResourceId(Guid resourceId);
    #endregion

    #region ResourcePermission Methods
    DT.ResourcePermission GetResourcePermission(Guid resourceId, Guid grantedUserId);
    IEnumerable<DT.ResourcePermission> GetResourcePermissions(Expression<Func<ResourcePermission, bool>> predicate);
    void AddResourcePermission(DT.ResourcePermission dto);
    void UpdateResourcePermission(DT.ResourcePermission dto);
    void DeleteResourcePermission(Guid resourceId, Guid grantedUserId);
    #endregion

    #region Authorization Methods
    Permission GetPermissionForTask(Guid taskId, Guid userId);
    Permission GetPermissionForJob(Guid jobId, Guid userId);
    Guid GetJobForTask(Guid taskId);
    #endregion

    #region Lifecycle Methods
    DateTime GetLastCleanup();
    void SetLastCleanup(DateTime datetime);
    #endregion

    #region Downtime Methods
    DT.Downtime GetDowntime(Guid id);
    IEnumerable<DT.Downtime> GetDowntimes(Expression<Func<Downtime, bool>> predicate);
    Guid AddDowntime(DT.Downtime dto);
    void UpdateDowntime(DT.Downtime dto);
    void DeleteDowntime(Guid id);
    #endregion

    #region Statistics Methods
    Dictionary<Guid, int> GetWaitingTasksByUserForResources(List<Guid> resourceIds);
    Dictionary<Guid, int> GetCalculatingTasksByUserForResources(List<Guid> resourceIds);
    DT.Statistics GetStatistic(Guid id);
    IEnumerable<DT.Statistics> GetStatistics(Expression<Func<Statistics, bool>> predicate);
    Guid AddStatistics(DT.Statistics dto);
    void DeleteStatistics(Guid id);
    List<DT.UserStatistics> GetUserStatistics();
    #endregion

    #region UserPriority Methods
    IEnumerable<DT.UserPriority> GetUserPriorities(Expression<Func<UserPriority, bool>> predicate);
    void EnqueueUserPriority(DT.UserPriority userPriority);
    #endregion
  }
}
