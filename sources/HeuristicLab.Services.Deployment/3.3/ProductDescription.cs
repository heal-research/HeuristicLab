#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [DataContract(Name = "ProductDescription")]
  public class ProductDescription {
    [DataMember(Name = "Name")]
    private string name;
    public string Name {
      get { return name; }
    }

    [DataMember(Name = "Version")]
    private Version version;
    public Version Version {
      get { return version; }
    }

    [DataMember(Name = "Plugins")]
    private IEnumerable<PluginDescription> plugins;
    public IEnumerable<PluginDescription> Plugins {
      get { return plugins; }
    }

    public ProductDescription(string name, Version version, IEnumerable<PluginDescription> plugins) {
      if (string.IsNullOrEmpty(name)) throw new ArgumentException("name is empty");
      if (version == null || plugins == null) throw new ArgumentNullException();
      this.name = name;
      this.version = version;
      this.plugins = new List<PluginDescription>(plugins).AsReadOnly();
    }

    public ProductDescription(string name, Version version) : this(name, version, Enumerable.Empty<PluginDescription>()) { }
  }
}
