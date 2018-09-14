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
using System.Security;
using HeuristicLab.Services.Access;
using HeuristicLab.Services.Hive.DataAccess;
using HeuristicLab.Services.Hive.DataAccess.Interfaces;
using DA = HeuristicLab.Services.Hive.DataAccess;
using DT = HeuristicLab.Services.Hive.DataTransfer;
using System.Collections.Generic;
using System.Linq;

namespace HeuristicLab.Services.Hive {
  public class AuthorizationManager : IAuthorizationManager {

    private const string NOT_AUTHORIZED_USERRESOURCE = "Current user is not authorized to access the requested resource";
    private const string NOT_AUTHORIZED_USERPROJECT = "Current user is not authorized to access the requested project";
    private const string NOT_AUTHORIZED_USERJOB = "Current user is not authorized to access the requested job";
    private const string NOT_AUTHORIZED_PROJECTRESOURCE = "Selected project is not authorized to access the requested resource";
    private const string USER_NOT_IDENTIFIED = "User could not be identified";
    private const string TASK_NOT_EXISTENT = "Queried task could not be found";
    private const string PROJECT_NOT_EXISTENT = "Queried project could not be found";

    private IPersistenceManager PersistenceManager {
      get { return ServiceLocator.Instance.PersistenceManager; }
    }

    private IUserManager UserManager {
      get { return ServiceLocator.Instance.UserManager; }
    }

    private IRoleVerifier RoleVerifier {
      get { return ServiceLocator.Instance.RoleVerifier; }
    }

    public void Authorize(Guid userId) {
      if (userId != ServiceLocator.Instance.UserManager.CurrentUserId)
        throw new SecurityException(NOT_AUTHORIZED_USERRESOURCE);
    }

    public void AuthorizeForTask(Guid taskId, DT.Permission requiredPermission) {
      if (ServiceLocator.Instance.RoleVerifier.IsInRole(HiveRoles.Slave)) return; // slave-users can access all tasks
      var pm = PersistenceManager;
      var taskDao = pm.TaskDao;
      pm.UseTransaction(() => {
        var task = taskDao.GetById(taskId);
        if (task == null) throw new SecurityException(TASK_NOT_EXISTENT);
        AuthorizeJob(pm, task.JobId, requiredPermission);
      });
    }

    public void AuthorizeForJob(Guid jobId, DT.Permission requiredPermission) {
      var pm = PersistenceManager;
      pm.UseTransaction(() => {
        AuthorizeJob(pm, jobId, requiredPermission);
      });
    }

    // authorize if user is admin or resource owner
    public void AuthorizeForResourceAdministration(Guid resourceId) {
      var currentUserId = UserManager.CurrentUserId;
      var pm = PersistenceManager;
      var resourceDao = pm.ResourceDao;
      pm.UseTransaction(() => {
        var resource = resourceDao.GetById(resourceId);
        if (resource == null) throw new SecurityException(NOT_AUTHORIZED_USERRESOURCE);

        if (resource.OwnerUserId != currentUserId
            && !RoleVerifier.IsInRole(HiveRoles.Administrator)) {
          throw new SecurityException(NOT_AUTHORIZED_USERRESOURCE);
        }
      });
    }

    // authorize if user is admin, project owner or owner of a parent project
    public void AuthorizeForProjectAdministration(Guid projectId, bool parentalOwnership) {
      if (projectId == null || projectId == Guid.Empty) return;
      var currentUserId = UserManager.CurrentUserId;
      var pm = PersistenceManager;
      var projectDao = pm.ProjectDao;
      pm.UseTransaction(() => {
        var project = projectDao.GetById(projectId);
        if (project == null) throw new ArgumentException(PROJECT_NOT_EXISTENT);
        if(!RoleVerifier.IsInRole(HiveRoles.Administrator)
          && !project.ParentProjectId.HasValue) {
          throw new SecurityException(NOT_AUTHORIZED_USERPROJECT);
        }

        List<Project> projectBranch = null;
        if(parentalOwnership) projectBranch = projectDao.GetParentProjectsById(projectId).ToList();
        else projectBranch = projectDao.GetCurrentAndParentProjectsById(projectId).ToList();

        if(!RoleVerifier.IsInRole(HiveRoles.Administrator)
            && !projectBranch.Select(x => x.OwnerUserId).Contains(currentUserId)) {
          throw new SecurityException(NOT_AUTHORIZED_USERPROJECT);
        }
      });
    }

