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
    private dsHiveServerTableAdapters.ClientTableAdapter adapter =
        new dsHiveServerTableAdapters.ClientTableAdapter();

    private ResourceAdapter resAdapter = 
      new ResourceAdapter();
    
    #region IClientAdapter Members
    private ClientInfo Convert(dsHiveServer.ClientRow row, 
      ClientInfo client) {
      if(row != null && client != null) {      
        /*Parent - resource*/
        client.ResourceId = row.ResourceId;
        resAdapter.FillResource(client);

        /*ClientInfo*/
        client.ClientId = row.GUID;
       
        if (!row.IsCPUSpeedNull())
          client.CpuSpeedPerCore = row.CPUSpeed;
        else
          client.CpuSpeedPerCore = 0;

        if (!row.IsMemoryNull())
          client.Memory = row.Memory;
        else
          client.Memory = 0;

        if (!row.IsLoginNull())
          client.Login = row.Login;
        else
          client.Login = DateTime.MinValue;

        if (!row.IsStatusNull())
          client.State = (State)Enum.Parse(typeof(State), row.Status, true);
        else
          client.State = State.idle;

        if (!row.IsNumberOfCoresNull())
          client.NrOfCores = row.NumberOfCores;
        else
          client.NrOfCores = 0;

        //todo: config adapter (client.config)

        return client;
      }
      else
        return null;
    }

    private dsHiveServer.ClientRow Convert(ClientInfo client,
      dsHiveServer.ClientRow row) {
      if (client != null && row != null) {      
        row.ResourceId = client.ResourceId;
        row.GUID = client.ClientId;
        row.CPUSpeed = client.CpuSpeedPerCore;
        row.Memory = client.Memory;
        row.Login = client.Login;
        row.Status = client.State.ToString();
        row.NumberOfCores = client.NrOfCores;

        //todo: config adapter
        /*if (client.Config != null)
          row.ClientConfigId = client.Config.ClientConfigId;
         else
          row.ClientConfigId = null;*/
      }

      return row;
    }

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

        Convert(client, row);

        adapter.Update(data);
      }
    }

    public ClientInfo GetClientById(Guid clientId) {
      ClientInfo client = new ClientInfo();
      
      dsHiveServer.ClientDataTable data =
          adapter.GetDataById(clientId);
      if (data.Count == 1) {
        dsHiveServer.ClientRow row = 
          data[0];
        Convert(row, client);

        return client;
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
        ClientInfo client = new ClientInfo();
        Convert(row, client);
        allClients.Add(client);
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
