using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Server.DataAccess;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.DataAccess.ADOHelper;

namespace HeuristicLab.Hive.Server.ADODataAccess {
  class JobResultsAdapter: 
    DataAdapterBase<dsHiveServerTableAdapters.JobResultTableAdapter, 
                    JobResult, 
                    dsHiveServer.JobResultRow>,
    IJobResultsAdapter {
    #region Fields
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
    protected override dsHiveServer.JobResultRow ConvertObj(JobResult result,
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

        if (result.Client != null)  {
          ClientInfo client = 
                 ClientAdapter.GetById(result.Client.Id);

          if (client != null)
            row.ResourceId = client.Id;
          else
            row.SetResourceIdNull();
        }           
        else
          row.SetResourceIdNull();

        if (result.Exception != null)
          row.Message = result.Exception.ToString();
        else
          row.SetMessageNull();

        row.Percentage = result.Percentage;

        if (result.DateFinished != DateTime.MinValue)
          row.DateFinished = result.DateFinished;
        else
          row.SetDateFinishedNull();

        return row;
      } else
        return null;
    }

    protected override JobResult ConvertRow(dsHiveServer.JobResultRow row, 
      JobResult result) {
      if (row != null && result != null) {
        row.JobResultId = result.Id;
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

        if (!row.IsMessageNull())
          result.Exception = new Exception(row.Message);
        else
          result.Exception = null;

        result.Percentage = row.Percentage;

        if (!row.IsDateFinishedNull())
          result.DateFinished = row.DateFinished;
        else
          result.DateFinished = DateTime.MinValue;

        return result;
      } else
        return null;
    }

    protected override void UpdateRow(dsHiveServer.JobResultRow row) {
      Adapter.Update(row);
    }

    protected override dsHiveServer.JobResultRow InsertNewRow(JobResult obj) {
      dsHiveServer.JobResultDataTable data =
        new dsHiveServer.JobResultDataTable();

      dsHiveServer.JobResultRow row = data.NewJobResultRow();
      row.JobResultId = obj.Id;
      data.AddJobResultRow(row);

      return row;
    }

    protected override IEnumerable<dsHiveServer.JobResultRow> FindById(Guid id) {
      return Adapter.GetDataById(id);
    }

    protected override IEnumerable<dsHiveServer.JobResultRow> FindAll() {
      return Adapter.GetData();
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
              return Adapter.GetDataByJob(job.Id);
            });
      }

      return null;
    }
    #endregion
  }
}
