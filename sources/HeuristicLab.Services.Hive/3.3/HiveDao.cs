#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Linq.Expressions;
using DT = HeuristicLab.Services.Hive.DataTransfer;

namespace HeuristicLab.Services.Hive.DataAccess {
  public class HiveDao : IHiveDao {
    public static HiveDataContext CreateContext(bool longRunning = false) {
      var context = new HiveDataContext(Settings.Default.HeuristicLab_Hive_LinqConnectionString);
      if (longRunning) context.CommandTimeout = (int)Settings.Default.LongRunningDatabaseCommandTimeout.TotalSeconds;
      return context;
    }

    public HiveDao() { }

    #region Task Methods
    public DT.Task GetTask(Guid id) {
      using (var db = CreateContext()) {
        return DT.Convert.ToDto(db.Tasks.SingleOrDefault(x => x.TaskId == id));
      }
    }

    public IEnumerable<DT.Task> GetTasks(Expression<Func<Task, bool>> predicate) {
      using (var db = CreateContext()) {
        return db.Tasks.Where(predicate).Select(x => DT.Convert.ToDto(x)).ToArray();
      }
    }

    public IEnumerable<DT.LightweightTask> GetLightweightTasks(Expression<Func<Task, bool>> predicate) {
      List<DT.LightweightTask> tasks = new List<DT.LightweightTask>();

      using (var db = CreateContext()) {
        var tasksQuery = db.Tasks.Where(predicate).Select(task => new { task.TaskId, task.ExecutionTimeMs, task.ParentTaskId, task.StateLogs, task.State, task.Command });
        var taskDatasQuery = db.Tasks.Where(predicate).Where(task => task.JobData != null).Select(task => new { task.TaskId, task.JobData.LastUpdate });

        foreach (var task in tasksQuery) {
          DT.LightweightTask t = new DT.LightweightTask();
          t.Id = task.TaskId;
          t.ExecutionTime = TimeSpan.FromMilliseconds(task.ExecutionTimeMs);
          t.ParentTaskId = task.ParentTaskId;
          t.StateLog = task.StateLogs == null ? new List<DT.StateLog>() : task.StateLogs.Select(x => DataTransfer.Convert.ToDto(x)).OrderBy(x => x.DateTime).ToList();
          t.State = DataTransfer.Convert.ToDto(task.State);
          t.Command = DataTransfer.Convert.ToDto(task.Command);
          t.LastTaskDataUpdate = taskDatasQuery.Where(x => x.TaskId == task.TaskId).Count() > 0 ? taskDatasQuery.Select(x => x.LastUpdate).First() : DateTime.MinValue;
          tasks.Add(t);
        }
      }
      return tasks;
    }

    public Guid AddTask(DT.Task dto) {
      using (var db = CreateContext()) {
        var entity = DT.Convert.ToEntity(dto);
        db.Tasks.InsertOnSubmit(entity);
        db.SubmitChanges();
        foreach (Guid pluginId in dto.PluginsNeededIds) {
          db.RequiredPlugins.InsertOnSubmit(new RequiredPlugin() { TaskId = entity.TaskId, PluginId = pluginId });
        }
        db.SubmitChanges();
        return entity.TaskId;
      }
    }

    public void UpdateTask(DT.Task dto) {
      using (var db = CreateContext()) {
        var entity = db.Tasks.FirstOrDefault(x => x.TaskId == dto.Id);
        if (entity == null) db.Tasks.InsertOnSubmit(DT.Convert.ToEntity(dto));
        else DT.Convert.ToEntity(dto, entity);
        foreach (Guid pluginId in dto.PluginsNeededIds) {
          if (db.RequiredPlugins.Count(p => p.PluginId == pluginId) == 0) {
            db.RequiredPlugins.InsertOnSubmit(new RequiredPlugin() { TaskId = entity.TaskId, PluginId = pluginId });
          }
        }
        db.SubmitChanges();
      }
    }

    public void DeleteTask(Guid id) {
      using (var db = CreateContext()) {
        var entity = db.Tasks.FirstOrDefault(x => x.TaskId == id);
        if (entity != null) db.Tasks.DeleteOnSubmit(entity);
        db.SubmitChanges(); // taskData and child tasks are deleted by db-trigger
      }
    }

