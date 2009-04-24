using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.DataAccess.ADOHelper;
using System.Data.SqlClient;
using HeuristicLab.Security.Contracts.BusinessObjects;
using System.Data.Common;

namespace HeuristicLab.Security.ADODataAccess.TableAdapterWrapper {
  class PermissionOwnerAdapterWrapper :
   TableAdapterWrapperBase<
       dsSecurityTableAdapters.PermissionOwnerTableAdapter,
       PermissionOwner,
       dsSecurity.PermissionOwnerRow> {
    public override void UpdateRow(dsSecurity.PermissionOwnerRow row) {
      TransactionalAdapter.Update(row);
    }

    public override dsSecurity.PermissionOwnerRow
      InsertNewRow(PermissionOwner permission) {
      dsSecurity.PermissionOwnerDataTable data =
        new dsSecurity.PermissionOwnerDataTable();

      dsSecurity.PermissionOwnerRow row = data.NewPermissionOwnerRow();
      row.PermissionOwnerId = permission.Id;
      data.AddPermissionOwnerRow(row);

      return row;
    }

    public override IEnumerable<dsSecurity.PermissionOwnerRow>
      FindById(Guid id) {
      return TransactionalAdapter.GetDataById(id);
    }

    public override IEnumerable<dsSecurity.PermissionOwnerRow>
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
