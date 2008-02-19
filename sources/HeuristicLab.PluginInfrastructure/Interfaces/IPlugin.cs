#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;

namespace HeuristicLab.PluginInfrastructure {
  /// <summary>
  /// Represents a plugin.
  /// Plugin developers have to include exactly one class that implements this interface in one of the
  /// assemblies of the plugin. Plugin developers can use the properties of this interface to store
  /// plugin data (name, version, files, update location ...).
  /// The methods OnInstall(), OnDelete(), OnPreUpdate(), OnPostUpdate() are called by the framework
  /// when the corresponding actions are executed. This mechanism allows that the plugin reacts to such
  /// events. For instance to store plugin specific settings.
  /// </summary>
  interface IPlugin {
    string Name { get; }
    Version Version { get; }
    /// <summary>
    /// a list of all files that are bundled with this plugin including all assembly files (*.dll)
    /// </summary>
    string[] Files { get; }

    /// <summary>
    /// called by the framework after the plugin was successfully installed
    /// </summary>
    void OnInstall();
    /// <summary>
    /// called by the framework before the files of the plugin are deleted
    /// </summary>
    void OnDelete();
    /// <summary>
    /// called by the framework before the files of the plugin are deleted for an update
    /// OnPreUpdate() is sent only to the old instance of the plugin
    /// </summary>
    void OnPreUpdate();
    /// <summary>
    /// called by the framework after the updated files for the plugin have been installed
    /// OnPostUpdate() is sent only to the new updated instance of the plugin
    /// </summary>
    void OnPostUpdate();
  }
}
