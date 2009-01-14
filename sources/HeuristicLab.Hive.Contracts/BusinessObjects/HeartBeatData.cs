using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HeuristicLab.Hive.Contracts.BusinessObjects {

  [DataContract]
  public class HeartBeatData {
    [DataMember]
    public Guid ClientId { get; set; }
    [DataMember]
    public int freeMemory { get; set; }
    [DataMember]
    public int freeCores { get; set; }
    [DataMember]
    public Dictionary<long, double> jobProgress { get; set; } // TODO: define Type 
  }
}
