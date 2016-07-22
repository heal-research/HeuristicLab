#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using Google.ProtocolBuffers;
using HeuristicLab.Core;

namespace HeuristicLab.Problems.ExternalEvaluation {
  public interface IEvaluationServiceClient : IItem {
    /// <summary>
    /// Evaluates a given solution in a blocking manner.
    /// </summary>
    /// <param name="solution">The solution message that should be evaluated.</param>
    /// <param name="qualityExtensions">An extension registry for the quality message that specifies additional custom fields.</param>
    /// <returns>The resulting quality message from the external process.</returns>
    QualityMessage Evaluate(SolutionMessage solution, ExtensionRegistry qualityExtensions);
    /// <summary>
    /// Evaluates a given solution in a non-blocking manner.
    /// </summary>
    /// <param name="solution">The solution message that should be evaluated.</param>
    /// <param name="qualityExtensions">An extension registry for the quality message that specifies additional custom fields.</param>
    /// <param name="callback">The callback method that is invoked when evaluation is finished.</param>
    void EvaluateAsync(SolutionMessage solution, ExtensionRegistry qualityExtensions, Action<QualityMessage> callback);
  }
}
