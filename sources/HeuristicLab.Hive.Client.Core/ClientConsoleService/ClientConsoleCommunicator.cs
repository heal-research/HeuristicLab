using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Client.Core.ClientConsoleService.Interfaces;

namespace HeuristicLab.Hive.Client.Core.ClientConsoleService {
  public class ClientConsoleCommunicator: IClientConsoleCommunicator {
    #region IClientConsoleCommunicator Members

    public StatusCommons GetStatusInfos() {
      throw new NotImplementedException();
    }

    public ConnectionContainer GetConnection() {
      throw new NotImplementedException();
    }

    public void SetConnection(ConnectionContainer container) {
      throw new NotImplementedException();
    }

    #endregion
  }
}
