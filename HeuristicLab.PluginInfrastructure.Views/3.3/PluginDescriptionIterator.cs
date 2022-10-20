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

using System.Collections.Generic;
using System.Linq;

namespace HeuristicLab.PluginInfrastructure.Manager {
  internal static class PluginDescriptionIterator {
    internal static IEnumerable<PluginDescription> IterateDependenciesBottomUp(IEnumerable<PluginDescription> pluginDescriptions) {
      // list to make sure we yield each description only once
      HashSet<PluginDescription> yieldedDescriptions = new HashSet<PluginDescription>();
      foreach (var desc in pluginDescriptions) {
        foreach (var dependency in IterateDependenciesBottomUp(desc.Dependencies)) {
          if (!yieldedDescriptions.Contains(dependency)) {
            yieldedDescriptions.Add(dependency);
            yield return dependency;
          }
        }
        if (!yieldedDescriptions.Contains(desc)) {
          yieldedDescriptions.Add(desc);
          yield return desc;
        }
      }
    }

    internal static IEnumerable<PluginDescription> IterateDependentsTopDown(IEnumerable<PluginDescription> pluginDescriptions, IEnumerable<PluginDescription> allPlugins) {
      HashSet<PluginDescription> yieldedDescriptions = new HashSet<PluginDescription>();
      foreach (var desc in pluginDescriptions) {
        foreach (var dependent in IterateDependentsTopDown(GetDependentPlugins(desc, allPlugins), allPlugins)) {
          if (!yieldedDescriptions.Contains(dependent)) {
            yieldedDescriptions.Add(dependent);
            yield return dependent;
          }
        }
        if (!yieldedDescriptions.Contains(desc)) {
          yieldedDescriptions.Add(desc);
          yield return desc;
        }
      }
    }

    private static IEnumerable<PluginDescription> GetDependentPlugins(PluginDescription desc, IEnumerable<PluginDescription> allPlugins) {
      return from plugin in allPlugins
             where plugin.Dependencies.Contains(desc)
             select plugin;
    }
  }
}
