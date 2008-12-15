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
  class JobAdapter : DataAdapterBase, IJobAdapter {
    private dsHiveServerTableAdapters.JobTableAdapter adapter =
        new dsHiveServerTableAdapters.JobTableAdapter();

    private dsHiveServer.JobDataTable data =
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

    public JobAdapter() {
      adapter.Fill(data);
    }

    protected override void Update() {
      this.adapter.Update(this.data);
    }

    private Job Convert(dsHiveServer.JobRow row,
      Job job) {
      if (row != null && job != null) {
        job.JobId = row.JobId;

        if (!row.IsParentJobIdNull())
          job.ParentJob = GetJobById(row.ParentJobId);
        else
          job.ParentJob = null;

        if (!row.IsResourceIdNull())
          job.Client = ClientAdapter.GetClientById(row.ResourceId);
        else
          job.Client = null;
        
        if (!row.IsStatusNull())
          job.State = (State)Enum.Parse(job.State.GetType(), row.Status);
        else
          job.State = State.idle;

        return job;
      } else
        return null;
    }

    private dsHiveServer.JobRow Convert(Job job,
      dsHiveServer.JobRow row) {
      if (job != null && row != null) {
        if (job.Client != null) {
          ClientAdapter.UpdateClient(job.Client);
          row.ResourceId = job.Client.ResourceId;
        } else
          row.SetResourceIdNull();

        if (job.ParentJob != null) {
          UpdateJob(job.ParentJob);
          row.ParentJobId = job.ParentJob.JobId;
        } else
          row.SetParentJobIdNull();

        row.Status = job.State.ToString();
      }

      return row;
    }

    #region IJobAdapter Members
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void UpdateJob(Job job) {
      if (job != null) {
        dsHiveServer.JobRow row =
          data.FindByJobId(job.JobId);

        if (row == null) {
          row = data.NewJobRow();
          data.AddJobRow(row);

          //write row to db to get primary key
          adapter.Update(row);
        }

        Convert(job, row);
        job.JobId = row.JobId;
      }
    }

    public Job GetJobById(long id) {
      dsHiveServer.JobRow row =
        data.FindByJobId(id);

      if (row != null) {
        Job job = new Job();
        
        Convert(row, job);

        return job;
      }

      return null;
    }

    public ICollection<Job> GetAllJobs() {
      IList<Job> allJobs =
        new List<Job>();

      foreach (dsHiveServer.JobRow row in data) {
        Job job = new Job();
        Convert(row, job);
        allJobs.Add(job);
      }

      return allJobs;
    }

    public ICollection<Job> GetAllSubjobs(Job job) {
      IList<Job> allJobs =
        new List<Job>();

      if (job != null) {
        IEnumerable<dsHiveServer.JobRow> clientJobs =
         from j in
           data.AsEnumerable<dsHiveServer.JobRow>()
         where j.ParentJobId == job.JobId
         select j;

        foreach (dsHiveServer.JobRow row in
          clientJobs) {
          Job j = new Job();
          Convert(row, j);
          allJobs.Add(j);
        }
      }

      return allJobs;
    }

    public JobResult GetResult(Job job) {
      throw new NotImplementedException();
    }

    public ICollection<Job> GetJobsOf(ClientInfo client) {
      IList<Job> allJobs =
        new List<Job>();

      if (client != null) {
        IEnumerable<dsHiveServer.JobRow> clientJobs =
         from job in
           data.AsEnumerable<dsHiveServer.JobRow>()
         where !job.IsResourceIdNull() && 
          job.ResourceId == client.ResourceId
         select job;

        foreach (dsHiveServer.JobRow row in
          clientJobs) {
          Job job = new Job();
          Convert(row, job);
          allJobs.Add(job);
        }
      }

      return allJobs;
    }

    public ICollection<Job> GetJobsOf(User user) {
      IList<Job> allJobs =
        new List<Job>();

      if (user != null) {
        IEnumerable<dsHiveServer.JobRow> userJobs =
        from job in
          data.AsEnumerable<dsHiveServer.JobRow>()
        where
          !job.IsPermissionOwnerIdNull() &&
          job.PermissionOwnerId == user.PermissionOwnerId
        select job;

        foreach (dsHiveServer.JobRow row in
          userJobs) {
          Job job = new Job();
          Convert(row, job);
          allJobs.Add(job);
        }
      }

      return allJobs;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool DeleteJob(Job job) {
      if (job != null) {
        dsHiveServer.JobRow row =
          data.FindByJobId(job.JobId);

        if (row != null) {
          row.Delete();
          adapter.Update(row);

          return true;
        }
      }

      return false;
    }

    #endregion
  }
}
