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
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using System.Collections;
using System.Xml;
using System.Runtime.Serialization;
using System.IO;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;
using System.Drawing;
using HeuristicLab.Modeling.Database;

namespace HeuristicLab.CEDMA.Core {
  public class Results : ItemBase {
    private string[] categoricalVariables = null;
    public string[] CategoricalVariables {
      get {
        if (categoricalVariables == null) {
          LoadModelAttributes();
        }
        return categoricalVariables;
      }
    }

    private string[] ordinalVariables = null;
    public string[] OrdinalVariables {
      get {
        if (ordinalVariables == null) {
          LoadModelAttributes();
        }
        return ordinalVariables;
      }
    }

    private string[] multiDimensionalOrdinalVariables;
    public string[] MultiDimensionalOrdinalVariables {
      get { return multiDimensionalOrdinalVariables; }
    }

    private string[] multiDimensionalCategoricalVariables = new string[] { "VariableImpacts: InputVariableName" };
    public string[] MultiDimensionalCategoricalVariables {
      get { return multiDimensionalCategoricalVariables; }
    }

    private IModelingDatabase database;

    private Dictionary<string, Dictionary<object, double>> categoricalValueIndices = new Dictionary<string, Dictionary<object, double>>();

    public Results(IModelingDatabase database) {
      this.database = database;
      multiDimensionalOrdinalVariables = database.GetAllResultsForInputVariables().Select(x => "VariableImpacts: " + x.Name).ToArray();
    }

    private List<ResultsEntry> entries = null;
    private bool cached = false;
    public IEnumerable<ResultsEntry> GetEntries() {
      if (!cached)
        return SelectRows();
      return entries.AsEnumerable();
    }

    private IEnumerable<ResultsEntry> SelectRows() {
      database.Connect();      
      entries = new List<ResultsEntry>();
      foreach (var model in database.GetAllModels()) {
        ResultsEntry modelEntry = new ResultsEntry();
        foreach (var modelResult in database.GetModelResults(model)) {
          modelEntry.Set(modelResult.Result.Name, modelResult.Value);
        }
        modelEntry.Set("PersistedData", database.GetModelData(model));
        modelEntry.Set("TargetVariable", model.TargetVariable.Name);
        modelEntry.Set("Algorithm", model.Algorithm.Name);
        Dictionary<HeuristicLab.Modeling.Database.IVariable, ResultsEntry> inputVariableResultsEntries =
          new Dictionary<HeuristicLab.Modeling.Database.IVariable, ResultsEntry>();

        foreach (IInputVariableResult inputVariableResult in database.GetInputVariableResults(model)) {
          if (!inputVariableResultsEntries.ContainsKey(inputVariableResult.Variable)) {
            inputVariableResultsEntries[inputVariableResult.Variable] = new ResultsEntry();
            inputVariableResultsEntries[inputVariableResult.Variable].Set("InputVariableName", inputVariableResult.Variable.Name);
          }
          inputVariableResultsEntries[inputVariableResult.Variable].Set(inputVariableResult.Result.Name, inputVariableResult.Value);
        }
        modelEntry.Set("VariableImpacts", inputVariableResultsEntries.Values);
        entries.Add(modelEntry);
      }
      database.Disconnect();
      FireChanged();
      cached = true;
      return entries;
    }

    private bool IsAlmost(double x, double y) {
      return Math.Abs(x - y) < 1.0E-12;
    }

    internal IEnumerable<string> SelectModelAttributes() {
      return CategoricalVariables.Concat(OrdinalVariables);
    }

    private void LoadModelAttributes() {
      ordinalVariables = database.GetAllResults().Select(r => r.Name).ToArray();
      categoricalVariables = new string[] { "TargetVariable", "Algorithm" };
    }

    public double IndexOfCategoricalValue(string variable, object value) {
      if (value == null) return double.NaN;
      Dictionary<object, double> valueToIndexMap;
      if (categoricalValueIndices.ContainsKey(variable)) {
        valueToIndexMap = categoricalValueIndices[variable];
      } else {
        valueToIndexMap = new Dictionary<object, double>();
        categoricalValueIndices[variable] = valueToIndexMap;
      }
      if (!valueToIndexMap.ContainsKey(value)) {
        if (valueToIndexMap.Values.Count == 0) valueToIndexMap[value] = 1.0;
        else valueToIndexMap[value] = 1.0 + valueToIndexMap.Values.Max();
      }
      return valueToIndexMap[value];
    }
  }
}
