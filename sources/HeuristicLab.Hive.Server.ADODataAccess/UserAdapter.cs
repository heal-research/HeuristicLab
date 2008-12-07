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
  class UserAdapter : DataAdapterBase, IUserAdapter {
    private dsHiveServerTableAdapters.HiveUserTableAdapter adapter =
        new dsHiveServerTableAdapters.HiveUserTableAdapter();

    private dsHiveServer.HiveUserDataTable data =
        new dsHiveServer.HiveUserDataTable();

    private IPermissionOwnerAdapter permOwnerAdapter =
       ServiceLocator.GetPermissionOwnerAdapter();

    public UserAdapter() {
      adapter.Fill(data);
    }

    protected override void Update() {
      this.adapter.Update(this.data);
    }

    private User Convert(dsHiveServer.HiveUserRow row, 
      User user) {
      if (row != null && user != null) {
        /*Parent - PermissionOwner*/
        user.PermissionOwnerId = row.PermissionOwnerId;
        permOwnerAdapter.GetPermissionOwnerById(user);

        /*User*/
        if (!row.IsPasswordNull())
          user.Password = row.Password;
        else
          user.Password = String.Empty;

        return user;
      } else
        return null;
    }

    private dsHiveServer.HiveUserRow Convert(User user,
      dsHiveServer.HiveUserRow row) {
      if (user != null && row != null) {
        row.Password = user.Password;

        return row;
      } else
        return null;     
    }

    #region IUserAdapter Members

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void UpdateUser(User user) {
      if (user != null) {
        permOwnerAdapter.UpdatePermissionOwner(user);

        dsHiveServer.HiveUserRow row = 
          data.FindByPermissionOwnerId(user.PermissionOwnerId);
        if (row == null) {
          row = data.NewHiveUserRow();
          row.PermissionOwnerId = user.PermissionOwnerId;
          data.AddHiveUserRow(row);
        } 

        Convert(user, row);
      }
    }

    public User GetUserById(long userId) {
      User user = new User();

      dsHiveServer.HiveUserRow row =
          data.FindByPermissionOwnerId(userId);

      if(row != null) {
        Convert(row, user);

        return user;
      } else {
        return null;
      }
    }

    public User GetUserByName(String name) {
      User user = new User();

      PermissionOwner permOwner =
        permOwnerAdapter.GetPermissionOwnerByName(name);

      if (permOwner != null) {
        dsHiveServer.HiveUserRow row =
          data.FindByPermissionOwnerId(permOwner.PermissionOwnerId);

        if (row != null) {
          Convert(row, user);

          return user;
        }
      }

      return null;
    }

    public ICollection<User> GetAllUsers() {
      ICollection<User> allUsers =
        new List<User>();

      foreach (dsHiveServer.HiveUserRow row in data) {
        User user = new User();
        Convert(row, user);
        allUsers.Add(user);
      }

      return allUsers;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool DeleteUser(User user) {
      if (user != null) {
        dsHiveServer.HiveUserRow row =
          data.FindByPermissionOwnerId(user.PermissionOwnerId);

        if (row != null) {
          data.RemoveHiveUserRow(row);

          return permOwnerAdapter.DeletePermissionOwner(user);
        }
      }

      return false;
    }

    #endregion
  }
}
