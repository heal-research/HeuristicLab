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

namespace HeuristicLab.PluginInfrastructure.Manager {
  /// <summary>
  /// Class that provides information about an application.
  /// </summary>
  [Serializable]
  public sealed class ApplicationDescription : IApplicationDescription {
    private string name;

    /// <summary>
    /// Gets or sets the name of the application.
    /// </summary>
    public string Name {
      get { return name; }
      internal set { name = value; }
    }
    private Version version;

    /// <summary>
    /// Gets or sets the version of the application.
    /// </summary>
    internal Version Version {
      get { return version; }
      set { version = value; }
    }
    private string description;

    /// <summary>
    /// Gets or sets the description of the application.
    /// </summary>
    public string Description {
      get { return description; }
      internal set { description = value; }
    }

    private bool autoRestart;
    /// <summary>
    /// Gets or sets the boolean flag if the application should be automatically restarted.
    /// </summary>
    internal bool AutoRestart {
      get { return autoRestart; }
      set { autoRestart = value; }
    }

    private string declaringAssemblyName;
    /// <summary>
    /// Gets or sets the name of the assembly that contains the IApplication type.
    /// </summary>
    internal string DeclaringAssemblyName {
      get { return declaringAssemblyName; }
      set { declaringAssemblyName = value; }
    }

    private string declaringTypeName;
    /// <summary>
    /// Gets or sets the name of the type that implements the interface IApplication.
    /// </summary>
    internal string DeclaringTypeName {
      get { return declaringTypeName; }
      set { declaringTypeName = value; }
    }

    public override string ToString() {
      return Name + " " + Version;
    }
  }
}
