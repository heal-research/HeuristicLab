using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Client.Core.ClientConsoleService.Interfaces;
using HeuristicLab.Hive.Client.Core.ConfigurationManager;
using HeuristicLab.Hive.Client.Communication;

namespace HeuristicLab.Hive.Client.Core.ClientConsoleService {
  public class ClientConsoleCommunicator: IClientConsoleCommunicator {
    #region IClientConsoleCommunicator Members

    public StatusCommons GetStatusInfos() {
      lock (Core.Locker) {
        return ConfigManager.Instance.GetStatusForClientConsole();
      }
    }

    public ConnectionContainer GetConnection() {
      return new ConnectionContainer{IPAdress = WcfService.Instance.ServerIP, Port = WcfService.Instance.ServerPort } ;
    }

    public void SetConnection(ConnectionContainer container) {
      ConfigManager.Instance.SetServerIPAndPort(container);
      WcfService.Instance.Connect(container.IPAdress, container.Port);
    }

    public void Disconnect() {
      WcfService.Instance.Disconnect();
    }   

    public ConnectionContainer GetCurrentConnection() {
      return new ConnectionContainer { IPAdress = WcfService.Instance.ServerIP, Port = WcfService.Instance.ServerPort };
    }

    #endregion
  }
}
