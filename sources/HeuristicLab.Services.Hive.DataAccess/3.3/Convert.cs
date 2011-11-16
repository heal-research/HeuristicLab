#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Data.Linq;
using System.Linq;
using DT = HeuristicLab.Services.Hive.Common.DataTransfer;

namespace HeuristicLab.Services.Hive.DataAccess {
  public static class Convert {
    #region Job
    public static DT.Job ToDto(Job source) {
      if (source == null) return null;
      return new DT.Job {
        Id = source.JobId,
        CoresNeeded = source.CoresNeeded,
        ExecutionTime = TimeSpan.FromMilliseconds(source.ExecutionTimeMs),
        MemoryNeeded = source.MemoryNeeded,
        ParentJobId = source.ParentJobId,
        Priority = source.Priority,
        PluginsNeededIds = (source.RequiredPlugins == null ? new List<Guid>() : source.RequiredPlugins.Select(x => x.PluginId).ToList()),
        LastHeartbeat = source.LastHeartbeat,
        State = source.State,
        StateLog = (source.StateLogs == null ? new List<DT.StateLog>() : source.StateLogs.Select(x => Convert.ToDto(x)).OrderBy(x => x.DateTime).ToList()),
        IsParentJob = source.IsParentJob,
        FinishWhenChildJobsFinished = source.FinishWhenChildJobsFinished,
        Command = source.Command,
        LastJobDataUpdate = (source.JobData == null ? DateTime.MinValue : source.JobData.LastUpdate),
        HiveExperimentId = source.HiveExperimentId,
        IsPrivileged = source.IsPrivileged
      };
    }
    public static Job ToEntity(DT.Job source) {
      if (source == null) return null;
      var entity = new Job(); ToEntity(source, entity);
      return entity;
    }
    public static void ToEntity(DT.Job source, Job target) {
      if ((source != null) && (target != null)) {
        target.JobId = source.Id;
        target.CoresNeeded = source.CoresNeeded;
        target.ExecutionTimeMs = source.ExecutionTime.TotalMilliseconds;
        target.MemoryNeeded = source.MemoryNeeded;
        target.ParentJobId = source.ParentJobId;
        target.Priority = source.Priority;
        target.LastHeartbeat = source.LastHeartbeat;
        target.State = source.State;
        if (target.StateLogs == null) target.StateLogs = new EntitySet<StateLog>();
        foreach (DT.StateLog sl in source.StateLog.Where(x => x.Id == Guid.Empty)) {
          target.StateLogs.Add(Convert.ToEntity(sl));
        }
        target.IsParentJob = source.IsParentJob;
        target.FinishWhenChildJobsFinished = source.FinishWhenChildJobsFinished;
        target.Command = source.Command;
        // RequiredPlugins are added by Dao
        target.HiveExperimentId = source.HiveExperimentId;
        target.IsPrivileged = source.IsPrivileged;
      }
    }
    #endregion

    #region JobData
    public static DT.JobData ToDto(JobData source) {
      if (source == null) return null;
      return new DT.JobData { JobId = source.JobId, Data = source.Data.ToArray(), LastUpdate = source.LastUpdate };
    }
    public static JobData ToEntity(DT.JobData source) {
      if (source == null) return null;
      var entity = new JobData(); ToEntity(source, entity);
      return entity;
    }
    public static void ToEntity(DT.JobData source, JobData target) {
      if ((source != null) && (target != null)) {
        target.JobId = source.JobId; target.Data = new Binary(source.Data); target.LastUpdate = source.LastUpdate;
      }
    }
    #endregion

    #region StateLog
    public static DT.StateLog ToDto(StateLog source) {
      if (source == null) return null;
      return new DT.StateLog { Id = source.StateLogId, DateTime = source.DateTime, Exception = source.Exception, JobId = source.JobId, SlaveId = source.SlaveId, State = source.State, UserId = source.UserId };
    }
    public static StateLog ToEntity(DT.StateLog source) {
      if (source == null) return null;
      var entity = new StateLog(); ToEntity(source, entity);
      return entity;
    }
    public static void ToEntity(DT.StateLog source, StateLog target) {
      if ((source != null) && (target != null)) {
        target.StateLogId = source.Id; target.DateTime = source.DateTime; target.Exception = source.Exception; target.JobId = source.JobId; target.SlaveId = source.SlaveId; target.State = source.State; target.UserId = source.UserId;
      }
    }
    #endregion

    #region Downtimes
    public static DT.Downtime ToDto(Downtime source) {
      if (source == null) return null;
      return new DT.Downtime { Id = source.DowntimeId, AllDayEvent = source.AllDayEvent, EndDate = source.EndDate, Recurring = source.Recurring, RecurringId = source.RecurringId, ResourceId = source.ResourceId, StartDate = source.StartDate };
    }
    public static Downtime ToEntity(DT.Downtime source) {
      if (source == null) return null;
      var entity = new Downtime(); ToEntity(source, entity);
      return entity;
    }
    public static void ToEntity(DT.Downtime source, Downtime target) {
      if ((source != null) && (target != null)) {
        target.DowntimeId = source.Id; target.AllDayEvent = source.AllDayEvent; target.EndDate = source.EndDate; target.Recurring = source.Recurring; target.RecurringId = source.RecurringId; target.ResourceId = source.ResourceId; target.StartDate = source.StartDate;
      }
    }
    #endregion

