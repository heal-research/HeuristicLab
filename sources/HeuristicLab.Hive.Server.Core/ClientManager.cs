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

namespace HeuristicLab.Hive.Server.Core {
  class ClientManager: IClientManager {

    List<ClientInfo> clients;
    List<ClientGroup> clientGroups;

    public ClientManager() {
      clients = new List<ClientInfo>();
      clientGroups = new List<ClientGroup>();

      ClientInfo c1 = new ClientInfo { ClientId=Guid.NewGuid(), Name="Client1", CpuSpeedPerCore=2500, Memory=4096, ResourceId=1, State=State.idle };
      ClientInfo c2 = new ClientInfo { ClientId=Guid.NewGuid(), Name="Client2",  CpuSpeedPerCore=2100, Memory=2048, ResourceId=2, State=State.idle };
      ClientInfo c3 = new ClientInfo { ClientId = Guid.NewGuid(), Name="Client3", CpuSpeedPerCore = 3400, Memory = 4096, ResourceId = 3, State = State.calculating };

      clients.Add(c1);
      clients.Add(c2);
      clients.Add(c3);

      ClientGroup cg = new ClientGroup { ResourceId = 4, Name = "SuperGroup", ClientGroupId = 1 };
      cg.Resources = new List<Resource>();
      cg.Resources.Add(c1);      
      cg.Resources.Add(c2);
      cg.Resources.Add(c3);

      clientGroups.Add(cg);
    }

    #region IClientManager Members

    public ResponseList<ClientInfo> GetAllClients() {
      ResponseList<ClientInfo> response = new ResponseList<ClientInfo>();
      response.List = clients;
      response.Success = true;
      return response;
    }

    public ResponseList<ClientGroup> GetAllClientGroups() {
      ResponseList<ClientGroup> response = new ResponseList<ClientGroup>();
      response.List = clientGroups;
      response.Success = true;
      return response;
    }

    public ResponseList<UpTimeStatistics> GetAllUpTimeStatistics() {
      ResponseList<UpTimeStatistics> response = new ResponseList<UpTimeStatistics>();
      response.Success = true;
      return response;
    }

    #endregion
  }
}
