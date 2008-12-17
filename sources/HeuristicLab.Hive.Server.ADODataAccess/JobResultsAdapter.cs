using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Server.Core.InternalInterfaces.DataAccess;
using HeuristicLab.Hive.Contracts.BusinessObjects;

namespace HeuristicLab.Hive.Server.ADODataAccess {
  class JobResultsAdapter: 
    DataAdapterBase<dsHiveServerTableAdapters.JobResultTableAdapter, 
                    JobResult, 
                    dsHiveServer.JobResultRow>,
    IJobResultsAdapter {
    #region Overrides
    protected override dsHiveServer.JobResultRow Convert(JobResult obj, dsHiveServer.JobResultRow row) {
      throw new NotImplementedException();
    }

    protected override JobResult Convert(dsHiveServer.JobResultRow row, JobResult obj) {
      throw new NotImplementedException();
    }

    protected override dsHiveServer.JobResultRow InsertNewRow(JobResult obj) {
      throw new NotImplementedException();
    }

    protected override void UpdateRow(dsHiveServer.JobResultRow row) {
      throw new NotImplementedException();
    }

    protected override IEnumerable<dsHiveServer.JobResultRow> FindById(long id) {
      throw new NotImplementedException();
    }

    protected override IEnumerable<dsHiveServer.JobResultRow> FindAll() {
      throw new NotImplementedException();
    }
    #endregion

    #region IJobResultsAdapter Members
    public override void Update(JobResult ob) {
      throw new NotImplementedException();
    }

    public override JobResult GetById(long id) {
      throw new NotImplementedException();
    }

    public override ICollection<JobResult> GetAll() {
      throw new NotImplementedException();
    }

    public override bool Delete(JobResult obj) {
      throw new NotImplementedException();
    }

    public ICollection<JobResult> GetResultsOf(Job job) {
      throw new NotImplementedException();
    }
    #endregion
  }
}
