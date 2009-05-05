using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using HeuristicLab.Security.Contracts.BusinessObjects;

namespace HeuristicLab.Security.Contracts.Interfaces {

  [ServiceContract]
  public interface ISecurityManager {

    [OperationContract]
    [FaultContractAttribute(typeof(CommunicationException))]
    Permission AddPermission(Permission permission);

    [OperationContract]
    [FaultContractAttribute(typeof(CommunicationException))]
    bool RemovePermission(Guid permissionId);

    [OperationContract]
    [FaultContractAttribute(typeof(CommunicationException))]
    Permission UpdatePermission(Permission permission);

    [OperationContract]
    [FaultContractAttribute(typeof(CommunicationException))]
    User AddNewUser(User user);

    [OperationContract]
    [FaultContractAttribute(typeof(CommunicationException))]
    bool RemoveUser(Guid userId);

    [OperationContract]
    [FaultContractAttribute(typeof(CommunicationException))]
    User UpdateUser(User user);

    [OperationContract]
    [FaultContractAttribute(typeof(CommunicationException))]
    ICollection<User> GetAllUsers();

    [OperationContract]
    [FaultContractAttribute(typeof(CommunicationException))]
    User GetUserByName(string name);

    [OperationContract]
    [FaultContractAttribute(typeof(CommunicationException))]
    UserGroup AddNewUserGroup(UserGroup group);

    [OperationContract]
    [FaultContractAttribute(typeof(CommunicationException))]
    bool RemoveUserGroup(Guid userGroupId);

    [OperationContract]
    [FaultContractAttribute(typeof(CommunicationException))]
    UserGroup UpdateUserGroup(UserGroup group);

    [OperationContract]
    [FaultContractAttribute(typeof(CommunicationException))]
    ICollection<UserGroup> GetAllUserGroups();

    [OperationContract]
    [FaultContractAttribute(typeof(CommunicationException))]
    UserGroup GetUserGroupByName(string name);

    [OperationContract]
    [FaultContractAttribute(typeof(CommunicationException))]
    PermissionOwner UpdatePermissionOwner(PermissionOwner permissionOwner);

    [OperationContract]
    [FaultContractAttribute(typeof(CommunicationException))]
    bool AddPermissionOwnerToGroup(Guid userGroupId, Guid permissionOwnerId);

    [OperationContract]
    [FaultContractAttribute(typeof(CommunicationException))]
    bool RemovePermissionOwnerFromGroup(Guid userGroupId, Guid permissionOwnerId);

    [OperationContract]
    [FaultContractAttribute(typeof(CommunicationException))]
    bool GrantPermission(Guid permissionOwnerId, Guid permissionId, Guid entityId);

    [OperationContract]
    [FaultContractAttribute(typeof(CommunicationException))]
    Permission GetPermissionById(Guid permissionId);

    [OperationContract]
    [FaultContractAttribute(typeof(CommunicationException))]
    bool RevokePermission(Guid permissionOwnerId, Guid permissionId, Guid entityId);
  }
}
