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
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Optimization {
  [StorableType("51d66e7a-e4bd-429a-b6e5-1cfe9ce4364f")]
  /// <summary>
  /// Interface to represent a heuristic optimization problem.
  /// </summary>
  public interface IHeuristicOptimizationProblem : IProblem {
    IParameter SolutionCreatorParameter { get; }
    ISolutionCreator SolutionCreator { get; }
    IParameter EvaluatorParameter { get; }
    IEvaluator Evaluator { get; }

    event EventHandler SolutionCreatorChanged;
    event EventHandler EvaluatorChanged;
  }
}
