#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 * and the BEACON Center for the Study of Evolution in Action.
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
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("4EC1B359-D145-434C-A373-3EDD764D2D63")]
  [Item("Gradient boosted trees model", "")]
  // this is essentially a collection of weighted regression models
  public sealed class GradientBoostedTreesModel : RegressionModel, IGradientBoostedTreesModel {
    [Storable(Name = "models")]
    private IList<IRegressionModel> __persistedModels {
      set {
        this.models.Clear();
        foreach (var m in value) this.models.Add(m);
      }
      get { return models; }
    }
    [Storable(Name = "weights")]
    private IList<double> __persistedWeights {
      set {
        this.weights.Clear();
        foreach (var w in value) this.weights.Add(w);
      }
      get { return weights; }
    }

    public override IEnumerable<string> VariablesUsedForPrediction {
      get { return models.SelectMany(x => x.VariablesUsedForPrediction).Distinct().OrderBy(x => x); }
    }

    private readonly IList<IRegressionModel> models;
    public IEnumerable<IRegressionModel> Models { get { return models; } }

    private readonly IList<double> weights;
    public IEnumerable<double> Weights { get { return weights; } }

    [StorableConstructor]
    private GradientBoostedTreesModel(StorableConstructorFlag _) : base(_) {
      models = new List<IRegressionModel>();
      weights = new List<double>();
    }
    private GradientBoostedTreesModel(GradientBoostedTreesModel original, Cloner cloner)
      : base(original, cloner) {
      this.weights = new List<double>(original.weights);
      this.models = new List<IRegressionModel>(original.models.Select(m => cloner.Clone(m)));
    }

    internal GradientBoostedTreesModel(IEnumerable<IRegressionModel> models, IEnumerable<double> weights)
      : base(string.Empty, "Gradient boosted tree model", string.Empty) {
      this.models = new List<IRegressionModel>(models);
      this.weights = new List<double>(weights);

      if (this.models.Count != this.weights.Count) throw new ArgumentException();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GradientBoostedTreesModel(this, cloner);
    }

    public override IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows) {
      // allocate target array go over all models and add up weighted estimation for each row
      if (!rows.Any()) return Enumerable.Empty<double>(); // return immediately if rows is empty. This prevents multiple iteration over lazy rows enumerable.
      // (which essentially looks up indexes in a dictionary)
      var res = new double[rows.Count()];
      for (int i = 0; i < models.Count; i++) {
        var w = weights[i];
        var m = models[i];
        int r = 0;
        foreach (var est in m.GetEstimatedValues(dataset, rows)) {
          res[r++] += w * est;
        }
      }
      return res;
    }

    public override IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new RegressionSolution(this, (IRegressionProblemData)problemData.Clone());
    }

  }
}
