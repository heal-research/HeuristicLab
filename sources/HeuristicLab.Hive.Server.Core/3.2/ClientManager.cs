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

namespace HeuristicLab.Hive.Server.Core {
  class ClientManager: IClientManager {

    ISessionFactory factory;
    List<ClientGroup> clientGroups;

    public ClientManager() {
      factory = ServiceLocator.GetSessionFactory();
      
      clientGroups = new List<ClientGroup>();
    }

    #region IClientManager Members

    /// <summary>
    /// Returns all clients stored in the database
    /// </summary>
    /// <returns></returns>
    public ResponseList<ClientInfo> GetAllClients() {
      ISession session = factory.GetSessionForCurrentThread();

      try {
        IClientAdapter clientAdapter =
          session.GetDataAdapter<ClientInfo, IClientAdapter>();

        ResponseList<ClientInfo> response = new ResponseList<ClientInfo>();

        response.List = new List<ClientInfo>(clientAdapter.GetAll());
        response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_GET_ALL_CLIENTS;
        response.Success = true;

        return response;
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    /// <summary>
    /// returns all client groups stored in the database
    /// </summary>
    /// <returns></returns>
    public ResponseList<ClientGroup> GetAllClientGroups() {
      ISession session = factory.GetSessionForCurrentThread();

      try {
        IClientGroupAdapter clientGroupAdapter =
          session.GetDataAdapter<ClientGroup, IClientGroupAdapter>();
        IClientAdapter clientAdapter =
          session.GetDataAdapter<ClientInfo, IClientAdapter>();
        ResponseList<ClientGroup> response = new ResponseList<ClientGroup>();

        List<ClientGroup> allClientGroups = new List<ClientGroup>(clientGroupAdapter.GetAll());
        ClientGroup emptyClientGroup = new ClientGroup();
        ICollection<ClientInfo> groupLessClients = clientAdapter.GetGrouplessClients();
        if (groupLessClients != null) {
          foreach (ClientInfo currClient in groupLessClients) {
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
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    public ResponseList<UpTimeStatistics> GetAllUpTimeStatistics() {
      ResponseList<UpTimeStatistics> response = new ResponseList<UpTimeStatistics>();
      response.Success = true;
      return response;
    }

    /// <summary>
    /// Add a client group into the database
    /// </summary>
    /// <param name="clientGroup"></param>
    /// <returns></returns>
    public ResponseObject<ClientGroup> AddClientGroup(ClientGroup clientGroup) {
      ISession session = factory.GetSessionForCurrentThread();

      try {
        IClientGroupAdapter clientGroupAdapter =
          session.GetDataAdapter<ClientGroup, IClientGroupAdapter>();

        ResponseObject<ClientGroup> response = new ResponseObject<ClientGroup>();

        if (clientGroup.Id != Guid.Empty) {
          response.Success = false;
          response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_ID_MUST_NOT_BE_SET;
        } else {
          clientGroupAdapter.Update(clientGroup);
          response.Obj = clientGroup;
          response.Success = true;
          response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_CLIENTGROUP_ADDED;
        }

        return response;
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    /// <summary>
    ///  Add a resource to a group
    /// </summary>
    /// <param name="clientGroupId"></param>
    /// <param name="resource"></param>
    /// <returns></returns>
    public Response AddResourceToGroup(Guid clientGroupId, Resource resource) {
      ISession session = factory.GetSessionForCurrentThread();

      try {
        IClientGroupAdapter clientGroupAdapter =
          session.GetDataAdapter<ClientGroup, IClientGroupAdapter>();

        Response response = new Response();

        ClientGroup clientGroup = clientGroupAdapter.GetById(clientGroupId);
        if (clientGroup == null) {
          response.Success = false;
          response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_CLIENTGROUP_DOESNT_EXIST;
          return response;
        }
        clientGroup.Resources.Add(resource);
        clientGroupAdapter.Update(clientGroup);

        response.Success = true;
        response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_RESOURCE_ADDED_TO_GROUP;

        return response;
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    /// <summary>
    /// Remove a resource from a group
    /// </summary>
    /// <param name="clientGroupId"></param>
    /// <param name="resourceId"></param>
    /// <returns></returns>
    public Response DeleteResourceFromGroup(Guid clientGroupId, Guid resourceId) {
      ISession session = factory.GetSessionForCurrentThread();

      try {
        IClientGroupAdapter clientGroupAdapter =
          session.GetDataAdapter<ClientGroup, IClientGroupAdapter>();

        Response response = new Response();

        ClientGroup clientGroup = clientGroupAdapter.GetById(clientGroupId);
        if (clientGroup == null) {
          response.Success = false;
          response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_CLIENTGROUP_DOESNT_EXIST;
          return response;
        }
        foreach (Resource resource in clientGroup.Resources) {
          if (resource.Id == resourceId) {
            clientGroup.Resources.Remove(resource);
            response.Success = true;
            response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_RESOURCE_REMOVED;
            return response;
          }
        }
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_RESOURCE_NOT_FOUND;

        return response;
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    public ResponseObject<List<ClientGroup>> GetAllGroupsOfResource(Guid resourceId) {
      ISession session = factory.GetSessionForCurrentThread();

      try {
        IClientGroupAdapter clientGroupAdapter =
          session.GetDataAdapter<ClientGroup, IClientGroupAdapter>();
        IClientAdapter clientAdapter =
          session.GetDataAdapter<ClientInfo, IClientAdapter>();

        ResponseObject<List<ClientGroup>> response = new ResponseObject<List<ClientGroup>>();

        ClientInfo client = clientAdapter.GetById(resourceId);
        List<ClientGroup> groupsOfClient = new List<ClientGroup>(clientGroupAdapter.MemberOf(client));
        response.Obj = groupsOfClient;
        response.Success = true;
        response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_GET_GROUPS_OF_CLIENT;

        return response;
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    public Response DeleteClientGroup(Guid clientGroupId) {
      ISession session = factory.GetSessionForCurrentThread();

      try {
        IClientGroupAdapter clientGroupAdapter =
          session.GetDataAdapter<ClientGroup, IClientGroupAdapter>();

        Response response = new Response();

        ClientGroup clientGroup = clientGroupAdapter.GetById(clientGroupId);
        if (clientGroup == null) {
          response.Success = false;
          response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_CLIENTGROUP_DOESNT_EXIST;
          return response;
        }

        clientGroupAdapter.Delete(clientGroup);

        response.Success = true;
        response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_CLIENTGROUP_DELETED;
        return response;

      } finally {
        if (session != null)
          session.EndSession();
      }
    }

    #endregion
  }
}
