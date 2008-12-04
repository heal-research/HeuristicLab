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
  class UserAdapter: IUserAdapter {
    private dsHiveServerTableAdapters.HiveUserTableAdapter adapter =
        new dsHiveServerTableAdapters.HiveUserTableAdapter();

    private PermissionOwnerAdapter permOwnerAdapter =
      new PermissionOwnerAdapter();

    private User Convert(dsHiveServer.HiveUserRow row, 
      User user) {
      if (row != null && user != null) {
        /*Parent - PermissionOwner*/
        user.PermissionOwnerId = row.PermissionOwnerId;
        permOwnerAdapter.FillPermissionOwner(user);

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
        row.PermissionOwnerId = user.PermissionOwnerId;
        row.Password = user.Password;

        return row;
      } else
        return null;     
    }

    #region IUserAdapter Members

    public void UpdateUser(User user) {
      throw new NotImplementedException();
    }

    public User GetUserById(long userId) {
      throw new NotImplementedException();
    }

    public User GetUserByName(String name) {
      throw new NotImplementedException();
    }

    public ICollection<User> GetAllUsers() {
      throw new NotImplementedException();
    }

    public bool DeleteUser(User user) {
      throw new NotImplementedException();
    }

    #endregion
  }
}
