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
  [StorableType("7aef6b19-899c-4510-a3d1-9d5056ee204e")]
  public interface IClassificationSolution : IDataAnalysisSolution {
    new IClassificationModel Model { get; }
    new IClassificationProblemData ProblemData { get; set; }

    IEnumerable<double> EstimatedClassValues { get; }
    IEnumerable<double> EstimatedTrainingClassValues { get; }
    IEnumerable<double> EstimatedTestClassValues { get; }
    IEnumerable<double> GetEstimatedClassValues(IEnumerable<int> rows);

    double TrainingAccuracy { get; }
    double TestAccuracy { get; }
  }
}
