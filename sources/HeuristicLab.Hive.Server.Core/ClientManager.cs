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

namespace HeuristicLab.Hive.Server.Core {
  class ClientManager: IClientManager {

    IClientAdapter clientAdapter;
    IClientGroupAdapter clientGroupAdapter;

    List<ClientGroup> clientGroups;

    public ClientManager() {
      clientAdapter = ServiceLocator.GetClientAdapter();
      clientGroupAdapter = ServiceLocator.GetClientGroupAdapter();

      clientGroups = new List<ClientGroup>();

      ClientGroup cg = new ClientGroup { Id = Guid.NewGuid(), Name = "SuperGroup" };
      cg.Resources = new List<Resource>();

      clientGroups.Add(cg);
    }

    #region IClientManager Members

    /// <summary>
    /// Returns all clients stored in the database
    /// </summary>
    /// <returns></returns>
    public ResponseList<ClientInfo> GetAllClients() {
      ResponseList<ClientInfo> response = new ResponseList<ClientInfo>();

      response.List = new List<ClientInfo>(clientAdapter.GetAll());
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

      response.List = new List<ClientGroup>(clientGroupAdapter.GetAll());
      response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_GET_ALL_CLIENTGROUPS;
      response.Success = true;

      return response;
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
    public Response AddClientGroup(ClientGroup clientGroup) {
      Response response = new Response();

      if (clientGroup.Id != Guid.Empty) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_ID_MUST_NOT_BE_SET;
        return response;
      }
      clientGroupAdapter.Update(clientGroup);
      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_CLIENTGROUP_ADDED;

      return response;
    }

    /// <summary>
    ///  Add a resource to a group
    /// </summary>
    /// <param name="clientGroupId"></param>
    /// <param name="resource"></param>
    /// <returns></returns>
    public Response AddResourceToGroup(Guid clientGroupId, Resource resource) {
      Response response = new Response();

      if (resource.Id != Guid.Empty) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_ID_MUST_NOT_BE_SET;
        return response;
      }

      ClientGroup clientGroup = clientGroupAdapter.GetById(clientGroupId);
      if (clientGroup == null) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_CLIENTGROUP_DOESNT_EXIST;
        return response;
      }
      clientGroup.Resources.Add(resource);

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
    #endregion
  }
}
