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
  /// This attribute can be used to specify meta data for applications. 
  /// For example to specify name and description of applications.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class ApplicationAttribute : System.Attribute {
    private string name;
    /// <summary>
    /// Gets the name of the application.
    /// </summary>
    public string Name {
      get { return name; }
    }

    private string description;
    /// <summary>
    /// Gets the description of the application.
    /// </summary>
    public string Description {
      get { return description; }
    }

    private bool restartOnErrors;
    /// <summary>
    /// Gets whether the plugin should be automatically restarted when it is closed because of an exception (for services).
    /// </summary>
    public bool RestartOnErrors {
      get { return restartOnErrors; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ApplicationAttribute"/>.
    /// <param name="name">Name of the application</param>
    /// </summary>
    public ApplicationAttribute(string name)
      : this(name, String.Empty) {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ApplicationAttribute"/>.
    /// <param name="name">Name of the application</param>
    /// <param name="description">Description of the application</param>
    /// </summary>
    public ApplicationAttribute(string name, string description)
      : this(name, description, false) {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ApplicationAttribute"/>.
    /// <param name="name">Name of the application</param>
    /// <param name="description">Description of the application</param>
    /// <param name="restartOnErrors">Flag that indicates if the application should be restarted on exceptions (for services)</param>
    /// </summary>
    public ApplicationAttribute(string name, string description, bool restartOnErrors) {
      if (name == null) throw new ArgumentNullException("name", "Application name is null.");
      if (description == null) throw new ArgumentNullException("description", "Application description is null.");
      this.name = name;
      this.description = description;
      this.restartOnErrors = restartOnErrors;
    }
  }
}
