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
using System.Linq;
using DA = HeuristicLab.Services.Hive.DataAccess;
using DT = HeuristicLab.Services.Hive.DataTransfer;

namespace HeuristicLab.Services.Hive {
  public static class Converter {

    #region Task
    public static DT.Task ToDto(this DA.Task source) {
      if (source == null) return null;
      return new DT.Task {
        Id = source.TaskId,
        State = source.State.ToDto(),
        ExecutionTime = TimeSpan.FromMilliseconds(source.ExecutionTimeMs),
        LastHeartbeat = source.LastHeartbeat,
        ParentTaskId = source.ParentTaskId,
        Priority = source.Priority,
        CoresNeeded = source.CoresNeeded,
        MemoryNeeded = source.MemoryNeeded,
        IsParentTask = source.IsParentTask,
        FinishWhenChildJobsFinished = source.FinishWhenChildJobsFinished,
        Command = source.Command.ToDto(),
        JobId = source.JobId,
        PluginsNeededIds = source.RequiredPlugins.Select(x => x.PluginId).ToList(),
        StateLog = source.StateLogs.Select(x => x.ToDto()).OrderBy(x => x.DateTime).ToList(),
        LastTaskDataUpdate = source.JobData == null ? DateTime.MinValue : source.JobData.LastUpdate,
        IsPrivileged = true
      };
    }

    public static DA.Task ToEntity(this DT.Task source) {
      if (source == null) return null;
      var result = new DA.Task();
      source.CopyToEntity(result);
      return result;
    }

    public static void CopyToEntity(this DT.Task source, DA.Task target) {
      if ((source == null) || (target == null)) return;
      target.TaskId = source.Id;
      target.State = source.State.ToEntity();
      target.ExecutionTimeMs = source.ExecutionTime.TotalMilliseconds;
      target.LastHeartbeat = source.LastHeartbeat;
      target.ParentTaskId = source.ParentTaskId;
      target.Priority = source.Priority;
      target.CoresNeeded = source.CoresNeeded;
      target.MemoryNeeded = source.MemoryNeeded;
      target.IsParentTask = source.IsParentTask;
      target.FinishWhenChildJobsFinished = source.FinishWhenChildJobsFinished;
      target.Command = source.Command.ToEntity();
      target.JobId = source.JobId;
      var ids = target.RequiredPlugins.Select(x => x.PluginId);
      target.RequiredPlugins.AddRange(source.PluginsNeededIds
        .Where(x => !ids.Contains(x))
        .Select(x => new DA.RequiredPlugin {
          PluginId = x
        })
      );
      target.StateLogs.AddRange(source.StateLog
        .Where(x => x.Id == Guid.Empty)
        .Select(x => x.ToEntity())
      );
      // result.JobData missing
      // result.AssignedResources missing
    }
    #endregion

    #region TaskData
    public static DT.TaskData ToDto(this DA.TaskData source) {
      if (source == null) return null;
      return new DT.TaskData {
        TaskId = source.TaskId,
        Data = source.Data,
        LastUpdate = source.LastUpdate
      };
    }

    public static DA.TaskData ToEntity(this DT.TaskData source) {
      if (source == null) return null;
      var result = new DA.TaskData();
      source.CopyToEntity(result);
      return result;
    }

    public static void CopyToEntity(this DT.TaskData source, DA.TaskData target) {
      if ((source == null) || (target == null)) return;
      target.TaskId = source.TaskId;
      target.Data = source.Data;
      target.LastUpdate = source.LastUpdate;
    }
    #endregion

    #region Job
    public static DT.Job ToDto(this DA.Job source) {
      return new DT.Job {
        Id = source.JobId,
        Description = source.Description,
        Name = source.Name,
        OwnerUserId = source.OwnerUserId,
        DateCreated = source.DateCreated,
        ProjectId = source.ProjectId,
        State = source.State.ToDto()
      };
    }

    public static DA.Job ToEntity(this DT.Job source) {
      if (source == null) return null;
      var result = new DA.Job();
      source.CopyToEntity(result);
      return result;
    }

