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
using System.Data;

namespace HeuristicLab.Hive.Server.ADODataAccess {
  class UserGroupAdapter: DataAdapterBase, IUserGroupAdapter {
    private dsHiveServerTableAdapters.UserGroupTableAdapter adapter =
        new dsHiveServerTableAdapters.UserGroupTableAdapter();

    private dsHiveServer.UserGroupDataTable data =
      new dsHiveServer.UserGroupDataTable();

    private dsHiveServerTableAdapters.PermissionOwner_UserGroupTableAdapter permOwnerUserGroupAdapter =
      new dsHiveServerTableAdapters.PermissionOwner_UserGroupTableAdapter();

    private dsHiveServer.PermissionOwner_UserGroupDataTable permOwnerUserGroupData =
      new dsHiveServer.PermissionOwner_UserGroupDataTable();

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

    public UserGroupAdapter() {
      adapter.Fill(data);
      permOwnerUserGroupAdapter.Fill(permOwnerUserGroupData);
    }

    protected override void Update() {
      this.adapter.Update(this.data);
      this.permOwnerUserGroupAdapter.Update(permOwnerUserGroupData);
    }

    private UserGroup Convert(dsHiveServer.UserGroupRow row,
      UserGroup userGroup) {
      if (row != null && userGroup != null) {
        /*Parent - Permission Owner*/
        userGroup.PermissionOwnerId = row.PermissionOwnerId;
        PermOwnerAdapter.GetPermissionOwnerById(userGroup);

        //first check for created references
        IEnumerable<dsHiveServer.PermissionOwner_UserGroupRow> userGroupRows =
          from permOwner in
            permOwnerUserGroupData.AsEnumerable<dsHiveServer.PermissionOwner_UserGroupRow>()
          where permOwner.UserGroupId == userGroup.PermissionOwnerId
          select permOwner;

        foreach (dsHiveServer.PermissionOwner_UserGroupRow permOwnerUserGroupRow in
          userGroupRows) {
          PermissionOwner permOwner = null;
          
          IEnumerable<PermissionOwner> permOwners =
            from p in
              userGroup.Members
            where p.PermissionOwnerId == permOwnerUserGroupRow.PermissionOwnerId
            select p;
          if (permOwners.Count<PermissionOwner>() == 1)
            permOwner = permOwners.First<PermissionOwner>();

          if (permOwner == null) {
            PermissionOwner permissionOwner = 
              UserAdapter.GetUserById(permOwnerUserGroupRow.PermissionOwnerId);

            if (permissionOwner == null) {
              //is a user group
              permissionOwner =
                GetUserGroupById(permOwnerUserGroupRow.PermissionOwnerId);
            }

            if(permissionOwner != null)
              userGroup.Members.Add(permissionOwner);
          }
        }

        //secondly check for deleted references
        ICollection<PermissionOwner> deleted = 
          new List<PermissionOwner>();

        foreach (PermissionOwner permOwner in userGroup.Members) {
          dsHiveServer.PermissionOwner_UserGroupRow permOwnerUserGroupRow =
            permOwnerUserGroupData.FindByPermissionOwnerIdUserGroupId(
              permOwner.PermissionOwnerId,
              userGroup.PermissionOwnerId);

          if (permOwnerUserGroupRow == null) {
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

    private dsHiveServer.UserGroupRow Convert(UserGroup userGroup,
      dsHiveServer.UserGroupRow row) {
      if (userGroup != null && row != null) {
        row.PermissionOwnerId = userGroup.PermissionOwnerId;
      
        //update references
        foreach (PermissionOwner permOwner in userGroup.Members) {          
          //first update the member to make sure it exists in the DB
          if (permOwner is User) {
            UserAdapter.UpdateUser(permOwner as User);
          } else if (permOwner is UserGroup) {
            UpdateUserGroup(permOwner as UserGroup);
          }

          //secondly check for created references
          dsHiveServer.PermissionOwner_UserGroupRow permOwnerUserGroupRow =
            permOwnerUserGroupData.FindByPermissionOwnerIdUserGroupId(
              permOwner.PermissionOwnerId,
              userGroup.PermissionOwnerId);

          if (permOwnerUserGroupRow == null) {
            permOwnerUserGroupRow = 
              permOwnerUserGroupData.NewPermissionOwner_UserGroupRow();

            permOwnerUserGroupRow.PermissionOwnerId =
              permOwner.PermissionOwnerId;
            permOwnerUserGroupRow.UserGroupId =
              userGroup.PermissionOwnerId;

            permOwnerUserGroupData.AddPermissionOwner_UserGroupRow(
              permOwnerUserGroupRow);
          }
        }

        //thirdly check for deleted references
        IEnumerable<dsHiveServer.PermissionOwner_UserGroupRow> userGroupRows = 
          from permOwner in 
            permOwnerUserGroupData.AsEnumerable<dsHiveServer.PermissionOwner_UserGroupRow>()
          where permOwner.UserGroupId == userGroup.PermissionOwnerId  
          select permOwner;

        ICollection<dsHiveServer.PermissionOwner_UserGroupRow> deleted =
          new List<dsHiveServer.PermissionOwner_UserGroupRow>();

        foreach (dsHiveServer.PermissionOwner_UserGroupRow permOwnerUserGroupRow in
          userGroupRows) {
          PermissionOwner permOwner = null;

          IEnumerable<PermissionOwner> permOwners =
            from p in
              userGroup.Members
            where p.PermissionOwnerId == permOwnerUserGroupRow.PermissionOwnerId
            select p;

          if (permOwners.Count<PermissionOwner>() == 1)
            permOwner = permOwners.First<PermissionOwner>();

          if (permOwner == null) {
            deleted.Add(permOwnerUserGroupRow);
          }
        }

        foreach (dsHiveServer.PermissionOwner_UserGroupRow permOwnerUserGroupRow in
          deleted) {
          permOwnerUserGroupData.RemovePermissionOwner_UserGroupRow(
            permOwnerUserGroupRow);
        }

      }

      return row;
    }

    #region IUserGroupAdapter Members
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void UpdateUserGroup(UserGroup group) {
      if (group != null) {
        PermOwnerAdapter.UpdatePermissionOwner(group);

        dsHiveServer.UserGroupRow row =
          data.FindByPermissionOwnerId(group.PermissionOwnerId);

        if (row == null) {
          row = data.NewUserGroupRow();
          row.PermissionOwnerId = group.PermissionOwnerId;
          data.AddUserGroupRow(row);
        }

        Convert(group, row);
      }
    }

    public UserGroup GetUserGroupById(long userGroupId) {
      UserGroup userGroup = new UserGroup();

      dsHiveServer.UserGroupRow row =
        data.FindByPermissionOwnerId(userGroupId);

      if (row != null) {
        Convert(row, userGroup);

        return userGroup;
      } else {
        return null;
      }
    }

    public ICollection<UserGroup> GetAllUserGroups() {
      ICollection<UserGroup> allUserGroups =
        new List<UserGroup>();

      foreach (dsHiveServer.UserGroupRow row in data) {
        UserGroup userGroup = new UserGroup();

        Convert(row, userGroup);
        allUserGroups.Add(userGroup);
      }

      return allUserGroups;
    }

    public ICollection<UserGroup> MemberOf(PermissionOwner permOwner) {
      ICollection<UserGroup> userGroups =
        new List<UserGroup>();
      
      if (permOwner != null) {
        IEnumerable<dsHiveServer.PermissionOwner_UserGroupRow> userGroupRows =
         from userGroup in
           permOwnerUserGroupData.AsEnumerable<dsHiveServer.PermissionOwner_UserGroupRow>()
         where userGroup.PermissionOwnerId == permOwner.PermissionOwnerId
         select userGroup;

        foreach (dsHiveServer.PermissionOwner_UserGroupRow userGroupRow in
          userGroupRows) {
          UserGroup userGroup = 
            GetUserGroupById(userGroupRow.UserGroupId);
          userGroups.Add(userGroup);
        }
      }

      return userGroups;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool DeleteUserGroup(UserGroup group) {
      if (group != null) {
        dsHiveServer.UserGroupRow row =
          data.FindByPermissionOwnerId(group.PermissionOwnerId);

        if (row != null) {
          ICollection<dsHiveServer.PermissionOwner_UserGroupRow> deleted =
            new List<dsHiveServer.PermissionOwner_UserGroupRow>();
          
          foreach (dsHiveServer.PermissionOwner_UserGroupRow permOwnerUserGroupRow in
            permOwnerUserGroupData) {
            if (permOwnerUserGroupRow.UserGroupId == group.PermissionOwnerId || 
              permOwnerUserGroupRow.PermissionOwnerId == group.PermissionOwnerId) {
              deleted.Add(permOwnerUserGroupRow);
            }
          }

          foreach (dsHiveServer.PermissionOwner_UserGroupRow permOwnerUserGroupRow in
            deleted) {
            permOwnerUserGroupData.RemovePermissionOwner_UserGroupRow(
              permOwnerUserGroupRow);
          }
          
          data.RemoveUserGroupRow(row);
          return PermOwnerAdapter.DeletePermissionOwner(group);
        }
      }

      return false;
    }

    #endregion
  }
}