    /// <summary>
    /// returns all parent tasks which are waiting for their child tasks to finish
    /// </summary>
    /// <param name="resourceIds">list of resourceids which for which the task should be valid</param>
    /// <param name="count">maximum number of task to return</param>
    /// <param name="finished">if true, all parent task which have FinishWhenChildJobsFinished=true are returned, otherwise only FinishWhenChildJobsFinished=false are returned</param>
    /// <returns></returns>
    public IEnumerable<DT.Task> GetParentTasks(IEnumerable<Guid> resourceIds, int count, bool finished) {
      using (var db = CreateContext()) {
        var query = from ar in db.AssignedResources
                    where resourceIds.Contains(ar.ResourceId)
                       && ar.Task.State == TaskState.Waiting
                       && ar.Task.IsParentTask
                       && (finished ? ar.Task.FinishWhenChildJobsFinished : !ar.Task.FinishWhenChildJobsFinished)
                       && (from child in db.Tasks
                           where child.ParentTaskId == ar.Task.TaskId
                           select child.State == TaskState.Finished
                               || child.State == TaskState.Aborted
                               || child.State == TaskState.Failed).All(x => x)
                       && (from child in db.Tasks // avoid returning WaitForChildTasks task where no child-task exist (yet)
                           where child.ParentTaskId == ar.Task.TaskId
                           select child).Count() > 0
                    orderby ar.Task.Priority descending, db.Random()
                    select DT.Convert.ToDto(ar.Task);
        return count == 0 ? query.ToArray() : query.Take(count).ToArray();
      }
    }

    public IEnumerable<DT.Task> GetWaitingTasks(DT.Slave slave, int count) {
      using (var db = CreateContext()) {
        var resourceIds = GetParentResources(slave.Id).Select(r => r.Id);
        //Originally we checked here if there are parent tasks which should be calculated (with GetParentTasks(resourceIds, count, false);).
        //Because there is at the moment no case where this makes sense (there don't exist parent tasks which need to be calculated), 
        //we skip this step because it's wasted runtime

        var query = from ar in db.AssignedResources
                    where resourceIds.Contains(ar.ResourceId)
                       && !(ar.Task.IsParentTask && ar.Task.FinishWhenChildJobsFinished)
                       && ar.Task.State == TaskState.Waiting
                       && ar.Task.CoresNeeded <= slave.FreeCores
                       && ar.Task.MemoryNeeded <= slave.FreeMemory
                    orderby ar.Task.Priority descending, db.Random() // take random task to avoid the race condition that occurs when this method is called concurrently (the same task would be returned)
                    select DT.Convert.ToDto(ar.Task);
        var waitingTasks = (count == 0 ? query : query.Take(count)).ToArray();
        return waitingTasks;
      }
    }

    public DT.Task UpdateTaskState(Guid taskId, TaskState taskState, Guid? slaveId, Guid? userId, string exception) {
      using (var db = CreateContext()) {
        var job = db.Tasks.SingleOrDefault(x => x.TaskId == taskId);
        job.State = taskState;
        db.StateLogs.InsertOnSubmit(new StateLog {
          TaskId = taskId,
          State = taskState,
          SlaveId = slaveId,
          UserId = userId,
          Exception = exception,
          DateTime = DateTime.Now
        });
        db.SubmitChanges();
        job = db.Tasks.SingleOrDefault(x => x.TaskId == taskId);
        return DT.Convert.ToDto(job);
      }
    }
    #endregion

    #region TaskData Methods
    public DT.TaskData GetTaskData(Guid id) {
      using (var db = CreateContext(true)) {
        return DT.Convert.ToDto(db.TaskDatas.SingleOrDefault(x => x.TaskId == id));
      }
    }

    public IEnumerable<DT.TaskData> GetTaskDatas(Expression<Func<TaskData, bool>> predicate) {
      using (var db = CreateContext(true)) {
        return db.TaskDatas.Where(predicate).Select(x => DT.Convert.ToDto(x)).ToArray();
      }
    }

    public Guid AddTaskData(DT.TaskData dto) {
      using (var db = CreateContext(true)) {
        var entity = DT.Convert.ToEntity(dto);
        db.TaskDatas.InsertOnSubmit(entity);
        db.SubmitChanges();
        return entity.TaskId;
      }
    }

    public void UpdateTaskData(DT.TaskData dto) {
      using (var db = CreateContext(true)) {
        var entity = db.TaskDatas.FirstOrDefault(x => x.TaskId == dto.TaskId);
        if (entity == null) db.TaskDatas.InsertOnSubmit(DT.Convert.ToEntity(dto));
        else DT.Convert.ToEntity(dto, entity);
        db.SubmitChanges();
      }
    }

    public void DeleteTaskData(Guid id) {
      using (var db = CreateContext()) {
        var entity = db.TaskDatas.FirstOrDefault(x => x.TaskId == id); // check if all the byte[] is loaded into memory here. otherwise work around to delete without loading it
        if (entity != null) db.TaskDatas.DeleteOnSubmit(entity);
        db.SubmitChanges();
      }
    }
    #endregion

    #region StateLog Methods
    public DT.StateLog GetStateLog(Guid id) {
      using (var db = CreateContext()) {
        return DT.Convert.ToDto(db.StateLogs.SingleOrDefault(x => x.StateLogId == id));
      }
    }

    public IEnumerable<DT.StateLog> GetStateLogs(Expression<Func<StateLog, bool>> predicate) {
      using (var db = CreateContext()) {
        return db.StateLogs.Where(predicate).Select(x => DT.Convert.ToDto(x)).ToArray();
      }
    }

