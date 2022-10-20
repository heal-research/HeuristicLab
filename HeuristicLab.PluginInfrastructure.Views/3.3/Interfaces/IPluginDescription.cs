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

namespace HeuristicLab.PluginInfrastructure {
  /// <summary>
  /// Represents meta-data of a plugin.
  /// </summary>
  public interface IPluginDescription {
    /// <summary>
    /// Gets the name of the plugin.
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Gets the version of the plugin.
    /// </summary>
    Version Version { get; }
    /// <summary>
    /// Gets the description of the plugin.
    /// </summary>
    string Description { get; }
    /// <summary>
    /// Gets the dependencies of the plugin.
    /// </summary>
    IEnumerable<IPluginDescription> Dependencies { get; }
    /// <summary>
    /// Gets the file names of files that are part of the plugin.
    /// </summary>
    IEnumerable<IPluginFile> Files { get; }
    /// <summary>
    /// Gets the name of the contact person of the plugin.
    /// </summary>
    string ContactName { get; }
    /// <summary>
    /// Gets the e-mail address of the contact person of the plugin.
    /// </summary>
    string ContactEmail { get; }
    /// <summary>
    /// Gets the license text of the plugin.
    /// </summary>
    string LicenseText { get; }
  }
}