    public static void CopyToEntity(this DT.Job source, DA.Job target) {
      if ((source == null) || (target == null)) return;
      target.JobId = source.Id;
      target.Description = source.Description;
      target.Name = source.Name;
      target.OwnerUserId = source.OwnerUserId;
      target.DateCreated = source.DateCreated;
      target.ProjectId = source.ProjectId;
      target.State = source.State.ToEntity();
    }
    #endregion

    #region AssignedJobResource
    public static DT.AssignedJobResource ToDto(this DA.AssignedJobResource source) {
      if (source == null) return null;
      return new DT.AssignedJobResource {
        JobId = source.JobId,
        ResourceId = source.ResourceId
      };
    }
    public static DA.AssignedJobResource ToEntity(this DT.AssignedJobResource source) {
      if (source == null) return null;
      var result = new DA.AssignedJobResource();
      source.CopyToEntity(result);
      return result;
    }
    public static void CopyToEntity(this DT.AssignedJobResource source, DA.AssignedJobResource target) {
      if ((source == null) || (target == null)) return;
      target.JobId = source.JobId;
      target.ResourceId = source.ResourceId;
    }
    #endregion

    #region JobPermission
    public static DT.JobPermission ToDto(this DA.JobPermission source) {
      if (source == null) return null;
      return new DT.JobPermission {
        JobId = source.JobId,
        GrantedUserId = source.GrantedUserId,
        GrantedByUserId = source.GrantedByUserId,
        Permission = source.Permission.ToDto()
      };
    }

    public static DA.JobPermission ToEntity(this DT.JobPermission source) {
      if (source == null) return null;
      var result = new DA.JobPermission();
      source.CopyToEntity(result);
      return result;
    }

    public static void CopyToEntity(this DT.JobPermission source, DA.JobPermission target) {
      if ((source == null) || (target == null)) return;
      target.JobId = source.JobId;
      target.GrantedUserId = source.GrantedUserId;
      target.GrantedByUserId = source.GrantedByUserId;
      target.Permission = source.Permission.ToEntity();
    }
    #endregion

    #region Slave
    public static DT.Slave ToDto(this DA.Slave source) {
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
        SlaveState = source.SlaveState.ToDto(),
        CpuArchitecture = source.CpuArchitecture.ToDto(),
        OperatingSystem = source.OperatingSystem,
        LastHeartbeat = source.LastHeartbeat,
        CpuUtilization = source.CpuUtilization,
        HbInterval = source.HbInterval,
        IsDisposable = source.IsDisposable,
        OwnerUserId = source.OwnerUserId
      };
    }
    public static DA.Slave ToEntity(this DT.Slave source) {
      if (source == null) return null;
      var result = new DA.Slave();
      source.CopyToEntity(result);
      return result;
    }
    public static void CopyToEntity(this DT.Slave source, DA.Slave target) {
      if ((source == null) || (target == null)) return;
      target.ResourceId = source.Id;
      target.ParentResourceId = source.ParentResourceId;
      target.Cores = source.Cores;
      target.CpuSpeed = source.CpuSpeed;
      target.FreeCores = source.FreeCores;
      target.FreeMemory = source.FreeMemory;
      target.IsAllowedToCalculate = source.IsAllowedToCalculate;
      target.Memory = source.Memory;
      target.Name = source.Name;
      target.SlaveState = source.SlaveState.ToEntity();
      target.CpuArchitecture = source.CpuArchitecture.ToEntity();
      target.OperatingSystem = source.OperatingSystem;
      target.LastHeartbeat = source.LastHeartbeat;
      target.CpuUtilization = source.CpuUtilization;
      target.HbInterval = source.HbInterval;
      target.IsDisposable = source.IsDisposable;
      target.OwnerUserId = source.OwnerUserId;
    }
    #endregion

    #region TaskState
    public static DT.TaskState ToDto(this DA.TaskState source) {
      switch (source) {
        case DA.TaskState.Aborted: return DT.TaskState.Aborted;
        case DA.TaskState.Calculating: return DT.TaskState.Calculating;
        case DA.TaskState.Failed: return DT.TaskState.Failed;
        case DA.TaskState.Finished: return DT.TaskState.Finished;
        case DA.TaskState.Offline: return DT.TaskState.Offline;
        case DA.TaskState.Paused: return DT.TaskState.Paused;
        case DA.TaskState.Transferring: return DT.TaskState.Transferring;
        case DA.TaskState.Waiting: return DT.TaskState.Waiting;
        default: return DT.TaskState.Failed;
      }
    }

