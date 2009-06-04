using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.DataAccess.ADOHelper;
using System.Data.SqlClient;
using System.Data.Common;

namespace HeuristicLab.Hive.Server.ADODataAccess.TableAdapterWrapper {
  class ClientConfigAdapterWrapper :
    TableAdapterWrapperBase<
        dsHiveServerTableAdapters.ClientConfigTableAdapter,
    ClientConfig,
    dsHiveServer.ClientConfigRow> {
    public override void UpdateRow(dsHiveServer.ClientConfigRow row) {
      TransactionalAdapter.Update(row);
    }

    public override dsHiveServer.ClientConfigRow
      InsertNewRow(ClientConfig config) {
      dsHiveServer.ClientConfigDataTable data =
        new dsHiveServer.ClientConfigDataTable();

      dsHiveServer.ClientConfigRow row = data.NewClientConfigRow();
      row.ClientConfigId = config.Id;
      data.AddClientConfigRow(row);

      return row;
    }

    public override IEnumerable<dsHiveServer.ClientConfigRow>
      FindById(Guid id) {
      return TransactionalAdapter.GetDataById(id);
    }

    public override IEnumerable<dsHiveServer.ClientConfigRow>
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
