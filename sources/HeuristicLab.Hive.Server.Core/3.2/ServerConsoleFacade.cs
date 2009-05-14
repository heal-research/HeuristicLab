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
using HeuristicLab.Hive.Server.Core.InternalInterfaces;

namespace HeuristicLab.Hive.Server.Core {
  public class ServerConsoleFacade: IServerConsoleFacade {
    private IClientManager clientManager = 
      ServiceLocator.GetClientManager();

    private IJobManager jobManager = 
      ServiceLocator.GetJobManager();

    private IHivePermissionManager secMan = ServiceLocator.GetHivePermissionManager();

    public Guid sessionID = Guid.Empty;

    public Response Login(string username, string password) {
      Response resp = new Response();
     
      /*
      sessionID = secMan.Login(username, password);
      if (sessionID == Guid.Empty) {
        resp.Success = false;
        resp.StatusMessage = ApplicationConstants.RESPONSE_SERVERCONSOLE_LOGIN_FAILED;
      } else {
        resp.Success = true;
        resp.StatusMessage =
          ApplicationConstants.RESPONSE_SERVERCONSOLE_LOGIN_SUCCESS;
      }
      */ 
      sessionID = Guid.Empty;
      resp.Success = true;
      resp.StatusMessage = ApplicationConstants.RESPONSE_SERVERCONSOLE_LOGIN_SUCCESS;
      return resp;
    }


    public ResponseList<ClientInfo> GetAllClients() {
      if (HasPermission(PermissiveSecurityAction.List_AllClients))
        return clientManager.GetAllClients();
      else
        throw new PermissionException();
    }

    public ResponseList<ClientGroup> GetAllClientGroups() {
      if (HasPermission(PermissiveSecurityAction.List_AllClientGroups))
        return clientManager.GetAllClientGroups();
      else
        throw new PermissionException();
    }

    public ResponseList<UpTimeStatistics> GetAllUpTimeStatistics() {
      if (HasPermission(PermissiveSecurityAction.Show_Statistics))
        return clientManager.GetAllUpTimeStatistics();
      else
        throw new PermissionException();
    }

    public Response AddClientGroup(ClientGroup clientGroup) {
      if (HasPermission(PermissiveSecurityAction.Add_ClientGroup))
        return clientManager.AddClientGroup(clientGroup);
      else
        throw new PermissionException();
    }

    public Response AddResourceToGroup(Guid clientGroupId, Resource resource) {
      if (HasPermission(PermissiveSecurityAction.Add_Resource))
        return clientManager.AddResourceToGroup(clientGroupId, resource);
      else
        throw new PermissionException();
    }

    public Response DeleteResourceFromGroup(Guid clientGroupId, Guid resourceId) {
      if (HasPermission(PermissiveSecurityAction.Delete_Resource))
        return clientManager.DeleteResourceFromGroup(clientGroupId, resourceId);
      else
        throw new PermissionException();
    }


    public ResponseList<HeuristicLab.Hive.Contracts.BusinessObjects.Job> GetAllJobs() {
      if (HasPermission(PermissiveSecurityAction.Get_AllJobs))
        return jobManager.GetAllJobs();
      else
        throw new PermissionException();
    }

    public ResponseObject<Job> AddNewJob(Job job) {
      if (HasPermission(PermissiveSecurityAction.Add_Job))
        return jobManager.AddNewJob(job);
      else
        throw new PermissionException();
    }

    public ResponseObject<JobResult> GetLastJobResultOf(Guid jobId, bool requested) {
      if (HasPermission(PermissiveSecurityAction.Get_LastJobResult))
        return jobManager.GetLastJobResultOf(jobId, requested);
      else
        throw new PermissionException();
    }

    public ResponseList<JobResult> GetAllJobResults(Guid jobId) {
      if (HasPermission(PermissiveSecurityAction.Get_AllJobResults))
        return jobManager.GetAllJobResults(jobId);
      else
        throw new PermissionException();
    }

    public Response RemoveJob(Guid jobId) {
      if (HasPermission(PermissiveSecurityAction.Remove_Job))
        return jobManager.RemoveJob(jobId);
      else
        throw new PermissionException();
    }

    public Response RequestSnapshot(Guid jobId) {
      if (HasPermission(PermissiveSecurityAction.Request_Snapshot))
        return jobManager.RequestSnapshot(jobId);
      else
        throw new PermissionException();
    }

    public Response AbortJob(Guid jobId) {
      if (HasPermission(PermissiveSecurityAction.Abort_Job))
        return jobManager.AbortJob(jobId);
      else
        throw new PermissionException();
    }

    public ResponseObject<List<ClientGroup>> GetAllGroupsOfResource(Guid resourceId) {
      if (HasPermission(PermissiveSecurityAction.Get_AllGroupsOfResource))
        return clientManager.GetAllGroupsOfResource(resourceId);
      else
        throw new PermissionException();      
    }

  /*
    private bool HasPermission(Guid action) {
      return (sessionID == Guid.Empty) ? false : secMan.CheckPermission(sessionID, action, Guid.Empty);
    }

    private bool HasPermission(Guid action, Guid entityId) {
      return (sessionID == Guid.Empty) ? false : secMan.CheckPermission(sessionID, action, entityId);
    }
   */

    [Obsolete("Only for testing!")]
    private bool HasPermission(Guid g) { return true; }
    [Obsolete("Only for testing!")]
    private bool HasPermission(Guid g, Guid f) { return true; }

    public class PermissionException : Exception {
      public PermissionException()
        : base("Current user has insufficent rights for this action!") {
      }

      public PermissionException(string msg)
        : base(msg) {
      }


    }


  }
}
