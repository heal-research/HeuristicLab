using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.PluginInfrastructure;
using System.Diagnostics;
using HeuristicLab.Hive.Client.Common;

namespace HeuristicLab.Hive.Client.Core {
  [ClassInfo(Name = "Hive Client Core", Description = "Hive Client Core baseclass", AutoRestart = true)]
  public class CoreApplication: ApplicationBase {
    public override void Run() {
      Logging.getInstance().Info(this.Name, "Info Message");
      Logging.getInstance().Error(this.Name, "Error Message");
      Logging.getInstance().Error(this.Name, "Exception Message", new Exception("Exception"));
    }
  }
}
