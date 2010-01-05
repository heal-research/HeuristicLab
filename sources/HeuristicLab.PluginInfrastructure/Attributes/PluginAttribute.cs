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
  /// This attribute can be used to specify meta data for plugins. 
  /// For example to specify name and description of plugins.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class PluginAttribute : System.Attribute {
    private string name;
    /// <summary>
    /// Gets the name of the plugin.
    /// </summary>
    public string Name {
      get { return name; }
    }

    private string description;
    /// <summary>
    /// Gets the description of the plugin.
    /// </summary>
    public string Description {
      get { return description; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="PluginAttribute"/>.
    /// <param name="name">Name of the plugin</param>
    /// </summary>
    public PluginAttribute(string name)
      : this(name, "") {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="PluginAttribute"/>.
    /// <param name="name">Name of the plugin</param>
    /// <param name="description">Description of the plugin</param>
    /// </summary>
    public PluginAttribute(string name, string description) {
      this.name = name;
      this.description = description;
    }
  }
}
