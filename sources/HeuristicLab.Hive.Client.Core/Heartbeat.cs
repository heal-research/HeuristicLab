using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using HeuristicLab.Hive.Client.Common;

namespace HeuristicLab.Hive.Client.Core {
  public class Heartbeat {
    public double Interval { get; set; }
    
    private Timer heartbeatTimer = null;
        
    public Heartbeat() {
      Interval = 100;
    }

    public Heartbeat(double interval) {
      Interval = interval;      
    }

    public void StartHeartbeat() {
      heartbeatTimer = new System.Timers.Timer();
      heartbeatTimer.Interval = this.Interval;
      heartbeatTimer.AutoReset = true;
      heartbeatTimer.Elapsed += new ElapsedEventHandler(heartbeatTimer_Elapsed);
      heartbeatTimer.Start();               
    }

    void heartbeatTimer_Elapsed(object sender, ElapsedEventArgs e) {
      Console.WriteLine("tick");
      MessageQueue.GetInstance().AddMessage(MessageQueue.MessageType.FetchJob);
    }

  }
}
