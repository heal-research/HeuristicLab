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
using HeuristicLab.CEDMA.DB.Interfaces;
using System.Xml;
using System.Runtime.Serialization;
using System.IO;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;
using System.Drawing;

namespace HeuristicLab.CEDMA.Core {
  public class Results : ItemBase {
    private const int PAGE_SIZE = 1000;
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

    private string[] multiDimensionalOrdinalVariables = new string[] { "VariableImpacts: EvaluationImpact", "VariableImpacts: QualityImpact" };
    public string[] MultiDimensionalOrdinalVariables {
      get { return multiDimensionalOrdinalVariables; }
    }

    private string[] multiDimensionalCategoricalVariables = new string[] { "VariableImpacts: InputVariableName" };
    public string[] MultiDimensionalCategoricalVariables {
      get { return multiDimensionalCategoricalVariables; }
    }

    private IStore store;
    public IStore Store {
      get { return store; }
      set {
        store = value;
      }
    }

    private Dictionary<string, Dictionary<object, double>> categoricalValueIndices = new Dictionary<string, Dictionary<object, double>>();

    public Results(IStore store) {
      this.store = store;
    }

    private List<ResultsEntry> entries = null;
    private bool cached = false;
    public IEnumerable<ResultsEntry> GetEntries() {
      if (!cached)
        return SelectRows();
      return entries.AsEnumerable();
    }

    long nStatements;
    DateTime start, stop;
    private IEnumerable<ResultsEntry> SelectRows() {
      start = DateTime.Now;
      nStatements = 0;
      int page = 0;
      int resultsReturned = 0;
      entries = new List<ResultsEntry>();
      if (store == null) return entries;
      do {
        var allModels = store.Query(
          "?Model <" + Ontology.InstanceOf + "> <" + Ontology.TypeModel + "> ." +
          "?Model <" + Ontology.TargetVariable + "> ?TargetVariable ." +
          "?Model <" + Ontology.TestMeanSquaredError + "> ?TestMSE .",
          0, 3000)
          .Select(modelBinding => new {
            Model = ((Entity)modelBinding.Get("Model")).Uri,
            TestMSE = (double)((Literal)modelBinding.Get("TestMSE")).Value,
            TargetVariable = (string)((Literal)modelBinding.Get("TargetVariable")).Value
          })
          .Distinct()
          .GroupBy(m => m.TargetVariable)
          .Select(grouping => grouping.OrderBy(m => m.TestMSE));
        foreach (var targetVariableBindings in allModels) {
          resultsReturned = targetVariableBindings.Count();
          nStatements += resultsReturned;
          int nModels = Math.Max(10, resultsReturned * 10 / 100);
          nModels = Math.Min(nModels, resultsReturned);
          foreach (var modelBindings in targetVariableBindings.Take(nModels)) {
            ResultsEntry entry = new ResultsEntry();
            entry.Uri = modelBindings.Model;
            entries.Add(entry);
            SetModelAttributes(entry, modelBindings.Model);
            entry.Set("VariableImpacts", SelectVariableImpacts(modelBindings.Model));
          }
        }
        page++;
      } while (resultsReturned == PAGE_SIZE);
      stop = DateTime.Now;
      FireChanged();
      cached = true;
      return entries;
    }

    private void SetModelAttributes(ResultsEntry entry, string modelUri) {
      var modelBindings = store.Query(
        "<" + modelUri + "> ?Attribute ?Value .",
        0, PAGE_SIZE);
      nStatements += modelBindings.Count();
      foreach (var binding in modelBindings) {
        if (binding.Get("Value") is Literal) {
          string name = ((Entity)binding.Get("Attribute")).Uri.Replace(Ontology.CedmaNameSpace, "");
          if (entry.Get(name) == null) {
            object value = ((Literal)binding.Get("Value")).Value;
            entry.Set(name, value);
          }
        }
      }
    }

