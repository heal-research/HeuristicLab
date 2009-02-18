using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Hive.Client.Core.JobStorrage {
  public class JobStorrageInfo {
    public String ServerIP { get; set; }
    public long ServerPort { get; set; }
    public DateTime TimeFinished { get; set; }
    public long JobID { get; set; }
  }
}
