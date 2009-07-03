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

    private IEnumerable<ResultsEntry> SelectRows() {
      int page = 0;
      int resultsReturned = 0;
      bool newEntry = false;
      if (store == null) yield break;
      entries = new List<ResultsEntry>();
      do {
        var allBindings = store.Query(
          "?Model <" + Ontology.InstanceOf + "> <" + Ontology.TypeModel + "> ." +
          "?Model ?Attribute ?Value .",
          page, PAGE_SIZE);
        var allModelBindings = allBindings.GroupBy(x => (Entity)x.Get("Model"));
        resultsReturned = allBindings.Count;

        foreach (var modelBindings in allModelBindings) {
          ResultsEntry entry = entries.FirstOrDefault(x => x.Uri == modelBindings.Key.Uri);
          newEntry = false;
          if (entry == null) {
            entry = new ResultsEntry();
            entry.Uri = modelBindings.Key.Uri;
            entries.Add(entry);
            newEntry = true;
            entry.Set("VariableImpacts", SelectVariableImpacts(entry.Uri));
          }
          foreach (var binding in modelBindings) {
            if (binding.Get("Value") is Literal) {
              string name = ((Entity)binding.Get("Attribute")).Uri.Replace(Ontology.CedmaNameSpace, "");
              if (entry.Get(name) == null) {
                object value = ((Literal)binding.Get("Value")).Value;
                entry.Set(name, value);
              }
            }
          }
          if (newEntry) yield return entry;
        }
        page++;
      } while (resultsReturned == PAGE_SIZE);

      FireChanged();
      cached = true;
    }

    private IEnumerable<ResultsEntry> SelectVariableImpacts(string modelUri) {
      var allBindings = store.Query(
          "<" + modelUri + "> <" + Ontology.HasInputVariable + "> ?InputVariable ." +
          "?InputVariable <" + Ontology.Name + "> ?InputName ." +
          "?InputVariable <" + Ontology.QualityImpact + "> ?QualityImpact ." +
          "?InputVariable <" + Ontology.EvaluationImpact + "> ?EvaluationImpact .",
          0, PAGE_SIZE);
      var allInputVariableBindings = allBindings.GroupBy(x => (Entity)x.Get("InputVariable"));
      List<ResultsEntry> variableImpacts = new List<ResultsEntry>();

      foreach (var inputVariableBinding in allInputVariableBindings) {
        ResultsEntry entry = new ResultsEntry();
        VariableBindings binding = inputVariableBinding.First();
        double evaluationImpact = (double)((Literal)binding.Get("EvaluationImpact")).Value;
        double qualityImpact = (double)((Literal)binding.Get("QualityImpact")).Value;
        if (binding.Get("InputName") != null) entry.Set("InputVariableName", ((Literal)binding.Get("InputName")).Value);
        if (binding.Get("QualityImpact") != null) entry.Set("QualityImpact", qualityImpact);
        if (binding.Get("EvaluationImpact") != null) entry.Set("EvaluationImpact", evaluationImpact);
        if (!IsAlmost(evaluationImpact, 0.0) && !IsAlmost(qualityImpact, 1.0)) {
          variableImpacts.Add(entry);
        }
      }
      return variableImpacts;
    }

    private bool IsAlmost(double x, double y) {
      return (y + 1.0E-7 > x) && (y - 1.0E-7 < x);
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
