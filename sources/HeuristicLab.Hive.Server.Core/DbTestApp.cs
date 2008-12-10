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
using System.Text;
using HeuristicLab.PluginInfrastructure;
using System.Net;
using HeuristicLab.Hive.Contracts;
using HeuristicLab.Hive.Contracts.Interfaces;
using HeuristicLab.Hive.Server.Core.InternalInterfaces.DataAccess;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using System.Diagnostics;

namespace HeuristicLab.Hive.Server {
  [ClassInfo(Name = "Hive DB Test App",
      Description = "Test Application for the Hive DataAccess Layer",
      AutoRestart = true)]
  class HiveDbTestApplication : ApplicationBase {
    private void TestClientAdapter() {
      IClientAdapter clientAdapter =
        ServiceLocator.GetClientAdapter();

      ClientInfo client = new ClientInfo();
      client.Login = DateTime.Now;
      client.ClientId = Guid.NewGuid();
      clientAdapter.UpdateClient(client);

      ClientInfo clientRead =
        clientAdapter.GetClientById(client.ClientId);
      Debug.Assert(
        clientRead != null &&
        client.ClientId == clientRead.ClientId);

      client.CpuSpeedPerCore = 2000;
      clientAdapter.UpdateClient(client);
      clientRead =
        clientAdapter.GetClientById(client.ClientId);
      Debug.Assert(
       clientRead != null &&
       client.ClientId == clientRead.ClientId &&
       clientRead.CpuSpeedPerCore == 2000);

      ICollection<ClientInfo> clients = clientAdapter.GetAllClients();
      int count = clients.Count;

      clientAdapter.DeleteClient(client);

      clients = clientAdapter.GetAllClients();
      Debug.Assert(count - 1 == clients.Count);
    }

    private void TestUserAdapter() {
      IUserAdapter userAdapter =
        ServiceLocator.GetUserAdapter();

      User user = new User();
      user.Name = "Stefan";

      userAdapter.UpdateUser(user);

      User userRead =
        userAdapter.GetUserById(user.PermissionOwnerId);
      Debug.Assert(
        userRead != null &&
        user.PermissionOwnerId == userRead.PermissionOwnerId);

      user.Password = "passme";
      userAdapter.UpdateUser(user);
      userRead =
        userAdapter.GetUserByName(user.Name);
      Debug.Assert(
       userRead != null &&
       userRead.Name == user.Name &&
       userRead.Password == user.Password);

      ICollection<User> users = userAdapter.GetAllUsers();
      int count = users.Count;

      userAdapter.DeleteUser(user);

      users = userAdapter.GetAllUsers();
      Debug.Assert(count - 1 == users.Count);
    }

    private void TestUserGroupAdapter() {
      IUserGroupAdapter userGroupAdapter =
       ServiceLocator.GetUserGroupAdapter();

      User user =
        new User();
      user.Name = "Stefan";

      User user2 =
        new User();
      user2.Name = "Martin";

      UserGroup group =
        new UserGroup();

      UserGroup subGroup =
        new UserGroup();
      subGroup.Members.Add(user);

      group.Members.Add(user2);
      group.Members.Add(subGroup);

      userGroupAdapter.UpdateUserGroup(group);

      UserGroup read =
        userGroupAdapter.GetUserGroupById(group.PermissionOwnerId);

      ICollection<UserGroup> userGroups =
        userGroupAdapter.GetAllUserGroups();

      userGroupAdapter.DeleteUserGroup(subGroup);

      userGroups =
        userGroupAdapter.GetAllUserGroups();

      read =
        userGroupAdapter.GetUserGroupById(group.PermissionOwnerId);

      userGroupAdapter.DeleteUserGroup(group);

      userGroups =
        userGroupAdapter.GetAllUserGroups();

      IUserAdapter userAdapter =
        ServiceLocator.GetUserAdapter();

      userAdapter.DeleteUser(user);
      userAdapter.DeleteUser(user2);
    }

    public override void Run() {
      TestClientAdapter();
      TestUserAdapter();
      TestUserGroupAdapter();

      ITransactionManager transactionManager =
        ServiceLocator.GetTransactionManager();

      transactionManager.UpdateDB();
    }
  }
}