    public Guid AddStateLog(DT.StateLog dto) {
      using (var db = CreateContext()) {
        var entity = DT.Convert.ToEntity(dto);
        db.StateLogs.InsertOnSubmit(entity);
        db.SubmitChanges();
        return entity.StateLogId;
      }
    }

    public void UpdateStateLog(DT.StateLog dto) {
      using (var db = CreateContext()) {
        var entity = db.StateLogs.FirstOrDefault(x => x.StateLogId == dto.Id);
        if (entity == null) db.StateLogs.InsertOnSubmit(DT.Convert.ToEntity(dto));
        else DT.Convert.ToEntity(dto, entity);
        db.SubmitChanges();
      }
    }

    public void DeleteStateLog(Guid id) {
      using (var db = CreateContext()) {
        var entity = db.StateLogs.FirstOrDefault(x => x.StateLogId == id);
        if (entity != null) db.StateLogs.DeleteOnSubmit(entity);
        db.SubmitChanges();
      }
    }
    #endregion

    #region Job Methods
    public DT.Job GetJob(Guid id) {
      using (var db = CreateContext()) {
        return AddStatsToJob(db, DT.Convert.ToDto(db.Jobs.SingleOrDefault(x => x.JobId == id)));
      }
    }

    private DT.Job AddStatsToJob(HiveDataContext db, DT.Job exp) {
      if (exp == null)
        return null;

      var jobs = db.Tasks.Where(j => j.JobId == exp.Id);
      exp.JobCount = jobs.Count();
      exp.CalculatingCount = jobs.Count(j => j.State == TaskState.Calculating);
      exp.FinishedCount = jobs.Count(j => j.State == TaskState.Finished);
      return exp;
    }

    public IEnumerable<DT.Job> GetJobs(Expression<Func<Job, bool>> predicate) {
      using (var db = CreateContext()) {
        return db.Jobs.Where(predicate).Select(x => AddStatsToJob(db, DT.Convert.ToDto(x))).ToArray();
      }
    }

    public Guid AddJob(DT.Job dto) {
      using (var db = CreateContext()) {
        var entity = DT.Convert.ToEntity(dto);
        db.Jobs.InsertOnSubmit(entity);
        db.SubmitChanges();
        return entity.JobId;
      }
    }

    public void UpdateJob(DT.Job dto) {
      using (var db = CreateContext()) {
        var entity = db.Jobs.FirstOrDefault(x => x.JobId == dto.Id);
        if (entity == null) db.Jobs.InsertOnSubmit(DT.Convert.ToEntity(dto));
        else DT.Convert.ToEntity(dto, entity);
        db.SubmitChanges();
      }
    }

    public void DeleteJob(Guid id) {
      using (var db = CreateContext()) {
        var entity = db.Jobs.FirstOrDefault(x => x.JobId == id);
        if (entity != null) db.Jobs.DeleteOnSubmit(entity);
        db.SubmitChanges();
      }
    }
    #endregion

    #region JobPermission Methods
    public DT.JobPermission GetJobPermission(Guid jobId, Guid grantedUserId) {
      using (var db = CreateContext()) {
        return DT.Convert.ToDto(db.JobPermissions.SingleOrDefault(x => x.JobId == jobId && x.GrantedUserId == grantedUserId));
      }
    }

    public IEnumerable<DT.JobPermission> GetJobPermissions(Expression<Func<JobPermission, bool>> predicate) {
      using (var db = CreateContext()) {
        return db.JobPermissions.Where(predicate).Select(x => DT.Convert.ToDto(x)).ToArray();
      }
    }

    public void AddJobPermission(DT.JobPermission dto) {
      using (var db = CreateContext()) {
        var entity = DT.Convert.ToEntity(dto);
        db.JobPermissions.InsertOnSubmit(entity);
        db.SubmitChanges();
      }
    }

    public void UpdateJobPermission(DT.JobPermission dto) {
      using (var db = CreateContext()) {
        var entity = db.JobPermissions.FirstOrDefault(x => x.JobId == dto.JobId && x.GrantedUserId == dto.GrantedUserId);
        if (entity == null) db.JobPermissions.InsertOnSubmit(DT.Convert.ToEntity(dto));
        else DT.Convert.ToEntity(dto, entity);
        db.SubmitChanges();
      }
    }

    public void DeleteJobPermission(Guid jobId, Guid grantedUserId) {
      using (var db = CreateContext()) {
        var entity = db.JobPermissions.FirstOrDefault(x => x.JobId == jobId && x.GrantedUserId == grantedUserId);
        if (entity != null) db.JobPermissions.DeleteOnSubmit(entity);
        db.SubmitChanges();
      }
    }

