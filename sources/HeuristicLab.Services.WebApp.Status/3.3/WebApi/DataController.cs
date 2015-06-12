#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Web.Http;
using HeuristicLab.Services.Hive;
using HeuristicLab.Services.Hive.DataAccess;
using DAL = HeuristicLab.Services.Hive.DataAccess;
using DTO = HeuristicLab.Services.WebApp.Status.WebApi.DataTransfer;

namespace HeuristicLab.Services.WebApp.Status.WebApi {
  public class DataController : ApiController {

    // start temporary quickfix
    private const string SQL_USER_TASK_STATUS =
      @"WITH UserTasks AS (
          SELECT Job.OwnerUserId AS UserId, TaskState, COUNT(Task.TaskId) AS Count
          FROM Task, Job
          WHERE Task.JobId = Job.JobId AND TaskState IN ('Calculating', 'Waiting')
          GROUP BY Job.OwnerUserId, TaskState
        )
        SELECT 
          DISTINCT UserId,
          (SELECT Count FROM UserTasks WHERE TaskState = 'Calculating' AND UserId = ut.UserId) AS CalculatingTasks,
          (SELECT Count FROM UserTasks WHERE TaskState = 'Waiting' AND UserId = ut.UserId) AS WaitingTasks
        FROM UserTasks ut;";


    private class UserTaskStatus {
      public Guid UserId { get; set; }
      public int CalculatingTasks { get; set; }
      public int WaitingTasks { get; set; }
    }

    public IEnumerable<DTO.TaskStatus> GetTaskStatus(HiveDataContext db) {
      var query = db.ExecuteQuery<UserTaskStatus>(SQL_USER_TASK_STATUS).ToList();
      return query.Select(uts => new DTO.TaskStatus {
        User = new DTO.User {
          Id = uts.UserId.ToString(),
          Name = ServiceLocator.Instance.UserManager.GetUserById(uts.UserId).UserName
        },
        CalculatingTasks = uts.CalculatingTasks,
        WaitingTasks = uts.WaitingTasks
      }).OrderBy(x => x.User.Name);
    }
    // end temporary quickfix

    public DTO.Status GetStatus() {
      using (var db = new HiveDataContext()) {
        var onlineSlaves = (from slave in db.Resources.OfType<DAL.Slave>()
                            where slave.SlaveState == SlaveState.Calculating || slave.SlaveState == SlaveState.Idle
                            select slave).ToList();
        var activeSlaves = onlineSlaves.Where(s => s.IsAllowedToCalculate).ToList();
        var calculatingSlaves = activeSlaves.Where(s => s.SlaveState == SlaveState.Calculating).ToList();

        int calculatingMemory = calculatingSlaves.Any() ? (int)calculatingSlaves.Sum(s => s.Memory) / 1024 : 0;
        int freeCalculatingMemory = calculatingSlaves.Any() ? (int)calculatingSlaves.Sum(s => s.FreeMemory) / 1024 : 0;

        return new DTO.Status {
          CoreStatus = new DTO.CoreStatus {
            TotalCores = onlineSlaves.Sum(s => s.Cores ?? 0),
            FreeCores = onlineSlaves.Sum(s => s.FreeCores ?? 0), // temporary for old chart data
            ActiveCores = activeSlaves.Sum(s => s.Cores ?? 0),
            CalculatingCores = calculatingSlaves.Sum(s => s.Cores ?? 0)
          },
          CpuUtilizationStatus = new DTO.CpuUtilizationStatus {
            TotalCpuUtilization = onlineSlaves.Any()
                                  ? Math.Round(onlineSlaves.Average(s => s.CpuUtilization), 2)
                                  : 0.0,
            ActiveCpuUtilization = activeSlaves.Any()
                                   ? Math.Round(activeSlaves.Average(s => s.CpuUtilization), 2)
                                   : 0.0,
            CalculatingCpuUtilization = calculatingSlaves.Any()
                                        ? Math.Round(calculatingSlaves.Average(s => s.CpuUtilization), 2)
                                        : 0.0
          },
          MemoryStatus = new DTO.MemoryStatus {
            TotalMemory = onlineSlaves.Any() ? (int)onlineSlaves.Sum(s => s.Memory) / 1024 : 0,
            FreeMemory = onlineSlaves.Any() ? (int)onlineSlaves.Sum(s => s.FreeMemory) / 1024 : 0,
            ActiveMemory = activeSlaves.Any() ? (int)activeSlaves.Sum(s => s.Memory) / 1024 : 0,
            UsedMemory = calculatingMemory - freeCalculatingMemory
          },
          TasksStatus = GetTaskStatus(db),
          SlavesStatus = onlineSlaves.Select(x => new DTO.SlaveStatus {
            Slave = new DTO.Slave {
              Id = x.ResourceId.ToString(),
              Name = x.Name
            },
            CpuUtilization = x.CpuUtilization,
            Cores = x.Cores ?? 0,
            FreeCores = x.FreeCores ?? 0,
            Memory = (x.Memory ?? 0) / 1024,
            FreeMemory = (x.FreeMemory ?? 0) / 1024,
            IsAllowedToCalculate = x.IsAllowedToCalculate,
            State = x.SlaveState.ToString()
          }).OrderBy(x => x.Slave.Name),
          Timestamp = JavascriptUtils.ToTimestamp(DateTime.Now)
        };
      }
    }

