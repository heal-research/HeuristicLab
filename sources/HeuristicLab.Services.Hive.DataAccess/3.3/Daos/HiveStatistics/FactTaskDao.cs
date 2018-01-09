#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Services.Hive.DataAccess.Daos.HiveStatistics {
  public class FactTaskDao : GenericDao<Guid, FactTask> {

    private static readonly TaskState[] CompletedStates = { TaskState.Finished, TaskState.Aborted, TaskState.Failed };

    private Table<DimClient> DimClientTable {
      get { return DataContext.GetTable<DimClient>(); }
    }
    private Table<DimJob> DimJobTable {
      get { return DataContext.GetTable<DimJob>(); }
    }

    public FactTaskDao(DataContext dataContext) : base(dataContext) { }

    public override FactTask GetById(Guid id) {
      return GetByIdQuery(DataContext, id);
    }

    public IQueryable<FactTask> GetNotFinishedTasks() {
      return Table.Where(x => x.EndTime == null);
    }

    public IQueryable<FactTask> GetByJobId(Guid id) {
      return Table.Where(x => x.JobId == id);
    }

    public DateTime? GetLastCompletedTaskFromJob(Guid id) {
      return GetLastCompletedTaskFromJobQuery(DataContext, id);
    }

    public IQueryable<FactTask> GetByClientId(Guid id) {
      return Table.Where(x => x.LastClientId == id);
    }

    public IQueryable<FactTask> GetTasksWithException() {
      return Table.Where(x => x.Exception.Trim() != string.Empty);
    }

    public IQueryable<FactTask> GetByGroupId(Guid id) {
      return from factTask in Table
             join client in DimClientTable on factTask.LastClientId equals client.Id
             where client.ResourceGroupId == id
             select factTask;
    }

    public IQueryable<FactTask> GetByUserId(Guid id) {
      return from factTask in Table
             join job in DimJobTable on factTask.JobId equals job.JobId
             where job.UserId == id
             select factTask;
    }

    public IQueryable<FactTask> GetCompletedTasks() {
      return Table.Where(x => CompletedStates.Contains(x.TaskState));
    }

    public override void Delete(IEnumerable<Guid> ids) {
      string paramIds = string.Join(",", ids.Select(x => string.Format("'{0}'", x)));
      if (!string.IsNullOrWhiteSpace(paramIds)) {
        string query = string.Format(BatchDeleteQuery, paramIds);
        DataContext.ExecuteCommand(query);
      }
    }

    public void DeleteByJobId(Guid jobId) {
      DataContext.ExecuteCommand(DeleteByJobIdQuery, jobId);
    }

    #region Compiled queries
    private static readonly Func<DataContext, Guid, FactTask> GetByIdQuery =
      CompiledQuery.Compile((DataContext db, Guid id) =>
        (from factTask in db.GetTable<FactTask>()
         where factTask.TaskId == id
         select factTask).SingleOrDefault());


    private static readonly Func<DataContext, Guid, DateTime?> GetLastCompletedTaskFromJobQuery =
      CompiledQuery.Compile((DataContext db, Guid id) =>
        (from factTask in db.GetTable<FactTask>()
         where factTask.JobId == id && factTask.EndTime != null
         select factTask.EndTime).Max());
    #endregion

    #region String queries
    private const string BatchDeleteQuery =
      @"DELETE FROM [statistics].[FactTask]
         WHERE TaskId IN ({0});";

    private const string DeleteByJobIdQuery =
      @"DELETE FROM [statistics].[FactTask]
         WHERE JobId = {0};";
    #endregion
  }
}
