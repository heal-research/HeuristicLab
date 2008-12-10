using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HeuristicLab.Hive.Client.Core.ClientConsoleService {
  [DataContract]
  public class JobStatus {
    [DataMember]
    public long JobId { get; set; }
    [DataMember]
    public DateTime Since { get; set; }
    [DataMember]
    public double Progress { get; set; }
  }
}
