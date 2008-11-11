using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.PluginInfrastructure;
using System.Diagnostics;
using HeuristicLab.Hive.Client.Common;
using System.Timers;
using System.Threading;

namespace HeuristicLab.Hive.Client.Core {
  [ClassInfo(Name = "Hive Client Core", Description = "Hive Client Core baseclass", AutoRestart = true)]
  public class CoreApplication: ApplicationBase {
    public override void Run() {
 
      //Logging.getInstance().Info(this.Name, "Info Message");
      //Logging.getInstance().Error(this.Name, "Error Message");
      //Logging.getInstance().Error(this.Name, "Exception Message", new Exception("Exception"));
      
      Heartbeat beat = new Heartbeat();
      beat.Interval = 1000;
      beat.StartHeartbeat();
      MessageQueue queue = MessageQueue.GetInstance();
      
      while (true) {
        MessageContainer container = queue.GetMessage();
        Console.WriteLine(container.Message.ToString());
      }

      
    }
  }
}
