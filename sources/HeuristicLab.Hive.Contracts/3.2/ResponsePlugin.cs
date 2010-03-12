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
      Plugins = new List<CachedHivePluginInfoDto>();
    }

    [DataMember]
    public List<CachedHivePluginInfoDto> Plugins { get; set; }
  }
}
