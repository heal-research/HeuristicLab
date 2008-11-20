using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Contracts.Interfaces;
using HeuristicLab.Hive.Contracts.BusinessObjects;

namespace HeuristicLab.Hive.Server.Core {
  class ClientManager: IClientManager {
    #region IClientManager Members

    public List<ClientInfo> GetAllClients() {
      return new List<ClientInfo>();
    }

    public List<ClientGroup> GetAllClientGroups() {
      return new List<ClientGroup>();
    }

    public List<UpTimeStatistics> GetAllUpTimeStatistics() {
      return new List<UpTimeStatistics>();
    }

    #endregion
  }
}
