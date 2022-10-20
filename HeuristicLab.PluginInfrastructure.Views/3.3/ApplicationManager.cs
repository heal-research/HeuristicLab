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
  /// The ApplicationManager has a reference to the application manager singleton.
  /// </summary>
  public static class ApplicationManager {
    // the singleton instance is initialized to LightweightApplicationManager as long as no other application manager is registered    
    private static IApplicationManager appManager;

    /// <summary>
    /// Gets the application manager singleton.
    /// </summary>
    public static IApplicationManager Manager {
      get {
        if (appManager == null)
          appManager = new LightweightApplicationManager();
        return appManager;
      }
    }

    /// <summary>
    /// Registers a new application manager.
    /// </summary>
    /// <param name="manager"></param>
    internal static void RegisterApplicationManager(IApplicationManager manager) {
      if (appManager != null && !(appManager is LightweightApplicationManager)) throw new InvalidOperationException("The application manager has already been set.");
      else {
        appManager = manager;
      }
    }
  }
}
