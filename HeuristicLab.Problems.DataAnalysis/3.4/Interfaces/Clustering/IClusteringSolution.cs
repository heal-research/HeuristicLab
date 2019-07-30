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
  [StorableType("dbb9226f-312d-4ee2-a6cc-b74cc6ab5cc0")]
  public interface IClusteringSolution : IDataAnalysisSolution {
    new IClusteringModel Model { get; }
    new IClusteringProblemData ProblemData { get; set; }

    IEnumerable<int> ClusterValues { get; }
    IEnumerable<int> TrainingClusterValues { get; }
    IEnumerable<int> TestClusterValues { get; }
    IEnumerable<int> GetClusterValues(IEnumerable<int> rows);
  }
}