    /// <summary>
    /// Sets the permissions for a experiment. makes sure that only one permission per user exists.
    /// </summary>
    public void SetJobPermission(Guid jobId, Guid grantedByUserId, Guid grantedUserId, Permission permission) {
      using (var db = CreateContext()) {
        JobPermission jobPermission = db.JobPermissions.SingleOrDefault(x => x.JobId == jobId && x.GrantedUserId == grantedUserId);
        if (jobPermission != null) {
          if (permission == Permission.NotAllowed) {
            // not allowed, delete
            db.JobPermissions.DeleteOnSubmit(jobPermission);
          } else {
            // update
            jobPermission.Permission = permission;
            jobPermission.GrantedByUserId = grantedByUserId; // update grantedByUserId, always the last "granter" is stored
          }
        } else {
          // insert
          if (permission != Permission.NotAllowed) {
            jobPermission = new JobPermission() { JobId = jobId, GrantedByUserId = grantedByUserId, GrantedUserId = grantedUserId, Permission = permission };
            db.JobPermissions.InsertOnSubmit(jobPermission);
          }
        }
        db.SubmitChanges();
      }
    }
    #endregion

    #region Plugin Methods
    public DT.Plugin GetPlugin(Guid id) {
      using (var db = CreateContext()) {
        return DT.Convert.ToDto(db.Plugins.SingleOrDefault(x => x.PluginId == id));
      }
    }

    public IEnumerable<DT.Plugin> GetPlugins(Expression<Func<Plugin, bool>> predicate) {
      using (var db = CreateContext()) {
        return db.Plugins.Where(predicate).Select(x => DT.Convert.ToDto(x)).ToArray();
      }
    }

    public Guid AddPlugin(DT.Plugin dto) {
      using (var db = CreateContext()) {
        var entity = DT.Convert.ToEntity(dto);
        db.Plugins.InsertOnSubmit(entity);
        db.SubmitChanges();
        return entity.PluginId;
      }
    }

    public void UpdatePlugin(DT.Plugin dto) {
      using (var db = CreateContext()) {
        var entity = db.Plugins.FirstOrDefault(x => x.PluginId == dto.Id);
        if (entity == null) db.Plugins.InsertOnSubmit(DT.Convert.ToEntity(dto));
        else DT.Convert.ToEntity(dto, entity);
        db.SubmitChanges();
      }
    }

    public void DeletePlugin(Guid id) {
      using (var db = CreateContext()) {
        var entity = db.Plugins.FirstOrDefault(x => x.PluginId == id);
        if (entity != null) db.Plugins.DeleteOnSubmit(entity);
        db.SubmitChanges();
      }
    }
    #endregion

    #region PluginData Methods
    public DT.PluginData GetPluginData(Guid id) {
      using (var db = CreateContext()) {
        return DT.Convert.ToDto(db.PluginDatas.SingleOrDefault(x => x.PluginDataId == id));
      }
    }

    public IEnumerable<DT.PluginData> GetPluginDatas(Expression<Func<PluginData, bool>> predicate) {
      using (var db = CreateContext()) {
        return db.PluginDatas.Where(predicate).Select(x => DT.Convert.ToDto(x)).ToArray();
      }
    }

    public Guid AddPluginData(DT.PluginData dto) {
      using (var db = CreateContext()) {
        var entity = DT.Convert.ToEntity(dto);
        db.PluginDatas.InsertOnSubmit(entity);
        db.SubmitChanges();
        return entity.PluginDataId;
      }
    }

    public void UpdatePluginData(DT.PluginData dto) {
      using (var db = CreateContext()) {
        var entity = db.PluginDatas.FirstOrDefault(x => x.PluginId == dto.PluginId);
        if (entity == null) db.PluginDatas.InsertOnSubmit(DT.Convert.ToEntity(dto));
        else DT.Convert.ToEntity(dto, entity);
        db.SubmitChanges();
      }
    }

    public void DeletePluginData(Guid id) {
      using (var db = CreateContext()) {
        var entity = db.PluginDatas.FirstOrDefault(x => x.PluginDataId == id);
        if (entity != null) db.PluginDatas.DeleteOnSubmit(entity);
        db.SubmitChanges();
      }
    }
    #endregion

    #region Slave Methods
    public DT.Slave GetSlave(Guid id) {
      using (var db = CreateContext()) {
        return DT.Convert.ToDto(db.Resources.OfType<Slave>().SingleOrDefault(x => x.ResourceId == id));
      }
    }

    public IEnumerable<DT.Slave> GetSlaves(Expression<Func<Slave, bool>> predicate) {
      using (var db = CreateContext()) {
        return db.Resources.OfType<Slave>().Where(predicate).Select(x => DT.Convert.ToDto(x)).ToArray();
      }
    }

    public Guid AddSlave(DT.Slave dto) {
      using (var db = CreateContext()) {
        var entity = DT.Convert.ToEntity(dto);
        db.Resources.InsertOnSubmit(entity);
        db.SubmitChanges();
        return entity.ResourceId;
      }
    }

    public void UpdateSlave(DT.Slave dto) {
      using (var db = CreateContext()) {
        var entity = db.Resources.OfType<Slave>().FirstOrDefault(x => x.ResourceId == dto.Id);
        if (entity == null) db.Resources.InsertOnSubmit(DT.Convert.ToEntity(dto));
        else DT.Convert.ToEntity(dto, entity);
        db.SubmitChanges();
      }
    }

