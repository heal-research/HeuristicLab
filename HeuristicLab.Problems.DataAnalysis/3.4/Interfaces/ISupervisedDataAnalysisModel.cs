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
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis {

  /// <summary>
  /// Interface for all data-analysis models (regression/classification/clustering).
  /// <remarks>All methods and properties in this interface must be implemented thread safely</remarks>
  /// </summary>

  [StorableType("68245386-0EEC-4FB4-BFC1-8C4FFE9AB0BC")]
  public interface ISupervisedDataAnalysisModel : IDataAnalysisModel {
    string TargetVariable { get; set; }
    event EventHandler TargetVariableChanged;
  }
}
