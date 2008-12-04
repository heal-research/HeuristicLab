using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Contracts.Interfaces;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Server.Core.InternalInterfaces.DataAccess;
using System.Resources;
using System.Reflection;
using HeuristicLab.Hive.Contracts;

namespace HeuristicLab.Hive.Server.Core {
  class UserRoleManager: IUserRoleManager {

    List<User> users;
    List<UserGroup> userGroups;

    IUserAdapter userAdapter;
    ResourceManager rm;

    public UserRoleManager() {
      userAdapter = ServiceLocator.GetUserAdapter();
      rm = new ResourceManager("HiveServerMessages.resx", Assembly.GetExecutingAssembly()); 

      users = new List<User>();
      userGroups = new List<UserGroup>();

      users.Add(new User { PermissionOwnerId = 1, Name = "Hugo", Password = "hUg0" });
      users.Add(new User { PermissionOwnerId = 2, Name = "Seppl", Password = "seppl" });
      users.Add(new User { PermissionOwnerId = 3, Name = "Greg", Password = "greg" });

      userGroups.Add(new UserGroup { UserGroupId = 1 });
      userGroups.Add(new UserGroup { UserGroupId = 2 });
    }

    #region IUserRoleManager Members

    public ResponseList<User> GetAllUsers() {
      return null;      
    }

    public Response AddNewUser(User user) {
      User dbUser = userAdapter.GetUserByName(user.Name);

      return null;
    }

    public ResponseList<UserGroup> GetAllUserGroups() {
      return null;
    }

    public Response RemoveUser(long userId) {
      return null;
    }

    public Response AddNewUserGroup(UserGroup userGroup) {
      return null;
    }

    public Response RemoveUserGroup(long groupId) {
      return null;
    }

    public Response AddUserToGroup(long groupId, long userId) {
      throw new NotImplementedException();
    }

    public Response RemoveUserFromGroup(long groupId, long userId) {
      throw new NotImplementedException();
    }

    #endregion
  }
}
