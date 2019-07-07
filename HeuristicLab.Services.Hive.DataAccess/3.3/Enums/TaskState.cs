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

namespace HeuristicLab.Services.Hive.DataAccess {
  [Serializable]
  public enum TaskState {
    /// <summary>
    /// A job is offline as long as he is not yet submitted to the hive
    /// </summary>
    Offline,

    /// <summary>
    /// Job is waiting to be calculated
    /// </summary>
    Waiting,

    /// <summary>
    /// Job is beeing transferred
    /// </summary>
    Transferring,

    /// <summary>
    /// Job is actively calculated on a Slave
    /// </summary>
    Calculating,

    /// <summary>
    /// Job is paused, will not be picked up by slaves
    /// </summary>
    Paused,

    /// <summary>
    /// Job as finished and is ready to be collected by the Client
    /// </summary>
    Finished,

    /// <summary>
    /// Job is aborted and result can be collected by the Client
    /// </summary>
    Aborted,

    /// <summary>
    /// Job as been aborted due to an error. Results are ready to be collected
    /// </summary>
    Failed
  };
}
