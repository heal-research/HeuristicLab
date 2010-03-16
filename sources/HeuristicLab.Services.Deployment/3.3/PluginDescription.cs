using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HeuristicLab.Services.Deployment {
  [DataContract(Name = "PluginDescription")]
  public class PluginDescription {

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

    [DataMember(Name = "ContactName")]
    private string contactName;
    public string ContactName {
      get { return contactName; }
    }

    [DataMember(Name = "ContactEmail")]
    private string contactEmail;
    public string ContactEmail {
      get { return contactEmail; }
    }

    [DataMember(Name = "LicenseText")]
    private string licenseText;
    public string LicenseText {
      get { return licenseText; }
    }

    [DataMember(Name = "Dependencies")]
    private List<PluginDescription> dependencies;
    public List<PluginDescription> Dependencies {
      get { return dependencies; }
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
