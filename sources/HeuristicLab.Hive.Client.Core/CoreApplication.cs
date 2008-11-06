using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.PluginInfrastructure;
using System.Diagnostics;

namespace HeuristicLab.Hive.Client.Core {
  [ClassInfo(Name = "Hive Client Core", Description = "Hive Client Core baseclass", AutoRestart = true)]
  public class CoreApplication: ApplicationBase {
    public override void Run() {
      //EventLog logger = new EventLog(      
      Console.WriteLine("awesome");
    }
  }
}