    public void DeleteSlave(Guid id) {
      using (var db = CreateContext()) {
        var entity = db.Resources.OfType<Slave>().FirstOrDefault(x => x.ResourceId == id);
        if (entity != null) db.Resources.DeleteOnSubmit(entity);
        db.SubmitChanges();
      }
    }
    #endregion

    #region SlaveGroup Methods
    public DT.SlaveGroup GetSlaveGroup(Guid id) {
      using (var db = CreateContext()) {
        return DT.Convert.ToDto(db.Resources.OfType<SlaveGroup>().SingleOrDefault(x => x.ResourceId == id));
      }
    }

    public IEnumerable<DT.SlaveGroup> GetSlaveGroups(Expression<Func<SlaveGroup, bool>> predicate) {
      using (var db = CreateContext()) {
        return db.Resources.OfType<SlaveGroup>().Where(predicate).Select(x => DT.Convert.ToDto(x)).ToArray();
      }
    }

    public Guid AddSlaveGroup(DT.SlaveGroup dto) {
      using (var db = CreateContext()) {
        if (dto.Id == Guid.Empty)
          dto.Id = Guid.NewGuid();
        var entity = DT.Convert.ToEntity(dto);
        db.Resources.InsertOnSubmit(entity);
        db.SubmitChanges();
        return entity.ResourceId;
      }
    }

    public void UpdateSlaveGroup(DT.SlaveGroup dto) {
      using (var db = CreateContext()) {
        var entity = db.Resources.OfType<SlaveGroup>().FirstOrDefault(x => x.ResourceId == dto.Id);
        if (entity == null) db.Resources.InsertOnSubmit(DT.Convert.ToEntity(dto));
        else DT.Convert.ToEntity(dto, entity);
        db.SubmitChanges();
      }
    }

    public void DeleteSlaveGroup(Guid id) {
      using (var db = CreateContext()) {
        var entity = db.Resources.OfType<SlaveGroup>().FirstOrDefault(x => x.ResourceId == id);
        if (entity != null) {
          if (db.Resources.Where(r => r.ParentResourceId == id).Count() > 0) {
            throw new InvalidOperationException("Cannot delete SlaveGroup as long as there are Slaves in the group");
          }
          db.Resources.DeleteOnSubmit(entity);
        }
        db.SubmitChanges();
      }
    }
    #endregion

    #region Resource Methods
    public DT.Resource GetResource(Guid id) {
      using (var db = CreateContext()) {
        return DT.Convert.ToDto(db.Resources.SingleOrDefault(x => x.ResourceId == id));
      }
    }

    public IEnumerable<DT.Resource> GetResources(Expression<Func<Resource, bool>> predicate) {
      using (var db = CreateContext()) {
        return db.Resources.Where(predicate).Select(x => DT.Convert.ToDto(x)).ToArray();
      }
    }

    public Guid AddResource(DT.Resource dto) {
      using (var db = CreateContext()) {
        var entity = DT.Convert.ToEntity(dto);
        db.Resources.InsertOnSubmit(entity);
        db.SubmitChanges();
        return entity.ResourceId;
      }
    }

    public void UpdateResource(DT.Resource dto) {
      using (var db = CreateContext()) {
        var entity = db.Resources.FirstOrDefault(x => x.ResourceId == dto.Id);
        if (entity == null) db.Resources.InsertOnSubmit(DT.Convert.ToEntity(dto));
        else DT.Convert.ToEntity(dto, entity);
        db.SubmitChanges();
      }
    }

    public void DeleteResource(Guid id) {
      using (var db = CreateContext()) {
        var entity = db.Resources.FirstOrDefault(x => x.ResourceId == id);
        if (entity != null) db.Resources.DeleteOnSubmit(entity);
        db.SubmitChanges();
      }
    }

    public void AssignJobToResource(Guid jobId, Guid resourceId) {
      using (var db = CreateContext()) {
        var job = db.Tasks.Where(x => x.TaskId == jobId).Single();
        job.AssignedResources.Add(new AssignedResource() { TaskId = jobId, ResourceId = resourceId });
        db.SubmitChanges();
      }
    }

    public IEnumerable<DT.Resource> GetAssignedResources(Guid jobId) {
      using (var db = CreateContext()) {
        var job = db.Tasks.Where(x => x.TaskId == jobId).Single();
        return job.AssignedResources.Select(x => DT.Convert.ToDto(x.Resource)).ToArray();
      }
    }

    /// <summary>
    /// Returns all parent resources of a resource (the given resource is also added)
    /// </summary>
    public IEnumerable<DT.Resource> GetParentResources(Guid resourceId) {
      using (var db = CreateContext()) {
        var resources = new List<Resource>();
        CollectParentResources(resources, db.Resources.Where(r => r.ResourceId == resourceId).Single());
        return resources.Select(r => DT.Convert.ToDto(r)).ToArray();
      }
    }

