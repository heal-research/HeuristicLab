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
  class PermissionOwnerAdapter: 
    DataAdapterBase<
      dsHiveServerTableAdapters.PermissionOwnerTableAdapter, 
      PermissionOwner, 
      dsHiveServer.PermissionOwnerRow>, 
    IPermissionOwnerAdapter {
    #region Overrides
    protected override PermissionOwner ConvertRow(dsHiveServer.PermissionOwnerRow row,
      PermissionOwner permOwner) {
      if (row != null && permOwner != null) {
        permOwner.Id = row.PermissionOwnerId;

        if (!row.IsNameNull())
          permOwner.Name = row.Name;
        else
          permOwner.Name = String.Empty;

        return permOwner;
      } else
        return null;
    }

    protected override dsHiveServer.PermissionOwnerRow ConvertObj(PermissionOwner permOwner,
      dsHiveServer.PermissionOwnerRow row) {
      if (row != null && permOwner != null) {
        row.Name = permOwner.Name;

        return row;
      } else
        return null;
    }

    protected override dsHiveServer.PermissionOwnerRow
      InsertNewRow(PermissionOwner permOwner) {
      dsHiveServer.PermissionOwnerDataTable data =
        new dsHiveServer.PermissionOwnerDataTable();

      dsHiveServer.PermissionOwnerRow row =
        data.NewPermissionOwnerRow();

      data.AddPermissionOwnerRow(row);

      return row;
    }

    protected override void
      UpdateRow(dsHiveServer.PermissionOwnerRow row) {
      Adapter.Update(row);
    }

    protected override IEnumerable<dsHiveServer.PermissionOwnerRow>
      FindById(long id) {
      return Adapter.GetDataById(id);
    }

    protected override IEnumerable<dsHiveServer.PermissionOwnerRow>
      FindAll() {
      return Adapter.GetData();
    }
    #endregion

    #region IPermissionOwner Members

    public bool GetById(PermissionOwner permOwner) {
      if (permOwner != null) {
        dsHiveServer.PermissionOwnerRow row = 
          base.GetRowById(permOwner.Id);

        if(row != null) {
          Convert(row, permOwner);

          return true;
        }
      }

      return false;
    }
    
    public PermissionOwner GetByName(String name) {
      if (name != null) {
        return base.FindSingle(
          delegate() {
            return Adapter.GetDataByName(name);
          });
      }

      return null;
    }

    #endregion
  }
}
