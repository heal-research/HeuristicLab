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

      clients.Add(new ClientInfo { ClientId=Guid.NewGuid(), CpuSpeedPerCore=2500, Memory=4096, ResourceId=1, State=State.idle });
      clients.Add(new ClientInfo { ClientId=Guid.NewGuid(), CpuSpeedPerCore=2100, Memory=2048, ResourceId=2, State=State.idle });
      clients.Add(new ClientInfo { ClientId=Guid.NewGuid(), CpuSpeedPerCore=3400, Memory=4096, ResourceId=3, State=State.calculating });

      clientGroups.Add(new ClientGroup { ResourceId = 4, Name = "SuperGroup", ClientGroupId = 1 });
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