    public static DA.TaskState ToEntity(this DT.TaskState source) {
      switch (source) {
        case DT.TaskState.Aborted: return DA.TaskState.Aborted;
        case DT.TaskState.Calculating: return DA.TaskState.Calculating;
        case DT.TaskState.Failed: return DA.TaskState.Failed;
        case DT.TaskState.Finished: return DA.TaskState.Finished;
        case DT.TaskState.Offline: return DA.TaskState.Offline;
        case DT.TaskState.Paused: return DA.TaskState.Paused;
        case DT.TaskState.Transferring: return DA.TaskState.Transferring;
        case DT.TaskState.Waiting: return DA.TaskState.Waiting;
        default: return DA.TaskState.Failed;
      }
    }
    #endregion

    #region JobState
    public static DT.JobState ToDto(this DA.JobState source) {
      switch (source) {
        case DA.JobState.Online: return DT.JobState.Online;
        case DA.JobState.StatisticsPending: return DT.JobState.StatisticsPending;
        case DA.JobState.DeletionPending: return DT.JobState.DeletionPending;
        default: return DT.JobState.Online;
      }
    }

    public static DA.JobState ToEntity(this DT.JobState source) {
      switch (source) {
        case DT.JobState.Online: return DA.JobState.Online;
        case DT.JobState.StatisticsPending: return DA.JobState.StatisticsPending;
        case DT.JobState.DeletionPending: return DA.JobState.DeletionPending;
        default: return DA.JobState.Online;
      }
    }
    #endregion

    #region StateLogs
    public static DT.StateLog ToDto(this DA.StateLog source) {
      return new DT.StateLog {
        Id = source.StateLogId,
        State = source.State.ToDto(),
        DateTime = source.DateTime,
        TaskId = source.TaskId,
        UserId = source.UserId,
        SlaveId = source.SlaveId,
        Exception = source.Exception
      };
    }

    public static DA.StateLog ToEntity(this DT.StateLog source) {
      return new DA.StateLog {
        StateLogId = source.Id,
        State = source.State.ToEntity(),
        DateTime = source.DateTime,
        TaskId = source.TaskId,
        UserId = source.UserId,
        SlaveId = source.SlaveId,
        Exception = source.Exception
      };
    }
    #endregion

    #region Plugin
    public static DT.Plugin ToDto(this DA.Plugin source) {
      if (source == null) return null;
      return new DT.Plugin {
        Id = source.PluginId,
        Name = source.Name,
        Version = new Version(source.Version),
        UserId = source.UserId,
        DateCreated = source.DateCreated,
        Hash = source.Hash
      };
    }
    public static DA.Plugin ToEntity(this DT.Plugin source) {
      if (source == null) return null;
      var result = new DA.Plugin();
      source.CopyToEntity(result);
      return result;
    }
    public static void CopyToEntity(this DT.Plugin source, DA.Plugin target) {
      if ((source == null) || (target == null)) return;
      target.PluginId = source.Id;
      target.Name = source.Name;
      target.Version = source.Version.ToString();
      target.UserId = source.UserId;
      target.DateCreated = source.DateCreated;
      target.Hash = source.Hash;
    }
    #endregion

    #region PluginData
    public static DT.PluginData ToDto(this DA.PluginData source) {
      if (source == null) return null;
      return new DT.PluginData {
        Id = source.PluginDataId,
        PluginId = source.PluginId,
        Data = source.Data.ToArray(),
        FileName = source.FileName
      };
    }

    public static DA.PluginData ToEntity(this DT.PluginData source) {
      if (source == null) return null;
      var result = new DA.PluginData();
      source.CopyToEntity(result);
      return result;
    }

    public static void CopyToEntity(this DT.PluginData source, DA.PluginData target) {
      if ((source == null) || (target == null)) return;
      target.PluginDataId = source.Id;
      target.PluginId = source.PluginId;
      target.Data = source.Data;
      target.FileName = source.FileName;
    }
    #endregion

