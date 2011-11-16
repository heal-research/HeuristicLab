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
using System.Runtime.Serialization;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Services.Hive {
  /// <summary>
  /// The MessageContainer is a container class for Messages. Its two parts are:
  /// the actual message itself and the TaskId, refered by the message
  /// </summary>
  [StorableClass]
  [Serializable]
  [DataContract]
  public class MessageContainer : IDeepCloneable {

    public enum MessageType {
      // *** commands from hive server ***
      CalculateTask, // slave should calculate a task. the task is already assigned to the slave
      StopTask,   // slave should stop the task and submit results
      StopAll,   // stop all and submit results
      AbortTask,  // slave should shut the task down immediately without submitting results
      AbortAll,  // slave should abort all task immediately
      PauseTask,  // pause the task and submit the results   
      PauseAll,  // pause all task and submit results
      Restart,   // restart operation after Sleep
      Sleep,     // disconnect from server, but don't shutdown
      ShutdownSlave,  // slave should shutdown immediately without submitting results
      SayHello,  // Slave should say hello, because job is unknown to the server
      NewHBInterval,
    };

    [Storable]
    [DataMember]
    public MessageType Message { get; set; }

    [Storable]
    [DataMember]
    public Guid TaskId { get; set; }

    [StorableConstructor]
    protected MessageContainer(bool deserializing) { }
    protected MessageContainer() { }
    public MessageContainer(MessageType message) {
      Message = message;
      TaskId = Guid.Empty;
    }
    public MessageContainer(MessageType message, Guid jobId) {
      Message = message;
      TaskId = jobId;
    }
    protected MessageContainer(MessageContainer original, Cloner cloner) {
      cloner.RegisterClonedObject(original, this);
      this.Message = original.Message;
      this.TaskId = original.TaskId;
    }
    public virtual IDeepCloneable Clone(Cloner cloner) {
      return new MessageContainer(this, cloner);
    }
    public object Clone() {
      return Clone(new Cloner());
    }
  }
}