    private void CollectParentResources(List<Resource> resources, Resource resource) {
      if (resource == null) return;
      resources.Add(resource);
      CollectParentResources(resources, resource.ParentResource);
    }

    /// <summary>
    /// Returns all child resources of a resource (without the given resource)
    /// </summary>
    public IEnumerable<DT.Resource> GetChildResources(Guid resourceId) {
      using (var db = CreateContext()) {
        return CollectChildResources(resourceId, db);
      }
    }

    public IEnumerable<DT.Resource> CollectChildResources(Guid resourceId, HiveDataContext db) {
      var childs = new List<DT.Resource>();
      foreach (var child in db.Resources.Where(x => x.ParentResourceId == resourceId)) {
        childs.Add(DT.Convert.ToDto(child));
        childs.AddRange(CollectChildResources(child.ResourceId, db));
      }
      return childs;
    }

    public IEnumerable<DT.Task> GetJobsByResourceId(Guid resourceId) {
      using (var db = CreateContext()) {
        var resources = GetChildResources(resourceId).Select(x => x.Id).ToList();
        resources.Add(resourceId);

        var jobs = db.Tasks.Where(j =>
          j.State == TaskState.Calculating &&
          j.StateLogs.OrderByDescending(x => x.DateTime).First().SlaveId.HasValue &&
          resources.Contains(j.StateLogs.OrderByDescending(x => x.DateTime).First().SlaveId.Value));
        return jobs.Select(j => DT.Convert.ToDto(j)).ToArray();
      }
    }
    #endregion

    #region ResourcePermission Methods
    public DT.ResourcePermission GetResourcePermission(Guid resourceId, Guid grantedUserId) {
      using (var db = CreateContext()) {
        return DT.Convert.ToDto(db.ResourcePermissions.SingleOrDefault(x => x.ResourceId == resourceId && x.GrantedUserId == grantedUserId));
      }
    }

    public IEnumerable<DT.ResourcePermission> GetResourcePermissions(Expression<Func<ResourcePermission, bool>> predicate) {
      using (var db = CreateContext()) {
        return db.ResourcePermissions.Where(predicate).Select(x => DT.Convert.ToDto(x)).ToArray();
      }
    }

    public void AddResourcePermission(DT.ResourcePermission dto) {
      using (var db = CreateContext()) {
        var entity = db.ResourcePermissions.SingleOrDefault(x => x.ResourceId == dto.ResourceId && x.GrantedUserId == dto.GrantedUserId);
        if (entity == null) { db.ResourcePermissions.InsertOnSubmit(DT.Convert.ToEntity(dto)); db.SubmitChanges(); }
      }
    }

    public void UpdateResourcePermission(DT.ResourcePermission dto) {
      using (var db = CreateContext()) {
        var entity = db.ResourcePermissions.FirstOrDefault(x => x.ResourceId == dto.ResourceId && x.GrantedUserId == dto.GrantedUserId);
        if (entity == null) db.ResourcePermissions.InsertOnSubmit(DT.Convert.ToEntity(dto));
        else DT.Convert.ToEntity(dto, entity);
        db.SubmitChanges();
      }
    }

    public void DeleteResourcePermission(Guid resourceId, Guid grantedUserId) {
      using (var db = CreateContext()) {
        var entity = db.ResourcePermissions.FirstOrDefault(x => x.ResourceId == resourceId && x.GrantedUserId == grantedUserId);
        if (entity != null) db.ResourcePermissions.DeleteOnSubmit(entity);
        db.SubmitChanges();
      }
    }
    #endregion

    #region Authorization Methods
    public Permission GetPermissionForTask(Guid taskId, Guid userId) {
      using (var db = CreateContext()) {
        return GetPermissionForJob(GetJobForTask(taskId), userId);
      }
    }

    public Permission GetPermissionForJob(Guid jobId, Guid userId) {
      using (var db = CreateContext()) {
        Job job = db.Jobs.SingleOrDefault(x => x.JobId == jobId);
        if (job == null) return Permission.NotAllowed;
        if (job.OwnerUserId == userId) return Permission.Full;
        JobPermission permission = db.JobPermissions.SingleOrDefault(p => p.JobId == jobId && p.GrantedUserId == userId);
        return permission != null ? permission.Permission : Permission.NotAllowed;
      }
    }

    public Guid GetJobForTask(Guid taskId) {
      using (var db = CreateContext()) {
        return db.Tasks.Single(j => j.TaskId == taskId).JobId;
      }
    }
    #endregion

    #region Lifecycle Methods
    public DateTime GetLastCleanup() {
      using (var db = CreateContext()) {
        var entity = db.Lifecycles.SingleOrDefault();
        return entity != null ? entity.LastCleanup : DateTime.MinValue;
      }
    }

