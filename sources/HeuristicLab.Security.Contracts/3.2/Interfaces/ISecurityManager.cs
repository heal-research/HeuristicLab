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
    UserGroup AddNewUserGroup(UserGroup group);

    [OperationContract]
    bool RemoveUserGroup(UserGroup group);

    [OperationContract]
    UserGroup UpdateUserGroup(UserGroup group);

    [OperationContract]
    bool AddPermissionOwnerToGroup(UserGroup userGroup, PermissionOwner permissionOwner);

    [OperationContract]
    bool RemovePermissionOwnerFromGroup(UserGroup userGroup, PermissionOwner permissionOwner);

    [OperationContract]
    bool GrantPermission(PermissionOwner permissionOwner, Permission permission);

    [OperationContract]
    bool RevokePermission(PermissionOwner permissionOwner, Permission permission);
  }
}
