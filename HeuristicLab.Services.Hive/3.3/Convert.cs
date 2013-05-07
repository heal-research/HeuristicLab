#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using DB = HeuristicLab.Services.Hive.DataAccess;
using DT = HeuristicLab.Services.Hive.DataTransfer;

namespace HeuristicLab.Services.Hive.DataTransfer {
  public static class Convert {
    #region Task
    public static DT.Task ToDto(DB.Task source) {
      if (source == null) return null;
      return new DT.Task {
        Id = source.TaskId,
        CoresNeeded = source.CoresNeeded,
        ExecutionTime = TimeSpan.FromMilliseconds(source.ExecutionTimeMs),
        MemoryNeeded = source.MemoryNeeded,
        ParentTaskId = source.ParentTaskId,
        Priority = source.Priority,
        PluginsNeededIds = (source.RequiredPlugins == null ? new List<Guid>() : source.RequiredPlugins.Select(x => x.PluginId).ToList()),
        LastHeartbeat = source.LastHeartbeat,
        State = Convert.ToDto(source.State),
        StateLog = (source.StateLogs == null ? new List<DT.StateLog>() : source.StateLogs.Select(x => Convert.ToDto(x)).OrderBy(x => x.DateTime).ToList()),
        IsParentTask = source.IsParentTask,
        FinishWhenChildJobsFinished = source.FinishWhenChildJobsFinished,
        Command = Convert.ToDto(source.Command),
        LastTaskDataUpdate = (source.JobData == null ? DateTime.MinValue : source.JobData.LastUpdate),
        JobId = source.JobId,
        IsPrivileged = source.IsPrivileged
      };
    }

    public static DB.Task ToEntity(DT.Task source) {
      if (source == null) return null;
      var entity = new DB.Task(); ToEntity(source, entity);
      return entity;
    }
    public static void ToEntity(DT.Task source, DB.Task target) {
      if ((source != null) && (target != null)) {
        target.TaskId = source.Id;
        target.CoresNeeded = source.CoresNeeded;
        target.ExecutionTimeMs = source.ExecutionTime.TotalMilliseconds;
        target.MemoryNeeded = source.MemoryNeeded;
        target.ParentTaskId = source.ParentTaskId;
        target.Priority = source.Priority;
        target.LastHeartbeat = source.LastHeartbeat;
        target.State = Convert.ToEntity(source.State);
        foreach (DT.StateLog sl in source.StateLog.Where(x => x.Id == Guid.Empty)) {
          target.StateLogs.Add(Convert.ToEntity(sl));
        }
        target.IsParentTask = source.IsParentTask;
        target.FinishWhenChildJobsFinished = source.FinishWhenChildJobsFinished;
        target.Command = Convert.ToEntity(source.Command);
        // RequiredPlugins are added by Dao
        target.JobId = source.JobId;
        target.IsPrivileged = source.IsPrivileged;
      }
    }

    public static void ToEntityTaskOnly(DT.Task source, DB.Task target) {
      if ((source != null) && (target != null)) {
        target.TaskId = source.Id;
        target.CoresNeeded = source.CoresNeeded;
        target.ExecutionTimeMs = source.ExecutionTime.TotalMilliseconds;
        target.MemoryNeeded = source.MemoryNeeded;
        target.ParentTaskId = source.ParentTaskId;
        target.Priority = source.Priority;
        target.LastHeartbeat = source.LastHeartbeat;
        target.State = Convert.ToEntity(source.State);
        target.IsParentTask = source.IsParentTask;
        target.FinishWhenChildJobsFinished = source.FinishWhenChildJobsFinished;
        target.Command = Convert.ToEntity(source.Command);
        // RequiredPlugins are added by Dao
        target.JobId = source.JobId;
        target.IsPrivileged = source.IsPrivileged;
      }
    }
    #endregion

    #region TaskData
    public static DT.TaskData ToDto(DB.TaskData source) {
      if (source == null) return null;
      return new DT.TaskData { TaskId = source.TaskId, Data = source.Data.ToArray(), LastUpdate = source.LastUpdate };
    }
    public static DB.TaskData ToEntity(DT.TaskData source) {
      if (source == null) return null;
      var entity = new DB.TaskData(); ToEntity(source, entity);
      return entity;
    }
    public static void ToEntity(DT.TaskData source, DB.TaskData target) {
      if ((source != null) && (target != null)) {
        target.TaskId = source.TaskId; target.Data = new Binary(source.Data); target.LastUpdate = source.LastUpdate;
      }
    }
    #endregion

