#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Fossil;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableType("E3F334B4-9980-473C-B77F-128AFAFD1DD1")]
  [Item("Prognosis Results", "Represents a collection of time series prognosis results.")]
  public class TimeSeriesPrognosisResults : ResultCollection {
    #region result names
    protected const string PrognosisTrainingMeanSquaredErrorResultName = "Mean squared error (training)";
    protected const string PrognosisTestMeanSquaredErrorResultName = "Mean squared error (test)";
    protected const string PrognosisTrainingMeanAbsoluteErrorResultName = "Mean absolute error (training)";
    protected const string PrognosisTestMeanAbsoluteErrorResultName = "Mean absolute error (test)";
    protected const string PrognosisTrainingSquaredCorrelationResultName = "Pearson's R² (training)";
    protected const string PrognosisTestSquaredCorrelationResultName = "Pearson's R² (test)";
    protected const string PrognosisTrainingRelativeErrorResultName = "Average relative error (training)";
    protected const string PrognosisTestRelativeErrorResultName = "Average relative error (test)";
    protected const string PrognosisTrainingNormalizedMeanSquaredErrorResultName = "Normalized mean squared error (training)";
    protected const string PrognosisTestNormalizedMeanSquaredErrorResultName = "Normalized mean squared error (test)";
    protected const string PrognosisTrainingMeanErrorResultName = "Mean error (training)";
    protected const string PrognosisTestMeanErrorResultName = "Mean error (test)";

    protected const string PrognosisTrainingDirectionalSymmetryResultName = "Average directional symmetry (training)";
    protected const string PrognosisTestDirectionalSymmetryResultName = "Average directional symmetry (test)";
    protected const string PrognosisTrainingWeightedDirectionalSymmetryResultName = "Average weighted directional symmetry (training)";
    protected const string PrognosisTestWeightedDirectionalSymmetryResultName = "Average weighted directional symmetry (test)";
    protected const string PrognosisTrainingTheilsUStatisticAR1ResultName = "Theil's U2 (AR1) (training)";
    protected const string PrognosisTestTheilsUStatisticAR1ResultName = "Theil's U2 (AR1) (test)";
    protected const string PrognosisTrainingTheilsUStatisticMeanResultName = "Theil's U2 (mean) (training)";
    protected const string PrognosisTestTheilsUStatisticMeanResultName = "Theil's U2 (mean) (test)";
    #endregion

    #region result descriptions
    protected const string PrognosisTrainingMeanSquaredErrorResultDescription = "Mean of squared errors of the model on the training partition";
    protected const string PrognosisTestMeanSquaredErrorResultDescription = "Mean of squared errors of the model on the test partition";
    protected const string PrognosisTrainingMeanAbsoluteErrorResultDescription = "Mean of absolute errors of the model on the training partition";
    protected const string PrognosisTestMeanAbsoluteErrorResultDescription = "Mean of absolute errors of the model on the test partition";
    protected const string PrognosisTrainingSquaredCorrelationResultDescription = "Squared Pearson's correlation coefficient of the model output and the actual values on the training partition";
    protected const string PrognosisTestSquaredCorrelationResultDescription = "Squared Pearson's correlation coefficient of the model output and the actual values on the test partition";
    protected const string PrognosisTrainingRelativeErrorResultDescription = "Average of the relative errors of the model output and the actual values on the training partition";
    protected const string PrognosisTestRelativeErrorResultDescription = "Average of the relative errors of the model output and the actual values on the test partition";
    protected const string PrognosisTrainingNormalizedMeanSquaredErrorResultDescription = "Normalized mean of squared errors of the model on the training partition";
    protected const string PrognosisTestNormalizedMeanSquaredErrorResultDescription = "Normalized mean of squared errors of the model on the test partition";
    protected const string PrognosisTrainingMeanErrorResultDescription = "Mean of errors of the model on the training partition";
    protected const string PrognosisTestMeanErrorResultDescription = "Mean of errors of the model on the test partition";

    protected const string PrognosisTrainingDirectionalSymmetryResultDescription = "The average directional symmetry of the forecasts of the model on the training partition";
    protected const string PrognosisTestDirectionalSymmetryResultDescription = "The average directional symmetry of the forecasts of the model on the test partition";
    protected const string PrognosisTrainingWeightedDirectionalSymmetryResultDescription = "The average weighted directional symmetry of the forecasts of the model on the training partition";
    protected const string PrognosisTestWeightedDirectionalSymmetryResultDescription = "The average weighted directional symmetry of the forecasts of the model on the test partition";
    protected const string PrognosisTrainingTheilsUStatisticAR1ResultDescription = "The Theil's U statistic (reference: AR1 model) of the forecasts of the model on the training partition";
    protected const string PrognosisTestTheilsUStatisticAR1ResultDescription = "The Theil's U statistic (reference: AR1 model) of the forecasts of the model on the test partition";
    protected const string PrognosisTrainingTheilsUStatisticMeanResultDescription = "The Theil's U statistic (reference: mean model) of the forecasts of the model on the training partition";
    protected const string PrognosisTestTheilsUStatisticMeanResultDescription = "The Theil's U statistic (reference: mean value) of the forecasts of the model on the test partition";
    #endregion

    #region result properties
    //prognosis results for different horizons
    public double PrognosisTrainingMeanSquaredError {
      get {
        if (!ContainsKey(PrognosisTrainingMeanSquaredErrorResultName)) return double.NaN;
        return ((DoubleValue)this[PrognosisTrainingMeanSquaredErrorResultName].Value).Value;
      }
      private set {
        if (!ContainsKey(PrognosisTrainingMeanSquaredErrorResultName)) Add(new Result(PrognosisTrainingMeanSquaredErrorResultName, PrognosisTrainingMeanSquaredErrorResultDescription, new DoubleValue()));
        ((DoubleValue)this[PrognosisTrainingMeanSquaredErrorResultName].Value).Value = value;
      }
    }

    public double PrognosisTestMeanSquaredError {
      get {
        if (!ContainsKey(PrognosisTestMeanSquaredErrorResultName)) return double.NaN;
        return ((DoubleValue)this[PrognosisTestMeanSquaredErrorResultName].Value).Value;
      }
      private set {
        if (!ContainsKey(PrognosisTestMeanSquaredErrorResultName)) Add(new Result(PrognosisTestMeanSquaredErrorResultName, PrognosisTestMeanSquaredErrorResultDescription, new DoubleValue()));
        ((DoubleValue)this[PrognosisTestMeanSquaredErrorResultName].Value).Value = value;
      }
    }

    public double PrognosisTrainingMeanAbsoluteError {
      get {
        if (!ContainsKey(PrognosisTrainingMeanAbsoluteErrorResultName)) return double.NaN;
        return ((DoubleValue)this[PrognosisTrainingMeanAbsoluteErrorResultName].Value).Value;
      }
      private set {
        if (!ContainsKey(PrognosisTrainingMeanAbsoluteErrorResultName)) Add(new Result(PrognosisTrainingMeanAbsoluteErrorResultName, PrognosisTrainingMeanAbsoluteErrorResultDescription, new DoubleValue()));
        ((DoubleValue)this[PrognosisTrainingMeanAbsoluteErrorResultName].Value).Value = value;
      }
    }

    public double PrognosisTestMeanAbsoluteError {
      get {
        if (!ContainsKey(PrognosisTestMeanAbsoluteErrorResultName)) return double.NaN;
        return ((DoubleValue)this[PrognosisTestMeanAbsoluteErrorResultName].Value).Value;
      }
      private set {
        if (!ContainsKey(PrognosisTestMeanAbsoluteErrorResultName)) Add(new Result(PrognosisTestMeanAbsoluteErrorResultName, PrognosisTestMeanAbsoluteErrorResultDescription, new DoubleValue()));
        ((DoubleValue)this[PrognosisTestMeanAbsoluteErrorResultName].Value).Value = value;
      }
    }

    public double PrognosisTrainingRSquared {
      get {
        if (!ContainsKey(PrognosisTrainingSquaredCorrelationResultName)) return double.NaN;
        return ((DoubleValue)this[PrognosisTrainingSquaredCorrelationResultName].Value).Value;
      }
      private set {
        if (!ContainsKey(PrognosisTrainingSquaredCorrelationResultName)) Add(new Result(PrognosisTrainingSquaredCorrelationResultName, PrognosisTrainingSquaredCorrelationResultDescription, new DoubleValue()));
        ((DoubleValue)this[PrognosisTrainingSquaredCorrelationResultName].Value).Value = value;
      }
    }

    public double PrognosisTestRSquared {
      get {
        if (!ContainsKey(PrognosisTestSquaredCorrelationResultName)) return double.NaN;
        return ((DoubleValue)this[PrognosisTestSquaredCorrelationResultName].Value).Value;
      }
      private set {
        if (!ContainsKey(PrognosisTestSquaredCorrelationResultName)) Add(new Result(PrognosisTestSquaredCorrelationResultName, PrognosisTestSquaredCorrelationResultDescription, new DoubleValue()));
        ((DoubleValue)this[PrognosisTestSquaredCorrelationResultName].Value).Value = value;
      }
    }

    public double PrognosisTrainingRelativeError {
      get {
        if (!ContainsKey(PrognosisTrainingRelativeErrorResultName)) return double.NaN;
        return ((DoubleValue)this[PrognosisTrainingRelativeErrorResultName].Value).Value;
      }
      private set {
        if (!ContainsKey(PrognosisTrainingRelativeErrorResultName)) Add(new Result(PrognosisTrainingRelativeErrorResultName, PrognosisTrainingRelativeErrorResultDescription, new DoubleValue()));
        ((DoubleValue)this[PrognosisTrainingRelativeErrorResultName].Value).Value = value;
      }
    }

    public double PrognosisTestRelativeError {
      get {
        if (!ContainsKey(PrognosisTestRelativeErrorResultName)) return double.NaN;
        return ((DoubleValue)this[PrognosisTestRelativeErrorResultName].Value).Value;
      }
      private set {
        if (!ContainsKey(PrognosisTestRelativeErrorResultName)) Add(new Result(PrognosisTestRelativeErrorResultName, PrognosisTestRelativeErrorResultDescription, new DoubleValue()));
        ((DoubleValue)this[PrognosisTestRelativeErrorResultName].Value).Value = value;
      }
    }

    public double PrognosisTrainingNormalizedMeanSquaredError {
      get {
        if (!ContainsKey(PrognosisTrainingNormalizedMeanSquaredErrorResultName)) return double.NaN;
        return ((DoubleValue)this[PrognosisTrainingNormalizedMeanSquaredErrorResultName].Value).Value;
      }
      private set {
        if (!ContainsKey(PrognosisTrainingNormalizedMeanSquaredErrorResultName)) Add(new Result(PrognosisTrainingNormalizedMeanSquaredErrorResultName, PrognosisTrainingNormalizedMeanSquaredErrorResultDescription, new DoubleValue()));
        ((DoubleValue)this[PrognosisTrainingNormalizedMeanSquaredErrorResultName].Value).Value = value;
      }
    }

    public double PrognosisTestNormalizedMeanSquaredError {
      get {
        if (!ContainsKey(PrognosisTestNormalizedMeanSquaredErrorResultName)) return double.NaN;
        return ((DoubleValue)this[PrognosisTestNormalizedMeanSquaredErrorResultName].Value).Value;
      }
      private set {
        if (!ContainsKey(PrognosisTestNormalizedMeanSquaredErrorResultName)) Add(new Result(PrognosisTestNormalizedMeanSquaredErrorResultName, PrognosisTestNormalizedMeanSquaredErrorResultDescription, new DoubleValue()));
        ((DoubleValue)this[PrognosisTestNormalizedMeanSquaredErrorResultName].Value).Value = value;
      }
    }

    public double PrognosisTrainingMeanError {
      get {
        if (!ContainsKey(PrognosisTrainingMeanErrorResultName)) return double.NaN;
        return ((DoubleValue)this[PrognosisTrainingMeanErrorResultName].Value).Value;
      }
      private set {
        if (!ContainsKey(PrognosisTrainingMeanErrorResultName)) Add(new Result(PrognosisTrainingMeanErrorResultName, PrognosisTrainingMeanErrorResultDescription, new DoubleValue()));
        ((DoubleValue)this[PrognosisTrainingMeanErrorResultName].Value).Value = value;
      }
    }

    public double PrognosisTestMeanError {
      get {
        if (!ContainsKey(PrognosisTestMeanErrorResultName)) return double.NaN;
        return ((DoubleValue)this[PrognosisTestMeanErrorResultName].Value).Value;
      }
      private set {
        if (!ContainsKey(PrognosisTestMeanErrorResultName)) Add(new Result(PrognosisTestMeanErrorResultName, PrognosisTestMeanErrorResultDescription, new DoubleValue()));
        ((DoubleValue)this[PrognosisTestMeanErrorResultName].Value).Value = value;
      }
    }


    public double PrognosisTrainingDirectionalSymmetry {
      get {
        if (!ContainsKey(PrognosisTrainingDirectionalSymmetryResultName)) return double.NaN;
        return ((DoubleValue)this[PrognosisTrainingDirectionalSymmetryResultName].Value).Value;
      }
      private set {
        if (!ContainsKey(PrognosisTrainingDirectionalSymmetryResultName)) Add(new Result(PrognosisTrainingDirectionalSymmetryResultName, PrognosisTrainingDirectionalSymmetryResultDescription, new DoubleValue()));
        ((DoubleValue)this[PrognosisTrainingDirectionalSymmetryResultName].Value).Value = value;
      }
    }
    public double PrognosisTestDirectionalSymmetry {
      get {
        if (!ContainsKey(PrognosisTestDirectionalSymmetryResultName)) return double.NaN;
        return ((DoubleValue)this[PrognosisTestDirectionalSymmetryResultName].Value).Value;
      }
      private set {
        if (!ContainsKey(PrognosisTestDirectionalSymmetryResultName)) Add(new Result(PrognosisTestDirectionalSymmetryResultName, PrognosisTestDirectionalSymmetryResultDescription, new DoubleValue()));
        ((DoubleValue)this[PrognosisTestDirectionalSymmetryResultName].Value).Value = value;
      }
    }
    public double PrognosisTrainingWeightedDirectionalSymmetry {
      get {
        if (!ContainsKey(PrognosisTrainingWeightedDirectionalSymmetryResultName)) return double.NaN;
        return ((DoubleValue)this[PrognosisTrainingWeightedDirectionalSymmetryResultName].Value).Value;
      }
      private set {
        if (!ContainsKey(PrognosisTrainingWeightedDirectionalSymmetryResultName)) Add(new Result(PrognosisTrainingWeightedDirectionalSymmetryResultName, PrognosisTrainingWeightedDirectionalSymmetryResultDescription, new DoubleValue()));
        ((DoubleValue)this[PrognosisTrainingWeightedDirectionalSymmetryResultName].Value).Value = value;
      }
    }
    public double PrognosisTestWeightedDirectionalSymmetry {
      get {
        if (!ContainsKey(PrognosisTestWeightedDirectionalSymmetryResultName)) return double.NaN;
        return ((DoubleValue)this[PrognosisTestWeightedDirectionalSymmetryResultName].Value).Value;
      }
      private set {
        if (!ContainsKey(PrognosisTestWeightedDirectionalSymmetryResultName)) Add(new Result(PrognosisTestWeightedDirectionalSymmetryResultName, PrognosisTestWeightedDirectionalSymmetryResultDescription, new DoubleValue()));
        ((DoubleValue)this[PrognosisTestWeightedDirectionalSymmetryResultName].Value).Value = value;
      }
    }
    public double PrognosisTrainingTheilsUStatisticAR1 {
      get {
        if (!ContainsKey(PrognosisTrainingTheilsUStatisticAR1ResultName)) return double.NaN;
        return ((DoubleValue)this[PrognosisTrainingTheilsUStatisticAR1ResultName].Value).Value;
      }
      private set {
        if (!ContainsKey(PrognosisTrainingTheilsUStatisticAR1ResultName)) Add(new Result(PrognosisTrainingTheilsUStatisticAR1ResultName, PrognosisTrainingTheilsUStatisticAR1ResultDescription, new DoubleValue()));
        ((DoubleValue)this[PrognosisTrainingTheilsUStatisticAR1ResultName].Value).Value = value;
      }
    }
    public double PrognosisTestTheilsUStatisticAR1 {
      get {
        if (!ContainsKey(PrognosisTestTheilsUStatisticAR1ResultName)) return double.NaN;
        return ((DoubleValue)this[PrognosisTestTheilsUStatisticAR1ResultName].Value).Value;
      }
      private set {
        if (!ContainsKey(PrognosisTestTheilsUStatisticAR1ResultName)) Add(new Result(PrognosisTestTheilsUStatisticAR1ResultName, PrognosisTestTheilsUStatisticAR1ResultDescription, new DoubleValue()));
        ((DoubleValue)this[PrognosisTestTheilsUStatisticAR1ResultName].Value).Value = value;
      }
    }
    public double PrognosisTrainingTheilsUStatisticMean {
      get {
        if (!ContainsKey(PrognosisTrainingTheilsUStatisticMeanResultName)) return double.NaN;
        return ((DoubleValue)this[PrognosisTrainingTheilsUStatisticMeanResultName].Value).Value;
      }
      private set {
        if (!ContainsKey(PrognosisTrainingTheilsUStatisticMeanResultName)) Add(new Result(PrognosisTrainingTheilsUStatisticMeanResultName, PrognosisTrainingTheilsUStatisticMeanResultDescription, new DoubleValue()));
        ((DoubleValue)this[PrognosisTrainingTheilsUStatisticMeanResultName].Value).Value = value;
      }
    }
    public double PrognosisTestTheilsUStatisticMean {
      get {
        if (!ContainsKey(PrognosisTestTheilsUStatisticMeanResultName)) return double.NaN;
        return ((DoubleValue)this[PrognosisTestTheilsUStatisticMeanResultName].Value).Value;
      }
      private set {
        if (!ContainsKey(PrognosisTestTheilsUStatisticMeanResultName)) Add(new Result(PrognosisTestTheilsUStatisticMeanResultName, PrognosisTestTheilsUStatisticMeanResultDescription, new DoubleValue()));
        ((DoubleValue)this[PrognosisTestTheilsUStatisticMeanResultName].Value).Value = value;
      }
    }
    #endregion

    [Storable]
    private int trainingHorizon;
    public int TrainingHorizon {
      get { return trainingHorizon; }
      set {
        if (trainingHorizon != value) {
          trainingHorizon = value;
          OnTrainingHorizonChanged();
        }
      }
    }

    [Storable]
    private int testHorizon;
    public int TestHorizon {
      get { return testHorizon; }
      set {
        if (testHorizon != value) {
          testHorizon = value;
          OnTestHorizonChanged();
        }
      }
    }

    private ITimeSeriesPrognosisSolution solution;
    [Storable]
    public ITimeSeriesPrognosisSolution Solution {
      get { return solution; }
      private set { solution = value; } //necessary for persistence
    }

    [StorableConstructor]
    public TimeSeriesPrognosisResults(StorableConstructorFlag _) : base(_) { }
    protected TimeSeriesPrognosisResults(TimeSeriesPrognosisResults original, Cloner cloner)
      : base(original, cloner) {
      this.trainingHorizon = original.trainingHorizon;
      this.testHorizon = original.testHorizon;
      this.solution = cloner.Clone(original.solution);
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new TimeSeriesPrognosisResults(this, cloner);
    }

    public TimeSeriesPrognosisResults(int trainingHorizon, int testHorizon, ITimeSeriesPrognosisSolution solution)
      : base() {
      this.trainingHorizon = trainingHorizon;
      this.testHorizon = testHorizon;
      this.solution = solution;
      CalculateTrainingPrognosisResults();
      CalculateTestPrognosisResults();
    }

    #region events
    public event EventHandler TrainingHorizonChanged;
    protected virtual void OnTrainingHorizonChanged() {
      CalculateTrainingPrognosisResults();
      var handler = TrainingHorizonChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler TestHorizonChanged;
    protected virtual void OnTestHorizonChanged() {
      CalculateTestPrognosisResults();
      var handler = TestHorizonChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    #endregion

    private void CalculateTrainingPrognosisResults() {
      OnlineCalculatorError errorState;
      var problemData = Solution.ProblemData;
      if (!problemData.TrainingIndices.Any()) return;
      var model = Solution.Model;
      //mean model
      double trainingMean = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, problemData.TrainingIndices).Average();
      var meanModel = new ConstantModel(trainingMean, problemData.TargetVariable);

      //AR1 model
      double alpha, beta;
      IEnumerable<double> trainingStartValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, problemData.TrainingIndices.Select(r => r - 1).Where(r => r > 0)).ToList();
      OnlineLinearScalingParameterCalculator.Calculate(problemData.Dataset.GetDoubleValues(problemData.TargetVariable, problemData.TrainingIndices.Where(x => x > 0)), trainingStartValues, out alpha, out beta, out errorState);
      var AR1model = new TimeSeriesPrognosisAutoRegressiveModel(problemData.TargetVariable, new double[] { beta }, alpha);

      var trainingHorizions = problemData.TrainingIndices.Select(r => Math.Min(trainingHorizon, problemData.TrainingPartition.End - r)).ToList();
      IEnumerable<IEnumerable<double>> trainingTargetValues = problemData.TrainingIndices.Zip(trainingHorizions, Enumerable.Range).Select(r => problemData.Dataset.GetDoubleValues(problemData.TargetVariable, r)).ToList();
      IEnumerable<IEnumerable<double>> trainingEstimatedValues = model.GetPrognosedValues(problemData.Dataset, problemData.TrainingIndices, trainingHorizions).ToList();
      IEnumerable<IEnumerable<double>> trainingMeanModelPredictions = meanModel.GetPrognosedValues(problemData.Dataset, problemData.TrainingIndices, trainingHorizions).ToList();
      IEnumerable<IEnumerable<double>> trainingAR1ModelPredictions = AR1model.GetPrognosedValues(problemData.Dataset, problemData.TrainingIndices, trainingHorizions).ToList();

      IEnumerable<double> originalTrainingValues = trainingTargetValues.SelectMany(x => x).ToList();
      IEnumerable<double> estimatedTrainingValues = trainingEstimatedValues.SelectMany(x => x).ToList();

      double trainingMSE = OnlineMeanSquaredErrorCalculator.Calculate(originalTrainingValues, estimatedTrainingValues, out errorState);
      PrognosisTrainingMeanSquaredError = errorState == OnlineCalculatorError.None ? trainingMSE : double.NaN;
      double trainingMAE = OnlineMeanAbsoluteErrorCalculator.Calculate(originalTrainingValues, estimatedTrainingValues, out errorState);
      PrognosisTrainingMeanAbsoluteError = errorState == OnlineCalculatorError.None ? trainingMAE : double.NaN;
      double trainingR = OnlinePearsonsRCalculator.Calculate(originalTrainingValues, estimatedTrainingValues, out errorState);
      PrognosisTrainingRSquared = errorState == OnlineCalculatorError.None ? trainingR * trainingR : double.NaN;
      double trainingRelError = OnlineMeanAbsolutePercentageErrorCalculator.Calculate(originalTrainingValues, estimatedTrainingValues, out errorState);
      PrognosisTrainingRelativeError = errorState == OnlineCalculatorError.None ? trainingRelError : double.NaN;
      double trainingNMSE = OnlineNormalizedMeanSquaredErrorCalculator.Calculate(originalTrainingValues, estimatedTrainingValues, out errorState);
      PrognosisTrainingNormalizedMeanSquaredError = errorState == OnlineCalculatorError.None ? trainingNMSE : double.NaN;
      double trainingME = OnlineMeanErrorCalculator.Calculate(originalTrainingValues, estimatedTrainingValues, out errorState);
      PrognosisTrainingMeanError = errorState == OnlineCalculatorError.None ? trainingME : double.NaN;

      PrognosisTrainingDirectionalSymmetry = OnlineDirectionalSymmetryCalculator.Calculate(trainingStartValues, trainingTargetValues, trainingEstimatedValues, out errorState);
      PrognosisTrainingDirectionalSymmetry = errorState == OnlineCalculatorError.None ? PrognosisTrainingDirectionalSymmetry : 0.0;
      PrognosisTrainingWeightedDirectionalSymmetry = OnlineWeightedDirectionalSymmetryCalculator.Calculate(trainingStartValues, trainingTargetValues, trainingEstimatedValues, out errorState);
      PrognosisTrainingWeightedDirectionalSymmetry = errorState == OnlineCalculatorError.None ? PrognosisTrainingWeightedDirectionalSymmetry : 0.0;
      PrognosisTrainingTheilsUStatisticAR1 = OnlineTheilsUStatisticCalculator.Calculate(trainingStartValues, trainingTargetValues, trainingAR1ModelPredictions, trainingEstimatedValues, out errorState);
      PrognosisTrainingTheilsUStatisticAR1 = errorState == OnlineCalculatorError.None ? PrognosisTrainingTheilsUStatisticAR1 : double.PositiveInfinity;
      PrognosisTrainingTheilsUStatisticMean = OnlineTheilsUStatisticCalculator.Calculate(trainingStartValues, trainingTargetValues, trainingMeanModelPredictions, trainingEstimatedValues, out errorState);
      PrognosisTrainingTheilsUStatisticMean = errorState == OnlineCalculatorError.None ? PrognosisTrainingTheilsUStatisticMean : double.PositiveInfinity;
    }

    private void CalculateTestPrognosisResults() {
      OnlineCalculatorError errorState;
      var problemData = Solution.ProblemData;
      if (!problemData.TestIndices.Any()) return;
      var model = Solution.Model;
      var testHorizions = problemData.TestIndices.Select(r => Math.Min(testHorizon, problemData.TestPartition.End - r)).ToList();
      IEnumerable<IEnumerable<double>> testTargetValues = problemData.TestIndices.Zip(testHorizions, Enumerable.Range).Select(r => problemData.Dataset.GetDoubleValues(problemData.TargetVariable, r)).ToList();
      IEnumerable<IEnumerable<double>> testEstimatedValues = model.GetPrognosedValues(problemData.Dataset, problemData.TestIndices, testHorizions).ToList();
      IEnumerable<double> testStartValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, problemData.TestIndices.Select(r => r - 1).Where(r => r > 0)).ToList();

      IEnumerable<double> originalTestValues = testTargetValues.SelectMany(x => x).ToList();
      IEnumerable<double> estimatedTestValues = testEstimatedValues.SelectMany(x => x).ToList();

      double testMSE = OnlineMeanSquaredErrorCalculator.Calculate(originalTestValues, estimatedTestValues, out errorState);
      PrognosisTestMeanSquaredError = errorState == OnlineCalculatorError.None ? testMSE : double.NaN;
      double testMAE = OnlineMeanAbsoluteErrorCalculator.Calculate(originalTestValues, estimatedTestValues, out errorState);
      PrognosisTestMeanAbsoluteError = errorState == OnlineCalculatorError.None ? testMAE : double.NaN;
      double testR = OnlinePearsonsRCalculator.Calculate(originalTestValues, estimatedTestValues, out errorState);
      PrognosisTestRSquared = errorState == OnlineCalculatorError.None ? testR * testR : double.NaN;
      double testRelError = OnlineMeanAbsolutePercentageErrorCalculator.Calculate(originalTestValues, estimatedTestValues, out errorState);
      PrognosisTestRelativeError = errorState == OnlineCalculatorError.None ? testRelError : double.NaN;
      double testNMSE = OnlineNormalizedMeanSquaredErrorCalculator.Calculate(originalTestValues, estimatedTestValues, out errorState);
      PrognosisTestNormalizedMeanSquaredError = errorState == OnlineCalculatorError.None ? testNMSE : double.NaN;
      double testME = OnlineMeanErrorCalculator.Calculate(originalTestValues, estimatedTestValues, out errorState);
      PrognosisTestMeanError = errorState == OnlineCalculatorError.None ? testME : double.NaN;

      PrognosisTestDirectionalSymmetry = OnlineDirectionalSymmetryCalculator.Calculate(testStartValues, testTargetValues, testEstimatedValues, out errorState);
      PrognosisTestDirectionalSymmetry = errorState == OnlineCalculatorError.None ? PrognosisTestDirectionalSymmetry : 0.0;
      PrognosisTestWeightedDirectionalSymmetry = OnlineWeightedDirectionalSymmetryCalculator.Calculate(testStartValues, testTargetValues, testEstimatedValues, out errorState);
      PrognosisTestWeightedDirectionalSymmetry = errorState == OnlineCalculatorError.None ? PrognosisTestWeightedDirectionalSymmetry : 0.0;


      if (problemData.TrainingIndices.Any()) {
        //mean model
        double trainingMean = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, problemData.TrainingIndices).Average();
        var meanModel = new ConstantModel(trainingMean, problemData.TargetVariable);

        //AR1 model
        double alpha, beta;
        IEnumerable<double> trainingStartValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, problemData.TrainingIndices.Select(r => r - 1).Where(r => r > 0)).ToList();
        OnlineLinearScalingParameterCalculator.Calculate(problemData.Dataset.GetDoubleValues(problemData.TargetVariable, problemData.TrainingIndices.Where(x => x > 0)), trainingStartValues, out alpha, out beta, out errorState);
        var AR1model = new TimeSeriesPrognosisAutoRegressiveModel(problemData.TargetVariable, new double[] { beta }, alpha);

        IEnumerable<IEnumerable<double>> testMeanModelPredictions = meanModel.GetPrognosedValues(problemData.Dataset, problemData.TestIndices, testHorizions).ToList();
        IEnumerable<IEnumerable<double>> testAR1ModelPredictions = AR1model.GetPrognosedValues(problemData.Dataset, problemData.TestIndices, testHorizions).ToList();

        PrognosisTestTheilsUStatisticAR1 = OnlineTheilsUStatisticCalculator.Calculate(testStartValues, testTargetValues, testAR1ModelPredictions, testEstimatedValues, out errorState);
        PrognosisTestTheilsUStatisticAR1 = errorState == OnlineCalculatorError.None ? PrognosisTestTheilsUStatisticAR1 : double.PositiveInfinity;
        PrognosisTestTheilsUStatisticMean = OnlineTheilsUStatisticCalculator.Calculate(testStartValues, testTargetValues, testMeanModelPredictions, testEstimatedValues, out errorState);
        PrognosisTestTheilsUStatisticMean = errorState == OnlineCalculatorError.None ? PrognosisTestTheilsUStatisticMean : double.PositiveInfinity;
      }
    }
  }
}
