using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.DataAccess.ADOHelper;
using System.Data.SqlClient;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using System.Data.Common;

namespace HeuristicLab.Hive.Server.ADODataAccess.TableAdapterWrapper {
  class ClientAdapterWrapper :
   TableAdapterWrapperBase<
       dsHiveServerTableAdapters.ClientTableAdapter,
   ClientInfo,
   dsHiveServer.ClientRow> {
    public override void UpdateRow(dsHiveServer.ClientRow row) {
      TransactionalAdapter.Update(row);
    }

    public override dsHiveServer.ClientRow
      InsertNewRow(ClientInfo client) {
      dsHiveServer.ClientDataTable data =
        new dsHiveServer.ClientDataTable();

      dsHiveServer.ClientRow row = data.NewClientRow();
      row.ResourceId = client.Id;
      data.AddClientRow(row);

      return row;
    }

    public override IEnumerable<dsHiveServer.ClientRow>
      FindById(Guid id) {
      return TransactionalAdapter.GetDataById(id);
    }

    public override IEnumerable<dsHiveServer.ClientRow>
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