    #region StateLog
    public static DT.StateLog ToDto(DB.StateLog source) {
      if (source == null) return null;
      return new DT.StateLog { Id = source.StateLogId, DateTime = source.DateTime, Exception = source.Exception, TaskId = source.TaskId, SlaveId = source.SlaveId, State = Convert.ToDto(source.State), UserId = source.UserId };
    }
    public static DB.StateLog ToEntity(DT.StateLog source) {
      if (source == null) return null;
      var entity = new DB.StateLog(); ToEntity(source, entity);
      return entity;
    }
    public static void ToEntity(DT.StateLog source, DB.StateLog target) {
      if ((source != null) && (target != null)) {
        target.StateLogId = source.Id; target.DateTime = source.DateTime; target.Exception = source.Exception; target.TaskId = source.TaskId; target.SlaveId = source.SlaveId; target.State = Convert.ToEntity(source.State); target.UserId = source.UserId;
      }
    }
    #endregion

    #region Downtimes
    public static DT.Downtime ToDto(DB.Downtime source) {
      if (source == null) return null;
      return new DT.Downtime { Id = source.DowntimeId, AllDayEvent = source.AllDayEvent, EndDate = source.EndDate, Recurring = source.Recurring, RecurringId = source.RecurringId, ResourceId = source.ResourceId, StartDate = source.StartDate, DowntimeType = source.DowntimeType };
    }
    public static DB.Downtime ToEntity(DT.Downtime source) {
      if (source == null) return null;
      var entity = new DB.Downtime(); ToEntity(source, entity);
      return entity;
    }
    public static void ToEntity(DT.Downtime source, DB.Downtime target) {
      if ((source != null) && (target != null)) {
        target.DowntimeId = source.Id; target.AllDayEvent = source.AllDayEvent; target.EndDate = source.EndDate; target.Recurring = source.Recurring; target.RecurringId = source.RecurringId; target.ResourceId = source.ResourceId; target.StartDate = source.StartDate; target.DowntimeType = source.DowntimeType;
      }
    }
    #endregion

    #region Job
    public static DT.Job ToDto(DB.Job source) {
      if (source == null) return null;
      return new DT.Job { Id = source.JobId, Description = source.Description, Name = source.Name, OwnerUserId = source.OwnerUserId, DateCreated = source.DateCreated, ResourceNames = source.ResourceIds };
    }
    public static DB.Job ToEntity(DT.Job source) {
      if (source == null) return null;
      var entity = new DB.Job(); ToEntity(source, entity);
      return entity;
    }
    public static void ToEntity(DT.Job source, DB.Job target) {
      if ((source != null) && (target != null)) {
        target.JobId = source.Id; target.Description = source.Description; target.Name = source.Name; target.OwnerUserId = source.OwnerUserId; target.DateCreated = source.DateCreated; target.ResourceIds = source.ResourceNames;
      }
    }
    #endregion

    #region JobPermission
    public static DT.JobPermission ToDto(DB.JobPermission source) {
      if (source == null) return null;
      return new DT.JobPermission { JobId = source.JobId, GrantedUserId = source.GrantedUserId, GrantedByUserId = source.GrantedByUserId, Permission = Convert.ToDto(source.Permission) };
    }
    public static DB.JobPermission ToEntity(DT.JobPermission source) {
      if (source == null) return null;
      var entity = new DB.JobPermission(); ToEntity(source, entity);
      return entity;
    }
    public static void ToEntity(DT.JobPermission source, DB.JobPermission target) {
      if ((source != null) && (target != null)) {
        target.JobId = source.JobId; target.GrantedUserId = source.GrantedUserId; target.GrantedByUserId = source.GrantedByUserId; target.Permission = Convert.ToEntity(source.Permission);
      }
    }
    #endregion

