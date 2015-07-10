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
using System.Data.Linq;
using System.Linq;
using DT = HeuristicLab.Services.Hive.DataTransfer;

namespace HeuristicLab.Services.Hive.DataAccess {
  public class OptimizedHiveDao : IOptimizedHiveDao {
    private HiveDataContext Db { get; set; }

    public OptimizedHiveDao(HiveDataContext db) {
      Db = db;
    }

    #region Task Methods
    public Task GetTaskById(Guid taskId) {
      return GetTaskByIdQuery(Db, taskId).SingleOrDefault();
    }

    private static readonly Func<HiveDataContext, Guid, IQueryable<Task>> GetTaskByIdQuery = CompiledQuery.Compile((HiveDataContext db, Guid taskId) =>
      from t in db.Tasks
      where t.TaskId == taskId
      select t
    );

    public Task GetTaskByDto(DT.Task taskDto) {
      var task = GetTaskById(taskDto.Id);
      DT.Convert.ToEntity(taskDto, task);
      return task;
    }

    public Tuple<Task, Guid?> GetTaskByIdAndLastStateLogSlaveId(Guid taskId) {
      return GetTaskByIdAndLastStateLogSlaveIdQuery(Db, taskId).SingleOrDefault();
    }

    private static readonly Func<HiveDataContext, Guid, IQueryable<Tuple<Task, Guid?>>> GetTaskByIdAndLastStateLogSlaveIdQuery = CompiledQuery.Compile((HiveDataContext db, Guid taskId) =>
      from t in db.Tasks
      let lastStateLog = t.StateLogs.OrderByDescending(sl => sl.DateTime).FirstOrDefault()
      where t.TaskId == taskId
      select new Tuple<Task, Guid?>(t, lastStateLog != null ? lastStateLog.SlaveId : null)
    );

    private const string GetWaitingTasksQueryString = @"
      WITH pr AS (
        SELECT ResourceId, ParentResourceId
        FROM [Resource]
        WHERE ResourceId = {0}
        UNION ALL
        SELECT r.ResourceId, r.ParentResourceId
        FROM [Resource] r JOIN pr ON r.ResourceId = pr.ParentResourceId
      )
      SELECT DISTINCT t.TaskId, t.JobId, t.Priority
      FROM pr JOIN AssignedResources ar ON ar.ResourceId = pr.ResourceId
          JOIN Task t ON t.TaskId = ar.TaskId
      WHERE NOT (t.IsParentTask = 1 AND t.FinishWhenChildJobsFinished = 1)
          AND t.TaskState = {1}
          AND t.CoresNeeded <= {2}
          AND t.MemoryNeeded <= {3}
    ";

    public IEnumerable<TaskInfoForScheduler> GetWaitingTasks(Slave slave) {
      //Originally we checked here if there are parent tasks which should be calculated (with GetParentTasks(resourceIds, count, false);).
      //Because there is at the moment no case where this makes sense (there don't exist parent tasks which need to be calculated), 
      //we skip this step because it's wasted runtime
      return Db.ExecuteQuery<TaskInfoForScheduler>(GetWaitingTasksQueryString, slave.ResourceId, Enum.GetName(typeof(TaskState), TaskState.Waiting), slave.FreeCores, slave.FreeMemory);
    }

    public IQueryable<DT.LightweightTask> GetLightweightTasks(Guid jobId) {
      return GetLightweightTasksQuery(Db, jobId);
    }

    private static readonly Func<HiveDataContext, Guid, IQueryable<DT.LightweightTask>> GetLightweightTasksQuery = CompiledQuery.Compile((HiveDataContext db, Guid jobId) =>
        from task in db.Tasks
        where task.JobId == jobId
        select new DT.LightweightTask {
          Id = task.TaskId,
          ExecutionTime = TimeSpan.FromMilliseconds(task.ExecutionTimeMs),
          ParentTaskId = task.ParentTaskId,
          StateLog = task.StateLogs.OrderBy(sl => sl.DateTime).Select(sl => ConvertStateLog(sl)).ToList(),
          State = ConvertTaskState(task.State),
          Command = ConvertCommand(task.Command),
          LastTaskDataUpdate = task.JobData.LastUpdate
        }
    );

