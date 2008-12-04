using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Contracts.Interfaces;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Contracts;

namespace HeuristicLab.Hive.Server.Core {
  public class ServerConsoleFacade: IServerConsoleFacade {
    private IClientManager clientManager = new ClientManager();
    private IJobManager jobManager = new JobManager();
    private IUserRoleManager userRoleManager = new UserRoleManager();
    
    #region IClientManager Members

    public ResponseList<ClientInfo> GetAllClients() {
      return clientManager.GetAllClients();
    }

    public ResponseList<ClientGroup> GetAllClientGroups() {
      return clientManager.GetAllClientGroups();
    }

    public ResponseList<UpTimeStatistics> GetAllUpTimeStatistics() {
      return clientManager.GetAllUpTimeStatistics();
    }

    #endregion

    #region IJobManager Members

    public ResponseList<HeuristicLab.Hive.Contracts.BusinessObjects.Job> GetAllJobs() {
      return jobManager.GetAllJobs();
    }

    #endregion

    #region IUserRoleManager Members

    public ResponseList<HeuristicLab.Hive.Contracts.BusinessObjects.User> GetAllUsers() {
      return userRoleManager.GetAllUsers();
    }

    public Response AddNewUser(User user) {
      return userRoleManager.AddNewUser(user);
    }

    public ResponseList<HeuristicLab.Hive.Contracts.BusinessObjects.UserGroup> GetAllUserGroups() {
      return userRoleManager.GetAllUserGroups();
    }

    public Response RemoveUser(long userId) {
      throw new NotImplementedException();
    }

    public Response AddNewUserGroup(UserGroup userGroup) {
      throw new NotImplementedException();
    }

    public Response RemoveUserGroup(long groupId) {
      throw new NotImplementedException();
    }

    public Response AddUserToGroup(long groupId, long userId) {
      throw new NotImplementedException();
    }

    public Response RemoveUserFromGroup(long groupId, long userId) {
      throw new NotImplementedException();
    }

    #endregion

  }
}
