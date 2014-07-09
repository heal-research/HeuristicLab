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
using System.Linq.Expressions;
using HeuristicLab.Services.Hive.Common.DataTransfer;
using DT = HeuristicLab.Services.Hive.Common.DataTransfer;

namespace HeuristicLab.Services.Hive.DataAccess {
  public interface IHiveDao {
    #region Job Methods
    DT.Job GetJob(Guid id);
    IEnumerable<DT.Job> GetJobs(Expression<Func<Job, bool>> predicate);
    Guid AddJob(DT.Job dto);
    void UpdateJob(DT.Job dto);
    void DeleteJob(Guid id);
    IEnumerable<DT.Job> GetWaitingJobs(DT.Slave slave, int count);
    IEnumerable<DT.Job> GetParentJobs(IEnumerable<Guid> resourceIds, int count, bool finished);
    DT.Job UpdateJobState(Guid jobId, JobState jobState, Guid? slaveId, Guid? userId, string exception);
    #endregion

    #region JobData Methods
    DT.JobData GetJobData(Guid id);
    IEnumerable<DT.JobData> GetJobDatas(Expression<Func<JobData, bool>> predicate);
    Guid AddJobData(DT.JobData dto);
    void UpdateJobData(DT.JobData dto);
    void DeleteJobData(Guid id);
    #endregion

    #region StateLog Methods
    DT.StateLog GetStateLog(Guid id);
    IEnumerable<DT.StateLog> GetStateLogs(Expression<Func<StateLog, bool>> predicate);
    Guid AddStateLog(DT.StateLog dto);
    void UpdateStateLog(DT.StateLog dto);
    void DeleteStateLog(Guid id);
    #endregion

    #region HiveExperiment Methods
    DT.HiveExperiment GetHiveExperiment(Guid id);
    IEnumerable<DT.HiveExperiment> GetHiveExperiments(Expression<Func<HiveExperiment, bool>> predicate);
    Guid AddHiveExperiment(DT.HiveExperiment dto);
    void UpdateHiveExperiment(DT.HiveExperiment dto);
    void DeleteHiveExperiment(Guid id);
    #endregion

    #region HiveExperimentPermission Methods
    DT.HiveExperimentPermission GetHiveExperimentPermission(Guid hiveExperimentId, Guid grantedUserId);
    IEnumerable<DT.HiveExperimentPermission> GetHiveExperimentPermissions(Expression<Func<HiveExperimentPermission, bool>> predicate);
    void AddHiveExperimentPermission(DT.HiveExperimentPermission dto);
    void UpdateHiveExperimentPermission(DT.HiveExperimentPermission dto);
    void DeleteHiveExperimentPermission(Guid hiveExperimentId, Guid grantedUserId);
    void SetHiveExperimentPermission(Guid hiveExperimentId, Guid grantedByUserId, Guid grantedUserId, Permission permission);
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

    #region Calendar Methods
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
    void AssignJobToResource(Guid jobId, Guid resourceId);
    IEnumerable<DT.Resource> GetAssignedResources(Guid jobId);
    IEnumerable<DT.Resource> GetParentResources(Guid resourceId);
    IEnumerable<DT.Resource> GetChildResources(Guid resourceId);
    IEnumerable<DT.Job> GetJobsByResourceId(Guid resourceId);
    #endregion

    #region Authorization Methods
    Permission GetPermissionForJob(Guid jobId, Guid userId);
    Permission GetPermissionForExperiment(Guid experimentId, Guid userId);
    Guid GetExperimentForJob(Guid jobId);
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
    DT.Statistics GetStatistic(Guid id);
    IEnumerable<DT.Statistics> GetStatistics(Expression<Func<Statistics, bool>> predicate);
    Guid AddStatistics(DT.Statistics dto);
    void DeleteStatistics(Guid id);
    List<DT.UserStatistics> GetUserStatistics();
    #endregion
  }
}
