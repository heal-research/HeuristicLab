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

using System.Collections.Generic;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [StorableType("09d522e0-c10f-474c-b7c0-7d7f98e63f44")]
  public interface ISingleObjectiveOptimizationSupport<TEncodedSolution>
    where TEncodedSolution : IDeepCloneable {

    void Analyze(TEncodedSolution[] individuals, double[] qualities, ResultCollection results, IRandom random);
    IEnumerable<TEncodedSolution> GetNeighbors(TEncodedSolution individual, IRandom random);
  }
}
