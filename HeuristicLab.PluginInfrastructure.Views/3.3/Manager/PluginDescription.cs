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

namespace HeuristicLab.PluginInfrastructure.Manager {
  /// <summary>
  /// Holds information of loaded plugins that is needed for plugin management.
  /// Used to represent plugins in AppDomains without loading the plugin assemblies.
  /// </summary>
  [Serializable]
  public sealed class PluginDescription : IPluginDescription {
    private int nTimesLoaded;

    private string name;
    /// <summary>
    /// Gets the name of the plugin.
    /// </summary>
    public string Name {
      get { return name; }
      internal set { name = value; }
    }

    private string description;
    /// <summary>
    /// Gets or sets the description of the plugin.
    /// </summary>
    public string Description {
      get { return description; }
      internal set { description = value; }
    }
    private Version version;
    /// <summary>
    /// Gets the version of the plugin.
    /// </summary>
    public Version Version {
      get { return version; }
      internal set { version = value; }
    }

    private string contactName;
    /// <summary>
    /// Gets or sets the name of the contact person for this plugin.
    /// </summary>
    public string ContactName {
      get { return contactName; }
      internal set { contactName = value; }
    }

    private string contactEmail;
    /// <summary>
    /// Gets or sets the e-mail address of the contact person for this plugin.
    /// </summary>
    public string ContactEmail {
      get { return contactEmail; }
      internal set { contactEmail = value; }
    }
    private string licenseText;
    /// <summary>
    /// Gets or sets the license text of the plugin.
    /// </summary>
    public string LicenseText {
      get { return licenseText; }
      internal set { licenseText = value; }
    }

    private PluginState pluginState;
    /// <summary>
    /// Gets or sets the plugin state.
    /// </summary>
    public PluginState PluginState {
      get { return pluginState; }
    }

    private string loadingErrorInformation;
    /// <summary>
    /// Gets the error message why this plugin has been disabled.
    /// </summary>
    internal string LoadingErrorInformation {
      get {
        return loadingErrorInformation;
      }
    }

    private List<PluginFile> files = new List<PluginFile>();
    /// <summary>
    /// Gets the names of all files that belong to this plugin.
    /// These files are deleted when the plugin is removed or updated.
    /// </summary>
    public IEnumerable<IPluginFile> Files {
      get { return files.Cast<IPluginFile>(); }
    }

    internal void AddFiles(IEnumerable<PluginFile> fileNames) {
      files.AddRange(fileNames);
    }

    private List<PluginDescription> dependencies = new List<PluginDescription>();
    internal IEnumerable<PluginDescription> Dependencies {
      get { return dependencies; }
    }
    /// <summary>
    /// Gets all dependencies of the plugin.
    /// </summary>
    IEnumerable<IPluginDescription> IPluginDescription.Dependencies {
      get { return dependencies.Cast<IPluginDescription>(); }
    }

    internal void AddDependency(PluginDescription dependency) {
      dependencies.Add(dependency);
    }


    /// <summary>
    /// Gets the locations (file names) of the assemblies that belong to this plugin.
    /// </summary>
    public IEnumerable<string> AssemblyLocations {
      get { return Files.Where(f => f.Type == PluginFileType.Assembly).Select(f => f.Name); }
    }

    /// <summary>
    /// Gets and sets the list of assembly names for this plugin. Assembly names are only available after the plugin has been loaded.
    /// </summary>
    private List<string> assemblyNames;
    public IEnumerable<string> AssemblyNames {
      get { return assemblyNames; }
      set { this.assemblyNames = new List<string>(value); }
    }

    internal PluginDescription() {
      pluginState = PluginState.Undefined;
    }

    internal void Disable(string loadingErrorInformation) {
      if (pluginState != PluginState.Undefined)
        throw new InvalidOperationException("Cannot disable a plugin in state " + pluginState);
      pluginState = PluginState.Disabled;
      this.loadingErrorInformation = loadingErrorInformation;
    }

    internal void Enable() {
      if (pluginState != PluginState.Undefined)
        throw new InvalidOperationException("Cannot enable a plugin in state " + pluginState);
      pluginState = PluginState.Enabled;
    }

    internal void Load() {
      if (!(pluginState == PluginState.Enabled || pluginState == PluginState.Loaded))
        throw new InvalidOperationException("Cannot load a plugin in state " + pluginState);
      pluginState = PluginState.Loaded;
      nTimesLoaded++;
    }

    internal void Unload() {
      if (pluginState != PluginState.Loaded)
        throw new InvalidOperationException("Cannot unload a plugin in state " + pluginState);
      nTimesLoaded--;
      if (nTimesLoaded == 0) pluginState = PluginState.Enabled;
    }


    /// <summary>
    /// Gets the string representation of the plugin.
    /// </summary>
    /// <returns>The name of the plugin.</returns>
    public override string ToString() {
      return Name + " " + Version;
    }

    // equals and hashcode have to be implemented because we want to compare PluginDescriptions from 
    // different AppDomains and serialization destroys reference equality
    /// <summary>
    /// Checks whether the given object is equal to the current plugin.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><c>true</c> if it is equal, <c>false</c> otherwise.</returns>
    public override bool Equals(object obj) {
      PluginDescription other = obj as PluginDescription;
      if (other == null) return false;

      return other.Name == this.Name && other.Version == this.Version;
    }
    /// <summary>
    /// Gets the hash code of the current plugin.
    /// </summary>
    /// <returns>The hash code of the plugin.</returns>
    public override int GetHashCode() {
      if (version != null) {
        return name.GetHashCode() + version.GetHashCode();
      } else return name.GetHashCode();
    }
  }
}
