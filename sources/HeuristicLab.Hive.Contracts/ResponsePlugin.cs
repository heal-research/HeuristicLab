using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HeuristicLab.Hive.Contracts {

  [DataContract]
  public class ResponsePlugin : Response {
    [DataMember]
    public byte[] Plugins { get; set; }
  }
}
