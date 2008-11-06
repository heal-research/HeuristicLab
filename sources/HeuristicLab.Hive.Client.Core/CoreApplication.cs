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
      //EventLog logger = new EventLog();
      //logger.Log = "Hive Client Core";
      //logger.Source = "CoreApplication";
      //logger.WriteEntry("Program has Started");
      //logger.Close();
      Console.WriteLine("awesome");
    }
  }
}
