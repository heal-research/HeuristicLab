using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.DataAccess.ADOHelper;
using System.Data.SqlClient;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using System.Data.Common;

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

    public byte[] GetSerializedJob(Guid jobId) {
      return TransactionalAdapter.GetSerializedJobById(jobId);
    }

    public bool UpdateSerialiedJob(byte[] serializedJob, Guid jobId) {
      return TransactionalAdapter.UpdateSerializedJob(serializedJob, jobId) > 0;
    }

    protected override void SetConnection(DbConnection connection) {
      adapter.Connection = connection as SqlConnection;
    }

    protected override void SetTransaction(DbTransaction transaction) {
      adapter.Transaction = transaction as SqlTransaction;
    }
  }
}
