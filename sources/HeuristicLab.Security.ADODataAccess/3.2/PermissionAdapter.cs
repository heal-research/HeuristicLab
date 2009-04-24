using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.DataAccess.ADOHelper;
using HeuristicLab.Security.Contracts.BusinessObjects;
using HeuristicLab.Security.DataAccess;
using HeuristicLab.Security.ADODataAccess.TableAdapterWrapper;

namespace HeuristicLab.Security.ADODataAccess {
  class PermissionAdapter: DataAdapterBase<
      dsSecurityTableAdapters.PermissionTableAdapter, 
      Permission, 
      dsSecurity.PermissionRow>,
      IPermissionAdapter {
    public PermissionAdapter() :
      base(new PermissionAdapterWrapper()) {
    }

    protected override dsSecurity.PermissionRow ConvertObj(Permission perm, 
      dsSecurity.PermissionRow row) {
      if (row != null && perm != null) {
        row.PermissionId = perm.Id;
        row.Name = perm.Name;
        row.Description = perm.Description;
        row.Plugin = perm.Plugin;

        return row;
      } else {
        return null;
      }
    }

    protected override Permission ConvertRow(dsSecurity.PermissionRow row,
      Permission perm) {
      if (row != null && perm != null) {
        perm.Id = row.PermissionId;
        if (!row.IsNameNull())
          perm.Name = row.Name;
        else
          perm.Name = String.Empty;

        if (!row.IsDescriptionNull())
          perm.Description = row.Description;
        else
          perm.Description = String.Empty;

        if (!row.IsPluginNull())
          perm.Plugin = row.Plugin;
        else
          perm.Plugin = String.Empty;

        return perm;
      } else {
        return null;
      }
    }

    #region IPermissionAdapter Members

    public GrantedPermission getPermission(PermissionOwner permissionOwner, 
      Permission permission, 
      Guid entityId) {
      throw new NotImplementedException();
    }

    public bool grantPermission(Guid permissionOwnerId, 
      Guid permissionId, 
      Guid entityId) {
      throw new NotImplementedException();
    }

    public bool revokePermission(Guid permissionOwnerId, 
      Guid permissionId, 
      Guid entityId) {
      throw new NotImplementedException();
    }

    #endregion
  }
}
