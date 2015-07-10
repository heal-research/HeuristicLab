#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item("Gradient boosted tree model", "")]
  // this is essentially a collection of weighted regression models
  public sealed class GradientBoostedTreesModel : NamedItem, IRegressionModel {
    [Storable]
    private readonly IList<IRegressionModel> models;
    public IEnumerable<IRegressionModel> Models { get { return models; } }

    [Storable]
    private readonly IList<double> weights;
    public IEnumerable<double> Weights { get { return weights; } }

    [StorableConstructor]
    private GradientBoostedTreesModel(bool deserializing) : base(deserializing) { }
    private GradientBoostedTreesModel(GradientBoostedTreesModel original, Cloner cloner)
      : base(original, cloner) {
      this.weights = new List<double>(original.weights);
      this.models = new List<IRegressionModel>(original.models.Select(m => cloner.Clone(m)));
    }
    public GradientBoostedTreesModel(IEnumerable<IRegressionModel> models, IEnumerable<double> weights)
      : base("Gradient boosted tree model", string.Empty) {
      this.models = new List<IRegressionModel>(models);
      this.weights = new List<double>(weights);

      if (this.models.Count != this.weights.Count) throw new ArgumentException();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GradientBoostedTreesModel(this, cloner);
    }

    public IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows) {
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

    public IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new RegressionSolution(this, (IRegressionProblemData)problemData.Clone());
    }
  }
}
