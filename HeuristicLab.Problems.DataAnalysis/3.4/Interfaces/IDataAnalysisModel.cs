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
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableType("f85ccf7a-7df5-431e-bc4d-be6f3c4c2338")]
  /// <summary>
  /// Interface for all data-analysis models (regression/classification/clustering).
  /// <remarks>All methods and properties in in this interface must be implemented thread safely</remarks>
  /// </summary>
  public interface IDataAnalysisModel : INamedItem {
    IEnumerable<string> VariablesUsedForPrediction { get; }
    bool IsDatasetCompatible(IDataset dataset, out string errorMessage);
    bool IsProblemDataCompatible(IDataAnalysisProblemData problemData, out string errorMessage);
  }
}
