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
using HeuristicLab.Hive.Server.Core.InternalInterfaces.DataAccess;
using HeuristicLab.Hive.Contracts.BusinessObjects;

namespace HeuristicLab.Hive.Server.ADODataAccess {
  class ClientAdapter: IClientAdapter {
    #region IClientAdapter Members
    private dsHiveServerTableAdapters.ClientTableAdapter adapter =
        new dsHiveServerTableAdapters.ClientTableAdapter();

    private ResourceAdapter resAdapter = 
      new ResourceAdapter();

    public void UpdateClient(ClientInfo client) {
      if (client != null) {
        resAdapter.UpdateResource(client);

        dsHiveServer.ClientDataTable data =
          adapter.GetDataById(client.ClientId);

        dsHiveServer.ClientRow row;
        if (data.Count == 0) {
          row = data.NewClientRow();
          row.ResourceId = client.ResourceId;
          data.AddClientRow(row);
        } else {
          row = data[0];
        }

        row.ResourceId = client.ResourceId;
        row.GUID = client.ClientId;
        row.CPUSpeed = client.CpuSpeedPerCore;
        //row.Memory = client.Memory;
        /*if(client.Login != null)
          row.Login = client.Login.ToString();*/
        row.Status = client.State.ToString();
        
        //todo: config adapter
        /*if (client.Config != null)
          row.ClientConfigId = client.Config.ClientConfigId;*/
        
        row.NumberOfCores = client.NrOfCores;

        adapter.Update(data);
      }
    }

    private ClientInfo Convert(dsHiveServer.ClientRow row) {
      ClientInfo client = new ClientInfo();

      client.ResourceId = row.ResourceId;
      client.ClientId = row.GUID;
      client.CpuSpeedPerCore = row.CPUSpeed;
      //client.Memory = row.Memory;
      //client.Login = row.Login;
      //client.State =;
      //client.Config
      client.NrOfCores = row.NumberOfCores;

      return client;
    }

    public ClientInfo GetClientById(Guid clientId) {
      dsHiveServer.ClientDataTable data =
          adapter.GetDataById(clientId);
      if (data.Count == 1) {
        dsHiveServer.ClientRow row = 
          data[0];
        return Convert(row);
      } else {
        return null;
      }
    }

    public ICollection<ClientInfo> GetAllClients() {
      ICollection<ClientInfo> allClients =
        new List<ClientInfo>();

      dsHiveServer.ClientDataTable data =
          adapter.GetData();

      foreach (dsHiveServer.ClientRow row in data) {
        allClients.Add(Convert(row));
      }

      return allClients;
    }

    public bool DeleteClient(ClientInfo client) {
      //referential integrity will delete the client object
      return resAdapter.DeleteResource(client);
    }

    #endregion
  }
}
