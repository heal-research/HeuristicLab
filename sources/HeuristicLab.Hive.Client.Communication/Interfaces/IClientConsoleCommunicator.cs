using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Client.Communication.ClientConsole;
using System.ServiceModel;

namespace HeuristicLab.Hive.Client.Communication.Interfaces {
  [ServiceContract]
  public interface IClientConsoleCommunicator {
    [OperationContract]
    StatusCommons GetStatusInfos();
    [OperationContract]
    ConnectionContainer GetConnection();
    [OperationContract]
    void SetConnection(ConnectionContainer container);
  }
}
