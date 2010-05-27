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
using HeuristicLab.Core;

namespace HeuristicLab.Problems.ExternalEvaluation {
  public interface IExternalEvaluationDriver : IItem {
    /// <summary>
    /// A flag that describes whether the driver has been initialized or not.
    /// </summary>
    bool IsInitialized { get; }
    /// <summary>
    /// Tells the driver to start.
    /// </summary>
    /// <remarks>
    /// Must be called before calling <seealso cref="Send"/> or <seealso cref="Receive"/>.
    /// The concrete implementation of the driver may require additional information on how to start a connection,
    /// which should be passed into the concrete driver's constructor.
    /// </remarks>
    void Start();
    /// <summary>
    /// Evaluates a given solution in a blocking manner.
    /// </summary>
    /// <param name="solution">The solution message that should be evaluated.</param>
    /// <returns>The resulting quality message from the external process.</returns>
    QualityMessage Evaluate(SolutionMessage solution);
    /// <summary>
    /// Evaluates a given solution in a non-blocking manner.
    /// </summary>
    /// <param name="solution">The solution message that should be evaluated.</param>
    /// <param name="callback">The callback method that is invoked when evaluation is finished.</param>
    void EvaluateAsync(SolutionMessage solution, Action<QualityMessage> callback);
    /// <summary>
    /// Tells the driver to stop and terminate open connections.
    /// </summary>
    void Stop();
  }
}
