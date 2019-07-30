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
using System.Threading;
using HEAL.Attic;

namespace HeuristicLab.Core {
  [StorableType("c9c72e72-ad11-4683-a66f-7da5436990c7")]
  /// <summary>
  /// Interface to represent an operator.
  /// </summary>
  public interface IOperator : IParameterizedNamedItem {
    bool Breakpoint { get; set; }

    IOperation Execute(IExecutionContext context, CancellationToken cancellationToken);

    event EventHandler BreakpointChanged;
    event EventHandler Executed;
  }
}
