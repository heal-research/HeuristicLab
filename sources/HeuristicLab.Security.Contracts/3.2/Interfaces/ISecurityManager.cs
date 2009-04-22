using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using HeuristicLab.Security.Contracts.BusinessObjects;

namespace HeuristicLab.Security.Contracts.Interfaces {

  [ServiceContract]
  public interface ISecurityManager {

    [OperationContract]
    User AddNewUser(User user);

    [OperationContract]
    bool RemoveUser(Guid userId);

    [OperationContract]
    User UpdateUser(User user);

    [OperationContract]
    ICollection<User> GetAllUsers();

    [OperationContract]
    User GetUserByName(string name);

    [OperationContract]
    UserGroup AddNewUserGroup(UserGroup group);

    [OperationContract]
    bool RemoveUserGroup(Guid userGroupId);

    [OperationContract]
    UserGroup UpdateUserGroup(UserGroup group);

    [OperationContract]
    ICollection<UserGroup> GetAllUserGroups();

    [OperationContract]
    UserGroup GetUserGroupByName(string name);

    [OperationContract]
    PermissionOwner UpdatePermissionOwner(PermissionOwner permissionOwner);

    [OperationContract]
    bool AddPermissionOwnerToGroup(Guid userGroupId, Guid permissionOwnerId);

    [OperationContract]
    bool RemovePermissionOwnerFromGroup(Guid userGroupId, Guid permissionOwnerId);

    [OperationContract]
    bool GrantPermission(Guid permissionOwnerId, Guid permissionId, Guid entityId);

    [OperationContract]
    Permission GetPermissionById(Guid permissionId);

    [OperationContract]
    bool RevokePermission(Guid permissionOwnerId, Guid permissionId, Guid entityId);
  }
}
