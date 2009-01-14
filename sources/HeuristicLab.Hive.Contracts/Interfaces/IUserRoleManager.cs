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
using System.ServiceModel;
using HeuristicLab.Hive.Contracts.BusinessObjects;

namespace HeuristicLab.Hive.Contracts.Interfaces {
  /// <summary>
  /// This is the facade for the User/Role Manager used by the Management Console
  /// </summary>
  [ServiceContract]
  public interface IUserRoleManager {
    [OperationContract]
    ResponseList<User> GetAllUsers();
    [OperationContract]
    ResponseObject<User> AddNewUser(User user);
    [OperationContract]
    Response RemoveUser(long userId);
    [OperationContract]
    ResponseObject<UserGroup> AddNewUserGroup(UserGroup userGroup);
    [OperationContract]
    Response RemoveUserGroup(long groupId);
    [OperationContract]
    [ServiceKnownType(typeof(PermissionOwner))]
    [ServiceKnownType(typeof(User))]
    [ServiceKnownType(typeof(UserGroup))]
    ResponseList<UserGroup> GetAllUserGroups();
    [OperationContract]
    Response AddUserToGroup(long groupId, long userId);
    [OperationContract]
    Response AddUserGroupToGroup(long groupId, long groupToAddId);
    [OperationContract]
    Response RemovePermissionOwnerFromGroup(long groupId, long userId);
  }
}