    #region HiveExperiment
    public static DT.HiveExperiment ToDto(HiveExperiment source) {
      if (source == null) return null;
      return new DT.HiveExperiment { Id = source.HiveExperimentId, Description = source.Description, Name = source.Name, OwnerUserId = source.OwnerUserId, DateCreated = source.DateCreated, ResourceNames = source.ResourceIds, LastAccessed = source.LastAccessed };
    }
    public static HiveExperiment ToEntity(DT.HiveExperiment source) {
      if (source == null) return null;
      var entity = new HiveExperiment(); ToEntity(source, entity);
      return entity;
    }
    public static void ToEntity(DT.HiveExperiment source, HiveExperiment target) {
      if ((source != null) && (target != null)) {
        target.HiveExperimentId = source.Id; target.Description = source.Description; target.Name = source.Name; target.OwnerUserId = source.OwnerUserId; target.DateCreated = source.DateCreated; target.ResourceIds = source.ResourceNames; target.LastAccessed = source.LastAccessed;
      }
    }
    #endregion

    #region HiveExperimentPermission
    public static DT.HiveExperimentPermission ToDto(HiveExperimentPermission source) {
      if (source == null) return null;
      return new DT.HiveExperimentPermission { HiveExperimentId = source.HiveExperimentId, GrantedUserId = source.GrantedUserId, GrantedByUserId = source.GrantedByUserId, Permission = source.Permission };
    }
    public static HiveExperimentPermission ToEntity(DT.HiveExperimentPermission source) {
      if (source == null) return null;
      var entity = new HiveExperimentPermission(); ToEntity(source, entity);
      return entity;
    }
    public static void ToEntity(DT.HiveExperimentPermission source, HiveExperimentPermission target) {
      if ((source != null) && (target != null)) {
        target.HiveExperimentId = source.HiveExperimentId; target.GrantedUserId = source.GrantedUserId; target.GrantedByUserId = source.GrantedByUserId; target.Permission = source.Permission;
      }
    }
    #endregion

    #region Plugin
    public static DT.Plugin ToDto(Plugin source) {
      if (source == null) return null;
      return new DT.Plugin { Id = source.PluginId, Name = source.Name, Version = new Version(source.Version), UserId = source.UserId, DateCreated = source.DateCreated, Hash = source.Hash };
    }
    public static Plugin ToEntity(DT.Plugin source) {
      if (source == null) return null;
      var entity = new Plugin(); ToEntity(source, entity);
      return entity;
    }
    public static void ToEntity(DT.Plugin source, Plugin target) {
      if ((source != null) && (target != null)) {
        target.PluginId = source.Id; target.Name = source.Name; target.Version = source.Version.ToString(); target.UserId = source.UserId; target.DateCreated = source.DateCreated; target.Hash = source.Hash;
      }
    }
    #endregion

    #region PluginData
    public static DT.PluginData ToDto(PluginData source) {
      if (source == null) return null;
      return new DT.PluginData { Id = source.PluginDataId, PluginId = source.PluginId, Data = source.Data.ToArray(), FileName = source.FileName };
    }
    public static PluginData ToEntity(DT.PluginData source) {
      if (source == null) return null;
      var entity = new PluginData(); ToEntity(source, entity);
      return entity;
    }
    public static void ToEntity(DT.PluginData source, PluginData target) {
      if ((source != null) && (target != null)) {
        target.PluginDataId = source.Id; target.PluginId = source.PluginId; target.Data = new Binary(source.Data); target.FileName = source.FileName;
      }
    }
    #endregion

    #region Slave
    public static DT.Slave ToDto(Slave source) {
      if (source == null) return null;
      return new DT.Slave {
        Id = source.ResourceId,
        ParentResourceId = source.ParentResourceId,
        Cores = source.Cores,
        CpuSpeed = source.CpuSpeed,
        FreeCores = source.FreeCores,
        FreeMemory = source.FreeMemory,
        IsAllowedToCalculate = source.IsAllowedToCalculate,
        Memory = source.Memory,
        Name = source.Name,
        SlaveState = source.SlaveState,
        CpuArchitecture = source.CpuArchitecture,
        OperatingSystem = source.OperatingSystem,
        LastHeartbeat = source.LastHeartbeat,
        CpuUtilization = source.CpuUtilization
      };
    }
    public static Slave ToEntity(DT.Slave source) {
      if (source == null) return null;
      var entity = new Slave(); ToEntity(source, entity);
      return entity;
    }
    public static void ToEntity(DT.Slave source, Slave target) {
      if ((source != null) && (target != null)) {
        target.ResourceId = source.Id;
        target.ParentResourceId = source.ParentResourceId;
        target.Cores = source.Cores;
        target.CpuSpeed = source.CpuSpeed;
        target.FreeCores = source.FreeCores;
        target.FreeMemory = source.FreeMemory;
        target.IsAllowedToCalculate = source.IsAllowedToCalculate;
        target.Memory = source.Memory;
        target.Name = source.Name;
        target.SlaveState = source.SlaveState;
        target.CpuArchitecture = source.CpuArchitecture;
        target.OperatingSystem = source.OperatingSystem;
        target.LastHeartbeat = source.LastHeartbeat;
        target.CpuUtilization = source.CpuUtilization;
      }
    }
    #endregion

