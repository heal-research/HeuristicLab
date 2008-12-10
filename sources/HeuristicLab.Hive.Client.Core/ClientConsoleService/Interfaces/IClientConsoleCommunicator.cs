using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace HeuristicLab.Hive.Client.Core.ClientConsoleService.Interfaces {
  [ServiceContract]
  public interface IClientConsoleCommunicator {
    [OperationContract]
    StatusCommons GetStatusInfos();
    [OperationContract]
    ConnectionContainer GetConnection();
    [OperationContract]
    void SetConnection(ConnectionContainer container);
    [OperationContract]
    void Disconnect();
  }
}
