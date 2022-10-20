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
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("HeuristicLab.PluginInfrastructure.Extension-3.3, PublicKey=0024000004800000940000000602000000240000525341310004000001000100e3d38bc66a0dd8dd36f57285e34632ec04b3049866ab1e64cf230e95ffcbfbb90c437b4d11dfe74ba981f746274290bb03f3e636e139e685b501031dc6e0bc8409153f0c842721eb9e8e2a703c9e4d102283f3ddbdfab4078c08de51869715992a694d2f608d0fa865c9d17c06b8d6a9135004e982fd862cdb2277e4ad15a4a6")]
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
