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
          ISNULL((SELECT Count FROM UserTasks WHERE TaskState = 'Calculating' AND UserId = ut.UserId), 0) AS CalculatingTasks,
          ISNULL((SELECT Count FROM UserTasks WHERE TaskState = 'Waiting' AND UserId = ut.UserId), 0) AS WaitingTasks
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
      });
    }
    // end temporary quickfix

    public DTO.Status GetStatus() {
      using (var db = new HiveDataContext()) {
        var onlineSlaves = (from slave in db.Resources.OfType<DAL.Slave>()
                            where slave.SlaveState == SlaveState.Calculating || slave.SlaveState == SlaveState.Idle
                            select slave).ToList();
        return new DTO.Status {
          CoreStatus = new DTO.CoreStatus {
            TotalCores = onlineSlaves.Sum(s => s.Cores ?? 0),
            AvailableCores = onlineSlaves.Where(s => s.IsAllowedToCalculate).Sum(s => s.Cores ?? 0),
            FreeCores = onlineSlaves.Sum(s => s.FreeCores ?? 0)
          },
          CpuUtilizationStatus = new DTO.CpuUtilizationStatus {
            TotalCpuUtilization = onlineSlaves.Any()
                                  ? Math.Round(onlineSlaves.Average(s => s.CpuUtilization), 2)
                                  : 0.0,
            UsedCpuUtilization = onlineSlaves.Any(x => x.IsAllowedToCalculate)
                                 ? Math.Round(onlineSlaves.Where(x => x.IsAllowedToCalculate).Average(s => s.CpuUtilization), 2)
                                 : 0.0
          },
          MemoryStatus = new DTO.MemoryStatus {
            TotalMemory = onlineSlaves.Any() ? (int)onlineSlaves.Sum(s => s.Memory) / 1024 : 0,
            FreeMemory = onlineSlaves.Any() ? (int)onlineSlaves.Sum(s => s.FreeMemory) / 1024 : 0
          },
          TasksStatus = GetTaskStatus(db),
          SlavesCpuStatus = onlineSlaves.Select(x => new DTO.SlaveCpuStatus {
            CpuUtilization = Math.Round(x.CpuUtilization, 2),
            Slave = new DTO.Slave {
              Id = x.ResourceId.ToString(),
              Name = x.Name
            }
          }),
          Timestamp = JavascriptUtils.ToTimestamp(DateTime.Now)
        };
      }
    }

    public IEnumerable<DTO.Status> GetStatusHistory(DateTime start, DateTime end) {
      using (var db = new HiveDataContext()) {
        var statistics = db.Statistics.Where(s => s.Timestamp >= start && s.Timestamp <= end)
                                      .OrderBy(x => x.Timestamp)
                                      .ToList();
        foreach (var statistic in statistics) {
          yield return new DTO.Status {
            CoreStatus = new DTO.CoreStatus {
              TotalCores = statistic.SlaveStatistics.Sum(x => x.Cores),
              AvailableCores = 0,
              FreeCores = statistic.SlaveStatistics.Sum(x => x.FreeCores)
            },
            CpuUtilizationStatus = new DTO.CpuUtilizationStatus {
              TotalCpuUtilization = 0.0,
              UsedCpuUtilization = statistic.SlaveStatistics.Any() ? statistic.SlaveStatistics.Average(x => x.CpuUtilization) : 0.0
            },
            MemoryStatus = new DTO.MemoryStatus {
              TotalMemory = statistic.SlaveStatistics.Sum(x => x.Memory) / 1024,
              FreeMemory = statistic.SlaveStatistics.Sum(x => x.FreeMemory) / 1024
            },
            Timestamp = JavascriptUtils.ToTimestamp(statistic.Timestamp)
          };
        }
      }
    }
  }
}