#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
    private const string NOT_AUTHORIZED_PROJECTRESOURCE = "Selected project is not authorized to access the requested resource";
    private const string NOT_AUTHORIZED_USERPROJECT = "Current user is not authorized to access the requested project";
    private const string NOT_AUTHORIZED_PROJECTOWNER = "The set user is not authorized to own the project";
    private const string NO_JOB_UPDATE_POSSIBLE = "This job has already been flagged for deletion, thus, it can not be updated anymore.";

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

    public Guid AddTask(DT.Task task, DT.TaskData taskData) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      AuthorizationManager.AuthorizeForJob(task.JobId, DT.Permission.Full);
      var pm = PersistenceManager;
      using (new PerformanceLogger("AddTask")) {
        var taskDao = pm.TaskDao;
        var stateLogDao = pm.StateLogDao;
        var newTask = task.ToEntity();
        newTask.JobData = taskData.ToEntity();
        newTask.JobData.LastUpdate = DateTime.Now;
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
      task.ParentTaskId = parentTaskId;
      return AddTask(task, taskData);
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
          } else if (task.State != DA.TaskState.Paused
                     && task.State != DA.TaskState.Aborted
                     && task.State != DA.TaskState.Finished
                     && task.State != DA.TaskState.Failed) {
            UpdateTaskState(pm, task, DT.TaskState.Paused, null, null, string.Empty);
          }
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
            .Where(x => x.State == DA.JobState.Online
                          && (x.OwnerUserId == currentUserId
                            || x.JobPermissions.Count(y => y.Permission != DA.Permission.NotAllowed
                              && y.GrantedUserId == currentUserId) > 0)
                          )
            .Select(x => x.ToDto())
            .ToList();

          EvaluateJobs(pm, jobs);
          return jobs;
        });
      }
    }

    public IEnumerable<DT.Job> GetJobsByProjectId(Guid projectId) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetJobsByProjectId")) {
        var currentUserId = UserManager.CurrentUserId;
        var projectDao = pm.ProjectDao;
        var jobDao = pm.JobDao;

        return pm.UseTransaction(() => {
          // check if user is granted to administer the requested projectId
          bool isAdmin = RoleVerifier.IsInRole(HiveRoles.Administrator);
          List<DA.Project> administrationGrantedProjects;
          if (isAdmin) {
            administrationGrantedProjects = projectDao.GetAll().ToList();
          } else {
            administrationGrantedProjects = projectDao
              .GetAdministrationGrantedProjectsForUser(currentUserId)              
              .ToList();
          }

          if (administrationGrantedProjects.Select(x => x.ProjectId).Contains(projectId)) {
            var jobs = jobDao.GetByProjectId(projectId)
            .Select(x => x.ToDto())
            .ToList();

            EvaluateJobs(pm, jobs);
            return jobs;
          } else {
            return Enumerable.Empty<DT.Job>();
          }
        });
      }
    }

    public IEnumerable<DT.Job> GetJobsByProjectIds(IEnumerable<Guid> projectIds) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetJobsByProjectIds")) {
        var currentUserId = UserManager.CurrentUserId;
        var projectDao = pm.ProjectDao;
        var jobDao = pm.JobDao;
        return pm.UseTransaction(() => {
          // check for which of requested projectIds the user is granted to administer
          bool isAdmin = RoleVerifier.IsInRole(HiveRoles.Administrator);
          List<Guid> administrationGrantedProjectIds;
          if (isAdmin) {
            administrationGrantedProjectIds = projectDao.GetAll().Select(x => x.ProjectId).ToList();
          } else {
            administrationGrantedProjectIds = projectDao
              .GetAdministrationGrantedProjectsForUser(currentUserId)
              .Select(x => x.ProjectId)
              .ToList();
          }
          var requestedAndGrantedProjectIds = projectIds.Intersect(administrationGrantedProjectIds);

          if (requestedAndGrantedProjectIds.Any()) {
            var jobs = jobDao.GetByProjectIds(requestedAndGrantedProjectIds)
              .Select(x => x.ToDto())
              .ToList();

            EvaluateJobs(pm, jobs);
            return jobs;
          } else {
            return Enumerable.Empty<DT.Job>();
          }
        });
      }
    }

    public Guid AddJob(DT.Job jobDto, IEnumerable<Guid> resourceIds) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      var dateCreated = DateTime.Now;
      var pm = PersistenceManager;

      // check project availability (cf. duration)
      CheckProjectAvailability(pm, jobDto.ProjectId, dateCreated);

      // check user - project
      AuthorizationManager.AuthorizeUserForProjectUse(UserManager.CurrentUserId, jobDto.ProjectId);

      // check project - resources
      AuthorizationManager.AuthorizeProjectForResourcesUse(jobDto.ProjectId, resourceIds);

      using (new PerformanceLogger("AddJob")) {
        var jobDao = pm.JobDao;
        var userPriorityDao = pm.UserPriorityDao;

        return pm.UseTransaction(() => {
          var newJob = jobDto.ToEntity();
          newJob.OwnerUserId = UserManager.CurrentUserId;
          newJob.DateCreated = dateCreated;

          // add resource assignments
          if (resourceIds != null && resourceIds.Any()) {
            newJob.AssignedJobResources.AddRange(resourceIds.Select(
              x => new DA.AssignedJobResource {
                ResourceId = x
              }));
          }

          var job = jobDao.Save(newJob);
          if (userPriorityDao.GetById(newJob.OwnerUserId) == null) {
            userPriorityDao.Save(new DA.UserPriority {
              UserId = newJob.OwnerUserId,
              DateEnqueued = newJob.DateCreated
            });
          }
          pm.SubmitChanges();
          return job.JobId;
        });
      }
    }

    public void UpdateJob(DT.Job jobDto, IEnumerable<Guid> resourceIds) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      AuthorizationManager.AuthorizeForJob(jobDto.Id, DT.Permission.Full);
      var pm = PersistenceManager;
      var dateUpdated = DateTime.Now;

      // check project availability
      CheckProjectAvailability(pm, jobDto.ProjectId, dateUpdated);
      // check user - project permission
      AuthorizationManager.AuthorizeUserForProjectUse(UserManager.CurrentUserId, jobDto.ProjectId);
      // check project - resources permission
      AuthorizationManager.AuthorizeProjectForResourcesUse(jobDto.ProjectId, resourceIds);

      using (new PerformanceLogger("UpdateJob")) {
        bool exists = true;
        var jobDao = pm.JobDao;
        pm.UseTransaction(() => {
          var job = jobDao.GetById(jobDto.Id);
          if (job == null) {
            exists = false;
            job = new DA.Job();
          } else if (job.State != DA.JobState.Online) {
            throw new InvalidOperationException(NO_JOB_UPDATE_POSSIBLE);
          }


          jobDto.CopyToEntity(job);

          if (!exists) {
            // add resource assignments
            if (resourceIds != null && resourceIds.Any()) {
              job.AssignedJobResources.AddRange(resourceIds.Select(
                x => new DA.AssignedJobResource {
                  ResourceId = x
                }));
            }
            jobDao.Save(job);
          } else if (resourceIds != null) {
            var addedJobResourceIds = resourceIds.Except(job.AssignedJobResources.Select(x => x.ResourceId));
            var removedJobResourceIds = job.AssignedJobResources
              .Select(x => x.ResourceId)
              .Except(resourceIds)
              .ToArray();

            // remove resource assignments
            foreach (var rid in removedJobResourceIds) {
              var ajr = job.AssignedJobResources.Where(x => x.ResourceId == rid).SingleOrDefault();
              if (ajr != null) job.AssignedJobResources.Remove(ajr);
            }

            // add resource assignments
            job.AssignedJobResources.AddRange(addedJobResourceIds.Select(
              x => new DA.AssignedJobResource {
                ResourceId = x
              }));
          }
          pm.SubmitChanges();
        });
      }
    }

    public void UpdateJobState(Guid jobId, DT.JobState jobState) {
      if (jobState != JobState.StatisticsPending) return; // only process requests for "StatisticsPending"

      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      bool isAdministrator = RoleVerifier.IsInRole(HiveRoles.Administrator);
      var currentUserId = UserManager.CurrentUserId;

      var pm = PersistenceManager;
      using (new PerformanceLogger("UpdateJobState")) {
        var jobDao = pm.JobDao;
        var projectDao = pm.ProjectDao;
        pm.UseTransaction(() => {
          var job = jobDao.GetById(jobId);
          if (job != null) {

            var administrationGrantedProjects = projectDao
              .GetAdministrationGrantedProjectsForUser(currentUserId)
              .ToList();

            // check if user is an admin, or granted to administer a job-parenting project,...
            if (!isAdministrator && !administrationGrantedProjects.Contains(job.Project))
              AuthorizationManager.AuthorizeForJob(jobId, DT.Permission.Full); // ... or job owner

            // note: allow solely state changes from "Online" to "StatisticsPending" = deletion request by user for HiveStatisticGenerator            
            var jobStateEntity = jobState.ToEntity();
            if (job.State == DA.JobState.Online && jobStateEntity == DA.JobState.StatisticsPending) {
              job.State = jobStateEntity;
              foreach (var task in job.Tasks
              .Where(x => x.State == DA.TaskState.Waiting
                || x.State == DA.TaskState.Paused
                || x.State == DA.TaskState.Offline)) {
                task.State = DA.TaskState.Aborted;
              }
              pm.SubmitChanges();
            }
          }
        });
      }
    }

    public void UpdateJobStates(IEnumerable<Guid> jobIds, DT.JobState jobState) {
      if (jobState != JobState.StatisticsPending) return; // only process requests for "StatisticsPending"

      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      bool isAdministrator = RoleVerifier.IsInRole(HiveRoles.Administrator);
      var currentUserId = UserManager.CurrentUserId;

      var pm = PersistenceManager;
      using (new PerformanceLogger("UpdateJobStates")) {
        var jobDao = pm.JobDao;
        var projectDao = pm.ProjectDao;
        pm.UseTransaction(() => {
          var administrationGrantedProjects = projectDao
            .GetAdministrationGrantedProjectsForUser(currentUserId)
            .ToList();

          foreach (var jobId in jobIds) {
            var job = jobDao.GetById(jobId);
            if (job != null) {

              // check if user is an admin, or granted to administer a job-parenting project,...
              if (!isAdministrator && !administrationGrantedProjects.Contains(job.Project))
                AuthorizationManager.AuthorizeForJob(jobId, DT.Permission.Full); // ... or job owner

              // note: allow solely state changes from "Online" to "StatisticsPending" = deletion request by user for HiveStatisticGenerator
              var jobStateEntity = jobState.ToEntity();
              if (job.State == DA.JobState.Online && jobStateEntity == DA.JobState.StatisticsPending) {
                job.State = jobStateEntity;
                foreach (var task in job.Tasks
                .Where(x => x.State == DA.TaskState.Waiting
                  || x.State == DA.TaskState.Paused
                  || x.State == DA.TaskState.Offline)) {
                  task.State = DA.TaskState.Aborted;
                }
                pm.SubmitChanges();
              }              
            }
          }
        });
      }
    }

    public IEnumerable<DT.AssignedJobResource> GetAssignedResourcesForJob(Guid jobId) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      AuthorizationManager.AuthorizeForJob(jobId, DT.Permission.Full);
      var pm = PersistenceManager;
      var assignedJobResourceDao = pm.AssignedJobResourceDao;
      using (new PerformanceLogger("GetAssignedResourcesForProject")) {
        return pm.UseTransaction(() =>
          assignedJobResourceDao.GetByJobId(jobId)
          .Select(x => x.ToDto())
          .ToList()
        );
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
      } catch (Exception ex) {
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

    #region Project Methods
    public Guid AddProject(DT.Project projectDto) {
      if (projectDto == null || projectDto.Id != Guid.Empty) return Guid.Empty;
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      // check if current (non-admin) user is owner of one of projectDto's-parents
      // note: non-admin users are not allowed to administer root projects (i.e. projects without parental entry)
      bool isAdmin = RoleVerifier.IsInRole(HiveRoles.Administrator);
      if (!isAdmin) {
        if (projectDto != null && projectDto.ParentProjectId.HasValue) {
          AuthorizationManager.AuthorizeForProjectAdministration(projectDto.ParentProjectId.Value, false);
        } else {
          throw new SecurityException(NOT_AUTHORIZED_USERPROJECT);
        }
      }

      // check that non-admins can not be set as owner of root projects
      if (projectDto != null && !projectDto.ParentProjectId.HasValue) {
        var owner = UserManager.GetUserById(projectDto.OwnerUserId);
        if (owner == null || !RoleVerifier.IsUserInRole(owner.UserName, HiveRoles.Administrator)) {
          throw new SecurityException(NOT_AUTHORIZED_PROJECTOWNER);
        }
      }

      var pm = PersistenceManager;
      using (new PerformanceLogger("AddProject")) {
        var projectDao = pm.ProjectDao;

        return pm.UseTransaction(() => {
          var project = projectDao.Save(projectDto.ToEntity());

          var parentProjects = projectDao.GetParentProjectsById(project.ProjectId).ToList();
          bool isParent = parentProjects.Select(x => x.OwnerUserId == UserManager.CurrentUserId).Any();

          // if user is no admin, but owner of a parent project
          // check start/end date time boundaries of parent projects before updating child project
          if (!isAdmin) {
            var parentProject = parentProjects.Where(x => x.ProjectId == project.ParentProjectId).FirstOrDefault();
            if (parentProject != null) {
              if (project.StartDate < parentProject.StartDate) project.StartDate = parentProject.StartDate;
              if ((parentProject.EndDate.HasValue && project.EndDate.HasValue && project.EndDate > parentProject.EndDate)
              || (parentProject.EndDate.HasValue && !project.EndDate.HasValue))
                project.EndDate = parentProject.EndDate;
            }
          }


          project.ProjectPermissions.Clear();
          project.ProjectPermissions.Add(new DA.ProjectPermission {
            GrantedUserId = project.OwnerUserId,
            GrantedByUserId = UserManager.CurrentUserId
          });

          pm.SubmitChanges();
          return project.ProjectId;
        });
      }
    }

    public void UpdateProject(DT.Project projectDto) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      // check if current (non-admin) user is owner of the project or the projectDto's-parents
      // note: non-admin users are not allowed to administer root projects (i.e. projects without parental entry)
      bool isAdmin = RoleVerifier.IsInRole(HiveRoles.Administrator);
      if (!isAdmin) {
        if (projectDto != null && projectDto.ParentProjectId.HasValue) {
          AuthorizationManager.AuthorizeForProjectAdministration(projectDto.Id, false);
        } else {
          throw new SecurityException(NOT_AUTHORIZED_USERPROJECT);
        }
      }

      // check that non-admins can not be set as owner of root projects
      if (projectDto != null && !projectDto.ParentProjectId.HasValue) {
        var owner = UserManager.GetUserById(projectDto.OwnerUserId);
        if (owner == null || !RoleVerifier.IsUserInRole(owner.UserName, HiveRoles.Administrator)) {
          throw new SecurityException(NOT_AUTHORIZED_PROJECTOWNER);
        }
      }

      var pm = PersistenceManager;
      using (new PerformanceLogger("UpdateProject")) {
        var projectDao = pm.ProjectDao;
        var assignedJobResourceDao = pm.AssignedJobResourceDao;
        pm.UseTransaction(() => {
          var project = projectDao.GetById(projectDto.Id);
          if (project != null) { // (1) update existent project
            var parentProjects = projectDao.GetParentProjectsById(project.ProjectId).ToList();
            bool isParent = parentProjects.Select(x => x.OwnerUserId == UserManager.CurrentUserId).Any();

            var formerOwnerId = project.OwnerUserId;
            var formerStartDate = project.StartDate;
            var formerEndDate = project.EndDate;
            projectDto.CopyToEntity(project);

            // if user is no admin, but owner of parent project(s)
            // check start/end date time boundaries of parent projects before updating child project
            if (!isAdmin && isParent) {
              var parentProject = parentProjects.Where(x => x.ProjectId == project.ParentProjectId).FirstOrDefault();
              if (parentProject != null) {
                if (project.StartDate < parentProject.StartDate) project.StartDate = formerStartDate;
                if ((parentProject.EndDate.HasValue && project.EndDate.HasValue && project.EndDate > parentProject.EndDate)
                || (parentProject.EndDate.HasValue && !project.EndDate.HasValue))
                  project.EndDate = formerEndDate;
              }
            }

            // if user is admin or owner of parent project(s)
            if (isAdmin || isParent) {
              // if owner has changed...
              if (formerOwnerId != projectDto.OwnerUserId) {
                // Add permission for new owner if not already done
                if (!project.ProjectPermissions
                  .Select(pp => pp.GrantedUserId)
                  .Contains(projectDto.OwnerUserId)) {
                  project.ProjectPermissions.Add(new DA.ProjectPermission {
                    GrantedUserId = projectDto.OwnerUserId,
                    GrantedByUserId = UserManager.CurrentUserId
                  });
                }
              }
            } else { // if user is only owner of current project, but no admin and no owner of parent project(s)
              project.OwnerUserId = formerOwnerId;
              project.StartDate = formerStartDate;
              project.EndDate = formerEndDate;
            }

          } else { // (2) save new project
            var newProject = projectDao.Save(projectDto.ToEntity());

            var parentProjects = projectDao.GetParentProjectsById(project.ProjectId).ToList();
            bool isParent = parentProjects.Select(x => x.OwnerUserId == UserManager.CurrentUserId).Any();

            // if user is no admin, but owner of a parent project
            // check start/end date time boundaries of parent projects before updating child project
            if (!isAdmin) {
              var parentProject = parentProjects.Where(x => x.ProjectId == project.ParentProjectId).FirstOrDefault();
              if (parentProject != null) {
                if (project.StartDate < parentProject.StartDate) project.StartDate = parentProject.StartDate;
                if ((parentProject.EndDate.HasValue && project.EndDate.HasValue && project.EndDate > parentProject.EndDate)
                || (parentProject.EndDate.HasValue && !project.EndDate.HasValue))
                  project.EndDate = parentProject.EndDate;
              }
            }

            newProject.ProjectPermissions.Clear();
            newProject.ProjectPermissions.Add(new DA.ProjectPermission {
              GrantedUserId = projectDto.OwnerUserId,
              GrantedByUserId = UserManager.CurrentUserId
            });
          }

          pm.SubmitChanges();
        });
      }
    }

    public void DeleteProject(Guid projectId) {
      if (projectId == Guid.Empty) return;
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      // check if current (non-admin) user is owner of one of the projectDto's-parents
      // note: non-admin users are not allowed to administer root projects (i.e. projects without parental entry)
      if (!RoleVerifier.IsInRole(HiveRoles.Administrator)) {
        AuthorizationManager.AuthorizeForProjectAdministration(projectId, true);
      }

      var pm = PersistenceManager;
      using (new PerformanceLogger("DeleteProject")) {
        var projectDao = pm.ProjectDao;
        var jobDao = pm.JobDao;
        var assignedJobResourceDao = pm.AssignedJobResourceDao;
        pm.UseTransaction(() => {
          var projectIds = new HashSet<Guid> { projectId };
          projectIds.Union(projectDao.GetChildProjectIdsById(projectId));

          var jobs = jobDao.GetByProjectIds(projectIds)
            .Select(x => x.ToDto())
            .ToList();

          if (jobs.Count > 0) {
            throw new InvalidOperationException("There are " + jobs.Count + " job(s) using this project and/or child-projects. It is necessary to delete them before the project.");
          } else {
            assignedJobResourceDao.DeleteByProjectIds(projectIds);
            projectDao.DeleteByIds(projectIds);
            pm.SubmitChanges();
          }
        });
      }
    }

    // query granted project for use (i.e. to calculate on)
    public DT.Project GetProject(Guid projectId) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetProject")) {
        var projectDao = pm.ProjectDao;
        var currentUserId = UserManager.CurrentUserId;
        var userAndGroupIds = new List<Guid> { currentUserId };
        userAndGroupIds.AddRange(UserManager.GetUserGroupIdsOfUser(currentUserId));
        return pm.UseTransaction(() => {
          return projectDao.GetUsageGrantedProjectsForUser(userAndGroupIds)
          .Where(x => x.ProjectId == projectId)
          .Select(x => x.ToDto())
          .SingleOrDefault();
        });
      }
    }

    // query granted projects for use (i.e. to calculate on)
    public IEnumerable<DT.Project> GetProjects() {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetProjects")) {
        var projectDao = pm.ProjectDao;
        var currentUserId = UserManager.CurrentUserId;
        var userAndGroupIds = new List<Guid> { currentUserId };
        userAndGroupIds.AddRange(UserManager.GetUserGroupIdsOfUser(currentUserId));
        return pm.UseTransaction(() => {
          var projects = projectDao.GetUsageGrantedProjectsForUser(userAndGroupIds)
            .Select(x => x.ToDto()).ToList();
          var now = DateTime.Now;
          return projects.Where(x => x.StartDate <= now && (x.EndDate == null || x.EndDate >= now)).ToList();
        });
      }
    }

    public IEnumerable<DT.Project> GetProjectsForAdministration() {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      bool isAdministrator = RoleVerifier.IsInRole(HiveRoles.Administrator);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetProjectsForAdministration")) {
        var projectDao = pm.ProjectDao;

        return pm.UseTransaction(() => {
          if (isAdministrator) {
            return projectDao.GetAll().Select(x => x.ToDto()).ToList();
          } else {
            var currentUserId = UserManager.CurrentUserId;
            return projectDao.GetAdministrationGrantedProjectsForUser(currentUserId)
              .Select(x => x.ToDto()).ToList();

          }
        });
      }
    }

    public IDictionary<Guid, HashSet<Guid>> GetProjectGenealogy() {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetProjectGenealogy")) {
        var projectDao = pm.ProjectDao;
        var projectAncestors = new Dictionary<Guid, HashSet<Guid>>();
        return pm.UseTransaction(() => {
          var projects = projectDao.GetAll().ToList();
          projects.ForEach(p => projectAncestors.Add(p.ProjectId, new HashSet<Guid>()));
          foreach (var p in projects) {
            var parentProject = p.ParentProject;
            while (parentProject != null) {
              projectAncestors[p.ProjectId].Add(parentProject.ProjectId);
              parentProject = parentProject.ParentProject;
            }
          }
          return projectAncestors;
        });
      }
    }

    public IDictionary<Guid, string> GetProjectNames() {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetProjectNames")) {
        var projectDao = pm.ProjectDao;
        var projectNames = new Dictionary<Guid, string>();
        return pm.UseTransaction(() => {
          projectDao
            .GetAll().ToList()
            .ForEach(p => projectNames.Add(p.ProjectId, p.Name));
          return projectNames;
        });
      }
    }
    #endregion

    #region ProjectPermission Methods
    public void SaveProjectPermissions(Guid projectId, List<Guid> grantedUserIds, bool reassign, bool cascading, bool reassignCascading) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      if (projectId == null || grantedUserIds == null) return;
      AuthorizationManager.AuthorizeForProjectAdministration(projectId, false);
      var pm = PersistenceManager;
      using (new PerformanceLogger("SaveProjectPermissions")) {
        var projectDao = pm.ProjectDao;
        var projectPermissionDao = pm.ProjectPermissionDao;
        var assignedJobResourceDao = pm.AssignedJobResourceDao;

        pm.UseTransaction(() => {
          var project = projectDao.GetById(projectId);
          if (project == null) return;
          var projectPermissions = project.ProjectPermissions.Select(x => x.GrantedUserId).ToArray();

          // guarantee that project owner is always permitted
          if (!grantedUserIds.Contains(project.OwnerUserId)) {
            grantedUserIds.Add(project.OwnerUserId);
          }

          //var addedPermissions = grantedUserIds.Except(projectPermissions);
          var removedPermissions = projectPermissions.Except(grantedUserIds);

          // remove job assignments and project permissions
          if (reassign) {
            assignedJobResourceDao.DeleteByProjectId(project.ProjectId);
            project.ProjectPermissions.Clear();
          } else {

            var ugt = GetUserGroupTree();
            var permittedGuids = new HashSet<Guid>(); // User- and Group-Guids
            var notpermittedGuids = new HashSet<Guid>();

            // remove job assignments:
            // (1) get all member-Guids of all still or fresh permitted user/groups
            foreach (var item in grantedUserIds) {
              permittedGuids.Add(item);
              if (ugt.ContainsKey(item)) {
                ugt[item].ToList().ForEach(x => permittedGuids.Add(x));
              }
            }

            // (2) get all member-Guids of users and groups in removedPermissions
            foreach (var item in removedPermissions) {
              notpermittedGuids.Add(item);
              if (ugt.ContainsKey(item)) {
                ugt[item].ToList().ForEach(x => notpermittedGuids.Add(x));
              }
            }

            // (3) get all Guids which are in removedPermissions but not in grantedUserIds
            var definitelyNotPermittedGuids = notpermittedGuids.Except(permittedGuids);

            // (4) delete jobs of those
            assignedJobResourceDao.DeleteByProjectIdAndUserIds(project.ProjectId, definitelyNotPermittedGuids);


            // remove project permissions
            foreach (var item in project.ProjectPermissions
              .Where(x => removedPermissions.Contains(x.GrantedUserId))
              .ToList()) {
              project.ProjectPermissions.Remove(item);
            }
          }
          pm.SubmitChanges();

          // add project permissions
          foreach (var id in grantedUserIds) {
            if (project.ProjectPermissions.All(x => x.GrantedUserId != id)) {
              project.ProjectPermissions.Add(new DA.ProjectPermission {
                GrantedUserId = id,
                GrantedByUserId = UserManager.CurrentUserId
              });
            }
          }
          pm.SubmitChanges();

          if (cascading) {
            var childProjects = projectDao.GetChildProjectsById(projectId).ToList();
            var childProjectIds = childProjects.Select(x => x.ProjectId).ToList();

            // remove job assignments
            if (reassignCascading) {
              assignedJobResourceDao.DeleteByProjectIds(childProjectIds);
            } else {
              assignedJobResourceDao.DeleteByProjectIdsAndUserIds(childProjectIds, removedPermissions);
            }

            foreach (var p in childProjects) {
              var cpAssignedPermissions = p.ProjectPermissions.Select(x => x.GrantedUserId).ToList();
              // guarantee that project owner is always permitted
              if (!cpAssignedPermissions.Contains(p.OwnerUserId)) {
                cpAssignedPermissions.Add(p.OwnerUserId);
              }
              var cpRemovedPermissions = cpAssignedPermissions.Where(x => x != p.OwnerUserId).Except(grantedUserIds);

              // remove left-over job assignments (for non-reassignments)
              if (!reassignCascading) {
                assignedJobResourceDao.DeleteByProjectIdAndUserIds(p.ProjectId, cpRemovedPermissions);
              }

              // remove project permissions
              if (reassignCascading) {
                p.ProjectPermissions.Clear();
              } else {
                foreach (var item in p.ProjectPermissions
                  .Where(x => x.GrantedUserId != p.OwnerUserId
                    && (removedPermissions.Contains(x.GrantedUserId) || cpRemovedPermissions.Contains(x.GrantedUserId)))
                  .ToList()) {
                  p.ProjectPermissions.Remove(item);
                }
              }
              pm.SubmitChanges();

              // add project permissions
              var cpGrantedUserIds = new HashSet<Guid>(grantedUserIds);
              cpGrantedUserIds.Add(p.OwnerUserId);

              foreach (var id in cpGrantedUserIds) {
                if (p.ProjectPermissions.All(x => x.GrantedUserId != id)) {
                  p.ProjectPermissions.Add(new DA.ProjectPermission {
                    GrantedUserId = id,
                    GrantedByUserId = UserManager.CurrentUserId
                  });
                }
              }
            }
          }
          pm.SubmitChanges();
        });
      }
    }

    //private void GrantProjectPermissions(Guid projectId, List<Guid> grantedUserIds, bool cascading) {
    //  throw new NotImplementedException();
    //}

    //private void RevokeProjectPermissions(Guid projectId, List<Guid> grantedUserIds, bool cascading) {
    //  RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
    //  if (projectId == null || grantedUserIds == null || !grantedUserIds.Any()) return;
    //  AuthorizationManager.AuthorizeForProjectAdministration(projectId, false);
    //  var pm = PersistenceManager;
    //  using (new PerformanceLogger("RevokeProjectPermissions")) {
    //    var projectPermissionDao = pm.ProjectPermissionDao;
    //    var projectDao = pm.ProjectDao;
    //    var assignedJobResourceDao = pm.AssignedJobResourceDao;
    //    pm.UseTransaction(() => {
    //      if (cascading) {
    //        var childProjectIds = projectDao.GetChildProjectIdsById(projectId).ToList();
    //        projectPermissionDao.DeleteByProjectIdsAndGrantedUserIds(childProjectIds, grantedUserIds);
    //        assignedJobResourceDao.DeleteByProjectIdsAndUserIds(childProjectIds, grantedUserIds);
    //      }
    //      projectPermissionDao.DeleteByProjectIdAndGrantedUserIds(projectId, grantedUserIds);
    //      assignedJobResourceDao.DeleteByProjectIdAndUserIds(projectId, grantedUserIds);
    //      pm.SubmitChanges();
    //    });
    //  }
    //}

    public IEnumerable<DT.ProjectPermission> GetProjectPermissions(Guid projectId) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      AuthorizationManager.AuthorizeForProjectAdministration(projectId, false);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetProjectPermissions")) {
        var projectPermissionDao = pm.ProjectPermissionDao;
        return pm.UseTransaction(() => projectPermissionDao.GetByProjectId(projectId)
          .Select(x => x.ToDto())
          .ToList()
        );
      }
    }
    #endregion

    #region AssignedProjectResource Methods
    // basic: remove and add assignments (resourceIds) to projectId and its depending jobs
    // reassign: clear all assignments from project and its depending jobs, before adding new ones (resourceIds)
    // cascading: "basic" mode for child-projects
    // reassignCascading: "reassign" mode for child-projects
    public void SaveProjectResourceAssignments(Guid projectId, List<Guid> resourceIds, bool reassign, bool cascading, bool reassignCascading) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      if (projectId == null || resourceIds == null) return;
      AuthorizationManager.AuthorizeForProjectResourceAdministration(projectId, resourceIds);
      bool isAdmin = RoleVerifier.IsInRole(HiveRoles.Administrator);
      var pm = PersistenceManager;
      using (new PerformanceLogger("SaveProjectResourceAssignments")) {
        var projectDao = pm.ProjectDao;
        var assignedProjectResourceDao = pm.AssignedProjectResourceDao;
        var assignedJobResourceDao = pm.AssignedJobResourceDao;
        pm.UseTransaction(() => {
          var project = projectDao.GetById(projectId);

          var parentProjects = projectDao.GetParentProjectsById(project.ProjectId).ToList();
          bool isParent = parentProjects.Select(x => x.OwnerUserId == UserManager.CurrentUserId).Any();

          var assignedResources = project.AssignedProjectResources.Select(x => x.ResourceId).ToArray();
          if (!isParent && !isAdmin) resourceIds = assignedResources.ToList();
          var removedAssignments = assignedResources.Except(resourceIds);

          // if user is admin or owner of parent project(s)
          if (isAdmin || isParent) {
            // remove job and project assignments
            if (reassign) {
              assignedJobResourceDao.DeleteByProjectId(project.ProjectId);
              project.AssignedProjectResources.Clear();
            } else {
              assignedJobResourceDao.DeleteByProjectIdAndResourceIds(projectId, removedAssignments);
              foreach (var item in project.AssignedProjectResources
                .Where(x => removedAssignments.Contains(x.ResourceId))
                .ToList()) {
                project.AssignedProjectResources.Remove(item);
              }
            }
            pm.SubmitChanges();

            // add project assignments
            foreach (var id in resourceIds) {
              if (project.AssignedProjectResources.All(x => x.ResourceId != id)) {
                project.AssignedProjectResources.Add(new DA.AssignedProjectResource {
                  ResourceId = id
                });
              }
            }
            pm.SubmitChanges();
          }

          // if user is admin, project owner or owner of parent projects
          if (cascading) {
            var childProjects = projectDao.GetChildProjectsById(projectId).ToList();
            var childProjectIds = childProjects.Select(x => x.ProjectId).ToList();

            // remove job assignments
            if (reassignCascading) {
              assignedJobResourceDao.DeleteByProjectIds(childProjectIds);
            } else {
              assignedJobResourceDao.DeleteByProjectIdsAndResourceIds(childProjectIds, removedAssignments);
            }
            foreach (var p in childProjects) {
              var cpAssignedResources = p.AssignedProjectResources.Select(x => x.ResourceId).ToArray();
              var cpRemovedAssignments = cpAssignedResources.Except(resourceIds);

              // remove left-over job assignments (for non-reassignments)
              if (!reassignCascading) {
                assignedJobResourceDao.DeleteByProjectIdAndResourceIds(p.ProjectId, cpRemovedAssignments);
              }

              // remove project assignments
              if (reassignCascading) {
                p.AssignedProjectResources.Clear();
              } else {
                foreach (var item in p.AssignedProjectResources
                  .Where(x => removedAssignments.Contains(x.ResourceId) || cpRemovedAssignments.Contains(x.ResourceId))
                  .ToList()) {
                  p.AssignedProjectResources.Remove(item);
                }
              }
              pm.SubmitChanges();

              // add project assignments
              foreach (var id in resourceIds) {
                if (p.AssignedProjectResources.All(x => x.ResourceId != id)) {
                  p.AssignedProjectResources.Add(new DA.AssignedProjectResource {
                    ResourceId = id
                  });
                }
              }
            }
          }
          pm.SubmitChanges();
        });
      }
    }

    //private void AssignProjectResources(Guid projectId, List<Guid> resourceIds, bool cascading) {
    //  throw new NotImplementedException();
    //}

    //private void UnassignProjectResources(Guid projectId, List<Guid> resourceIds, bool cascading) {
    //  RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
    //  if (projectId == null || resourceIds == null || !resourceIds.Any()) return;
    //  AuthorizationManager.AuthorizeForProjectResourceAdministration(projectId, resourceIds);
    //  var pm = PersistenceManager;
    //  using (new PerformanceLogger("UnassignProjectResources")) {
    //    var assignedProjectResourceDao = pm.AssignedProjectResourceDao;
    //    var assignedJobResourceDao = pm.AssignedJobResourceDao;
    //    var projectDao = pm.ProjectDao;
    //    pm.UseTransaction(() => {
    //      if (cascading) {
    //        var childProjectIds = projectDao.GetChildProjectIdsById(projectId).ToList();
    //        assignedProjectResourceDao.DeleteByProjectIdsAndResourceIds(childProjectIds, resourceIds);
    //        assignedJobResourceDao.DeleteByProjectIdsAndResourceIds(childProjectIds, resourceIds);
    //      }
    //      assignedProjectResourceDao.DeleteByProjectIdAndResourceIds(projectId, resourceIds);
    //      assignedJobResourceDao.DeleteByProjectIdAndResourceIds(projectId, resourceIds);
    //      pm.SubmitChanges();
    //    });
    //  }
    //}

    public IEnumerable<DT.AssignedProjectResource> GetAssignedResourcesForProject(Guid projectId) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      AuthorizationManager.AuthorizeUserForProjectUse(UserManager.CurrentUserId, projectId);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetAssignedResourcesForProject")) {
        var assignedProjectResourceDao = pm.AssignedProjectResourceDao;
        return pm.UseTransaction(() => assignedProjectResourceDao.GetByProjectId(projectId)
          .Select(x => x.ToDto())
          .ToList()
        );
      }
    }

    public IEnumerable<DT.AssignedProjectResource> GetAssignedResourcesForProjectAdministration(Guid projectId) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      AuthorizationManager.AuthorizeForProjectAdministration(projectId, false);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetAssignedResourcesForProject")) {
        var assignedProjectResourceDao = pm.AssignedProjectResourceDao;
        return pm.UseTransaction(() => assignedProjectResourceDao.GetByProjectId(projectId)
          .Select(x => x.ToDto())
          .ToList()
        );
      }
    }

    public IEnumerable<DT.AssignedProjectResource> GetAssignedResourcesForProjectsAdministration(IEnumerable<Guid> projectIds) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      foreach (var id in projectIds)
        AuthorizationManager.AuthorizeForProjectAdministration(id, false);

      var pm = PersistenceManager;
      using (new PerformanceLogger("GetAssignedResourcesForProject")) {
        var assignedProjectResourceDao = pm.AssignedProjectResourceDao;
        var assignments = new List<DT.AssignedProjectResource>();
        pm.UseTransaction(() => {
          foreach (var id in projectIds) {
            assignments.AddRange(assignedProjectResourceDao.GetByProjectId(id)
              .Select(x => x.ToDto()));
          }
        });
        return assignments.Distinct();
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
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator);
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

    // query granted slaves for use (i.e. to calculate on)
    public IEnumerable<DT.Slave> GetSlaves() {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetSlaves")) {
        var slaveDao = pm.SlaveDao;
        var projectDao = pm.ProjectDao;
        var assignedProjectResourceDao = pm.AssignedProjectResourceDao;

        // collect user information
        var currentUserId = UserManager.CurrentUserId;
        var userAndGroupIds = new List<Guid> { currentUserId };
        userAndGroupIds.AddRange(UserManager.GetUserGroupIdsOfUser(currentUserId));

        return pm.UseTransaction(() => {
          var slaves = slaveDao.GetAll()
            .Select(x => x.ToDto())
            .ToList();
          var grantedProjectIds = projectDao.GetUsageGrantedProjectsForUser(userAndGroupIds)
            .Select(x => x.ProjectId)
            .ToList();
          var grantedResourceIds = assignedProjectResourceDao.GetAllGrantedResourcesByProjectIds(grantedProjectIds)
            .Select(x => x.ResourceId)
            .ToList();

          return slaves
            .Where(x => grantedResourceIds.Contains(x.Id))
            .ToList();
        });
      }
    }

    // query granted slave groups for use (i.e. to calculate on)
    public IEnumerable<DT.SlaveGroup> GetSlaveGroups() {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetSlaveGroups")) {
        var slaveGroupDao = pm.SlaveGroupDao;
        var projectDao = pm.ProjectDao;
        var assignedProjectResourceDao = pm.AssignedProjectResourceDao;

        // collect user information
        var currentUserId = UserManager.CurrentUserId;
        var userAndGroupIds = new List<Guid> { currentUserId };
        userAndGroupIds.AddRange(UserManager.GetUserGroupIdsOfUser(currentUserId));

        return pm.UseTransaction(() => {
          var slaveGroups = slaveGroupDao.GetAll()
            .Select(x => x.ToDto())
            .ToList();
          var grantedProjectIds = projectDao.GetUsageGrantedProjectsForUser(userAndGroupIds)
            .Select(x => x.ProjectId)
            .ToList();
          var grantedResourceIds = assignedProjectResourceDao.GetAllGrantedResourcesByProjectIds(grantedProjectIds)
            .Select(x => x.ResourceId)
            .ToList();

          return slaveGroups
            .Where(x => grantedResourceIds.Contains(x.Id))
            .ToList();
        });
      }
    }

    // query granted slaves for resource administration
    public IEnumerable<DT.Slave> GetSlavesForAdministration() {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      bool isAdministrator = RoleVerifier.IsInRole(HiveRoles.Administrator);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetSlavesForAdministration")) {
        var slaveDao = pm.SlaveDao;
        var currentUserId = UserManager.CurrentUserId;

        if (isAdministrator) {
          return pm.UseTransaction(() => {
            return slaveDao.GetAll()
              .Select(x => x.ToDto())
              .ToList();
          });
        } else {
          var slaves = slaveDao.GetAll()
            .Select(x => x.ToDto())
            .ToList();
          var projectDao = pm.ProjectDao;
          var assignedProjectResourceDao = pm.AssignedProjectResourceDao;
          var projects = projectDao.GetAdministrationGrantedProjectsForUser(currentUserId).ToList();
          var resourceIds = assignedProjectResourceDao
            .GetAllGrantedResourcesByProjectIds(projects.Select(x => x.ProjectId).ToList())
            .Select(x => x.ResourceId)
            .ToList();

          return slaves
            .Where(x => resourceIds.Contains(x.Id))
            .ToList();
        }
      }
    }

    // query granted slave groups for resource administration
    public IEnumerable<DT.SlaveGroup> GetSlaveGroupsForAdministration() {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      bool isAdministrator = RoleVerifier.IsInRole(HiveRoles.Administrator);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetSlaveGroupsForAdministration")) {
        var slaveGroupDao = pm.SlaveGroupDao;
        var currentUserId = UserManager.CurrentUserId;

        if (isAdministrator) {
          return pm.UseTransaction(() => {
            return slaveGroupDao.GetAll()
              .Select(x => x.ToDto())
              .ToList();
          });
        } else {
          var slaveGroups = slaveGroupDao.GetAll()
            .Select(x => x.ToDto())
            .ToList();
          var projectDao = pm.ProjectDao;
          var assignedProjectResourceDao = pm.AssignedProjectResourceDao;
          var projects = projectDao.GetAdministrationGrantedProjectsForUser(currentUserId).ToList();
          var resourceIds = assignedProjectResourceDao
            .GetAllGrantedResourcesByProjectIds(projects.Select(x => x.ProjectId).ToList())
            .Select(x => x.ResourceId)
            .ToList();

          return slaveGroups
            .Where(x => resourceIds.Contains(x.Id))
            .ToList();
        }
      }
    }

    public IDictionary<Guid, HashSet<Guid>> GetResourceGenealogy() {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetResourceGenealogy")) {
        var resourceDao = pm.ResourceDao;
        var resourceAncestors = new Dictionary<Guid, HashSet<Guid>>();
        return pm.UseTransaction(() => {
          var resources = resourceDao.GetAll().ToList();
          resources.ForEach(r => resourceAncestors.Add(r.ResourceId, new HashSet<Guid>()));

          foreach (var r in resources) {
            var parentResource = r.ParentResource;
            while (parentResource != null) {
              resourceAncestors[r.ResourceId].Add(parentResource.ResourceId);
              parentResource = parentResource.ParentResource;
            }
          }
          return resourceAncestors;
        });
      }
    }

    public IDictionary<Guid, string> GetResourceNames() {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      var pm = PersistenceManager;
      using (new PerformanceLogger("GetResourceNames")) {
        var resourceDao = pm.ResourceDao;
        var resourceNames = new Dictionary<Guid, string>();
        return pm.UseTransaction(() => {
          resourceDao
            .GetAll().ToList()
            .ForEach(p => resourceNames.Add(p.ResourceId, p.Name));
          return resourceNames;
        });
      }
    }

    public void UpdateSlave(DT.Slave slaveDto) {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      if (slaveDto == null) return;
      AuthorizationManager.AuthorizeForResourceAdministration(slaveDto.Id);
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
      if (slaveGroupDto == null) return;
      AuthorizationManager.AuthorizeForResourceAdministration(slaveGroupDto.Id);
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
      if (slaveId == Guid.Empty) return;
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
      if (slaveGroupId == Guid.Empty) return;
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      AuthorizationManager.AuthorizeForResourceAdministration(slaveGroupId);
      var pm = PersistenceManager;
      using (new PerformanceLogger("DeleteSlaveGroup")) {
        var resourceDao = pm.ResourceDao;
        pm.UseTransaction(() => {
          var resourceIds = new HashSet<Guid> { slaveGroupId };
          resourceIds.Union(resourceDao.GetChildResourceIdsById(slaveGroupId));
          resourceDao.DeleteByIds(resourceIds);
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

    public Dictionary<Guid, HashSet<Guid>> GetUserGroupTree() {
      var userGroupTree = new Dictionary<Guid, HashSet<Guid>>();
      var userGroupMapping = UserManager.GetUserGroupMapping();

      foreach (var ugm in userGroupMapping) {
        if (ugm.Parent == null || ugm.Child == null) continue;

        if (!userGroupTree.ContainsKey(ugm.Parent)) {
          userGroupTree.Add(ugm.Parent, new HashSet<Guid>());
        }
        userGroupTree[ugm.Parent].Add(ugm.Child);
      }

      return userGroupTree;
    }

    public bool CheckAccessToAdminAreaGranted() {
      RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator, HiveRoles.Client);
      bool isAdministrator = RoleVerifier.IsInRole(HiveRoles.Administrator);
      var pm = PersistenceManager;
      using (new PerformanceLogger("CheckAccessToAdminAreaGranted")) {
        if (isAdministrator) {
          return true;
        } else {
          var projectDao = pm.ProjectDao;
          var currentUserId = UserManager.CurrentUserId;
          return projectDao.GetAdministrationGrantedProjectsForUser(currentUserId).Any();
        }
      }
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

      if (task.State == DA.TaskState.Transferring && taskStateEntity == DA.TaskState.Paused && task.Command == null) {
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

      if (task.Command == DA.Command.Pause && task.State == DA.TaskState.Paused
          || task.Command == DA.Command.Abort && task.State == DA.TaskState.Aborted
          || task.Command == DA.Command.Stop && task.State == DA.TaskState.Aborted) {
        task.Command = null;
      }
    }

    private void CheckProjectAvailability(IPersistenceManager pm, Guid projectId, DateTime date) {
      var projectDao = pm.ProjectDao;
      using (new PerformanceLogger("UpdateJob")) {
        var project = pm.UseTransaction(() => {
          return projectDao.GetById(projectId);
        });
        if (project != null) {
          if (project.StartDate > date) throw new ArgumentException("Cannot add job to specified project. The start date of the project is still in the future.");
          else if (project.EndDate != null && project.EndDate < date) throw new ArgumentException("Cannot add job to specified project. The end date of the project is already reached.");
        } else {
          throw new ArgumentException("Cannot add job to specified project. The project seems not to be available anymore.");
        }
      }
    }

    private void EvaluateJobs(IPersistenceManager pm, IEnumerable<DT.Job> jobs) {
      if (jobs == null || !jobs.Any()) return;

      var currentUserId = UserManager.CurrentUserId;
      var taskDao = pm.TaskDao;
      var jobPermissionDao = pm.JobPermissionDao;

      var statistics = taskDao.GetAll()
        .Where(x => jobs.Select(y => y.Id).Contains(x.JobId))
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
    }
    #endregion
  }
}
