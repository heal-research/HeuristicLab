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
using HeuristicLab.Hive.Server.DataAccess;
using HeuristicLab.DataAccess.Interfaces;
using HeuristicLab.Hive.Server.LINQDataAccess;
using ClientGroup=HeuristicLab.Hive.Contracts.BusinessObjects.ClientGroupDto;

namespace HeuristicLab.Hive.Server.Core {
  internal class ClientManager : IClientManager {
    private List<ClientGroup> clientGroups;

    public ClientManager() {
      clientGroups = new List<ClientGroup>();
    }

    #region IClientManager Members

    /// <summary>
    /// Returns all clients stored in the database
    /// </summary>
    /// <returns></returns>
    public ResponseList<ClientDto> GetAllClients() {
      ResponseList<ClientDto> response = new ResponseList<ClientDto>();

      response.List = new List<ClientDto>(DaoLocator.ClientDao.FindAll());
      response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_GET_ALL_CLIENTS;
      response.Success = true;

      return response;
    }

    /// <summary>
    /// returns all client groups stored in the database
    /// </summary>
    /// <returns></returns>
    public ResponseList<ClientGroup> GetAllClientGroups() {
      ResponseList<ClientGroup> response = new ResponseList<ClientGroup>();

      List<ClientGroup> allClientGroups =
        new List<ClientGroup>(DaoLocator.ClientGroupDao.FindAllWithSubGroupsAndClients());
      ClientGroup emptyClientGroup = new ClientGroup();
      IEnumerable<ClientDto> groupLessClients = DaoLocator.ClientDao.FindAllClientsWithoutGroup();
      if (groupLessClients != null) {
        foreach (ClientDto currClient in groupLessClients) {
          emptyClientGroup.Resources.Add(currClient);
        }
      }
      emptyClientGroup.Id = Guid.Empty;
      allClientGroups.Add(emptyClientGroup);

      response.List = allClientGroups;
      response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_GET_ALL_CLIENTGROUPS;
      response.Success = true;

      return response;
    }

    public ResponseList<UpTimeStatisticsDto> GetAllUpTimeStatistics() {
      ResponseList<UpTimeStatisticsDto> response = new ResponseList<UpTimeStatisticsDto>();
      response.Success = true;
      return response;
    }

    /// <summary>
    /// Add a client group into the database
    /// </summary>
    /// <param name="clientGroup"></param>
    /// <returns></returns>
    public ResponseObject<ClientGroup> AddClientGroup(ClientGroup clientGroup) {
      ResponseObject<ClientGroup> response = new ResponseObject<ClientGroup>();

      if (clientGroup.Id != Guid.Empty) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_ID_MUST_NOT_BE_SET;
      }
      else {
        clientGroup = DaoLocator.ClientGroupDao.Insert(clientGroup);
        //clientGroupAdapter.Update(clientGroup);
        response.Obj = clientGroup;
        response.Success = true;
        response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_CLIENTGROUP_ADDED;
      }

      return response;
    }

    /// <summary>
    ///  Add a resource to a group
    /// </summary>
    /// <param name="clientGroupId"></param>
    /// <param name="resource"></param>
    /// <returns></returns>
    public Response AddResourceToGroup(Guid clientGroupId, ResourceDto resource) {
      Response response = new Response();

      ClientGroup clientGroup = DaoLocator.ClientGroupDao.FindById(clientGroupId);
      if (clientGroup == null) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_CLIENTGROUP_DOESNT_EXIST;
        return response;
      }

      clientGroup.Resources.Add(resource);
      DaoLocator.ClientGroupDao.AddRessourceToClientGroup(resource.Id, clientGroup.Id);

      //If our resource is in fact a client => set the callendar from the resource as current one.
      
      ClientDto dbr = DaoLocator.ClientDao.FindById(resource.Id);
      if(dbr != null) {
        DaoLocator.ClientDao.SetServerSideCalendar(dbr, clientGroup.Id);  
      }

      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_RESOURCE_ADDED_TO_GROUP;

      return response;
    }

    /// <summary>
    /// Remove a resource from a group
    /// </summary>
    /// <param name="clientGroupId"></param>
    /// <param name="resourceId"></param>
    /// <returns></returns>
    public Response DeleteResourceFromGroup(Guid clientGroupId, Guid resourceId) {
      Response response = new Response();

      ClientGroup clientGroup = DaoLocator.ClientGroupDao.FindById(clientGroupId);
      if (clientGroup == null) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_CLIENTGROUP_DOESNT_EXIST;
        return response;
      }
     
      DaoLocator.ClientGroupDao.RemoveRessourceFromClientGroup(resourceId, clientGroup.Id);
      
      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_RESOURCE_REMOVED;
      return response;                
    }

    public ResponseObject<List<ClientGroup>> GetAllGroupsOfResource(Guid resourceId) {
      ResponseObject<List<ClientGroup>> response = new ResponseObject<List<ClientGroup>>();

      ClientDto client = DaoLocator.ClientDao.FindById(resourceId);
      if (client != null) {
        List<ClientGroup> groupsOfClient = new List<ClientGroup>(DaoLocator.ClientGroupDao.MemberOf(client));
        response.Obj = groupsOfClient;
        response.Success = true;
        response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_GET_GROUPS_OF_CLIENT;
      }
      else {
        response.Obj = new List<ClientGroup>();
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_RESOURCE_NOT_FOUND;
      }

      return response;
    }

    public Response DeleteClientGroup(Guid clientGroupId) {
      Response response = new Response();

      ClientGroup clientGroup = DaoLocator.ClientGroupDao.FindById(clientGroupId);
      if (clientGroup == null) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_CLIENTGROUP_DOESNT_EXIST;
        return response;
      }

      DaoLocator.ClientGroupDao.Delete(clientGroup);

      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_CLIENTGROUP_DELETED;
      return response;
    }

    public ResponseList<AppointmentDto> GetUptimeCalendarForResource(Guid guid) {
      ResponseList<AppointmentDto> response = new ResponseList<AppointmentDto>();
      response.List = new List<AppointmentDto>(DaoLocator.UptimeCalendarDao.GetUptimeCalendarForResource(guid));
      response.Success = true;
      return response;
    }

    public Response SetUptimeCalendarForResource(Guid guid, IEnumerable<AppointmentDto> appointments, bool isForced) {
      DaoLocator.UptimeCalendarDao.SetUptimeCalendarForResource(guid, appointments);
      DaoLocator.UptimeCalendarDao.NotifyClientsOfNewCalendar(guid, isForced);
      return new Response {Success = true};
    }
    #endregion
  }
}