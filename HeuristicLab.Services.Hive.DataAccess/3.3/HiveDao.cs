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
using System.Linq;
using System.Linq.Expressions;
using HeuristicLab.Services.Hive.Common.DataTransfer;
using DT = HeuristicLab.Services.Hive.Common.DataTransfer;

namespace HeuristicLab.Services.Hive.DataAccess {
  public class HiveDao : IHiveDao {
    public static HiveDataContext CreateContext(bool longRunning = false) {
      var context = new HiveDataContext(Settings.Default.HeuristicLab_Hive_LinqConnectionString);
      if (longRunning) context.CommandTimeout = (int)Settings.Default.LongRunningDatabaseCommandTimeout.TotalSeconds;
      return context;
    }

    public HiveDao() { }

    #region Job Methods
    public DT.Job GetJob(Guid id) {
      using (var db = CreateContext()) {
        return Convert.ToDto(db.Jobs.SingleOrDefault(x => x.JobId == id));
      }
    }

    public IEnumerable<DT.Job> GetJobs(Expression<Func<Job, bool>> predicate) {
      using (var db = CreateContext()) {
        return db.Jobs.Where(predicate).Select(x => Convert.ToDto(x)).ToArray();
      }
    }

    public Guid AddJob(DT.Job dto) {
      using (var db = CreateContext()) {
        var entity = Convert.ToEntity(dto);
        db.Jobs.InsertOnSubmit(entity);
        db.SubmitChanges();
        foreach (Guid pluginId in dto.PluginsNeededIds) {
          db.RequiredPlugins.InsertOnSubmit(new RequiredPlugin() { JobId = entity.JobId, PluginId = pluginId });
        }
        db.SubmitChanges();
        return entity.JobId;
      }
    }

    public void UpdateJob(DT.Job dto) {
      using (var db = CreateContext()) {
        var entity = db.Jobs.FirstOrDefault(x => x.JobId == dto.Id);
        if (entity == null) db.Jobs.InsertOnSubmit(Convert.ToEntity(dto));
        else Convert.ToEntity(dto, entity);
        foreach (Guid pluginId in dto.PluginsNeededIds) {
          if (db.RequiredPlugins.Count(p => p.PluginId == pluginId) == 0) {
            db.RequiredPlugins.InsertOnSubmit(new RequiredPlugin() { JobId = entity.JobId, PluginId = pluginId });
          }
        }
        db.SubmitChanges();
      }
    }

    public void DeleteJob(Guid id) {
      using (var db = CreateContext()) {
        var entity = db.Jobs.FirstOrDefault(x => x.JobId == id);
        if (entity != null) db.Jobs.DeleteOnSubmit(entity);
        db.SubmitChanges(); // JobData and child jobs are deleted by db-trigger
      }
    }

    /// <summary>
    /// returns all parent jobs which are waiting for their child jobs to finish
    /// </summary>
    /// <param name="resourceIds">list of resourceids which for which the jobs should be valid</param>
    /// <param name="count">maximum number of jobs to return</param>
    /// <param name="finished">if true, all parent jobs which have FinishWhenChildJobsFinished=true are returned, otherwise only FinishWhenChildJobsFinished=false are returned</param>
    /// <returns></returns>
    public IEnumerable<DT.Job> GetParentJobs(IEnumerable<Guid> resourceIds, int count, bool finished) {
      using (var db = CreateContext()) {
        var query = from ar in db.AssignedResources
                    where resourceIds.Contains(ar.ResourceId)
                       && ar.Job.State == JobState.Waiting
                       && ar.Job.IsParentJob
                       && (finished ? ar.Job.FinishWhenChildJobsFinished : !ar.Job.FinishWhenChildJobsFinished)
                       && (from child in db.Jobs
                           where child.ParentJobId == ar.Job.JobId
                           select child.State == JobState.Finished
                               || child.State == JobState.Aborted
                               || child.State == JobState.Failed).All(x => x)
                       && (from child in db.Jobs // avoid returning WaitForChildJobs jobs where no child-jobs exist (yet)
                           where child.ParentJobId == ar.Job.JobId
                           select child).Count() > 0
                    orderby ar.Job.Priority descending, db.Random()
                    select Convert.ToDto(ar.Job);
        return count == 0 ? query.ToArray() : query.Take(count).ToArray();
      }
    }

