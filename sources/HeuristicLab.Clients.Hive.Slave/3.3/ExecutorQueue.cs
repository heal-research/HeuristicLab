#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using HeuristicLab.Clients.Hive.SlaveCore.Properties;

namespace HeuristicLab.Clients.Hive.SlaveCore {
  /// <summary>
  /// Every Executor gets an ExecutorQueue in which it can push messages. 
  /// These messages are then read and processed from outside the appdomain.
  /// </summary>
  public class ExecutorQueue : MarshalByRefObject {
    private Queue<ExecutorMessage> queue = null;
    private Semaphore semaphore = null;

    public ExecutorQueue() {
      queue = new Queue<ExecutorMessage>();
      semaphore = new Semaphore(0, Settings.Default.QueuesMaxThreads);
    }

    /// <summary>
    /// Returns the oldest ExecutorMessage Object from the Queue. 
    /// </summary>
    /// <returns>the oldest ExecutorMessage Object</returns>
    public ExecutorMessage GetMessage() {
      semaphore.WaitOne(Settings.Default.ExecutorQueueTimeout);
      lock (this) {
        if (queue.Count > 0) {
          return queue.Dequeue();
        }
      }
      return null;
    }

    /// <summary>
    /// Adds a ExecutorMessage Object to the Queue 
    /// </summary>
    /// <param name="message">the ExecutorMessage</param>
    public void AddMessage(ExecutorMessage message) {
      lock (this) {
        queue.Enqueue(message);
        semaphore.Release();
      }
    }

    /// <summary>
    /// Adds a message to the Queue. The ExecutorMessage Object is built in the Method
    /// </summary>
    /// <param name="message">the Message</param>
    public void AddMessage(ExecutorMessageType message) {
      lock (this) {
        queue.Enqueue(new ExecutorMessage(message));
        semaphore.Release();
      }
    }
  }
}
