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
using HeuristicLab.Hive.Server.Core.InternalInterfaces.DataAccess;

namespace HeuristicLab.Hive.Server.Core {
  class ClientManager: IClientManager {

    IClientAdapter clientAdapter;
    IClientGroupAdapter clientGroupAdapter;

    List<ClientGroup> clientGroups;

    public ClientManager() {
      clientAdapter = ServiceLocator.GetClientAdapter();
      clientGroupAdapter = ServiceLocator.GetClientGroupAdapter();

      clientGroups = new List<ClientGroup>();

      ClientGroup cg = new ClientGroup { ResourceId = 4, Name = "SuperGroup" };
      cg.Resources = new List<Resource>();

      clientGroups.Add(cg);
    }

    #region IClientManager Members

    public ResponseList<ClientInfo> GetAllClients() {
      ResponseList<ClientInfo> response = new ResponseList<ClientInfo>();

      response.List = new List<ClientInfo>(clientAdapter.GetAllClients());
      response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_GET_ALL_CLIENTS;
      response.Success = true;

      return response;
    }

    public ResponseList<ClientGroup> GetAllClientGroups() {
      ResponseList<ClientGroup> response = new ResponseList<ClientGroup>();

      response.List = new List<ClientGroup>(clientGroupAdapter.GetAllClientGroups());
      response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_GET_ALL_CLIENTGROUPS;
      response.Success = true;

      return response;
    }

    public ResponseList<UpTimeStatistics> GetAllUpTimeStatistics() {
      ResponseList<UpTimeStatistics> response = new ResponseList<UpTimeStatistics>();
      response.Success = true;
      return response;
    }

    public Response AddClientGroup(ClientGroup clientGroup) {
      Response response = new Response();

      if (clientGroup.ResourceId != 0) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_ID_MUST_NOT_BE_SET;
        return response;
      }
      clientGroupAdapter.UpdateClientGroup(clientGroup);
      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_CLIENTGROUP_ADDED;

      return response;
    }

    public Response AddResourceToGroup(long clientGroupId, Resource resource) {
      Response response = new Response();

      if (resource.ResourceId != 0) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_ID_MUST_NOT_BE_SET;
        return response;
      }

      ClientGroup clientGroup = clientGroupAdapter.GetClientGroupById(clientGroupId);
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

    public Response DeleteResourceFromGroup(long clientGroupId, long resourceId) {
      Response response = new Response();

      ClientGroup clientGroup = clientGroupAdapter.GetClientGroupById(clientGroupId);
      if (clientGroup == null) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_CLIENT_CLIENTGROUP_DOESNT_EXIST;
        return response;
      }
      foreach (Resource resource in clientGroup.Resources) {
        if (resource.ResourceId == resourceId) {
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
