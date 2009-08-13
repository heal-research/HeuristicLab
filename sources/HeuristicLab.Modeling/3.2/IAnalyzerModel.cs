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
   public interface IAnalyzerModel {
    Dataset Dataset { get; set; }
    string TargetVariable { get; set; }
    IEnumerable<string> InputVariables { get; }
    int TrainingSamplesStart { get; set; }
    int TrainingSamplesEnd { get; set; }
    int ValidationSamplesStart { get; set; }
    int ValidationSamplesEnd { get; set; }
    int TestSamplesStart { get; set; }
    int TestSamplesEnd { get; set; }
    double TrainingMeanSquaredError { get; set; }
    double ValidationMeanSquaredError { get; set; }
    double TestMeanSquaredError { get; set; }
    double TrainingMeanAbsolutePercentageError { get; set; }
    double ValidationMeanAbsolutePercentageError { get; set; }
    double TestMeanAbsolutePercentageError { get; set; }
    double TrainingMeanAbsolutePercentageOfRangeError { get; set; }
    double ValidationMeanAbsolutePercentageOfRangeError { get; set; }
    double TestMeanAbsolutePercentageOfRangeError { get; set; }
    double TrainingCoefficientOfDetermination { get; set; }
    double ValidationCoefficientOfDetermination { get; set; }
    double TestCoefficientOfDetermination { get; set; }
    double TrainingVarianceAccountedFor { get; set; }
    double ValidationVarianceAccountedFor { get; set; }
    double TestVarianceAccountedFor { get; set; }
    double GetVariableEvaluationImpact(string variableName);
    double GetVariableQualityImpact(string variableName);
    void AddInputVariable(string variableName);
    void SetVariableEvaluationImpact(string variableName, double impact);
    void SetVariableQualityImpact(string variableName, double impact);
    IPredictor Predictor { get; set; }
  }
}