    public IEnumerable<DTO.Status> GetStatusHistory(DateTime start, DateTime end) {
      TimeSpan ts = end - start;
      int increment = 1;
      double totalMinutes = ts.TotalMinutes;
      while (totalMinutes > 5761) {
        totalMinutes -= 5761;
        increment += 5;
      }
      using (var db = new HiveDataContext()) {
        var statistics = db.Statistics.Where(s => s.Timestamp >= start && s.Timestamp <= end);
        var status = new DTO.Status {
          CoreStatus = new DTO.CoreStatus(),
          CpuUtilizationStatus = new DTO.CpuUtilizationStatus(),
          MemoryStatus = new DTO.MemoryStatus()
        };
        int freeCores = 0;
        int freeMemory = 0;
        int i = 1;
        foreach (var statistic in statistics) {
          status.CoreStatus.TotalCores += statistic.SlaveStatistics.Sum(x => x.Cores);
          freeCores += statistic.SlaveStatistics.Sum(x => x.FreeCores);
          status.CpuUtilizationStatus.TotalCpuUtilization += statistic.SlaveStatistics.Any()
                                                             ? statistic.SlaveStatistics.Average(x => x.CpuUtilization)
                                                             : 0.0;
          status.MemoryStatus.TotalMemory += statistic.SlaveStatistics.Sum(x => x.Memory) / 1024;
          freeMemory += statistic.SlaveStatistics.Sum(x => x.FreeMemory) / 1024;
          if (i >= increment) {
            status.Timestamp = JavascriptUtils.ToTimestamp(statistic.Timestamp);
            status.CoreStatus.TotalCores /= i;
            freeCores /= i;
            status.CpuUtilizationStatus.TotalCpuUtilization /= i;
            status.MemoryStatus.TotalMemory /= i;
            freeMemory /= i;
            status.CoreStatus.ActiveCores = status.CoreStatus.TotalCores;
            status.MemoryStatus.ActiveMemory = status.MemoryStatus.TotalMemory;
            status.CpuUtilizationStatus.ActiveCpuUtilization = status.CpuUtilizationStatus.TotalCpuUtilization;
            status.CpuUtilizationStatus.CalculatingCpuUtilization = status.CpuUtilizationStatus.CalculatingCpuUtilization;
            status.CoreStatus.CalculatingCores = status.CoreStatus.TotalCores - freeCores;
            status.MemoryStatus.UsedMemory = status.MemoryStatus.TotalMemory - freeMemory;
            yield return status;
            status = new DTO.Status {
              CoreStatus = new DTO.CoreStatus(),
              CpuUtilizationStatus = new DTO.CpuUtilizationStatus(),
              MemoryStatus = new DTO.MemoryStatus()
            };
            freeCores = 0;
            freeMemory = 0;
            i = 1;
          } else {
            i++;
          }
        }
      }
    }
  }
}