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
    IPredictor Predictor { get; set; }
    Dataset Dataset { get; set; }
    string TargetVariable { get; set; }
    IEnumerable<string> InputVariables { get; }
    IEnumerable<KeyValuePair<string, double>> Results { get; }
    IEnumerable<KeyValuePair<string, double>> MetaData { get; }
    int TrainingSamplesStart { get; set; }
    int TrainingSamplesEnd { get; set; }
    int ValidationSamplesStart { get; set; }
    int ValidationSamplesEnd { get; set; }
    int TestSamplesStart { get; set; }
    int TestSamplesEnd { get; set; }
    void SetResult(string name, double value);
    double GetResult(string name);
    void SetMetaData(string name, double data);
    double GetMetaData(string name);
    double GetVariableEvaluationImpact(string variableName);
    double GetVariableQualityImpact(string variableName);
    void AddInputVariable(string variableName);
    void SetVariableEvaluationImpact(string variableName, double impact);
    void SetVariableQualityImpact(string variableName, double impact);
  }
}
