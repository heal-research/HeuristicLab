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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableClass]
  public abstract class TimeSeriesPrognosisSolutionBase : DataAnalysisSolution, ITimeSeriesPrognosisSolution {
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
    private const string TrainingDirectionalSymmetryResultName = "Directional symmetry (training)";
    private const string TestDirectionalSymmetryResultName = "Directional symmetry (test)";
    private const string TrainingWeightedDirectionalSymmetryResultName = "Weighted directional symmetry (training)";
    private const string TestWeightedDirectionalSymmetryResultName = "Weighted directional symmetry (test)";
    private const string TrainingTheilsUStatisticResultName = "Theil's U (training)";
    private const string TestTheilsUStatisticResultName = "Theil's U (test)";

    public new ITimeSeriesPrognosisModel Model {
      get { return (ITimeSeriesPrognosisModel)base.Model; }
      protected set { base.Model = value; }
    }

    public new ITimeSeriesPrognosisProblemData ProblemData {
      get { return (ITimeSeriesPrognosisProblemData)base.ProblemData; }
      set { base.ProblemData = value; }
    }

    public abstract IEnumerable<double> PrognosedValues { get; }
    public abstract IEnumerable<double> PrognosedTrainingValues { get; }
    public abstract IEnumerable<double> PrognosedTestValues { get; }
    public abstract IEnumerable<double> GetPrognosedValues(IEnumerable<int> rows);

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
    public double TrainingDirectionalSymmetry {
      get { return ((DoubleValue)this[TrainingDirectionalSymmetryResultName].Value).Value; }
      private set { ((DoubleValue)this[TrainingDirectionalSymmetryResultName].Value).Value = value; }
    }
    public double TestDirectionalSymmetry {
      get { return ((DoubleValue)this[TestDirectionalSymmetryResultName].Value).Value; }
      private set { ((DoubleValue)this[TestDirectionalSymmetryResultName].Value).Value = value; }
    }
    public double TrainingWeightedDirectionalSymmetry {
      get { return ((DoubleValue)this[TrainingWeightedDirectionalSymmetryResultName].Value).Value; }
      private set { ((DoubleValue)this[TrainingWeightedDirectionalSymmetryResultName].Value).Value = value; }
    }
    public double TestWeightedDirectionalSymmetry {
      get { return ((DoubleValue)this[TestWeightedDirectionalSymmetryResultName].Value).Value; }
      private set { ((DoubleValue)this[TestWeightedDirectionalSymmetryResultName].Value).Value = value; }
    }
    public double TrainingTheilsUStatistic {
      get { return ((DoubleValue)this[TrainingTheilsUStatisticResultName].Value).Value; }
      private set { ((DoubleValue)this[TrainingTheilsUStatisticResultName].Value).Value = value; }
    }
    public double TestTheilsUStatistic {
      get { return ((DoubleValue)this[TestTheilsUStatisticResultName].Value).Value; }
      private set { ((DoubleValue)this[TestTheilsUStatisticResultName].Value).Value = value; }
    }
    #endregion

    [StorableConstructor]
    protected TimeSeriesPrognosisSolutionBase(bool deserializing) : base(deserializing) { }
    protected TimeSeriesPrognosisSolutionBase(TimeSeriesPrognosisSolutionBase original, Cloner cloner)
      : base(original, cloner) {
    }
    protected TimeSeriesPrognosisSolutionBase(ITimeSeriesPrognosisModel model, ITimeSeriesPrognosisProblemData problemData)
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
      Add(new Result(TrainingDirectionalSymmetryResultName, "The directional symmetry of the output of the model on the training partition", new DoubleValue()));
      Add(new Result(TestDirectionalSymmetryResultName, "The directional symmetry of the output of the model on the test partition", new DoubleValue()));
      Add(new Result(TrainingWeightedDirectionalSymmetryResultName, "The weighted directional symmetry of the output of the model on the training partition", new DoubleValue()));
      Add(new Result(TestWeightedDirectionalSymmetryResultName, "The weighted directional symmetry of the output of the model on the test partition", new DoubleValue()));
      Add(new Result(TrainingTheilsUStatisticResultName, "The Theil's U statistic of the output of the model on the training partition", new DoubleValue()));
      Add(new Result(TestTheilsUStatisticResultName, "The Theil's U statistic of the output of the model on the test partition", new DoubleValue()));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {

    }

    protected void CalculateResults() {
      double[] estimatedTrainingValues = PrognosedTrainingValues.ToArray(); // cache values
      double[] originalTrainingValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TrainingIndizes).ToArray();
      double[] estimatedTestValues = PrognosedTestValues.ToArray(); // cache values
      double[] originalTestValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TestIndizes).ToArray();

      OnlineCalculatorError errorState;
      double trainingMse = OnlineMeanSquaredErrorCalculator.Calculate(estimatedTrainingValues, originalTrainingValues, out errorState);
      TrainingMeanSquaredError = errorState == OnlineCalculatorError.None ? trainingMse : double.NaN;
      double testMse = OnlineMeanSquaredErrorCalculator.Calculate(estimatedTestValues, originalTestValues, out errorState);
      TestMeanSquaredError = errorState == OnlineCalculatorError.None ? testMse : double.NaN;

      double trainingMae = OnlineMeanAbsoluteErrorCalculator.Calculate(estimatedTrainingValues, originalTrainingValues, out errorState);
      TrainingMeanAbsoluteError = errorState == OnlineCalculatorError.None ? trainingMae : double.NaN;
      double testMae = OnlineMeanAbsoluteErrorCalculator.Calculate(estimatedTestValues, originalTestValues, out errorState);
      TestMeanAbsoluteError = errorState == OnlineCalculatorError.None ? testMae : double.NaN;

      double trainingR2 = OnlinePearsonsRSquaredCalculator.Calculate(estimatedTrainingValues, originalTrainingValues, out errorState);
      TrainingRSquared = errorState == OnlineCalculatorError.None ? trainingR2 : double.NaN;
      double testR2 = OnlinePearsonsRSquaredCalculator.Calculate(estimatedTestValues, originalTestValues, out errorState);
      TestRSquared = errorState == OnlineCalculatorError.None ? testR2 : double.NaN;

      double trainingRelError = OnlineMeanAbsolutePercentageErrorCalculator.Calculate(estimatedTrainingValues, originalTrainingValues, out errorState);
      TrainingRelativeError = errorState == OnlineCalculatorError.None ? trainingRelError : double.NaN;
      double testRelError = OnlineMeanAbsolutePercentageErrorCalculator.Calculate(estimatedTestValues, originalTestValues, out errorState);
      TestRelativeError = errorState == OnlineCalculatorError.None ? testRelError : double.NaN;

      double trainingNmse = OnlineNormalizedMeanSquaredErrorCalculator.Calculate(estimatedTrainingValues, originalTrainingValues, out errorState);
      TrainingNormalizedMeanSquaredError = errorState == OnlineCalculatorError.None ? trainingNmse : double.NaN;
      double testNmse = OnlineNormalizedMeanSquaredErrorCalculator.Calculate(estimatedTestValues, originalTestValues, out errorState);
      TestNormalizedMeanSquaredError = errorState == OnlineCalculatorError.None ? testNmse : double.NaN;

      double trainingDirectionalSymmetry = OnlineDirectionalSymmetryCalculator.Calculate(estimatedTrainingValues, originalTrainingValues, out errorState);
      TrainingDirectionalSymmetry = errorState == OnlineCalculatorError.None ? trainingDirectionalSymmetry : double.NaN;
      double testDirectionalSymmetry = OnlineDirectionalSymmetryCalculator.Calculate(estimatedTestValues, originalTestValues, out errorState);
      TestDirectionalSymmetry = errorState == OnlineCalculatorError.None ? testDirectionalSymmetry : double.NaN;

      double trainingWeightedDirectionalSymmetry = OnlineWeightedDirectionalSymmetryCalculator.Calculate(estimatedTrainingValues, originalTrainingValues, out errorState);
      TrainingWeightedDirectionalSymmetry = errorState == OnlineCalculatorError.None ? trainingWeightedDirectionalSymmetry : double.NaN;
      double testWeightedDirectionalSymmetry = OnlineWeightedDirectionalSymmetryCalculator.Calculate(estimatedTestValues, originalTestValues, out errorState);
      TestWeightedDirectionalSymmetry = errorState == OnlineCalculatorError.None ? testWeightedDirectionalSymmetry : double.NaN;

      double trainingTheilsU = OnlineTheilsUStatisticCalculator.Calculate(estimatedTrainingValues, originalTrainingValues, out errorState);
      TrainingTheilsUStatistic = errorState == OnlineCalculatorError.None ? trainingTheilsU : double.NaN;
      double testTheilsU = OnlineTheilsUStatisticCalculator.Calculate(estimatedTestValues, originalTestValues, out errorState);
      TestTheilsUStatistic = errorState == OnlineCalculatorError.None ? testTheilsU : double.NaN;


    }
  }
}
