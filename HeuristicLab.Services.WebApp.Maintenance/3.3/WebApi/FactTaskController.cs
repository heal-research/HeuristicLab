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
using System.Linq;
using System.Web.Http;
using HeuristicLab.Services.Hive;
using HeuristicLab.Services.Hive.DataAccess;
using HeuristicLab.Services.Hive.DataAccess.Interfaces;
using DT = HeuristicLab.Services.WebApp.Maintenance.WebApi.DataTransfer;

namespace HeuristicLab.Services.WebApp.Maintenance.WebApi {
  public class FactTaskController : ApiController {
    private IPersistenceManager PersistenceManager {
      get { return ServiceLocator.Instance.PersistenceManager; }
    }

    public DT.JobPage GetJobs(DateTime start, DateTime end, int page, int size) {
      var pm = PersistenceManager;
      var dimJobDao = pm.DimJobDao;
      var factTaskDao = pm.FactTaskDao;
      return pm.UseTransaction(() => {
        var query = dimJobDao.GetAll().Where(x => x.DateCreated >= start && x.DateCreated <= end && x.DateCompleted != null);
        return new DT.JobPage {
          TotalJobs = query.Count(),
          Jobs = query.OrderBy(x => x.DateCreated)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(x => new DT.Job {
              Id = x.JobId,
              Name = x.JobName,
              Username = x.UserName,
              DateCreated = x.DateCreated,
              Tasks = x.TotalTasks,
              Aggregated = !(factTaskDao.GetAll().Count(y => y.JobId == x.JobId) > 1)
            })
            .ToList()
        };
      });
    }

    [HttpPost]
    public void AggregateJob(Guid id) {
      var pm = PersistenceManager;
      AggregateJob(pm, id);
    }

    [HttpPost]
    public void AggregateAllJobs(DateTime start, DateTime end) {
      var pm = PersistenceManager;
      var dimJobDao = pm.DimJobDao;
      var jobIds = pm.UseTransaction(() => dimJobDao.GetAll()
        .Where(x => x.DateCreated >= start && x.DateCreated <= end && x.DateCompleted != null)
        .Select(x => x.JobId)
        .ToList()
      );
      foreach (var id in jobIds) {
        AggregateJob(pm, id);
      }
    }

    private void AggregateJob(IPersistenceManager pm, Guid id) {
      var jobDao = pm.JobDao;
      var factTaskDao = pm.FactTaskDao;
      pm.UseTransaction(() => {
        if (jobDao.Exists(id)) return;
        var jobTimeData = factTaskDao.GetByJobId(id).GroupBy(x => x.JobId).Select(x => new {
          CalculatingTime = x.Sum(y => y.CalculatingTime),
          WaitingTime = x.Sum(y => y.WaitingTime),
          TransferTime = x.Sum(y => y.TransferTime),
          InitialWaitingTime = x.Sum(y => y.InitialWaitingTime),
          StartTime = x.Min(y => y.StartTime),
          EndTime = x.Max(y => y.EndTime)
        }).SingleOrDefault();
        if (jobTimeData != null) {
          factTaskDao.DeleteByJobId(id);
          factTaskDao.Save(new FactTask {
            TaskId = Guid.NewGuid(),
            JobId = id,
            CalculatingTime = jobTimeData.CalculatingTime,
            WaitingTime = jobTimeData.WaitingTime,
            TransferTime = jobTimeData.TransferTime,
            InitialWaitingTime = jobTimeData.InitialWaitingTime,
            StartTime = jobTimeData.StartTime,
            EndTime = jobTimeData.EndTime,
            NumCalculationRuns = 0,
            NumRetries = 0,
            CoresRequired = 0,
            MemoryRequired = 0,
            Priority = 0,
            TaskState = TaskState.Finished
          });
        }
        pm.SubmitChanges();
      });
    }
  }
}
