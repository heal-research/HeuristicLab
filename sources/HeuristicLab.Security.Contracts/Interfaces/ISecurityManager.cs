using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Security.Contracts.BusinessObjects;

namespace HeuristicLab.Security.Contracts.Interfaces {
  interface ISecurityManager {
    User AddNewUser(User user);
    bool RemoveUser(Guid userId);
    User UpdateUser(User user);

    UserGroup AddNewUserGroup(UserGroup group);
    bool RemoveUserGroup(Guid groupId);
    UserGroup UpdateUserGroup(UserGroup group);

    bool AddPermissionOwnerToGroup(Guid groupId, Guid permissionOwnerId);
    bool RemovePermissionOwnerFromGroup(Guid groupId, Guid permissionOwnerId);

    bool GrantPermission(Guid permissionOwnerId, Guid permissionId);
    bool RevokePermission(Guid permissionOwnerId, Guid permissionId);
  }
}
