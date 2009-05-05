using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace HeuristicLab.Security.Contracts.Interfaces {

  [ServiceContract]
  public interface IPermissionManager  {

    [OperationContract]
    [FaultContractAttribute(typeof(CommunicationException))]
    Guid Authenticate(String userName, String password);

    [OperationContract]
    [FaultContractAttribute(typeof(CommunicationException))]
    bool CheckPermission(Guid sessionId, Guid permissionId,
      Guid enitityId);

    [OperationContract]
    [FaultContractAttribute(typeof(CommunicationException))]
    void EndSession(Guid sessionId);

  }
}
