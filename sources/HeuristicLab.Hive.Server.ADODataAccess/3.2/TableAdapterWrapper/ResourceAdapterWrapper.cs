using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.DataAccess.ADOHelper;
using System.Data.SqlClient;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using System.Data.Common;

namespace HeuristicLab.Hive.Server.ADODataAccess.TableAdapterWrapper {
  class ResourceAdapterWrapper :
  TableAdapterWrapperBase<
    dsHiveServerTableAdapters.ResourceTableAdapter,
    ResourceDto,
    dsHiveServer.ResourceRow> {
    public override void UpdateRow(dsHiveServer.ResourceRow row) {
      TransactionalAdapter.Update(row);
    }

    public override dsHiveServer.ResourceRow
      InsertNewRow(ResourceDto resource) {
      dsHiveServer.ResourceDataTable data =
        new dsHiveServer.ResourceDataTable();

      dsHiveServer.ResourceRow row = data.NewResourceRow();
      row.ResourceId = resource.Id;
      data.AddResourceRow(row);

      return row;
    }

    public override IEnumerable<dsHiveServer.ResourceRow>
      FindById(Guid id) {
      return TransactionalAdapter.GetDataById(id);
    }

    public override IEnumerable<dsHiveServer.ResourceRow>
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
