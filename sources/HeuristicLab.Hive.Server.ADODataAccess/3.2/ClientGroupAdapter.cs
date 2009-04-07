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
using System.Data;
using HeuristicLab.DataAccess.ADOHelper;
using HeuristicLab.Hive.Server.ADODataAccess.dsHiveServerTableAdapters;
using System.Data.Common;
using System.Data.SqlClient;

namespace HeuristicLab.Hive.Server.ADODataAccess {
  class ClientGroupAdapterWrapper :
    DataAdapterWrapperBase<dsHiveServerTableAdapters.ClientGroupTableAdapter,
    ClientGroup,
    dsHiveServer.ClientGroupRow> {
    public override dsHiveServer.ClientGroupRow
     InsertNewRow(ClientGroup group) {
      dsHiveServer.ClientGroupDataTable data =
         new dsHiveServer.ClientGroupDataTable();

      dsHiveServer.ClientGroupRow row =
        data.NewClientGroupRow();

      row.ResourceId = group.Id;

      data.AddClientGroupRow(row);
      TransactionalAdapter.Update(row);

      return row;
    }

    public override void
      UpdateRow(dsHiveServer.ClientGroupRow row) {
      TransactionalAdapter.Update(row);
    }

    public override IEnumerable<dsHiveServer.ClientGroupRow>
      FindById(Guid id) {
      return TransactionalAdapter.GetDataById(id);
    }

    public override IEnumerable<dsHiveServer.ClientGroupRow>
      FindAll() {
      return TransactionalAdapter.GetData();
    }

    protected override void SetConnection(DbConnection connection) {
      adapter.Connection = connection as SqlConnection;
    }

    protected override void SetTransaction(DbTransaction transaction) {
      adapter.Transaction = transaction as SqlTransaction;
    }
  }

  class ClientGroup_ResourceAdapterWrapper :
  DataAdapterWrapperBase<dsHiveServerTableAdapters.ClientGroup_ResourceTableAdapter,
  BinaryRelation,
  dsHiveServer.ClientGroup_ResourceRow> {
    public override dsHiveServer.ClientGroup_ResourceRow
     InsertNewRow(BinaryRelation relation) {
      dsHiveServer.ClientGroup_ResourceDataTable data =
         new dsHiveServer.ClientGroup_ResourceDataTable();

      dsHiveServer.ClientGroup_ResourceRow row =
        data.NewClientGroup_ResourceRow();

      row.ClientGroupId = relation.Id;
      row.ResourceId = relation.Id2;

      data.AddClientGroup_ResourceRow(row);
      TransactionalAdapter.Update(row);

      return row;
    }

    public override void
      UpdateRow(dsHiveServer.ClientGroup_ResourceRow row) {
      TransactionalAdapter.Update(row);
    }

    public override IEnumerable<dsHiveServer.ClientGroup_ResourceRow>
      FindById(Guid id) {
      return TransactionalAdapter.GetDataByClientGroupId(id);
    }

    public override IEnumerable<dsHiveServer.ClientGroup_ResourceRow>
      FindAll() {
      return TransactionalAdapter.GetData();
    }

    protected override void SetConnection(DbConnection connection) {
      adapter.Connection = connection as SqlConnection;
    }

    protected override void SetTransaction(DbTransaction transaction) {
      adapter.Transaction = transaction as SqlTransaction;
    }
  }
  
  class ClientGroupAdapter : 
    DataAdapterBase<dsHiveServerTableAdapters.ClientGroupTableAdapter, 
    ClientGroup, 
    dsHiveServer.ClientGroupRow>, 
    IClientGroupAdapter {
    #region Fields
    private BinaryRelationHelper<
      dsHiveServerTableAdapters.ClientGroup_ResourceTableAdapter, 
      dsHiveServer.ClientGroup_ResourceRow> binaryRelationHelper = null;

    private BinaryRelationHelper<dsHiveServerTableAdapters.ClientGroup_ResourceTableAdapter, 
      dsHiveServer.ClientGroup_ResourceRow> BinaryRelationHelper {
      get {
        if (binaryRelationHelper == null) {
          binaryRelationHelper =
            new BinaryRelationHelper<dsHiveServerTableAdapters.ClientGroup_ResourceTableAdapter,
              dsHiveServer.ClientGroup_ResourceRow>(new ClientGroup_ResourceAdapterWrapper());
        }

        binaryRelationHelper.Session = Session as Session;

        return binaryRelationHelper;
      }
    }

    private IResourceAdapter resourceAdapter = null;

    private IResourceAdapter ResAdapter {
      get {
        if (resourceAdapter == null)
          resourceAdapter =
            this.Session.GetDataAdapter<Resource, IResourceAdapter>();

        return resourceAdapter;
      }
    }

    private IClientAdapter clientAdapter = null;

    private IClientAdapter ClientAdapter {
      get {
        if (clientAdapter == null)
          clientAdapter =
            this.Session.GetDataAdapter<ClientInfo, IClientAdapter>();

        return clientAdapter;
      }
    }
    #endregion

    public ClientGroupAdapter(): 
      base(new ClientGroupAdapterWrapper()) {
    }

    #region Overrides
    protected override ClientGroup ConvertRow(dsHiveServer.ClientGroupRow row,
      ClientGroup clientGroup) {
      if (row != null && clientGroup != null) {
        /*Parent - Permission Owner*/
        clientGroup.Id = row.ResourceId;
        ResAdapter.GetById(clientGroup.Id);

        ICollection<Guid> resources =
          BinaryRelationHelper.GetRelationships(clientGroup.Id);

       clientGroup.Resources.Clear();
        foreach(Guid resource in resources) {
          ClientInfo client = 
            ClientAdapter.GetById(resource);

          if (client == null) {
            //client group
            ClientGroup group =
              GetById(resource);

            clientGroup.Resources.Add(group);
          } else {
            clientGroup.Resources.Add(client);
          }          
        }

        return clientGroup;
      } else
        return null;
    }

    protected override dsHiveServer.ClientGroupRow ConvertObj(ClientGroup clientGroup,
      dsHiveServer.ClientGroupRow row) {
      if (clientGroup != null && row != null) {
        row.ResourceId = clientGroup.Id;
      }

      return row;
    }
    #endregion

    #region IClientGroupAdapter Members
    protected override void doUpdate(ClientGroup group) {
      if (group != null) {
        ResAdapter.Update(group);

        base.doUpdate(group);

        List<Guid> relationships = 
          new List<Guid>();
        foreach(Resource res in group.Resources) {
          if (res is ClientInfo) {
            ClientAdapter.Update(res as ClientInfo);
          } else if (res is ClientGroup) {
            Update(res as ClientGroup);
          } else {
            ResAdapter.Update(res);
          }

          relationships.Add(res.Id);
        }

        BinaryRelationHelper.UpdateRelationships(group.Id,
          relationships);
      }
    }

    public ClientGroup GetByName(string name) {
      ClientGroup group = new ClientGroup();
      Resource res =
        ResAdapter.GetByName(name);

      if (res != null) {
        return GetById(res.Id);
      }

      return null;
    }

    public ICollection<ClientGroup> MemberOf(Resource resource) {
      throw new NotImplementedException();
    }

    protected override bool doDelete(ClientGroup group) {
      if (group != null) {
        //delete all relationships
        BinaryRelationHelper.UpdateRelationships(group.Id,
          new List<Guid>());

        return base.doDelete(group) && 
          ResAdapter.Delete(group);
      }

      return false;
    }

    #endregion
  }
}
