#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.DataAnalysis {
  /// <summary>
  /// Represents a classification solution that uses a discriminant function and classification thresholds.
  /// </summary>
  [StorableClass]
  [Item("DiscriminantFunctionClassificationSolution", "Represents a classification solution that uses a discriminant function and classification thresholds.")]
  public class DiscriminantFunctionClassificationSolution : ClassificationSolution, IDiscriminantFunctionClassificationSolution {
    public new IDiscriminantFunctionClassificationModel Model {
      get { return (IDiscriminantFunctionClassificationModel)base.Model; }
      protected set {
        if (value != null && value != Model) {
          if (Model != null) {
            Model.ThresholdsChanged -= new EventHandler(Model_ThresholdsChanged);
          }
          value.ThresholdsChanged += new EventHandler(Model_ThresholdsChanged);
          base.Model = value;
        }
      }
    }

    [StorableConstructor]
    protected DiscriminantFunctionClassificationSolution(bool deserializing) : base(deserializing) { }
    protected DiscriminantFunctionClassificationSolution(DiscriminantFunctionClassificationSolution original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandler();
    }
    public DiscriminantFunctionClassificationSolution(IRegressionModel model, IClassificationProblemData problemData)
      : this(new DiscriminantFunctionClassificationModel(model), problemData) {
    }
    public DiscriminantFunctionClassificationSolution(IDiscriminantFunctionClassificationModel model, IClassificationProblemData problemData)
      : base(model, problemData) {
      RegisterEventHandler();
      SetAccuracyMaximizingThresholds();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandler();
    }

    private void RegisterEventHandler() {
      Model.ThresholdsChanged += new EventHandler(Model_ThresholdsChanged);
    }
    private void Model_ThresholdsChanged(object sender, EventArgs e) {
      OnModelThresholdsChanged(e);
    }

    public void SetAccuracyMaximizingThresholds() {
      double[] classValues;
      double[] thresholds;
      var targetClassValues = ProblemData.Dataset.GetEnumeratedVariableValues(ProblemData.TargetVariable, ProblemData.TrainingIndizes);
      AccuracyMaximizationThresholdCalculator.CalculateThresholds(ProblemData, EstimatedTrainingValues, targetClassValues, out classValues, out thresholds);

      Model.SetThresholdsAndClassValues(thresholds, classValues);
    }

    public void SetClassDistibutionCutPointThresholds() {
      double[] classValues;
      double[] thresholds;
      var targetClassValues = ProblemData.Dataset.GetEnumeratedVariableValues(ProblemData.TargetVariable, ProblemData.TrainingIndizes);
      NormalDistributionCutPointsThresholdCalculator.CalculateThresholds(ProblemData, EstimatedTrainingValues, targetClassValues, out classValues, out thresholds);

      Model.SetThresholdsAndClassValues(thresholds, classValues);
    }

    protected override void OnModelChanged(EventArgs e) {
      base.OnModelChanged(e);
      SetAccuracyMaximizingThresholds();
    }

    protected override void OnProblemDataChanged(EventArgs e) {
      base.OnProblemDataChanged(e);
      SetAccuracyMaximizingThresholds();
    }
    protected virtual void OnModelThresholdsChanged(EventArgs e) {
      RecalculateResults();
    }

    public IEnumerable<double> EstimatedValues {
      get { return GetEstimatedValues(Enumerable.Range(0, ProblemData.Dataset.Rows)); }
    }

    public IEnumerable<double> EstimatedTrainingValues {
      get { return GetEstimatedValues(ProblemData.TrainingIndizes); }
    }

    public IEnumerable<double> EstimatedTestValues {
      get { return GetEstimatedValues(ProblemData.TestIndizes); }
    }

    public IEnumerable<double> GetEstimatedValues(IEnumerable<int> rows) {
      return Model.GetEstimatedValues(ProblemData.Dataset, rows);
    }
  }
}
