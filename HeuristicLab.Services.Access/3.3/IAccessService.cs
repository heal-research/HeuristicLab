#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Net.Security;
using System.ServiceModel;
using HeuristicLab.Services.Access.DataTransfer;

namespace HeuristicLab.Services.Access {
  [ServiceContract(ProtectionLevel = ProtectionLevel.EncryptAndSign)]
  public interface IAccessService {

    #region Client
    [OperationContract]
    bool ClientExists(Guid id);

    [OperationContract]
    Client GetClient(Guid id);

    [OperationContract]
    IEnumerable<Client> GetClients(IEnumerable<Guid> ids);

    [OperationContract]
    IEnumerable<Client> GetAllClients();

    [OperationContract]
    void AddClient(Client client);

    [OperationContract]
    void UpdateClient(Client client);

    [OperationContract]
    void DeleteClient(Client client);
    #endregion

    #region Client Group
    [OperationContract]
    IEnumerable<ClientGroup> GetAllClientGroups();

    [OperationContract]
    IEnumerable<ClientGroup> GetClientGroups(IEnumerable<Guid> ids);

    [OperationContract]
    Guid AddClientGroup(ClientGroup group);

    [OperationContract]
    void UpdateClientGroup(ClientGroup group);

    [OperationContract]
    void DeleteClientGroup(ClientGroup group);

    [OperationContract]
    void AddResourceToGroup(Resource resource, ClientGroup group);

    [OperationContract]
    void RemoveResourceFromGroup(Resource resource, ClientGroup group);
    #endregion

    #region ClientGroupMapping
    [OperationContract]
    IEnumerable<ClientGroupMapping> GetClientGroupMapping();
    #endregion

    #region Resource
    [OperationContract]
    IEnumerable<Resource> GetResources();
    #endregion

    #region ClientLog
    [OperationContract]
    ClientLog GetLastClientLog(Guid clientId);

    [OperationContract]
    IEnumerable<ClientLog> GetClientLogs(Guid clientId);

    [OperationContract]
    IEnumerable<ClientLog> GetClientLogsSince(DateTime startDate);

    [OperationContract]
    void AddClientLog(ClientLog log);

    [OperationContract]
    void DeleteClientLog(ClientLog log);
    #endregion

    #region User
    [OperationContract]
    LightweightUser Login();

    [OperationContract]
    IEnumerable<UserGroup> GetGroupsOfCurrentUser();

    [OperationContract]
    IEnumerable<Role> GetRolesOfCurrentUser();

    [OperationContract]
    IEnumerable<LightweightUser> GetAllLightweightUsers();

    [OperationContract]
    IEnumerable<LightweightUser> GetLightweightUsers(IEnumerable<Guid> ids);

    [OperationContract]
    void UpdateLightweightUser(LightweightUser user);

    [OperationContract]
    IEnumerable<User> GetAllUsers();

    [OperationContract]
    IEnumerable<User> GetUsers(IEnumerable<Guid> ids);

    [OperationContract]
    User AddUser(User user);

    [OperationContract]
    void DeleteUser(User user);

    [OperationContract]
    void UpdateUser(User user);

    [OperationContract]
    void AddUserToRole(Role role, User user);

    [OperationContract]
    void RemoveUserFromRole(Role role, User user);

    [OperationContract]
    bool ChangePassword(Guid userId, string oldPassword, string newPassword);

    [OperationContract]
    string ResetPassword(Guid userId);
    #endregion

    #region UserGroup
    [OperationContract]
    IEnumerable<UserGroup> GetAllUserGroups();

    [OperationContract]
    IEnumerable<UserGroup> GetUserGroupsOfUser(Guid userId);

    [OperationContract]
    IEnumerable<UserGroup> GetUserGroups(IEnumerable<Guid> ids);

    [OperationContract]
    Guid AddUserGroup(UserGroup group);

    [OperationContract]
    void UpdateUserGroup(UserGroup group);

    [OperationContract]
    void DeleteUserGroup(UserGroup group);

    [OperationContract]
    void AddUserGroupBaseToGroup(UserGroupBase resource, UserGroup group);

    [OperationContract]
    void RemoveUserGroupBaseFromGroup(UserGroupBase resource, UserGroup group);
    #endregion

    #region UserGroupBase
    [OperationContract]
    IEnumerable<UserGroupBase> GetUsersAndGroups();

    [OperationContract]
    IEnumerable<UserGroupBase> GetAllLeightweightUsersAndGroups();

    [OperationContract]
    IEnumerable<UserGroupBase> GetLeightweightUsersAndGroups(IEnumerable<Guid> ids);
    #endregion

    #region UserGroupMapping
    [OperationContract]
    IEnumerable<UserGroupMapping> GetUserGroupMapping();

    [OperationContract]
    IEnumerable<Guid> GetUserGroupIdsOfGroup(Guid groupId);
    #endregion

    #region Role
    [OperationContract]
    IEnumerable<Role> GetRoles();

    [OperationContract]
    Role AddRole(Role role);

    /*[OperationContract]
    void UpdateRole(Role role);*/

    [OperationContract]
    void DeleteRole(Role role);

    [OperationContract]
    IEnumerable<Role> GetUserRoles(User user);

    [OperationContract]
    void AddRoleToGroup(UserGroup userGroup, Role role);

    [OperationContract]
    void RemoveRoleFromGroup(UserGroup userGroup, Role role);
    #endregion

    #region ClientError
    [OperationContract]
    void ReportError(ClientError error);

    [OperationContract]
    IEnumerable<ClientError> GetClientErrors();

    [OperationContract]
    IEnumerable<ClientError> GetLastClientErrors(DateTime startDate);

    [OperationContract]
    void DeleteError(ClientError error);
    #endregion
  }
}
