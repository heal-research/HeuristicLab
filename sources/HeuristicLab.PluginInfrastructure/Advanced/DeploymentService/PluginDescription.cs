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
using System.Linq;
using System.Text;
using HeuristicLab.PluginInfrastructure.Manager;
using System.IO;
using System.ComponentModel;
using System.Reflection;

namespace HeuristicLab.PluginInfrastructure.Advanced.DeploymentService {
  // extension of auto-generated DataContract class PluginDescription
  public partial class PluginDescription : IPluginDescription {
    public PluginDescription(string name, Version version) : this(name, version, new List<PluginDescription>()) { }
    public PluginDescription(string name, Version version, IEnumerable<PluginDescription> dependencies)
      : this(name, version, dependencies, string.Empty, string.Empty, string.Empty) {
    }

    public PluginDescription(string name, Version version, IEnumerable<PluginDescription> dependencies, string contactName, string contactEmail, string licenseText) {
      this.Name = name;
      this.Version = version;
      this.Dependencies = dependencies.ToArray();
      this.LicenseText = licenseText;
    }

    #region IPluginDescription Members
    public string Description {
      get { return string.Empty; }
    }

    [Obsolete]
    public DateTime BuildDate {
      get { throw new NotImplementedException(); }
    }

    IEnumerable<IPluginDescription> IPluginDescription.Dependencies {
      get {
        return Dependencies;
      }
    }

    public IEnumerable<IPluginFile> Files {
      get { return Enumerable.Empty<IPluginFile>(); }
    }

    #endregion

    public override string ToString() {
      return Name + " " + Version;
    }
  }
}
