#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Data.Linq;
using System.Linq;

namespace HeuristicLab.Services.Hive.DataAccess.Daos.HiveStatistics {
  public class DimJobDao : GenericDao<Guid, DimJob> {
    public DimJobDao(DataContext dataContext) : base(dataContext) { }

    public override DimJob GetById(Guid id) {
      return GetByIdQuery(DataContext, id);
    }

    public IQueryable<DimJob> GetByUserId(Guid id) {
      return Table.Where(x => x.UserId == id);
    }

    public IQueryable<DimJob> GetNotCompletedJobs() {
      return Table.Where(x => x.DateCompleted == null);
    }

    public IQueryable<DimJob> GetCompletedJobs() {
      return Table.Where(x => x.DateCompleted != null);
    }

    public void UpdateExistingDimJobs() {
      DataContext.ExecuteCommand(UpdateExistingDimJobsQuery);
    }

    #region Compiled queries
    private static readonly Func<DataContext, Guid, DimJob> GetByIdQuery =
      CompiledQuery.Compile((DataContext db, Guid id) =>
        (from dimJob in db.GetTable<DimJob>()
         where dimJob.JobId == id
         select dimJob).SingleOrDefault());
    #endregion

    #region String queries
    private const string UpdateExistingDimJobsQuery = @"
UPDATE u
SET
  u.JobName = case when x.JobId is null then u.JobName else x.JobName end,
  u.TotalTasks = x.TotalTasks,
  u.CompletedTasks = x.CompletedTasks,
  u.DateCompleted =
    case when x.totaltasks = x.CompletedTasks
      then (case when x.JobId is null and x.DateCompleted is null then GETDATE() else x.DateCompleted end)
    else u.DateCompleted
  end,
  u.ProjectId = case when x.JobId is null then u.ProjectId else x.ProjectId end
FROM [statistics].dimjob u
JOIN (
	SELECT
	  dj.JobId as DimJobId,
	  j.JobId as JobId,
	  j.Name as JobName,
	  COUNT(*) as TotalTasks,
	  SUM(
	  CASE
		WHEN TaskState in ('Finished', 'Aborted', 'Failed') then 1
		ELSE 0
	  END) as CompletedTasks,
	  MAX(EndTime) as DateCompleted,
	  dp.ProjectId as ProjectId
	from [statistics].DimJob dj
	join [statistics].FactTask ft on dj.JobId = ft.JobId
	left join Job j on j.JobId = dj.JobId
	left join [statistics].DimProject dp on j.ProjectId = dp.ProjectId 
	where dj.DateCompleted is null and dp.DateExpired is null
	group by dj.JobId, j.JobId, j.Name, dp.ProjectId 
) as x on u.JobId = x.DimJobId";
    #endregion
  }
}
