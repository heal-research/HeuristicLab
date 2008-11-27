using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Contracts.Interfaces;
using HeuristicLab.Hive.Contracts.BusinessObjects;

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

    public List<ClientInfo> GetAllClients() {
      return clients;
    }

    public List<ClientGroup> GetAllClientGroups() {
      return clientGroups;
    }

    public List<UpTimeStatistics> GetAllUpTimeStatistics() {
      return new List<UpTimeStatistics>();
    }

    #endregion
  }
}
