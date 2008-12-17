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
    #region Fields
    dsHiveServer.JobResultDataTable data =
        new dsHiveServer.JobResultDataTable();

    private IClientAdapter clientAdapter = null;

    private IClientAdapter ClientAdapter {
      get {
        if (clientAdapter == null)
          clientAdapter = ServiceLocator.GetClientAdapter();

        return clientAdapter;
      }
    }

    private IJobAdapter jobAdapter = null;

    private IJobAdapter JobAdapter {
      get {
        if (jobAdapter == null)
          jobAdapter = ServiceLocator.GetJobAdapter();

        return jobAdapter;
      }
    }
    #endregion

    #region Overrides
    protected override dsHiveServer.JobResultRow Convert(JobResult result,
      dsHiveServer.JobResultRow row) {
      if (row != null && result != null) {
        if (result.Job != null)
          row.JobId = result.Job.Id;
        else
          row.SetJobIdNull();

        if (result.Result != null)
          row.JobResult = result.Result;
        else
          row.SetJobResultNull();

        if (result.Client != null)
          row.ResourceId = result.Client.Id;
        else
          row.SetResourceIdNull();

        return row;
      } else
        return null;
    }

    protected override JobResult Convert(dsHiveServer.JobResultRow row, 
      JobResult result) {
      if (row != null && result != null) {
        result.Id = row.JobResultId;

        if (!row.IsJobIdNull())
          result.Job = JobAdapter.GetById(row.JobId);
        else
          result.Job = null;

        if (!row.IsJobResultNull())
          result.Result = row.JobResult;
        else
          result.Result = null;

        if (!row.IsResourceIdNull())
          result.Client = ClientAdapter.GetById(row.ResourceId);
        else
          result.Client = null;

        return result;
      } else
        return null;
    }

    protected override void UpdateRow(dsHiveServer.JobResultRow row) {
      adapter.Update(row);
    }

    protected override dsHiveServer.JobResultRow InsertNewRow(JobResult obj) {
      dsHiveServer.JobResultRow row = data.NewJobResultRow();
      data.AddJobResultRow(row);

      return row;
    }

    protected override IEnumerable<dsHiveServer.JobResultRow> FindById(long id) {
      return adapter.GetDataById(id);
    }

    protected override IEnumerable<dsHiveServer.JobResultRow> FindAll() {
      return adapter.GetData();
    }
    #endregion

    #region IJobResultsAdapter Members
    public override void Update(JobResult result) {
      if (result != null) {
        ClientAdapter.Update(result.Client);
        JobAdapter.Update(result.Job);

        base.Update(result);
      }
    }

    public ICollection<JobResult> GetResultsOf(Job job) {
      if (job != null) {
        return 
          base.FindMultiple(
            delegate() {
              return adapter.GetDataByJob(job.Id);
            });
      }

      return null;
    }
    #endregion
  }
}
