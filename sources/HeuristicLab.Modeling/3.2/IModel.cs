#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Modeling {
  public interface IModel {
    Dataset Dataset { get; }
    string TargetVariable { get; }
    IEnumerable<string> InputVariables { get; }
    double TrainingMeanSquaredError { get; }
    double ValidationMeanSquaredError { get; }
    double TestMeanSquaredError { get; }
    double TrainingMeanAbsolutePercentageError { get; }
    double ValidationMeanAbsolutePercentageError { get; }
    double TestMeanAbsolutePercentageError { get; }
    double TrainingMeanAbsolutePercentageOfRangeError { get; }
    double ValidationMeanAbsolutePercentageOfRangeError { get; }
    double TestMeanAbsolutePercentageOfRangeError { get; }
    double TrainingCoefficientOfDetermination { get; }
    double ValidationCoefficientOfDetermination { get; }
    double TestCoefficientOfDetermination { get; }
    double TrainingVarianceAccountedFor { get; }
    double ValidationVarianceAccountedFor { get; }
    double TestVarianceAccountedFor { get; }
    double GetVariableEvaluationImpact(string variableName);
    double GetVariableQualityImpact(string variableName);

    IItem Data { get; }
  }
}
