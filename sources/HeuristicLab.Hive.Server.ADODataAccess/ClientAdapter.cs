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
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace HeuristicLab.Hive.Server.ADODataAccess {
  class ClientAdapter: DataAdapterBase, IClientAdapter {
    private dsHiveServerTableAdapters.ClientTableAdapter adapter =
        new dsHiveServerTableAdapters.ClientTableAdapter();

    private dsHiveServer.ClientDataTable data =
      new dsHiveServer.ClientDataTable();

    private IResourceAdapter resAdapter =
      ServiceLocator.GetResourceAdapter();

    public ClientAdapter() {
      adapter.Fill(data);
    }

    protected override void Update() {
      this.adapter.Update(this.data);
    }
    
    private ClientInfo Convert(dsHiveServer.ClientRow row, 
      ClientInfo client) {
      if(row != null && client != null) {      
        /*Parent - resource*/
        client.ResourceId = row.ResourceId;
        resAdapter.GetResourceById(client);

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

    #region IClientAdapter Members
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void UpdateClient(ClientInfo client) {
      if (client != null) {
        resAdapter.UpdateResource(client);

        dsHiveServer.ClientRow row = 
          data.FindByResourceId(client.ResourceId);

        if (row == null) {
          row = data.NewClientRow();
          row.ResourceId = client.ResourceId;
          data.AddClientRow(row);
        } 

        Convert(client, row);
      }
    }

    public ClientInfo GetClientById(Guid clientId) {
      ClientInfo client = new ClientInfo();

      dsHiveServer.ClientRow row =
        data.Single<dsHiveServer.ClientRow>(
          r => !r.IsGUIDNull() && r.GUID == clientId);

      if (row != null) {
        Convert(row, client);

        return client;
      } else {
        return null;
      }
    }

    public ICollection<ClientInfo> GetAllClients() {
      ICollection<ClientInfo> allClients =
        new List<ClientInfo>();

      foreach (dsHiveServer.ClientRow row in data) {
        ClientInfo client = new ClientInfo();
        Convert(row, client);
        allClients.Add(client);
      }

      return allClients;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool DeleteClient(ClientInfo client) {
      if (client != null) {
        dsHiveServer.ClientRow row =
          data.Single<dsHiveServer.ClientRow>(
            r => r.GUID == client.ClientId);

        if (row != null) {
          data.RemoveClientRow(row);

          return resAdapter.DeleteResource(client);
        }
      }

      return false;
    }

    #endregion
  }
}
