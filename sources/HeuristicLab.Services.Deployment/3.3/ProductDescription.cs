using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
