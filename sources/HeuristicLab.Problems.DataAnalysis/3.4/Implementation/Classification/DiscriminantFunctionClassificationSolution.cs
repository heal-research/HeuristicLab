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
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  /// <summary>
  /// Represents a classification solution that uses a discriminant function and classification thresholds.
  /// </summary>
  [StorableClass]
  [Item("DiscriminantFunctionClassificationSolution", "Represents a classification solution that uses a discriminant function and classification thresholds.")]
  public class DiscriminantFunctionClassificationSolution : ClassificationSolution, IDiscriminantFunctionClassificationSolution {
    private const string TrainingMeanSquaredErrorResultName = "Mean squared error (training)";
    private const string TestMeanSquaredErrorResultName = "Mean squared error (test)";
    private const string TrainingRSquaredResultName = "Pearson's R² (training)";
    private const string TestRSquaredResultName = "Pearson's R² (test)";

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

    public double TrainingMeanSquaredError {
      get { return ((DoubleValue)this[TrainingMeanSquaredErrorResultName].Value).Value; }
      private set { ((DoubleValue)this[TrainingMeanSquaredErrorResultName].Value).Value = value; }
    }

    public double TestMeanSquaredError {
      get { return ((DoubleValue)this[TestMeanSquaredErrorResultName].Value).Value; }
      private set { ((DoubleValue)this[TestMeanSquaredErrorResultName].Value).Value = value; }
    }

    public double TrainingRSquared {
      get { return ((DoubleValue)this[TrainingRSquaredResultName].Value).Value; }
      private set { ((DoubleValue)this[TrainingRSquaredResultName].Value).Value = value; }
    }

    public double TestRSquared {
      get { return ((DoubleValue)this[TestRSquaredResultName].Value).Value; }
      private set { ((DoubleValue)this[TestRSquaredResultName].Value).Value = value; }
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
      Add(new Result(TrainingMeanSquaredErrorResultName, "Mean of squared errors of the model on the training partition", new DoubleValue()));
      Add(new Result(TestMeanSquaredErrorResultName, "Mean of squared errors of the model on the test partition", new DoubleValue()));
      Add(new Result(TrainingRSquaredResultName, "Squared Pearson's correlation coefficient of the model output and the actual values on the training partition", new DoubleValue()));
      Add(new Result(TestRSquaredResultName, "Squared Pearson's correlation coefficient of the model output and the actual values on the test partition", new DoubleValue()));
      SetAccuracyMaximizingThresholds();

      //mkommend: important to recalculate accuracy because during the calculation before no thresholds were present     
      base.RecalculateResults();
      CalculateResults();
      RegisterEventHandler();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandler();
    }

    protected override void OnModelChanged(EventArgs e) {
      DeregisterEventHandler();
      SetAccuracyMaximizingThresholds();
      RegisterEventHandler();
      base.OnModelChanged(e);
    }

    protected override void RecalculateResults() {
      base.RecalculateResults();
      CalculateResults();
    }

    private void CalculateResults() {
      double[] estimatedTrainingValues = EstimatedTrainingValues.ToArray(); // cache values
      IEnumerable<double> originalTrainingValues = ProblemData.Dataset.GetEnumeratedVariableValues(ProblemData.TargetVariable, ProblemData.TrainingIndizes);
      double[] estimatedTestValues = EstimatedTestValues.ToArray(); // cache values
      IEnumerable<double> originalTestValues = ProblemData.Dataset.GetEnumeratedVariableValues(ProblemData.TargetVariable, ProblemData.TestIndizes);

      OnlineCalculatorError errorState;
      double trainingMSE = OnlineMeanSquaredErrorCalculator.Calculate(estimatedTrainingValues, originalTrainingValues, out errorState);
      TrainingMeanSquaredError = errorState == OnlineCalculatorError.None ? trainingMSE : double.NaN;
      double testMSE = OnlineMeanSquaredErrorCalculator.Calculate(estimatedTestValues, originalTestValues, out errorState);
      TestMeanSquaredError = errorState == OnlineCalculatorError.None ? testMSE : double.NaN;

      double trainingR2 = OnlinePearsonsRSquaredCalculator.Calculate(estimatedTrainingValues, originalTrainingValues, out errorState);
      TrainingRSquared = errorState == OnlineCalculatorError.None ? trainingR2 : double.NaN;
      double testR2 = OnlinePearsonsRSquaredCalculator.Calculate(estimatedTestValues, originalTestValues, out errorState);
      TestRSquared = errorState == OnlineCalculatorError.None ? testR2 : double.NaN;
    }

    private void RegisterEventHandler() {
      Model.ThresholdsChanged += new EventHandler(Model_ThresholdsChanged);
    }
    private void DeregisterEventHandler() {
      Model.ThresholdsChanged -= new EventHandler(Model_ThresholdsChanged);
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
