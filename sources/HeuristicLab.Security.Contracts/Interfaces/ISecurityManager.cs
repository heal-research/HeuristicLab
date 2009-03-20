using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Security.Contracts.BusinessObjects;

namespace HeuristicLab.Security.Contracts.Interfaces {
  interface ISecurityManager {
    User AddNewUser(User user);
    bool RemoveUser(long userId);
    User UpdateUser(User user);

    UserGroup AddNewUserGroup(UserGroup group);
    bool RemoveUserGroup(long groupId);
    UserGroup UpdateUserGroup(UserGroup group);
 
    bool AddPermissionOwnerToGroup(long groupId, long permissionOwnerId);
    bool RemovePermissionOwnerFromGroup(long groupId, long permissionOwnerId);

    bool GrantPermission(long permissionOwnerId, Guid permissionToken);
    bool RevokePermission(long permissionOwnerId, Guid permissionToken);
  }
}
