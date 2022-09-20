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
using System.Linq;
using System.Windows.Forms;

namespace HeuristicLab.PluginInfrastructure.Advanced {
  internal static class Util {
    internal static void ResizeColumn(ColumnHeader columnHeader) {
      columnHeader.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
      int headerSize = columnHeader.Width;
      columnHeader.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
      int contentSize = columnHeader.Width;
      columnHeader.Width = Math.Max(headerSize, contentSize);
    }

    internal static void ResizeColumns(IEnumerable<ColumnHeader> columnHeaders) {
      foreach (var columnHeader in columnHeaders)
        ResizeColumn(columnHeader);
    }

    internal static IEnumerable<IPluginDescription> GetAllDependents(IPluginDescription plugin, IEnumerable<IPluginDescription> availablePlugins) {
      return from p in availablePlugins
             let matchingEntries = from dep in GetAllDependencies(p)
                                   where dep.Name == plugin.Name
                                   where dep.Version == plugin.Version
                                   select dep
             where matchingEntries.Any()
             select p as IPluginDescription;
    }

    internal static IEnumerable<IPluginDescription> GetAllDependencies(IPluginDescription plugin) {
      HashSet<IPluginDescription> yieldedPlugins = new HashSet<IPluginDescription>();
      foreach (var dep in plugin.Dependencies) {
        foreach (var recDep in GetAllDependencies(dep)) {
          if (!yieldedPlugins.Contains(recDep)) {
            yieldedPlugins.Add(recDep);
            yield return recDep;
          }
        }
        if (!yieldedPlugins.Contains(dep)) {
          yieldedPlugins.Add(dep);
          yield return dep;
        }
      }
    }

    // compares for two plugins with same major and minor version if plugin1 is newer than plugin2
    internal static bool IsNewerThan(IPluginDescription plugin1, IPluginDescription plugin2) {
      // newer: build version is higher, or if build version is the same revision is higher
      return plugin1.Version.Build > plugin2.Version.Build ||
        (plugin1.Version.Build == plugin2.Version.Build && plugin1.Version.Revision > plugin2.Version.Revision);
    }
  }
}
