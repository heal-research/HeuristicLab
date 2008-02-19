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
  [Serializable]
  public class ApplicationInfo {
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
    private string description;

    public string Description {
      get { return description; }
      set { description = value; }
    }

    private string pluginAssembly;
    /// <summary>
    /// Name of the assembly that contains the IApplication type.
    /// NEEDED?
    /// </summary>
    public string PluginAssembly {
      get { return pluginAssembly; }
      set { pluginAssembly = value; }
    }

    private string pluginType;
    /// <summary>
    /// Name of the type that implements the interface IApplication.
    /// NEEDED?
    /// </summary>
    public string PluginType {
      get { return pluginType; }
      set { pluginType = value; }
    }
  }
}
