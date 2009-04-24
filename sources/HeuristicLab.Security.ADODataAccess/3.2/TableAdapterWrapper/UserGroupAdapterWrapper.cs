using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.DataAccess.ADOHelper;
using System.Data.SqlClient;
using HeuristicLab.Security.Contracts.BusinessObjects;
using System.Data.Common;

namespace HeuristicLab.Security.ADODataAccess.TableAdapterWrapper {
  class UserGroupAdapterWrapper :
   TableAdapterWrapperBase<
       dsSecurityTableAdapters.UserGroupTableAdapter,
       UserGroup,
       dsSecurity.UserGroupRow> {
    public override void UpdateRow(dsSecurity.UserGroupRow row) {
      TransactionalAdapter.Update(row);
    }

    public override dsSecurity.UserGroupRow
      InsertNewRow(UserGroup group) {
      dsSecurity.UserGroupDataTable data =
        new dsSecurity.UserGroupDataTable();

      dsSecurity.UserGroupRow row = data.NewUserGroupRow();
      row.PermissionOwnerId = group.Id;
      data.AddUserGroupRow(row);

      return row;
    }

    public override IEnumerable<dsSecurity.UserGroupRow>
      FindById(Guid id) {
      return TransactionalAdapter.GetDataById(id);
    }

    public override IEnumerable<dsSecurity.UserGroupRow>
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
