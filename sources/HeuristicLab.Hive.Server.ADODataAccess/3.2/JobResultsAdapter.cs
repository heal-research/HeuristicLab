using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Server.DataAccess;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.DataAccess.ADOHelper;
using HeuristicLab.Hive.Server.ADODataAccess.dsHiveServerTableAdapters;
using System.Data.Common;
using System.Data.SqlClient;
using HeuristicLab.Hive.Server.ADODataAccess.TableAdapterWrapper;

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
          clientAdapter =
            this.Session.GetDataAdapter<ClientInfo, IClientAdapter>();

        return clientAdapter;
      }
    }

    private IJobAdapter jobAdapter = null;

    private IJobAdapter JobAdapter {
      get {
        if (jobAdapter == null)
          jobAdapter =
            this.Session.GetDataAdapter<Job, IJobAdapter>();

        return jobAdapter;
      }
    }
    #endregion

    public JobResultsAdapter(): base(new JobResultsAdapterWrapper()) {
    }

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
    #endregion

    #region IJobResultsAdapter Members
    protected override void doUpdate(JobResult result) {
      if (result != null) {
        ClientAdapter.Update(result.Client);
        JobAdapter.Update(result.Job);

        base.doUpdate(result);
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
