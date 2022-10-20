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
using System.Linq;

namespace HeuristicLab.PluginInfrastructure.Advanced.DeploymentService {
  // extension of auto-generated DataContract class PluginDescription
  public partial class PluginDescription : IPluginDescription {
    /// <summary>
    /// Initializes an new instance of <see cref="PluginDescription" />
    /// with no dependencies, empty contact details and empty license text.
    /// </summary>
    /// <param name="name">Name of the plugin</param>
    /// <param name="version">Version of the plugin</param>
    public PluginDescription(string name, Version version) : this(name, version, new List<PluginDescription>()) { }
    /// <summary>
    /// Initializes a new instance of <see cref="PluginDescription" /> 
    /// with empty contact details and empty license text.
    /// </summary>
    /// <param name="name">Name of the plugin</param>
    /// <param name="version">Version of the plugin</param>
    /// <param name="dependencies">Enumerable of dependencies of the plugin</param>
    public PluginDescription(string name, Version version, IEnumerable<PluginDescription> dependencies)
      : this(name, version, dependencies, string.Empty, string.Empty, string.Empty) {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="PluginDescription" />.
    /// </summary>
    /// <param name="name">Name of the plugin</param>
    /// <param name="version">Version of the plugin</param>
    /// <param name="dependencies">Enumerable of dependencies of the plugin</param>
    /// <param name="contactName">Name of the contact person for the plugin</param>
    /// <param name="contactEmail">E-mail of the contact person for the plugin</param>
    /// <param name="licenseText">License text for the plugin</param>
    public PluginDescription(string name, Version version, IEnumerable<PluginDescription> dependencies, string contactName, string contactEmail, string licenseText) {
      this.Name = name;
      this.Version = version;
      this.Dependencies = dependencies.ToArray();
      this.LicenseText = licenseText;
    }

    #region IPluginDescription Members
    /// <summary>
    /// Gets the description of the plugin. Always string.Empty.
    /// </summary>
    public string Description {
      get { return string.Empty; }
    }

    /// <summary>
    /// Gets an enumerable of dependencies of the plugin
    /// </summary>
    IEnumerable<IPluginDescription> IPluginDescription.Dependencies {
      get {
        return Dependencies;
      }
    }

    /// <summary>
    /// Gets and enumerable of files that are part of this pluing. Always empty.
    /// </summary>
    public IEnumerable<IPluginFile> Files {
      get { return Enumerable.Empty<IPluginFile>(); }
    }

    #endregion

    /// <summary>
    /// ToString override
    /// </summary>
    /// <returns>String representation of the PluginDescription (name + version)</returns>
    public override string ToString() {
      return Name + " " + Version;
    }

    public override bool Equals(object obj) {
      PluginDescription other = obj as PluginDescription;
      if (other == null) return false;
      else return other.Name == this.Name && other.Version == this.Version;
    }

    public override int GetHashCode() {
      return Name.GetHashCode() + Version.GetHashCode();
    }
  }
}
