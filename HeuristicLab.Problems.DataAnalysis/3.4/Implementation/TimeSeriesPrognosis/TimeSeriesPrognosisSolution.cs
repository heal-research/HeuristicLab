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
using System.Linq;
using HeuristicLab.Common;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis {
  /// <summary>
  /// Represents a time series prognosis data analysis solution
  /// </summary>
  [StorableType("42CF9704-DCB2-4159-89B3-A30645399B86")]
  public class TimeSeriesPrognosisSolution : TimeSeriesPrognosisSolutionBase {
    [StorableConstructor]
    protected TimeSeriesPrognosisSolution(StorableConstructorFlag _) : base(_) { }
    protected TimeSeriesPrognosisSolution(TimeSeriesPrognosisSolution original, Cloner cloner) : base(original, cloner) { }
    protected internal TimeSeriesPrognosisSolution(ITimeSeriesPrognosisModel model, ITimeSeriesPrognosisProblemData problemData)
      : base(model, problemData) {
      CalculateRegressionResults();
      CalculateTimeSeriesResults();
      CalculateTimeSeriesResults(ProblemData.TrainingHorizon, ProblemData.TestHorizon);
    }

    public override IEnumerable<IEnumerable<double>> GetPrognosedValues(IEnumerable<int> rows, IEnumerable<int> horizons) {
      return Model.GetPrognosedValues(ProblemData.Dataset, rows, horizons);
    }

    public override IEnumerable<double> PrognosedTestValues {
      get {
        return Model.GetPrognosedValues(ProblemData.Dataset, ProblemData.TestIndices.Take(1),
          new int[] { ProblemData.TestIndices.Count() }).First();
      }
    }
  }
}
