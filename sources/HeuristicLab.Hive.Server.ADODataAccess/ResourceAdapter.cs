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
  class ResourceAdapter: IResourceAdapter {
    #region IResourceAdapter Members
    private dsHiveServerTableAdapters.ResourceTableAdapter adapter =
      new dsHiveServerTableAdapters.ResourceTableAdapter();

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

    public void UpdateResource(Resource resource) {
      if (resource != null) {
        dsHiveServer.ResourceDataTable data =
          adapter.GetDataById(resource.ResourceId);

        dsHiveServer.ResourceRow row;
        if (data.Count == 0) {
          row = data.NewResourceRow();
          data.AddResourceRow(row);
        } else {
          row = data[0];
        }

        Convert(resource, row);

        adapter.Update(data);

        resource.ResourceId = row.ResourceId;
      }
    }

    internal bool FillResource(Resource resource) {
      if (resource != null) {
        dsHiveServer.ResourceDataTable data =
          adapter.GetDataById(resource.ResourceId);
        if (data.Count == 1) {
          dsHiveServer.ResourceRow row =
            data[0];
          Convert(row, resource);

          return true;
        }
      }

      return false;
    }

    public Resource GetResourceById(long resourceId) {
      Resource resource = new Resource();
      resource.ResourceId = resourceId;

      if(FillResource(resource)) 
        return resource;
      else 
        return null;
    }

    public ICollection<Resource> GetAllResources() {
      ICollection<Resource> allResources =
        new List<Resource>();

      dsHiveServer.ResourceDataTable data =
          adapter.GetData();

      foreach (dsHiveServer.ResourceRow row in data) {
        Resource resource = new Resource();
        Convert(row, resource);
        allResources.Add(resource);
      }

      return allResources;
    }

    public bool DeleteResource(Resource resource) {
      if(resource != null) {

        dsHiveServer.ResourceDataTable data =
           adapter.GetDataById(resource.ResourceId);

        if (data.Count == 1) {
          dsHiveServer.ResourceRow row = data[0];

          row.Delete();
          return adapter.Update(data) > 0;
        } 
      }
       
      return false;
    }

    #endregion
  }
}
