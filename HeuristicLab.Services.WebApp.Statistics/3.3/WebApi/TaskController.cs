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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using HeuristicLab.Services.Access;
using HeuristicLab.Services.Hive;
using HeuristicLab.Services.Hive.DataAccess.Interfaces;
using DA = HeuristicLab.Services.Hive.DataAccess;
using DT = HeuristicLab.Services.WebApp.Statistics.WebApi.DataTransfer;

namespace HeuristicLab.Services.WebApp.Statistics.WebApi {
  [Authorize]
  public class TaskController : ApiController {
    private IPersistenceManager PersistenceManager {
      get { return ServiceLocator.Instance.PersistenceManager; }
    }

    private IUserManager UserManager {
      get { return ServiceLocator.Instance.UserManager; }
    }

    private IRoleVerifier RoleVerifier {
      get { return ServiceLocator.Instance.RoleVerifier; }
    }

    [HttpPost]
    public DT.TaskPage GetTasksByJobId(Guid id, int page, int size, IEnumerable<string> states) {
      var pm = PersistenceManager;
      var dimJobDao = pm.DimJobDao;
      var dimClientDao = pm.DimClientDao;
      var factTaskDao = pm.FactTaskDao;

      DA.DimJob job = pm.UseTransaction(() => dimJobDao.GetById(id));
      if (job == null) {
        throw new ArgumentException("invalid job id");
      }
      if (job.UserId != UserManager.CurrentUserId) {
        RoleVerifier.AuthenticateForAnyRole(HiveRoles.Administrator);
      }

      return pm.UseTransaction(() => {
        var tasks = factTaskDao.GetByJobId(id).Where(x => states.Contains(x.TaskState.ToString()));
        return new DT.TaskPage {
          TotalTasks = tasks.Count(),
          Tasks = (from factTask in tasks
                   join dimJob in dimJobDao.GetAll() on factTask.JobId equals dimJob.JobId
                   join dimClient in dimClientDao.GetAll() on factTask.LastClientId equals
                     dimClient.Id into taskClientJoin
                   from a in taskClientJoin.DefaultIfEmpty()
                   let startTime = factTask.StartTime ?? DateTime.Now
                   let endTime = factTask.EndTime ?? DateTime.Now
                   select new DT.Task {
                     Id = factTask.TaskId,
                     JobId = factTask.JobId,
                     JobName = dimJob.JobName,
                     TotalTime = (long)(endTime - startTime).TotalSeconds,
                     CalculatingTime = factTask.CalculatingTime,
                     WaitingTime = factTask.WaitingTime,
                     TransferTime = factTask.TransferTime,
                     InitialWaitingTime = factTask.InitialWaitingTime,
                     NumCalculationRuns = factTask.NumCalculationRuns,
                     NumRetries = factTask.NumRetries,
                     CoresRequired = factTask.CoresRequired,
                     MemoryRequired = factTask.MemoryRequired,
                     Priority = factTask.Priority,
                     State = factTask.TaskState.ToString(),
                     LastClientId = factTask.LastClientId,
                     LastClientName = a != null ? a.Name : string.Empty,
                     UserId = dimJob.UserId,
                     UserName = dimJob.UserName,
                     StartTime = factTask.StartTime,
                     EndTime = factTask.EndTime,
                     Exception = factTask.Exception
                   })
            .Skip((page - 1) * size)
            .Take(size)
            .ToList()
        };
      });
    }

    [HttpPost]
    public DT.TaskPage GetTasksByClientId(Guid id, int page, int size, IEnumerable<string> states, Guid userId = default(Guid)) {
      bool isAdministrator = User.IsInRole(HiveRoles.Administrator);
      var pm = PersistenceManager;
      var dimJobDao = pm.DimJobDao;
      var dimClientDao = pm.DimClientDao;
      var factTaskDao = pm.FactTaskDao;
      return pm.UseTransaction(() => {
        var tasks = factTaskDao.GetByClientId(id).Where(x => states.Contains(x.TaskState.ToString()));
        if (userId != Guid.Empty) {
          tasks = tasks.Where(x => x.DimJob.UserId == userId);
        }
        return new DT.TaskPage {
          TotalTasks = tasks.Count(),
          Tasks = (from factTask in tasks
                   join dimJob in dimJobDao.GetAll() on factTask.JobId equals dimJob.JobId
                   join dimClient in dimClientDao.GetAll() on factTask.LastClientId equals
                     dimClient.Id into taskClientJoin
                   from a in taskClientJoin.DefaultIfEmpty()
                   let startTime = factTask.StartTime ?? DateTime.Now
                   let endTime = factTask.EndTime ?? DateTime.Now
                   select new DT.Task {
                     Id = isAdministrator ? factTask.TaskId : default(Guid),
                     JobId = isAdministrator ? factTask.JobId : default(Guid),
                     JobName = isAdministrator ? dimJob.JobName : string.Empty,
                     TotalTime = (long)(endTime - startTime).TotalSeconds,
                     CalculatingTime = factTask.CalculatingTime,
                     WaitingTime = factTask.WaitingTime,
                     TransferTime = factTask.TransferTime,
                     InitialWaitingTime = factTask.InitialWaitingTime,
                     NumCalculationRuns = factTask.NumCalculationRuns,
                     NumRetries = factTask.NumRetries,
                     CoresRequired = factTask.CoresRequired,
                     MemoryRequired = factTask.MemoryRequired,
                     Priority = factTask.Priority,
                     State = factTask.TaskState.ToString(),
                     LastClientId = factTask.LastClientId,
                     LastClientName = a != null ? a.Name : string.Empty,
                     UserId = isAdministrator ? dimJob.UserId : default(Guid),
                     UserName = isAdministrator ? dimJob.UserName : string.Empty,
                     StartTime = factTask.StartTime,
                     EndTime = factTask.EndTime,
                     Exception = isAdministrator ? factTask.Exception : string.Empty
                   })
                .OrderByDescending(x => x.EndTime ?? DateTime.MaxValue)
                .Skip((page - 1) * size)
                .Take(size)
                .ToList()
        };
      });
    }

    public HttpResponseMessage GetTaskDataById(Guid id) {
      var pm = PersistenceManager;
      var taskDataDao = pm.TaskDataDao;
      return pm.UseTransaction(() => {
        var taskData = taskDataDao.GetById(id);
        if (taskData == null)
          return new HttpResponseMessage(HttpStatusCode.NotFound);
        HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
        var stream = new MemoryStream(taskData.Data);
        result.Content = new StreamContent(stream);
        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        result.Content.Headers.ContentDisposition =
          new ContentDispositionHeaderValue("attachment") {
            FileName = string.Format("{0}.hl", id)
          };
        return result;
      });
    }
  }
}