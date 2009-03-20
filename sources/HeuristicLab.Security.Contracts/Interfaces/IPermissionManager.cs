using System;
using System.Collections.Generic;
using System.Text;

namespace HeuristicLab.Security.Contracts.Interfaces {
  interface IPermissionManager {
    Guid Authenticate(String userName, String password);
    bool CheckPermission(Guid sessionId, Guid permissionToken,
      Guid enitityId);
    void EndSession(Guid sessionId);
  }
}