    #region Project
    public static DT.Project ToDto(this DA.Project source) {
      if (source == null) return null;
      return new DT.Project {
        Id = source.ProjectId,
        ParentProjectId = source.ParentProjectId,
        DateCreated = source.DateCreated,
        Name = source.Name,
        Description = source.Description,
        OwnerUserId = source.OwnerUserId,
        StartDate = source.StartDate,
        EndDate = source.EndDate
      };
    }
    public static DA.Project ToEntity(this DT.Project source) {
      if (source == null) return null;
      var result = new DA.Project();
      source.CopyToEntity(result);
      return result;
    }
    public static void CopyToEntity(this DT.Project source, DA.Project target) {
      if ((source == null) || (target == null)) return;
      target.ProjectId = source.Id;
      target.ParentProjectId = source.ParentProjectId;
      target.DateCreated = source.DateCreated;
      target.Name = source.Name;
      target.Description = source.Description;
      target.OwnerUserId = source.OwnerUserId;
      target.StartDate = source.StartDate;
      target.EndDate = source.EndDate;
    }
    #endregion

    #region ProjectPermission
    public static DT.ProjectPermission ToDto(this DA.ProjectPermission source) {
      if (source == null) return null;
      return new DT.ProjectPermission {
        ProjectId = source.ProjectId,
        GrantedUserId = source.GrantedUserId,
        GrantedByUserId = source.GrantedByUserId
      };
    }
    public static DA.ProjectPermission ToEntity(this DT.ProjectPermission source) {
      if (source == null) return null;
      var result = new DA.ProjectPermission();
      source.CopyToEntity(result);
      return result;
    }
    public static void CopyToEntity(this DT.ProjectPermission source, DA.ProjectPermission target) {
      if ((source == null) || (target == null)) return;
      target.ProjectId = source.ProjectId;
      target.GrantedUserId = source.GrantedUserId;
      target.GrantedByUserId = source.GrantedByUserId;
    }
    #endregion

    #region AssignedProjectResource
    public static DT.AssignedProjectResource ToDto(this DA.AssignedProjectResource source) {
      if (source == null) return null;
      return new DT.AssignedProjectResource {
        ProjectId = source.ProjectId,
        ResourceId = source.ResourceId
      };
    }
    public static DA.AssignedProjectResource ToEntity(this DT.AssignedProjectResource source) {
      if (source == null) return null;
      var result = new DA.AssignedProjectResource();
      source.CopyToEntity(result);
      return result;
    }
    public static void CopyToEntity(this DT.AssignedProjectResource source, DA.AssignedProjectResource target) {
      if ((source == null) || (target == null)) return;
      target.ProjectId = source.ProjectId;
      target.ResourceId = source.ResourceId;
    }
    #endregion

    #region SlaveGroup
    public static DT.SlaveGroup ToDto(this DA.SlaveGroup source) {
      if (source == null) return null;
      return new DT.SlaveGroup {
        Id = source.ResourceId,
        Name = source.Name,
        ParentResourceId = source.ParentResourceId,
        HbInterval = source.HbInterval,
        OwnerUserId = source.OwnerUserId
      };
    }

    public static DA.SlaveGroup ToEntity(this DT.SlaveGroup source) {
      if (source == null) return null;
      var result = new DA.SlaveGroup();
      source.CopyToEntity(result);
      return result;
    }

    public static void CopyToEntity(this DT.SlaveGroup source, DA.SlaveGroup target) {
      if ((source == null) || (target == null)) return;
      target.ResourceId = source.Id;
      target.Name = source.Name;
      target.ParentResourceId = source.ParentResourceId;
      target.HbInterval = source.HbInterval;
      target.OwnerUserId = source.OwnerUserId;
    }
    #endregion

    #region Downtimes
    public static DT.Downtime ToDto(this DA.Downtime source) {
      if (source == null) return null;
      return new DT.Downtime {
        Id = source.DowntimeId,
        AllDayEvent = source.AllDayEvent,
        EndDate = source.EndDate,
        Recurring = source.Recurring,
        RecurringId = source.RecurringId,
        ResourceId = source.ResourceId,
        StartDate = source.StartDate,
        DowntimeType = source.DowntimeType
      };
    }
    public static DA.Downtime ToEntity(this DT.Downtime source) {
      if (source == null) return null;
      var result = new DA.Downtime();
      source.CopyToEntity(result);
      return result;
    }
    public static void CopyToEntity(this DT.Downtime source, DA.Downtime target) {
      if ((source == null) || (target == null)) return;
      target.DowntimeId = source.Id;
      target.AllDayEvent = source.AllDayEvent;
      target.EndDate = source.EndDate;
      target.Recurring = source.Recurring;
      target.RecurringId = source.RecurringId;
      target.ResourceId = source.ResourceId;
      target.StartDate = source.StartDate;
      target.DowntimeType = source.DowntimeType;
    }
    #endregion


