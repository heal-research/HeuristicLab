using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.Services.Administration {
  class AuthorizationManagementService : IAuthorizationManagementService{
    #region IAuthorizationManagementService Member

    public void CreateRole(string roleName, bool isPermission) {
      new Service.Provider.HeuristicLabRoleProvider().CreateRole(roleName, isPermission);
      

    }

    #endregion
  }
}
