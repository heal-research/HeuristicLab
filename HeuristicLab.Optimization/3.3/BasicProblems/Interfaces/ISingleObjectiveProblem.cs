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
  [StorableType("24830fd5-7d97-41a5-9d7e-d84f1b7ab259")]
  public interface ISingleObjectiveProblem : ISingleObjectiveHeuristicOptimizationProblem {
    event EventHandler MaximizationChanged;
  }

  [StorableType("9cc9422f-0bb5-41e8-9d9e-6e0b66a66449")]
  public interface ISingleObjectiveProblem<TEncoding, TEncodedSolution> : ISingleObjectiveProblem, IProblem<TEncoding, TEncodedSolution>
    where TEncoding : class, IEncoding<TEncodedSolution>
    where TEncodedSolution : class, IEncodedSolution { }
}