    #region Command
    public static DT.Command? ToDto(this DA.Command? source) {
      if (source.HasValue) {
        switch (source) {
          case DA.Command.Abort: return DT.Command.Abort;
          case DA.Command.Pause: return DT.Command.Pause;
          case DA.Command.Stop: return DT.Command.Stop;
          default: return DT.Command.Pause;
        }
      }
      return null;
    }

    public static DA.Command? ToEntity(this DT.Command? source) {
      if (source.HasValue) {
        switch (source) {
          case DT.Command.Abort: return DA.Command.Abort;
          case DT.Command.Pause: return DA.Command.Pause;
          case DT.Command.Stop: return DA.Command.Stop;
          default: return DA.Command.Pause;
        }
      }
      return null;
    }
    #endregion

    #region Permission
    public static DT.Permission ToDto(this DA.Permission source) {
      switch (source) {
        case DA.Permission.Full: return DT.Permission.Full;
        case DA.Permission.NotAllowed: return DT.Permission.NotAllowed;
        case DA.Permission.Read: return DT.Permission.Read;
        default: return DT.Permission.NotAllowed;
      }
    }

    public static DA.Permission ToEntity(this DT.Permission source) {
      switch (source) {
        case DT.Permission.Full: return DA.Permission.Full;
        case DT.Permission.NotAllowed: return DA.Permission.NotAllowed;
        case DT.Permission.Read: return DA.Permission.Read;
        default: return DA.Permission.NotAllowed;
      }
    }
    #endregion

    #region CpuArchiteture
    public static DT.CpuArchitecture ToDto(this DA.CpuArchitecture source) {
      switch (source) {
        case DA.CpuArchitecture.x64: return DT.CpuArchitecture.x64;
        case DA.CpuArchitecture.x86: return DT.CpuArchitecture.x86;
        default: return DT.CpuArchitecture.x86;
      }
    }

    public static DA.CpuArchitecture ToEntity(this DT.CpuArchitecture source) {
      switch (source) {
        case DT.CpuArchitecture.x64: return DA.CpuArchitecture.x64;
        case DT.CpuArchitecture.x86: return DA.CpuArchitecture.x86;
        default: return DA.CpuArchitecture.x86;
      }
    }
    #endregion

    #region SlaveState
    public static DT.SlaveState ToDto(this DA.SlaveState source) {
      switch (source) {
        case DA.SlaveState.Calculating: return DT.SlaveState.Calculating;
        case DA.SlaveState.Idle: return DT.SlaveState.Idle;
        case DA.SlaveState.Offline: return DT.SlaveState.Offline;
        default: return DT.SlaveState.Offline;
      }
    }

    public static DA.SlaveState ToEntity(this DT.SlaveState source) {
      switch (source) {
        case DT.SlaveState.Calculating: return DA.SlaveState.Calculating;
        case DT.SlaveState.Idle: return DA.SlaveState.Idle;
        case DT.SlaveState.Offline: return DA.SlaveState.Offline;
        default: return DA.SlaveState.Offline;
      }
    }
    #endregion

    #region UserPriority
    public static DT.UserPriority ToDto(this DA.UserPriority source) {
      if (source == null) return null;
      return new DT.UserPriority() {
        Id = source.UserId,
        DateEnqueued = source.DateEnqueued
      };
    }
    public static DA.UserPriority ToEntity(this DT.UserPriority source) {
      if (source == null) return null;
      var result = new DA.UserPriority();
      source.CopyToEntity(result);
      return result;
    }
    public static void CopyToEntity(this DT.UserPriority source, DA.UserPriority target) {
      if ((source == null) || (target == null)) return;
      target.UserId = source.Id;
      target.DateEnqueued = source.DateEnqueued;
    }
    #endregion

  }
}
