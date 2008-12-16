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
  class ClientAdapter: 
    CachedDataAdapter<
      dsHiveServerTableAdapters.ClientTableAdapter, 
      ClientInfo, 
      dsHiveServer.ClientRow, 
      dsHiveServer.ClientDataTable>,
    IClientAdapter {
    #region Fields
    dsHiveServer.ClientDataTable data =
        new dsHiveServer.ClientDataTable();

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

    public ClientAdapter() {
      parentAdapters.Add(this.ResAdapter as ICachedDataAdapter);
    }

    #region Overrides
    protected override ClientInfo Convert(dsHiveServer.ClientRow row, 
      ClientInfo client) {
      if(row != null && client != null) {      
        /*Parent - resource*/
        client.Id = row.ResourceId;
        ResAdapter.GetById(client);

        /*ClientInfo*/
        if (!row.IsGUIDNull())
          client.ClientId = row.GUID;
        else
          client.ClientId = Guid.Empty;
       
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

    protected override dsHiveServer.ClientRow Convert(ClientInfo client,
      dsHiveServer.ClientRow row) {
      if (client != null && row != null) {      
        row.GUID = client.ClientId;
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
      adapter.Update(row);
    }

    protected override dsHiveServer.ClientRow
      InsertNewRow(ClientInfo client) {
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
      cache = adapter.GetDataByActive();
    }

    public override void SyncWithDb() {
      adapter.Update(cache);
    }

    protected override bool PutInCache(ClientInfo obj) {
      return (obj.State != State.offline && obj.State != State.nullState);
    }

    protected override IEnumerable<dsHiveServer.ClientRow>
      FindById(long id) {
      return adapter.GetDataByResourceId(id);
    }

    protected override dsHiveServer.ClientRow
      FindCachedById(long id) {
      return cache.FindByResourceId(id);
    }

    protected override IEnumerable<dsHiveServer.ClientRow>
      FindAll() {
      return FindMultipleRows(
        new Selector(adapter.GetData),
        new Selector(cache.AsEnumerable<dsHiveServer.ClientRow>));
    }
    #endregion

    #region IClientAdapter Members
    [MethodImpl(MethodImplOptions.Synchronized)]
    public override void Update(ClientInfo client) {
      if (client != null) {
        ResAdapter.Update(client);

        base.Update(client);
      }
    }

    public ClientInfo GetById(Guid clientId) {
      return base.FindSingle(
        delegate() {
          return adapter.GetDataById(clientId);
        }, 
        delegate() {
          return from c in
                     cache.AsEnumerable<dsHiveServer.ClientRow>()
                   where !c.IsGUIDNull() && 
                          c.GUID == clientId
                   select c; 
        });
    }

    public ClientInfo GetByName(string name) {
      ClientInfo client = new ClientInfo();
      Resource res =
        ResAdapter.GetByName(name);

      return GetById(res.Id);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public override bool Delete(ClientInfo client) {      
      if (client != null) {
        dsHiveServer.ClientRow row =
          GetRowById(client.Id);

        if (row != null) {
          //Referential integrity with client groups
          ICollection<ClientGroup> clientGroups =
            ClientGroupAdapter.MemberOf(client);
          foreach (ClientGroup group in clientGroups) {
            group.Resources.Remove(client);
            ClientGroupAdapter.Update(group);
          }

          //Referential integrity with jobs
          ICollection<Job> jobs =
            JobAdapter.GetJobsOf(client);
          foreach (Job job in jobs) {
            JobAdapter.Delete(job);
          }

          return base.Delete(client) && 
            ResAdapter.Delete(client);
        }
      }

      return false;
    }

    #endregion
  }
}
