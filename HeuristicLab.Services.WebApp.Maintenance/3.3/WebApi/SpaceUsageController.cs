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

using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using HeuristicLab.Services.Hive;
using HeuristicLab.Services.Hive.DataAccess.Interfaces;
using HeuristicLab.Services.WebApp.Maintenance.WebApi.DataTransfer;
using DA = HeuristicLab.Services.Hive.DataAccess.Data;

namespace HeuristicLab.Services.WebApp.Maintenance.WebApi {
  public class SpaceUsageController : ApiController {
    private IPersistenceManager PersistenceManager {
      get { return ServiceLocator.Instance.PersistenceManager; }
    }

    public IEnumerable<TableInformation> GetHiveTableInformation() {
      var tables = new List<string> {
        "AssignedJobResource",
        "AssignedProjectResource",
        "Downtime",
        "Job",
        "JobPermission",
        "Lifecycle",
        "Plugin",
        "PluginData",
        "Project",
        "ProjectPermission",
        "RequiredPlugins",
        "Resource",
        "StateLog",
        "Task",
        "TaskData",
        "UserPriority"
      };
      var pm = PersistenceManager;
      return pm.UseTransaction(() => (
          from table in tables
          select pm.GetTableInformation(table) into tableInformation
          where tableInformation != null
          select Convert(tableInformation)
          ).ToList()
        );
    }

    public IEnumerable<TableInformation> GetStatisticsTableInformation() {
      var tables = new List<string> {
        "statistics.DimClient",
        "statistics.DimJob",
        "statistics.DimProject",
        "statistics.DimTime",
        "statistics.DimUser",
        "statistics.FactClientInfo",
        "statistics.FactProjectInfo",
        "statistics.FactTask"
      };
      var pm = PersistenceManager;
      return pm.UseTransaction(() => (
          from table in tables
          select pm.GetTableInformation(table) into tableInformation
          where tableInformation != null
          select Convert(tableInformation)
          ).ToList()
        );
    }

    private TableInformation Convert(DA.TableInformation tableInformation) {
      return new TableInformation {
        Name = tableInformation.Name,
        Rows = tableInformation.Rows,
        Reserved = tableInformation.Reserved,
        Data = tableInformation.Data,
        IndexSize = tableInformation.IndexSize,
        Unused = tableInformation.Unused
      };
    }
  }
}
