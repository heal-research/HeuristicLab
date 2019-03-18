#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using System.Threading.Tasks;
using HeuristicLab.Common;

namespace HeuristicLab.Core {
  public interface IExecutable : IItem {
    ExecutionState ExecutionState { get; }
    TimeSpan ExecutionTime { get; }

    void Prepare();
    void Start();
    void Start(CancellationToken cancellationToken);
    Task StartAsync();
    Task StartAsync(CancellationToken cancellationToken);
    void Pause();
    void Stop();

    event EventHandler ExecutionStateChanged;
    event EventHandler ExecutionTimeChanged;
    event EventHandler Prepared;
    event EventHandler Started;
    event EventHandler Paused;
    event EventHandler Stopped;
    event EventHandler<EventArgs<Exception>> ExceptionOccurred;
  }
}
