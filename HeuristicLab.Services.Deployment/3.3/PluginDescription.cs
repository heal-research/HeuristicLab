#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Runtime.Serialization;

namespace HeuristicLab.Services.Deployment {
  [DataContract(Name = "PluginDescription", IsReference = true)]
  public class PluginDescription {

    [DataMember(Name = "Name")]
    private string name;
    public string Name {
      get { return name; }
      set {
        if (string.IsNullOrEmpty(value)) throw new ArgumentException();
        name = value;
      }
    }

    [DataMember(Name = "Version")]
    private Version version;
    public Version Version {
      get { return version; }
      set {
        if (value == null) throw new ArgumentNullException();
        version = value;
      }
    }

    [DataMember(Name = "ContactName")]
    private string contactName;
    public string ContactName {
      get { return contactName; }
      set {
        if (value == null) throw new ArgumentNullException();
        contactName = value;
      }
    }

    [DataMember(Name = "ContactEmail")]
    private string contactEmail;
    public string ContactEmail {
      get { return contactEmail; }
      set {
        if (value == null) throw new ArgumentNullException();
        contactEmail = value;
      }
    }

    [DataMember(Name = "LicenseText")]
    private string licenseText;
    public string LicenseText {
      get { return licenseText; }
      set {
        if (value == null) throw new ArgumentNullException();
        licenseText = value;
      }
    }

    [DataMember(Name = "Dependencies")]
    private List<PluginDescription> dependencies;
    public List<PluginDescription> Dependencies {
      get { return dependencies; }
      set {
        if (value == null) throw new ArgumentNullException();
        dependencies = value;
      }
    }

    public PluginDescription(string name, Version version, IEnumerable<PluginDescription> dependencies,
      string contactName, string contactEmail, string license) {
      if (string.IsNullOrEmpty(name)) throw new ArgumentException("name is empty");
      if (version == null || dependencies == null ||
        contactName == null || contactEmail == null ||
        license == null) throw new ArgumentNullException();
      this.name = name;
      this.version = version;
      this.dependencies = new List<PluginDescription>(dependencies);
      this.licenseText = license;
      this.contactName = contactName;
      this.contactEmail = contactEmail;
    }

    public PluginDescription(string name, Version version)
      : this(name, version, Enumerable.Empty<PluginDescription>()) {
    }

    public PluginDescription(string name, Version version, IEnumerable<PluginDescription> dependencies)
      : this(name, version, dependencies, string.Empty, string.Empty, string.Empty) {
    }
  }
}
