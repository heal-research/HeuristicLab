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
using System.Linq;
using DA = HeuristicLab.Services.Hive.DataAccess;

namespace HeuristicLab.Services.Hive {
  public class RoundRobinTaskScheduler : TaskScheduler {
    private class TaskPriorityResult {
      public Guid TaskId { get; set; }
      public Guid OwnerUserId { get; set; }
    }

    protected override IReadOnlyList<Guid> ScheduleInternal(DA.Slave slave, int count) {
      var pm = PersistenceManager;

      var result = pm.DataContext.ExecuteQuery<TaskPriorityResult>(
        GetHighestPriorityWaitingTasksQuery, slave.ResourceId, count, slave.FreeCores, slave.FreeMemory).ToList();

      foreach (var row in result) {
        pm.DataContext.ExecuteCommand("UPDATE UserPriority SET DateEnqueued = SYSDATETIME() WHERE UserId = {0}", row.OwnerUserId);
      }

      return result.Select(x => x.TaskId).ToArray();
    }

    #region Query Strings
    private string GetHighestPriorityWaitingTasksQuery = @"
WITH rbranch AS(
  SELECT ResourceId, ParentResourceId
  FROM [Resource]
  WHERE ResourceId = {0}
  UNION ALL
  SELECT r.ResourceId, r.ParentResourceId
  FROM [Resource] r
  JOIN rbranch rb ON rb.ParentResourceId = r.ResourceId
)
SELECT TOP ({1}) t.TaskId, j.OwnerUserId
FROM Task t
  JOIN Job j on t.JobId = j.JobId
  JOIN AssignedJobResource ajr on j.JobId = ajr.JobId
  JOIN rbranch on ajr.ResourceId = rbranch.ResourceId
  JOIN UserPriority u on j.OwnerUserId = u.UserId
WHERE NOT (t.IsParentTask = 1 AND t.FinishWhenChildJobsFinished = 1)
AND t.TaskState = 'Waiting'
AND t.CoresNeeded <= {2}
AND t.MemoryNeeded <= {3}
AND j.JobState = 'Online'
ORDER BY t.Priority DESC, u.DateEnqueued ASC, j.DateCreated ASC";
    #endregion
  }
}
