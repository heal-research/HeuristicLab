using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.DataAccess.ADOHelper;
using System.Data.SqlClient;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using System.Data.Common;
using System.IO;

namespace HeuristicLab.Hive.Server.ADODataAccess.TableAdapterWrapper {
  class JobResultsAdapterWrapper :
    TableAdapterWrapperBase<dsHiveServerTableAdapters.JobResultTableAdapter,
                    JobResult,
                    dsHiveServer.JobResultRow> {
    public override void UpdateRow(dsHiveServer.JobResultRow row) {
      TransactionalAdapter.Update(row);
    }

    public override dsHiveServer.JobResultRow InsertNewRow(JobResult obj) {
      dsHiveServer.JobResultDataTable data =
        new dsHiveServer.JobResultDataTable();

      dsHiveServer.JobResultRow row = data.NewJobResultRow();
      row.JobResultId = obj.Id;
      data.AddJobResultRow(row);

      return row;
    }

    public override IEnumerable<dsHiveServer.JobResultRow> FindById(Guid id) {
      return TransactionalAdapter.GetDataById(id);
    }

    public override IEnumerable<dsHiveServer.JobResultRow> FindAll() {
      return TransactionalAdapter.GetData();
    }

    public Stream GetSerializedJobResultStream(Guid jobResultId,
      bool useExistingConnection) {
      SqlConnection connection = null;
      SqlTransaction transaction = null;

      if (useExistingConnection) {
        connection =
          base.Session.Connection as SqlConnection;

        transaction =
          adapter.Transaction;
      } else {
        connection =
         ((SessionFactory)
           (base.Session.Factory)).CreateConnection()
           as SqlConnection;
      }

      VarBinarySource source =
        new VarBinarySource(
          connection, transaction,
          "JobResult", "JobResult", "JobResultId", jobResultId);

      return new VarBinaryStream(source);
    }

    protected override void SetConnection(DbConnection connection) {
      adapter.Connection = connection as SqlConnection;
    }

    protected override void SetTransaction(DbTransaction transaction) {
      adapter.Transaction = transaction as SqlTransaction;
    }
  }
}
