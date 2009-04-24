using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.DataAccess.ADOHelper;
using System.Data.SqlClient;
using HeuristicLab.Security.Contracts.BusinessObjects;
using System.Data.Common;

namespace HeuristicLab.Security.ADODataAccess.TableAdapterWrapper {
  class HLUserAdapterWrapper :
   TableAdapterWrapperBase<
       dsSecurityTableAdapters.HLUserTableAdapter,
       User,
       dsSecurity.HLUserRow> {
    public override void UpdateRow(dsSecurity.HLUserRow row) {
      TransactionalAdapter.Update(row);
    }

    public override dsSecurity.HLUserRow
      InsertNewRow(User user) {
      dsSecurity.HLUserDataTable data =
        new dsSecurity.HLUserDataTable();

      dsSecurity.HLUserRow row = data.NewHLUserRow();
      row.PermissionOwnerId = user.Id;
      data.AddHLUserRow(row);

      return row;
    }

    public override IEnumerable<dsSecurity.HLUserRow>
      FindById(Guid id) {
      return TransactionalAdapter.GetDataById(id);
    }

    public override IEnumerable<dsSecurity.HLUserRow>
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