    // authorize if user is admin, or owner of a project or parent project, for which the resources are assigned to
    public void AuthorizeForProjectResourceAdministration(Guid projectId, IEnumerable<Guid> resourceIds) {
      if (projectId == null || projectId == Guid.Empty) return;
      var currentUserId = UserManager.CurrentUserId;
      var pm = PersistenceManager;
      var projectDao = pm.ProjectDao;
      var resourceDao = pm.ResourceDao;
      var assignedProjectResourceDao = pm.AssignedProjectResourceDao;
      pm.UseTransaction(() => {
        // check if project exists (not necessary)
        var project = projectDao.GetById(projectId);
        if (project == null) throw new SecurityException(NOT_AUTHORIZED_USERRESOURCE);

        // check if resourceIds exist
        if (resourceIds != null && resourceIds.Any() && !resourceDao.CheckExistence(resourceIds))
          throw new SecurityException(NOT_AUTHORIZED_USERRESOURCE);

        // check if user is admin
        if (RoleVerifier.IsInRole(HiveRoles.Administrator)) return;

        // check if user is owner of the project or a parent project
        var projectBranch = projectDao.GetCurrentAndParentProjectsById(projectId).ToList();
        if (!projectBranch.Select(x => x.OwnerUserId).Contains(currentUserId)
            && !RoleVerifier.IsInRole(HiveRoles.Administrator)) {
          throw new SecurityException(NOT_AUTHORIZED_USERPROJECT);
        }

        // check if the all argument resourceIds are among the assigned resources of the owned projects
        var grantedResourceIds = assignedProjectResourceDao.GetAllGrantedResourceIdsOfOwnedParentProjects(projectId, currentUserId).ToList();
        if(resourceIds.Except(grantedResourceIds).Any()) {
          throw new SecurityException(NOT_AUTHORIZED_USERRESOURCE);
        }
      });
    }

    // Check if a project is authorized to use a list of resources
    public void AuthorizeProjectForResourcesUse(Guid projectId, IEnumerable<Guid> resourceIds) {
      if (projectId == null || projectId == Guid.Empty || resourceIds == null || !resourceIds.Any()) return;
      var pm = PersistenceManager;
      var assignedProjectResourceDao = pm.AssignedProjectResourceDao;
      if (!assignedProjectResourceDao.CheckProjectGrantedForResources(projectId, resourceIds))
        throw new SecurityException(NOT_AUTHORIZED_PROJECTRESOURCE);
    }

    // Check if current user is authorized to use an explicit project (e.g. in order to add a job)
    // note: administrators and project owner are NOT automatically granted
    public void AuthorizeUserForProjectUse(Guid userId, Guid projectId) {
      if(userId == null || userId == Guid.Empty) {
        throw new SecurityException(USER_NOT_IDENTIFIED);
      }
      if(projectId == null) return;

      var pm = PersistenceManager;
      // collect current and group membership Ids
      var userAndGroupIds = new List<Guid>() { userId };
      userAndGroupIds.AddRange(UserManager.GetUserGroupIdsOfUser(userId));
      // perform the actual check
      var projectPermissionDao = pm.ProjectPermissionDao;
      if (!projectPermissionDao.CheckUserGrantedForProject(projectId, userAndGroupIds)) {
        throw new SecurityException(NOT_AUTHORIZED_USERPROJECT);
      }
    }

    private DA.Permission GetPermissionForJob(IPersistenceManager pm, Guid jobId, Guid userId) {
      var jobDao = pm.JobDao;
      var jobPermissionDao = pm.JobPermissionDao;
      var job = jobDao.GetById(jobId);
      if (job == null) return DA.Permission.NotAllowed;
      if (job.OwnerUserId == userId) return DA.Permission.Full;
      var jobPermission = jobPermissionDao.GetByJobAndUserId(jobId, userId);
      if (jobPermission == null) return DA.Permission.NotAllowed;
      return jobPermission.Permission;
    }

    private void AuthorizeJob(IPersistenceManager pm, Guid jobId, DT.Permission requiredPermission) {
      var currentUserId = UserManager.CurrentUserId;
      var requiredPermissionEntity = requiredPermission.ToEntity();
      DA.Permission permission = GetPermissionForJob(pm, jobId, currentUserId);
      if (permission == Permission.NotAllowed
          || ((permission != requiredPermissionEntity) && requiredPermissionEntity == Permission.Full)) {
        throw new SecurityException(NOT_AUTHORIZED_USERJOB);
      }
    }
  }
}
