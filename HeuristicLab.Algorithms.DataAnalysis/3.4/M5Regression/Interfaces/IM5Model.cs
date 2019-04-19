#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimization;
using HeuristicLab.Problems.DataAnalysis;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("A5399E6A-6A4D-4616-A1CD-CE12FE670F12")]
  public interface IM5Model : IRegressionModel {
    void Build(IReadOnlyList<int> trainingRows, IReadOnlyList<int> pruningRows, IScope stateScope, ResultCollection results, CancellationToken cancellationToken);
    void Update(IReadOnlyList<int> rows, IScope stateScope, CancellationToken cancellationToken);
  }
}