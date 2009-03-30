#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Server.DataAccess;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using System.Linq.Expressions;
using HeuristicLab.DataAccess.ADOHelper;
using HeuristicLab.Hive.Server.ADODataAccess.dsHiveServerTableAdapters;
using System.Data.Common;
using System.Data.SqlClient;

namespace HeuristicLab.Hive.Server.ADODataAccess {
  class JobAdapterWrapper :
    DataAdapterWrapperBase<dsHiveServerTableAdapters.JobTableAdapter,
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

    protected override void SetConnection(DbConnection connection) {
      adapter.Connection = connection as SqlConnection;
    }

    protected override void SetTransaction(DbTransaction transaction) {
      adapter.Transaction = transaction as SqlTransaction;
    }
  }
  
  class JobAdapter :
    DataAdapterBase<dsHiveServerTableAdapters.JobTableAdapter,
                      Job, 
                      dsHiveServer.JobRow>, 
    IJobAdapter {
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

    private IJobResultsAdapter resultsAdapter = null;

    private IJobResultsAdapter ResultsAdapter {
      get {
        if (resultsAdapter == null) {
          resultsAdapter =
            this.Session.GetDataAdapter<JobResult, IJobResultsAdapter>();
        }

        return resultsAdapter;
      }
    }
    #endregion

    public JobAdapter(): base(new JobAdapterWrapper()) {
    }

    #region Overrides
    protected override Job ConvertRow(dsHiveServer.JobRow row,
      Job job) {
      if (row != null && job != null) {
        job.Id = row.JobId;

        if (!row.IsParentJobIdNull())
          job.ParentJob = GetById(row.ParentJobId);
        else
          job.ParentJob = null;

        if (!row.IsResourceIdNull())
          job.Client = ClientAdapter.GetById(row.ResourceId);
        else
          job.Client = null;

        if (!row.IsUserIdNull())
          job.UserId = Guid.Empty;
        else
          job.UserId = Guid.Empty;
        
        if (!row.IsJobStateNull())
          job.State = (State)Enum.Parse(job.State.GetType(), row.JobState);
        else
          job.State = State.nullState;

        if (!row.IsPercentageNull())
          job.Percentage = row.Percentage;
        else
          job.Percentage = 0.0;

        if (!row.IsSerializedJobNull())
          job.SerializedJob = row.SerializedJob;
        else
          job.SerializedJob = null;

        if (!row.IsDateCreatedNull())
          job.DateCreated = row.DateCreated;
        else
          job.DateCreated = DateTime.MinValue;

        if (!row.IsDateCalculatedNull())
          job.DateCalculated = row.DateCalculated;
        else
          job.DateCalculated = DateTime.MinValue;

        if (!row.IsPriorityNull())
          job.Priority = row.Priority;
        else
          job.Priority = default(int);

        return job;
      } else
        return null;
    }

    protected override dsHiveServer.JobRow ConvertObj(Job job,
      dsHiveServer.JobRow row) {
      if (job != null && row != null) {
        row.JobId = job.Id;
        
        if (job.Client != null) {
          if (row.IsResourceIdNull() ||
            row.ResourceId != job.Client.Id) {
            ClientAdapter.Update(job.Client);
            row.ResourceId = job.Client.Id;
          }
        } else
          row.SetResourceIdNull();

        if (job.ParentJob != null) {
          if (row.IsParentJobIdNull() ||
            row.ParentJobId != job.ParentJob.Id) {
            Update(job.ParentJob);
            row.ParentJobId = job.ParentJob.Id;
          }
        } else
          row.SetParentJobIdNull();

        if (job.UserId != Guid.Empty) {
          if (row.IsUserIdNull() ||
           row.UserId != Guid.Empty) {
            row.UserId = Guid.Empty;
          }
        } else
          row.SetUserIdNull();

        if (job.State != State.nullState)
          row.JobState = job.State.ToString();
        else
          row.SetJobStateNull();

        row.Percentage = job.Percentage;

        row.SerializedJob = job.SerializedJob;

        if (job.DateCreated != DateTime.MinValue)
          row.DateCreated = job.DateCreated;
        else
          row.SetDateCreatedNull();

        if (job.DateCalculated != DateTime.MinValue)
          row.DateCalculated = job.DateCalculated;
        else
          row.SetDateCalculatedNull();

        row.Priority = job.Priority;
      }

      return row;
    }
    #endregion

    #region IJobAdapter Members
    public ICollection<Job> GetAllSubjobs(Job job) {
      if (job != null) {
        return
          base.FindMultiple(
            delegate() {
              return Adapter.GetDataByParentJob(job.Id);
            });
      }

      return null;
    }

    public ICollection<Job> GetJobsByState(State state) {
      return
         base.FindMultiple(
           delegate() {
             return Adapter.GetDataByState(state.ToString());
           });
    }

    public ICollection<Job> GetJobsOf(ClientInfo client) {
      if (client != null) {
        return
          base.FindMultiple(
            delegate() {
              return Adapter.GetDataByClient(client.Id);
            });
      }

      return null;
    }

    public ICollection<Job> GetActiveJobsOf(ClientInfo client) {

      if (client != null) {
        return
          base.FindMultiple(
            delegate() {
              return Adapter.GetDataByCalculatingClient(client.Id);
            });
      }

      return null;
    }

    public ICollection<Job> GetJobsOf(Guid userId) {      
      return 
          base.FindMultiple(
            delegate() {
              return Adapter.GetDataByUser(userId);
            });
    }

    protected override bool doDelete(Job job) {
      if (job != null) {
        dsHiveServer.JobRow row =
          GetRowById(job.Id);

        if (row != null) {
          //Referential integrity with job results
          ICollection<JobResult> results =
            ResultsAdapter.GetResultsOf(job);

          foreach (JobResult result in results) {
            ResultsAdapter.Delete(result);
          }

          return base.doDelete(job);
        }
      }

      return false;
    }
    #endregion
  }
}
