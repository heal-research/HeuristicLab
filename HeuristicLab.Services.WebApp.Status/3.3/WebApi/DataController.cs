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
using System.Linq;
using System.Web.Http;
using HeuristicLab.Services.Hive;
using HeuristicLab.Services.Hive.DataAccess;
using HeuristicLab.Services.Hive.DataAccess.Interfaces;
using DT = HeuristicLab.Services.WebApp.Status.WebApi.DataTransfer;

namespace HeuristicLab.Services.WebApp.Status.WebApi {
  public class DataController : ApiController {
    private const int LAST_TASKS = 20;

    private IPersistenceManager PersistenceManager {
      get { return ServiceLocator.Instance.PersistenceManager; }
    }

    private const string SQL_USER_TASK_STATUS =
      @"WITH UserTasks AS (
          SELECT Job.OwnerUserId AS UserId, TaskState, COUNT(Task.TaskId) AS Count
          FROM Task, Job
          WHERE Task.JobId = Job.JobId AND TaskState IN ('Calculating', 'Waiting')
          GROUP BY Job.OwnerUserId, TaskState
        )
        SELECT 
          DISTINCT UserId,
          ISNULL((SELECT Count FROM UserTasks WHERE TaskState = 'Calculating' AND UserId = ut.UserId), 0) AS CalculatingTasks,
          ISNULL((SELECT Count FROM UserTasks WHERE TaskState = 'Waiting' AND UserId = ut.UserId), 0) AS WaitingTasks
        FROM UserTasks ut;";

    private class UserTaskStatus {
      public Guid UserId { get; set; }
      public int CalculatingTasks { get; set; }
      public int WaitingTasks { get; set; }
    }

    private IEnumerable<DT.TaskStatus> GetTaskStatus(IPersistenceManager pm) {
      return pm.UseTransaction(() => {
        var query = pm.DataContext.ExecuteQuery<UserTaskStatus>(SQL_USER_TASK_STATUS).ToList();
        return query.Select(uts => new DT.TaskStatus {
          User = new DT.User {
            Id = uts.UserId.ToString(),
            Name = ServiceLocator.Instance.UserManager.GetUserNameById(uts.UserId)
          },
          CalculatingTasks = uts.CalculatingTasks,
          WaitingTasks = uts.WaitingTasks
        }).OrderBy(x => x.User.Name);
      });
    }

    private DT.TimeStatus GetTimeStatus(IPersistenceManager pm) {
      return pm.UseTransaction(() => {
        var factTaskDao = pm.FactTaskDao;
        var factTasks = factTaskDao.GetAll();
        var completedTasks = factTaskDao.GetCompletedTasks()
          .OrderByDescending(x => x.EndTime)
          .Take(LAST_TASKS);
        var lastCalculatingTimes = completedTasks
          .GroupBy(x => 1)
          .Select(x => new {
            Min = x.Min(y => y.CalculatingTime),
            Max = x.Max(y => y.CalculatingTime),
            Avg = (long)x.Average(y => (long?)y.CalculatingTime)
          }).FirstOrDefault();
        var calculatingTasks = factTasks.Where(x => x.TaskState == TaskState.Calculating);
        int count = calculatingTasks.Count() / 3;
        return new DT.TimeStatus {
          MinCalculatingTime = lastCalculatingTimes != null ? lastCalculatingTimes.Min : 0,
          MaxCalculatingTime = lastCalculatingTimes != null ? lastCalculatingTimes.Max : 0,
          AvgWaitingTime = count > 0 ? (long)calculatingTasks.OrderBy(x => x.StartTime).Take(count).Average(x => x.InitialWaitingTime) : 0,
          AvgCalculatingTime = lastCalculatingTimes != null ? lastCalculatingTimes.Avg : 0,
          TotalCpuTime = factTasks.Sum(x => (long?)x.CalculatingTime) ?? 0,
          StandardDeviationCalculatingTime = (long)StandardDeviation(completedTasks.Select(x => (double)x.CalculatingTime)),
          BeginDate = factTasks.Where(x => x.StartTime.HasValue).OrderBy(x => x.StartTime).Select(x => x.StartTime).FirstOrDefault()
        };
      });
    }

