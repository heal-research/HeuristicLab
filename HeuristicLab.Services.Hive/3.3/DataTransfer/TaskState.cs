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

namespace HeuristicLab.Services.Hive.DataTransfer {
  [Serializable]
  public enum TaskState {
    /// <summary>
    /// A task is offline as long as job is not yet submitted to the hive
    /// </summary>
    Offline,

    /// <summary>
    /// Task is waiting to be calculated
    /// </summary>
    Waiting,

    /// <summary>
    /// Task is beeing transferred
    /// </summary>
    Transferring,

    /// <summary>
    /// Task is actively calculated on a Slave
    /// </summary>
    Calculating,

    /// <summary>
    /// Task is paused, will not be picked up by slaves
    /// </summary>
    Paused,

    /// <summary>
    /// Task as finished and is ready to be collected by the Client
    /// </summary>
    Finished,

    /// <summary>
    /// Task is aborted and result can be collected by the Client
    /// </summary>
    Aborted,

    /// <summary>
    /// Task as been aborted due to an error. Results are ready to be collected
    /// </summary>
    Failed
  };

  public static class TaskStateExtensions {
    /// <summary>
    /// This task is not yet done
    /// </summary>
    public static bool IsActive(this TaskState taskState) {
      return !taskState.IsDone();
    }

    /// <summary>
    /// This task is Waiting
    /// </summary>
    public static bool IsWaiting(this TaskState taskState) {
      return taskState == TaskState.Waiting;
    }

    /// <summary>
    /// This task is Finished || Failed || Aborted
    /// </summary>
    public static bool IsDone(this TaskState taskState) {
      return taskState == TaskState.Finished ||
        taskState == TaskState.Aborted ||
        taskState == TaskState.Failed;
    }
  }
}