    private static readonly Func<StateLog, DT.StateLog> ConvertStateLog = sl => DT.Convert.ToDto(sl);
    private static readonly Func<TaskState, DT.TaskState> ConvertTaskState = ts => DT.Convert.ToDto(ts);
    private static readonly Func<Command?, DT.Command?> ConvertCommand = c => DT.Convert.ToDto(c);

    public void UpdateTask(Task task) {
      Db.SubmitChanges();
    }

    public Task UpdateTaskState(Guid taskId, TaskState taskState, Guid? slaveId, Guid? userId, string exception) {
      Db.StateLogs.InsertOnSubmit(new StateLog {
        TaskId = taskId,
        State = taskState,
        SlaveId = slaveId,
        UserId = userId,
        Exception = exception,
        DateTime = DateTime.Now
      });

      var task = GetTaskById(taskId);
      task.State = taskState;

      Db.SubmitChanges();

      return task;
    }

    public Guid AddTask(Task task) {
      Db.Tasks.InsertOnSubmit(task);
      Db.SubmitChanges();
      return task.TaskId;
    }

    public void AssignJobToResource(Guid taskId, IEnumerable<Guid> resourceIds) {
      Db.AssignedResources.InsertAllOnSubmit(resourceIds.Select(resourceId => new AssignedResource { TaskId = taskId, ResourceId = resourceId }));
      Db.SubmitChanges();
    }

    private const string TaskIsAllowedToBeCalculatedBySlaveQueryString = @"
      WITH pr AS (
        SELECT ResourceId, ParentResourceId
        FROM [Resource]
        WHERE ResourceId = {0}
        UNION ALL
        SELECT r.ResourceId, r.ParentResourceId
        FROM [Resource] r JOIN pr ON r.ResourceId = pr.ParentResourceId
      )
      SELECT COUNT(ar.TaskId)
      FROM pr JOIN AssignedResources ar ON pr.ResourceId = ar.ResourceId
      WHERE ar.TaskId = {1}
    ";

    public bool TaskIsAllowedToBeCalculatedBySlave(Guid taskId, Guid slaveId) {
      return Db.ExecuteQuery<int>(TaskIsAllowedToBeCalculatedBySlaveQueryString, slaveId, taskId).First() > 0;
    }
    #endregion

    #region TaskData Methods
    public TaskData GetTaskDataById(Guid id) {
      return GetTaskDataByIdQuery(Db, id).SingleOrDefault();
    }

    private static readonly Func<HiveDataContext, Guid, IQueryable<TaskData>> GetTaskDataByIdQuery = CompiledQuery.Compile((HiveDataContext db, Guid id) =>
      from t in db.TaskDatas
      where t.TaskId == id
      select t
    );

    public TaskData GetTaskDataByDto(DT.TaskData dto) {
      var taskData = GetTaskDataById(dto.TaskId);
      DT.Convert.ToEntity(dto, taskData);
      return taskData;
    }

    public void UpdateTaskData(TaskData taskData) {
      Db.SubmitChanges();
    }
    #endregion

    #region Plugin Methods
    public Plugin GetPluginById(Guid pluginId) {
      return GetPluginByIdQuery(Db, pluginId).SingleOrDefault();
    }

    private static readonly Func<HiveDataContext, Guid, IQueryable<Plugin>> GetPluginByIdQuery = CompiledQuery.Compile((HiveDataContext db, Guid pluginId) =>
      from p in db.Plugins
      where p.PluginId == pluginId
      select p
    );
    #endregion

    #region Slave Methods
    public Slave GetSlaveById(Guid id) {
      return GetSlaveByIdQuery(Db, id).SingleOrDefault();
    }

    private static readonly Func<HiveDataContext, Guid, IQueryable<Slave>> GetSlaveByIdQuery = CompiledQuery.Compile((HiveDataContext db, Guid slaveId) =>
      from s in db.Resources.OfType<Slave>()
      where s.ResourceId == slaveId
      select s
    );

