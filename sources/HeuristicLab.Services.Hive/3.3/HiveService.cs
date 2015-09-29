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
using System.Linq;
using System.Security;
using System.ServiceModel;
using HeuristicLab.Services.Access;
using HeuristicLab.Services.Hive.DataAccess.Interfaces;
using HeuristicLab.Services.Hive.DataTransfer;
using HeuristicLab.Services.Hive.Manager;
using HeuristicLab.Services.Hive.ServiceContracts;
using DA = HeuristicLab.Services.Hive.DataAccess;
using DT = HeuristicLab.Services.Hive.DataTransfer;

namespace HeuristicLab.Services.Hive {
  /// <summary>
  /// Implementation of the Hive service (interface <see cref="IHiveService"/>).
  /// We need 'IgnoreExtensionDataObject' Attribute for the slave to work. 
  /// </summary>
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, IgnoreExtensionDataObject = true)]
  [HiveOperationContextBehavior]
  public class HiveService : IHiveService {
    private static readonly DA.TaskState[] CompletedStates = { DA.TaskState.Finished, DA.TaskState.Aborted, DA.TaskState.Failed };

    private IPersistenceManager PersistenceManager {
      get { return ServiceLocator.Instance.PersistenceManager; }
    }

    private IUserManager UserManager {
      get { return ServiceLocator.Instance.UserManager; }
    }

    private IRoleVerifier RoleVerifier {
      get { return ServiceLocator.Instance.RoleVerifier; }
    }

    private IAuthorizationManager AuthorizationManager {
      get { return ServiceLocator.Instance.AuthorizationManager; }
    }
    private IEventManager EventManager {
      get { return ServiceLocator.Instance.EventManager; }
    }
    private HeartbeatManager HeartbeatManager {
      get { return ServiceLocator.Instance.HeartbeatManager; }
    }

    #region Task Methods
    public Guid AddTask(DT.Task task, DT.TaskData taskData, IEnumerable<Guid> resourceIds) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      var pm = PersistenceManager;
      using (new PerformanceLogger("AddTask")) {
        var taskDao = pm.TaskDao;
        var stateLogDao = pm.StateLogDao;
        var newTask = task.ToEntity();
        newTask.JobData = taskData.ToEntity();
        newTask.JobData.LastUpdate = DateTime.Now;
        newTask.AssignedResources.AddRange(resourceIds.Select(
          x => new DA.AssignedResource {
            ResourceId = x
          }));
        newTask.State = DA.TaskState.Waiting;
        return pm.UseTransaction(() => {
          taskDao.Save(newTask);
          pm.SubmitChanges();
          stateLogDao.Save(new DA.StateLog {
            State = DA.TaskState.Waiting,
            DateTime = DateTime.Now,
            TaskId = newTask.TaskId,
            UserId = UserManager.CurrentUserId,
            SlaveId = null,
            Exception = null
          });
          pm.SubmitChanges();
          return newTask.TaskId;
        }, false, true);
      }
    }

    public Guid AddChildTask(Guid parentTaskId, DT.Task task, DT.TaskData taskData) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      IEnumerable<Guid> resourceIds;
      var pm = PersistenceManager;
      using (new PerformanceLogger("AddChildTask")) {
        var assignedResourceDao = pm.AssignedResourceDao;
        resourceIds = pm.UseTransaction(() => {
          return assignedResourceDao.GetByTaskId(parentTaskId)
            .Select(x => x.ResourceId)
            .ToList();
        });
      }
      task.ParentTaskId = parentTaskId;
      return AddTask(task, taskData, resourceIds);
    }

    public DT.Task GetTask(Guid taskId) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client, HiveRoles.Slave);
      AuthorizationManager.AuthorizeForTask(taskId, Permission.Read);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetTask")) {
        var taskDao = pm.TaskDao;
        return pm.UseTransaction(() => {
          var task = taskDao.GetById(taskId);
          return task.ToDto();
        });
      }
    }

    public IEnumerable<DT.LightweightTask> GetLightweightJobTasks(Guid jobId) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      AuthorizationManager.AuthorizeForJob(jobId, Permission.Read);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetLightweightJobTasks")) {
        var taskDao = pm.TaskDao;
        return pm.UseTransaction(() => {
          return taskDao.GetByJobId(jobId)
            .ToList()
            .Select(x => new DT.LightweightTask {
              Id = x.TaskId,
              ExecutionTime = TimeSpan.FromMilliseconds(x.ExecutionTimeMs),
              ParentTaskId = x.ParentTaskId,
              StateLog = x.StateLogs.OrderBy(y => y.DateTime)
                                    .Select(z => z.ToDto())
                                    .ToList(),
              State = x.State.ToDto(),
              Command = x.Command.ToDto(),
              LastTaskDataUpdate = x.JobData.LastUpdate
            })
            .ToList();
        }, false, true);
      }
    }

    public IEnumerable<DT.LightweightTask> GetLightweightJobTasksWithoutStateLog(Guid jobId) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      AuthorizationManager.AuthorizeForJob(jobId, Permission.Read);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetLightweightJobTasksWithoutStateLog")) {
        var taskDao = pm.TaskDao;
        return pm.UseTransaction(() => {
          return taskDao.GetByJobId(jobId)
            .ToList()
            .Select(x => new DT.LightweightTask {
              Id = x.TaskId,
              ExecutionTime = TimeSpan.FromMilliseconds(x.ExecutionTimeMs),
              ParentTaskId = x.ParentTaskId,
              StateLog = new List<DT.StateLog>(),
              State = x.State.ToDto(),
              Command = x.Command.ToDto(),
              LastTaskDataUpdate = x.JobData.LastUpdate
            })
            .ToList();
        }, false, true);
      }
    }

    public DT.TaskData GetTaskData(Guid taskId) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client, HiveRoles.Slave);
      AuthorizationManager.AuthorizeForTask(taskId, Permission.Read);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetTaskData")) {
        var taskDataDao = pm.TaskDataDao;
        return pm.UseTransaction(() => taskDataDao.GetById(taskId).ToDto());
      }
    }

    public void UpdateTask(DT.Task taskDto) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client, HiveRoles.Slave);
      AuthorizationManager.AuthorizeForTask(taskDto.Id, Permission.Full);
      var pm = PersistenceManager;
      using (new PerformanceLogger("UpdateTask")) {
        var taskDao = pm.TaskDao;
        pm.UseTransaction(() => {
          var task = taskDao.GetById(taskDto.Id);
          taskDto.CopyToEntity(task);
          pm.SubmitChanges();
        });
      }
    }

    public void UpdateTaskData(DT.Task taskDto, DT.TaskData taskDataDto) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client, HiveRoles.Slave);
      AuthorizationManager.AuthorizeForTask(taskDto.Id, Permission.Full);
      var pm = PersistenceManager;
      using (new PerformanceLogger("UpdateTaskData")) {
        var taskDao = pm.TaskDao;
        var taskDataDao = pm.TaskDataDao;
        pm.UseTransaction(() => {
          var task = taskDao.GetById(taskDto.Id);
          var taskData = taskDataDao.GetById(taskDataDto.TaskId);
          taskDto.CopyToEntity(task);
          taskDataDto.CopyToEntity(taskData);
          taskData.LastUpdate = DateTime.Now;
          pm.SubmitChanges();
        });
      }
    }

    public DT.Task UpdateTaskState(Guid taskId, DT.TaskState taskState, Guid? slaveId, Guid? userId, string exception) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client, HiveRoles.Slave);
      AuthorizationManager.AuthorizeForTask(taskId, Permission.Full);
      var pm = PersistenceManager;
      using (new PerformanceLogger("UpdateTaskState")) {
        var taskDao = pm.TaskDao;
        return pm.UseTransaction(() => {
          var task = taskDao.GetById(taskId);
          UpdateTaskState(pm, task, taskState, slaveId, userId, exception);
          pm.SubmitChanges();
          return task.ToDto();
        });
      }
    }
    #endregion

    #region Task Control Methods
    public void StopTask(Guid taskId) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client, HiveRoles.Slave);
      AuthorizationManager.AuthorizeForTask(taskId, Permission.Full);
      var pm = PersistenceManager;
      using (new PerformanceLogger("StopTask")) {
        var taskDao = pm.TaskDao;
        pm.UseTransaction(() => {
          var task = taskDao.GetById(taskId);
          if (task.State == DA.TaskState.Calculating || task.State == DA.TaskState.Transferring) {
            task.Command = DA.Command.Stop;
          } else if (task.State != DA.TaskState.Aborted
                     && task.State != DA.TaskState.Finished
                     && task.State != DA.TaskState.Failed) {
            UpdateTaskState(pm, task, DT.TaskState.Aborted, null, null, string.Empty);
          }
          pm.SubmitChanges();
        });
      }
    }

    public void PauseTask(Guid taskId) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client, HiveRoles.Slave);
      AuthorizationManager.AuthorizeForTask(taskId, Permission.Full);
      var pm = PersistenceManager;
      using (new PerformanceLogger("PauseTask")) {
        var taskDao = pm.TaskDao;
        pm.UseTransaction(() => {
          var task = taskDao.GetById(taskId);
          if (task.State == DA.TaskState.Calculating || task.State == DA.TaskState.Transferring) {
            task.Command = DA.Command.Pause;
          }
          UpdateTaskState(pm, task, DT.TaskState.Paused, null, null, string.Empty);
          pm.SubmitChanges();
        });
      }
    }

    public void RestartTask(Guid taskId) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client, HiveRoles.Slave);
      AuthorizationManager.AuthorizeForTask(taskId, Permission.Full);
      var pm = PersistenceManager;
      using (new PerformanceLogger("RestartTask")) {
        var taskDao = pm.TaskDao;
        pm.UseTransaction(() => {
          var task = taskDao.GetById(taskId);
          task.Command = null;
          UpdateTaskState(pm, task, DT.TaskState.Waiting, null, UserManager.CurrentUserId, string.Empty);
          pm.SubmitChanges();
        });
      }
    }
    #endregion

    #region Job Methods
    public DT.Job GetJob(Guid id) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      AuthorizationManager.AuthorizeForJob(id, DT.Permission.Read);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetJob")) {
        var jobDao = pm.JobDao;
        var jobPermissionDao = pm.JobPermissionDao;
        var taskDao = pm.TaskDao;
        var currentUserId = UserManager.CurrentUserId;
        return pm.UseTransaction(() => {
          var job = jobDao.GetById(id).ToDto();
          if (job != null) {
            var statistics = taskDao.GetByJobId(job.Id)
              .GroupBy(x => x.JobId)
              .Select(x => new {
                TotalCount = x.Count(),
                CalculatingCount = x.Count(y => y.State == DA.TaskState.Calculating),
                FinishedCount = x.Count(y => CompletedStates.Contains(y.State))
              }).FirstOrDefault();
            if (statistics != null) {
              job.JobCount = statistics.TotalCount;
              job.CalculatingCount = statistics.CalculatingCount;
              job.FinishedCount = statistics.FinishedCount;
            }
            job.OwnerUsername = UserManager.GetUserNameById(job.OwnerUserId);
            if (currentUserId == job.OwnerUserId) {
              job.Permission = Permission.Full;
            } else {
              var jobPermission = jobPermissionDao.GetByJobAndUserId(job.Id, currentUserId);
              job.Permission = jobPermission == null ? Permission.NotAllowed : jobPermission.Permission.ToDto();
            }
          }
          return job;
        });
      }
    }

    public IEnumerable<DT.Job> GetJobs() {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetJobs")) {
        var jobDao = pm.JobDao;
        var jobPermissionDao = pm.JobPermissionDao;
        var taskDao = pm.TaskDao;
        var currentUserId = UserManager.CurrentUserId;
        return pm.UseTransaction(() => {
          var jobs = jobDao.GetAll()
            .Where(x => x.OwnerUserId == currentUserId
                     || x.JobPermissions.Count(y => y.Permission != DA.Permission.NotAllowed
                                                 && y.GrantedUserId == currentUserId) > 0)
            .Select(x => x.ToDto())
            .ToList();
          var statistics = taskDao.GetAll()
              .GroupBy(x => x.JobId)
              .Select(x => new {
                x.Key,
                TotalCount = x.Count(),
                CalculatingCount = x.Count(y => y.State == DA.TaskState.Calculating),
                FinishedCount = x.Count(y => CompletedStates.Contains(y.State))
              })
              .ToList();
          foreach (var job in jobs) {
            var statistic = statistics.FirstOrDefault(x => x.Key == job.Id);
            if (statistic != null) {
              job.JobCount = statistic.TotalCount;
              job.CalculatingCount = statistic.CalculatingCount;
              job.FinishedCount = statistic.FinishedCount;
            }
            job.OwnerUsername = UserManager.GetUserNameById(job.OwnerUserId);
            if (currentUserId == job.OwnerUserId) {
              job.Permission = Permission.Full;
            } else {
              var jobPermission = jobPermissionDao.GetByJobAndUserId(job.Id, currentUserId);
              job.Permission = jobPermission == null ? Permission.NotAllowed : jobPermission.Permission.ToDto();
            }
          }
          return jobs;
        });
      }
    }

    public Guid AddJob(DT.Job jobDto) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      var pm = PersistenceManager;
      using (new PerformanceLogger("AddJob")) {
        var jobDao = pm.JobDao;
        var userPriorityDao = pm.UserPriorityDao;
        return pm.UseTransaction(() => {
          jobDto.OwnerUserId = UserManager.CurrentUserId;
          jobDto.DateCreated = DateTime.Now;
          var job = jobDao.Save(jobDto.ToEntity());
          if (userPriorityDao.GetById(jobDto.OwnerUserId) == null) {
            userPriorityDao.Save(new DA.UserPriority {
              UserId = jobDto.OwnerUserId,
              DateEnqueued = jobDto.DateCreated
            });
          }
          pm.SubmitChanges();
          return job.JobId;
        });
      }
    }

    public void UpdateJob(DT.Job jobDto) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      AuthorizationManager.AuthorizeForJob(jobDto.Id, DT.Permission.Full);
      var pm = PersistenceManager;
      using (new PerformanceLogger("UpdateJob")) {
        bool exists = true;
        var jobDao = pm.JobDao;
        pm.UseTransaction(() => {
          var job = jobDao.GetById(jobDto.Id);
          if (job == null) {
            exists = false;
            job = new DA.Job();
          }
          jobDto.CopyToEntity(job);
          if (!exists) {
            jobDao.Save(job);
          }
          pm.SubmitChanges();
        });
      }
    }

    public void DeleteJob(Guid jobId) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      AuthorizationManager.AuthorizeForJob(jobId, DT.Permission.Full);
      var pm = PersistenceManager;
      using (new PerformanceLogger("DeleteJob")) {
        var jobDao = pm.JobDao;
        pm.UseTransaction(() => {
          // child task will be deleted by db-trigger
          jobDao.Delete(jobId);
          pm.SubmitChanges();
        });
      }
    }
    #endregion

    #region JobPermission Methods
    public void GrantPermission(Guid jobId, Guid grantedUserId, DT.Permission permission) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      AuthorizationManager.AuthorizeForJob(jobId, Permission.Full);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GrantPermission")) {
        var jobPermissionDao = pm.JobPermissionDao;
        var currentUserId = UserManager.CurrentUserId;
        pm.UseTransaction(() => {
          jobPermissionDao.SetJobPermission(jobId, currentUserId, grantedUserId, permission.ToEntity());
          pm.SubmitChanges();
        });
      }
    }

    public void RevokePermission(Guid jobId, Guid grantedUserId) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      AuthorizationManager.AuthorizeForJob(jobId, Permission.Full);
      var pm = PersistenceManager;
      using (new PerformanceLogger("RevokePermission")) {
        var jobPermissionDao = pm.JobPermissionDao;
        var currentUserId = UserManager.CurrentUserId;
        pm.UseTransaction(() => {
          jobPermissionDao.SetJobPermission(jobId, currentUserId, grantedUserId, DA.Permission.NotAllowed);
          pm.SubmitChanges();
        });
      }
    }

    public IEnumerable<JobPermission> GetJobPermissions(Guid jobId) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      AuthorizationManager.AuthorizeForJob(jobId, Permission.Full);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetJobPermissions")) {
        var jobPermissionDao = pm.JobPermissionDao;
        return pm.UseTransaction(() => jobPermissionDao.GetByJobId(jobId)
          .Select(x => x.ToDto())
          .ToList()
        );
      }
    }

    // BackwardsCompatibility3.3
    #region Backwards compatible code, remove with 3.4
    public bool IsAllowedPrivileged() {
      return true;
    }
    #endregion
    #endregion

    #region Login Methods
    public void Hello(DT.Slave slaveInfo) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Slave);
      if (UserManager.CurrentUser.UserName != "hiveslave") {
        slaveInfo.OwnerUserId = UserManager.CurrentUserId;
      }
      var pm = PersistenceManager;
      using (new PerformanceLogger("Hello")) {
        var slaveDao = pm.SlaveDao;
        pm.UseTransaction(() => {
          var slave = slaveDao.GetById(slaveInfo.Id);
          if (slave == null) {
            slaveDao.Save(slaveInfo.ToEntity());
          } else {
            bool oldIsAllowedToCalculate = slave.IsAllowedToCalculate;
            Guid? oldParentResourceId = slave.ParentResourceId;
            slaveInfo.CopyToEntity(slave);
            slave.IsAllowedToCalculate = oldIsAllowedToCalculate;
            slave.ParentResourceId = oldParentResourceId;
            slave.LastHeartbeat = DateTime.Now;
            slave.SlaveState = DA.SlaveState.Idle;
          }
          pm.SubmitChanges();
        });
      }
    }

    public void GoodBye(Guid slaveId) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Slave);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GoodBye")) {
        var slaveDao = pm.SlaveDao;
        pm.UseTransaction(() => {
          var slave = slaveDao.GetById(slaveId);
          if (slave != null) {
            slave.SlaveState = DA.SlaveState.Offline;
            pm.SubmitChanges();
          }
        });
      }
    }
    #endregion

    #region Heartbeat Methods
    public List<MessageContainer> Heartbeat(DT.Heartbeat heartbeat) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Slave);
      List<MessageContainer> result = new List<MessageContainer>();
      try {
        using (new PerformanceLogger("ProcessHeartbeat")) {
          result = HeartbeatManager.ProcessHeartbeat(heartbeat);
        }
      }
      catch (Exception ex) {
        DA.LogFactory.GetLogger(this.GetType().Namespace).Log(string.Format("Exception processing Heartbeat: {0}", ex));
      }
      if (HeuristicLab.Services.Hive.Properties.Settings.Default.TriggerEventManagerInHeartbeat) {
        TriggerEventManager(false);
      }
      return result;
    }
    #endregion

    #region Plugin Methods
    public DT.Plugin GetPlugin(Guid pluginId) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client, HiveRoles.Slave);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetPlugin")) {
        var pluginDao = pm.PluginDao;
        return pm.UseTransaction(() => pluginDao.GetById(pluginId).ToDto());
      }
    }

    public Guid AddPlugin(DT.Plugin plugin, List<DT.PluginData> pluginData) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      var pm = PersistenceManager;
      using (new PerformanceLogger("AddPlugin")) {
        var pluginDao = pm.PluginDao;
        plugin.UserId = UserManager.CurrentUserId;
        plugin.DateCreated = DateTime.Now;
        return pm.UseTransaction(() => {
          var pluginEntity = pluginDao.GetByHash(plugin.Hash).SingleOrDefault();
          if (pluginEntity != null) {
            throw new FaultException<PluginAlreadyExistsFault>(new PluginAlreadyExistsFault(pluginEntity.PluginId));
          }
          pluginEntity = plugin.ToEntity();
          foreach (var data in pluginData) {
            data.PluginId = default(Guid); // real id will be assigned from linq2sql
            pluginEntity.PluginData.Add(data.ToEntity());
          }
          pluginDao.Save(pluginEntity);
          pm.SubmitChanges();
          return pluginEntity.PluginId;
        });
      }
    }

    public IEnumerable<DT.Plugin> GetPlugins() {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client, HiveRoles.Slave);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetPlugins")) {
        var pluginDao = pm.PluginDao;
        return pm.UseTransaction(() => pluginDao.GetAll()
          .Where(x => x.Hash != null)
          .Select(x => x.ToDto())
          .ToList()
        );
      }
    }

    public IEnumerable<DT.PluginData> GetPluginDatas(List<Guid> pluginIds) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client, HiveRoles.Slave);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetPluginDatas")) {
        var pluginDataDao = pm.PluginDataDao;
        return pm.UseTransaction(() => pluginDataDao.GetAll()
            .Where(x => pluginIds.Contains(x.PluginId))
            .Select(x => x.ToDto())
            .ToList()
        );
      }
    }
    #endregion

    #region ResourcePermission Methods
    public void GrantResourcePermissions(Guid resourceId, Guid[] grantedUserIds) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GrantResourcePermissions")) {
        pm.UseTransaction(() => {
          var resource = AuthorizeForResource(pm, resourceId);
          var resourcePermissions = resource.ResourcePermissions.ToList();
          foreach (var id in grantedUserIds) {
            if (resourcePermissions.All(x => x.GrantedUserId != id)) {
              resource.ResourcePermissions.Add(new DA.ResourcePermission {
                GrantedUserId = id,
                GrantedByUserId = UserManager.CurrentUserId
              });
            }
          }
          pm.SubmitChanges();
        });
      }
    }

    public void RevokeResourcePermissions(Guid resourceId, Guid[] grantedUserIds) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      var pm = PersistenceManager;
      using (new PerformanceLogger("RevokeResourcePermissions")) {
        var resourcePermissionDao = pm.ResourcePermissionDao;
        pm.UseTransaction(() => {
          AuthorizeForResource(pm, resourceId);
          resourcePermissionDao.DeleteByResourceAndGrantedUserId(resourceId, grantedUserIds);
          pm.SubmitChanges();
        });
      }
    }

    public IEnumerable<DT.ResourcePermission> GetResourcePermissions(Guid resourceId) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetResourcePermissions")) {
        var resourcePermissionDao = pm.ResourcePermissionDao;
        return pm.UseTransaction(() => resourcePermissionDao.GetByResourceId(resourceId)
          .Select(x => x.ToDto())
          .ToList()
        );
      }
    }
    #endregion

    #region Slave Methods
    public Guid AddSlave(DT.Slave slaveDto) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator);
      var pm = PersistenceManager;
      using (new PerformanceLogger("AddSlave")) {
        var slaveDao = pm.SlaveDao;
        return pm.UseTransaction(() => {
          var slave = slaveDao.Save(slaveDto.ToEntity());
          pm.SubmitChanges();
          return slave.ResourceId;
        });
      }
    }

    public Guid AddSlaveGroup(DT.SlaveGroup slaveGroupDto) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      var pm = PersistenceManager;
      using (new PerformanceLogger("AddSlaveGroup")) {
        var slaveGroupDao = pm.SlaveGroupDao;
        return pm.UseTransaction(() => {
          if (slaveGroupDto.Id == Guid.Empty) {
            slaveGroupDto.Id = Guid.NewGuid();
          }
          var slaveGroup = slaveGroupDao.Save(slaveGroupDto.ToEntity());
          pm.SubmitChanges();
          return slaveGroup.ResourceId;
        });
      }
    }

    public DT.Slave GetSlave(Guid slaveId) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetSlave")) {
        var slaveDao = pm.SlaveDao;
        return pm.UseTransaction(() => slaveDao.GetById(slaveId).ToDto());
      }
    }

    public IEnumerable<DT.Slave> GetSlaves() {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      bool isAdministrator = RoleVerifier.IsInRole(HiveRoles.Administrator);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetSlaves")) {
        var slaveDao = pm.SlaveDao;
        var resourcePermissionDao = pm.ResourcePermissionDao;
        var currentUserId = UserManager.CurrentUserId;
        return pm.UseTransaction(() => {
          var resourcePermissions = resourcePermissionDao.GetAll();
          return slaveDao.GetAll()
            .Where(x => isAdministrator
              || x.OwnerUserId == null
              || x.OwnerUserId == currentUserId
              || UserManager.VerifyUser(currentUserId, resourcePermissions
                  .Where(y => y.ResourceId == x.ResourceId)
                  .Select(z => z.GrantedUserId)
                  .ToList())
              )
            .Select(x => x.ToDto())
            .ToList();
        });
      }
    }

    public IEnumerable<DT.SlaveGroup> GetSlaveGroups() {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      bool isAdministrator = RoleVerifier.IsInRole(HiveRoles.Administrator);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetSlaveGroups")) {
        var slaveGroupDao = pm.SlaveGroupDao;
        var resourcePermissionDao = pm.ResourcePermissionDao;
        var currentUserId = UserManager.CurrentUserId;
        return pm.UseTransaction(() => {
          var resourcePermissions = resourcePermissionDao.GetAll();
          return slaveGroupDao.GetAll()
            .Where(x => isAdministrator
              || x.OwnerUserId == null
              || x.OwnerUserId == currentUserId
              || UserManager.VerifyUser(currentUserId, resourcePermissions
                  .Where(y => y.ResourceId == x.ResourceId)
                  .Select(z => z.GrantedUserId)
                  .ToList())
              )
            .Select(x => x.ToDto())
            .ToList();
        });
      }
    }

    public void UpdateSlave(DT.Slave slaveDto) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      var pm = PersistenceManager;
      using (new PerformanceLogger("UpdateSlave")) {
        var slaveDao = pm.SlaveDao;
        pm.UseTransaction(() => {
          var slave = slaveDao.GetById(slaveDto.Id);
          if (slave != null) {
            slaveDto.CopyToEntity(slave);
          } else {
            slaveDao.Save(slaveDto.ToEntity());
          }
          pm.SubmitChanges();
        });
      }
    }

    public void UpdateSlaveGroup(DT.SlaveGroup slaveGroupDto) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      var pm = PersistenceManager;
      using (new PerformanceLogger("UpdateSlaveGroup")) {
        var slaveGroupDao = pm.SlaveGroupDao;
        pm.UseTransaction(() => {
          var slaveGroup = slaveGroupDao.GetById(slaveGroupDto.Id);
          if (slaveGroup != null) {
            slaveGroupDto.CopyToEntity(slaveGroup);
          } else {
            slaveGroupDao.Save(slaveGroupDto.ToEntity());
          }
          pm.SubmitChanges();
        });
      }
    }

    public void DeleteSlave(Guid slaveId) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      AuthorizationManager.AuthorizeForResourceAdministration(slaveId);
      var pm = PersistenceManager;
      using (new PerformanceLogger("DeleteSlave")) {
        var slaveDao = pm.SlaveDao;
        pm.UseTransaction(() => {
          slaveDao.Delete(slaveId);
          pm.SubmitChanges();
        });
      }
    }

    public void DeleteSlaveGroup(Guid slaveGroupId) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      AuthorizationManager.AuthorizeForResourceAdministration(slaveGroupId);
      var pm = PersistenceManager;
      using (new PerformanceLogger("DeleteSlaveGroup")) {
        var slaveGroupDao = pm.SlaveGroupDao;
        pm.UseTransaction(() => {
          slaveGroupDao.Delete(slaveGroupId);
          pm.SubmitChanges();
        });
      }
    }

    public void AddResourceToGroup(Guid slaveGroupId, Guid resourceId) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator);
      var pm = PersistenceManager;
      using (new PerformanceLogger("AddResourceToGroup")) {
        var resourceDao = pm.ResourceDao;
        pm.UseTransaction(() => {
          var resource = resourceDao.GetById(resourceId);
          resource.ParentResourceId = slaveGroupId;
          pm.SubmitChanges();
        });
      }
    }

    public void RemoveResourceFromGroup(Guid slaveGroupId, Guid resourceId) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator);
      var pm = PersistenceManager;
      using (new PerformanceLogger("RemoveResourceFromGroup")) {
        var resourceDao = pm.ResourceDao;
        pm.UseTransaction(() => {
          var resource = resourceDao.GetById(resourceId);
          resource.ParentResourceId = null;
          pm.SubmitChanges();
        });
      }
    }

    public Guid GetResourceId(string resourceName) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetResourceId")) {
        var resourceDao = pm.ResourceDao;
        return pm.UseTransaction(() => {
          var resource = resourceDao.GetByName(resourceName);
          return resource != null ? resource.ResourceId : Guid.Empty;
        });
      }
    }

    public void TriggerEventManager(bool force) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator);
      // use a serializable transaction here to ensure not two threads execute this simultaniously (mutex-lock would not work since IIS may use multiple AppDomains)
      bool cleanup;
      var pm = PersistenceManager;
      using (new PerformanceLogger("TriggerEventManager")) {
        cleanup = false;
        var lifecycleDao = pm.LifecycleDao;
        pm.UseTransaction(() => {
          var lastLifecycle = lifecycleDao.GetLastLifecycle();
          DateTime lastCleanup = lastLifecycle != null ? lastLifecycle.LastCleanup : DateTime.MinValue;
          if (force || DateTime.Now - lastCleanup > HeuristicLab.Services.Hive.Properties.Settings.Default.CleanupInterval) {
            lifecycleDao.UpdateLifecycle();
            cleanup = true;
            pm.SubmitChanges();
          }
        }, true);
      }
      if (cleanup) {
        EventManager.Cleanup();
      }
    }

    public int GetNewHeartbeatInterval(Guid slaveId) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Slave);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetNewHeartbeatInterval")) {
        var slaveDao = pm.SlaveDao;
        return pm.UseTransaction(() => {
          var slave = slaveDao.GetById(slaveId);
          if (slave != null) {
            return slave.HbInterval;
          }
          return -1;
        });
      }
    }
    #endregion

    #region Downtime Methods
    public Guid AddDowntime(DT.Downtime downtimeDto) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      AuthorizationManager.AuthorizeForResourceAdministration(downtimeDto.ResourceId);
      var pm = PersistenceManager;
      using (new PerformanceLogger("AddDowntime")) {
        var downtimeDao = pm.DowntimeDao;
        return pm.UseTransaction(() => {
          var downtime = downtimeDao.Save(downtimeDto.ToEntity());
          pm.SubmitChanges();
          return downtime.ResourceId;
        });
      }
    }

    public void DeleteDowntime(Guid downtimeId) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      var pm = PersistenceManager;
      using (new PerformanceLogger("DeleteDowntime")) {
        var downtimeDao = pm.DowntimeDao;
        pm.UseTransaction(() => {
          downtimeDao.Delete(downtimeId);
          pm.SubmitChanges();
        });
      }
    }

    public void UpdateDowntime(DT.Downtime downtimeDto) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      AuthorizationManager.AuthorizeForResourceAdministration(downtimeDto.ResourceId);
      var pm = PersistenceManager;
      using (new PerformanceLogger("UpdateDowntime")) {
        var downtimeDao = pm.DowntimeDao;
        pm.UseTransaction(() => {
          var downtime = downtimeDao.GetById(downtimeDto.Id);
          if (downtime != null) {
            downtimeDto.CopyToEntity(downtime);
          } else {
            downtimeDao.Save(downtimeDto.ToEntity());
          }
          pm.SubmitChanges();
        });
      }
    }

    public IEnumerable<DT.Downtime> GetDowntimesForResource(Guid resourceId) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetDowntimesForResource")) {
        var downtimeDao = pm.DowntimeDao;
        return pm.UseTransaction(() => downtimeDao.GetByResourceId(resourceId)
          .Select(x => x.ToDto())
          .ToList()
        );
      }
    }
    #endregion

    #region User Methods
    public string GetUsernameByUserId(Guid userId) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      var user = UserManager.GetUserById(userId);
      return user != null ? user.UserName : null;
    }

    public Guid GetUserIdByUsername(string username) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      var user = ServiceLocator.Instance.UserManager.GetUserByName(username);
      return user != null ? (Guid?)user.ProviderUserKey ?? Guid.Empty : Guid.Empty;
    }
    #endregion

    #region UserPriorities Methods
    public IEnumerable<DT.UserPriority> GetUserPriorities() {
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetUserPriorities")) {
        var userPriorityDao = pm.UserPriorityDao;
        return pm.UseTransaction(() => userPriorityDao.GetAll()
          .Select(x => x.ToDto())
          .ToList()
        );
      }
    }
    #endregion

    #region Private Helper Methods
    private void UpdateTaskState(IPersistenceManager pm, DA.Task task, DT.TaskState taskState, Guid? slaveId, Guid? userId, string exception) {
      var stateLogDao = pm.StateLogDao;
      var taskStateEntity = taskState.ToEntity();
      if (task.Command == DA.Command.Pause && task.State == DA.TaskState.Paused
          || task.Command == DA.Command.Abort && task.State == DA.TaskState.Aborted
          || task.Command == DA.Command.Stop && task.State == DA.TaskState.Aborted) {
        task.Command = null;
      } else if (taskStateEntity == DA.TaskState.Paused && task.Command != null) {
        // slave paused and uploaded the task (no user-command) -> set waiting.
        taskStateEntity = DA.TaskState.Waiting;
      }
      stateLogDao.Save(new DA.StateLog {
        State = taskStateEntity,
        DateTime = DateTime.Now,
        TaskId = task.TaskId,
        UserId = userId,
        SlaveId = slaveId,
        Exception = exception
      });
      task.State = taskStateEntity;
    }

    private DA.Resource AuthorizeForResource(IPersistenceManager pm, Guid resourceId) {
      var resourceDao = pm.ResourceDao;
      var resource = resourceDao.GetById(resourceId);
      if (resource == null) throw new SecurityException("Not authorized");
      if (resource.OwnerUserId != UserManager.CurrentUserId
          && !RoleVerifier.IsInRole(HiveRoles.Administrator)) {
        throw new SecurityException("Not authorized");
      }
      return resource;
    }
    #endregion
  }
}
