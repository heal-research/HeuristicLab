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
using HeuristicLab.Hive.Server.Core.InternalInterfaces.DataAccess;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace HeuristicLab.Hive.Server.ADODataAccess {
  class JobAdapter :
    CachedDataAdapter<dsHiveServerTableAdapters.JobTableAdapter,
                      Job, 
                      dsHiveServer.JobRow, 
                      dsHiveServer.JobDataTable>, 
    IJobAdapter {
    #region Fields
    dsHiveServer.JobDataTable data =
        new dsHiveServer.JobDataTable();

    private IClientAdapter clientAdapter = null;

    private IClientAdapter ClientAdapter {
      get {
        if (clientAdapter == null)
          clientAdapter = ServiceLocator.GetClientAdapter();

        return clientAdapter;
      }
    }

    private IUserAdapter userAdapter = null;

    private IUserAdapter UserAdapter {
      get {
        if (userAdapter == null) {
          userAdapter = ServiceLocator.GetUserAdapter();
        }

        return userAdapter;
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
    protected override Job Convert(dsHiveServer.JobRow row,
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
        
        if (!row.IsStatusNull())
          job.State = (State)Enum.Parse(job.State.GetType(), row.Status);
        else
          job.State = State.nullState;

        return job;
      } else
        return null;
    }

    protected override dsHiveServer.JobRow Convert(Job job,
      dsHiveServer.JobRow row) {
      if (job != null && row != null) {
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

        if (job.State != State.nullState)
          row.Status = job.State.ToString();
        else
          row.SetStatusNull();
      }

      return row;
    }

    protected override void UpdateRow(dsHiveServer.JobRow row) {
      adapter.Update(row);
    }

    protected override dsHiveServer.JobRow 
      InsertNewRow(Job job) {      
      dsHiveServer.JobRow row = data.NewJobRow();
      data.AddJobRow(row);

      return row;
    }

    protected override dsHiveServer.JobRow 
      InsertNewRowInCache(Job job) {
      dsHiveServer.JobRow row = cache.NewJobRow();
      cache.AddJobRow(row);

      return row;
    }

    protected override void FillCache() {
      adapter.FillByActive(cache);
    }

    public override void SyncWithDb() { 
      this.adapter.Update(this.cache);
    }

    protected override bool PutInCache(Job job) {
      return job != null
        && (job.State == State.calculating
            || job.State == State.idle);
    }

    protected override IEnumerable<dsHiveServer.JobRow>
      FindById(long id) {
      return adapter.GetDataById(id);
    }

    protected override dsHiveServer.JobRow
      FindCachedById(long id) {
      return cache.FindByJobId(id);
    }

    protected override IEnumerable<dsHiveServer.JobRow>
      FindAll() {
      return FindMultipleRows(
        new Selector(adapter.GetData),
        new Selector(cache.AsEnumerable<dsHiveServer.JobRow>));
    }

    #endregion

    #region IJobAdapter Members
    public ICollection<Job> GetAllSubjobs(Job job) {
      if (job != null) {
        return
          base.FindMultiple(
            delegate() {
              return adapter.GetDataBySubjobs(job.Id);
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
             return adapter.GetDataByState(state.ToString());
           },
           delegate() {
             return from job in
                      cache.AsEnumerable<dsHiveServer.JobRow>()
                    where !job.IsStatusNull() &&
                           job.Status == state.ToString()
                    select job;
           });
    }

    public ICollection<Job> GetJobsOf(ClientInfo client) {
      if (client != null) {
        return
          base.FindMultiple(
            delegate() {
              return adapter.GetDataByClient(client.Id);
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

    public ICollection<Job> GetJobsOf(User user) {
      if (user != null) {
        return 
          base.FindMultiple(
            delegate() {
              return adapter.GetDataByUser(user.Id);
            },
            delegate() {
              return from job in
                cache.AsEnumerable<dsHiveServer.JobRow>()
              where !job.IsPermissionOwnerIdNull() &&
                job.PermissionOwnerId == user.Id
              select job;
            });
      }

      return null;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
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
