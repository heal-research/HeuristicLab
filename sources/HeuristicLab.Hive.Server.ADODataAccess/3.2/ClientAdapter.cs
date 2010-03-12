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
using HeuristicLab.Hive.Server.DataAccess;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using System.Linq.Expressions;
using HeuristicLab.DataAccess.Interfaces;
using HeuristicLab.DataAccess.ADOHelper;
using HeuristicLab.Hive.Server.ADODataAccess.dsHiveServerTableAdapters;
using System.Data.Common;
using System.Data.SqlClient;
using HeuristicLab.Hive.Server.ADODataAccess.TableAdapterWrapper;

namespace HeuristicLab.Hive.Server.ADODataAccess {
  class ClientAdapter: 
    DataAdapterBase<
      dsHiveServerTableAdapters.ClientTableAdapter, 
      ClientDto, 
      dsHiveServer.ClientRow>,
    IClientAdapter {
    #region Fields
    private IResourceAdapter resAdapter = null;

    private IResourceAdapter ResAdapter {
      get {
        if (resAdapter == null)
          resAdapter =
            this.Session.GetDataAdapter<ResourceDto, IResourceAdapter>();

        return resAdapter;
      }
    }

    private IClientGroupAdapter clientGroupAdapter = null;

    private IClientGroupAdapter ClientGroupAdapter {
      get {
        if (clientGroupAdapter == null) {
          clientGroupAdapter =
            this.Session.GetDataAdapter<ClientGroupDto, IClientGroupAdapter>();
        }

        return clientGroupAdapter;
      }
    }

    private IJobAdapter jobAdapter = null;

    private IJobAdapter JobAdapter {
      get {
        if (jobAdapter == null) {
          this.Session.GetDataAdapter<JobDto, IJobAdapter>();
        }

        return jobAdapter;
      }
    }

    private IClientConfigAdapter clientConfigAdapter = null;

    private IClientConfigAdapter ClientConfigAdapter {
      get {
        if (clientConfigAdapter == null) {
          clientConfigAdapter =
            this.Session.GetDataAdapter<ClientConfigDto, IClientConfigAdapter>();
        }

        return clientConfigAdapter;
      }
    }
    #endregion

    public ClientAdapter(): 
      base(new ClientAdapterWrapper()) {
    }

    #region Overrides
    protected override ClientDto ConvertRow(dsHiveServer.ClientRow row, 
      ClientDto client) {
      if(row != null && client != null) {      
        /*Parent - resource*/
        client.Id = row.ResourceId;
        ResAdapter.GetById(client);

        /*ClientInfo*/       
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
          client.State = State.nullState;

        if (!row.IsNumberOfCoresNull())
          client.NrOfCores = row.NumberOfCores;
        else
          client.NrOfCores = 0;

        if (!row.IsNumberOfFreeCoresNull())
          client.NrOfFreeCores = row.NumberOfFreeCores;
        else
          client.NrOfFreeCores = 0;

        if (!row.IsFreeMemoryNull())
          client.FreeMemory = row.FreeMemory;
        else
          client.FreeMemory = 0;

        if (!row.IsClientConfigIdNull())
          client.Config = ClientConfigAdapter.GetById(row.ClientConfigId);
        else
          client.Config = null;

        return client;
      }
      else
        return null;
    }

    protected override dsHiveServer.ClientRow ConvertObj(ClientDto client,
      dsHiveServer.ClientRow row) {
      if (client != null && row != null) {
        row.ResourceId = client.Id;
        row.CPUSpeed = client.CpuSpeedPerCore;
        row.Memory = client.Memory;
        row.Login = client.Login;
        if (client.State != State.nullState)
          row.Status = client.State.ToString();
        else
          row.SetStatusNull();
        row.NumberOfCores = client.NrOfCores;
        row.NumberOfFreeCores = client.NrOfFreeCores;
        row.FreeMemory = client.FreeMemory;

        if (client.Config != null)
          row.ClientConfigId = client.Config.Id;
        else
          row.SetClientConfigIdNull();
      }

      return row;
    }

    #endregion

    #region IClientAdapter Members
    protected override void doUpdate(ClientDto client) {
      if (client != null) {
        ResAdapter.Update(client);
        ClientConfigAdapter.Update(client.Config);

        base.doUpdate(client);
      }
    }

    public ClientDto GetByName(string name) {
      return (ClientDto)
        base.doInTransaction(
        delegate() {
          ClientDto client = new ClientDto();
          ResourceDto res =
            ResAdapter.GetByName(name);

          return GetById(res.Id);
        });
    }

    protected override bool doDelete(ClientDto client) {
      return (bool)base.doInTransaction(
        delegate() {
          bool success = false;

          if (client != null) {
            dsHiveServer.ClientRow row =
              GetRowById(client.Id);

            if (row != null) {
              success = base.doDelete(client) &&
                ResAdapter.Delete(client);
            }
          }

          return success;
        });
    }

    public ICollection<ClientDto> GetGrouplessClients() {
      return
        base.FindMultiple(
          delegate() {
            return Adapter.GetDataByGroupless();
          });
    }

    #endregion
  }
}
