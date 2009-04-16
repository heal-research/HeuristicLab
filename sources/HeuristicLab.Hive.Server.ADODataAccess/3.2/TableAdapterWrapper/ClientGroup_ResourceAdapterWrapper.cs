using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.DataAccess.ADOHelper;
using System.Data.SqlClient;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using System.Data.Common;

namespace HeuristicLab.Hive.Server.ADODataAccess.TableAdapterWrapper {
  class ClientGroup_ResourceAdapterWrapper :
  TableAdapterWrapperBase<dsHiveServerTableAdapters.ClientGroup_ResourceTableAdapter,
  ManyToManyRelation,
  dsHiveServer.ClientGroup_ResourceRow> {
    public override dsHiveServer.ClientGroup_ResourceRow
     InsertNewRow(ManyToManyRelation relation) {
      dsHiveServer.ClientGroup_ResourceDataTable data =
         new dsHiveServer.ClientGroup_ResourceDataTable();

      dsHiveServer.ClientGroup_ResourceRow row =
        data.NewClientGroup_ResourceRow();

      row.ClientGroupId = relation.Id;
      row.ResourceId = relation.Id2;

      data.AddClientGroup_ResourceRow(row);
      TransactionalAdapter.Update(row);

      return row;
    }

    public override void
      UpdateRow(dsHiveServer.ClientGroup_ResourceRow row) {
      TransactionalAdapter.Update(row);
    }

    public override IEnumerable<dsHiveServer.ClientGroup_ResourceRow>
      FindById(Guid id) {
      return TransactionalAdapter.GetDataByClientGroupId(id);
    }

    public override IEnumerable<dsHiveServer.ClientGroup_ResourceRow>
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
