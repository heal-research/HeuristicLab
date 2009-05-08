#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Hive.Contracts.Interfaces;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Contracts;
using HeuristicLab.Security.Contracts.Interfaces;

namespace HeuristicLab.Hive.Server.Core {
  public class ServerConsoleFacade: IServerConsoleFacade {
    private IClientManager clientManager = 
      ServiceLocator.GetClientManager();

    private IJobManager jobManager = 
      ServiceLocator.GetJobManager();

    //private IPermissionManager permManager = ServiceLocator.GetPermissionManager();

    #region IServerConsoleFacade Members

    public Guid sessionID = Guid.Empty;

    public Response Login(string username, string password) {
      Response resp = new Response();
      /*
      sessionID = permManager.Authenticate(username, password);
      if (sessionID == Guid.Empty)
        resp.Success = false;
      else {
       
        resp.Success = true;
        resp.StatusMessage =
          ApplicationConstants.RESPONSE_SERVERCONSOLE_LOGIN_SUCCESS;
      }
       */
      resp.Success = true;
      resp.StatusMessage =
        ApplicationConstants.RESPONSE_SERVERCONSOLE_LOGIN_SUCCESS;
      return resp;
    }

    #endregion

    #region IClientManager Members

    public ResponseList<ClientInfo> GetAllClients() {
      if (hasPermission(PermissiveSecurityAction.List_AllClients))
        return clientManager.GetAllClients();
      else
        throw new PermissionException();
    }

    public ResponseList<ClientGroup> GetAllClientGroups() {
      if (hasPermission(PermissiveSecurityAction.List_AllClientGroups))
        return clientManager.GetAllClientGroups();
      else
        throw new PermissionException();
    }

    public ResponseList<UpTimeStatistics> GetAllUpTimeStatistics() {
      if (hasPermission(PermissiveSecurityAction.Show_Statistics))
        return clientManager.GetAllUpTimeStatistics();
      else
        throw new PermissionException();
    }

    public Response AddClientGroup(ClientGroup clientGroup) {
      if (hasPermission(PermissiveSecurityAction.Add_ClientGroup))
        return clientManager.AddClientGroup(clientGroup);
      else
        throw new PermissionException();
    }

    public Response AddResourceToGroup(Guid clientGroupId, Resource resource) {
      if (hasPermission(PermissiveSecurityAction.Add_Resource))
        return clientManager.AddResourceToGroup(clientGroupId, resource);
      else
        throw new PermissionException();
    }

    public Response DeleteResourceFromGroup(Guid clientGroupId, Guid resourceId) {
      if (hasPermission(PermissiveSecurityAction.Delete_Resource))
        return clientManager.DeleteResourceFromGroup(clientGroupId, resourceId);
      else
        throw new PermissionException();
    }

    #endregion

    #region IJobManager Members

    public ResponseList<HeuristicLab.Hive.Contracts.BusinessObjects.Job> GetAllJobs() {
      if (hasPermission(PermissiveSecurityAction.Get_AllJobs))
        return jobManager.GetAllJobs();
      else
        throw new PermissionException();
    }

    public ResponseObject<Job> AddNewJob(Job job) {
      if (hasPermission(PermissiveSecurityAction.Add_Job))
        return jobManager.AddNewJob(job);
      else
        throw new PermissionException();
    }

    public ResponseObject<JobResult> GetLastJobResultOf(Guid jobId, bool requested) {
      if (hasPermission(PermissiveSecurityAction.Get_LastJobResult))
        return jobManager.GetLastJobResultOf(jobId, requested);
      else
        throw new PermissionException();
    }

    public ResponseObject<List<JobResult>> GetAllJobResults(Guid jobId) {
      if (hasPermission(PermissiveSecurityAction.Get_AllJobResults))
        return jobManager.GetAllJobResults(jobId);
      else
        throw new PermissionException();
    }

    public Response RemoveJob(Guid jobId) {
      if (hasPermission(PermissiveSecurityAction.Remove_Job))
        return jobManager.RemoveJob(jobId);
      else
        throw new PermissionException();
    }

    public Response RequestSnapshot(Guid jobId) {
      if (hasPermission(PermissiveSecurityAction.Request_Snapshot))
        return jobManager.RequestSnapshot(jobId);
      else
        throw new PermissionException();
    }

    public Response AbortJob(Guid jobId) {
      if (hasPermission(PermissiveSecurityAction.Abort_Job))
        return jobManager.AbortJob(jobId);
      else
        throw new PermissionException();
    }

    private bool hasPermission(Guid action) {
      return true;
      /*
      if (sessionID == Guid.Empty)
        throw new Exception("sessionID is not set! Please check if user is successfully logged on!");
      return permManager.CheckPermission(sessionID, action, Guid.Empty);
       */
    }

    public class PermissionException : Exception {
      public PermissionException()
        : base("Current user has insufficent rights for this action!") {
      }

      public PermissionException(string msg)
        : base(msg) {
      }


    }

    public ResponseObject<List<ClientGroup>> GetAllGroupsOfResource(Guid resourceId) {
      return clientManager.GetAllGroupsOfResource(resourceId);
    }

    #endregion
  }
}
