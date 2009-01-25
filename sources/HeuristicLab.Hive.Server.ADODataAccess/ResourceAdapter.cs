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
  class ResourceAdapter: 
    CachedDataAdapter<
      dsHiveServerTableAdapters.ResourceTableAdapter, 
      Resource, 
      dsHiveServer.ResourceRow, 
      dsHiveServer.ResourceDataTable>,  
    IResourceAdapter {
    #region Fields
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
        row.Name = resource.Name;

        return row;
      } else
        return null;
    }

    protected override void UpdateRow(dsHiveServer.ResourceRow row) {
      Adapter.Update(row);
    }

    protected override dsHiveServer.ResourceRow
      InsertNewRow(Resource resource) {
      dsHiveServer.ResourceDataTable data =
        new dsHiveServer.ResourceDataTable();

      dsHiveServer.ResourceRow row = data.NewResourceRow();
      data.AddResourceRow(row);

      return row;
    }

    protected override dsHiveServer.ResourceRow
      InsertNewRowInCache(Resource resource) {
      dsHiveServer.ResourceRow row = cache.NewResourceRow();
      cache.AddResourceRow(row);

      return row;
    }

    protected override void FillCache() {
      Adapter.FillByActive(cache);
    }

    protected override void SynchronizeWithDb() {
      Adapter.Update(cache);
    }

    protected override bool PutInCache(Resource obj) {
      return (obj is ClientInfo &&
        (obj as ClientInfo).State != State.offline);
    }

    protected override IEnumerable<dsHiveServer.ResourceRow>
      FindById(long id) {
      return Adapter.GetDataById(id);
    }

    protected override dsHiveServer.ResourceRow
      FindCachedById(long id) {
      return cache.FindByResourceId(id);
    }

    protected override IEnumerable<dsHiveServer.ResourceRow>
      FindAll() {
      return FindMultipleRows(
        new Selector(Adapter.GetData),
        new Selector(cache.AsEnumerable<dsHiveServer.ResourceRow>));
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
      dsHiveServer.ResourceRow row =
        base.FindSingleRow(
          delegate() {
            return Adapter.GetDataByName(name);
          },
          delegate() {
            return from r in
                     cache.AsEnumerable<dsHiveServer.ResourceRow>()
                   where !r.IsNameNull() && 
                          r.Name == name
                   select r;
          });

      if (row != null) {
        Resource res = new Resource();
        res = Convert(row, res);

        return res;
      } else {
        return null;
      }
    }

    #endregion
  }
}
