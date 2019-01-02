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
  [StorableType("ea474735-61d0-4ada-8970-ac1d78ce4d15")]
  public interface IDiscriminantFunctionClassificationModel : IClassificationModel {
    IEnumerable<double> Thresholds { get; }
    IEnumerable<double> ClassValues { get; }
    IDiscriminantFunctionThresholdCalculator ThresholdCalculator { get; }
    void RecalculateModelParameters(IClassificationProblemData problemData, IEnumerable<int> rows);
    // class values and thresholds can only be assigned simultanously
    void SetThresholdsAndClassValues(IEnumerable<double> thresholds, IEnumerable<double> classValues);
    IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows);

    event EventHandler ThresholdsChanged;

    IDiscriminantFunctionClassificationSolution CreateDiscriminantFunctionClassificationSolution(IClassificationProblemData problemData);
  }
}
