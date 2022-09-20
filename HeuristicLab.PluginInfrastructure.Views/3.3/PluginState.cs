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
  /// Possible states that a plugin can have.
  /// </summary>
  [Serializable]
  public enum PluginState {
    /// <summary>
    /// Undefined state is the default plugin state.
    /// </summary>
    Undefined,
    /// <summary>
    /// Enabled state means that the plugin has correct meta-data and can be loaded.
    /// </summary>
    Enabled,
    /// <summary>
    /// Disabled state means that the plugin has incorrect meta-data or missing dependencies and cannot be loaded.
    /// </summary>
    Disabled,
    /// <summary>
    /// Loaded means the plugin is currently loaded by an application.
    /// </summary>
    Loaded
  }
}