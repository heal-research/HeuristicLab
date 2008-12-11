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

namespace HeuristicLab.Hive.Server.ADODataAccess {
  class ResourceAdapter: DataAdapterBase, IResourceAdapter {
    private dsHiveServerTableAdapters.ResourceTableAdapter adapter =
        new dsHiveServerTableAdapters.ResourceTableAdapter();

    private dsHiveServer.ResourceDataTable data =
        new dsHiveServer.ResourceDataTable();

    public ResourceAdapter() {
      adapter.Fill(data);
    }

    protected override void Update() {
      this.adapter.Update(this.data);
    }

    private Resource Convert(dsHiveServer.ResourceRow row,
      Resource resource) {
      if (row != null && resource != null) {
        resource.ResourceId = row.ResourceId;
        if (!row.IsNameNull())
          resource.Name = row.Name;
        else
          resource.Name = String.Empty;

        return resource;
      } else
        return null;
    }

    private dsHiveServer.ResourceRow Convert(Resource resource,
      dsHiveServer.ResourceRow row) {
      if (resource != null && row != null) {
        row.Name = resource.Name;

        return row;
      } else
        return null;
    }

    #region IResourceAdapter Members
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void UpdateResource(Resource resource) {
      if (resource != null) {
        dsHiveServer.ResourceRow row =
          data.FindByResourceId(resource.ResourceId);

        if (row == null) {
          row = data.NewResourceRow();
          data.AddResourceRow(row);

          //write row to db to get primary key
          adapter.Update(row);
        } 

        Convert(resource, row);
        resource.ResourceId = row.ResourceId;
      }
    }

    public bool GetResourceById(Resource resource) {
      if (resource != null) {
        dsHiveServer.ResourceRow row =
          data.FindByResourceId(resource.ResourceId);
        if (row != null) {
          Convert(row, resource);

          return true;
        }
      }

      return false;
    }

    public Resource GetResourceById(long resourceId) {
      Resource resource = new Resource();
      resource.ResourceId = resourceId;

      if(GetResourceById(resource)) 
        return resource;
      else 
        return null;
    }

    public Resource GetResourceByName(string name) {
      dsHiveServer.ResourceRow row = null;

      IEnumerable<dsHiveServer.ResourceRow> permOwners =
        from r in
          data.AsEnumerable<dsHiveServer.ResourceRow>()
        where !r.IsNameNull() && r.Name == name
        select r;

      if (permOwners.Count<dsHiveServer.ResourceRow>() == 1)
        row = permOwners.First<dsHiveServer.ResourceRow>();

      if (row != null) {
        Resource res = new Resource();
        Convert(row, res);

        return res;
      } else {
        return null;
      }
    }

    public ICollection<Resource> GetAllResources() {
      IList<Resource> allResources =
        new List<Resource>();
      
      foreach (dsHiveServer.ResourceRow row in data) {
        Resource resource = new Resource();
        Convert(row, resource);
        allResources.Add(resource);
      }

      return allResources;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool DeleteResource(Resource resource) {
      if(resource != null) {
        dsHiveServer.ResourceRow row =
          data.FindByResourceId(resource.ResourceId);

        if (row != null) {
          row.Delete();
          adapter.Update(row);

          return true;
        } 
      }
       
      return false;
    }

    #endregion
  }
}
