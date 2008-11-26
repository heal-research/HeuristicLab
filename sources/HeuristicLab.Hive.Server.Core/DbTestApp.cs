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
    public override void Run() {
      IClientAdapter clientAdapter =
        ServiceLocator.GetClientAdapter();

      ClientInfo client = new ClientInfo();
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
  }
}
