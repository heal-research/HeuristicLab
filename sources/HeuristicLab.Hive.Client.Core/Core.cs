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
using HeuristicLab.Hive.Client.ExecutionEngine;
using HeuristicLab.Hive.Client.Common;
using System.Threading;


namespace HeuristicLab.Hive.Client.Core {
  public class Core {

    Dictionary<long, Executor> engines = new Dictionary<long, Executor>();
    
    public void Start() {
      Logging.getInstance().Info(this.ToString(), "Info Message");
      //Logging.getInstance().Error(this.ToString(), "Error Message");
      //Logging.getInstance().Error(this.ToString(), "Exception Message", new Exception("Exception"));      

      Heartbeat beat = new Heartbeat();
      beat.Interval = 5000;
      beat.StartHeartbeat();

      MessageQueue queue = MessageQueue.GetInstance();
      
      JobBase job = new TestJob();
      
      ExecutionEngine.Executor engine = new ExecutionEngine.Executor();
      engine.Job = job;
      engine.JobId = 1L;
      engine.Queue = queue;      
      engine.Start();
      engines.Add(engine.JobId, engine);

      Thread.Sleep(15000);
      engine.RequestSnapshot();
      while (true) {
        MessageContainer container = queue.GetMessage();
        Logging.getInstance().Info(this.ToString(), container.Message.ToString()); 
        DetermineAction(container);

        
      }
    }

    private void DetermineAction(MessageContainer container) {
      if(container.Message == MessageContainer.MessageType.AbortJob)
        engines[container.JobId].Abort();
      else if (container.Message == MessageContainer.MessageType.JobAborted)
        //kill appdomain
        Console.WriteLine("tmp");
      else if (container.Message == MessageContainer.MessageType.RequestSnapshot)
        engines[container.JobId].RequestSnapshot();
      else if (container.Message == MessageContainer.MessageType.SnapshotReady)
        // must be async!
        engines[container.JobId].GetSnapshot();
    }        
  }
}