    public IEnumerable<DT.Job> GetWaitingJobs(DT.Slave slave, int count) {
      using (var db = CreateContext()) {
        var resourceIds = GetParentResources(slave.Id).Select(r => r.Id);
        var waitingParentJobs = GetParentJobs(resourceIds, count, false);
        if (count > 0 && waitingParentJobs.Count() >= count) return waitingParentJobs.Take(count).ToArray();

        var query = from ar in db.AssignedResources
                    where resourceIds.Contains(ar.ResourceId)
                       && !(ar.Job.IsParentJob && ar.Job.FinishWhenChildJobsFinished)
                       && ar.Job.State == JobState.Waiting
                       && ar.Job.CoresNeeded <= slave.FreeCores
                       && ar.Job.MemoryNeeded <= slave.FreeMemory
                    orderby ar.Job.Priority descending, db.Random() // take random job to avoid the race condition that occurs when this method is called concurrently (the same job would be returned)
                    select Convert.ToDto(ar.Job);
        var waitingJobs = (count == 0 ? query : query.Take(count)).ToArray();
        return waitingJobs.Union(waitingParentJobs).OrderByDescending(x => x.Priority);
      }
    }

    public DT.Job UpdateJobState(Guid jobId, JobState jobState, Guid? slaveId, Guid? userId, string exception) {
      using (var db = CreateContext()) {
        var job = db.Jobs.SingleOrDefault(x => x.JobId == jobId);
        job.State = jobState;
        db.StateLogs.InsertOnSubmit(new StateLog {
          JobId = jobId,
          State = jobState,
          SlaveId = slaveId,
          UserId = userId,
          Exception = exception,
          DateTime = DateTime.Now
        });
        db.SubmitChanges();
        job = db.Jobs.SingleOrDefault(x => x.JobId == jobId);
        return Convert.ToDto(job);
      }
    }
    #endregion

    #region JobData Methods
    public DT.JobData GetJobData(Guid id) {
      using (var db = CreateContext(true)) {
        return Convert.ToDto(db.JobDatas.SingleOrDefault(x => x.JobId == id));
      }
    }

    public IEnumerable<DT.JobData> GetJobDatas(Expression<Func<JobData, bool>> predicate) {
      using (var db = CreateContext(true)) {
        return db.JobDatas.Where(predicate).Select(x => Convert.ToDto(x)).ToArray();
      }
    }

    public Guid AddJobData(DT.JobData dto) {
      using (var db = CreateContext(true)) {
        var entity = Convert.ToEntity(dto);
        db.JobDatas.InsertOnSubmit(entity);
        db.SubmitChanges();
        return entity.JobId;
      }
    }

    public void UpdateJobData(DT.JobData dto) {
      using (var db = CreateContext(true)) {
        var entity = db.JobDatas.FirstOrDefault(x => x.JobId == dto.JobId);
        if (entity == null) db.JobDatas.InsertOnSubmit(Convert.ToEntity(dto));
        else Convert.ToEntity(dto, entity);
        db.SubmitChanges();
      }
    }

    public void DeleteJobData(Guid id) {
      using (var db = CreateContext()) {
        var entity = db.JobDatas.FirstOrDefault(x => x.JobId == id); // check if all the byte[] is loaded into memory here. otherwise work around to delete without loading it
        if (entity != null) db.JobDatas.DeleteOnSubmit(entity);
        db.SubmitChanges();
      }
    }
    #endregion

    #region StateLog Methods
    public DT.StateLog GetStateLog(Guid id) {
      using (var db = CreateContext()) {
        return Convert.ToDto(db.StateLogs.SingleOrDefault(x => x.StateLogId == id));
      }
    }

