#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Services.Hive.DataTransfer;

namespace HeuristicLab.Services.Hive {
  public class RoundRobinTaskScheduler : ITaskScheduler {
    private IHiveDao dao {
      get { return ServiceLocator.Instance.HiveDao; }
    }

    public IEnumerable<TaskInfoForScheduler> Schedule(IEnumerable<TaskInfoForScheduler> tasks, int count = 1) {
      if (!tasks.Any()) return Enumerable.Empty<TaskInfoForScheduler>();

      var userPriorities = dao.GetUserPriorities(x => true).OrderBy(x => x.DateEnqueued).ToArray();

      var jobs = new List<JobInfoForScheduler>();
      foreach (var userPriority in userPriorities) {
        jobs.AddRange(dao.GetJobInfoForScheduler(x => x.OwnerUserId == userPriority.Id));
      }

      var taskJobRelations = tasks.Join(jobs,
        task => task.JobId,
        job => job.Id,
        (task, job) => new { Task = task, JobInfo = job })
        .OrderByDescending(x => x.Task.Priority)
        .ToList();

      var scheduledTasks = new List<TaskInfoForScheduler>();
      int priorityIndex = 0;

      if (count == 0 || count > taskJobRelations.Count) count = taskJobRelations.Count;

      for (int i = 0; i < count; i++) {
        var defaultEntry = taskJobRelations.First(); // search first task which is not included yet
        var priorityEntries = taskJobRelations.Where(x => x.JobInfo.OwnerUserId == userPriorities[priorityIndex].Id).ToArray(); // search for tasks with desired user priority
        while (!priorityEntries.Any() && priorityIndex < userPriorities.Length - 1) {
          priorityIndex++;
          priorityEntries = taskJobRelations.Where(x => x.JobInfo.OwnerUserId == userPriorities[priorityIndex].Id).ToArray();
        }
        if (priorityEntries.Any()) { // tasks with desired user priority found
          var priorityEntry = priorityEntries.OrderByDescending(x => x.Task.Priority).ThenBy(x => x.JobInfo.DateCreated).First();
          if (defaultEntry.Task.Priority <= priorityEntry.Task.Priority) {
            taskJobRelations.Remove(priorityEntry);
            scheduledTasks.Add(priorityEntry.Task);
            UpdateUserPriority(userPriorities[priorityIndex]);
            priorityIndex++;
          } else { // there are other tasks with higher priorities
            taskJobRelations.Remove(defaultEntry);
            scheduledTasks.Add(defaultEntry.Task);
          }
        } else {
          taskJobRelations.Remove(defaultEntry);
          scheduledTasks.Add(defaultEntry.Task);
        }

        if (priorityIndex >= (userPriorities.Length - 1)) priorityIndex = 0;
      }

      return scheduledTasks;
    }

    private void UpdateUserPriority(UserPriority up) {
      up.DateEnqueued = DateTime.Now;
      dao.EnqueueUserPriority(up);
    }
  }
}
