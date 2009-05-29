using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.DataAccess.ADOHelper;
using System.Data.SqlClient;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using System.Data.Common;

namespace HeuristicLab.Hive.Server.ADODataAccess.TableAdapterWrapper {
  class AssignedResourcesAdapterWrapper :
  TableAdapterWrapperBase<dsHiveServerTableAdapters.AssignedResourcesTableAdapter,
  ManyToManyRelation,
  dsHiveServer.AssignedResourcesRow> {
    public override dsHiveServer.AssignedResourcesRow
     InsertNewRow(ManyToManyRelation relation) {
      dsHiveServer.AssignedResourcesDataTable data =
         new dsHiveServer.AssignedResourcesDataTable();

      dsHiveServer.AssignedResourcesRow row =
        data.NewAssignedResourcesRow();

      row.JobId = relation.Id;
      row.ResourceId = relation.Id2;

      data.AddAssignedResourcesRow(row);

      return row;
    }

    public override void
      UpdateRow(dsHiveServer.AssignedResourcesRow row) {
      TransactionalAdapter.Update(row);
    }

    public override IEnumerable<dsHiveServer.AssignedResourcesRow>
      FindById(Guid id) {
      return TransactionalAdapter.GetDataByJobId(id);
    }

    public override IEnumerable<dsHiveServer.AssignedResourcesRow>
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
