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
  class PermissionOwnerAdapter: DataAdapterBase, IPermissionOwnerAdapter {
    private dsHiveServerTableAdapters.PermissionOwnerTableAdapter adapter =
     new dsHiveServerTableAdapters.PermissionOwnerTableAdapter();

    private dsHiveServer.PermissionOwnerDataTable data =
      new dsHiveServer.PermissionOwnerDataTable();

    public PermissionOwnerAdapter() {
      adapter.Fill(data);
    }

    protected override void Update() {
      this.adapter.Update(this.data);
    }

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
        row.Name = permOwner.Name;

        return row;
      } else
        return null;
    }
    
    #region IPermissionOwner Members
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void UpdatePermissionOwner(PermissionOwner permOwner) {
      if (permOwner != null) {
        dsHiveServer.PermissionOwnerRow row = 
          data.FindByPermissionOwnerId(permOwner.PermissionOwnerId);
        
        if (row == null) {
          row = data.NewPermissionOwnerRow();
          data.AddPermissionOwnerRow(row);

          //write row to db to get primary key
          adapter.Update(row);
        }

        Convert(permOwner, row);
        permOwner.PermissionOwnerId = row.PermissionOwnerId;
      }
    }

    public bool GetPermissionOwnerById(PermissionOwner permOwner) {
      if (permOwner != null) {
          dsHiveServer.PermissionOwnerRow row =
            data.FindByPermissionOwnerId(permOwner.PermissionOwnerId);

        if(row != null) {
          Convert(row, permOwner);

          return true;
        }
      }

      return false;
    }

    public PermissionOwner GetPermissionOwnerById(long permOwnerId) {
      PermissionOwner permOwner = new PermissionOwner();
      permOwner.PermissionOwnerId = permOwnerId;

      if (GetPermissionOwnerById(permOwner))
        return permOwner;
      else
        return null;
    }

    public PermissionOwner GetPermissionOwnerByName(String name) {
       dsHiveServer.PermissionOwnerRow row = null;

      IEnumerable<dsHiveServer.PermissionOwnerRow> permOwners =
        from r in
          data.AsEnumerable<dsHiveServer.PermissionOwnerRow>()
        where !r.IsNameNull() && r.Name == name
        select r;

      if (permOwners.Count<dsHiveServer.PermissionOwnerRow>() == 1)
        row = permOwners.First<dsHiveServer.PermissionOwnerRow>();

      if (row != null) {
        PermissionOwner permOwner = new PermissionOwner();
        Convert(row, permOwner);

        return permOwner;
      } else {
        return null;
      }
    }

    public ICollection<PermissionOwner> GetAllPermissionOwners() {
      ICollection<PermissionOwner> allPermissionOwners =
        new List<PermissionOwner>();

      foreach (dsHiveServer.PermissionOwnerRow row in data) {
        PermissionOwner permOwner = new PermissionOwner();
        Convert(row, permOwner);
        allPermissionOwners.Add(permOwner);
      }

      return allPermissionOwners;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool DeletePermissionOwner(PermissionOwner permOwner) {
      if (permOwner != null) {
          dsHiveServer.PermissionOwnerRow row = 
            data.FindByPermissionOwnerId(permOwner.PermissionOwnerId);

          if(row != null) {
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