    public void SetLastCleanup(DateTime datetime) {
      using (var db = CreateContext()) {
        var entity = db.Lifecycles.SingleOrDefault();
        if (entity != null) {
          entity.LastCleanup = datetime;
        } else {
          entity = new Lifecycle();
          entity.LifecycleId = 0; // always only one entry with ID:0
          entity.LastCleanup = datetime;
          db.Lifecycles.InsertOnSubmit(entity);
        }
        db.SubmitChanges();
      }
    }
    #endregion

    #region Downtime Methods
    public DT.Downtime GetDowntime(Guid id) {
      using (var db = CreateContext()) {
        return DT.Convert.ToDto(db.Downtimes.SingleOrDefault(x => x.DowntimeId == id));
      }
    }

    public IEnumerable<DT.Downtime> GetDowntimes(Expression<Func<Downtime, bool>> predicate) {
      using (var db = CreateContext()) {
        return db.Downtimes.Where(predicate).Select(x => DT.Convert.ToDto(x)).ToArray();
      }
    }

    public Guid AddDowntime(DT.Downtime dto) {
      using (var db = CreateContext()) {
        var entity = DT.Convert.ToEntity(dto);
        db.Downtimes.InsertOnSubmit(entity);
        db.SubmitChanges();
        return entity.DowntimeId;
      }
    }

    public void UpdateDowntime(DT.Downtime dto) {
      using (var db = CreateContext()) {
        var entity = db.Downtimes.FirstOrDefault(x => x.DowntimeId == dto.Id);
        if (entity == null) db.Downtimes.InsertOnSubmit(DT.Convert.ToEntity(dto));
        else DT.Convert.ToEntity(dto, entity);
        db.SubmitChanges();
      }
    }

    public void DeleteDowntime(Guid id) {
      using (var db = CreateContext()) {
        var entity = db.Downtimes.FirstOrDefault(x => x.DowntimeId == id);
        if (entity != null) db.Downtimes.DeleteOnSubmit(entity);
        db.SubmitChanges();
      }
    }
    #endregion

    #region Statistics Methods
    public DT.Statistics GetStatistic(Guid id) {
      using (var db = CreateContext()) {
        return DT.Convert.ToDto(db.Statistics.SingleOrDefault(x => x.StatisticsId == id));
      }
    }

    public IEnumerable<DT.Statistics> GetStatistics(Expression<Func<Statistics, bool>> predicate) {
      using (var db = CreateContext()) {
        return db.Statistics.Where(predicate).Select(x => DT.Convert.ToDto(x)).ToArray();
      }
    }

    public Guid AddStatistics(DT.Statistics dto) {
      using (var db = CreateContext()) {
        var entity = DT.Convert.ToEntity(dto);
        db.Statistics.InsertOnSubmit(entity);
        db.SubmitChanges();
        foreach (var slaveStat in dto.SlaveStatistics) {
          slaveStat.Id = entity.StatisticsId;
          db.SlaveStatistics.InsertOnSubmit(DT.Convert.ToEntity(slaveStat));
        }
        if (dto.UserStatistics != null) {
          foreach (var userStat in dto.UserStatistics) {
            userStat.Id = entity.StatisticsId;
            db.UserStatistics.InsertOnSubmit(DT.Convert.ToEntity(userStat));
          }
        }
        db.SubmitChanges();
        return entity.StatisticsId;
      }
    }

    public void DeleteStatistics(Guid id) {
      using (var db = CreateContext()) {
        var entity = db.Statistics.FirstOrDefault(x => x.StatisticsId == id);
        if (entity != null) db.Statistics.DeleteOnSubmit(entity);
        db.SubmitChanges();
      }
    }

    public Dictionary<Guid, int> GetWaitingTasksByUser() {
      using (var db = CreateContext()) {
        var waitingTasksByUser = from task in db.Tasks
                                 where task.State == TaskState.Waiting
                                 group task by task.Job.OwnerUserId into g
                                 select new { UserId = g.Key, UsedCores = g.Count() };
        return waitingTasksByUser.ToDictionary(x => x.UserId, x => x.UsedCores);
      }
    }

    public Dictionary<Guid, int> GetWaitingTasksByUserForResources(List<Guid> resourceIds) {
      using (var db = CreateContext()) {
        var waitingTasksByUser = from task in db.Tasks
                                 where task.State == TaskState.Waiting && task.AssignedResources.Any(x => resourceIds.Contains(x.ResourceId))
                                 group task by task.Job.OwnerUserId into g
                                 select new { UserId = g.Key, UsedCores = g.Count() };
        return waitingTasksByUser.ToDictionary(x => x.UserId, x => x.UsedCores);
      }
    }

    public Dictionary<Guid, int> GetCalculatingTasksByUser() {
      using (var db = CreateContext()) {
        var calculatingTasksByUser = from task in db.Tasks
                                     where task.State == TaskState.Calculating
                                     group task by task.Job.OwnerUserId into g
                                     select new { UserId = g.Key, UsedCores = g.Count() };
        return calculatingTasksByUser.ToDictionary(x => x.UserId, x => x.UsedCores);
      }
    }

