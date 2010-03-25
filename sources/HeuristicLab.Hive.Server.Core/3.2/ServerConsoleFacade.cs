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

    public ServerConsoleFacade() {      
    }

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


    public ResponseList<ClientDto> GetAllClients() {
      secMan.Authorize("AccessClients", sessionID, Guid.Empty);
      return clientManager.GetAllClients();
    }

    public ResponseList<ClientGroupDto> GetAllClientGroups() {
      //secMan.Authorize("AccessClientGroup", sessionID, Guid.Empty);
      return clientManager.GetAllClientGroups();
    }

    public ResponseList<UpTimeStatisticsDto> GetAllUpTimeStatistics() {
      secMan.Authorize("AccessStatistics", sessionID, Guid.Empty);
      return clientManager.GetAllUpTimeStatistics();
    }

    public ResponseObject<ClientGroupDto> AddClientGroup(ClientGroupDto clientGroup) {
      secMan.Authorize("AddClientGroup", sessionID, Guid.Empty);
      return clientManager.AddClientGroup(clientGroup);
    }

    public Response AddResourceToGroup(Guid clientGroupId, ResourceDto resource) {
      secMan.Authorize("AddResource", sessionID, Guid.Empty);                
      return clientManager.AddResourceToGroup(clientGroupId, resource);
    }

    public Response DeleteResourceFromGroup(Guid clientGroupId, Guid resourceId) {
        return clientManager.DeleteResourceFromGroup(clientGroupId, resourceId);
    }

    public ResponseList<JobDto> GetAllJobs() {
      secMan.Authorize("AccessJobs", sessionID, Guid.Empty);
      return jobManager.GetAllJobs();
    }

    public ResponseObject<JobDto> GetJobById(Guid jobId) {
      secMan.Authorize("AccessJobs", sessionID, jobId);
      return jobManager.GetJobById(jobId);
    }

    public ResponseObject<JobDto> AddNewJob(SerializedJob job) {
      secMan.Authorize("AddJob", sessionID, job.JobInfo.Id);
      return jobManager.AddNewJob(job);
    }

    public ResponseObject<JobDto> GetLastJobResultOf(Guid jobId) {
      secMan.Authorize("AccessJobResults", sessionID, jobId);
      return jobManager.GetLastJobResultOf(jobId);
    }

    public ResponseObject<SerializedJob> GetLastSerializedJobResultOf(Guid jobId, bool requested) {
      secMan.Authorize("AccessJobResults", sessionID, jobId);
      return jobManager.GetLastSerializedJobResultOf(jobId, requested);
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

    public ResponseObject<List<ClientGroupDto>> GetAllGroupsOfResource(Guid resourceId) {
      secMan.Authorize("AccessUserGroup", sessionID, Guid.Empty);
      return clientManager.GetAllGroupsOfResource(resourceId);
    }

    public Response DeleteClientGroup(Guid clientGroupId) {
      secMan.Authorize("DeleteClientGroup", sessionID, Guid.Empty);
      return clientManager.DeleteClientGroup(clientGroupId);
    }

    public ResponseList<ProjectDto> GetAllProjects() {
      secMan.Authorize("AccessProjects", sessionID, Guid.Empty);
      return jobManager.GetAllProjects();
    }

    public Response CreateProject(ProjectDto project) {
      secMan.Authorize("CreateProjects", sessionID, Guid.Empty);
      return jobManager.CreateProject(project);
    }

    public Response ChangeProject(ProjectDto project) {
      secMan.Authorize("ChangeProjects", sessionID, Guid.Empty);
      return jobManager.ChangeProject(project);
    }

    public Response DeleteProject(Guid projectId) {
      secMan.Authorize("DeleteProjects", sessionID, projectId);
      return jobManager.DeleteProject(projectId);
    }

    public ResponseList<JobDto> GetJobsByProject(Guid projectId) {
      secMan.Authorize("AccessJobs", sessionID, Guid.Empty);
      return jobManager.GetJobsByProject(projectId);
    }

    public ResponseList<AppointmentDto> GetUptimeCalendarForResource(Guid guid) {
      return clientManager.GetUptimeCalendarForResource(guid);
    }

    public Response SetUptimeCalendarForResource(Guid guid, IEnumerable<AppointmentDto> appointments, bool isForced) {
      return clientManager.SetUptimeCalendarForResource(guid, appointments, isForced);
    }

    public ResponseObject<JobDto> AddJobWithGroupStrings(SerializedJob job, IEnumerable<string> resources) {
      return jobManager.AddJobWithGroupStrings(job, resources);
    }
  }
}
