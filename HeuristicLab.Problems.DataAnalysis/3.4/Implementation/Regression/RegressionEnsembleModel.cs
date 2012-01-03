#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  /// <summary>
  /// Represents regression solutions that contain an ensemble of multiple regression models
  /// </summary>
  [StorableClass]
  [Item("RegressionEnsembleModel", "A regression model that contains an ensemble of multiple regression models")]
  public class RegressionEnsembleModel : NamedItem, IRegressionEnsembleModel {

    private List<IRegressionModel> models;
    public IEnumerable<IRegressionModel> Models {
      get { return new List<IRegressionModel>(models); }
    }

    [Storable(Name = "Models")]
    private IEnumerable<IRegressionModel> StorableModels {
      get { return models; }
      set { models = value.ToList(); }
    }

    #region backwards compatiblity 3.3.5
    [Storable(Name = "models", AllowOneWay = true)]
    private List<IRegressionModel> OldStorableModels {
      set { models = value; }
    }
    #endregion

    [StorableConstructor]
    protected RegressionEnsembleModel(bool deserializing) : base(deserializing) { }
    protected RegressionEnsembleModel(RegressionEnsembleModel original, Cloner cloner)
      : base(original, cloner) {
      this.models = original.Models.Select(m => cloner.Clone(m)).ToList();
    }

    public RegressionEnsembleModel() : this(Enumerable.Empty<IRegressionModel>()) { }
    public RegressionEnsembleModel(IEnumerable<IRegressionModel> models)
      : base() {
      this.name = ItemName;
      this.description = ItemDescription;
      this.models = new List<IRegressionModel>(models);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RegressionEnsembleModel(this, cloner);
    }

    #region IRegressionEnsembleModel Members

    public void Add(IRegressionModel model) {
      models.Add(model);
    }
    public void Remove(IRegressionModel model) {
      models.Remove(model);
    }

    public IEnumerable<IEnumerable<double>> GetEstimatedValueVectors(Dataset dataset, IEnumerable<int> rows) {
      var estimatedValuesEnumerators = (from model in models
                                        select model.GetEstimatedValues(dataset, rows).GetEnumerator())
                                       .ToList();

      while (estimatedValuesEnumerators.All(en => en.MoveNext())) {
        yield return from enumerator in estimatedValuesEnumerators
                     select enumerator.Current;
      }
    }

    #endregion

    #region IRegressionModel Members

    public IEnumerable<double> GetEstimatedValues(Dataset dataset, IEnumerable<int> rows) {
      foreach (var estimatedValuesVector in GetEstimatedValueVectors(dataset, rows)) {
        yield return estimatedValuesVector.Average();
      }
    }

    public RegressionEnsembleSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new RegressionEnsembleSolution(this.Models, problemData);
    }
    IRegressionSolution IRegressionModel.CreateRegressionSolution(IRegressionProblemData problemData) {
      return CreateRegressionSolution(problemData);
    }

    #endregion
  }
}
