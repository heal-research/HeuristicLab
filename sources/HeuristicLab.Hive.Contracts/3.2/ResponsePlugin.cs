using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Hive.Contracts {

  [DataContract]
  [Serializable]
  public class ResponsePlugin : Response {
    public ResponsePlugin() {
      Plugins = new List<CachedHivePluginInfo>();
    }

    [DataMember]
    public List<CachedHivePluginInfo> Plugins { get; set; }
  }
}
