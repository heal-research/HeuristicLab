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
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableClass]
  public abstract class RegressionSolutionBase : DataAnalysisSolution, IRegressionSolution {
    private const string TrainingMeanSquaredErrorResultName = "Mean squared error (training)";
    private const string TestMeanSquaredErrorResultName = "Mean squared error (test)";
    private const string TrainingMeanAbsoluteErrorResultName = "Mean absolute error (training)";
    private const string TestMeanAbsoluteErrorResultName = "Mean absolute error (test)";
    private const string TrainingSquaredCorrelationResultName = "Pearson's R² (training)";
    private const string TestSquaredCorrelationResultName = "Pearson's R² (test)";
    private const string TrainingRelativeErrorResultName = "Average relative error (training)";
    private const string TestRelativeErrorResultName = "Average relative error (test)";
    private const string TrainingNormalizedMeanSquaredErrorResultName = "Normalized mean squared error (training)";
    private const string TestNormalizedMeanSquaredErrorResultName = "Normalized mean squared error (test)";
    private const string TrainingMeanErrorResultName = "Mean error (training)";
    private const string TestMeanErrorResultName = "Mean error (test)";

    public new IRegressionModel Model {
      get { return (IRegressionModel)base.Model; }
      protected set { base.Model = value; }
    }

    public new IRegressionProblemData ProblemData {
      get { return (IRegressionProblemData)base.ProblemData; }
      set { base.ProblemData = value; }
    }

    public abstract IEnumerable<double> EstimatedValues { get; }
    public abstract IEnumerable<double> EstimatedTrainingValues { get; }
    public abstract IEnumerable<double> EstimatedTestValues { get; }
    public abstract IEnumerable<double> GetEstimatedValues(IEnumerable<int> rows);

    #region Results
    public double TrainingMeanSquaredError {
      get { return ((DoubleValue)this[TrainingMeanSquaredErrorResultName].Value).Value; }
      private set { ((DoubleValue)this[TrainingMeanSquaredErrorResultName].Value).Value = value; }
    }
    public double TestMeanSquaredError {
      get { return ((DoubleValue)this[TestMeanSquaredErrorResultName].Value).Value; }
      private set { ((DoubleValue)this[TestMeanSquaredErrorResultName].Value).Value = value; }
    }
    public double TrainingMeanAbsoluteError {
      get { return ((DoubleValue)this[TrainingMeanAbsoluteErrorResultName].Value).Value; }
      private set { ((DoubleValue)this[TrainingMeanAbsoluteErrorResultName].Value).Value = value; }
    }
    public double TestMeanAbsoluteError {
      get { return ((DoubleValue)this[TestMeanAbsoluteErrorResultName].Value).Value; }
      private set { ((DoubleValue)this[TestMeanAbsoluteErrorResultName].Value).Value = value; }
    }
    public double TrainingRSquared {
      get { return ((DoubleValue)this[TrainingSquaredCorrelationResultName].Value).Value; }
      private set { ((DoubleValue)this[TrainingSquaredCorrelationResultName].Value).Value = value; }
    }
    public double TestRSquared {
      get { return ((DoubleValue)this[TestSquaredCorrelationResultName].Value).Value; }
      private set { ((DoubleValue)this[TestSquaredCorrelationResultName].Value).Value = value; }
    }
    public double TrainingRelativeError {
      get { return ((DoubleValue)this[TrainingRelativeErrorResultName].Value).Value; }
      private set { ((DoubleValue)this[TrainingRelativeErrorResultName].Value).Value = value; }
    }
    public double TestRelativeError {
      get { return ((DoubleValue)this[TestRelativeErrorResultName].Value).Value; }
      private set { ((DoubleValue)this[TestRelativeErrorResultName].Value).Value = value; }
    }
    public double TrainingNormalizedMeanSquaredError {
      get { return ((DoubleValue)this[TrainingNormalizedMeanSquaredErrorResultName].Value).Value; }
      private set { ((DoubleValue)this[TrainingNormalizedMeanSquaredErrorResultName].Value).Value = value; }
    }
    public double TestNormalizedMeanSquaredError {
      get { return ((DoubleValue)this[TestNormalizedMeanSquaredErrorResultName].Value).Value; }
      private set { ((DoubleValue)this[TestNormalizedMeanSquaredErrorResultName].Value).Value = value; }
    }
    public double TrainingMeanError {
      get { return ((DoubleValue)this[TrainingMeanErrorResultName].Value).Value; }
      private set { ((DoubleValue)this[TrainingMeanErrorResultName].Value).Value = value; }
    }
    public double TestMeanError {
      get { return ((DoubleValue)this[TestMeanErrorResultName].Value).Value; }
      private set { ((DoubleValue)this[TestMeanErrorResultName].Value).Value = value; }
    }
    #endregion

    [StorableConstructor]
    protected RegressionSolutionBase(bool deserializing) : base(deserializing) { }
    protected RegressionSolutionBase(RegressionSolutionBase original, Cloner cloner)
      : base(original, cloner) {
    }
    protected RegressionSolutionBase(IRegressionModel model, IRegressionProblemData problemData)
      : base(model, problemData) {
      Add(new Result(TrainingMeanSquaredErrorResultName, "Mean of squared errors of the model on the training partition", new DoubleValue()));
      Add(new Result(TestMeanSquaredErrorResultName, "Mean of squared errors of the model on the test partition", new DoubleValue()));
      Add(new Result(TrainingMeanAbsoluteErrorResultName, "Mean of absolute errors of the model on the training partition", new DoubleValue()));
      Add(new Result(TestMeanAbsoluteErrorResultName, "Mean of absolute errors of the model on the test partition", new DoubleValue()));
      Add(new Result(TrainingSquaredCorrelationResultName, "Squared Pearson's correlation coefficient of the model output and the actual values on the training partition", new DoubleValue()));
      Add(new Result(TestSquaredCorrelationResultName, "Squared Pearson's correlation coefficient of the model output and the actual values on the test partition", new DoubleValue()));
      Add(new Result(TrainingRelativeErrorResultName, "Average of the relative errors of the model output and the actual values on the training partition", new PercentValue()));
      Add(new Result(TestRelativeErrorResultName, "Average of the relative errors of the model output and the actual values on the test partition", new PercentValue()));
      Add(new Result(TrainingNormalizedMeanSquaredErrorResultName, "Normalized mean of squared errors of the model on the training partition", new DoubleValue()));
      Add(new Result(TestNormalizedMeanSquaredErrorResultName, "Normalized mean of squared errors of the model on the test partition", new DoubleValue()));
      Add(new Result(TrainingMeanErrorResultName, "Mean of errors of the model on the training partition", new DoubleValue()));
      Add(new Result(TestMeanErrorResultName, "Mean of errors of the model on the test partition", new DoubleValue()));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.4

      #region Backwards compatible code, remove with 3.5

      if (!ContainsKey(TrainingMeanAbsoluteErrorResultName)) {
        OnlineCalculatorError errorState;
        Add(new Result(TrainingMeanAbsoluteErrorResultName, "Mean of absolute errors of the model on the training partition", new DoubleValue()));
        double trainingMAE = OnlineMeanAbsoluteErrorCalculator.Calculate(EstimatedTrainingValues, ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TrainingIndices), out errorState);
        TrainingMeanAbsoluteError = errorState == OnlineCalculatorError.None ? trainingMAE : double.NaN;
      }

      if (!ContainsKey(TestMeanAbsoluteErrorResultName)) {
        OnlineCalculatorError errorState;
        Add(new Result(TestMeanAbsoluteErrorResultName, "Mean of absolute errors of the model on the test partition", new DoubleValue()));
        double testMAE = OnlineMeanAbsoluteErrorCalculator.Calculate(EstimatedTestValues, ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TestIndices), out errorState);
        TestMeanAbsoluteError = errorState == OnlineCalculatorError.None ? testMAE : double.NaN;
      }

      if (!ContainsKey(TrainingMeanErrorResultName)) {
        OnlineCalculatorError errorState;
        Add(new Result(TrainingMeanErrorResultName, "Mean of errors of the model on the training partition", new DoubleValue()));
        double trainingME = OnlineMeanErrorCalculator.Calculate(EstimatedTrainingValues, ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TrainingIndices), out errorState);
        TrainingMeanError = errorState == OnlineCalculatorError.None ? trainingME : double.NaN;
      }
      if (!ContainsKey(TestMeanErrorResultName)) {
        OnlineCalculatorError errorState;
        Add(new Result(TestMeanErrorResultName, "Mean of errors of the model on the test partition", new DoubleValue()));
        double testME = OnlineMeanErrorCalculator.Calculate(EstimatedTestValues, ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TestIndices), out errorState);
        TestMeanError = errorState == OnlineCalculatorError.None ? testME : double.NaN;
      }
      #endregion
    }

    protected void CalculateResults() {
      IEnumerable<double> estimatedTrainingValues = EstimatedTrainingValues; // cache values
      IEnumerable<double> originalTrainingValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TrainingIndices);
      IEnumerable<double> estimatedTestValues = EstimatedTestValues; // cache values
      IEnumerable<double> originalTestValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TestIndices);

      OnlineCalculatorError errorState;
      double trainingMSE = OnlineMeanSquaredErrorCalculator.Calculate(originalTrainingValues, estimatedTrainingValues, out errorState);
      TrainingMeanSquaredError = errorState == OnlineCalculatorError.None ? trainingMSE : double.NaN;
      double testMSE = OnlineMeanSquaredErrorCalculator.Calculate(originalTestValues, estimatedTestValues, out errorState);
      TestMeanSquaredError = errorState == OnlineCalculatorError.None ? testMSE : double.NaN;

      double trainingMAE = OnlineMeanAbsoluteErrorCalculator.Calculate(originalTrainingValues, estimatedTrainingValues, out errorState);
      TrainingMeanAbsoluteError = errorState == OnlineCalculatorError.None ? trainingMAE : double.NaN;
      double testMAE = OnlineMeanAbsoluteErrorCalculator.Calculate(originalTestValues, estimatedTestValues, out errorState);
      TestMeanAbsoluteError = errorState == OnlineCalculatorError.None ? testMAE : double.NaN;

      double trainingR2 = OnlinePearsonsRSquaredCalculator.Calculate(originalTrainingValues, estimatedTrainingValues, out errorState);
      TrainingRSquared = errorState == OnlineCalculatorError.None ? trainingR2 : double.NaN;
      double testR2 = OnlinePearsonsRSquaredCalculator.Calculate(originalTestValues, estimatedTestValues, out errorState);
      TestRSquared = errorState == OnlineCalculatorError.None ? testR2 : double.NaN;

      double trainingRelError = OnlineMeanAbsolutePercentageErrorCalculator.Calculate(originalTrainingValues, estimatedTrainingValues, out errorState);
      TrainingRelativeError = errorState == OnlineCalculatorError.None ? trainingRelError : double.NaN;
      double testRelError = OnlineMeanAbsolutePercentageErrorCalculator.Calculate(originalTestValues, estimatedTestValues, out errorState);
      TestRelativeError = errorState == OnlineCalculatorError.None ? testRelError : double.NaN;

      double trainingNMSE = OnlineNormalizedMeanSquaredErrorCalculator.Calculate(originalTrainingValues, estimatedTrainingValues, out errorState);
      TrainingNormalizedMeanSquaredError = errorState == OnlineCalculatorError.None ? trainingNMSE : double.NaN;
      double testNMSE = OnlineNormalizedMeanSquaredErrorCalculator.Calculate(originalTestValues, estimatedTestValues, out errorState);
      TestNormalizedMeanSquaredError = errorState == OnlineCalculatorError.None ? testNMSE : double.NaN;

      double trainingME = OnlineMeanErrorCalculator.Calculate(originalTrainingValues, estimatedTrainingValues, out errorState);
      TrainingMeanError = errorState == OnlineCalculatorError.None ? trainingME : double.NaN;
      double testME = OnlineMeanErrorCalculator.Calculate(originalTestValues, estimatedTestValues, out errorState);
      TestMeanError = errorState == OnlineCalculatorError.None ? testME : double.NaN;
    }
  }
}
