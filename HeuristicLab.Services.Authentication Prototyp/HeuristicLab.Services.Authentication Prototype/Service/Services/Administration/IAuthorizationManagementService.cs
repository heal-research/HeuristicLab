using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Service.Services.Administration {
  [ServiceContract]
  interface IAuthorizationManagementService {

    [OperationContract]
    void CreateRole(string roleName, bool isPermission);

  }
}
