using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.DataAccess.ADOHelper;
using HeuristicLab.Security.Contracts.BusinessObjects;
using HeuristicLab.Security.DataAccess;
using HeuristicLab.Security.ADODataAccess.TableAdapterWrapper;

namespace HeuristicLab.Security.ADODataAccess {
  class HLUserAdapter: DataAdapterBase<
      dsSecurityTableAdapters.HLUserTableAdapter, 
      User, 
      dsSecurity.HLUserRow>,
      IUserAdapter {
    public HLUserAdapter() :
      base(new HLUserAdapterWrapper()) {
    }

    private IPermissionOwnerAdapter permOwnerAdapter = null;

    private IPermissionOwnerAdapter PermOwnerAdapter {
      get {
        if (permOwnerAdapter == null)
          permOwnerAdapter =
            this.Session.GetDataAdapter<PermissionOwner, IPermissionOwnerAdapter>();

        return permOwnerAdapter;
      }
    }

    protected override dsSecurity.HLUserRow ConvertObj(User user, dsSecurity.HLUserRow row) {
      if (user != null && row != null) {
        row.PermissionOwnerId = user.Id;

        return row;
      } else {
        return null;
      }
    }

    protected override User ConvertRow(dsSecurity.HLUserRow row, User user) {
      if (user != null && row != null) {
        /*Parent - permissionOwner*/
        user.Id = row.PermissionOwnerId;
        PermOwnerAdapter.GetById(user);

        if (!row.IsLoginNull())
          user.Login = row.Login;
        else
          user.Login = String.Empty;

        if (!row.IsPasswordNull())
          user.Password = row.Password;
        else
          user.Password = String.Empty;

        if (!row.IsMailAddressNull())
          user.MailAddress = row.MailAddress;
        else
          user.MailAddress = String.Empty;

        return user;
      } else {
        return null;
      }
    }

    #region IUserAdapter Members

    protected override void doUpdate(User user) {
      if (user != null) {
        PermOwnerAdapter.Update(user);

        base.doUpdate(user);
      }
    }

    protected override bool doDelete(User user) {
      bool success = false;

      if (user != null) {
        dsSecurity.HLUserRow row =
          GetRowById(user.Id);

        if (row != null) {
          success = base.doDelete(user) &&
            PermOwnerAdapter.Delete(user);
        }
      }

      return success;
    }

    public User GetByName(string name)
    {
      User user = new User();
      PermissionOwner permOwner =
        PermOwnerAdapter.GetByName(name);

      return GetById(permOwner.Id);
    }

    #endregion
}
}
