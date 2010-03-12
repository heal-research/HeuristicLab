using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.DataAccess.ADOHelper;
using System.Data.SqlClient;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using System.Data.Common;

namespace HeuristicLab.Hive.Server.ADODataAccess.TableAdapterWrapper {
  class ProjectAdapterWrapper :
    TableAdapterWrapperBase<dsHiveServerTableAdapters.ProjectTableAdapter,
                      ProjectDto,
                      dsHiveServer.ProjectRow> {
    public override void UpdateRow(dsHiveServer.ProjectRow row) {
      TransactionalAdapter.Update(row);
    }

    public override dsHiveServer.ProjectRow
      InsertNewRow(ProjectDto project) {
      dsHiveServer.ProjectDataTable data =
        new dsHiveServer.ProjectDataTable();

      dsHiveServer.ProjectRow row = data.NewProjectRow();
      row.ProjectId = project.Id;
      data.AddProjectRow(row);

      return row;
    }

    public override IEnumerable<dsHiveServer.ProjectRow>
      FindById(Guid id) {
      return TransactionalAdapter.GetDataById(id);
    }

    public override IEnumerable<dsHiveServer.ProjectRow>
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
