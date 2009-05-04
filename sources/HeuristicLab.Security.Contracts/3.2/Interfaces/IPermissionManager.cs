using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace HeuristicLab.Security.Contracts.Interfaces {

  [ServiceContract]
  public interface IPermissionManager  {

    [OperationContract]
    Guid Authenticate(String userName, String password);

    [OperationContract]
    bool CheckPermission(Guid sessionId, Guid permissionId,
      Guid enitityId);

    [OperationContract]
    void EndSession(Guid sessionId);

  }
}
