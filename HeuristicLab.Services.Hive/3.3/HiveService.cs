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
using System.ServiceModel;
using HeuristicLab.Services.Hive.DataTransfer;
using HeuristicLab.Services.Hive.ServiceContracts;
using DA = HeuristicLab.Services.Hive.DataAccess;
using DT = HeuristicLab.Services.Hive.DataTransfer;


namespace HeuristicLab.Services.Hive {

  /// <summary>
  /// Implementation of the Hive service (interface <see cref="IHiveService"/>).
  /// We need 'IgnoreExtensionDataObject' Attribute for the slave to work. 
  /// </summary>
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, IgnoreExtensionDataObject = true)]
  public class HiveService : IHiveService {
    private IHiveDao dao {
      get { return ServiceLocator.Instance.HiveDao; }
    }
    private Access.IRoleVerifier authen {
      get { return ServiceLocator.Instance.RoleVerifier; }
    }
    private IAuthorizationManager author {
      get { return ServiceLocator.Instance.AuthorizationManager; }
    }
    private DataAccess.ITransactionManager trans {
      get { return ServiceLocator.Instance.TransactionManager; }
    }
    private IEventManager eventManager {
      get { return ServiceLocator.Instance.EventManager; }
    }
    private Access.IUserManager userManager {
      get { return ServiceLocator.Instance.UserManager; }
    }
    private HeartbeatManager heartbeatManager {
      get { return ServiceLocator.Instance.HeartbeatManager; }
    }

    #region Task Methods
    public Guid AddTask(Task task, TaskData taskData, IEnumerable<Guid> resourceIds) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      return trans.UseTransaction(() => {
        task.Id = dao.AddTask(task);
        taskData.TaskId = task.Id;
        taskData.LastUpdate = DateTime.Now;
        dao.AssignJobToResource(task.Id, resourceIds);
        dao.AddTaskData(taskData);
        dao.UpdateTaskState(task.Id, DA.TaskState.Waiting, null, userManager.CurrentUserId, null);
        return taskData.TaskId;
      }, false, true);
    }

    public Guid AddChildTask(Guid parentTaskId, Task task, TaskData taskData) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      return trans.UseTransaction(() => {
        task.ParentTaskId = parentTaskId;
        return AddTask(task, taskData, dao.GetAssignedResources(parentTaskId).Select(x => x.Id));
      }, false, true);
    }

    public Task GetTask(Guid taskId) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client, HiveRoles.Slave);
      author.AuthorizeForTask(taskId, Permission.Read);

      return trans.UseTransaction(() => {
        return dao.GetTask(taskId);
      }, false, false);
    }

    public IEnumerable<Task> GetTasks() {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      return trans.UseTransaction(() => {
        var tasks = dao.GetTasks(x => true);
        foreach (var task in tasks)
          author.AuthorizeForTask(task.Id, Permission.Read);
        return tasks;
      });
    }

    public IEnumerable<LightweightTask> GetLightweightTasks(IEnumerable<Guid> taskIds) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);

      return trans.UseTransaction(() => {
        var tasks = dao.GetTasks(x => taskIds.Contains(x.TaskId)).Select(x => new LightweightTask(x)).ToArray();
        foreach (var task in tasks)
          author.AuthorizeForTask(task.Id, Permission.Read);
        return tasks;
      }, false, false);
    }

    public IEnumerable<LightweightTask> GetLightweightChildTasks(Guid? parentTaskId, bool recursive, bool includeParent) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);

      return trans.UseTransaction(() => {
        var tasks = GetChildTasks(parentTaskId, recursive, includeParent).Select(x => new LightweightTask(x)).ToArray();
        foreach (var task in tasks)
          author.AuthorizeForTask(task.Id, Permission.Read);
        return tasks;
      }, false, false);
    }

    public IEnumerable<LightweightTask> GetLightweightJobTasks(Guid jobId) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      author.AuthorizeForJob(jobId, Permission.Read);

      return trans.UseTransaction(() => {
        return dao.GetLightweightTasks(task => task.JobId == jobId).ToArray();
      }, false, true);
    }

    public IEnumerable<LightweightTask> GetLightweightJobTasksWithoutStateLog(Guid jobId) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      author.AuthorizeForJob(jobId, Permission.Read);

      return trans.UseTransaction(() => {
        return dao.GetLightweightTasksWithoutStateLog(task => task.JobId == jobId).ToArray();
      }, false, false);
    }

    public TaskData GetTaskData(Guid taskId) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client, HiveRoles.Slave);
      author.AuthorizeForTask(taskId, Permission.Read);

      return trans.UseTransaction(() => {
        return dao.GetTaskData(taskId);
      });
    }

    public void UpdateTask(Task taskDto) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client, HiveRoles.Slave);
      author.AuthorizeForTask(taskDto.Id, Permission.Full);

      trans.UseTransaction(() => {
        dao.UpdateTaskAndPlugins(taskDto);
      });
    }

    public void UpdateTaskData(Task task, TaskData taskData) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client, HiveRoles.Slave);
      author.AuthorizeForTask(task.Id, Permission.Full);

      trans.UseTransaction(() => {
        dao.UpdateTaskAndPlugins(task);
      });

      trans.UseTransaction(() => {
        taskData.LastUpdate = DateTime.Now;
        dao.UpdateTaskData(taskData);
      });
    }

    public void DeleteTask(Guid taskId) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client, HiveRoles.Slave);
      author.AuthorizeForTask(taskId, Permission.Full);
      trans.UseTransaction(() => {
        dao.DeleteTask(taskId);
      });
    }

    public void DeleteChildTasks(Guid parentTaskId) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client, HiveRoles.Slave);
      author.AuthorizeForTask(parentTaskId, Permission.Full);
      trans.UseTransaction(() => {
        var tasks = GetChildTasks(parentTaskId, true, false);
        foreach (var task in tasks) {
          dao.DeleteTask(task.Id);
          dao.DeleteTaskData(task.Id);
        };
      });
    }

    public Task UpdateTaskState(Guid taskId, TaskState taskState, Guid? slaveId, Guid? userId, string exception) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client, HiveRoles.Slave);
      author.AuthorizeForTask(taskId, Permission.Full);
      return trans.UseTransaction(() => {
        Task task = dao.UpdateTaskState(taskId, DataTransfer.Convert.ToEntity(taskState), slaveId, userId, exception);

        if (task.Command.HasValue && task.Command.Value == Command.Pause && task.State == TaskState.Paused) {
          task.Command = null;
        } else if (task.Command.HasValue && task.Command.Value == Command.Abort && task.State == TaskState.Aborted) {
          task.Command = null;
        } else if (task.Command.HasValue && task.Command.Value == Command.Stop && task.State == TaskState.Aborted) {
          task.Command = null;
        } else if (taskState == TaskState.Paused && !task.Command.HasValue) {
          // slave paused and uploaded the task (no user-command) -> set waiting.
          task = dao.UpdateTaskState(taskId, DataTransfer.Convert.ToEntity(TaskState.Waiting), slaveId, userId, exception);
        }

        dao.UpdateTaskAndPlugins(task);
        return task;
      });
    }

    public IEnumerable<Task> GetTasksByResourceId(Guid resourceId) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator);
      var tasks = trans.UseTransaction(() => dao.GetJobsByResourceId(resourceId));
      foreach (var task in tasks)
        author.AuthorizeForTask(task.Id, Permission.Read);
      return tasks;
    }
    #endregion

    #region Task Control Methods
    public void StopTask(Guid taskId) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client, HiveRoles.Slave);
      author.AuthorizeForTask(taskId, Permission.Full);
      trans.UseTransaction(() => {
        var task = dao.GetTask(taskId);
        if (task.State == TaskState.Calculating || task.State == TaskState.Transferring) {
          task.Command = Command.Stop;
          dao.UpdateTask(task);
        } else {
          if (task.State != TaskState.Aborted && task.State != TaskState.Finished && task.State != TaskState.Failed) {
            task = UpdateTaskState(taskId, TaskState.Aborted, null, null, string.Empty);
          }
        }
      });
    }

    public void PauseTask(Guid taskId) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client, HiveRoles.Slave);
      author.AuthorizeForTask(taskId, Permission.Full);
      trans.UseTransaction(() => {
        var job = dao.GetTask(taskId);
        if (job.State == TaskState.Calculating || job.State == TaskState.Transferring) {
          job.Command = Command.Pause;
          dao.UpdateTask(job);
        } else {
          job = UpdateTaskState(taskId, TaskState.Paused, null, null, string.Empty);
        }
      });
    }

    public void RestartTask(Guid taskId) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client, HiveRoles.Slave);
      author.AuthorizeForTask(taskId, Permission.Full);
      trans.UseTransaction(() => {
        Task task = dao.UpdateTaskState(taskId, DA.TaskState.Waiting, null, userManager.CurrentUserId, string.Empty);
        task.Command = null;
        dao.UpdateTask(task);
      });
    }
    #endregion

    #region Job Methods
    public Job GetJob(Guid id) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      author.AuthorizeForJob(id, Permission.Read);
      return trans.UseTransaction(() => {
        var job = dao.GetJobs(x =>
              x.JobId == id
              && (x.OwnerUserId == userManager.CurrentUserId || x.JobPermissions.Count(hep => hep.Permission != DA.Permission.NotAllowed && hep.GrantedUserId == userManager.CurrentUserId) > 0)
            ).FirstOrDefault();
        if (job != null) {
          job.Permission = DT.Convert.ToDto(dao.GetPermissionForJob(job.Id, userManager.CurrentUserId));
          job.OwnerUsername = userManager.GetUserById(job.OwnerUserId).UserName;
        }
        return job;
      });
    }

    public IEnumerable<Job> GetJobs() {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      return trans.UseTransaction(() => {
        var jobs = dao.GetJobs(x => x.OwnerUserId == userManager.CurrentUserId || x.JobPermissions.Count(hep => hep.Permission != DA.Permission.NotAllowed && hep.GrantedUserId == userManager.CurrentUserId) > 0);
        foreach (var job in jobs) {
          author.AuthorizeForJob(job.Id, Permission.Read);
          job.Permission = DT.Convert.ToDto(dao.GetPermissionForJob(job.Id, userManager.CurrentUserId));
          job.OwnerUsername = userManager.GetUserById(job.OwnerUserId).UserName;
        }
        return jobs;
      });
    }

    public IEnumerable<Job> GetAllJobs() {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator);
      return trans.UseTransaction(() => {
        var jobs = dao.GetJobs(x => true);
        foreach (var job in jobs) { // no authorization here, since this method is admin-only! (admin is allowed to read all task)
          job.Permission = DT.Convert.ToDto(dao.GetPermissionForJob(job.Id, userManager.CurrentUserId));
          job.OwnerUsername = userManager.GetUserById(job.OwnerUserId).UserName;
        }
        return jobs;
      });
    }

    public Guid AddJob(Job jobDto) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      return trans.UseTransaction(() => {
        jobDto.OwnerUserId = userManager.CurrentUserId;
        jobDto.DateCreated = DateTime.Now;
        return dao.AddJob(jobDto);
      });
    }

    public void UpdateJob(Job jobDto) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      author.AuthorizeForJob(jobDto.Id, Permission.Full);
      trans.UseTransaction(() => {
        dao.UpdateJob(jobDto);
      });
    }

    public void DeleteJob(Guid jobId) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      author.AuthorizeForJob(jobId, Permission.Full);
      trans.UseTransaction(() => {
        dao.DeleteJob(jobId); // child task will be deleted by db-trigger
      });
    }
    #endregion

    #region JobPermission Methods
    public void GrantPermission(Guid jobId, Guid grantedUserId, Permission permission) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      trans.UseTransaction(() => {
        Job job = dao.GetJob(jobId);
        if (job == null) throw new FaultException<FaultReason>(new FaultReason("Could not find task with id " + jobId));
        Permission perm = DT.Convert.ToDto(dao.GetPermissionForJob(job.Id, userManager.CurrentUserId));
        if (perm != Permission.Full) throw new FaultException<FaultReason>(new FaultReason("Not allowed to grant permissions for this experiment"));
        dao.SetJobPermission(jobId, userManager.CurrentUserId, grantedUserId, DT.Convert.ToEntity(permission));
      });
    }

    public void RevokePermission(Guid jobId, Guid grantedUserId) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      trans.UseTransaction(() => {
        Job job = dao.GetJob(jobId);
        if (job == null) throw new FaultException<FaultReason>(new FaultReason("Could not find task with id " + jobId));
        DA.Permission perm = dao.GetPermissionForJob(job.Id, userManager.CurrentUserId);
        if (perm != DA.Permission.Full) throw new FaultException<FaultReason>(new FaultReason("Not allowed to grant permissions for this experiment"));
        dao.SetJobPermission(jobId, userManager.CurrentUserId, grantedUserId, DA.Permission.NotAllowed);
      });
    }

    public IEnumerable<JobPermission> GetJobPermissions(Guid jobId) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      return trans.UseTransaction(() => {
        DA.Permission currentUserPermission = dao.GetPermissionForJob(jobId, userManager.CurrentUserId);
        if (currentUserPermission != DA.Permission.Full) throw new FaultException<FaultReason>(new FaultReason("Not allowed to list permissions for this experiment"));
        return dao.GetJobPermissions(x => x.JobId == jobId);
      });
    }

    public bool IsAllowedPrivileged() {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      return authen.IsInRole(HiveRoles.IsAllowedPrivileged);
    }
    #endregion

    #region Login Methods
    public void Hello(Slave slaveInfo) {
      authen.AuthenticateForAnyRole(HiveRoles.Slave);
      if (userManager.CurrentUser.UserName != "hiveslave")
        slaveInfo.OwnerUserId = userManager.CurrentUserId;

      trans.UseTransaction(() => {
        var slave = dao.GetSlave(slaveInfo.Id);

        if (slave == null) {
          dao.AddSlave(slaveInfo);
        } else {
          slave.Name = slaveInfo.Name;
          slave.Description = slaveInfo.Description;
          slave.OwnerUserId = slaveInfo.OwnerUserId;

          slave.Cores = slaveInfo.Cores;
          slave.CpuArchitecture = slaveInfo.CpuArchitecture;
          slave.CpuSpeed = slaveInfo.CpuSpeed;
          slave.FreeCores = slaveInfo.FreeCores;
          slave.FreeMemory = slaveInfo.FreeMemory;
          slave.Memory = slaveInfo.Memory;
          slave.OperatingSystem = slaveInfo.OperatingSystem;

          slave.LastHeartbeat = DateTime.Now;
          slave.SlaveState = SlaveState.Idle;

          // don't update those properties: dbSlave.IsAllowedToCalculate, dbSlave.ParentResourceId 

          dao.UpdateSlave(slave);
        }
      });
    }

    public void GoodBye(Guid slaveId) {
      authen.AuthenticateForAnyRole(HiveRoles.Slave);
      trans.UseTransaction(() => {
        var slave = dao.GetSlave(slaveId);
        if (slave != null) {
          slave.SlaveState = SlaveState.Offline;
          dao.UpdateSlave(slave);
        }
      });
    }
    #endregion

    #region Heartbeat Methods
    public List<MessageContainer> Heartbeat(Heartbeat heartbeat) {
      authen.AuthenticateForAnyRole(HiveRoles.Slave);

      List<MessageContainer> result = new List<MessageContainer>();
      try {
        result = heartbeatManager.ProcessHeartbeat(heartbeat);
      }
      catch (Exception ex) {
        DA.LogFactory.GetLogger(this.GetType().Namespace).Log("Exception processing Heartbeat: " + ex.ToString());
      }

      if (HeuristicLab.Services.Hive.Properties.Settings.Default.TriggerEventManagerInHeartbeat) {
        TriggerEventManager(false);
      }

      return result;
    }
    #endregion

    #region Plugin Methods
    public Guid AddPlugin(Plugin plugin, List<PluginData> pluginDatas) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      return trans.UseTransaction(() => {
        plugin.UserId = userManager.CurrentUserId;
        plugin.DateCreated = DateTime.Now;

        var existing = dao.GetPlugins(x => x.Hash != null).Where(x => x.Hash.SequenceEqual(plugin.Hash));
        if (existing.Count() > 0) {
          // a plugin already exists.
          throw new FaultException<PluginAlreadyExistsFault>(new PluginAlreadyExistsFault(existing.Single().Id));
        }

        Guid pluginId = dao.AddPlugin(plugin);
        foreach (PluginData pluginData in pluginDatas) {
          pluginData.PluginId = pluginId;
          dao.AddPluginData(pluginData);
        }
        return pluginId;
      });
    }

    public Plugin GetPlugin(Guid pluginId) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client, HiveRoles.Slave);
      return trans.UseTransaction(() => {
        return dao.GetPlugin(pluginId);
      });
    }

    public Plugin GetPluginByHash(byte[] hash) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client, HiveRoles.Slave);
      return trans.UseTransaction(() => {
        return dao.GetPlugins(x => x.Hash == hash).FirstOrDefault();
      });
    }

    // note: this is a possible security problem, since a client is able to download all plugins, which may contain proprietary code (which can be disassembled)
    //       change so that only with GetPluginByHash it is possible to download plugins
    public IEnumerable<Plugin> GetPlugins() {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client, HiveRoles.Slave);
      return trans.UseTransaction(() => {
        return dao.GetPlugins(x => x.Hash != null);
      });
    }

    public IEnumerable<PluginData> GetPluginDatas(List<Guid> pluginIds) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client, HiveRoles.Slave);
      var pluginDatas = new List<PluginData>();
      return trans.UseTransaction(() => {
        foreach (Guid guid in pluginIds) {
          pluginDatas.AddRange(dao.GetPluginDatas(x => x.PluginId == guid).ToList());
        }
        return pluginDatas;
      });
    }

    public void DeletePlugin(Guid pluginId) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client, HiveRoles.Slave);
      trans.UseTransaction(() => {
        dao.DeletePlugin(pluginId);
      });
    }
    #endregion

    #region ResourcePermission Methods
    public void GrantResourcePermissions(Guid resourceId, Guid[] grantedUserIds) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      trans.UseTransaction(() => {
        Resource resource = dao.GetResource(resourceId);
        if (resource == null) throw new FaultException<FaultReason>(new FaultReason("Could not find resource with id " + resourceId));
        if (resource.OwnerUserId != userManager.CurrentUserId && !authen.IsInRole(HiveRoles.Administrator)) throw new FaultException<FaultReason>(new FaultReason("Not allowed to grant permission for this resource"));
        foreach (Guid id in grantedUserIds)
          dao.AddResourcePermission(new ResourcePermission { ResourceId = resourceId, GrantedByUserId = userManager.CurrentUserId, GrantedUserId = id });
      });
    }

    public void RevokeResourcePermissions(Guid resourceId, Guid[] grantedUserIds) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      trans.UseTransaction(() => {
        Resource resource = dao.GetResource(resourceId);
        if (resource == null) throw new FaultException<FaultReason>(new FaultReason("Could not find resource with id " + resourceId));
        if (resource.OwnerUserId != userManager.CurrentUserId && !authen.IsInRole(HiveRoles.Administrator)) throw new FaultException<FaultReason>(new FaultReason("Not allowed to revoke permission for this resource"));
        foreach (Guid id in grantedUserIds)
          dao.DeleteResourcePermission(resourceId, id);
      });
    }

    public IEnumerable<ResourcePermission> GetResourcePermissions(Guid resourceId) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      return trans.UseTransaction(() => {
        Resource resource = dao.GetResource(resourceId);
        if (resource == null) throw new FaultException<FaultReason>(new FaultReason("Could not find resource with id " + resourceId));
        return dao.GetResourcePermissions(x => x.ResourceId == resourceId);
      });
    }
    #endregion

    #region Resource Methods
    public IEnumerable<Resource> GetChildResources(Guid resourceId) {
      return trans.UseTransaction(() => { return dao.GetChildResources(resourceId); });
    }
    #endregion

    #region Slave Methods
    public int GetNewHeartbeatInterval(Guid slaveId) {
      authen.AuthenticateForAnyRole(HiveRoles.Slave);

      Slave s = trans.UseTransaction(() => { return dao.GetSlave(slaveId); });
      if (s != null) {
        return s.HbInterval;
      } else {
        return -1;
      }
    }

    public Guid AddSlave(Slave slave) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator);
      return trans.UseTransaction(() => dao.AddSlave(slave));
    }

    public Guid AddSlaveGroup(SlaveGroup slaveGroup) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      return trans.UseTransaction(() => dao.AddSlaveGroup(slaveGroup));
    }

    public Slave GetSlave(Guid slaveId) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator);
      return trans.UseTransaction(() => { return dao.GetSlave(slaveId); });
    }

    public SlaveGroup GetSlaveGroup(Guid slaveGroupId) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator);
      return trans.UseTransaction(() => { return dao.GetSlaveGroup(slaveGroupId); });
    }

    public IEnumerable<Slave> GetSlaves() {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      return trans.UseTransaction(() => {
        return dao.GetSlaves(x => true).Where(x => x.OwnerUserId == null
                                           || x.OwnerUserId == userManager.CurrentUserId
                                           || userManager.VerifyUser(userManager.CurrentUserId, GetResourcePermissions(x.Id).Select(y => y.GrantedUserId).ToList())
                                           || authen.IsInRole(HiveRoles.Administrator)).ToArray();
      });
    }

    public IEnumerable<SlaveGroup> GetSlaveGroups() {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      return trans.UseTransaction(() => {
        return dao.GetSlaveGroups(x => true).Where(x => x.OwnerUserId == null
                                                || x.OwnerUserId == userManager.CurrentUserId
                                                || userManager.VerifyUser(userManager.CurrentUserId, GetResourcePermissions(x.Id).Select(y => y.GrantedUserId).ToList())
                                                || authen.IsInRole(HiveRoles.Administrator)).ToArray();
      });
    }

    public void UpdateSlave(Slave slave) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      trans.UseTransaction(() => {
        dao.UpdateSlave(slave);
      });
    }

    public void UpdateSlaveGroup(SlaveGroup slaveGroup) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      trans.UseTransaction(() => {
        dao.UpdateSlaveGroup(slaveGroup);
      });
    }

    public void DeleteSlave(Guid slaveId) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      author.AuthorizeForResourceAdministration(slaveId);
      trans.UseTransaction(() => {
        dao.DeleteSlave(slaveId);
      });
    }

    public void DeleteSlaveGroup(Guid slaveGroupId) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      author.AuthorizeForResourceAdministration(slaveGroupId);
      trans.UseTransaction(() => {
        dao.DeleteSlaveGroup(slaveGroupId);
      });
    }

    public void AddResourceToGroup(Guid slaveGroupId, Guid resourceId) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator);
      trans.UseTransaction(() => {
        var resource = dao.GetResource(resourceId);
        resource.ParentResourceId = slaveGroupId;
        dao.UpdateResource(resource);
      });
    }

    public void RemoveResourceFromGroup(Guid slaveGroupId, Guid resourceId) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator);
      trans.UseTransaction(() => {
        var resource = dao.GetResource(resourceId);
        resource.ParentResourceId = null;
        dao.UpdateResource(resource);
      });
    }

    public Guid GetResourceId(string resourceName) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      return trans.UseTransaction(() => {
        var resource = dao.GetResources(x => x.Name == resourceName).FirstOrDefault();
        if (resource != null) {
          return resource.Id;
        } else {
          return Guid.Empty;
        }
      });
    }

    public void TriggerEventManager(bool force) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Slave);
      // use a serializable transaction here to ensure not two threads execute this simultaniously (mutex-lock would not work since IIS may use multiple AppDomains)
      bool cleanup = false;
      trans.UseTransaction(() => {
        DateTime lastCleanup = dao.GetLastCleanup();
        if (force || DateTime.Now - lastCleanup > HeuristicLab.Services.Hive.Properties.Settings.Default.CleanupInterval) {
          dao.SetLastCleanup(DateTime.Now);
          cleanup = true;
        }
      }, true);

      if (cleanup) {
        eventManager.Cleanup();
      }
    }
    #endregion

    #region Downtime Methods
    public Guid AddDowntime(Downtime downtime) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      author.AuthorizeForResourceAdministration(downtime.ResourceId);
      return trans.UseTransaction(() => dao.AddDowntime(downtime));
    }

    public void DeleteDowntime(Guid downtimeId) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      // TODO: pass resource id
      // author.AuthorizeForResource(resourceId);
      trans.UseTransaction(() => {
        dao.DeleteDowntime(downtimeId);
      });
    }

    public void UpdateDowntime(Downtime downtime) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      author.AuthorizeForResourceAdministration(downtime.ResourceId);
      trans.UseTransaction(() => {
        dao.UpdateDowntime(downtime);
      });
    }

    public IEnumerable<Downtime> GetDowntimesForResource(Guid resourceId) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      return trans.UseTransaction(() => dao.GetDowntimes(x => x.ResourceId == resourceId));
    }
    #endregion

    #region User Methods
    public string GetUsernameByUserId(Guid userId) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      var user = ServiceLocator.Instance.UserManager.GetUserById(userId);
      if (user != null)
        return user.UserName;
      else
        return null;
    }

    public Guid GetUserIdByUsername(string username) {
      authen.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      var user = ServiceLocator.Instance.UserManager.GetUserByName(username);
      return user != null ? (Guid)user.ProviderUserKey : Guid.Empty;
    }
    #endregion

    #region UserPriority Methods
    public IEnumerable<UserPriority> GetUserPriorities() {
      return trans.UseTransaction(() => dao.GetUserPriorities(x => true));
    }
    #endregion

    #region Helper Methods
    private IEnumerable<Task> GetChildTasks(Guid? parentTaskId, bool recursive, bool includeParent) {
      var tasks = new List<Task>(dao.GetTasks(x => parentTaskId == null ? !x.ParentTaskId.HasValue : x.ParentTaskId.Value == parentTaskId));

      if (recursive) {
        var childs = new List<Task>();
        foreach (var task in tasks) {
          childs.AddRange(GetChildTasks(task.Id, recursive, false));
        }
        tasks.AddRange(childs);
      }

      if (includeParent) tasks.Add(GetTask(parentTaskId.Value));
      return tasks;
    }
    #endregion

    #region Statistics Methods
    public IEnumerable<Statistics> GetStatistics() {
      return trans.UseTransaction(() => { return dao.GetStatistics(x => true); });
    }
    public IEnumerable<Statistics> GetStatisticsForTimePeriod(DateTime from, DateTime to) {
      return trans.UseTransaction(() => { return dao.GetStatistics(x => x.Timestamp >= from && x.Timestamp <= to); });
    }
    #endregion
  }
}
