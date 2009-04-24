using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.DataAccess.ADOHelper;
using HeuristicLab.Security.Contracts.BusinessObjects;
using HeuristicLab.Security.DataAccess;
using HeuristicLab.Security.ADODataAccess.TableAdapterWrapper;

namespace HeuristicLab.Security.ADODataAccess {
  class PermissionOwnerAdapter: DataAdapterBase<
      dsSecurityTableAdapters.PermissionOwnerTableAdapter, 
      PermissionOwner, 
      dsSecurity.PermissionOwnerRow>,
      IPermissionOwnerAdapter {
    public PermissionOwnerAdapter() : 
      base(new PermissionOwnerAdapterWrapper()) {
    }

    private IUserAdapter userAdapter = null;

    private IUserAdapter UserAdapter {
      get {
        if (userAdapter == null)
          userAdapter =
            this.Session.GetDataAdapter<User, IUserAdapter>();

        return userAdapter;
      }
    }

    private IUserGroupAdapter userGroupAdapter = null;

    private IUserGroupAdapter UserGroupAdapter {
      get {
        if (userGroupAdapter == null)
          userGroupAdapter =
            this.Session.GetDataAdapter<UserGroup, IUserGroupAdapter>();

        return userGroupAdapter;
      }
    }

    protected override dsSecurity.PermissionOwnerRow ConvertObj(PermissionOwner permOwner, 
      dsSecurity.PermissionOwnerRow row) {
      if (row != null && permOwner != null) {
        row.PermissionOwnerId = permOwner.Id;
        row.Name = permOwner.Name;

        return row;
      } else {
        return null;
      }
    }

    protected override PermissionOwner ConvertRow(dsSecurity.PermissionOwnerRow row,
      PermissionOwner permOwner) {
      if (row != null && permOwner != null) {
        permOwner.Id = row.PermissionOwnerId;

        if (!row.IsNameNull())
          permOwner.Name = row.Name;
        else
          permOwner.Name = String.Empty;

        return permOwner;
      } else {
        return null;
      }
    }

    #region IPermissionOwnerAdapter Members

    public PermissionOwner GetByName(string name) {
      return base.FindSingle(
        delegate() {
          return Adapter.GetDataByName(name);
        });
    }

    public bool GetById(PermissionOwner permOwner) {
      if (permOwner != null) {
        dsSecurity.PermissionOwnerRow row =
          GetRowById(permOwner.Id);

        if (row != null) {
          Convert(row, permOwner);

          return true;
        }
      }

      return false;
    }

    #endregion

    #region IPolymorphicDataAdapter<PermissionOwner> Members

    public void UpdatePolymorphic(PermissionOwner obj) {
      if (obj is User) {
        UserAdapter.Update(obj as User);
      } else if (obj is UserGroup) {
        UserGroupAdapter.Update(obj as UserGroup);
      } else {
        this.Update(obj);
      }
    }

    public PermissionOwner GetByIdPolymorphic(Guid id) {
      UserGroup group =
        UserGroupAdapter.GetById(id);

      if (group != null)
        return group;
      else {
        User user =
          UserAdapter.GetById(id);

        if (user != null)
          return user;
        else {
          return this.GetById(id);
        }
      }
    }

    public bool DeletePolymorphic(PermissionOwner obj) {
      if (obj is User) {
        return UserAdapter.Delete(obj as User);
      } else if (obj is UserGroup) {
        return UserGroupAdapter.Delete(obj as UserGroup);
      } else {
        return this.Delete(obj);
      }
    }

    #endregion
  }
}
