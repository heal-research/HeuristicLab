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
  /// Singleton class for the ControlManager (needed by HeuristicLab 3.2)
  /// </summary>
  public static class ControlManager {
    // singleton: only one control manager allowed in each application (i.e. AppDomain)
    private static IControlManager controlManager;
    /// <summary>
    /// Gets the control manager (is a singleton).
    /// </summary>
    public static IControlManager Manager {
      get { return controlManager; }
    }

    /// <summary>
    /// Set the current control manager. 
    /// </summary>
    /// <param name="manager">The control manager</param>
    /// <exception cref="InvalidOperationException">Throws exception when a control manager has previously been set.</exception>
    public static void RegisterManager(IControlManager manager) {
      if (controlManager != null) throw new InvalidOperationException("An control manager has already been set.");
      controlManager = manager;
    }
  }
}
