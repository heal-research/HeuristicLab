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

    /// <summary>
    /// Initializes a new instance of <see cref="PluginDependencyAttribute"/>.
    /// <param name="dependency">The name of the plugin that is needed to load a plugin.</param>
    /// </summary>
    public PluginDependencyAttribute(string dependency) {
      if (string.IsNullOrEmpty(dependency)) throw new ArgumentException("Dependency is null or empty.", "dependency");
      this.dependency = dependency;
    }
  }
}
