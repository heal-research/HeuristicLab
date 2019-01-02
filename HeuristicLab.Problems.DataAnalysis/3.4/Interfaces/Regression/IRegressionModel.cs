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

using System;
using System.Collections.Generic;
using HEAL.Fossil;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableType("56c31bd6-dc0d-4478-b121-ea1817eae301")]
  /// <summary>
  /// Interface for all regression models.
  /// <remarks>All methods and properties in in this interface must be implemented thread safely</remarks>
  /// </summary>
  public interface IRegressionModel : IDataAnalysisModel {
    IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows);
    IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData);
    bool IsProblemDataCompatible(IRegressionProblemData problemData, out string errorMessage);
    string TargetVariable { get; set; }
    event EventHandler TargetVariableChanged;
  }
}
