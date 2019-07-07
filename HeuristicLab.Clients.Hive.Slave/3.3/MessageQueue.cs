#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  /// Queue for the communication between threads and AppDomains. The Queue is threadsafe and uses a semaphore
  /// </summary>
  public class MessageQueue : MarshalByRefObject {

    private static MessageQueue instance = null;

    private Queue<MessageContainer> queue = null;
    private Semaphore semaphore = null;

    /// <summary>
    /// Returns the Instance of the MessageQueue. If the instance is null, it will be created.
    /// This is a Implementation of the Singleton pattern
    /// </summary>
    /// <returns>the MessageQueue Instance</returns>
    public static MessageQueue GetInstance() {
      if (instance == null) {
        instance = new MessageQueue();
      }
      return instance;
    }


    /// <summary>
    /// Creates a new MessageQueue Object.
    /// A new Queue and a Semaphore is created. The Semaphore is set to a max size of 5000.
    /// </summary>
    private MessageQueue() {
      queue = new Queue<MessageContainer>();
      semaphore = new Semaphore(0, Settings.Default.QueuesMaxThreads);
    }

    /// <summary>
    /// Returns the oldest MessageContainer Object from the Queue. 
    /// </summary>
    /// <returns>the oldest MessageContainer Object</returns>
    public MessageContainer GetMessage() {
      semaphore.WaitOne();
      lock (this) {
        if (queue.Count > 0) {
          return queue.Dequeue();
        }
      }
      return null;
    }

    /// <summary>
    /// Adds a MessageContainer Object to the Queue 
    /// </summary>
    /// <param name="messageContainer">the MessageContainer</param>
    public void AddMessage(MessageContainer messageContainer) {
      lock (this) {
        queue.Enqueue(messageContainer);
        semaphore.Release();
      }
    }

    /// <summary>
    /// Adds a message to the Queue. The MessageContainer Object is built in the Method
    /// </summary>
    /// <param name="message">the Message</param>
    public void AddMessage(MessageContainer.MessageType message) {
      lock (this) {
        queue.Enqueue(new MessageContainer(message));
        semaphore.Release();
      }
    }
  }
}
