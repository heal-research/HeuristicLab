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
using HeuristicLab.DataAccess.ADOHelper;
using HeuristicLab.Hive.Server.ADODataAccess.dsHiveServerTableAdapters;
using System.Data.Common;
using System.Data.SqlClient;
using HeuristicLab.Hive.Server.ADODataAccess.TableAdapterWrapper;

namespace HeuristicLab.Hive.Server.ADODataAccess {
  class ResourceAdapter: 
    DataAdapterBase<
      dsHiveServerTableAdapters.ResourceTableAdapter, 
      Resource, 
      dsHiveServer.ResourceRow>,  
    IResourceAdapter {
    #region Fields
    private IClientAdapter clientAdapter = null;

    private IClientAdapter ClientAdapter {
      get {
        if (clientAdapter == null)
          clientAdapter =
            this.Session.GetDataAdapter<ClientInfo, IClientAdapter>();
        
        return clientAdapter;
      }
    }

    private IClientGroupAdapter clientGroupAdapter = null;

    private IClientGroupAdapter ClientGroupAdapter {
      get {
        if (clientGroupAdapter == null)
          clientGroupAdapter =
            this.Session.GetDataAdapter<ClientGroup, IClientGroupAdapter>();

        return clientGroupAdapter;
      }
    }
    #endregion

    public ResourceAdapter(): base(new ResourceAdapterWrapper()) {
    }

    #region Overrides
    protected override Resource ConvertRow(dsHiveServer.ResourceRow row,
      Resource resource) {
      if (row != null && resource != null) {
        resource.Id = row.ResourceId;
        if (!row.IsNameNull())
          resource.Name = row.Name;
        else
          resource.Name = String.Empty;

        return resource;
      } else
        return null;
    }

    protected override dsHiveServer.ResourceRow ConvertObj(Resource resource,
      dsHiveServer.ResourceRow row) {
      if (resource != null && row != null) {
        row.ResourceId = resource.Id;
        row.Name = resource.Name;

        return row;
      } else
        return null;
    }
    #endregion

    #region IResourceAdapter Members
    public bool GetById(Resource resource) {
      if (resource != null) {
        dsHiveServer.ResourceRow row =
          GetRowById(resource.Id);

        if (row != null) {
          Convert(row, resource);

          return true;
        }
      }

      return false;
    }

    public Resource GetByName(string name) {
      return
        base.FindSingle (
          delegate() {
            return Adapter.GetDataByName(name);
          });
    }

    #endregion

    #region IPolymorphicDataAdapter<Resource> Members

    public void UpdatePolymorphic(Resource res) {
      if (res is ClientInfo) {
        ClientAdapter.Update(res as ClientInfo);
      } else if (res is ClientGroup) {
        ClientGroupAdapter.Update(res as ClientGroup);
      } else {
        this.Update(res);
      }
    }

    public Resource GetByIdPolymorphic(Guid id) {
      ClientGroup group =
        ClientGroupAdapter.GetById(id);

      if (group != null)
        return group;
      else {
        ClientInfo client =
          ClientAdapter.GetById(id);

        if (client != null)
          return client;
        else {
          return this.GetById(id);
        }
      }
    }

    public bool DeletePolymorphic(Resource res) {
      if (res is ClientInfo) {
        return ClientAdapter.Delete(res as ClientInfo);
      } else if (res is ClientGroup) {
        return ClientGroupAdapter.Delete(res as ClientGroup);
      } else {
        return this.Delete(res);
      }
    }

    #endregion
  }
}
