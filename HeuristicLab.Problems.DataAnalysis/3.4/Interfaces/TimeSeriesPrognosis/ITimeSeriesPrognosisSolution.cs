#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
namespace HeuristicLab.Problems.DataAnalysis {
  public interface ITimeSeriesPrognosisSolution : IRegressionSolution {
    new ITimeSeriesPrognosisModel Model { get; }
    new ITimeSeriesPrognosisProblemData ProblemData { get; set; }

    IEnumerable<IEnumerable<double>> GetPrognosedValues(IEnumerable<int> rows, IEnumerable<int> horizon);

    double TrainingTheilsUStatisticAR1 { get; }
    double TestTheilsUStatisticAR1 { get; }
    double TrainingTheilsUStatisticMean { get; }
    double TestTheilsUStatisticMean { get; }
    double TrainingDirectionalSymmetry { get; }
    double TestDirectionalSymmetry { get; }
    double TrainingWeightedDirectionalSymmetry { get; }
    double TestWeightedDirectionalSymmetry { get; }
  }
}
