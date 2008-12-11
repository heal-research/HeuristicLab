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
using System.Runtime.CompilerServices;
using System.Data;

namespace HeuristicLab.Hive.Server.ADODataAccess {
  class ClientGroupAdapter : DataAdapterBase, IClientGroupAdapter {
    private dsHiveServerTableAdapters.ClientGroupTableAdapter adapter =
        new dsHiveServerTableAdapters.ClientGroupTableAdapter();

    private dsHiveServer.ClientGroupDataTable data =
      new dsHiveServer.ClientGroupDataTable();

    private dsHiveServerTableAdapters.ClientGroup_ResourceTableAdapter resourceClientGroupAdapter =
      new dsHiveServerTableAdapters.ClientGroup_ResourceTableAdapter();

    private dsHiveServer.ClientGroup_ResourceDataTable resourceClientGroupData =
      new dsHiveServer.ClientGroup_ResourceDataTable();

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

    public ClientGroupAdapter() {
      adapter.Fill(data);
      resourceClientGroupAdapter.Fill(resourceClientGroupData);
    }

    protected override void Update() {
      this.adapter.Update(this.data);
      this.resourceClientGroupAdapter.Update(resourceClientGroupData);
    }

    private ClientGroup Convert(dsHiveServer.ClientGroupRow row,
      ClientGroup clientGroup) {
      if (row != null && clientGroup != null) {
        /*Parent - Permission Owner*/
        clientGroup.ResourceId = row.ResourceId;
        ResAdapter.GetResourceById(clientGroup);

        //first check for created references
        IEnumerable<dsHiveServer.ClientGroup_ResourceRow> clientGroupRows =
          from resource in
            resourceClientGroupData.AsEnumerable<dsHiveServer.ClientGroup_ResourceRow>()
          where resource.ClientGroupResource == clientGroup.ResourceId
          select resource;

        foreach (dsHiveServer.ClientGroup_ResourceRow resourceClientGroupRow in
          clientGroupRows) {
          Resource resource = null;

          IEnumerable<Resource> resources =
            from p in
              clientGroup.Resources
            where p.ResourceId == resourceClientGroupRow.ResourceId
            select p;
          if (resources.Count<Resource>() == 1)
            resource = resources.First<Resource>();

          if (resource == null) {
            Resource res =
              ClientAdapter.GetClientById(resourceClientGroupRow.ResourceId);

            if (res == null) {
              //is a client group
              res =
                GetClientGroupById(resourceClientGroupRow.ResourceId);
            }

            if (res != null)
              clientGroup.Resources.Add(res);
          }
        }

        //secondly check for deleted references
        ICollection<Resource> deleted =
          new List<Resource>();

        foreach (Resource resource in clientGroup.Resources) {
          dsHiveServer.ClientGroup_ResourceRow permOwnerUserGroupRow =
            resourceClientGroupData.FindByClientGroupResourceResourceId(
              clientGroup.ResourceId,
              resource.ResourceId);

          if (permOwnerUserGroupRow == null) {
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

    private dsHiveServer.ClientGroupRow Convert(ClientGroup clientGroup,
      dsHiveServer.ClientGroupRow row) {
      if (clientGroup != null && row != null) {
        row.ResourceId = clientGroup.ResourceId;

        //update references
        foreach (Resource resource in clientGroup.Resources) {
          //first update the member to make sure it exists in the DB
          if (resource is ClientInfo) {
            ClientAdapter.UpdateClient(resource as ClientInfo);
          } else if (resource is ClientGroup) {
            UpdateClientGroup(resource as ClientGroup);
          }

          //secondly check for created references
          dsHiveServer.ClientGroup_ResourceRow resourceClientGroupRow =
            resourceClientGroupData.FindByClientGroupResourceResourceId(
                            clientGroup.ResourceId,
                            resource.ResourceId);

          if (resourceClientGroupRow == null) {
            resourceClientGroupRow =
              resourceClientGroupData.NewClientGroup_ResourceRow();

            resourceClientGroupRow.ResourceId =
              resource.ResourceId;
            resourceClientGroupRow.ClientGroupResource =
              clientGroup.ResourceId;

            resourceClientGroupData.AddClientGroup_ResourceRow(
              resourceClientGroupRow);
          }
        }

        //thirdly check for deleted references
        IEnumerable<dsHiveServer.ClientGroup_ResourceRow> clientGroupRows =
          from permOwner in
            resourceClientGroupData.AsEnumerable<dsHiveServer.ClientGroup_ResourceRow>()
          where permOwner.ClientGroupResource == clientGroup.ResourceId
          select permOwner;

        ICollection<dsHiveServer.ClientGroup_ResourceRow> deleted =
          new List<dsHiveServer.ClientGroup_ResourceRow>();

        foreach (dsHiveServer.ClientGroup_ResourceRow resourceClientGroupRow in
          clientGroupRows) {
          Resource resource = null;

          IEnumerable<Resource> resources =
            from r in
              clientGroup.Resources
            where r.ResourceId == resourceClientGroupRow.ResourceId
            select r;

          if (resources.Count<Resource>() == 1)
            resource = resources.First<Resource>();

          if (resource == null) {
            deleted.Add(resourceClientGroupRow);
          }
        }

        foreach (dsHiveServer.ClientGroup_ResourceRow resoruceClientGroupRow in
          deleted) {
          resourceClientGroupData.RemoveClientGroup_ResourceRow(
            resoruceClientGroupRow);
        }

      }

      return row;
    }

    #region IClientGroupAdapter Members
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void UpdateClientGroup(ClientGroup group) {
      if (group != null) {
        ResAdapter.UpdateResource(group);

        dsHiveServer.ClientGroupRow row =
          data.FindByResourceId(group.ResourceId);

        if (row == null) {
          row = data.NewClientGroupRow();
          row.ResourceId = group.ResourceId;
          data.AddClientGroupRow(row);
        }

        Convert(group, row);
      }
    }

    public ClientGroup GetClientGroupById(long clientGroupId) {
      ClientGroup clientGroup = new ClientGroup();

      dsHiveServer.ClientGroupRow row =
        data.FindByResourceId(clientGroupId);

      if (row != null) {
        Convert(row, clientGroup);

        return clientGroup;
      } else {
        return null;
      }
    }

    public ClientGroup GetClientGroupByName(string name) {
      ClientGroup group = new ClientGroup();

      Resource res =
        ResAdapter.GetResourceByName(name);

      if (res != null) {
        dsHiveServer.ClientGroupRow row =
          data.FindByResourceId(res.ResourceId);

        if (row != null) {
          Convert(row, group);

          return group;
        }
      }

      return null;
    }

    public ICollection<ClientGroup> GetAllClientGroups() {
      ICollection<ClientGroup> allClientGroups =
        new List<ClientGroup>();

      foreach (dsHiveServer.ClientGroupRow row in data) {
        ClientGroup clientGroup = new ClientGroup();

        Convert(row, clientGroup);
        allClientGroups.Add(clientGroup);
      }

      return allClientGroups;
    }

    public ICollection<ClientGroup> MemberOf(Resource resource) {
      ICollection<ClientGroup> clientGroups =
        new List<ClientGroup>();

      if (resource != null) {
        IEnumerable<dsHiveServer.ClientGroup_ResourceRow> clientGroupRows =
         from clientGroup in
           resourceClientGroupData.AsEnumerable<dsHiveServer.ClientGroup_ResourceRow>()
         where clientGroup.ResourceId == resource.ResourceId
         select clientGroup;

        foreach (dsHiveServer.ClientGroup_ResourceRow clientGroupRow in
          clientGroupRows) {
          ClientGroup clientGroup =
            GetClientGroupById(clientGroupRow.ClientGroupResource);
          clientGroups.Add(clientGroup);
        }
      }

      return clientGroups;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool DeleteClientGroup(ClientGroup group) {
      if (group != null) {
        dsHiveServer.ClientGroupRow row =
          data.FindByResourceId(group.ResourceId);

        if (row != null) {
          ICollection<dsHiveServer.ClientGroup_ResourceRow> deleted =
            new List<dsHiveServer.ClientGroup_ResourceRow>();

          foreach (dsHiveServer.ClientGroup_ResourceRow resourceClientGroupRow in
            resourceClientGroupData) {
            if (resourceClientGroupRow.ClientGroupResource == group.ResourceId ||
              resourceClientGroupRow.ResourceId == group.ResourceId) {
              deleted.Add(resourceClientGroupRow);
            }
          }

          foreach (dsHiveServer.ClientGroup_ResourceRow resourceClientGroupRow in
            deleted) {
            resourceClientGroupData.RemoveClientGroup_ResourceRow(
              resourceClientGroupRow);
          }

          data.RemoveClientGroupRow(row);
          return ResAdapter.DeleteResource(group);
        }
      }

      return false;
    }

    #endregion
  }
}
