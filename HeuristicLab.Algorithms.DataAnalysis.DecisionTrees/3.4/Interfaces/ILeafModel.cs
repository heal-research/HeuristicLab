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
using System.Threading;
using HeuristicLab.Core;
using HeuristicLab.Problems.DataAnalysis;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("2A4CB43C-51EB-47AF-AEDD-9B84B27D318B")]
  public interface ILeafModel : IParameterizedNamedItem {
    bool ProvidesConfidence { get; }
    int MinLeafSize(IRegressionProblemData pd);

    void Initialize(IScope states);
    void Build(RegressionNodeTreeModel tree, IReadOnlyList<int> trainingRows, IScope stateScope, CancellationToken cancellationToken);
    IRegressionModel BuildModel(IReadOnlyList<int> rows, RegressionTreeParameters parameters, CancellationToken cancellation, out int numParams);
  }
}