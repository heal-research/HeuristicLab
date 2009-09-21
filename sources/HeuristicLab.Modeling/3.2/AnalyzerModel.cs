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
using HeuristicLab.Data;

namespace HeuristicLab.Modeling {
  public class AnalyzerModel : IAnalyzerModel {
    public AnalyzerModel() { } // for persistence

    #region IAnalyzerModel Members

    private Dataset dataset;
    public Dataset Dataset {
      get { return dataset; }
      set { dataset = value; }
    }

    private string targetVariable;
    public string TargetVariable {
      get { return targetVariable; }
      set { targetVariable = value; }
    }

    public ModelType Type { get; set; }

    private List<string> inputVariables = new List<string>();
    public IEnumerable<string> InputVariables {
      get { return inputVariables; }
    }

    public int TrainingSamplesStart { get; set; }
    public int TrainingSamplesEnd { get; set; }
    public int ValidationSamplesStart { get; set; }
    public int ValidationSamplesEnd { get; set; }
    public int TestSamplesStart { get; set; }
    public int TestSamplesEnd { get; set; }

    public void AddInputVariable(string variableName) {
      if (!inputVariables.Contains(variableName))
        inputVariables.Add(variableName);
    }

    private Dictionary<ModelingResult, double> results = new Dictionary<ModelingResult, double>();
    public IEnumerable<KeyValuePair<ModelingResult, double>> Results {
      get { return results; }
    }

    public void ExtractResult(IScope scope, ModelingResult result) {
      SetResult(result, scope.GetVariableValue<DoubleData>(result.ToString(), false).Data);
    }


    public void SetResult(ModelingResult result, double value) {
      results.Add(result, value);
    }

    public double GetResult(ModelingResult result) {
      return results[result];
    }

    private Dictionary<string, double> metadata = new Dictionary<string, double>();
    public IEnumerable<KeyValuePair<string, double>> MetaData {
      get { return metadata; }
    }

    public void SetMetaData(string name, double value) {
      metadata.Add(name, value);
    }

    public double GetMetaData(string name) {
      return metadata[name];
    }

    private Dictionary<string, Dictionary<ModelingResult, double>> variableResults = new Dictionary<string, Dictionary<ModelingResult, double>>();
    public double GetVariableResult(ModelingResult result, string variableName) {
      if (variableResults.ContainsKey(variableName)) {
        if (variableResults[variableName].ContainsKey(result)) {
          return variableResults[variableName][result];
        } else throw new ArgumentException("No value for modeling result: " + result + ".");
      } else throw new ArgumentException("No variable result for variable " + variableName + ".");
    }

    public void SetVariableResult(ModelingResult result, string variableName, double value) {
      if (!variableResults.ContainsKey(variableName)) {
        variableResults.Add(variableName, new Dictionary<ModelingResult, double>());
      }
      variableResults[variableName][result] = value;
    }

    public IEnumerable<KeyValuePair<ModelingResult, double>> GetVariableResults(string variableName) {
      return variableResults[variableName];
    }

    public IPredictor Predictor { get; set; }

    #endregion

  }
}
