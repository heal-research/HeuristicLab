using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Client.Common;

namespace HeuristicLab.Hive.Client.Core.ClientConsoleService {
  [Serializable]
  public class StatusCommons {    
    public Guid ClientGuid { get; set; }
    public NetworkEnum.WcfConnState Status { get; set; }
    public DateTime ConnectedSince { get; set; }
    public int JobsFetched { get; set; }
    public int JobsDone { get; set; }
    public int JobsAborted { get; set; }

    public List<JobStatus> Jobs { get; set; }   
  }
}
