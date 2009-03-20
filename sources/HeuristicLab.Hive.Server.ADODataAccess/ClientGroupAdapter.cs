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

namespace HeuristicLab.Hive.Server.ADODataAccess {
  class ClientGroupAdapter : 
    DataAdapterBase<dsHiveServerTableAdapters.ClientGroupTableAdapter, 
    ClientGroup, 
    dsHiveServer.ClientGroupRow>, 
    IClientGroupAdapter {
    #region Fields
    private dsHiveServerTableAdapters.ClientGroup_ResourceTableAdapter resourceClientGroupAdapter =
      new dsHiveServerTableAdapters.ClientGroup_ResourceTableAdapter();

    private IResourceAdapter resourceAdapter = null;

    private IResourceAdapter ResAdapter {
      get {
        if (resourceAdapter == null)
          resourceAdapter = ServiceLocator.GetResourceAdapter();

        return resourceAdapter;
      }
    }

    private IClientAdapter clientAdapter = null;

    private IClientAdapter ClientAdapter {
      get {
        if (clientAdapter == null)
          clientAdapter = ServiceLocator.GetClientAdapter();

        return clientAdapter;
      }
    }
    #endregion

    #region Overrides
    protected override ClientGroup ConvertRow(dsHiveServer.ClientGroupRow row,
      ClientGroup clientGroup) {
      if (row != null && clientGroup != null) {
        /*Parent - Permission Owner*/
        clientGroup.Id = row.ResourceId;
        ResAdapter.GetById(clientGroup);

        //first check for created references
        dsHiveServer.ClientGroup_ResourceDataTable clientGroupRows =
            resourceClientGroupAdapter.GetDataByClientGroupId(clientGroup.Id);

        foreach (dsHiveServer.ClientGroup_ResourceRow resourceClientGroupRow in
          clientGroupRows) {
          Resource resource = null;

          IEnumerable<Resource> resources =
            from p in
              clientGroup.Resources
            where p.Id == resourceClientGroupRow.ResourceId
            select p;
          if (resources.Count<Resource>() == 1)
            resource = resources.First<Resource>();

          if (resource == null) {
            Resource res =
              ClientAdapter.GetById(resourceClientGroupRow.ResourceId);

            if (res == null) {
              //is a client group
              res =
                GetById(resourceClientGroupRow.ResourceId);
            }

            if (res != null)
              clientGroup.Resources.Add(res);
          }
        }

        //secondly check for deleted references
        ICollection<Resource> deleted =
          new List<Resource>();

        foreach (Resource resource in clientGroup.Resources) {
          dsHiveServer.ClientGroup_ResourceDataTable found =
            resourceClientGroupAdapter.GetDataByClientGroupResourceId(
            clientGroup.Id,
            resource.Id);

          if (found.Count != 1) {
            deleted.Add(resource);
          }
        }

        foreach (Resource resource in deleted) {
          clientGroup.Resources.Remove(resource);
        }

        return clientGroup;
      } else
        return null;
    }

    protected override dsHiveServer.ClientGroupRow ConvertObj(ClientGroup clientGroup,
      dsHiveServer.ClientGroupRow row) {
      if (clientGroup != null && row != null) {
        row.ResourceId = clientGroup.Id;

        //update references
        foreach (Resource resource in clientGroup.Resources) {
          //first update the member to make sure it exists in the DB
          if (resource is ClientInfo) {
            ClientAdapter.Update(resource as ClientInfo);
          } else if (resource is ClientGroup) {
            Update(resource as ClientGroup);
          }

          //secondly check for created references
          dsHiveServer.ClientGroup_ResourceRow resourceClientGroupRow =
            null;
          dsHiveServer.ClientGroup_ResourceDataTable found =
            resourceClientGroupAdapter.GetDataByClientGroupResourceId(
              clientGroup.Id,
              resource.Id);
          if (found.Count == 1)
            resourceClientGroupRow = found[0];

          if (resourceClientGroupRow == null) {
            resourceClientGroupRow =
              found.NewClientGroup_ResourceRow();

            resourceClientGroupRow.ResourceId =
              resource.Id;
            resourceClientGroupRow.ClientGroupId =
              clientGroup.Id;

            found.AddClientGroup_ResourceRow(resourceClientGroupRow);

            resourceClientGroupAdapter.Update(
              resourceClientGroupRow);
          }
        }

        //thirdly check for deleted references
        dsHiveServer.ClientGroup_ResourceDataTable clientGroupRows =
          resourceClientGroupAdapter.GetDataByClientGroupId(clientGroup.Id);

        ICollection<dsHiveServer.ClientGroup_ResourceRow> deleted =
          new List<dsHiveServer.ClientGroup_ResourceRow>();

        foreach (dsHiveServer.ClientGroup_ResourceRow resourceClientGroupRow in
          clientGroupRows) {
          Resource resource = null;

          IEnumerable<Resource> resources =
            from r in
              clientGroup.Resources
            where r.Id == resourceClientGroupRow.ResourceId
            select r;

          if (resources.Count<Resource>() == 1)
            resource = resources.First<Resource>();

          if (resource == null) {
            deleted.Add(resourceClientGroupRow);
          }
        }

        foreach (dsHiveServer.ClientGroup_ResourceRow resourceClientGroupRow in deleted) {
          resourceClientGroupRow.Delete();
          resourceClientGroupAdapter.Update(resourceClientGroupRow);
        }
      }

      return row;
    }

    protected override dsHiveServer.ClientGroupRow
      InsertNewRow(ClientGroup group) {
      dsHiveServer.ClientGroupDataTable data =
         new dsHiveServer.ClientGroupDataTable();

      dsHiveServer.ClientGroupRow row =
        data.NewClientGroupRow();

      row.ResourceId = group.Id;

      data.AddClientGroupRow(row);
      Adapter.Update(row);

      return row;
    }

    protected override void
      UpdateRow(dsHiveServer.ClientGroupRow row) {
      Adapter.Update(row);
    }

    protected override IEnumerable<dsHiveServer.ClientGroupRow>
      FindById(long id) {
      return Adapter.GetDataById(id);
    }

    protected override IEnumerable<dsHiveServer.ClientGroupRow>
      FindAll() {
      return Adapter.GetData();
    }
    #endregion

    #region IClientGroupAdapter Members
    public override void Update(ClientGroup group) {
      if (group != null) {
        ResAdapter.Update(group);

        base.Update(group);
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
      ICollection<ClientGroup> clientGroups =
        new List<ClientGroup>();

      if (resource != null) {
        IEnumerable<dsHiveServer.ClientGroup_ResourceRow> clientGroupRows =
         resourceClientGroupAdapter.GetDataByResourceId(resource.Id);

        foreach (dsHiveServer.ClientGroup_ResourceRow clientGroupRow in
          clientGroupRows) {
          ClientGroup clientGroup =
            GetById(clientGroupRow.ClientGroupId);
          clientGroups.Add(clientGroup);
        }
      }

      return clientGroups;
    }

    public override bool Delete(ClientGroup group) {
      if (group != null) {
        return base.Delete(group) && 
          ResAdapter.Delete(group);
      }

      return false;
    }

    #endregion
  }
}