    #region Plugin
    public static DT.Plugin ToDto(DB.Plugin source) {
      if (source == null) return null;
      return new DT.Plugin { Id = source.PluginId, Name = source.Name, Version = new Version(source.Version), UserId = source.UserId, DateCreated = source.DateCreated, Hash = source.Hash };
    }
    public static DB.Plugin ToEntity(DT.Plugin source) {
      if (source == null) return null;
      var entity = new DB.Plugin(); ToEntity(source, entity);
      return entity;
    }
    public static void ToEntity(DT.Plugin source, DB.Plugin target) {
      if ((source != null) && (target != null)) {
        target.PluginId = source.Id; target.Name = source.Name; target.Version = source.Version.ToString(); target.UserId = source.UserId; target.DateCreated = source.DateCreated; target.Hash = source.Hash;
      }
    }
    #endregion

    #region PluginData
    public static DT.PluginData ToDto(DB.PluginData source) {
      if (source == null) return null;
      return new DT.PluginData { Id = source.PluginDataId, PluginId = source.PluginId, Data = source.Data.ToArray(), FileName = source.FileName };
    }
    public static DB.PluginData ToEntity(DT.PluginData source) {
      if (source == null) return null;
      var entity = new DB.PluginData(); ToEntity(source, entity);
      return entity;
    }
    public static void ToEntity(DT.PluginData source, DB.PluginData target) {
      if ((source != null) && (target != null)) {
        target.PluginDataId = source.Id; target.PluginId = source.PluginId; target.Data = new Binary(source.Data); target.FileName = source.FileName;
      }
    }
    #endregion

    #region Resource
    public static DT.Resource ToDto(DB.Resource source) {
      if (source == null) return null;
      return new DT.Resource { Id = source.ResourceId, Name = source.Name, ParentResourceId = source.ParentResourceId, HbInterval = source.HbInterval, OwnerUserId = source.OwnerUserId };
    }
    public static DB.Resource ToEntity(DT.Resource source) {
      if (source == null) return null;
      var entity = new DB.Resource(); ToEntity(source, entity);
      return entity;
    }
    public static void ToEntity(DT.Resource source, DB.Resource target) {
      if ((source != null) && (target != null)) {
        target.ResourceId = source.Id; target.Name = source.Name; target.ParentResourceId = source.ParentResourceId; target.HbInterval = source.HbInterval; target.OwnerUserId = source.OwnerUserId;
      }
    }
    #endregion

    #region SlaveGroup
    public static DT.SlaveGroup ToDto(DB.SlaveGroup source) {
      if (source == null) return null;
      return new DT.SlaveGroup { Id = source.ResourceId, Name = source.Name, ParentResourceId = source.ParentResourceId, HbInterval = source.HbInterval, OwnerUserId = source.OwnerUserId };
    }
    public static DB.SlaveGroup ToEntity(DT.SlaveGroup source) {
      if (source == null) return null;
      var entity = new DB.SlaveGroup(); ToEntity(source, entity);
      return entity;
    }
    public static void ToEntity(DT.SlaveGroup source, DB.SlaveGroup target) {
      if ((source != null) && (target != null)) {
        target.ResourceId = source.Id; target.Name = source.Name; target.ParentResourceId = source.ParentResourceId; target.HbInterval = source.HbInterval; target.OwnerUserId = source.OwnerUserId;
      }
    }
    #endregion

