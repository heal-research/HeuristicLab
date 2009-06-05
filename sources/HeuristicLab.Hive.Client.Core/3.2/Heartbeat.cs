#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using HeuristicLab.Hive.Client.Common;
using HeuristicLab.Hive.Client.Communication;
using System.Diagnostics;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Contracts;
using HeuristicLab.Hive.Client.Core.ConfigurationManager;
using HeuristicLab.Hive.Client.Communication.ServerService;
//using BO = HeuristicLab.Hive.Contracts.BusinessObjects;

namespace HeuristicLab.Hive.Client.Core {
  /// <summary>
  /// Heartbeat class. It sends every x ms a heartbeat to the server and receives a Message
  /// </summary>
  public class Heartbeat {

    private bool offline;

    public double Interval { get; set; }    
    private Timer heartbeatTimer = null;
        
    public Heartbeat() {
      Interval = 100;
    }

    public Heartbeat(double interval) {
      Interval = interval;      
    }

    private WcfService wcfService;

    /// <summary>
    /// Starts the Heartbeat signal.
    /// </summary>
    public void StartHeartbeat() {
      heartbeatTimer = new System.Timers.Timer();
      heartbeatTimer.Interval = this.Interval;
      heartbeatTimer.AutoReset = true;
      heartbeatTimer.Elapsed += new ElapsedEventHandler(heartbeatTimer_Elapsed);
      wcfService = WcfService.Instance;
      wcfService.SendHeartBeatCompleted += new EventHandler<ProcessHeartBeatCompletedEventArgs>(wcfService_ProcessHeartBeatCompleted);
      heartbeatTimer.Start();
    }

    /// <summary>
    /// This Method is called every time the timer ticks
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void heartbeatTimer_Elapsed(object sender, ElapsedEventArgs e) {
      Console.WriteLine("tick");  
      ClientInfo info = ConfigManager.Instance.GetClientInfo();      

      PerformanceCounter counter = new PerformanceCounter("Memory", "Available Bytes", true);
      int mb = (int)(counter.NextValue() / 1024 / 1024);

      HeartBeatData heartBeatData = new HeartBeatData {
        ClientId = info.Id,
        FreeCores = info.NrOfCores - ConfigManager.Instance.GetUsedCores(),
        FreeMemory = mb,
        JobProgress = ConfigManager.Instance.GetProgressOfAllJobs()      
      };
      
      DateTime lastFullHour = DateTime.Parse(DateTime.Now.Hour.ToString() + ":00");
      TimeSpan span = DateTime.Now - lastFullHour;
      if (span.TotalSeconds < (Interval/1000)) {
        if (UptimeManager.Instance.isOnline()) {
          //That's quiet simple: Just reconnect and you're good for new jobs
          if (wcfService.ConnState != NetworkEnum.WcfConnState.Connected) {
            Logging.Instance.Info(this.ToString(), "Client goes online according to timetable");
            wcfService.Connect();
          }
        } else {
          //We have quit a lot of work to do here: snapshot all jobs, submit them back, then disconnect and then pray to god that nothing goes wrong
          MessageQueue.GetInstance().AddMessage(MessageContainer.MessageType.UptimeLimitDisconnect);                  
        }        
      }
      if (wcfService.ConnState == NetworkEnum.WcfConnState.Failed) {
        wcfService.Connect();
      } else if (wcfService.ConnState == NetworkEnum.WcfConnState.Loggedin) {
        wcfService.SendHeartBeatAsync(heartBeatData);
      }
    }

    void wcfService_ProcessHeartBeatCompleted(object sender, ProcessHeartBeatCompletedEventArgs e) {
      System.Diagnostics.Debug.WriteLine("Heartbeat received! ");
      e.Result.ActionRequest.ForEach(mc => MessageQueue.GetInstance().AddMessage(mc));      
    }

    public void StopHeartBeat() {
      heartbeatTimer.Dispose();
    }
  }
}
