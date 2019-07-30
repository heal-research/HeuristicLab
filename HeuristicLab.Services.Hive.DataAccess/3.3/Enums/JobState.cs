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

namespace HeuristicLab.Services.Hive.DataAccess {
  [Serializable]
  public enum JobState {
    /// <summary>
    /// A job is online as long as he is no other state.
    /// </summary>
    Online,

    /// <summary>
    /// A job is in StatisticsPending if its deletion has been requested,
    /// but the final generation of statistics hasn't been performed yet.
    /// </summary>
    StatisticsPending,

    /// <summary>
    /// A job is in DeletionPending if its deletion has been requested,
    /// the final generation of statistics has already been performed,
    /// but the eventual deletion of the job is still pending. 
    /// </summary>
    DeletionPending
  };
}
