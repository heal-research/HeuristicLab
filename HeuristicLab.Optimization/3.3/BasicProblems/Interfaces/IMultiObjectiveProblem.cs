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
using HEAL.Attic;

namespace HeuristicLab.Optimization {
  [StorableType("251d79f1-a065-47f9-85a3-2e8dbdbf685e")]
  public interface IMultiObjectiveProblem : IProblem, IMultiObjectiveHeuristicOptimizationProblem {
    event EventHandler MaximizationChanged;
  }

  [StorableType("806fb361-1469-4903-9f54-f8678b0717b9")]
  public interface IMultiObjectiveProblem<TEncoding, TEncodedSolution> : IMultiObjectiveProblem, IProblem<TEncoding, TEncodedSolution>
    where TEncoding : class, IEncoding
    where TEncodedSolution : class, IEncodedSolution { }

  //TODO derive IMutliObjectiveProblem from IMultiObjectiveProblemDefinition?
}