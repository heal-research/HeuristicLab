#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

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
