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

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableType("710d4059-2512-415d-b4bc-8cd995d5b9fc")]
  public interface IConfidenceRegressionSolution : IRegressionSolution {
    new IConfidenceRegressionModel Model { get; }

    IEnumerable<double> EstimatedVariances { get; }
    IEnumerable<double> EstimatedTrainingVariances { get; }
    IEnumerable<double> EstimatedTestVariances { get; }
    IEnumerable<double> GetEstimatedVariances(IEnumerable<int> rows);
  }
}