    public Dictionary<Guid, int> GetCalculatingTasksByUserForResources(List<Guid> resourceIds) {
      using (var db = CreateContext()) {
        var calculatingTasksByUser = from task in db.Tasks
                                     where task.State == TaskState.Calculating && task.AssignedResources.Any(x => resourceIds.Contains(x.ResourceId))
                                     group task by task.Job.OwnerUserId into g
                                     select new { UserId = g.Key, UsedCores = g.Count() };
        return calculatingTasksByUser.ToDictionary(x => x.UserId, x => x.UsedCores);
      }
    }

    public List<DT.UserStatistics> GetUserStatistics() {
      using (var db = CreateContext()) {
        var userStats = new Dictionary<Guid, DT.UserStatistics>();

        var usedCoresByUser = from job in db.Tasks
                              where job.State == TaskState.Calculating
                              group job by job.Job.OwnerUserId into g
                              select new { UserId = g.Key, UsedCores = g.Count() };

        foreach (var item in usedCoresByUser) {
          if (!userStats.ContainsKey(item.UserId)) {
            userStats.Add(item.UserId, new DT.UserStatistics() { UserId = item.UserId });
          }
          userStats[item.UserId].UsedCores += item.UsedCores;
        }

        var executionTimesByUser = from task in db.Tasks
                                   group task by task.Job.OwnerUserId into g
                                   select new { UserId = g.Key, ExecutionTime = TimeSpan.FromMilliseconds(g.Select(x => x.ExecutionTimeMs).Sum()) };
        foreach (var item in executionTimesByUser) {
          if (!userStats.ContainsKey(item.UserId)) {
            userStats.Add(item.UserId, new DT.UserStatistics() { UserId = item.UserId });
          }
          userStats[item.UserId].ExecutionTime += item.ExecutionTime;
        }

        // execution times only of finished task - necessary to compute efficieny
        var executionTimesFinishedJobs = from job in db.Tasks
                                         where job.State == TaskState.Finished
                                         group job by job.Job.OwnerUserId into g
                                         select new { UserId = g.Key, ExecutionTimeFinishedJobs = TimeSpan.FromMilliseconds(g.Select(x => x.ExecutionTimeMs).Sum()) };

        foreach (var item in executionTimesFinishedJobs) {
          if (!userStats.ContainsKey(item.UserId)) {
            userStats.Add(item.UserId, new DT.UserStatistics() { UserId = item.UserId });
          }
          userStats[item.UserId].ExecutionTimeFinishedJobs += item.ExecutionTimeFinishedJobs;
        }

        // start to end times only of finished task - necessary to compute efficiency
        var startToEndTimesFinishedJobs = from job in db.Tasks
                                          where job.State == TaskState.Finished
                                          group job by job.Job.OwnerUserId into g
                                          select new {
                                            UserId = g.Key,
                                            StartToEndTime = new TimeSpan(g.Select(x => x.StateLogs.OrderByDescending(sl => sl.DateTime).First().DateTime - x.StateLogs.OrderBy(sl => sl.DateTime).First().DateTime).Sum(ts => ts.Ticks))
                                          };
        foreach (var item in startToEndTimesFinishedJobs) {
          if (!userStats.ContainsKey(item.UserId)) {
            userStats.Add(item.UserId, new DT.UserStatistics() { UserId = item.UserId });
          }
          userStats[item.UserId].StartToEndTime += item.StartToEndTime;
        }

        // also consider executiontimes of DeletedJobStats 
        var deletedJobsExecutionTimesByUsers = from del in db.DeletedJobStatistics
                                               group del by del.UserId into g
                                               select new {
                                                 UserId = g.Key,
                                                 ExecutionTime = TimeSpan.FromSeconds(g.Select(x => x.ExecutionTimeS).Sum()),
                                                 ExecutionTimeFinishedJobs = TimeSpan.FromSeconds(g.Select(x => x.ExecutionTimeSFinishedJobs).Sum()),
                                                 StartToEndTime = TimeSpan.FromSeconds(g.Select(x => x.StartToEndTimeS).Sum())
                                               };
        foreach (var item in deletedJobsExecutionTimesByUsers) {
          if (!userStats.ContainsKey(item.UserId)) {
            userStats.Add(item.UserId, new DT.UserStatistics() { UserId = item.UserId });
          }
          userStats[item.UserId].ExecutionTime += item.ExecutionTime;
          userStats[item.UserId].ExecutionTimeFinishedJobs += item.ExecutionTimeFinishedJobs;
          userStats[item.UserId].StartToEndTime += item.StartToEndTime;
        }

        return userStats.Values.ToList();
      }
    }
    #endregion

    #region Helpers
    private void CollectChildTasks(HiveDataContext db, Guid parentTaskId, List<Task> collection) {
      var tasks = db.Tasks.Where(j => j.ParentTaskId == parentTaskId);
      foreach (var task in tasks) {
        collection.Add(task);
        if (task.IsParentTask)
          CollectChildTasks(db, task.TaskId, collection);
      }
    }
    #endregion
  }
}
