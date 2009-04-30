using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.DataAccess.ADOHelper;
using System.Data.SqlClient;
using HeuristicLab.Security.Contracts.BusinessObjects;
using System.Data.Common;

namespace HeuristicLab.Security.ADODataAccess.TableAdapterWrapper {
  class GrantedPermissionsAdapterWrapper :
   TableAdapterWrapperBase<
       dsSecurityTableAdapters.GrantedPermissionsTableAdapter,
       GrantedPermission,
       dsSecurity.GrantedPermissionsRow> {
    public override void UpdateRow(dsSecurity.GrantedPermissionsRow row) {
      TransactionalAdapter.Update(row);
    }

    public override dsSecurity.GrantedPermissionsRow
      InsertNewRow(GrantedPermission relation) {
      dsSecurity.GrantedPermissionsDataTable data =
        new dsSecurity.GrantedPermissionsDataTable();

      dsSecurity.GrantedPermissionsRow row = data.NewGrantedPermissionsRow();
      row.PermissionOwnerId = relation.PermissionOwnerId;
      row.PermissionId = relation.PermissionId;
      row.EntityId = relation.EntityId;

      data.AddGrantedPermissionsRow(row);
      TransactionalAdapter.Update(row);

      return row;
    }

    public bool DeleteRow(GrantedPermission perm) {
      dsSecurity.GrantedPermissionsRow row =
        FindByPermissionPermissionOwnerEntityId(perm.PermissionId, perm.PermissionOwnerId, perm.EntityId);

      if (row != null) {
        row.Delete();
        UpdateRow(row);

        return true;
      } else {
        return false;
      }
    }

    public override IEnumerable<dsSecurity.GrantedPermissionsRow>
      FindById(Guid id) {
      return TransactionalAdapter.GetDataByPermissionOwnerId(id);
    }

    public dsSecurity.GrantedPermissionsRow
      FindByPermissionPermissionOwnerEntityId(Guid permissionId, 
      Guid permissionOwnerId, Guid entityId) {
      IEnumerable<dsSecurity.GrantedPermissionsRow> result = 
        TransactionalAdapter.GetDataByPermissionPermissionOwnerEntityId(
          permissionId, permissionOwnerId, entityId);

      if (result.Count() == 1)
        return result.First();
      else
        return null;
    }

    public override IEnumerable<dsSecurity.GrantedPermissionsRow>
      FindAll() {
      return TransactionalAdapter.GetData();
    }

    protected override void SetConnection(DbConnection connection) {
      adapter.Connection = connection as SqlConnection;
    }

    protected override void SetTransaction(DbTransaction transaction) {
      adapter.Transaction = transaction as SqlTransaction;
    }
  }
}
