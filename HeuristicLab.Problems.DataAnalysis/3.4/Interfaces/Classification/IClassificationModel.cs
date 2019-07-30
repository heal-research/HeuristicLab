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

using System;
using System.Collections.Generic;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableType("ff091491-c895-41cb-9c45-f3b1a11637d4")]
  /// <summary>
  /// Interface for all classification models.
  /// <remarks>All methods and properties in in this interface must be implemented thread safely</remarks>
  /// </summary>
  public interface IClassificationModel : IDataAnalysisModel {
    IEnumerable<double> GetEstimatedClassValues(IDataset dataset, IEnumerable<int> rows);
    IClassificationSolution CreateClassificationSolution(IClassificationProblemData problemData);
    bool IsProblemDataCompatible(IClassificationProblemData problemData, out string errorMessage);
    string TargetVariable { get; set; }
    event EventHandler TargetVariableChanged;
  }
}
