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
using System.Data;

namespace HeuristicLab.Hive.Server.ADODataAccess {
  class UserGroupAdapter: 
    DataAdapterBase<
      dsHiveServerTableAdapters.UserGroupTableAdapter, 
      UserGroup, 
      dsHiveServer.UserGroupRow>,
    IUserGroupAdapter {

    #region Fields
    private dsHiveServerTableAdapters.PermissionOwner_UserGroupTableAdapter permOwnerUserGroupAdapter =
      new dsHiveServerTableAdapters.PermissionOwner_UserGroupTableAdapter();

    private IPermissionOwnerAdapter permOwnerAdapter = null;

    private IPermissionOwnerAdapter PermOwnerAdapter {
      get {
        if (permOwnerAdapter == null)
          permOwnerAdapter = ServiceLocator.GetPermissionOwnerAdapter();

        return permOwnerAdapter;
      }
    }

    private IUserAdapter userAdapter = null;

    private IUserAdapter UserAdapter {
      get {
        if (userAdapter == null)
          userAdapter = ServiceLocator.GetUserAdapter();

        return userAdapter;
      }
    }
    #endregion

    #region Overrides
    protected override UserGroup ConvertRow(dsHiveServer.UserGroupRow row,
      UserGroup userGroup) {
      if (row != null && userGroup != null) {
        /*Parent - Permission Owner*/
        userGroup.Id = row.PermissionOwnerId;
        PermOwnerAdapter.GetById(userGroup);

        //first check for created references
        dsHiveServer.PermissionOwner_UserGroupDataTable userGroupRows =
          permOwnerUserGroupAdapter.GetDataByUserGroupId(userGroup.Id);

        foreach (dsHiveServer.PermissionOwner_UserGroupRow permOwnerUserGroupRow in
          userGroupRows) {
          PermissionOwner permOwner = null;
          
          IEnumerable<PermissionOwner> permOwners =
            from p in
              userGroup.Members
            where p.Id == permOwnerUserGroupRow.PermissionOwnerId
            select p;
          if (permOwners.Count<PermissionOwner>() == 1)
            permOwner = permOwners.First<PermissionOwner>();

          if (permOwner == null) {
            PermissionOwner permissionOwner = 
              UserAdapter.GetById(permOwnerUserGroupRow.PermissionOwnerId);

            if (permissionOwner == null) {
              //is a user group
              permissionOwner =
                GetById(permOwnerUserGroupRow.PermissionOwnerId);
            }

            if(permissionOwner != null)
              userGroup.Members.Add(permissionOwner);
          }
        }

        //secondly check for deleted references
        ICollection<PermissionOwner> deleted = 
          new List<PermissionOwner>();

        foreach (PermissionOwner permOwner in userGroup.Members) {
          dsHiveServer.PermissionOwner_UserGroupDataTable found =
            permOwnerUserGroupAdapter.GetDataByPermownerUsergroupId(
              permOwner.Id,
              userGroup.Id);
          if (found.Count != 1) {
            deleted.Add(permOwner);
          }
        }

        foreach (PermissionOwner permOwner in deleted) {
          userGroup.Members.Remove(permOwner);
        }

        return userGroup;
      } else
        return null;
    }

    protected override dsHiveServer.UserGroupRow ConvertObj(UserGroup userGroup,
      dsHiveServer.UserGroupRow row) {
      if (userGroup != null && row != null) {
        row.PermissionOwnerId = userGroup.Id;
      
        //update references
        foreach (PermissionOwner permOwner in userGroup.Members) {          
          //first update the member to make sure it exists in the DB
          if (permOwner is User) {
            UserAdapter.Update(permOwner as User);
          } else if (permOwner is UserGroup) {
            Update(permOwner as UserGroup);
          }

          //secondly check for created references
          dsHiveServer.PermissionOwner_UserGroupRow permOwnerUserGroupRow =
            null;
          dsHiveServer.PermissionOwner_UserGroupDataTable found = 
            permOwnerUserGroupAdapter.GetDataByPermownerUsergroupId(
              permOwner.Id,
              userGroup.Id);
          if (found.Count == 1)
            permOwnerUserGroupRow = found[0];

          if (permOwnerUserGroupRow == null) {
            permOwnerUserGroupRow = 
              found.NewPermissionOwner_UserGroupRow();

            permOwnerUserGroupRow.PermissionOwnerId =
              permOwner.Id;
            permOwnerUserGroupRow.UserGroupId =
              userGroup.Id;

            found.AddPermissionOwner_UserGroupRow(permOwnerUserGroupRow);

            permOwnerUserGroupAdapter.Update(permOwnerUserGroupRow);
          }
        }

        //thirdly check for deleted references
        dsHiveServer.PermissionOwner_UserGroupDataTable userGroupRows =
            permOwnerUserGroupAdapter.GetDataByUserGroupId(userGroup.Id);

        ICollection<dsHiveServer.PermissionOwner_UserGroupRow> deleted =
          new List<dsHiveServer.PermissionOwner_UserGroupRow>();

        foreach (dsHiveServer.PermissionOwner_UserGroupRow permOwnerUserGroupRow in
          userGroupRows) {
          PermissionOwner permOwner = null;

          IEnumerable<PermissionOwner> permOwners =
            from p in
              userGroup.Members
            where p.Id == permOwnerUserGroupRow.PermissionOwnerId
            select p;

          if (permOwners.Count<PermissionOwner>() == 1)
            permOwner = permOwners.First<PermissionOwner>();

          if (permOwner == null) {
            deleted.Add(permOwnerUserGroupRow);
          }
        }

        foreach (dsHiveServer.PermissionOwner_UserGroupRow 
          permOwnerUserGroupRow in deleted) {
          permOwnerUserGroupRow.Delete();
          permOwnerUserGroupAdapter.Update(permOwnerUserGroupRow);
        }
      }

      return row;
    }

    protected override dsHiveServer.UserGroupRow
      InsertNewRow(UserGroup group) {
      dsHiveServer.UserGroupDataTable data =
        new dsHiveServer.UserGroupDataTable();

      dsHiveServer.UserGroupRow row =
        data.NewUserGroupRow();

      row.PermissionOwnerId = group.Id;

      data.AddUserGroupRow(row);
      Adapter.Update(row);

      return row;
    }

    protected override void
     UpdateRow(dsHiveServer.UserGroupRow row) {
      Adapter.Update(row);
    }

    protected override IEnumerable<dsHiveServer.UserGroupRow>
      FindById(long id) {
      return Adapter.GetDataById(id);
    }

    protected override IEnumerable<dsHiveServer.UserGroupRow>
      FindAll() {
      return Adapter.GetData();
    }
    #endregion

    #region IUserGroupAdapter Members
    public override void Update(UserGroup group) {
      if (group != null) {
        PermOwnerAdapter.Update(group);

        base.Update(group);
      }
    }

    public UserGroup GetByName(String name) {
      if (name != null) {
        return base.FindSingle(
          delegate() {
            return Adapter.GetDataByName(name);
          });
      }

      return null;
    }

    public ICollection<UserGroup> MemberOf(PermissionOwner permOwner) {
      ICollection<UserGroup> userGroups =
        new List<UserGroup>();
      
      if (permOwner != null) {
        dsHiveServer.PermissionOwner_UserGroupDataTable userGroupRows =
          permOwnerUserGroupAdapter.GetDataByPermissionOwnerId(permOwner.Id);

        foreach (dsHiveServer.PermissionOwner_UserGroupRow userGroupRow in
          userGroupRows) {
          UserGroup userGroup = 
            GetById(userGroupRow.UserGroupId);
          userGroups.Add(userGroup);
        }
      }

      return userGroups;
    }

    public override bool Delete(UserGroup group) {
      if (group != null) {
        return base.Delete(group) && 
          PermOwnerAdapter.Delete(group);
      }

      return false;
    }

    #endregion
  }
}
