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
  /// This attribute can be used to declare that a plugin depends on a another plugin.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public sealed class PluginDependencyAttribute : System.Attribute {
    private string dependency;

    /// <summary>
    /// Gets the name of the plugin that is needed to load a plugin.
    /// </summary>
    public string Dependency {
      get { return dependency; }
    }

    private Version version;
    /// <summary>
    /// Gets the version of the plugin dependency.
    /// </summary>
    public Version Version {
      get { return version; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="PluginDependencyAttribute" />.
    /// </summary>
    /// <param name="dependency">Name of the plugin dependency.</param>
    /// <param name="version">Version of the plugin dependency.</param>
    public PluginDependencyAttribute(string dependency, string version) {
      if (string.IsNullOrEmpty(dependency)) throw new ArgumentException("Dependency name is null or empty.", "dependency");
      if (string.IsNullOrEmpty(version)) throw new ArgumentException("Dependency version is null or empty.", "version");
      this.dependency = dependency;
      this.version = new Version(version); // throws format exception if the version string can't be parsed
    }
  }
}
