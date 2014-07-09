#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Runtime.Serialization;

namespace HeuristicLab.Services.Hive.DataTransfer {
  [DataContract]
  [Serializable]
  public class LightweightTask : HiveItem {
    [DataMember]
    public TimeSpan ExecutionTime { get; set; }
    [DataMember]
    public Guid? ParentTaskId { get; set; }
    [DataMember]
    public List<StateLog> StateLog { get; set; }
    [DataMember]
    public TaskState State { get; set; }
    [DataMember]
    public Command? Command { get; set; }
    [DataMember]
    public DateTime LastTaskDataUpdate { get; set; }

    public StateLog CurrentStateLog { get { return StateLog.LastOrDefault(); } }
    public DateTime? DateCreated { get { return StateLog.Count > 0 ? new DateTime?(StateLog.First().DateTime) : null; } }
    public DateTime? DateFinished { get { return (StateLog.Count > 0 && CurrentStateLog.State == TaskState.Finished) ? new DateTime?(CurrentStateLog.DateTime) : null; } }

    public LightweightTask() {
      StateLog = new List<DataTransfer.StateLog>();
    }

    public LightweightTask(Task task) {
      this.Id = task.Id;
      this.ExecutionTime = task.ExecutionTime;
      this.ParentTaskId = task.ParentTaskId;
      this.StateLog = new List<StateLog>(task.StateLog);
      this.State = task.State;
      this.Command = task.Command;
      this.LastTaskDataUpdate = task.LastTaskDataUpdate;
    }
  }
}
