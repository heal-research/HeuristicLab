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
using System.Linq;
using System.Web.Http;
using HeuristicLab.Services.Hive;
using HeuristicLab.Services.Hive.DataAccess.Interfaces;
using DT = HeuristicLab.Services.WebApp.Maintenance.WebApi.DataTransfer;

namespace HeuristicLab.Services.WebApp.Maintenance.WebApi {
  public class PluginController : ApiController {
    private IPersistenceManager PersistenceManager {
      get { return ServiceLocator.Instance.PersistenceManager; }
    }

    public DT.PluginPage GetUnusedPlugins(int page, int size) {
      var pm = PersistenceManager;
      var pluginDao = pm.PluginDao;
      var requiredPluginDao = pm.RequiredPluginDao;
      var taskDao = pm.TaskDao;
      return pm.UseTransaction(() => {
        var taskIds = taskDao.GetAll().Select(x => x.TaskId);
        var usedPluginIds = requiredPluginDao.GetAll()
          .Where(x => taskIds.Contains(x.TaskId))
          .Select(x => x.PluginId)
          .Distinct();
        var query = pluginDao.GetAll().Where(x => !usedPluginIds.Any(y => y == x.PluginId));
        return new DT.PluginPage {
          TotalPlugins = query.Count(),
          Plugins = query.OrderBy(x => x.DateCreated).ThenBy(x => x.Name)
            .Skip((page - 1) * size)
            .Take(size).Select(x => new DT.Plugin {
              Id = x.PluginId,
              Name = x.Name,
              Version = x.Version,
              DateCreated = x.DateCreated
            })
            .ToList()
        };
      });
    }

    [HttpPost]
    public void DeletePlugin(Guid id) {
      var pm = PersistenceManager;
      var pluginDao = pm.PluginDao;
      pm.UseTransaction(() => {
        pluginDao.Delete(id);
        pm.SubmitChanges();
      });
    }

    [HttpPost]
    public void DeleteUnusedPlugins() {
      var pm = PersistenceManager;
      var pluginDao = pm.PluginDao;
      pm.UseTransaction(() => {
        pluginDao.DeleteUnusedPlugins();
        pm.SubmitChanges();
      });
    }
  }
}
