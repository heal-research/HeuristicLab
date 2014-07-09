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
using HeuristicLab.Core;
using HeuristicLab.Encodings.ScheduleEncoding;

namespace HeuristicLab.Problems.Scheduling {
  public static class GTAlgorithmUtils {

    public static ItemList<Task> GetEarliestNotScheduledTasks(ItemList<Job> jobData) {
      ItemList<Task> result = new ItemList<Task>();
      foreach (Job j in jobData) {
        foreach (Task t in j.Tasks) {
          if (!t.IsScheduled) {
            result.Add(t);
            break;
          }
        }
      }
      return result;
    }
    public static Task GetTaskWithMinimalEC(ItemList<Task> earliestTasksList, Schedule schedule) {
      double minEct = double.MaxValue;
      Task result = null;
      foreach (Task t in earliestTasksList) {
        double ect = ComputeEarliestCompletionTime(t, schedule);
        if (ect < minEct) {
          result = t;
          minEct = ect;
        }
      }
      return result;
    }
    public static ItemList<Task> GetConflictSetForTask(Task conflictedTask, ItemList<Task> earliestTasksList, ItemList<Job> jobData, Schedule schedule) {
      ItemList<Task> result = new ItemList<Task>();
      double conflictedCompletionTime = ComputeEarliestCompletionTime(conflictedTask, schedule);
      result.Add(conflictedTask);
      foreach (Task t in earliestTasksList) {
        if (t.ResourceNr == conflictedTask.ResourceNr) {
          if (ComputeEarliestStartTime(t, schedule) < conflictedCompletionTime)
            result.Add(t);
        }
      }
      return result;
    }

    public static double ComputeEarliestStartTime(Task t, Schedule schedule) {
      ScheduledTask previousTask = schedule.GetLastScheduledTaskForJobNr(t.JobNr);
      Resource affectedResource = schedule.Resources[t.ResourceNr];
      double lastMachineEndTime = affectedResource.TotalDuration;
      double previousJobTaskEndTime = 0;
      if (previousTask != null)
        previousJobTaskEndTime = previousTask.EndTime;

      return Math.Max(previousJobTaskEndTime, lastMachineEndTime);
    }
    public static double ComputeEarliestCompletionTime(Task t, Schedule schedule) {
      return ComputeEarliestStartTime(t, schedule) + t.Duration;
    }
  }
}
