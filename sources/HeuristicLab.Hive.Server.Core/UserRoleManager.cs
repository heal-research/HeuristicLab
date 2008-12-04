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
      ResponseList<User> response = new ResponseList<User>();

      List<User> allUsers = new List<User>(userAdapter.GetAllUsers());
      response.List = allUsers;
      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_USERROLE_GET_ALL_USERS;

      return response;
    }

    public Response AddNewUser(User user) {
      Response response = new Response();

      User dbUser = userAdapter.GetUserByName(user.Name);
      if (dbUser != null) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_USERROLE_USER_EXISTS_ALLREADY;
        return response;
      }
      userAdapter.UpdateUser(user);
      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_USERROLE_USER_SUCCESSFULLY_ADDED;
      
      return response;
    }

    public ResponseList<UserGroup> GetAllUserGroups() {
      ResponseList<UserGroup> response = new ResponseList<UserGroup>();

      return response;
    }

    public Response RemoveUser(long userId) {
      Response response = new Response();
      User user = userAdapter.GetUserById(userId);
      if (user == null) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_USERROLE_USER_DOESNT_EXIST;
        return response;
      }
      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_USERROLE_USER_REMOVED;

      return response;
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
