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
  class PermissionOwnerAdapter: IPermissionOwner {
    #region IPermissionOwner Members
    private dsHiveServerTableAdapters.PermissionOwnerTableAdapter adapter =
     new dsHiveServerTableAdapters.PermissionOwnerTableAdapter();

    private PermissionOwner Convert(dsHiveServer.PermissionOwnerRow row, 
      PermissionOwner permOwner) {
      if (row != null && permOwner != null) {
        permOwner.PermissionOwnerId = row.PermissionOwnerId;

        if (!row.IsNameNull())
          permOwner.Name = row.Name;
        else
          permOwner.Name = String.Empty;

        return permOwner;
      } else
        return null;
    }

    private dsHiveServer.PermissionOwnerRow Convert(PermissionOwner permOwner,
      dsHiveServer.PermissionOwnerRow row) {
      if (row != null && permOwner != null) {
        row.PermissionOwnerId = permOwner.PermissionOwnerId;
        row.Name = permOwner.Name;

        return row;
      } else
        return null;
    }

    public void UpdatePermissionOwner(PermissionOwner permOwner) {
      if (permOwner != null) {
        dsHiveServer.PermissionOwnerDataTable data =
          adapter.GetDataById(permOwner.PermissionOwnerId);

        dsHiveServer.PermissionOwnerRow row;
        if (data.Count == 0) {
          row = data.NewPermissionOwnerRow();
          data.AddPermissionOwnerRow(row);
        } else {
          row = data[0];
        }

        row.Name = permOwner.Name;

        adapter.Update(data);

        permOwner.PermissionOwnerId = row.PermissionOwnerId;
      }
    }

    internal bool FillPermissionOwner(PermissionOwner permOwner) {
      if (permOwner != null) {
        dsHiveServer.PermissionOwnerDataTable data =
          adapter.GetDataById(permOwner.PermissionOwnerId);
        if (data.Count == 1) {
          dsHiveServer.PermissionOwnerRow row =
            data[0];
          Convert(row, permOwner);

          return true;
        }
      }

      return false;
    }

    public PermissionOwner GetPermissionOwnerById(long permOwnerId) {
      PermissionOwner permOwner = new PermissionOwner();
      permOwner.PermissionOwnerId = permOwnerId;

      if (FillPermissionOwner(permOwner))
        return permOwner;
      else
        return null;
    }

    public ICollection<PermissionOwner> GetAllPermissionOwners() {
      ICollection<PermissionOwner> allPermissionOwners =
        new List<PermissionOwner>();

      dsHiveServer.PermissionOwnerDataTable data =
          adapter.GetData();

      foreach (dsHiveServer.PermissionOwnerRow row in data) {
        PermissionOwner permOwner = new PermissionOwner();
        Convert(row, permOwner);
        allPermissionOwners.Add(permOwner);
      }

      return allPermissionOwners;
    }

    public bool DeletePermissionOwner(PermissionOwner permOwner) {
      if (permOwner != null) {
        dsHiveServer.PermissionOwnerDataTable data =
           adapter.GetDataById(permOwner.PermissionOwnerId);

        if (data.Count == 1) {
          dsHiveServer.PermissionOwnerRow row = data[0];

          row.Delete();
          return adapter.Update(data) > 0;
        }
      }

      return false;
    }

    #endregion
  }
}
