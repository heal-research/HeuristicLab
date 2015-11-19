#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public abstract class DiscriminantFunctionClassificationSolutionBase : ClassificationSolutionBase, IDiscriminantFunctionClassificationSolution {
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

    #region Results
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
    #endregion

    [StorableConstructor]
    protected DiscriminantFunctionClassificationSolutionBase(bool deserializing) : base(deserializing) { }
    protected DiscriminantFunctionClassificationSolutionBase(DiscriminantFunctionClassificationSolutionBase original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandler();
    }
    protected DiscriminantFunctionClassificationSolutionBase(IDiscriminantFunctionClassificationModel model, IClassificationProblemData problemData)
      : base(model, problemData) {
      Add(new Result(TrainingMeanSquaredErrorResultName, "Mean of squared errors of the model on the training partition", new DoubleValue()));
      Add(new Result(TestMeanSquaredErrorResultName, "Mean of squared errors of the model on the test partition", new DoubleValue()));
      Add(new Result(TrainingRSquaredResultName, "Squared Pearson's correlation coefficient of the model output and the actual values on the training partition", new DoubleValue()));
      Add(new Result(TestRSquaredResultName, "Squared Pearson's correlation coefficient of the model output and the actual values on the test partition", new DoubleValue()));
      RegisterEventHandler();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandler();
    }

    protected void CalculateRegressionResults() {
      double[] estimatedTrainingValues = EstimatedTrainingValues.ToArray(); // cache values
      double[] originalTrainingValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TrainingIndices).ToArray();
      double[] estimatedTestValues = EstimatedTestValues.ToArray(); // cache values
      double[] originalTestValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TestIndices).ToArray();

      OnlineCalculatorError errorState;
      double trainingMSE = OnlineMeanSquaredErrorCalculator.Calculate(originalTrainingValues, estimatedTrainingValues, out errorState);
      TrainingMeanSquaredError = errorState == OnlineCalculatorError.None ? trainingMSE : double.NaN;
      double testMSE = OnlineMeanSquaredErrorCalculator.Calculate(originalTestValues, estimatedTestValues, out errorState);
      TestMeanSquaredError = errorState == OnlineCalculatorError.None ? testMSE : double.NaN;

      double trainingR = OnlinePearsonsRCalculator.Calculate(originalTrainingValues, estimatedTrainingValues, out errorState);
      TrainingRSquared = errorState == OnlineCalculatorError.None ? trainingR*trainingR : double.NaN;
      double testR = OnlinePearsonsRCalculator.Calculate(originalTestValues, estimatedTestValues, out errorState);
      TestRSquared = errorState == OnlineCalculatorError.None ? testR*testR : double.NaN;

      double trainingNormalizedGini = NormalizedGiniCalculator.Calculate(originalTrainingValues, estimatedTrainingValues, out errorState);
      if (errorState != OnlineCalculatorError.None) trainingNormalizedGini = double.NaN;
      double testNormalizedGini = NormalizedGiniCalculator.Calculate(originalTestValues, estimatedTestValues, out errorState);
      if (errorState != OnlineCalculatorError.None) testNormalizedGini = double.NaN;

      TrainingNormalizedGiniCoefficient = trainingNormalizedGini;
      TestNormalizedGiniCoefficient = testNormalizedGini;
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

    protected virtual void OnModelThresholdsChanged(EventArgs e) {
      OnModelChanged();
    }

    public abstract IEnumerable<double> EstimatedValues { get; }
    public abstract IEnumerable<double> EstimatedTrainingValues { get; }
    public abstract IEnumerable<double> EstimatedTestValues { get; }

    public abstract IEnumerable<double> GetEstimatedValues(IEnumerable<int> rows);

    protected override void RecalculateResults() {
      base.RecalculateResults();
      CalculateRegressionResults();
    }
  }
}
