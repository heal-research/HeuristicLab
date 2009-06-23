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
using HeuristicLab.Hive.Server.ADODataAccess.TableAdapterWrapper;

namespace HeuristicLab.Hive.Server.ADODataAccess {
  class JobAdapter :
    DataAdapterBase<dsHiveServerTableAdapters.JobTableAdapter,
                      Job, 
                      dsHiveServer.JobRow>, 
    IJobAdapter {
    #region Fields
    private ManyToManyRelationHelper<
      dsHiveServerTableAdapters.RequiredPluginsTableAdapter,
      dsHiveServer.RequiredPluginsRow> pluginsManyToManyRelationHelper = null;

    private ManyToManyRelationHelper<
      dsHiveServerTableAdapters.RequiredPluginsTableAdapter,
      dsHiveServer.RequiredPluginsRow> PluginsManyToManyRelationHelper {
      get {
        if (pluginsManyToManyRelationHelper == null) {
          pluginsManyToManyRelationHelper =
            new ManyToManyRelationHelper<dsHiveServerTableAdapters.RequiredPluginsTableAdapter,
              dsHiveServer.RequiredPluginsRow>(new RequiredPluginsAdapterWrapper(), 1);
        }

        pluginsManyToManyRelationHelper.Session = Session as Session;

        return pluginsManyToManyRelationHelper;
      }
    }

    private ManyToManyRelationHelper<
      dsHiveServerTableAdapters.AssignedResourcesTableAdapter,
      dsHiveServer.AssignedResourcesRow> assignedManyToManyRelationHelper = null;

    private ManyToManyRelationHelper<
      dsHiveServerTableAdapters.AssignedResourcesTableAdapter,
      dsHiveServer.AssignedResourcesRow> AssignedManyToManyRelationHelper {
      get {
        if (assignedManyToManyRelationHelper == null) {
          assignedManyToManyRelationHelper =
            new ManyToManyRelationHelper<dsHiveServerTableAdapters.AssignedResourcesTableAdapter,
              dsHiveServer.AssignedResourcesRow>(new AssignedResourcesAdapterWrapper(), 0);
        }

        assignedManyToManyRelationHelper.Session = Session as Session;

        return assignedManyToManyRelationHelper;
      }
    }

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

    private IPluginInfoAdapter pluginInfoAdapter = null;

    private IPluginInfoAdapter PluginInfoAdapter {
      get {
        if (pluginInfoAdapter == null) {
          pluginInfoAdapter =
            this.Session.GetDataAdapter<HivePluginInfo, IPluginInfoAdapter>();
        }

        return pluginInfoAdapter;
      }
    }

    private IProjectAdapter projectAdapter = null;

    private IProjectAdapter ProjectAdapter {
      get {
        if (projectAdapter == null) {
          projectAdapter =
            this.Session.GetDataAdapter<Project, IProjectAdapter>();
        }

        return projectAdapter;
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

        if (!row.IsCoresNeededNull())
          job.CoresNeeded = row.CoresNeeded;
        else
          job.CoresNeeded = default(int);

        if (!row.IsMemoryNeededNull())
          job.MemoryNeeded = row.MemoryNeeded;
        else
          job.MemoryNeeded = default(int);

        if (!row.IsProjectIdNull())
          job.Project = ProjectAdapter.GetById(
            row.ProjectId);
        else
          job.Project = null;

        ICollection<Guid> requiredPlugins =
          PluginsManyToManyRelationHelper.GetRelationships(job.Id);
        
        job.PluginsNeeded.Clear();
        foreach (Guid requiredPlugin in requiredPlugins) {
          HivePluginInfo pluginInfo = 
            PluginInfoAdapter.GetById(requiredPlugin);

          job.PluginsNeeded.Add(pluginInfo);
        }

        job.AssignedResourceIds =
          new List<Guid>(
            AssignedManyToManyRelationHelper.GetRelationships(job.Id));

        return job;
      } else
        return null;
    }

    protected override dsHiveServer.JobRow ConvertObj(Job job,
      dsHiveServer.JobRow row) {
      if (job != null && row != null) {
        row.JobId = job.Id;
        
        if (job.Client != null) {
          if (row.IsResourceIdNull()) {
            row.ResourceId = job.Client.Id;
          }
        } else
          row.SetResourceIdNull();

        if (job.ParentJob != null) {
          if (row.IsParentJobIdNull()) {
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

        if (job.DateCreated != DateTime.MinValue)
          row.DateCreated = job.DateCreated;
        else
          row.SetDateCreatedNull();

        if (job.DateCalculated != DateTime.MinValue)
          row.DateCalculated = job.DateCalculated;
        else
          row.SetDateCalculatedNull();

        row.Priority = job.Priority;

        row.CoresNeeded = job.CoresNeeded;

        row.MemoryNeeded = job.MemoryNeeded;

        if (job.Project != null)
          row.ProjectId = job.Project.Id;
        else
          row.SetProjectIdNull();
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

    public ICollection<Job> FindJobs(State state, int cores, int memory,
      Guid resourceId) {
      return
         base.FindMultiple(
           delegate() {
             return Adapter.GetDataByStateCoresMemoryResource(
               state.ToString(), 
               cores,
               memory, 
               resourceId);
           });
    }

    public ICollection<Job> GetJobsByProject(Guid projectId) {
      return
         base.FindMultiple(
           delegate() {
             return Adapter.GetDataByProjectId(projectId);
           });
    }

    protected override void doUpdate(Job obj) {
      if (obj != null) {
        ProjectAdapter.Update(obj.Project);
        ClientAdapter.Update(obj.Client);
        Update(obj.ParentJob);

        base.doUpdate(obj);

        //update relationships
        List<Guid> relationships =
          new List<Guid>();
        foreach (HivePluginInfo pluginInfo in obj.PluginsNeeded) {
          //first check if pluginInfo already exists in the db
          HivePluginInfo found = PluginInfoAdapter.GetByNameVersionBuilddate(
            pluginInfo.Name, pluginInfo.Version, pluginInfo.BuildDate);
          if (found != null) {
            pluginInfo.Id = found.Id;
          }

          PluginInfoAdapter.Update(pluginInfo);
          relationships.Add(pluginInfo.Id);
        }

        PluginsManyToManyRelationHelper.UpdateRelationships(
          obj.Id, relationships);

        AssignedManyToManyRelationHelper.UpdateRelationships(
          obj.Id, obj.AssignedResourceIds);
      }
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

          //delete all relationships
          PluginsManyToManyRelationHelper.UpdateRelationships(job.Id,
            new List<Guid>());

          //delete orphaned pluginInfos
          ICollection<HivePluginInfo> orphanedPluginInfos =
             PluginInfoAdapter.GetOrphanedPluginInfos();
          foreach(HivePluginInfo orphanedPlugin in orphanedPluginInfos) {
            PluginInfoAdapter.Delete(orphanedPlugin);
          }

          return base.doDelete(job);
        }
      }

      return false;
    }

    /// <summary>
    /// Gets the computable job with the secified jobId
    /// </summary>
    /// <param name="jobId"></param>
    /// <returns></returns>
    public ComputableJob GetComputableJob(Guid jobId) {
      return (ComputableJob)base.doInTransaction(
        delegate() {
          ComputableJob job =
            new ComputableJob();

          job.JobInfo = GetById(jobId);
          if (job.JobInfo != null) {
            job.SerializedJob =
              base.Adapter.GetSerializedJobById(jobId);

            return job;
          } else {
            return null;
          }
        });
    }

    /// <summary>
    /// Saves or update the computable job
    /// </summary>
    /// <param name="jobId"></param>
    public void UpdateComputableJob(ComputableJob job) {
      if(job != null && 
        job.JobInfo != null) {
        base.doInTransaction(
          delegate() {
            dsHiveServer.JobRow row
              = GetRowById(job.JobInfo.Id);

            if (row != null) {
              Update(job.JobInfo);
              return base.Adapter.UpdateSerializedJob(
                job.SerializedJob, job.JobInfo.Id);
            }

            return false;
          });
      }
      
    }

    #endregion
  }
}
