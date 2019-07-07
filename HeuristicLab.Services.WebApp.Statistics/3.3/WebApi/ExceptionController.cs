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
using System.Linq;
using System.Web.Http;
using HeuristicLab.Services.Hive;
using HeuristicLab.Services.Hive.DataAccess.Interfaces;
using DT = HeuristicLab.Services.WebApp.Statistics.WebApi.DataTransfer;

namespace HeuristicLab.Services.WebApp.Statistics.WebApi {
  [Authorize(Roles = HiveRoles.Administrator)]
  public class ExceptionController : ApiController {
    private IPersistenceManager PersistenceManager {
      get { return ServiceLocator.Instance.PersistenceManager; }
    }

    public DT.ExceptionPage GetExceptions(int page, int size) {
      var pm = PersistenceManager;
      var dimClientDao = pm.DimClientDao;
      var dimJobDao = pm.DimJobDao;
      var factTaskDao = pm.FactTaskDao;
      var tasks = factTaskDao.GetTasksWithException();
      return pm.UseTransaction(() => new DT.ExceptionPage {
        TotalExceptions = tasks.Count(),
        Exceptions = (from factTask in tasks
                      join dimJob in dimJobDao.GetAll() on factTask.JobId equals dimJob.JobId
                      join dimClient in dimClientDao.GetAll() on factTask.LastClientId equals
                        dimClient.Id into taskClientJoin
                      from a in taskClientJoin.DefaultIfEmpty()
                      let endTime = factTask.EndTime ?? DateTime.Now
                      select new DT.Exception {
                        TaskId = factTask.TaskId,
                        JobId = factTask.JobId,
                        JobName = dimJob.JobName,
                        UserId = dimJob.UserId,
                        UserName = dimJob.UserName,
                        ClientId = factTask.LastClientId ?? default(Guid),
                        ClientName = a != null ? a.Name : string.Empty,
                        Date = endTime,
                        Details = factTask.Exception
                      })
                      .OrderByDescending(x => x.Date)
                      .Skip((page - 1) * size)
                      .Take(size)
                      .ToList()
      });
    }
  }
}
