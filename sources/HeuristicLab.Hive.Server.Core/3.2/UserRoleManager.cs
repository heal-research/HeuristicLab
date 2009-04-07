#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

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

    IUserAdapter userAdapter;
    IUserGroupAdapter userGroupAdapter;
    IPermissionOwnerAdapter permissionOwnerAdapter;

    public UserRoleManager() {
      userAdapter = ServiceLocator.GetUserAdapter();
      userGroupAdapter = ServiceLocator.GetUserGroupAdapter();
    }

    #region IUserRoleManager Members

    /// <summary>
    /// returns all users from the database
    /// </summary>
    /// <returns></returns>
    public ResponseList<User> GetAllUsers() {
      ResponseList<User> response = new ResponseList<User>();

      List<User> allUsers = new List<User>(userAdapter.GetAll());
      response.List = allUsers;
      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_USERROLE_GET_ALL_USERS;

      return response;
    }

    /// <summary>
    /// Adds a new user to the database
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public ResponseObject<User> AddNewUser(User user) {
      ResponseObject<User> response = new ResponseObject<User>();

      if (user.Id != 0) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_USERROLE_ID_MUST_NOT_BE_SET;
        return response;
      }
      if (userAdapter.GetByName(user.Name) != null) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_USERROLE_USERNAME_EXISTS_ALLREADY;
        return response;
      }

      userAdapter.Update(user);
      response.Obj = user;
      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_USERROLE_USER_SUCCESSFULLY_ADDED;
      
      return response;
    }

    /// <summary>
    /// returns all usergroups from the database
    /// </summary>
    /// <returns></returns>
    public ResponseList<UserGroup> GetAllUserGroups() {
      ResponseList<UserGroup> response = new ResponseList<UserGroup>();

      response.List = new List<UserGroup>(userGroupAdapter.GetAll());
      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_USERROLE_GET_ALL_USERGROUPS;

      return response;
    }

    /// <summary>
    /// Removes a user from the database
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public Response RemoveUser(long userId) {
      Response response = new Response();
      User user = userAdapter.GetById(userId);
      if (user == null) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_USERROLE_USER_DOESNT_EXIST;
        return response;
      }
      userAdapter.Delete(user);
      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_USERROLE_USER_REMOVED;
                         
      return response;
    }

    /// <summary>
    /// Adds a new usergroup to the database
    /// </summary>
    /// <param name="userGroup"></param>
    /// <returns></returns>
    public ResponseObject<UserGroup> AddNewUserGroup(UserGroup userGroup) {
      ResponseObject<UserGroup> response = new ResponseObject<UserGroup>();
      
      if (userGroup.Id != 0) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_USERROLE_ID_MUST_NOT_BE_SET;
        return response;
      }

      userGroupAdapter.Update(userGroup);
      response.Obj = userGroup;
      response.Success = false;
      response.StatusMessage = ApplicationConstants.RESPONSE_USERROLE_USERGROUP_ADDED;

      return response;
    }

    /// <summary>
    /// Removes a user group from the database
    /// </summary>
    /// <param name="groupId"></param>
    /// <returns></returns>
    public Response RemoveUserGroup(long groupId) {
      Response response = new Response();

      UserGroup userGroupFromDb = userGroupAdapter.GetById(groupId);
      if (userGroupFromDb == null) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_USERROLE_USERGROUP_DOESNT_EXIST;
        return response;
      }
      userGroupAdapter.Delete(userGroupFromDb);
      response.Success = false;
      response.StatusMessage = ApplicationConstants.RESPONSE_USERROLE_USERGROUP_ADDED;

      return response;
    }

    /// <summary>
    /// Adds a user into a user group
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public Response AddUserToGroup(long groupId, long userId) {
      Response response = new Response();

      User user = userAdapter.GetById(userId);
      if (user == null) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_USERROLE_USER_DOESNT_EXIST;
        return response;
      }

      UserGroup userGroup = userGroupAdapter.GetById(groupId);
      if (userGroup == null) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_USERROLE_USERGROUP_DOESNT_EXIST;
        return response;
      }
      userGroup.Members.Add(user);
      userGroupAdapter.Update(userGroup);

      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_USERROLE_PERMISSIONOWNER_ADDED;

      return response;
    }

    /// <summary>
    /// Adds a user group to a user group
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="groupToAddId"></param>
    /// <returns></returns>
    public Response AddUserGroupToGroup(long groupId, long groupToAddId) {
      Response response = new Response();

      UserGroup userGroup = userGroupAdapter.GetById(groupId);
      UserGroup userGroupToAdd = userGroupAdapter.GetById(groupToAddId);

      if (userGroup == null || userGroupToAdd == null) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_USERROLE_USERGROUP_DOESNT_EXIST;
        return response;
      }
      userGroup.Members.Add(userGroupToAdd);
      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_USERROLE_USERGROUP_ADDED_TO_USERGROUP;

      return response;
    }

    /// <summary>
    /// Removes a permission owner (user, user group) from a user group
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="permissionOwnerId"></param>
    /// <returns></returns>
    public Response RemovePermissionOwnerFromGroup(long groupId, long permissionOwnerId) {
      Response response = new Response();

      UserGroup userGroup = userGroupAdapter.GetById(groupId);
      if (userGroup == null) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_USERROLE_USERGROUP_DOESNT_EXIST;
        return response;
      }
      User user = userAdapter.GetById(permissionOwnerId);
      if (user == null) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_USERROLE_PERMISSIONOWNER_DOESNT_EXIST;
        return response;
      }
      foreach (PermissionOwner permissionOwner in userGroup.Members) {
        if (permissionOwner.Id == permissionOwnerId) {
          userGroup.Members.Remove(permissionOwner);
          userGroupAdapter.Update(userGroup);
          response.Success = true;
          response.StatusMessage = ApplicationConstants.RESPONSE_USERROLE_PERMISSIONOWNER_REMOVED;
          return response;
        }
      }
      response.Success = false;
      response.StatusMessage = ApplicationConstants.RESPONSE_USERROLE_PERMISSIONOWNER_DOESNT_EXIST;
      
      return response;
    }

    #endregion
  }
}
