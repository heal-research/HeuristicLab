using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using HeuristicLab.Hive.Contracts.BusinessObjects;

namespace HeuristicLab.Hive.Contracts.Interfaces {
  /// <summary>
  /// This is the faced for the management console
  /// </summary>
  [ServiceContract]
  public interface IClientManager {
    [OperationContract]
    List<ClientInfo> GetAllClients();
    [OperationContract]
    List<ClientGroup> GetAllClientGroups();
    [OperationContract]
    List<UpTimeStatistics> GetAllUpTimeStatistics();
  }
}
