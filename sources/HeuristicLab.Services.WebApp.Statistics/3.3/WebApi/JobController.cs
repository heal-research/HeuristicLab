#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Services.Access;
using HeuristicLab.Services.Hive;
using HeuristicLab.Services.Hive.DataAccess.Interfaces;
using DA = HeuristicLab.Services.Hive.DataAccess;
using DT = HeuristicLab.Services.WebApp.Statistics.WebApi.DataTransfer;

namespace HeuristicLab.Services.WebApp.Statistics.WebApi {

  [Authorize]
  public class JobController : ApiController {
    private IPersistenceManager PersistenceManager {
      get { return ServiceLocator.Instance.PersistenceManager; }
    }

    private IUserManager UserManager {
      get { return ServiceLocator.Instance.UserManager; }
    }

    private IRoleVerifier RoleVerifier {
      get { return ServiceLocator.Instance.RoleVerifier; }
    }

    public DT.JobDetails GetJobDetails(Guid id) {
      var pm = PersistenceManager;
      var dimJobDao = pm.DimJobDao;
      var factTaskDao = pm.FactTaskDao;
      return pm.UseTransaction(() => {
        var job = dimJobDao.GetById(id);
        if (job != null) {
          var timeData = factTaskDao.GetByJobId(id).GroupBy(x => x.JobId).Select(x => new {
            AvgTransferringTime = (long)x.Average(y => y.TransferTime),
            MinCalculatingTime = (long)x.Min(y => y.CalculatingTime),
            MaxCalculatingTime = (long)x.Max(y => y.CalculatingTime),
            AvgCalculatingTime = (long)x.Average(y => y.CalculatingTime),
            TotalCalculatingTime = (long)x.Sum(y => y.CalculatingTime),
            TotalWaitingTime = (long)x.Sum(y => y.WaitingTime)
          }).FirstOrDefault();
          DateTime endTime = job.DateCompleted ?? DateTime.Now;
          long totalTime = (long)(endTime - job.DateCreated).TotalSeconds;
          return new DT.JobDetails {
            Id = job.JobId,
            Name = job.JobName,
            UserId = job.UserId,
            UserName = job.UserName,
            DateCreated = job.DateCreated,
            DateCompleted = job.DateCompleted,
            TotalTasks = job.TotalTasks,
            CompletedTasks = job.CompletedTasks,
            AvgTransferringTime = timeData != null ? timeData.AvgTransferringTime : 0,
            AvgCalculatingTime = timeData != null ? timeData.AvgCalculatingTime : 0,
            MinCalculatingTime = timeData != null ? timeData.MinCalculatingTime : 0,
            MaxCalculatingTime = timeData != null ? timeData.MaxCalculatingTime : 0,
            TotalCalculatingTime = timeData != null ? timeData.TotalCalculatingTime : 0,
            TotalWaitingTime = timeData != null ? timeData.TotalWaitingTime : 0,
            TotalTime = totalTime,
            TasksStates = factTaskDao.GetByJobId(id)
                          .GroupBy(x => x.TaskState)
                          .Select(x => new DT.TaskStateCount {
                            State = x.Key.ToString(),
                            Count = x.Count()
                          }).ToList()
          };
        }
        return null;
      });
    }

    public DT.JobPage GetJobs(int page, int size, bool completed = false) {
      return GetJobsByUserId(UserManager.CurrentUserId, page, size, completed);
    }

    public DT.JobPage GetJobsByUserId(Guid id, int page, int size, bool completed = false) {
      if (id != UserManager.CurrentUserId) {
        RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator);
      }
      var pm = PersistenceManager;
      var dimJobDao = pm.DimJobDao;
      return pm.UseTransaction(() => {
        var jobs = dimJobDao.GetByUserId(id).Where(x => completed ? (x.DateCompleted != null) : (x.DateCompleted == null));
        return new DT.JobPage {
          TotalJobs = jobs.Count(),
          Jobs = jobs.OrderByDescending(x => x.DateCompleted ?? DateTime.MaxValue)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(x => ConvertToDT(x))
            .ToList()
        };
      });
    }

    public IEnumerable<DT.Job> GetAllJobs(bool completed) {
      return GetAllJobsByUserId(UserManager.CurrentUserId, completed);
    }

    public IEnumerable<DT.Job> GetAllJobsByUserId(Guid id, bool completed) {
      if (id != UserManager.CurrentUserId) {
        RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator);
      }
      var pm = PersistenceManager;
      var dimJobDao = pm.DimJobDao;
      return pm.UseTransaction(() => {
        return dimJobDao.GetByUserId(id)
          .Where(x => completed ? (x.DateCompleted != null) : (x.DateCompleted == null))
          .Select(x => ConvertToDT(x))
          .ToList();
      });
    }

    [Authorize(Roles = HiveRoles.Administrator)]
    public IEnumerable<DT.Job> GetAllActiveJobsFromAllUsers() {
      var pm = PersistenceManager;
      var dimJobDao = pm.DimJobDao;
      return pm.UseTransaction(() => {
        return dimJobDao.GetAll()
          .Where(x => x.DateCompleted == null)
          .OrderByDescending(x => x.DateCreated)
          .Select(x => ConvertToDT(x))
          .ToList();
      });
    }

    private DT.Job ConvertToDT(DA.DimJob job) {
      return new DT.Job {
        Id = job.JobId,
        Name = job.JobName,
        UserId = job.UserId,
        UserName = job.UserName,
        DateCreated = job.DateCreated,
        DateCompleted = job.DateCompleted,
        TotalTasks = job.TotalTasks,
        CompletedTasks = job.CompletedTasks
      };
    }
  }
}