using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.DataAccess.ADOHelper;
using System.Data.SqlClient;
using HeuristicLab.Security.Contracts.BusinessObjects;
using System.Data.Common;

namespace HeuristicLab.Security.ADODataAccess.TableAdapterWrapper {
  class PermissionAdapterWrapper :
   TableAdapterWrapperBase<
       dsSecurityTableAdapters.PermissionTableAdapter,
       Permission,
       dsSecurity.PermissionRow> {
    public override void UpdateRow(dsSecurity.PermissionRow row) {
      TransactionalAdapter.Update(row);
    }

    public override dsSecurity.PermissionRow
      InsertNewRow(Permission permission) {
      dsSecurity.PermissionDataTable data =
        new dsSecurity.PermissionDataTable();

      dsSecurity.PermissionRow row = data.NewPermissionRow();
      row.PermissionId = permission.Id;
      data.AddPermissionRow(row);

      return row;
    }

    public override IEnumerable<dsSecurity.PermissionRow>
      FindById(Guid id) {
      return TransactionalAdapter.GetDataById(id);
    }

    public override IEnumerable<dsSecurity.PermissionRow>
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
