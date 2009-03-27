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

namespace HeuristicLab.Hive.Server.ADODataAccess {
  class JobAdapter :
    CachedDataAdapter<dsHiveServerTableAdapters.JobTableAdapter,
                      Job, 
                      dsHiveServer.JobRow, 
                      dsHiveServer.JobDataTable>, 
    IJobAdapter {
    public JobAdapter() : 
      base(ServiceLocator.GetDBSynchronizer()) {
    }

    #region Fields
    private IClientAdapter clientAdapter = null;

    private IClientAdapter ClientAdapter {
      get {
        if (clientAdapter == null)
          clientAdapter = ServiceLocator.GetClientAdapter();

        return clientAdapter;
      }
    }

    private IJobResultsAdapter resultsAdapter = null;

    private IJobResultsAdapter ResultsAdapter {
      get {
        if (resultsAdapter == null) {
          resultsAdapter = ServiceLocator.GetJobResultsAdapter();
        }

        return resultsAdapter;
      }
    }
    #endregion

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

    protected override void UpdateRow(dsHiveServer.JobRow row) {
      Adapter.Update(row);
    }

    protected override dsHiveServer.JobRow 
      InsertNewRow(Job job) {
      dsHiveServer.JobDataTable data =
        new dsHiveServer.JobDataTable();

      dsHiveServer.JobRow row = data.NewJobRow();
      row.JobId = job.Id;
      data.AddJobRow(row);

      return row;
    }

    protected override dsHiveServer.JobRow 
      InsertNewRowInCache(Job job) {
      dsHiveServer.JobRow row = cache.NewJobRow();
      row.JobId = job.Id;
      cache.AddJobRow(row);

      return row;
    }

    protected override void FillCache() {
      Adapter.FillByActive(cache);
    }

    protected override void SynchronizeWithDb() { 
      this.Adapter.Update(cache);
    }

    protected override bool PutInCache(Job job) {
      return job != null
        && (job.State == State.calculating
            || job.State == State.idle);
    }

    protected override IEnumerable<dsHiveServer.JobRow>
      FindById(Guid id) {
      return Adapter.GetDataById(id);
    }

    protected override dsHiveServer.JobRow
      FindCachedById(Guid id) {
      return cache.FindByJobId(id);
    }

    protected override IEnumerable<dsHiveServer.JobRow>
      FindAll() {
      return FindMultipleRows(
        new Selector(Adapter.GetData),
        new Selector(cache.AsEnumerable<dsHiveServer.JobRow>));
    }

    #endregion

    #region IJobAdapter Members
    public ICollection<Job> GetAllSubjobs(Job job) {
      if (job != null) {
        return
          base.FindMultiple(
            delegate() {
              return Adapter.GetDataByParentJob(job.Id);
            },
            delegate() {
              return from j in
                   cache.AsEnumerable<dsHiveServer.JobRow>()
                 where  !j.IsParentJobIdNull() && 
                        j.ParentJobId == job.Id
                 select j;
            });
      }

      return null;
    }

    public ICollection<Job> GetJobsByState(State state) {
      return
         base.FindMultiple(
           delegate() {
             return Adapter.GetDataByState(state.ToString());
           },
           delegate() {
             return from job in
                      cache.AsEnumerable<dsHiveServer.JobRow>()
                    where !job.IsJobStateNull() &&
                           job.JobState == state.ToString()
                    select job;
           });
    }

    public ICollection<Job> GetJobsOf(ClientInfo client) {
      if (client != null) {
        return
          base.FindMultiple(
            delegate() {
              return Adapter.GetDataByClient(client.Id);
            },
            delegate() {
              return from job in
                 cache.AsEnumerable<dsHiveServer.JobRow>()
               where !job.IsResourceIdNull() && 
                      job.ResourceId == client.Id
               select job;
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
            },
            delegate() {
              return from job in
                       cache.AsEnumerable<dsHiveServer.JobRow>()
                     where !job.IsResourceIdNull() &&
                            job.ResourceId == client.Id && 
                           !job.IsJobStateNull() && 
                            job.JobState == "calculating"
                     select job;
            });
      }

      return null;
    }

    public ICollection<Job> GetJobsOf(Guid userId) {
      throw new NotImplementedException();  
      
      return 
          base.FindMultiple(
            delegate() {
              return Adapter.GetDataByUser(userId);
            },
            delegate() {
              return from job in
                cache.AsEnumerable<dsHiveServer.JobRow>()
              where !job.IsUserIdNull() &&
                job.UserId == Guid.Empty
              select job;
            });
    }

    public override bool Delete(Job job) {
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

          return base.Delete(job);
        }
      }

      return false;
    }
    #endregion
  }
}
