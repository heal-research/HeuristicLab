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

namespace HeuristicLab.Hive.Server.ADODataAccess {
  class ClientAdapter: 
    CachedDataAdapter<
      dsHiveServerTableAdapters.ClientTableAdapter, 
      ClientInfo, 
      dsHiveServer.ClientRow, 
      dsHiveServer.ClientDataTable>,
    IClientAdapter {
    #region Fields
    private IResourceAdapter resAdapter = null;

    private IResourceAdapter ResAdapter {
      get {
        if (resAdapter == null)
          resAdapter = ServiceLocator.GetResourceAdapter();

        return resAdapter;
      }
    }

    private IClientGroupAdapter clientGroupAdapter = null;

    private IClientGroupAdapter ClientGroupAdapter {
      get {
        if (clientGroupAdapter == null) {
          clientGroupAdapter = ServiceLocator.GetClientGroupAdapter();
        }

        return clientGroupAdapter;
      }
    }

    private IJobAdapter jobAdapter = null;

    private IJobAdapter JobAdapter {
      get {
        if (jobAdapter == null) {
          jobAdapter = ServiceLocator.GetJobAdapter();
        }

        return jobAdapter;
      }
    }
    #endregion

    public ClientAdapter():
      base(ServiceLocator.GetDBSynchronizer()) {
      parentAdapters.Add(this.ResAdapter as ICachedDataAdapter);
    }


    #region Overrides
    protected override ClientInfo ConvertRow(dsHiveServer.ClientRow row, 
      ClientInfo client) {
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

        //todo: config adapter (client.config)

        return client;
      }
      else
        return null;
    }

    protected override dsHiveServer.ClientRow ConvertObj(ClientInfo client,
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

        //todo: config adapter
        /*if (client.Config != null)
          row.ClientConfigId = client.Config.ClientConfigId;
         else
          row.ClientConfigId = null;*/
      }

      return row;
    }

    protected override void UpdateRow(dsHiveServer.ClientRow row) {
      Adapter.Update(row);
    }

    protected override dsHiveServer.ClientRow
      InsertNewRow(ClientInfo client) {
      dsHiveServer.ClientDataTable data =
        new dsHiveServer.ClientDataTable();

      dsHiveServer.ClientRow row = data.NewClientRow();
      row.ResourceId = client.Id;
      data.AddClientRow(row);

      return row;
    }

    protected override dsHiveServer.ClientRow
      InsertNewRowInCache(ClientInfo client) {
      dsHiveServer.ClientRow row = cache.NewClientRow();
      row.ResourceId = client.Id;
      cache.AddClientRow(row);

      return row;
    }

    protected override void FillCache() {
      Adapter.FillByActive(cache);
    }

    protected override void SynchronizeWithDb() {
      Adapter.Update(cache);
    }

    protected override bool PutInCache(ClientInfo obj) {
      return (obj.State != State.offline && obj.State != State.nullState);
    }

    protected override IEnumerable<dsHiveServer.ClientRow>
      FindById(Guid id) {
      return Adapter.GetDataById(id);
    }

    protected override dsHiveServer.ClientRow
      FindCachedById(Guid id) {
      return cache.FindByResourceId(id);
    }

    protected override IEnumerable<dsHiveServer.ClientRow>
      FindAll() {
      return FindMultipleRows(
        new Selector(Adapter.GetData),
        new Selector(cache.AsEnumerable<dsHiveServer.ClientRow>));
    }

    #endregion

    #region IClientAdapter Members
    public override void Update(ClientInfo client) {
      if (client != null) {
        ResAdapter.Update(client);

        base.Update(client);
      }
    }

    public ClientInfo GetByName(string name) {
      ClientInfo client = new ClientInfo();
      Resource res =
        ResAdapter.GetByName(name);

      return GetById(res.Id);
    }

    public override bool Delete(ClientInfo client) {
      bool success = false;
      Guid locked = Guid.Empty;
      
      if (client != null) {
        if (client.Id != Guid.Empty) {
          LockRow(client.Id);
          locked = client.Id;
        }

        dsHiveServer.ClientRow row =
          GetRowById(client.Id);

        if (row != null) {
          //Referential integrity with jobs - they are cached
          ICollection<Job> jobs =
            JobAdapter.GetJobsOf(client);
          foreach (Job job in jobs) {
            JobAdapter.Delete(job);
          }

          success = base.Delete(client) && 
            ResAdapter.Delete(client);
        }
      }

      if (locked != Guid.Empty) {
        UnlockRow(locked);
      }

      return success;
    }

    #endregion
  }
}
