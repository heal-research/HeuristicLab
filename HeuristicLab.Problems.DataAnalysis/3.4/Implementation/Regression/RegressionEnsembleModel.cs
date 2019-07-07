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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis {
  /// <summary>
  /// Represents regression solutions that contain an ensemble of multiple regression models
  /// </summary>
  [StorableType("202C33A2-2B7A-42E2-B3F3-BE1C9A9B5B84")]
  [Item("RegressionEnsembleModel", "A regression model that contains an ensemble of multiple regression models")]
  public sealed class RegressionEnsembleModel : RegressionModel, IRegressionEnsembleModel {
    public override IEnumerable<string> VariablesUsedForPrediction {
      get { return models.SelectMany(x => x.VariablesUsedForPrediction).Distinct().OrderBy(x => x); }
    }

    private List<IRegressionModel> models;
    public IEnumerable<IRegressionModel> Models {
      get { return new List<IRegressionModel>(models); }
    }

    [Storable(Name = "Models")]
    private IEnumerable<IRegressionModel> StorableModels {
      get { return models; }
      set { models = value.ToList(); }
    }

    private List<double> modelWeights;
    public IEnumerable<double> ModelWeights {
      get { return modelWeights; }
    }

    [Storable(Name = "ModelWeights")]
    private IEnumerable<double> StorableModelWeights {
      get { return modelWeights; }
      set { modelWeights = value.ToList(); }
    }

    [Storable]
    private bool averageModelEstimates = true;
    public bool AverageModelEstimates {
      get { return averageModelEstimates; }
      set {
        if (averageModelEstimates != value) {
          averageModelEstimates = value;
          OnChanged();
        }
      }
    }

    #region backwards compatiblity 3.3.5
    [Storable(OldName = "models")]
    private List<IRegressionModel> OldStorableModels {
      set { models = value; }
    }
    #endregion

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility 3.3.14
      #region Backwards compatible code, remove with 3.4
      if (modelWeights == null || !modelWeights.Any())
        modelWeights = new List<double>(models.Select(m => 1.0));
      #endregion
    }

    [StorableConstructor]
    private RegressionEnsembleModel(StorableConstructorFlag _) : base(_) { }
    private RegressionEnsembleModel(RegressionEnsembleModel original, Cloner cloner)
      : base(original, cloner) {
      this.models = original.Models.Select(cloner.Clone).ToList();
      this.modelWeights = new List<double>(original.ModelWeights);
      this.averageModelEstimates = original.averageModelEstimates;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RegressionEnsembleModel(this, cloner);
    }

    public RegressionEnsembleModel() : this(Enumerable.Empty<IRegressionModel>()) { }
    public RegressionEnsembleModel(IEnumerable<IRegressionModel> models) : this(models, models.Select(m => 1.0)) { }
    public RegressionEnsembleModel(IEnumerable<IRegressionModel> models, IEnumerable<double> modelWeights)
      : base(string.Empty) {
      this.name = ItemName;
      this.description = ItemDescription;

      this.models = new List<IRegressionModel>(models);
      this.modelWeights = new List<double>(modelWeights);

      if (this.models.Any()) this.TargetVariable = this.models.First().TargetVariable;
    }

    public void Add(IRegressionModel model) {
      if (string.IsNullOrEmpty(TargetVariable)) TargetVariable = model.TargetVariable;
      Add(model, 1.0);
    }
    public void Add(IRegressionModel model, double weight) {
      if (string.IsNullOrEmpty(TargetVariable)) TargetVariable = model.TargetVariable;

      models.Add(model);
      modelWeights.Add(weight);
      OnChanged();
    }

    public void AddRange(IEnumerable<IRegressionModel> models) {
      AddRange(models, models.Select(m => 1.0));
    }
    public void AddRange(IEnumerable<IRegressionModel> models, IEnumerable<double> weights) {
      if (string.IsNullOrEmpty(TargetVariable)) TargetVariable = models.First().TargetVariable;

      this.models.AddRange(models);
      modelWeights.AddRange(weights);
      OnChanged();
    }

    public void Remove(IRegressionModel model) {
      var index = models.IndexOf(model);
      models.RemoveAt(index);
      modelWeights.RemoveAt(index);

      if (!models.Any()) TargetVariable = string.Empty;
      OnChanged();
    }
    public void RemoveRange(IEnumerable<IRegressionModel> models) {
      foreach (var model in models) {
        var index = this.models.IndexOf(model);
        this.models.RemoveAt(index);
        modelWeights.RemoveAt(index);
      }

      if (!models.Any()) TargetVariable = string.Empty;
      OnChanged();
    }

    public double GetModelWeight(IRegressionModel model) {
      var index = models.IndexOf(model);
      return modelWeights[index];
    }
    public void SetModelWeight(IRegressionModel model, double weight) {
      var index = models.IndexOf(model);
      modelWeights[index] = weight;
      OnChanged();
    }

    #region evaluation
    public IEnumerable<IEnumerable<double>> GetEstimatedValueVectors(IDataset dataset, IEnumerable<int> rows) {
      var estimatedValuesEnumerators = (from model in models
                                        let weight = GetModelWeight(model)
                                        select model.GetEstimatedValues(dataset, rows).Select(e => weight * e)
                                        .GetEnumerator()).ToList();

      while (estimatedValuesEnumerators.All(en => en.MoveNext())) {
        yield return from enumerator in estimatedValuesEnumerators
                     select enumerator.Current;
      }
    }

    public override IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows) {
      double weightsSum = modelWeights.Sum();
      var summedEstimates = from estimatedValuesVector in GetEstimatedValueVectors(dataset, rows)
                            select estimatedValuesVector.DefaultIfEmpty(double.NaN).Sum();

      if (AverageModelEstimates)
        return summedEstimates.Select(v => v / weightsSum);
      else
        return summedEstimates;

    }

    public IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows, Func<int, IRegressionModel, bool> modelSelectionPredicate) {
      var estimatedValuesEnumerators = GetEstimatedValueVectors(dataset, rows).GetEnumerator();
      var rowsEnumerator = rows.GetEnumerator();

      while (rowsEnumerator.MoveNext() & estimatedValuesEnumerators.MoveNext()) {
        var estimatedValueEnumerator = estimatedValuesEnumerators.Current.GetEnumerator();
        int currentRow = rowsEnumerator.Current;
        double weightsSum = 0.0;
        double filteredEstimatesSum = 0.0;

        for (int m = 0; m < models.Count; m++) {
          estimatedValueEnumerator.MoveNext();
          var model = models[m];
          if (!modelSelectionPredicate(currentRow, model)) continue;

          filteredEstimatesSum += estimatedValueEnumerator.Current;
          weightsSum += modelWeights[m];
        }

        if (AverageModelEstimates)
          yield return filteredEstimatesSum / weightsSum;
        else
          yield return filteredEstimatesSum;
      }
    }

    #endregion

    public event EventHandler Changed;
    private void OnChanged() {
      var handler = Changed;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }


    public override IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new RegressionEnsembleSolution(this, new RegressionEnsembleProblemData(problemData));
    }
  }
}
