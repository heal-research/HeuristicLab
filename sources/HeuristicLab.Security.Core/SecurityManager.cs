using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Security.Contracts.Interfaces;
using HeuristicLab.Security.Contracts.BusinessObjects;
using HeuristicLab.Security.DataAccess;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Security.Core {
  public class SecurityManager : ISecurityManager {

    private static DiscoveryService discoveryService =
      new DiscoveryService();

    private static IUserAdapter userAdapter = discoveryService.GetInstances<IUserAdapter>()[0];
    private static IUserGroupAdapter userGroupAdapter = discoveryService.GetInstances<IUserGroupAdapter>()[0];
    private static IPermissionOwnerAdapter permOwnerAdapter = discoveryService.GetInstances<IPermissionOwnerAdapter>()[0];
    private static IPermissionAdapter permissionAdapter = discoveryService.GetInstances<IPermissionAdapter>()[0];

    public User AddNewUser(User user) {
      userAdapter.Update(user);
      return user;
    }

    public User UpdateUser(User user) {
      userAdapter.Update(user);
      return user;
    }

    public bool RemoveUser(Guid userId) {
      User user = userAdapter.GetById(userId);
      if ( user != null )       // do we check this ?
        return userAdapter.Delete(user);
      return false;
    }

    public UserGroup AddNewUserGroup(UserGroup group) {
      userGroupAdapter.Update(group);
      return group;
    }

    public UserGroup UpdateUserGroup(UserGroup group) {
      userGroupAdapter.Update(group);
      return group;
    }

    public bool RemoveUserGroup(Guid groupId) {
      UserGroup userGroup = userGroupAdapter.GetById(groupId);
      if (userGroup != null)
        return userGroupAdapter.Delete(userGroup);
      return false;
    }

    public bool AddPermissionOwnerToGroup(Guid groupId, Guid permissionOwnerId) {
      UserGroup userGroup = userGroupAdapter.GetById(groupId);
      PermissionOwner permissionOwner = permOwnerAdapter.GetById(permissionOwnerId); 
      userGroup.Members.Add(permissionOwner);
      userGroupAdapter.Update(userGroup);
      return true;
    }

    public bool RemovePermissionOwnerFromGroup(Guid groupId, Guid permissionOwnerId) {
      UserGroup userGroup = userGroupAdapter.GetById(groupId);
      PermissionOwner permissionOwner = permOwnerAdapter.GetById(permissionOwnerId);
      userGroup.Members.Add(permissionOwner);
      userGroupAdapter.Delete(userGroup);
      return true;
    }

    public bool GrantPermission(Guid permissionOwnerId, Guid permissionId, Guid entityId) {
      return permissionAdapter.addPermission(permissionOwnerId, permissionId, entityId);
    }

    public bool RevokePermission(Guid permissionOwnerId, Guid permissionId, Guid entityId) {
      return permissionAdapter.removePermission(permissionOwnerId, permissionId, entityId);
    }
 
  }
}
