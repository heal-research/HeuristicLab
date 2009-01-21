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
  class UserAdapter :
    DataAdapterBase<
      dsHiveServerTableAdapters.HiveUserTableAdapter,
      User,
      dsHiveServer.HiveUserRow>,
    IUserAdapter {
    #region Fields
    private IPermissionOwnerAdapter permOwnerAdapter = null;

    private IPermissionOwnerAdapter PermOwnerAdapter {
      get {
        if (permOwnerAdapter == null)
          permOwnerAdapter = ServiceLocator.GetPermissionOwnerAdapter();

        return permOwnerAdapter;
      }
    }

    private IUserGroupAdapter userGroupAdapter = null;

    private IUserGroupAdapter UserGroupAdapter {
      get {
        if (userGroupAdapter == null)
          userGroupAdapter = ServiceLocator.GetUserGroupAdapter();

        return userGroupAdapter;
      }
    }

    private IJobAdapter jobAdapter = null;

    private IJobAdapter JobAdapter {
      get {
        if (jobAdapter == null)
          jobAdapter = ServiceLocator.GetJobAdapter();

        return jobAdapter;
      }
    }
    #endregion

    #region Overrides
    protected override User ConvertRow(dsHiveServer.HiveUserRow row,
      User user) {
      if (row != null && user != null) {
        /*Parent - PermissionOwner*/
        user.Id = row.PermissionOwnerId;
        PermOwnerAdapter.GetById(user);

        /*User*/
        if (!row.IsPasswordNull())
          user.Password = row.Password;
        else
          user.Password = String.Empty;

        return user;
      } else
        return null;
    }

    protected override dsHiveServer.HiveUserRow ConvertObj(User user,
      dsHiveServer.HiveUserRow row) {
      if (user != null && row != null) {
        if (user.Password == null)
          row.SetPasswordNull();
        else
          row.Password = user.Password;

        return row;
      } else
        return null;
    }

    protected override dsHiveServer.HiveUserRow
      InsertNewRow(User user) {
      dsHiveServer.HiveUserDataTable data =
        new dsHiveServer.HiveUserDataTable();

      dsHiveServer.HiveUserRow row =
        data.NewHiveUserRow();

      row.PermissionOwnerId = user.Id;

      data.AddHiveUserRow(row);

      return row;
    }

    protected override void
      UpdateRow(dsHiveServer.HiveUserRow row) {
      Adapter.Update(row);
    }

    protected override IEnumerable<dsHiveServer.HiveUserRow>
      FindById(long id) {
      return Adapter.GetDataById(id);
    }

    protected override IEnumerable<dsHiveServer.HiveUserRow>
      FindAll() {
      return Adapter.GetData();
    }
    #endregion

    #region IUserAdapter Members

    [MethodImpl(MethodImplOptions.Synchronized)]
    public override void Update(User user) {
      if (user != null) {
        PermOwnerAdapter.Update(user);

        base.Update(user);
      }
    }

    public User GetByName(String name) {
      if (name != null) {
        return base.FindSingle(
          delegate() {
            return Adapter.GetDataByName(name);
          });
      }

      return null;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public override bool Delete(User user) {
      if (user != null) {
        //Referential integrity with jobs - they are cached
        ICollection<Job> jobs =
          JobAdapter.GetJobsOf(user);
        foreach (Job job in jobs) {
          JobAdapter.Delete(job);
        }

        return base.Delete(user) &&
            PermOwnerAdapter.Delete(user);
      }

      return false;
    }

    #endregion
  }
}
