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
    ResponseList<ClientInfo> GetAllClients();
    [OperationContract]
    [ServiceKnownType(typeof (Resource))]
    [ServiceKnownType(typeof(ClientInfo))]
    [ServiceKnownType(typeof(ClientGroup))]
    ResponseList<ClientGroup> GetAllClientGroups();
    [OperationContract]
    Response AddClientGroup(ClientGroup clientGroup);
    [OperationContract]
    Response AddResourceToGroup(long clientGroupId, Resource resource);
    [OperationContract]
    Response DeleteResourceFromGroup(long clientGroupId, long resourceId);
    [OperationContract]
    ResponseList<UpTimeStatistics> GetAllUpTimeStatistics();
  }
}
