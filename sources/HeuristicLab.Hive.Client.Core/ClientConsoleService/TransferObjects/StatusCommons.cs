using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Client.Common;
using System.Runtime.Serialization;

namespace HeuristicLab.Hive.Client.Core.ClientConsoleService {
  [DataContract]
  public class StatusCommons {    
    [DataMember]
    public Guid ClientGuid { get; set; }
    [DataMember]
    public NetworkEnum.WcfConnState Status { get; set; }
    [DataMember]    
    public DateTime ConnectedSince { get; set; }
    [DataMember]
    public int JobsFetched { get; set; }
    [DataMember]
    public int JobsDone { get; set; }
    [DataMember]
    public int JobsAborted { get; set; }
    [DataMember]
    public List<JobStatus> Jobs { get; set; }   
  }
}
