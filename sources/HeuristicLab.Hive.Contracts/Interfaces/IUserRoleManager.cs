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
    List<User> GetAllUsers();
    [OperationContract]
    void AddNewUser(User user);
    [OperationContract]
    List<UserGroup> GetAllUserGroups();
  }
}