    public IEnumerable<DT.StateLog> GetStateLogs(Expression<Func<StateLog, bool>> predicate) {
      using (var db = CreateContext()) {
        return db.StateLogs.Where(predicate).Select(x => Convert.ToDto(x)).ToArray();
      }
    }

    public Guid AddStateLog(DT.StateLog dto) {
      using (var db = CreateContext()) {
        var entity = Convert.ToEntity(dto);
        db.StateLogs.InsertOnSubmit(entity);
        db.SubmitChanges();
        return entity.StateLogId;
      }
    }

    public void UpdateStateLog(DT.StateLog dto) {
      using (var db = CreateContext()) {
        var entity = db.StateLogs.FirstOrDefault(x => x.StateLogId == dto.Id);
        if (entity == null) db.StateLogs.InsertOnSubmit(Convert.ToEntity(dto));
        else Convert.ToEntity(dto, entity);
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

    #region HiveExperiment Methods
    public DT.HiveExperiment GetHiveExperiment(Guid id) {
      using (var db = CreateContext()) {
        return AddStatsToExperiment(db, Convert.ToDto(db.HiveExperiments.SingleOrDefault(x => x.HiveExperimentId == id)));
      }
    }

    private DT.HiveExperiment AddStatsToExperiment(HiveDataContext db, DT.HiveExperiment exp) {
      if (exp == null)
        return null;

      var jobs = db.Jobs.Where(j => j.HiveExperimentId == exp.Id);
      exp.JobCount = jobs.Count();
      exp.CalculatingCount = jobs.Count(j => j.State == JobState.Calculating);
      exp.FinishedCount = jobs.Count(j => j.State == JobState.Finished);
      return exp;
    }

    public IEnumerable<DT.HiveExperiment> GetHiveExperiments(Expression<Func<HiveExperiment, bool>> predicate) {
      using (var db = CreateContext()) {
        return db.HiveExperiments.Where(predicate).Select(x => AddStatsToExperiment(db, Convert.ToDto(x))).ToArray();
      }
    }

    public Guid AddHiveExperiment(DT.HiveExperiment dto) {
      using (var db = CreateContext()) {
        var entity = Convert.ToEntity(dto);
        db.HiveExperiments.InsertOnSubmit(entity);
        db.SubmitChanges();
        return entity.HiveExperimentId;
      }
    }

    public void UpdateHiveExperiment(DT.HiveExperiment dto) {
      using (var db = CreateContext()) {
        var entity = db.HiveExperiments.FirstOrDefault(x => x.HiveExperimentId == dto.Id);
        if (entity == null) db.HiveExperiments.InsertOnSubmit(Convert.ToEntity(dto));
        else Convert.ToEntity(dto, entity);
        db.SubmitChanges();
      }
    }

    public void DeleteHiveExperiment(Guid id) {
      using (var db = CreateContext()) {
        var entity = db.HiveExperiments.FirstOrDefault(x => x.HiveExperimentId == id);
        if (entity != null) db.HiveExperiments.DeleteOnSubmit(entity);
        db.SubmitChanges();
      }
    }
    #endregion

    #region HiveExperimentPermission Methods
    public DT.HiveExperimentPermission GetHiveExperimentPermission(Guid hiveExperimentId, Guid grantedUserId) {
      using (var db = CreateContext()) {
        return Convert.ToDto(db.HiveExperimentPermissions.SingleOrDefault(x => x.HiveExperimentId == hiveExperimentId && x.GrantedUserId == grantedUserId));
      }
    }

    public IEnumerable<DT.HiveExperimentPermission> GetHiveExperimentPermissions(Expression<Func<HiveExperimentPermission, bool>> predicate) {
      using (var db = CreateContext()) {
        return db.HiveExperimentPermissions.Where(predicate).Select(x => Convert.ToDto(x)).ToArray();
      }
    }

    public void AddHiveExperimentPermission(DT.HiveExperimentPermission dto) {
      using (var db = CreateContext()) {
        var entity = Convert.ToEntity(dto);
        db.HiveExperimentPermissions.InsertOnSubmit(entity);
        db.SubmitChanges();
      }
    }

    public void UpdateHiveExperimentPermission(DT.HiveExperimentPermission dto) {
      using (var db = CreateContext()) {
        var entity = db.HiveExperimentPermissions.FirstOrDefault(x => x.HiveExperimentId == dto.HiveExperimentId && x.GrantedUserId == dto.GrantedUserId);
        if (entity == null) db.HiveExperimentPermissions.InsertOnSubmit(Convert.ToEntity(dto));
        else Convert.ToEntity(dto, entity);
        db.SubmitChanges();
      }
    }

    public void DeleteHiveExperimentPermission(Guid hiveExperimentId, Guid grantedUserId) {
      using (var db = CreateContext()) {
        var entity = db.HiveExperimentPermissions.FirstOrDefault(x => x.HiveExperimentId == hiveExperimentId && x.GrantedUserId == grantedUserId);
        if (entity != null) db.HiveExperimentPermissions.DeleteOnSubmit(entity);
        db.SubmitChanges();
      }
    }

    /// <summary>
    /// Sets the permissions for a experiment. makes sure that only one permission per user exists.
    /// </summary>
    public void SetHiveExperimentPermission(Guid hiveExperimentId, Guid grantedByUserId, Guid grantedUserId, Permission permission) {
      using (var db = CreateContext()) {
        HiveExperimentPermission hiveExperimentPermission = db.HiveExperimentPermissions.SingleOrDefault(x => x.HiveExperimentId == hiveExperimentId && x.GrantedUserId == grantedUserId);
        if (hiveExperimentPermission != null) {
          if (permission == Permission.NotAllowed) {
            // not allowed, delete
            db.HiveExperimentPermissions.DeleteOnSubmit(hiveExperimentPermission);
          } else {
            // update
            hiveExperimentPermission.Permission = permission;
            hiveExperimentPermission.GrantedByUserId = grantedByUserId; // update grantedByUserId, always the last "granter" is stored
          }
        } else {
          // insert
          if (permission != Permission.NotAllowed) {
            hiveExperimentPermission = new HiveExperimentPermission() { HiveExperimentId = hiveExperimentId, GrantedByUserId = grantedByUserId, GrantedUserId = grantedUserId, Permission = permission };
            db.HiveExperimentPermissions.InsertOnSubmit(hiveExperimentPermission);
          }
        }
        db.SubmitChanges();
      }
    }
    #endregion

    #region Plugin Methods
    public DT.Plugin GetPlugin(Guid id) {
      using (var db = CreateContext()) {
        return Convert.ToDto(db.Plugins.SingleOrDefault(x => x.PluginId == id));
      }
    }

    public IEnumerable<DT.Plugin> GetPlugins(Expression<Func<Plugin, bool>> predicate) {
      using (var db = CreateContext()) {
        return db.Plugins.Where(predicate).Select(x => Convert.ToDto(x)).ToArray();
      }
    }

    public Guid AddPlugin(DT.Plugin dto) {
      using (var db = CreateContext()) {
        var entity = Convert.ToEntity(dto);
        db.Plugins.InsertOnSubmit(entity);
        db.SubmitChanges();
        return entity.PluginId;
      }
    }

    public void UpdatePlugin(DT.Plugin dto) {
      using (var db = CreateContext()) {
        var entity = db.Plugins.FirstOrDefault(x => x.PluginId == dto.Id);
        if (entity == null) db.Plugins.InsertOnSubmit(Convert.ToEntity(dto));
        else Convert.ToEntity(dto, entity);
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
        return Convert.ToDto(db.PluginDatas.SingleOrDefault(x => x.PluginDataId == id));
      }
    }

    public IEnumerable<DT.PluginData> GetPluginDatas(Expression<Func<PluginData, bool>> predicate) {
      using (var db = CreateContext()) {
        return db.PluginDatas.Where(predicate).Select(x => Convert.ToDto(x)).ToArray();
      }
    }

    public Guid AddPluginData(DT.PluginData dto) {
      using (var db = CreateContext()) {
        var entity = Convert.ToEntity(dto);
        db.PluginDatas.InsertOnSubmit(entity);
        db.SubmitChanges();
        return entity.PluginDataId;
      }
    }

    public void UpdatePluginData(DT.PluginData dto) {
      using (var db = CreateContext()) {
        var entity = db.PluginDatas.FirstOrDefault(x => x.PluginId == dto.PluginId);
        if (entity == null) db.PluginDatas.InsertOnSubmit(Convert.ToEntity(dto));
        else Convert.ToEntity(dto, entity);
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
        return Convert.ToDto(db.Resources.OfType<Slave>().SingleOrDefault(x => x.ResourceId == id));
      }
    }

    public IEnumerable<DT.Slave> GetSlaves(Expression<Func<Slave, bool>> predicate) {
      using (var db = CreateContext()) {
        return db.Resources.OfType<Slave>().Where(predicate).Select(x => Convert.ToDto(x)).ToArray();
      }
    }

    public Guid AddSlave(DT.Slave dto) {
      using (var db = CreateContext()) {
        var entity = Convert.ToEntity(dto);
        db.Resources.InsertOnSubmit(entity);
        db.SubmitChanges();
        return entity.ResourceId;
      }
    }

    public void UpdateSlave(DT.Slave dto) {
      using (var db = CreateContext()) {
        var entity = db.Resources.OfType<Slave>().FirstOrDefault(x => x.ResourceId == dto.Id);
        if (entity == null) db.Resources.InsertOnSubmit(Convert.ToEntity(dto));
        else Convert.ToEntity(dto, entity);
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
        return Convert.ToDto(db.Resources.OfType<SlaveGroup>().SingleOrDefault(x => x.ResourceId == id));
      }
    }

    public IEnumerable<DT.SlaveGroup> GetSlaveGroups(Expression<Func<SlaveGroup, bool>> predicate) {
      using (var db = CreateContext()) {
        return db.Resources.OfType<SlaveGroup>().Where(predicate).Select(x => Convert.ToDto(x)).ToArray();
      }
    }

    public Guid AddSlaveGroup(DT.SlaveGroup dto) {
      using (var db = CreateContext()) {
        if (dto.Id == Guid.Empty)
          dto.Id = Guid.NewGuid();
        var entity = Convert.ToEntity(dto);
        db.Resources.InsertOnSubmit(entity);
        db.SubmitChanges();
        return entity.ResourceId;
      }
    }

    public void UpdateSlaveGroup(DT.SlaveGroup dto) {
      using (var db = CreateContext()) {
        var entity = db.Resources.OfType<SlaveGroup>().FirstOrDefault(x => x.ResourceId == dto.Id);
        if (entity == null) db.Resources.InsertOnSubmit(Convert.ToEntity(dto));
        else Convert.ToEntity(dto, entity);
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
        return Convert.ToDto(db.Resources.SingleOrDefault(x => x.ResourceId == id));
      }
    }

    public IEnumerable<DT.Resource> GetResources(Expression<Func<Resource, bool>> predicate) {
      using (var db = CreateContext()) {
        return db.Resources.Where(predicate).Select(x => Convert.ToDto(x)).ToArray();
      }
    }

    public Guid AddResource(DT.Resource dto) {
      using (var db = CreateContext()) {
        var entity = Convert.ToEntity(dto);
        db.Resources.InsertOnSubmit(entity);
        db.SubmitChanges();
        return entity.ResourceId;
      }
    }

    public void UpdateResource(DT.Resource dto) {
      using (var db = CreateContext()) {
        var entity = db.Resources.FirstOrDefault(x => x.ResourceId == dto.Id);
        if (entity == null) db.Resources.InsertOnSubmit(Convert.ToEntity(dto));
        else Convert.ToEntity(dto, entity);
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
        var job = db.Jobs.Where(x => x.JobId == jobId).Single();
        job.AssignedResources.Add(new AssignedResource() { JobId = jobId, ResourceId = resourceId });
        db.SubmitChanges();
      }
    }

    public IEnumerable<DT.Resource> GetAssignedResources(Guid jobId) {
      using (var db = CreateContext()) {
        var job = db.Jobs.Where(x => x.JobId == jobId).Single();
        return job.AssignedResources.Select(x => Convert.ToDto(x.Resource)).ToArray();
      }
    }

    /// <summary>
    /// Returns all parent resources of a resource (the given resource is also added)
    /// </summary>
    public IEnumerable<DT.Resource> GetParentResources(Guid resourceId) {
      using (var db = CreateContext()) {
        var resources = new List<Resource>();
        CollectParentResources(resources, db.Resources.Where(r => r.ResourceId == resourceId).Single());
        return resources.Select(r => Convert.ToDto(r)).ToArray();
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
        var childs = new List<DT.Resource>();
        foreach (var child in db.Resources.Where(x => x.ParentResourceId == resourceId)) {
          childs.Add(Convert.ToDto(child));
          childs.AddRange(GetChildResources(child.ResourceId));
        }
        return childs;
      }
    }

    public IEnumerable<DT.Job> GetJobsByResourceId(Guid resourceId) {
      using (var db = CreateContext()) {
        var resources = GetChildResources(resourceId).Select(x => x.Id).ToList();
        resources.Add(resourceId);

        var jobs = db.Jobs.Where(j =>
          j.State == JobState.Calculating &&
          j.StateLogs.OrderByDescending(x => x.DateTime).First().SlaveId.HasValue &&
          resources.Contains(j.StateLogs.OrderByDescending(x => x.DateTime).First().SlaveId.Value));
        return jobs.Select(j => Convert.ToDto(j)).ToArray();
      }
    }
    #endregion

    #region Authorization Methods
    public Permission GetPermissionForJob(Guid jobId, Guid userId) {
      using (var db = CreateContext()) {
        return GetPermissionForExperiment(GetExperimentForJob(jobId), userId);
      }
    }

    public Permission GetPermissionForExperiment(Guid experimentId, Guid userId) {
      using (var db = CreateContext()) {
        HiveExperiment hiveExperiment = db.HiveExperiments.SingleOrDefault(x => x.HiveExperimentId == experimentId);
        if (hiveExperiment == null) return Permission.NotAllowed;
        if (hiveExperiment.OwnerUserId == userId) return Permission.Full;
        HiveExperimentPermission permission = db.HiveExperimentPermissions.SingleOrDefault(p => p.HiveExperimentId == experimentId && p.GrantedUserId == userId);
        return permission != null ? permission.Permission : Permission.NotAllowed;
      }
    }

    public Guid GetExperimentForJob(Guid jobId) {
      using (var db = CreateContext()) {
        return db.Jobs.Single(j => j.JobId == jobId).HiveExperimentId;
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
        return Convert.ToDto(db.Downtimes.SingleOrDefault(x => x.DowntimeId == id));
      }
    }

    public IEnumerable<DT.Downtime> GetDowntimes(Expression<Func<Downtime, bool>> predicate) {
      using (var db = CreateContext()) {
        return db.Downtimes.Where(predicate).Select(x => Convert.ToDto(x)).ToArray();
      }
    }

    public Guid AddDowntime(DT.Downtime dto) {
      using (var db = CreateContext()) {
        var entity = Convert.ToEntity(dto);
        db.Downtimes.InsertOnSubmit(entity);
        db.SubmitChanges();
        return entity.DowntimeId;
      }
    }

    public void UpdateDowntime(DT.Downtime dto) {
      using (var db = CreateContext()) {
        var entity = db.Downtimes.FirstOrDefault(x => x.DowntimeId == dto.Id);
        if (entity == null) db.Downtimes.InsertOnSubmit(Convert.ToEntity(dto));
        else Convert.ToEntity(dto, entity);
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
        return Convert.ToDto(db.Statistics.SingleOrDefault(x => x.StatisticsId == id));
      }
    }

    public IEnumerable<DT.Statistics> GetStatistics(Expression<Func<Statistics, bool>> predicate) {
      using (var db = CreateContext()) {
        return db.Statistics.Where(predicate).Select(x => Convert.ToDto(x)).ToArray();
      }
    }

    public Guid AddStatistics(DT.Statistics dto) {
      using (var db = CreateContext()) {
        var entity = Convert.ToEntity(dto);
        db.Statistics.InsertOnSubmit(entity);
        db.SubmitChanges();
        foreach (var slaveStat in dto.SlaveStatistics) {
          slaveStat.Id = entity.StatisticsId;
          db.SlaveStatistics.InsertOnSubmit(Convert.ToEntity(slaveStat));
        }
        foreach (var userStat in dto.UserStatistics) {
          userStat.Id = entity.StatisticsId;
          db.UserStatistics.InsertOnSubmit(Convert.ToEntity(userStat));
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

    public List<DT.UserStatistics> GetUserStatistics() {
      using (var db = CreateContext()) {
        var userStats = new Dictionary<Guid, DT.UserStatistics>();

        var usedCoresByUser = from job in db.Jobs
                              where job.State == JobState.Calculating
                              group job by job.HiveExperiment.OwnerUserId into g
                              select new { UserId = g.Key, UsedCores = g.Count() };

        foreach (var item in usedCoresByUser) {
          if (!userStats.ContainsKey(item.UserId)) {
            userStats.Add(item.UserId, new DT.UserStatistics() { UserId = item.UserId });
          }
          userStats[item.UserId].UsedCores += item.UsedCores;
        }

        var executionTimesByUser = from job in db.Jobs
                                   group job by job.HiveExperiment.OwnerUserId into g
                                   select new { UserId = g.Key, ExecutionTime = TimeSpan.FromMilliseconds(g.Select(x => x.ExecutionTimeMs).Sum()) };
        foreach (var item in executionTimesByUser) {
          if (!userStats.ContainsKey(item.UserId)) {
            userStats.Add(item.UserId, new DT.UserStatistics() { UserId = item.UserId });
          }
          userStats[item.UserId].ExecutionTime += item.ExecutionTime;
        }

        // execution times only of finished jobs - necessary to compute efficieny
        var executionTimesFinishedJobs = from job in db.Jobs
                                         where job.State == JobState.Finished
                                         group job by job.HiveExperiment.OwnerUserId into g
                                         select new { UserId = g.Key, ExecutionTimeFinishedJobs = TimeSpan.FromMilliseconds(g.Select(x => x.ExecutionTimeMs).Sum()) };

        foreach (var item in executionTimesFinishedJobs) {
          if (!userStats.ContainsKey(item.UserId)) {
            userStats.Add(item.UserId, new DT.UserStatistics() { UserId = item.UserId });
          }
          userStats[item.UserId].ExecutionTimeFinishedJobs += item.ExecutionTimeFinishedJobs;
        }

        // start to end times only of finished jobs - necessary to compute efficiency
        var startToEndTimesFinishedJobs = from job in db.Jobs
                                          where job.State == JobState.Finished
                                          group job by job.HiveExperiment.OwnerUserId into g
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
                                                 ExecutionTime = TimeSpan.FromMilliseconds(g.Select(x => x.ExecutionTimeMs).Sum()),
                                                 ExecutionTimeFinishedJobs = TimeSpan.FromMilliseconds(g.Select(x => x.ExecutionTimeMsFinishedJobs).Sum()),
                                                 StartToEndTime = TimeSpan.FromMilliseconds(g.Select(x => x.StartToEndTimeMs).Sum())
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
    private void CollectChildJobs(HiveDataContext db, Guid parentJobId, List<Job> collection) {
      var jobs = db.Jobs.Where(j => j.ParentJobId == parentJobId);
      foreach (var job in jobs) {
        collection.Add(job);
        if (job.IsParentJob)
          CollectChildJobs(db, job.JobId, collection);
      }
    }
    #endregion
  }
}
