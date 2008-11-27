using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Contracts.Interfaces;
using HeuristicLab.Hive.Contracts.BusinessObjects;

namespace HeuristicLab.Hive.Server.Core {
  public class ServerConsoleFacade: IServerConsoleFacade {
    private IClientManager clientManager = new ClientManager();
    private IJobManager jobManager = new JobManager();
    private IUserRoleManager userRoleManager = new UserRoleManager();
    
    #region IClientManager Members

    public List<ClientInfo> GetAllClients() {
      return clientManager.GetAllClients();
    }

    public List<ClientGroup> GetAllClientGroups() {
      return clientManager.GetAllClientGroups();
    }

    public List<UpTimeStatistics> GetAllUpTimeStatistics() {
      return clientManager.GetAllUpTimeStatistics();
    }

    #endregion

    #region IJobManager Members

    public List<HeuristicLab.Hive.Contracts.BusinessObjects.Job> GetAllJobs() {
      return jobManager.GetAllJobs();
    }

    #endregion

    #region IUserRoleManager Members

    public List<HeuristicLab.Hive.Contracts.BusinessObjects.User> GetAllUsers() {
      return userRoleManager.GetAllUsers();
    }

    public void AddNewUser(User user) {
      userRoleManager.AddNewUser(user);
    }

    public List<HeuristicLab.Hive.Contracts.BusinessObjects.UserGroup> GetAllUserGroups() {
      return userRoleManager.GetAllUserGroups();
    }

    #endregion
  }
}
