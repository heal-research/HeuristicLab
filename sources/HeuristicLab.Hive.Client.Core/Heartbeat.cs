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
//using BO = HeuristicLab.Hive.Contracts.BusinessObjects;

namespace HeuristicLab.Hive.Client.Core {
  /// <summary>
  /// Heartbeat class. It sends every x ms a heartbeat to the server and receives a Message
  /// </summary>
  public class Heartbeat {
    public double Interval { get; set; }    
    private Timer heartbeatTimer = null;
        
    public Heartbeat() {
      Interval = 100;
    }

    public Heartbeat(double interval) {
      Interval = interval;      
    }

    private ClientCommunicatorClient clientCommunicator;

    /// <summary>
    /// Starts the Heartbeat signal.
    /// </summary>
    public void StartHeartbeat() {
      heartbeatTimer = new System.Timers.Timer();
      heartbeatTimer.Interval = this.Interval;
      heartbeatTimer.AutoReset = true;
      heartbeatTimer.Elapsed += new ElapsedEventHandler(heartbeatTimer_Elapsed);
      clientCommunicator = ServiceLocator.GetClientCommunicator();
      clientCommunicator.SendHeartBeatCompleted += new EventHandler<SendHeartBeatCompletedEventArgs>(ClientCommunicator_SendHeartBeatCompleted);
      heartbeatTimer.Start();
    }

    /// <summary>
    /// This Method is called every time the timer ticks
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void heartbeatTimer_Elapsed(object sender, ElapsedEventArgs e) {
      Console.WriteLine("tick");
      HeartBeatData heartBeatData = new HeartBeatData { ClientId = Guid.NewGuid(), 
                                                              freeCores = 4, 
                                                              freeMemory = 1000, 
                                                              jobProgress = 1};
      clientCommunicator.SendHeartBeatAsync(heartBeatData);
    }

    void ClientCommunicator_SendHeartBeatCompleted(object sender, SendHeartBeatCompletedEventArgs e) {
      System.Diagnostics.Debug.WriteLine("Heartbeat received! " + e.Result.ActionRequest.ToString());
      foreach (MessageContainer mc in e.Result.ActionRequest) {
        MessageQueue.GetInstance().AddMessage(mc);

      }
    }
  }
}
