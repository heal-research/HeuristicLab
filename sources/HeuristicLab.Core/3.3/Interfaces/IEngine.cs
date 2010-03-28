#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Core {
  /// <summary>
  /// Interface to represent one run. (An engine is an interpreter, holding the code, 
  /// the data and the actual state, which is the runtime stack and a pointer onto the next operation.).
  /// It is responsible for operator execution and able to deal with parallelism.
  /// </summary>
  public interface IEngine : IItem {
    /// <summary>
    /// Gets the execution time of the current instance.
    /// </summary>
    TimeSpan ExecutionTime { get; }

    /// <summary>
    /// Gets information whether the engine is currently running.
    /// </summary>
    bool Running { get; }
    /// <summary>
    /// Gets information whether the engine has already terminated.
    /// </summary>
    bool Finished { get; }

    /// <summary>
    /// Prepares the engine with a given initial operation.
    /// </summary>
    void Prepare(IOperation initialOperation);
    /// <summary>
    /// Executes the whole run.
    /// </summary>
    void Start();
    /// <summary>
    /// Executes one step (one operation).
    /// </summary>
    void Step();
    /// <summary>
    /// Aborts the engine run.
    /// </summary>
    void Stop();

    /// <summary>
    /// Occurs when the execution time was changed.
    /// </summary>
    event EventHandler ExecutionTimeChanged;
    /// <summary>
    /// Occurs when the running flag was changed.
    /// </summary>
    event EventHandler RunningChanged;
    /// <summary>
    /// Occurs when the engine is prepared for a new run.
    /// </summary>
    event EventHandler Prepared;
    /// <summary>
    /// Occurs when the engine is executed.
    /// </summary>
    event EventHandler Started;
    /// <summary>
    /// Occurs when the engine is finished.
    /// </summary>
    event EventHandler Stopped;
    /// <summary>
    /// Occurs when an exception was thrown.
    /// </summary>
    event EventHandler<EventArgs<Exception>> ExceptionOccurred;
  }
}
