using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Hive.Client.Core.ConfigurationManager {
  public class ClientStatusInfo {
    static public int JobsProcessed { get; set; }
    static public int JobsAborted { get; set; }
    static public int JobsFetched { get; set; }
    static public DateTime LoginTime { get; set; }
    
  }
}
