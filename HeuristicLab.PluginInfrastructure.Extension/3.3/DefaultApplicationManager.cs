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

namespace HeuristicLab.PluginInfrastructure {

  /// <summary>  
  /// The DefaultApplicationManager is registered as ApplicationManager.Manager singleton for each HL application
  /// started via the plugin infrastructure.
  /// </summary>
  internal sealed class DefaultApplicationManager : SandboxApplicationManager {

    internal DefaultApplicationManager()
      : base() {
      AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {
        if (loadedAssemblies.ContainsKey(args.Name)) {
          return loadedAssemblies[args.Name];
        }
        return null;
      };
    }

    /// infinite lease time
    /// <summary>
    /// Initializes the life time service with infinite lease time.
    /// </summary>
    /// <returns><c>null</c>.</returns>
    public override object InitializeLifetimeService() {
      return null;
    }
  }
}
