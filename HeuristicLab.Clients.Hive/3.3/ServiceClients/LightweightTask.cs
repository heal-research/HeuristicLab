#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;

namespace HeuristicLab.Clients.Hive {
  public partial class LightweightTask : IDeepCloneable, IContent {
    public StateLog CurrentStateLog { get { return StateLog.LastOrDefault(); } }
    public DateTime? DateCreated { get { return StateLog.Count > 0 ? new DateTime?(StateLog.First().DateTime) : null; } }
    public DateTime? DateFinished { get { return (StateLog.Count > 0 && CurrentStateLog.State == TaskState.Finished) ? new DateTime?(CurrentStateLog.DateTime) : null; } }

    public LightweightTask() {
      StateLog = new List<StateLog>();
    }

    public LightweightTask(Task job) {
      this.Id = job.Id;
      this.ExecutionTime = job.ExecutionTime;
      this.ParentTaskId = job.ParentTaskId;
      this.StateLog = new List<StateLog>(job.StateLog);
      this.State = job.State;
      this.Command = job.Command;
      this.LastTaskDataUpdate = job.LastTaskDataUpdate;
    }

    protected LightweightTask(LightweightTask original, Cloner cloner)
      : base(original, cloner) {
      this.ExecutionTime = original.ExecutionTime;
      this.ParentTaskId = original.ParentTaskId;
      this.StateLog = new List<StateLog>(original.StateLog);
      this.State = original.State;
      this.Command = original.Command;
      this.LastTaskDataUpdate = original.LastTaskDataUpdate;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new LightweightTask(this, cloner);
    }
  }
}
