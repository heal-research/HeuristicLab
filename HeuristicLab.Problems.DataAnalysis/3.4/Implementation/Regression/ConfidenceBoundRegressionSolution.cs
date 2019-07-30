#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis {
  /// <summary>
  /// Represents a regression data analysis solution that supports confidence estimates
  /// </summary>
  [StorableType("C2D0DE07-E8F0-4850-AAF3-E2885EC1DDB6")]
  public class ConfidenceRegressionSolution : RegressionSolution, IConfidenceRegressionSolution {
    protected readonly Dictionary<int, double> varianceEvaluationCache;

    public new IConfidenceRegressionModel Model {
      get { return (IConfidenceRegressionModel)base.Model; }
      set { base.Model = value; }
    }

    [StorableConstructor]
    protected ConfidenceRegressionSolution(StorableConstructorFlag _) : base(_) {
      varianceEvaluationCache = new Dictionary<int, double>();
    }
    protected ConfidenceRegressionSolution(ConfidenceRegressionSolution original, Cloner cloner)
      : base(original, cloner) {
      varianceEvaluationCache = new Dictionary<int, double>(original.varianceEvaluationCache);
    }
    public ConfidenceRegressionSolution(IConfidenceRegressionModel model, IRegressionProblemData problemData)
      : base(model, problemData) {
      varianceEvaluationCache = new Dictionary<int, double>(problemData.Dataset.Rows);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ConfidenceRegressionSolution(this, cloner);
    }

    public IEnumerable<double> EstimatedVariances {
      get { return GetEstimatedVariances(Enumerable.Range(0, ProblemData.Dataset.Rows)); }
    }
    public IEnumerable<double> EstimatedTrainingVariances {
      get { return GetEstimatedVariances(ProblemData.TrainingIndices); }
    }
    public IEnumerable<double> EstimatedTestVariances {
      get { return GetEstimatedVariances(ProblemData.TestIndices); }
    }

    public IEnumerable<double> GetEstimatedVariances(IEnumerable<int> rows) {
      var rowsToEvaluate = rows.Except(varianceEvaluationCache.Keys);
      var rowsEnumerator = rowsToEvaluate.GetEnumerator();
      var valuesEnumerator = Model.GetEstimatedVariances(ProblemData.Dataset, rowsToEvaluate).GetEnumerator();

      while (rowsEnumerator.MoveNext() & valuesEnumerator.MoveNext()) {
        varianceEvaluationCache.Add(rowsEnumerator.Current, valuesEnumerator.Current);
      }

      return rows.Select(row => varianceEvaluationCache[row]);
    }

    protected override void OnProblemDataChanged() {
      varianceEvaluationCache.Clear();
      base.OnProblemDataChanged();
    }

    protected override void OnModelChanged() {
      varianceEvaluationCache.Clear();
      base.OnModelChanged();
    }
  }
}