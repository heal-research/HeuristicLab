using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Contracts.Interfaces;
using HeuristicLab.Hive.Contracts.BusinessObjects;

namespace HeuristicLab.Hive.Server.Core {
  class UserRoleManager: IUserRoleManager {

    List<User> users;
    List<UserGroup> userGroups;

    public UserRoleManager() {
      users = new List<User>();
      userGroups = new List<UserGroup>();

      users.Add(new User { UserId = 1, Name = "Hugo", Password = "hUg0" });
      users.Add(new User { UserId = 2, Name = "Seppl", Password = "seppl" });
      users.Add(new User { UserId = 3, Name = "Greg", Password = "greg" });

      userGroups.Add(new UserGroup { UserGroupId = 1 });
      userGroups.Add(new UserGroup { UserGroupId = 2 });
    }

    #region IUserRoleManager Members

    public List<User> GetAllUsers() {
      return users;      
    }

    public List<UserGroup> GetAllUserGroups() {
      return userGroups;
    }

    #endregion
  }
}
