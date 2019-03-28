#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Data.Linq;
using System.Linq;

namespace HeuristicLab.Services.Hive.DataAccess.Daos {
  public class JobDao : GenericDao<Guid, Job> {
    public JobDao(DataContext dataContext) : base(dataContext) { }

    public override Job GetById(Guid id) {
      return GetByIdQuery(DataContext, id);
    }

    public void DeleteByState(JobState state) {
      DataContext.ExecuteCommand(DeleteByStateQueryString, Enum.GetName(typeof(JobState), state));
    }

    public IEnumerable<Job> GetByProjectId(Guid id) {
      return GetByProjectIdQuery(DataContext, id);
    }

    public IEnumerable<Job> GetByProjectIds(IEnumerable<Guid> projectIds) {
      string paramProjectIds = string.Join(",", projectIds.ToList().Select(x => string.Format("'{0}'", x)));
      if(!string.IsNullOrWhiteSpace(paramProjectIds)) {
        string queryString = string.Format(GetByProjectIdsQueryString, paramProjectIds);
        return DataContext.ExecuteQuery<Job>(queryString);
      }
      return Enumerable.Empty<Job>();
    }

    public IEnumerable<Job> GetByState(JobState state) {
      return GetByStateQuery(DataContext, state);
    }

    public IEnumerable<Guid> GetJobIdsByState(JobState state) {
      return GetJobIdsByStateQuery(DataContext, state);
    }

    public IEnumerable<Job> GetJobsReadyForDeletion() {
      return GetJobsReadyForDeletionQuery(DataContext);
    }


    #region Compiled queries
    private static readonly Func<DataContext, Guid, Job> GetByIdQuery =
      CompiledQuery.Compile((DataContext db, Guid jobId) =>
        (from job in db.GetTable<Job>()
         where job.JobId == jobId
         select job).SingleOrDefault());
    private static readonly Func<DataContext, Guid, IEnumerable<Job>> GetByProjectIdQuery =
      CompiledQuery.Compile((DataContext db, Guid projectId) =>
        (from job in db.GetTable<Job>()
         where job.ProjectId == projectId
         select job));
    private static readonly Func<DataContext, JobState, IEnumerable<Job>> GetByStateQuery =
      CompiledQuery.Compile((DataContext db, JobState jobState) =>
        (from job in db.GetTable<Job>()
         where job.State == jobState
         select job));
    private static readonly Func<DataContext, JobState, IEnumerable<Guid>> GetJobIdsByStateQuery =
      CompiledQuery.Compile((DataContext db, JobState jobState) =>
        (from job in db.GetTable<Job>()
         where job.State == jobState
         select job.JobId));
    private static readonly Func<DataContext, IEnumerable<Job>> GetJobsReadyForDeletionQuery =
      CompiledQuery.Compile((DataContext db) =>
        (from job in db.GetTable<Job>()
         where job.State == JobState.StatisticsPending
               && (from task in db.GetTable<Task>()
                   where task.JobId == job.JobId
                   select task).All(x => x.State == TaskState.Finished
                                      || x.State == TaskState.Aborted
                                      || x.State == TaskState.Failed)
         select job));
    #endregion

    #region String queries
    private const string DeleteByStateQueryString = @"
      DELETE FROM [Job]
      WHERE JobState = {0}
    ";
    private const string GetStatisticsPendingJobs = @"
      SELECT DISTINCT j.*
      FROM [Job] j
      WHERE j.JobState = 'StatisticsPending'
      AND 'Calculating' NOT IN (
        SELECT t.TaskState 
        FROM [Task] t 
        WHERE t.JobId = j.JobId)
    ";
    private const string GetByProjectIdsQueryString = @"
      SELECT DISTINCT j.*
      FROM [Job] j
      WHERE j.ProjectId IN ({0})
    ";
    #endregion
  }
}