    #region Slave
    public static DT.Slave ToDto(DB.Slave source) {
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
        SlaveState = Convert.ToDto(source.SlaveState),
        CpuArchitecture = Convert.ToDto(source.CpuArchitecture),
        OperatingSystem = source.OperatingSystem,
        LastHeartbeat = source.LastHeartbeat,
        CpuUtilization = source.CpuUtilization,
        HbInterval = source.HbInterval,
        IsDisposable = source.IsDisposable,
        OwnerUserId = source.OwnerUserId
      };
    }
    public static DB.Slave ToEntity(DT.Slave source) {
      if (source == null) return null;
      var entity = new DB.Slave(); ToEntity(source, entity);
      return entity;
    }
    public static void ToEntity(DT.Slave source, DB.Slave target) {
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
        target.SlaveState = Convert.ToEntity(source.SlaveState);
        target.CpuArchitecture = Convert.ToEntity(source.CpuArchitecture);
        target.OperatingSystem = source.OperatingSystem;
        target.LastHeartbeat = source.LastHeartbeat;
        target.CpuUtilization = source.CpuUtilization;
        target.HbInterval = source.HbInterval;
        target.IsDisposable = source.IsDisposable;
        target.OwnerUserId = source.OwnerUserId;
      }
    }
    #endregion

    #region ResourcePermission
    public static DT.ResourcePermission ToDto(DB.ResourcePermission source) {
      if (source == null) return null;
      return new DT.ResourcePermission { ResourceId = source.ResourceId, GrantedUserId = source.GrantedUserId, GrantedByUserId = source.GrantedByUserId };
    }
    public static DB.ResourcePermission ToEntity(DT.ResourcePermission source) {
      if (source == null) return null;
      var entity = new DB.ResourcePermission(); ToEntity(source, entity);
      return entity;
    }
    public static void ToEntity(DT.ResourcePermission source, DB.ResourcePermission target) {
      if ((source != null) && (target != null)) {
        target.ResourceId = source.ResourceId; target.GrantedUserId = source.GrantedUserId; target.GrantedByUserId = source.GrantedByUserId;
      }
    }
    #endregion

    #region SlaveStatistics
    public static DT.SlaveStatistics ToDto(DB.SlaveStatistics source) {
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
    public static DB.SlaveStatistics ToEntity(DT.SlaveStatistics source) {
      if (source == null) return null;
      var entity = new DB.SlaveStatistics(); ToEntity(source, entity);
      return entity;
    }
    public static void ToEntity(DT.SlaveStatistics source, DB.SlaveStatistics target) {
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

    #region Statistics
    public static DT.Statistics ToDto(DB.Statistics source) {
      if (source == null) return null;
      return new DT.Statistics {
        Id = source.StatisticsId,
        TimeStamp = source.Timestamp,
        SlaveStatistics = source.SlaveStatistics.Select(x => Convert.ToDto(x)).ToArray(),
        UserStatistics = source.UserStatistics.Select(x => Convert.ToDto(x)).ToArray()
      };
    }
    public static DB.Statistics ToEntity(DT.Statistics source) {
      if (source == null) return null;
      var entity = new DB.Statistics(); ToEntity(source, entity);
      return entity;
    }
    public static void ToEntity(DT.Statistics source, DB.Statistics target) {
      if ((source != null) && (target != null)) {
        target.StatisticsId = source.Id;
        target.Timestamp = source.TimeStamp;

      }
    }
    #endregion

    #region UserStatistics
    public static DT.UserStatistics ToDto(DB.UserStatistics source) {
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
    public static DB.UserStatistics ToEntity(DT.UserStatistics source) {
      if (source == null) return null;
      var entity = new DB.UserStatistics(); ToEntity(source, entity);
      return entity;
    }
    public static void ToEntity(DT.UserStatistics source, DB.UserStatistics target) {
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

    #region TaskData
    public static DT.TaskState ToDto(DB.TaskState source) {
      if (source == DB.TaskState.Aborted) {
        return TaskState.Aborted;
      } else if (source == DB.TaskState.Calculating) {
        return TaskState.Calculating;
      } else if (source == DB.TaskState.Failed) {
        return TaskState.Failed;
      } else if (source == DB.TaskState.Finished) {
        return TaskState.Finished;
      } else if (source == DB.TaskState.Offline) {
        return TaskState.Offline;
      } else if (source == DB.TaskState.Paused) {
        return TaskState.Paused;
      } else if (source == DB.TaskState.Transferring) {
        return TaskState.Transferring;
      } else if (source == DB.TaskState.Waiting) {
        return TaskState.Waiting;
      } else
        return TaskState.Failed;
    }

    public static DB.TaskState ToEntity(DT.TaskState source) {
      if (source == DT.TaskState.Aborted) {
        return DB.TaskState.Aborted;
      } else if (source == DT.TaskState.Calculating) {
        return DB.TaskState.Calculating;
      } else if (source == DT.TaskState.Failed) {
        return DB.TaskState.Failed;
      } else if (source == DT.TaskState.Finished) {
        return DB.TaskState.Finished;
      } else if (source == DT.TaskState.Offline) {
        return DB.TaskState.Offline;
      } else if (source == DT.TaskState.Paused) {
        return DB.TaskState.Paused;
      } else if (source == DT.TaskState.Transferring) {
        return DB.TaskState.Transferring;
      } else if (source == DT.TaskState.Waiting) {
        return DB.TaskState.Waiting;
      } else
        return DB.TaskState.Failed;
    }
    #endregion

    #region Permission
    public static DT.Permission ToDto(DB.Permission source) {
      if (source == DB.Permission.Full) {
        return Permission.Full;
      } else if (source == DB.Permission.NotAllowed) {
        return Permission.NotAllowed;
      } else if (source == DB.Permission.Read) {
        return Permission.Read;
      } else
        return Permission.NotAllowed;
    }

    public static DB.Permission ToEntity(DT.Permission source) {
      if (source == DT.Permission.Full) {
        return DB.Permission.Full;
      } else if (source == DT.Permission.NotAllowed) {
        return DB.Permission.NotAllowed;
      } else if (source == DT.Permission.Read) {
        return DB.Permission.Read;
      } else
        return DB.Permission.NotAllowed;
    }
    #endregion

    #region Command
    public static DT.Command? ToDto(DB.Command? source) {
      if (source.HasValue) {
        if (source.Value == DB.Command.Abort) {
          return Command.Abort;
        } else if (source.Value == DB.Command.Pause) {
          return Command.Pause;
        } else if (source.Value == DB.Command.Stop) {
          return Command.Stop;
        } else
          return Command.Pause;
      }
      return null;
    }

    public static DB.Command? ToEntity(DT.Command? source) {
      if (source.HasValue) {
        if (source == DT.Command.Abort) {
          return DB.Command.Abort;
        } else if (source == DT.Command.Pause) {
          return DB.Command.Pause;
        } else if (source == DT.Command.Stop) {
          return DB.Command.Stop;
        } else
          return DB.Command.Pause;
      } else
        return null;
    }
    #endregion

    #region CpuArchiteture
    public static DT.CpuArchitecture ToDto(DB.CpuArchitecture source) {
      if (source == DB.CpuArchitecture.x64) {
        return CpuArchitecture.x64;
      } else if (source == DB.CpuArchitecture.x86) {
        return CpuArchitecture.x86;
      } else
        return CpuArchitecture.x86;
    }

    public static DB.CpuArchitecture ToEntity(DT.CpuArchitecture source) {
      if (source == DT.CpuArchitecture.x64) {
        return DB.CpuArchitecture.x64;
      } else if (source == DT.CpuArchitecture.x86) {
        return DB.CpuArchitecture.x86;
      } else
        return DB.CpuArchitecture.x86;
    }
    #endregion

    #region SlaveState
    public static DT.SlaveState ToDto(DB.SlaveState source) {
      if (source == DB.SlaveState.Calculating) {
        return SlaveState.Calculating;
      } else if (source == DB.SlaveState.Idle) {
        return SlaveState.Idle;
      } else if (source == DB.SlaveState.Offline) {
        return SlaveState.Offline;
      } else
        return SlaveState.Offline;
    }

    public static DB.SlaveState ToEntity(DT.SlaveState source) {
      if (source == DT.SlaveState.Calculating) {
        return DB.SlaveState.Calculating;
      } else if (source == DT.SlaveState.Idle) {
        return DB.SlaveState.Idle;
      } else if (source == DT.SlaveState.Offline) {
        return DB.SlaveState.Offline;
      } else
        return DB.SlaveState.Offline;
    }
    #endregion

    #region UserPriority
    public static DT.UserPriority ToDto(DB.UserPriority source) {
      if (source == null) return null;
      return new DT.UserPriority() { Id = source.UserId, DateEnqueued = source.DateEnqueued };
    }
    public static DB.UserPriority ToEntity(DT.UserPriority source) {
      if (source == null) return null;
      var entity = new DB.UserPriority(); ToEntity(source, entity);
      return entity;
    }
    public static void ToEntity(DT.UserPriority source, DB.UserPriority target) {
      if ((source != null) && (target != null)) {
        target.UserId = source.Id;
        target.DateEnqueued = source.DateEnqueued;
      }
    }
    #endregion
  }
}
