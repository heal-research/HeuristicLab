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
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace HeuristicLab.Services.Hive.DataAccess.Daos {
  public class TaskDao : GenericDao<Guid, Task> {
    public TaskDao(DataContext dataContext) : base(dataContext) { }

    public override Task GetById(Guid id) {
      return GetByIdQuery(DataContext, id);
    }

    public IQueryable<Task> GetAllChildTasks() {
      return Table.Where(x => !x.IsParentTask);
    }

    public IQueryable<Task> GetByJobId(Guid id) {
      return Table.Where(x => x.JobId == id);
    }
    public class TaskPriorityInfo {
      public Guid JobId { get; set; }
      public Guid TaskId { get; set; }
      public int Priority { get; set; }
    }

    public IEnumerable<TaskPriorityInfo> GetWaitingTasks(Slave slave) {
      //Originally we checked here if there are parent tasks which should be calculated (with GetParentTasks(resourceIds, count, false);).
      //Because there is at the moment no case where this makes sense (there don't exist parent tasks which need to be calculated), 
      //we skip this step because it's wasted runtime
      return DataContext.ExecuteQuery<TaskPriorityInfo>(GetWaitingTasksQueryString, 
        slave.ResourceId, 
        Enum.GetName(typeof(TaskState), TaskState.Waiting), 
        slave.FreeCores, 
        slave.FreeMemory).ToList();
    }

    /// <summary>
    /// returns all parent tasks which are waiting for their child tasks to finish
    /// </summary>
    /// <param name="resourceIds">list of resourceids which for which the task should be valid</param>
    /// <param name="count">maximum number of task to return</param>
    /// <param name="finished">if true, all parent tasks which have FinishWhenChildJobsFinished=true are returned, otherwise only FinishWhenChildJobsFinished=false are returned</param>
    /// <returns></returns>
    public IEnumerable<Task> GetParentTasks(IEnumerable<Guid> resourceIds, int count, bool finished) {
    var query = from t in Table
                where t.State == TaskState.Waiting
                    && t.IsParentTask
                    && t.Job.AssignedJobResources.All(x => resourceIds.ToList().Contains(x.ResourceId))
                    && t.FinishWhenChildJobsFinished == finished
                    && t.ChildJobs.Any()
                    && t.ChildJobs.All(x =>
                      x.State == TaskState.Finished
                      || x.State == TaskState.Aborted
                      || x.State == TaskState.Failed)
                  orderby t.Priority descending
                  select t;
      return count == 0 ? query.ToArray() : query.Take(count).ToArray();
    }

    public void UpdateExecutionTime(Guid taskId, double executionTime) {
      DataContext.ExecuteCommand(UpdateExecutionTimeQuery, executionTime, DateTime.Now, taskId);
    }

    #region Compiled queries
    private static readonly Func<DataContext, Guid, Task> GetByIdQuery =
      CompiledQuery.Compile((DataContext db, Guid taskId) =>
        (from task in db.GetTable<Task>()
         where task.TaskId == taskId
         select task).SingleOrDefault());
    #endregion

    #region String queries
    private const string GetCalculatingChildTasksByProjectId = @"
      SELECT t.* FROM [Task] t, [Job] j
        WHERE t.IsParentTask = 0
        AND t.TaskState = 'Calculating'
        AND t.JobId = j.JobId
        AND j.ProjectId = {0}
        ORDER BY j.ProjectId
    ";
    private const string GetWaitingTasksQueryString = @"
      WITH rbranch AS (
        SELECT ResourceId, ParentResourceId
        FROM [Resource]
        WHERE ResourceId = {0}
        UNION ALL
        SELECT r.ResourceId, r.ParentResourceId
        FROM [Resource] r
        JOIN rbranch rb ON rb.ParentResourceId = r.ResourceId
      )
      SELECT DISTINCT t.TaskId, t.JobId, t.Priority
      FROM [Task] t, [Job] j, [AssignedJobResource] ajr, rbranch
      WHERE NOT (t.IsParentTask = 1 AND t.FinishWhenChildJobsFinished = 1)
      AND t.TaskState = {1}
      AND t.CoresNeeded <= {2}
      AND t.MemoryNeeded <= {3}
      AND t.JobId = j.JobId
      AND j.JobState = 'Online'
      AND j.JobId = ajr.JobId
      AND ajr.ResourceId = rbranch.ResourceId
    ";

    private const string UpdateExecutionTimeQuery = @"
      UPDATE [Task] 
         SET ExecutionTimeMs = {0},
             LastHeartbeat = {1} 
       WHERE TaskId = {2}
    ";
    #endregion
  }
}