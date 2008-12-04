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
    Response AddNewUser(User user);
    [OperationContract]
    Response RemoveUser(long userId);
    [OperationContract]
    Response AddNewUserGroup(UserGroup userGroup);
    [OperationContract]
    Response RemoveUserGroup(long groupId);
    [OperationContract]
    ResponseList<UserGroup> GetAllUserGroups();
    [OperationContract]
    Response AddUserToGroup(long groupId, long userId);
    [OperationContract]
    Response RemoveUserFromGroup(long groupId, long userId);
  }
}
