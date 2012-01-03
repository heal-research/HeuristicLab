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
using System.Security;
using HeuristicLab.Services.Hive.DataAccess;
using DT = HeuristicLab.Services.Hive.DataTransfer;


namespace HeuristicLab.Services.Hive {
  public class AuthorizationManager : IAuthorizationManager {
    public void Authorize(Guid userId) {
      if (userId != ServiceLocator.Instance.UserManager.CurrentUserId)
        throw new SecurityException("Current user is not authorized to access object");
    }

    public void AuthorizeForTask(Guid taskId, DT.Permission requiredPermission) {
      if (ServiceLocator.Instance.AuthenticationManager.IsInRole(HiveRoles.Slave)) return; // slave-users can access all tasks

      Permission permission = ServiceLocator.Instance.HiveDao.GetPermissionForTask(taskId, ServiceLocator.Instance.UserManager.CurrentUserId);
      if (permission == Permission.NotAllowed || (permission != DT.Convert.ToEntity(requiredPermission) && DT.Convert.ToEntity(requiredPermission) == Permission.Full))
        throw new SecurityException("Current user is not authorized to access task");
    }

    public void AuthorizeForJob(Guid jobId, DT.Permission requiredPermission) {
      Permission permission = ServiceLocator.Instance.HiveDao.GetPermissionForJob(jobId, ServiceLocator.Instance.UserManager.CurrentUserId);
      if (permission == Permission.NotAllowed || (permission != DT.Convert.ToEntity(requiredPermission) && DT.Convert.ToEntity(requiredPermission) == Permission.Full))
        throw new SecurityException("Current user is not authorized to access task");
    }
  }
}
