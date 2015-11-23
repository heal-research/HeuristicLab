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
using System.Data.Linq;
using System.Linq;

namespace HeuristicLab.Services.Hive.DataAccess.Daos {
  public class PluginDao : GenericDao<Guid, Plugin> {
    public PluginDao(DataContext dataContext) : base(dataContext) { }

    public override Plugin GetById(Guid id) {
      return GetByIdQuery(DataContext, id);
    }

    public IEnumerable<Plugin> GetByHash(byte[] hash) {
      return GetByHashQuery(DataContext).Where(x => x.Hash.SequenceEqual(hash));
    }

    public int DeleteUnusedPlugins() {
      return DataContext.ExecuteCommand(DeleteUnusedPluginsStringQuery);
    }

    #region Compiled queries
    private static readonly Func<DataContext, Guid, Plugin> GetByIdQuery =
      CompiledQuery.Compile((DataContext db, Guid pluginId) =>
        (from plugin in db.GetTable<Plugin>()
         where plugin.PluginId == pluginId
         select plugin).SingleOrDefault());

    private static readonly Func<DataContext, IEnumerable<Plugin>> GetByHashQuery =
      CompiledQuery.Compile((DataContext db) =>
        (from plugin in db.GetTable<Plugin>()
         where plugin.Hash != null
         select plugin));
    #endregion

    #region String queries
    private const string DeleteUnusedPluginsStringQuery = @"
     DELETE FROM [Plugin] WHERE PluginId IN (
     SELECT p.[PluginId] FROM [Plugin] p
     LEFT JOIN [RequiredPlugins] rp
     ON rp.PluginId = p.PluginId
     WHERE rp.PluginId IS NULL)";
    #endregion
  }
}
