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

      ClientGroup cg = new ClientGroup { ResourceId = 4, Name = "SuperGroup", ClientGroupId = 1 };
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
      throw new NotImplementedException();
    }

    public Response AddResourceToGroup(long clientGroupId, Resource resource) {
      throw new NotImplementedException();
    }

    public Response DeleteResourceFromGroup(long clientGroupId, long resourceId) {
      throw new NotImplementedException();
    }
    #endregion
  }
}
