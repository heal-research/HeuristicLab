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
using System.Security;
using HeuristicLab.Services.Access;
using HeuristicLab.Services.Hive.DataAccess;
using HeuristicLab.Services.Hive.DataAccess.Interfaces;
using DA = HeuristicLab.Services.Hive.DataAccess;
using DT = HeuristicLab.Services.Hive.DataTransfer;


namespace HeuristicLab.Services.Hive {
  public class AuthorizationManager : IAuthorizationManager {

    private const string NOT_AUTHORIZED = "Current user is not authorized to access the requested resource";
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
        throw new SecurityException(NOT_AUTHORIZED);
    }

    public void AuthorizeForTask(Guid taskId, DT.Permission requiredPermission) {
      if (ServiceLocator.Instance.RoleVerifier.IsInRole(HiveRoles.Slave)) return; // slave-users can access all tasks
      var pm = PersistenceManager;
      var taskDao = pm.TaskDao;
      pm.UseTransaction(() => {
        var task = taskDao.GetById(taskId);
        if (task == null) throw new SecurityException(NOT_AUTHORIZED);
        AuthorizeJob(pm, task.JobId, requiredPermission);
      });
    }

    public void AuthorizeForJob(Guid jobId, DT.Permission requiredPermission) {
      var pm = PersistenceManager;
      pm.UseTransaction(() => {
        AuthorizeJob(pm, jobId, requiredPermission);
      });
    }

    public void AuthorizeForResourceAdministration(Guid resourceId) {
      var pm = PersistenceManager;
      var resourceDao = pm.ResourceDao;
      pm.UseTransaction(() => {
        var resource = resourceDao.GetById(resourceId);
        if (resource == null) throw new SecurityException(NOT_AUTHORIZED);
        if (resource.OwnerUserId != UserManager.CurrentUserId
            && !RoleVerifier.IsInRole(HiveRoles.Administrator)) {
          throw new SecurityException(NOT_AUTHORIZED);
        }
      });
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
      var requiredPermissionEntity = requiredPermission.ToEntity();
      DA.Permission permission = GetPermissionForJob(pm, jobId, UserManager.CurrentUserId);
      if (permission == Permission.NotAllowed
          || ((permission != requiredPermissionEntity) && requiredPermissionEntity == Permission.Full)) {
        throw new SecurityException(NOT_AUTHORIZED);
      }
    }
  }
}