    private IEnumerable<ResultsEntry> SelectVariableImpacts(string modelUri) {
      var inputVariableNameBindings = store.Query(
          "<" + modelUri + "> <" + Ontology.HasInputVariable + "> ?InputVariable ." +
          "?InputVariable <" + Ontology.Name + "> ?InputName .",
          0, PAGE_SIZE);

      var qualityImpactBindings = store.Query(
          "<" + modelUri + "> <" + Ontology.HasInputVariable + "> ?InputVariable ." +
          "?InputVariable <" + Ontology.QualityImpact + "> ?QualityImpact .",
          0, PAGE_SIZE);

      var evaluationImpactBindings = store.Query(
           "<" + modelUri + "> <" + Ontology.HasInputVariable + "> ?InputVariable ." +
           "?InputVariable <" + Ontology.EvaluationImpact + "> ?EvaluationImpact .",
           0, PAGE_SIZE);
      Dictionary<object, ResultsEntry> inputVariableAttributes = new Dictionary<object, ResultsEntry>();
      nStatements += inputVariableNameBindings.Count();
      nStatements += qualityImpactBindings.Count();
      nStatements += evaluationImpactBindings.Count();

      foreach (var inputVariableNameBinding in inputVariableNameBindings) {
        object inputVariable = inputVariableNameBinding.Get("InputVariable");
        object name = ((Literal)inputVariableNameBinding.Get("InputName")).Value;
        if (!inputVariableAttributes.ContainsKey(inputVariable)) {
          inputVariableAttributes[inputVariable] = new ResultsEntry();
          inputVariableAttributes[inputVariable].Set("InputVariableName", name);
        }
      }

      foreach (var qualityImpactBinding in qualityImpactBindings) {
        double qualityImpact = (double)((Literal)qualityImpactBinding.Get("QualityImpact")).Value;
        object inputVariable = qualityImpactBinding.Get("InputVariable");
        if (!IsAlmost(qualityImpact, 1.0)) {
          if (inputVariableAttributes[inputVariable].Get("QualityImpact") == null)
            inputVariableAttributes[inputVariable].Set("QualityImpact", qualityImpact);
        } else inputVariableAttributes.Remove(inputVariable);
      }

      foreach (var evaluationImpactBinding in evaluationImpactBindings) {
        double evaluationImpact = (double)((Literal)evaluationImpactBinding.Get("EvaluationImpact")).Value;
        object inputVariable = evaluationImpactBinding.Get("InputVariable");
        if (!IsAlmost(evaluationImpact, 0.0)) {
          if (inputVariableAttributes.ContainsKey(inputVariable) && inputVariableAttributes[inputVariable].Get("EvaluationImpact") == null)
            inputVariableAttributes[inputVariable].Set("EvaluationImpact", evaluationImpact);
        } else inputVariableAttributes.Remove(inputVariable);
      }

      return inputVariableAttributes.Values;
    }

    private bool IsAlmost(double x, double y) {
      return Math.Abs(x - y) < 1.0E-12;
    }

    internal IEnumerable<string> SelectModelAttributes() {
      return CategoricalVariables.Concat(OrdinalVariables);
    }

    private void LoadModelAttributes() {
      this.ordinalVariables =
        store
          .Query(
            "?ModelAttribute <" + Ontology.InstanceOf + "> <" + Ontology.TypeModelAttribute + "> ." + Environment.NewLine +
            "?ModelAttribute <" + Ontology.InstanceOf + "> <" + Ontology.TypeOrdinalAttribute + "> .", 0, 100)
          .Select(s => ((Entity)s.Get("ModelAttribute")).Uri.Replace(Ontology.CedmaNameSpace, ""))
          .Distinct()
          .ToArray();
      this.categoricalVariables =
        store
          .Query(
            "?ModelAttribute <" + Ontology.InstanceOf + "> <" + Ontology.TypeModelAttribute + "> ." + Environment.NewLine +
            "?ModelAttribute <" + Ontology.InstanceOf + "> <" + Ontology.TypeCategoricalAttribute + "> .", 0, 100)
          .Select(s => ((Entity)s.Get("ModelAttribute")).Uri.Replace(Ontology.CedmaNameSpace, ""))
          .Distinct()
          .ToArray();
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
