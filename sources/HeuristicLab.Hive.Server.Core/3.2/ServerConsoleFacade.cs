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
using System.ServiceModel;


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
     
      sessionID = secMan.Login(username, password);
      if (sessionID == Guid.Empty) {
        resp.Success = false;
        resp.StatusMessage = ApplicationConstants.RESPONSE_SERVERCONSOLE_LOGIN_FAILED;
      } else {
        resp.Success = true;
        resp.StatusMessage =
          ApplicationConstants.RESPONSE_SERVERCONSOLE_LOGIN_SUCCESS;
      }
      return resp;
    }


    public ResponseList<ClientInfo> GetAllClients() {
      secMan.Authorize("AccessClients", sessionID, Guid.Empty);
      return clientManager.GetAllClients();
    }

    public ResponseList<ClientGroup> GetAllClientGroups() {
      secMan.Authorize("AccessClientGroup", sessionID, Guid.Empty);
      return clientManager.GetAllClientGroups();
    }

    public ResponseList<UpTimeStatistics> GetAllUpTimeStatistics() {
      secMan.Authorize("AccessStatistics", sessionID, Guid.Empty);
      return clientManager.GetAllUpTimeStatistics();
    }

    public ResponseObject<ClientGroup> AddClientGroup(ClientGroup clientGroup) {
      secMan.Authorize("AddClientGroup", sessionID, Guid.Empty);
      return clientManager.AddClientGroup(clientGroup);
    }

    public Response AddResourceToGroup(Guid clientGroupId, Resource resource) {
      secMan.Authorize("AddResource", sessionID, Guid.Empty);                
      return clientManager.AddResourceToGroup(clientGroupId, resource);
    }

    public Response DeleteResourceFromGroup(Guid clientGroupId, Guid resourceId) {
        return clientManager.DeleteResourceFromGroup(clientGroupId, resourceId);
    }

    public ResponseList<Job> GetAllJobs() {
      secMan.Authorize("AccessJobs", sessionID, Guid.Empty);
      return jobManager.GetAllJobs();
    }

    public ResponseObject<Job> GetJobById(Guid jobId) {
      secMan.Authorize("AccessJobs", sessionID, jobId);
      return jobManager.GetJobById(jobId);
    }

    public ResponseObject<Job> AddNewJob(Job job) {
      secMan.Authorize("AddJob", sessionID, job.Id);
      return jobManager.AddNewJob(job);
    }

    public ResponseObject<JobResult> GetLastJobResultOf(Guid jobId, bool requested) {
      secMan.Authorize("AccessJobResults", sessionID, jobId);
      return jobManager.GetLastJobResultOf(jobId, requested);
    }

    public ResponseList<JobResult> GetAllJobResults(Guid jobId) {
      secMan.Authorize("AccessJobResults", sessionID, jobId);  
      return jobManager.GetAllJobResults(jobId);
    }

    public Response RemoveJob(Guid jobId) {
      secMan.Authorize("RemoveJob", sessionID, jobId);
      return jobManager.RemoveJob(jobId);
    }

    public Response RequestSnapshot(Guid jobId) {
      secMan.Authorize("AccessJobResults", sessionID, jobId);  
      return jobManager.RequestSnapshot(jobId);
    }

    public Response AbortJob(Guid jobId) {
      secMan.Authorize("AbortJob", sessionID, Guid.Empty);
      return jobManager.AbortJob(jobId);
    }

    public ResponseObject<List<ClientGroup>> GetAllGroupsOfResource(Guid resourceId) {
      secMan.Authorize("AccessUserGroup", sessionID, Guid.Empty);
      return clientManager.GetAllGroupsOfResource(resourceId);
    }

    public Response DeleteClientGroup(Guid clientGroupId) {
      secMan.Authorize("DeleteClientGroup", sessionID, Guid.Empty);
      return clientManager.DeleteClientGroup(clientGroupId);
    }

    public ResponseList<Project> GetAllProjects() {
      secMan.Authorize("AccessProjects", sessionID, Guid.Empty);
      return jobManager.GetAllProjects();
    }

    public Response CreateProject(Project project) {
      secMan.Authorize("CreateProjects", sessionID, Guid.Empty);
      return jobManager.CreateProject(project);
    }

    public Response ChangeProject(Project project) {
      secMan.Authorize("ChangeProjects", sessionID, Guid.Empty);
      return jobManager.ChangeProject(project);
    }

    public Response DeleteProject(Guid projectId) {
      secMan.Authorize("DeleteProjects", sessionID, projectId);
      return jobManager.DeleteProject(projectId);
    }

    public ResponseList<Job> GetJobsByProject(Guid projectId) {
      secMan.Authorize("AccessJobs", sessionID, Guid.Empty);
      return jobManager.GetJobsByProject(projectId);
    }

  }
}
