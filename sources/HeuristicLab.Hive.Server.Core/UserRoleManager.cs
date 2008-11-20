using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Contracts.Interfaces;
using HeuristicLab.Hive.Contracts.BusinessObjects;

namespace HeuristicLab.Hive.Server.Core {
  class UserRoleManager: IUserRoleManager {
    #region IUserRoleManager Members

    public List<User> GetAllUsers() {
      throw new NotImplementedException();
    }

    public List<UserGroup> GetAllUserGroups() {
      throw new NotImplementedException();
    }

    #endregion
  }
}
