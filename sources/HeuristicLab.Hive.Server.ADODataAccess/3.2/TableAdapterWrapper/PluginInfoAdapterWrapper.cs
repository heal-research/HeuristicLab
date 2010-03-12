using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.DataAccess.ADOHelper;
using System.Data.SqlClient;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using System.Data.Common;

namespace HeuristicLab.Hive.Server.ADODataAccess.TableAdapterWrapper {
  class PluginInfoAdapterWrapper :
   TableAdapterWrapperBase<
       dsHiveServerTableAdapters.PluginInfoTableAdapter,
   HivePluginInfoDto,
   dsHiveServer.PluginInfoRow> {
    public override void UpdateRow(dsHiveServer.PluginInfoRow row) {
      TransactionalAdapter.Update(row);
    }

    public override dsHiveServer.PluginInfoRow
      InsertNewRow(HivePluginInfoDto pluginInfo) {
      dsHiveServer.PluginInfoDataTable data =
        new dsHiveServer.PluginInfoDataTable();

      dsHiveServer.PluginInfoRow row = data.NewPluginInfoRow();
      row.PluginId = pluginInfo.Id;
      data.AddPluginInfoRow(row);

      return row;
    }

    public override IEnumerable<dsHiveServer.PluginInfoRow>
      FindById(Guid id) {
      return TransactionalAdapter.GetDataById(id);
    }

    public override IEnumerable<dsHiveServer.PluginInfoRow>
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
