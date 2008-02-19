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
  /// Holds information of loaded plugins that is needed for plugin management.
  /// Used to represent plugins in AppDomains without loading the plugin assemblies.
  /// </summary>
  [Serializable]
  public class PluginInfo {
    private string name;

    public string Name {
      get { return name; }
      set { name = value; }
    }

    private Version version;

    public Version Version {
      get { return version; }
      set { version = value; }
    }

    private List<string> files = new List<string> ();
    /// <summary>
    /// Names of all files that belong to this plugin.
    /// These files are deleted when the plugin is removed or updated.
    /// </summary>
    public List<string> Files {
      get { return files; }
    }

    private List<PluginInfo> dependencies = new List<PluginInfo> ();
    public List<PluginInfo> Dependencies {
      get { return dependencies; }
    }
    private List<string> assemblies = new List<string>();

    /// <summary>
    /// Names of the assemblies that belong to this plugin.
    /// </summary>
    public List<string> Assemblies {
      get { return assemblies; }
      set { assemblies = value; }
    }

    public override string ToString() {
      return Name;
    }

    // equals and hashcode have to be implemented because we want to compare PluginDescriptions from 
    // different AppDomains and serialization destroys reference equality
    public override bool Equals(object obj) {
      if(!(obj is PluginInfo))
        return false;
      PluginInfo other = (PluginInfo)obj;

      return other.Name == this.Name && other.Version == this.Version;
    }

    public override int GetHashCode() {
      return name.GetHashCode() + version.GetHashCode();
    }
  }
}
