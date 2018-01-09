#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  /// <summary>
  /// Represents a regression data analysis solution
  /// </summary>
  [StorableClass]
  public class RegressionSolution : RegressionSolutionBase {
    protected readonly Dictionary<int, double> evaluationCache;

    [StorableConstructor]
    protected RegressionSolution(bool deserializing)
      : base(deserializing) {
      evaluationCache = new Dictionary<int, double>();
    }
    protected RegressionSolution(RegressionSolution original, Cloner cloner)
      : base(original, cloner) {
      evaluationCache = new Dictionary<int, double>(original.evaluationCache);
    }
    public RegressionSolution(IRegressionModel model, IRegressionProblemData problemData)
      : base(model, problemData) {
      evaluationCache = new Dictionary<int, double>(problemData.Dataset.Rows);
      CalculateRegressionResults();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RegressionSolution(this, cloner);
    }

    public override IEnumerable<double> EstimatedValues {
      get { return GetEstimatedValues(Enumerable.Range(0, ProblemData.Dataset.Rows)); }
    }
    public override IEnumerable<double> EstimatedTrainingValues {
      get { return GetEstimatedValues(ProblemData.TrainingIndices); }
    }
    public override IEnumerable<double> EstimatedTestValues {
      get { return GetEstimatedValues(ProblemData.TestIndices); }
    }

    public override IEnumerable<double> GetEstimatedValues(IEnumerable<int> rows) {
      var rowsToEvaluate = rows.Where(row => !evaluationCache.ContainsKey(row));
      var rowsEnumerator = rowsToEvaluate.GetEnumerator();
      var valuesEnumerator = Model.GetEstimatedValues(ProblemData.Dataset, rowsToEvaluate).GetEnumerator();

      while (rowsEnumerator.MoveNext() & valuesEnumerator.MoveNext()) {
        evaluationCache.Add(rowsEnumerator.Current, valuesEnumerator.Current);
      }

      return rows.Select(row => evaluationCache[row]);
    }

    protected override void OnProblemDataChanged() {
      evaluationCache.Clear();
      base.OnProblemDataChanged();
    }

    protected override void OnModelChanged() {
      evaluationCache.Clear();
      base.OnModelChanged();
    }
  }
}
