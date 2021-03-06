#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HEAL.Attic;

namespace HeuristicLab.Clients.Hive {
  [StorableType("2590428E-5F97-48EC-AE82-E2F614BEF34C")]
  public partial class Task : LightweightTask {

    public Task() {
      Priority = 1;
    }

    protected Task(Task original, Cloner cloner)
      : base(original, cloner) {
      this.Priority = original.Priority;
      this.CoresNeeded = original.CoresNeeded;
      this.MemoryNeeded = original.MemoryNeeded;
      if (original.PluginsNeededIds != null) {
        this.PluginsNeededIds = new List<Guid>(original.PluginsNeededIds);
      }
      this.LastHeartbeat = original.LastHeartbeat;
      this.IsParentTask = original.IsParentTask;
      this.FinishWhenChildJobsFinished = original.FinishWhenChildJobsFinished;
      this.JobId = original.JobId;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Task(this, cloner);
    }

    public override string ToString() {
      return string.Format("State: {0}, SlaveId: {1}, DateCreated: {2}, CoresNeeded: {3}, MemoryNeeded: {4}",
        State,
        CurrentStateLog != null ? (CurrentStateLog.SlaveId.HasValue ? CurrentStateLog.SlaveId.Value.ToString() : string.Empty) : string.Empty,
        DateCreated.HasValue ? DateCreated.ToString() : string.Empty,
        CoresNeeded,
        MemoryNeeded);
    }
  }
}
