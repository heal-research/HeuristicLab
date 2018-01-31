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
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using HeuristicLab.Services.Hive;
using HeuristicLab.Services.Hive.DataAccess.Interfaces;
using DT = HeuristicLab.Services.WebApp.Statistics.WebApi.DataTransfer;

namespace HeuristicLab.Services.WebApp.Statistics.WebApi {

  [Authorize(Roles = HiveRoles.Administrator)]
  public class UserController : ApiController {
    private IPersistenceManager PersistenceManager {
      get { return ServiceLocator.Instance.PersistenceManager; }
    }

    public IEnumerable<DT.User> GetUsers() {
      var pm = PersistenceManager;
      var dimUserDao = pm.DimUserDao;
      return pm.UseTransaction(() => {
        return dimUserDao.GetAll().Select(x => new DT.User {
          Id = x.UserId,
          Name = x.Name
        }).ToList();
      });
    }

    public DT.UserDetails GetUser(Guid id) {
      var pm = PersistenceManager;
      var dimUserDao = pm.DimUserDao;
      var factTaskDao = pm.FactTaskDao;
      return pm.UseTransaction(() => {
        var user = dimUserDao.GetById(id);
        if (user != null) {
          return new DT.UserDetails {
            Id = user.UserId,
            Name = user.Name,
            TasksStates = factTaskDao.GetByUserId(id)
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
  }
}