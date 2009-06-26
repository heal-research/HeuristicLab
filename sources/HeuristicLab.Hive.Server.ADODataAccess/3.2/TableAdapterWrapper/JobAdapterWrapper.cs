using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.DataAccess.ADOHelper;
using System.Data.SqlClient;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using System.Data.Common;
using System.IO;
using System.Data.SqlTypes;
using System.Data;

namespace HeuristicLab.Hive.Server.ADODataAccess.TableAdapterWrapper {
  class JobAdapterWrapper :
    TableAdapterWrapperBase<dsHiveServerTableAdapters.JobTableAdapter,
                      Job,
                      dsHiveServer.JobRow> {
    public override void UpdateRow(dsHiveServer.JobRow row) {
      TransactionalAdapter.Update(row);
    }

    public override dsHiveServer.JobRow
      InsertNewRow(Job job) {
      dsHiveServer.JobDataTable data =
        new dsHiveServer.JobDataTable();

      dsHiveServer.JobRow row = data.NewJobRow();
      row.JobId = job.Id;
      data.AddJobRow(row);

      return row;
    }

    public override IEnumerable<dsHiveServer.JobRow>
      FindById(Guid id) {
      return TransactionalAdapter.GetDataById(id);
    }

    public override IEnumerable<dsHiveServer.JobRow>
      FindAll() {
      return TransactionalAdapter.GetData();
    }

    public Stream GetSerializedJobStream(Guid jobId, 
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
          "Job", "SerializedJob", "JobId", jobId);

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
