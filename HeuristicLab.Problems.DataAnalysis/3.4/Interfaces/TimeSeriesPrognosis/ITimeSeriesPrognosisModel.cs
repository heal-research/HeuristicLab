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
  [StorableType("c238682c-af0b-449e-aecf-8cf61418f425")]
  /// <summary>
  /// Interface for all time series prognosis models.
  /// <remarks>All methods and properties in in this interface must be implemented thread safely</remarks>
  /// </summary> 
  public interface ITimeSeriesPrognosisModel : IRegressionModel {
    IEnumerable<IEnumerable<double>> GetPrognosedValues(IDataset dataset, IEnumerable<int> rows, IEnumerable<int> horizons);
    ITimeSeriesPrognosisSolution CreateTimeSeriesPrognosisSolution(ITimeSeriesPrognosisProblemData problemData);
  }
}
