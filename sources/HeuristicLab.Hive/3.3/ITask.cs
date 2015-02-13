#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Hive {
  public interface ITask : IDeepCloneable, ICloneable {
    TimeSpan ExecutionTime { get; }

    ExecutionState ExecutionState { get; }

    /// <summary>
    /// indicates wether it is possible to create childjobs from this job
    /// </summary>
    bool IsParallelizable { get; set; }

    /// <summary>
    /// Configuration to indicate if this job should create child-jobs which will be executed in hive
    /// Cannot be set true if IsParallelizable is false
    /// </summary>
    bool ComputeInParallel { get; set; }


    void Prepare();

    void Start();

    void Pause();

    void Stop();

    event EventHandler ComputeInParallelChanged;

    event EventHandler ExecutionTimeChanged;

    event EventHandler ExecutionStateChanged;

    event EventHandler TaskFailed;

    event EventHandler TaskStopped;

    event EventHandler TaskPaused;

    event EventHandler TaskStarted;
  }
}