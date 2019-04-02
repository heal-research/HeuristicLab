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
using HeuristicLab.Core;
using HeuristicLab.Optimization;

namespace HeuristicLab.ExactOptimization.LinearProgramming {

  public interface ILinearSolver : IParameterizedNamedItem {
    double DualTolerance { get; set; }

    bool Presolve { get; set; }

    double PrimalTolerance { get; set; }

    ProblemType ProblemType { get; }

    double RelativeGapTolerance { get; set; }

    bool Scaling { get; set; }

    bool SupportsPause { get; }

    bool SupportsStop { get; }

    TimeSpan TimeLimit { get; set; }

    bool InterruptSolve();

    void Reset();

    void Solve(ILinearProblemDefinition problemDefintion, ResultCollection results);

    void Solve(ILinearProblemDefinition problemDefintion,
      ResultCollection results, CancellationToken cancellationToken);
  }
}