    #region SlaveGroup
    public static DT.SlaveGroup ToDto(SlaveGroup source) {
      if (source == null) return null;
      return new DT.SlaveGroup { Id = source.ResourceId, Name = source.Name, ParentResourceId = source.ParentResourceId };
    }
    public static SlaveGroup ToEntity(DT.SlaveGroup source) {
      if (source == null) return null;
      var entity = new SlaveGroup(); ToEntity(source, entity);
      return entity;
    }
    public static void ToEntity(DT.SlaveGroup source, SlaveGroup target) {
      if ((source != null) && (target != null)) {
        target.ResourceId = source.Id; target.Name = source.Name; target.ParentResourceId = source.ParentResourceId;
      }
    }
    #endregion

    #region Resource
    public static DT.Resource ToDto(Resource source) {
      if (source == null) return null;
      return new DT.Resource { Id = source.ResourceId, Name = source.Name, ParentResourceId = source.ParentResourceId };
    }
    public static Resource ToEntity(DT.Resource source) {
      if (source == null) return null;
      var entity = new Resource(); ToEntity(source, entity);
      return entity;
    }
    public static void ToEntity(DT.Resource source, Resource target) {
      if ((source != null) && (target != null)) {
        target.ResourceId = source.Id; target.Name = source.Name; target.ParentResourceId = source.ParentResourceId;
      }
    }
    #endregion

    #region Statistics
    public static DT.Statistics ToDto(Statistics source) {
      if (source == null) return null;
      return new DT.Statistics {
        Id = source.StatisticsId,
        TimeStamp = source.Timestamp,
        SlaveStatistics = source.SlaveStatistics.Select(x => Convert.ToDto(x)).ToArray(),
        UserStatistics = source.UserStatistics.Select(x => Convert.ToDto(x)).ToArray()
      };
    }
    public static Statistics ToEntity(DT.Statistics source) {
      if (source == null) return null;
      var entity = new Statistics(); ToEntity(source, entity);
      return entity;
    }
    public static void ToEntity(DT.Statistics source, Statistics target) {
      if ((source != null) && (target != null)) {
        target.StatisticsId = source.Id;
        target.Timestamp = source.TimeStamp;

      }
    }
    #endregion

    #region SlaveStatistics
    public static DT.SlaveStatistics ToDto(SlaveStatistics source) {
      if (source == null) return null;
      return new DT.SlaveStatistics {
        Id = source.StatisticsId,
        SlaveId = source.SlaveId,
        Cores = source.Cores,
        CpuUtilization = source.CpuUtilization,
        FreeCores = source.FreeCores,
        FreeMemory = source.FreeMemory,
        Memory = source.Memory
      };
    }
    public static SlaveStatistics ToEntity(DT.SlaveStatistics source) {
      if (source == null) return null;
      var entity = new SlaveStatistics(); ToEntity(source, entity);
      return entity;
    }
    public static void ToEntity(DT.SlaveStatistics source, SlaveStatistics target) {
      if ((source != null) && (target != null)) {
        target.StatisticsId = source.Id;
        target.SlaveId = source.SlaveId;
        target.Cores = source.Cores;
        target.CpuUtilization = source.CpuUtilization;
        target.FreeCores = source.FreeCores;
        target.FreeMemory = source.FreeMemory;
        target.Memory = source.Memory;
      }
    }
    #endregion

    #region UserStatistics
    public static DT.UserStatistics ToDto(UserStatistics source) {
      if (source == null) return null;
      return new DT.UserStatistics {
        Id = source.StatisticsId,
        UserId = source.UserId,
        UsedCores = source.UsedCores,
        ExecutionTime = TimeSpan.FromMilliseconds(source.ExecutionTimeMs),
        ExecutionTimeFinishedJobs = TimeSpan.FromMilliseconds(source.ExecutionTimeMsFinishedJobs),
        StartToEndTime = TimeSpan.FromMilliseconds(source.StartToEndTimeMs)
      };
    }
    public static UserStatistics ToEntity(DT.UserStatistics source) {
      if (source == null) return null;
      var entity = new UserStatistics(); ToEntity(source, entity);
      return entity;
    }
    public static void ToEntity(DT.UserStatistics source, UserStatistics target) {
      if ((source != null) && (target != null)) {
        target.StatisticsId = source.Id;
        target.UserId = source.UserId;
        target.UsedCores = source.UsedCores;
        target.ExecutionTimeMs = source.ExecutionTime.TotalMilliseconds;
        target.ExecutionTimeMsFinishedJobs = source.ExecutionTimeFinishedJobs.TotalMilliseconds;
        target.StartToEndTimeMs = source.StartToEndTime.TotalMilliseconds;
      }
    }
    #endregion
  }
}
