using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.DataAccess.ADOHelper;
using System.Data.SqlClient;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using System.Data.Common;

namespace HeuristicLab.Hive.Server.ADODataAccess.TableAdapterWrapper {
  class RequiredPluginsAdapterWrapper :
  TableAdapterWrapperBase<dsHiveServerTableAdapters.RequiredPluginsTableAdapter,
  ManyToManyRelation,
  dsHiveServer.RequiredPluginsRow> {
    public override dsHiveServer.RequiredPluginsRow
     InsertNewRow(ManyToManyRelation relation) {
      dsHiveServer.RequiredPluginsDataTable data =
         new dsHiveServer.RequiredPluginsDataTable();

      dsHiveServer.RequiredPluginsRow row =
        data.NewRequiredPluginsRow();

      row.JobId = relation.Id;
      row.PluginId = relation.Id2;

      data.AddRequiredPluginsRow(row);
      TransactionalAdapter.Update(row);

      return row;
    }

    public override void
      UpdateRow(dsHiveServer.RequiredPluginsRow row) {
      TransactionalAdapter.Update(row);
    }

    public override IEnumerable<dsHiveServer.RequiredPluginsRow>
      FindById(Guid id) {
      return TransactionalAdapter.GetDataById(id);
    }

    public override IEnumerable<dsHiveServer.RequiredPluginsRow>
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
