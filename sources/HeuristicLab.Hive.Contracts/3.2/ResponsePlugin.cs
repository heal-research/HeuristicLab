using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Hive.Contracts {

  [DataContract]
  public class ResponsePlugin : Response {
    [DataMember]
    public List<CachedHivePluginInfo> Plugins { get; set; }
  }
}
