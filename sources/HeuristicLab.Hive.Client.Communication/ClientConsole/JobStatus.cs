using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Hive.Client.Communication.ClientConsole {
  [Serializable]
  public class JobStatus {
    public long JobId { get; set; }
    public DateTime Since { get; set; }
    public double Progress { get; set; }
  }
}
