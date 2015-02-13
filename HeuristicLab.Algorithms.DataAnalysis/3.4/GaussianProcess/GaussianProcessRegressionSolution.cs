#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Represents a Gaussian process solution for a regression problem which can be visualized in the GUI.
  /// </summary>
  [Item("GaussianProcessRegressionSolution", "Represents a Gaussian process solution for a regression problem which can be visualized in the GUI.")]
  [StorableClass]
  public sealed class GaussianProcessRegressionSolution : RegressionSolution, IGaussianProcessSolution {
    private new readonly Dictionary<int, double> evaluationCache;

    public new IGaussianProcessModel Model {
      get { return (IGaussianProcessModel)base.Model; }
      set { base.Model = value; }
    }

    [StorableConstructor]
    private GaussianProcessRegressionSolution(bool deserializing)
      : base(deserializing) {
      evaluationCache = new Dictionary<int, double>();

    }
    private GaussianProcessRegressionSolution(GaussianProcessRegressionSolution original, Cloner cloner)
      : base(original, cloner) {
      evaluationCache = new Dictionary<int, double>(original.evaluationCache);
    }
    public GaussianProcessRegressionSolution(IGaussianProcessModel model, IRegressionProblemData problemData)
      : base(model, problemData) {

      evaluationCache = new Dictionary<int, double>(problemData.Dataset.Rows);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GaussianProcessRegressionSolution(this, cloner);
    }

    public IEnumerable<double> EstimatedVariance {
      get { return GetEstimatedVariance(Enumerable.Range(0, ProblemData.Dataset.Rows)); }
    }
    public IEnumerable<double> EstimatedTrainingVariance {
      get { return GetEstimatedVariance(ProblemData.TrainingIndices); }
    }
    public IEnumerable<double> EstimatedTestVariance {
      get { return GetEstimatedVariance(ProblemData.TestIndices); }
    }

    public IEnumerable<double> GetEstimatedVariance(IEnumerable<int> rows) {
      var rowsToEvaluate = rows.Except(evaluationCache.Keys);
      var rowsEnumerator = rowsToEvaluate.GetEnumerator();
      var valuesEnumerator = Model.GetEstimatedVariance(ProblemData.Dataset, rowsToEvaluate).GetEnumerator();

      while (rowsEnumerator.MoveNext() & valuesEnumerator.MoveNext()) {
        evaluationCache.Add(rowsEnumerator.Current, valuesEnumerator.Current);
      }

      return rows.Select(row => evaluationCache[row]);
    }

    protected override void OnModelChanged() {
      evaluationCache.Clear();
      base.OnModelChanged();
    }
    protected override void OnProblemDataChanged() {
      evaluationCache.Clear();
      base.OnProblemDataChanged();
    }
  }
}
