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
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis {
  /// <summary>
  /// Represents a classification solution that uses a discriminant function and classification thresholds.
  /// </summary>
  [StorableType("A3480DF9-49E7-4329-AD23-57B4441033C1")]
  [Item("DiscriminantFunctionClassificationSolution", "Represents a classification solution that uses a discriminant function and classification thresholds.")]
  public class DiscriminantFunctionClassificationSolution : DiscriminantFunctionClassificationSolutionBase {
    protected readonly Dictionary<int, double> valueEvaluationCache;
    protected readonly Dictionary<int, double> classValueEvaluationCache;

    [StorableConstructor]
    protected DiscriminantFunctionClassificationSolution(StorableConstructorFlag _) : base(_) {
      valueEvaluationCache = new Dictionary<int, double>();
      classValueEvaluationCache = new Dictionary<int, double>();
    }
    protected DiscriminantFunctionClassificationSolution(DiscriminantFunctionClassificationSolution original, Cloner cloner)
      : base(original, cloner) {
      valueEvaluationCache = new Dictionary<int, double>(original.valueEvaluationCache);
      classValueEvaluationCache = new Dictionary<int, double>(original.classValueEvaluationCache);
    }
    public DiscriminantFunctionClassificationSolution(IDiscriminantFunctionClassificationModel model, IClassificationProblemData problemData)
      : base(model, problemData) {
      valueEvaluationCache = new Dictionary<int, double>();
      classValueEvaluationCache = new Dictionary<int, double>();
      CalculateRegressionResults();
      CalculateClassificationResults();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new DiscriminantFunctionClassificationSolution(this, cloner);
    }

    public override IEnumerable<double> EstimatedClassValues {
      get { return GetEstimatedClassValues(Enumerable.Range(0, ProblemData.Dataset.Rows)); }
    }
    public override IEnumerable<double> EstimatedTrainingClassValues {
      get { return GetEstimatedClassValues(ProblemData.TrainingIndices); }
    }
    public override IEnumerable<double> EstimatedTestClassValues {
      get { return GetEstimatedClassValues(ProblemData.TestIndices); }
    }

    public override IEnumerable<double> GetEstimatedClassValues(IEnumerable<int> rows) {
      var rowsToEvaluate = rows.Except(classValueEvaluationCache.Keys);
      var rowsEnumerator = rowsToEvaluate.GetEnumerator();
      var valuesEnumerator = Model.GetEstimatedClassValues(ProblemData.Dataset, rowsToEvaluate).GetEnumerator();

      while (rowsEnumerator.MoveNext() & valuesEnumerator.MoveNext()) {
        classValueEvaluationCache.Add(rowsEnumerator.Current, valuesEnumerator.Current);
      }

      return rows.Select(row => classValueEvaluationCache[row]);
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
      var rowsToEvaluate = rows.Except(valueEvaluationCache.Keys);
      var rowsEnumerator = rowsToEvaluate.GetEnumerator();
      var valuesEnumerator = Model.GetEstimatedValues(ProblemData.Dataset, rowsToEvaluate).GetEnumerator();

      while (rowsEnumerator.MoveNext() & valuesEnumerator.MoveNext()) {
        valueEvaluationCache.Add(rowsEnumerator.Current, valuesEnumerator.Current);
      }

      return rows.Select(row => valueEvaluationCache[row]);
    }

    protected override void OnModelChanged() {
      valueEvaluationCache.Clear();
      classValueEvaluationCache.Clear();
      base.OnModelChanged();
    }
    protected override void OnModelThresholdsChanged(System.EventArgs e) {
      classValueEvaluationCache.Clear();
      base.OnModelThresholdsChanged(e);
    }
    protected override void OnProblemDataChanged() {
      valueEvaluationCache.Clear();
      classValueEvaluationCache.Clear();
      base.OnProblemDataChanged();
    }
  }
}