    public void UpdateSlave(Slave slave) {
      Db.SubmitChanges();
    }

    private const string DowntimeQueryString = @"
      WITH pr AS (
        SELECT ResourceId, ParentResourceId
        FROM [Resource]
        WHERE ResourceId = {0}
        UNION ALL
        SELECT r.ResourceId, r.ParentResourceId
        FROM [Resource] r JOIN pr ON r.ResourceId = pr.ParentResourceId
      )
      SELECT COUNT(dt.DowntimeId)
      FROM pr JOIN [Downtime] dt ON pr.ResourceId = dt.ResourceId
      WHERE {1} BETWEEN dt.StartDate AND dt.EndDate
        AND dt.DowntimeType = {2}
      ";

    public bool SlaveHasToShutdownComputer(Guid slaveId) {
      return Db.ExecuteQuery<int>(DowntimeQueryString, slaveId, DateTime.Now, DowntimeType.Shutdown.ToString()).FirstOrDefault() > 0;
    }

    public bool SlaveIsAllowedToCalculate(Guid slaveId) {
      return Db.ExecuteQuery<int>(DowntimeQueryString, slaveId, DateTime.Now, DowntimeType.Offline.ToString()).FirstOrDefault() == 0;
    }
    #endregion

    #region Resource Methods
    public IEnumerable<Guid> GetAssignedResourceIds(Guid taskId) {
      return GetAssignedResourceIdsQuery(Db, taskId);
    }

    private static readonly Func<HiveDataContext, Guid, IQueryable<Guid>> GetAssignedResourceIdsQuery = CompiledQuery.Compile((HiveDataContext db, Guid taskId) =>
      from ar in db.AssignedResources
      where ar.TaskId == taskId
      select ar.ResourceId
    );
    #endregion


    #region Website Methods
    private const string GetAllResourceIdsString = @"SELECT ResourceId FROM [Resource]";
    public IEnumerable<Guid> GetAllResourceIds() {
      return Db.ExecuteQuery<Guid>(GetAllResourceIdsString);
    }

    private const string GetNumberOfWaitingTasksString = @"SELECT COUNT(TaskId) 
                                                           FROM [Task] 
                                                           WHERE TaskState LIKE 'Waiting'";
    public int GetNumberOfWaitingTasks() {
      return Db.ExecuteQuery<int>(GetNumberOfWaitingTasksString).Single();
    }

    private class UserTasks {
      public Guid OwnerUserId;
      public int Count;
    }

    private const string GetCalculatingTasksByUserString = @"SELECT Job.OwnerUserId, COUNT(Task.TaskId) as Count 
                                                             FROM Task, Job 
                                                             WHERE TaskState LIKE 'Calculating' AND Task.JobId = Job.JobId
                                                             GROUP BY Job.OwnerUserId";

    public Dictionary<Guid, int> GetCalculatingTasksByUser() {
      var result = Db.ExecuteQuery<UserTasks>(GetCalculatingTasksByUserString);
      Dictionary<Guid, int> lst = new Dictionary<Guid, int>();

      foreach (var userTask in result) {
        lst.Add(userTask.OwnerUserId, userTask.Count);
      }
      return lst;
    }

    private const string GetWaitingTasksByUserString = @"SELECT Job.OwnerUserId, COUNT(Task.TaskId) as Count 
                                                         FROM Task, Job 
                                                         WHERE TaskState LIKE 'Waiting' AND Task.JobId = Job.JobId
                                                         GROUP BY Job.OwnerUserId";

    public Dictionary<Guid, int> GetWaitingTasksByUser() {
      var result = Db.ExecuteQuery<UserTasks>(GetWaitingTasksByUserString);
      Dictionary<Guid, int> lst = new Dictionary<Guid, int>();

      foreach (var userTask in result) {
        lst.Add(userTask.OwnerUserId, userTask.Count);
      }
      return lst;
    }
    #endregion
  }
}
