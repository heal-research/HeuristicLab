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
      clientAdapter.Update(client);

      ClientInfo clientRead =
        clientAdapter.GetById(client.ClientId);
      Debug.Assert(
        clientRead != null &&
        client.ClientId == clientRead.ClientId);

      client.CpuSpeedPerCore = 2000;
      clientAdapter.Update(client);
      clientRead =
        clientAdapter.GetById(client.ClientId);
      Debug.Assert(
       clientRead != null &&
       client.ClientId == clientRead.ClientId &&
       clientRead.CpuSpeedPerCore == 2000);

      ICollection<ClientInfo> clients = 
        clientAdapter.GetAll();
      int count = clients.Count;

      clientAdapter.Delete(client);

      clients = clientAdapter.GetAll();
      Debug.Assert(count - 1 == clients.Count);
    }

    private void TestUserAdapter() {
      IUserAdapter userAdapter =
        ServiceLocator.GetUserAdapter();

      User user = new User();
      user.Name = "TestDummy";

      userAdapter.Update(user);

      User userRead =
        userAdapter.GetById(user.Id);
      Debug.Assert(
        userRead != null &&
        user.Id == userRead.Id);

      user.Password = "passme";
      userAdapter.Update(user);
      userRead =
        userAdapter.GetByName(user.Name);
      Debug.Assert(
       userRead != null &&
       userRead.Name == user.Name &&
       userRead.Password == user.Password);

      ICollection<User> users = 
       userAdapter.GetAll();
      int count = users.Count;

      userAdapter.Delete(user);

      users = userAdapter.GetAll();
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

      User user3 =
        new User();
      user3.Name = "Heinz";

      UserGroup group =
        new UserGroup();

      UserGroup subGroup =
        new UserGroup();
      subGroup.Members.Add(user);

      group.Members.Add(user3);
      group.Members.Add(user2);
      group.Members.Add(subGroup);

      userGroupAdapter.Update(group);

      UserGroup read =
        userGroupAdapter.GetById(group.Id);

      ICollection<UserGroup> userGroups =
        userGroupAdapter.GetAll();

      IUserAdapter userAdapter =
        ServiceLocator.GetUserAdapter();

      userAdapter.Delete(user3);
      
      read =
         userGroupAdapter.GetById(group.Id);

      userGroupAdapter.Delete(subGroup);

      read =
         userGroupAdapter.GetById(group.Id);

      userGroups =
        userGroupAdapter.GetAll();

      userGroupAdapter.Delete(group);

      userGroups =
        userGroupAdapter.GetAll();

      userAdapter.Delete(user);
      userAdapter.Delete(user2);
    }

    private void TestClientGroupAdapter() {
      IClientGroupAdapter clientGroupAdapter =
       ServiceLocator.GetClientGroupAdapter();

      ClientInfo client =
        new ClientInfo();
      client.Name = "Stefan";
      client.ClientId = Guid.NewGuid();
      client.Login = DateTime.Now;

      ClientInfo client2 =
        new ClientInfo();
      client2.Name = "Martin";
      client2.ClientId = Guid.NewGuid();
      client2.Login = DateTime.Now;

      ClientInfo client3 =
        new ClientInfo();
      client3.Name = "Heinz";
      client3.ClientId = Guid.NewGuid();
      client3.Login = DateTime.Now;

      ClientGroup group =
        new ClientGroup();

      ClientGroup subGroup =
        new ClientGroup();
      subGroup.Resources.Add(client);

      group.Resources.Add(client3);
      group.Resources.Add(client2);
      group.Resources.Add(subGroup);

      clientGroupAdapter.Update(group);

      ClientGroup read =
        clientGroupAdapter.GetById(group.Id);

      ICollection<ClientGroup> clientGroups =
        clientGroupAdapter.GetAll();

      IClientAdapter clientAdapter =
        ServiceLocator.GetClientAdapter();

      clientAdapter.Delete(client3);

      read =
         clientGroupAdapter.GetById(group.Id);

      clientGroupAdapter.Delete(subGroup);

      read =
         clientGroupAdapter.GetById(group.Id);

      clientGroups =
        clientGroupAdapter.GetAll();

      clientGroupAdapter.Delete(group);

      clientGroups =
        clientGroupAdapter.GetAll();

      clientAdapter.Delete(client);
      clientAdapter.Delete(client2);
    }

    private void TestJobAdapter() {
      IJobAdapter jobAdapter = ServiceLocator.GetJobAdapter();

      Job job = new Job();

      ClientInfo client = new ClientInfo();
      client.ClientId = Guid.NewGuid();
      client.Login = DateTime.Now;

      job.Client = client;
      jobAdapter.Update(job);

      ICollection<Job> jobs = jobAdapter.GetAll();

      jobAdapter.Delete(job);

      jobs = jobAdapter.GetAll();
    }

    public override void Run() {
      ITransactionManager transactionManager =
        ServiceLocator.GetTransactionManager();
      
      TestClientAdapter();
      transactionManager.UpdateDB();

      TestUserAdapter();
      transactionManager.UpdateDB();

      TestUserGroupAdapter();
      transactionManager.UpdateDB();

      TestClientGroupAdapter();
      transactionManager.UpdateDB();  

      TestJobAdapter();
      transactionManager.UpdateDB();     
    }
  }
}