    public DT.Status GetStatus() {
      var pm = PersistenceManager;
      var slaveDao = pm.SlaveDao;
      var onlineSlaves = pm.UseTransaction(() => slaveDao.GetOnlineSlaves().ToList());
      var activeSlaves = onlineSlaves.Where(s => s.IsAllowedToCalculate).ToList();
      var calculatingSlaves = activeSlaves.Where(s => s.SlaveState == SlaveState.Calculating).ToList();
      int totalCores = onlineSlaves.Sum(s => s.Cores ?? 0);
      int totalMemory = onlineSlaves.Sum(s => s.Memory ?? 0);
      return new DT.Status {
        CoreStatus = new DT.CoreStatus {
          TotalCores = totalCores,
          UsedCores = totalCores - onlineSlaves.Sum(s => s.FreeCores ?? 0),
          ActiveCores = activeSlaves.Sum(s => s.Cores ?? 0),
          CalculatingCores = calculatingSlaves.Sum(s => s.Cores ?? 0) - calculatingSlaves.Sum(s => s.FreeCores ?? 0)
        },
        CpuUtilizationStatus = new DT.CpuUtilizationStatus {
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
        MemoryStatus = new DT.MemoryStatus {
          TotalMemory = totalMemory,
          UsedMemory = totalMemory - onlineSlaves.Sum(s => s.FreeMemory ?? 0),
          ActiveMemory = activeSlaves.Sum(s => s.Memory ?? 0),
          CalculatingMemory = calculatingSlaves.Sum(s => s.Memory ?? 0) - calculatingSlaves.Sum(s => s.FreeMemory ?? 0)
        },
        TimeStatus = GetTimeStatus(pm),
        TasksStatus = GetTaskStatus(pm),
        SlavesStatus = onlineSlaves.Select(x => new DT.SlaveStatus {
          Slave = new DT.Slave {
            Id = x.ResourceId.ToString(),
            Name = x.Name
          },
          CpuUtilization = x.CpuUtilization,
          Cores = x.Cores ?? 0,
          FreeCores = x.FreeCores ?? 0,
          Memory = x.Memory ?? 0,
          FreeMemory = x.FreeMemory ?? 0,
          IsAllowedToCalculate = x.IsAllowedToCalculate,
          State = x.SlaveState.ToString()
        }).OrderBy(x => x.Slave.Name),
        Timestamp = JavascriptUtils.ToTimestamp(DateTime.Now)
      };
    }

    public IEnumerable<DT.Status> GetStatusHistory(DateTime start, DateTime end) {
      TimeSpan ts = end - start;
      int increment = 1;
      double totalMinutes = ts.TotalMinutes;
      while (totalMinutes > 5761) {
        totalMinutes -= 5761;
        increment += 5;
      }
      var pm = PersistenceManager;
      var factClientInfoDao = pm.FactClientInfoDao;
      var clientInfos = pm.UseTransaction(() => {
        return factClientInfoDao.GetAll()
          .Where(s => s.Time >= start
                      && s.Time <= end
                      && s.SlaveState != SlaveState.Offline)
          .GroupBy(s => s.Time)
          .Select(x => new {
            Timestamp = x.Key,
            TotalCores = x.Sum(y => y.NumTotalCores),
            UsedCores = x.Sum(y => y.NumUsedCores),
            TotalMemory = x.Sum(y => y.TotalMemory),
            UsedMemory = x.Sum(y => y.UsedMemory),
            CpuUtilization = x.Where(y => y.IsAllowedToCalculate).Average(y => y.CpuUtilization)
          })
          .ToList();
      });
      var statusList = new List<DT.Status>();
      var e = clientInfos.GetEnumerator();
      do {
        var status = new DT.Status {
          CoreStatus = new DT.CoreStatus(),
          CpuUtilizationStatus = new DT.CpuUtilizationStatus(),
          MemoryStatus = new DT.MemoryStatus()
        };
        int i = 0;
        DateTime lastTimestamp = DateTime.Now;
        while (e.MoveNext()) {
          var clientInfo = e.Current;
          status.CoreStatus.TotalCores += clientInfo.TotalCores;
          status.CoreStatus.UsedCores += clientInfo.UsedCores;
          status.MemoryStatus.TotalMemory += clientInfo.TotalMemory;
          status.MemoryStatus.UsedMemory += clientInfo.UsedMemory;
          status.CpuUtilizationStatus.TotalCpuUtilization += clientInfo.CpuUtilization;
          lastTimestamp = clientInfo.Timestamp;
          i++;
          if (i >= increment)
            break;
        }
        if (i <= 0) continue;
        status.Timestamp = JavascriptUtils.ToTimestamp(lastTimestamp);
        status.CoreStatus.TotalCores /= i;
        status.CoreStatus.UsedCores /= i;
        status.MemoryStatus.TotalMemory /= i;
        status.MemoryStatus.UsedMemory /= i;
        status.CpuUtilizationStatus.TotalCpuUtilization /= i;
        statusList.Add(status);
      } while (e.Current != null);
      return statusList.OrderBy(x => x.Timestamp).ToList();
    }

    private double StandardDeviation(IEnumerable<double> source) {
      int n = 0;
      double mean = 0;
      double M2 = 0;
      foreach (double x in source) {
        n = n + 1;
        double delta = x - mean;
        mean = mean + delta / n;
        M2 += delta * (x - mean);
      }
      if (n < 2) {
        return M2;
      }
      return Math.Sqrt(M2 / (n - 1));
    }
  }
}