using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.DataAccess.ADOHelper;
using System.Data.SqlClient;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using System.Data.Common;

namespace HeuristicLab.Hive.Server.ADODataAccess.TableAdapterWrapper {
  class ClientGroupAdapterWrapper :
    TableAdapterWrapperBase<dsHiveServerTableAdapters.ClientGroupTableAdapter,
    ClientGroup,
    dsHiveServer.ClientGroupRow> {
    public override dsHiveServer.ClientGroupRow
     InsertNewRow(ClientGroup group) {
      dsHiveServer.ClientGroupDataTable data =
         new dsHiveServer.ClientGroupDataTable();

      dsHiveServer.ClientGroupRow row =
        data.NewClientGroupRow();

      row.ResourceId = group.Id;

      data.AddClientGroupRow(row);
      TransactionalAdapter.Update(row);

      return row;
    }

    public override void
      UpdateRow(dsHiveServer.ClientGroupRow row) {
      TransactionalAdapter.Update(row);
    }

    public override IEnumerable<dsHiveServer.ClientGroupRow>
      FindById(Guid id) {
      return TransactionalAdapter.GetDataById(id);
    }

    public override IEnumerable<dsHiveServer.ClientGroupRow>
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
