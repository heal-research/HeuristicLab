using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Hive.Client.Core.JobStorage {
  public class JobStorageInfo {
    public String ServerIP { get; set; }
    public long ServerPort { get; set; }
    public DateTime TimeFinished { get; set; }
    public Guid JobID { get; set; }
  }
}
