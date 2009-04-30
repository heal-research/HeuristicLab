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

    private GrantedPermissionsAdapterWrapper grantedPermissionsAdapter;

    private GrantedPermissionsAdapterWrapper GrantedPermissionsAdapter {
      get {
        if (grantedPermissionsAdapter == null)
          grantedPermissionsAdapter = new GrantedPermissionsAdapterWrapper();

        grantedPermissionsAdapter.Session = Session as Session;

        return grantedPermissionsAdapter;
      }
    }

    private IUserGroupAdapter userGroupAdapter;

    private IUserGroupAdapter UserGroupAdapter {
      get {
        if (userGroupAdapter == null)
          userGroupAdapter = this.Session.GetDataAdapter<UserGroup, IUserGroupAdapter>();

        return userGroupAdapter;
      }
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

    #endregion

    #region IPermissionAdapter Members

    public GrantedPermission getPermission(Guid permissionOwnerId, Guid permissionId, Guid entityId) {
      dsSecurity.GrantedPermissionsRow row =
          GrantedPermissionsAdapter.FindByPermissionPermissionOwnerEntityId(
        permissionId, permissionOwnerId, entityId); 

      if (row != null) {
        GrantedPermission perm = new GrantedPermission();
        perm.PermissionId = row.PermissionId;
        perm.PermissionOwnerId = row.PermissionOwnerId;
        perm.EntityId = row.EntityId;

        return perm;
      } else {
        ICollection<UserGroup> groups =
          UserGroupAdapter.MemberOf(permissionOwnerId);

        GrantedPermission perm = null;

        if (groups != null) {
          foreach(UserGroup group in groups) {
            perm = getPermission(group.Id, permissionId, entityId);

            if (perm != null)
              break;
          }
        }

        return perm;
      }
    }

    public bool grantPermission(Guid permissionOwnerId, Guid permissionId, Guid entityId) {
      if (GrantedPermissionsAdapter.FindByPermissionPermissionOwnerEntityId(
        permissionId, permissionOwnerId, entityId) == null) {
        GrantedPermission perm = new GrantedPermission();
        perm.PermissionId = permissionId;
        perm.PermissionOwnerId = permissionOwnerId;
        perm.EntityId = entityId;

        return GrantedPermissionsAdapter.InsertNewRow(perm) != null;
      } else {
        return false;
      }      
    }

    public bool revokePermission(Guid permissionOwnerId, Guid permissionId, Guid entityId) {
      GrantedPermission perm =
        getPermission(permissionOwnerId, permissionId, entityId);
      
      if (perm != null) {
        return GrantedPermissionsAdapter.DeleteRow(perm);
      } else {
        return false;
      } 
    }

    #endregion
  }
}
