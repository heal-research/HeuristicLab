#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Collections;

namespace HeuristicLab.Core {
  /// <summary>
  /// Interface to represent an operator (e.g. GreaterThanComparator,...), 
  /// a basic instruction of an algorithm.
  /// </summary>
  public interface IOperator : INamedItem {
    IObservableKeyedCollection<string, IParameter> Parameters { get; }

    /// <summary>
    /// Gets or sets a boolean value whether the engine should stop here during the run.
    /// </summary>
    bool Breakpoint { get; set; }

    /// <summary>
    /// Executes the current instance on the specified <paramref name="scope"/>.
    /// </summary>
    /// <param name="scope">The scope where to execute the current instance.</param>
    /// <returns>The next operation.</returns>
    IExecutionContext Execute(ExecutionContext context);
    /// <summary>
    /// Aborts the current operator.
    /// </summary>
    void Abort();

    /// <summary>
    /// Occurs when the breakpoint flag of the current instance was changed.
    /// </summary>
    event EventHandler BreakpointChanged; 
    /// <summary>
    /// Occurs when the current instance is executed.
    /// </summary>
    event EventHandler Executed;
  }
}
