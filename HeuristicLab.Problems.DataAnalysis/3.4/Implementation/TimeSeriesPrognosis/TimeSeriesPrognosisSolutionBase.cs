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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableClass]
  public abstract class TimeSeriesPrognosisSolutionBase : RegressionSolutionBase, ITimeSeriesPrognosisSolution {
    #region result names
    protected const string TrainingDirectionalSymmetryResultName = "Average directional symmetry (training)";
    protected const string TestDirectionalSymmetryResultName = "Average directional symmetry (test)";
    protected const string TrainingWeightedDirectionalSymmetryResultName = "Average weighted directional symmetry (training)";
    protected const string TestWeightedDirectionalSymmetryResultName = "Average weighted directional symmetry (test)";
    protected const string TrainingTheilsUStatisticAR1ResultName = "Theil's U2 (AR1) (training)";
    protected const string TestTheilsUStatisticLastResultName = "Theil's U2 (AR1) (test)";
    protected const string TrainingTheilsUStatisticMeanResultName = "Theil's U2 (mean) (training)";
    protected const string TestTheilsUStatisticMeanResultName = "Theil's U2 (mean) (test)";
    protected const string TimeSeriesPrognosisResultName = "Prognosis Results";
    #endregion

    #region result descriptions
    protected const string TrainingDirectionalSymmetryResultDescription = "The average directional symmetry of the forecasts of the model on the training partition";
    protected const string TestDirectionalSymmetryResultDescription = "The average directional symmetry of the forecasts of the model on the test partition";
    protected const string TrainingWeightedDirectionalSymmetryResultDescription = "The average weighted directional symmetry of the forecasts of the model on the training partition";
    protected const string TestWeightedDirectionalSymmetryResultDescription = "The average weighted directional symmetry of the forecasts of the model on the test partition";
    protected const string TrainingTheilsUStatisticAR1ResultDescription = "The Theil's U statistic (reference: AR1 model) of the forecasts of the model on the training partition";
    protected const string TestTheilsUStatisticAR1ResultDescription = "The Theil's U statistic (reference: AR1 model) of the forecasts of the model on the test partition";
    protected const string TrainingTheilsUStatisticMeanResultDescription = "The Theil's U statistic (reference: mean model) of the forecasts of the model on the training partition";
    protected const string TestTheilsUStatisticMeanResultDescription = "The Theil's U statistic (reference: mean value) of the forecasts of the model on the test partition";
    protected const string TimeSeriesPrognosisResultDescription = "The calculated results of predictions in the future.";
    #endregion

    public new ITimeSeriesPrognosisModel Model {
      get { return (ITimeSeriesPrognosisModel)base.Model; }
      protected set { base.Model = value; }
    }

    public new ITimeSeriesPrognosisProblemData ProblemData {
      get { return (ITimeSeriesPrognosisProblemData)base.ProblemData; }
      set { base.ProblemData = value; }
    }

    public abstract IEnumerable<double> PrognosedTestValues { get; }
    public abstract IEnumerable<IEnumerable<double>> GetPrognosedValues(IEnumerable<int> rows, IEnumerable<int> horizon);

    #region Results
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
    public double TrainingTheilsUStatisticAR1 {
      get { return ((DoubleValue)this[TrainingTheilsUStatisticAR1ResultName].Value).Value; }
      private set { ((DoubleValue)this[TrainingTheilsUStatisticAR1ResultName].Value).Value = value; }
    }
    public double TestTheilsUStatisticAR1 {
      get { return ((DoubleValue)this[TestTheilsUStatisticLastResultName].Value).Value; }
      private set { ((DoubleValue)this[TestTheilsUStatisticLastResultName].Value).Value = value; }
    }
    public double TrainingTheilsUStatisticMean {
      get { return ((DoubleValue)this[TrainingTheilsUStatisticMeanResultName].Value).Value; }
      private set { ((DoubleValue)this[TrainingTheilsUStatisticMeanResultName].Value).Value = value; }
    }
    public double TestTheilsUStatisticMean {
      get { return ((DoubleValue)this[TestTheilsUStatisticMeanResultName].Value).Value; }
      private set { ((DoubleValue)this[TestTheilsUStatisticMeanResultName].Value).Value = value; }
    }

    public TimeSeriesPrognosisResults TimeSeriesPrognosisResults {
      get {
        if (!ContainsKey(TimeSeriesPrognosisResultName)) return null;
        return (TimeSeriesPrognosisResults)this[TimeSeriesPrognosisResultName];
      }
      set {
        if (ContainsKey(TimeSeriesPrognosisResultName)) Remove(TimeSeriesPrognosisResultName);
        Add(new Result(TimeSeriesPrognosisResultName, TimeSeriesPrognosisResultDescription, value));
      }
    }
    #endregion


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
      return Model.GetEstimatedValues(ProblemData.Dataset, rows);
    }

    [StorableConstructor]
    protected TimeSeriesPrognosisSolutionBase(bool deserializing) : base(deserializing) { }
    protected TimeSeriesPrognosisSolutionBase(TimeSeriesPrognosisSolutionBase original, Cloner cloner) : base(original, cloner) { }
    protected TimeSeriesPrognosisSolutionBase(ITimeSeriesPrognosisModel model, ITimeSeriesPrognosisProblemData problemData)
      : base(model, problemData) {
      Add(new Result(TrainingDirectionalSymmetryResultName, TrainingDirectionalSymmetryResultDescription, new DoubleValue()));
      Add(new Result(TestDirectionalSymmetryResultName, TestDirectionalSymmetryResultDescription, new DoubleValue()));
      Add(new Result(TrainingWeightedDirectionalSymmetryResultName, TrainingWeightedDirectionalSymmetryResultDescription, new DoubleValue()));
      Add(new Result(TestWeightedDirectionalSymmetryResultName, TestWeightedDirectionalSymmetryResultDescription, new DoubleValue()));
      Add(new Result(TrainingTheilsUStatisticAR1ResultName, TrainingTheilsUStatisticAR1ResultDescription, new DoubleValue()));
      Add(new Result(TestTheilsUStatisticLastResultName, TestTheilsUStatisticAR1ResultDescription, new DoubleValue()));
      Add(new Result(TrainingTheilsUStatisticMeanResultName, TrainingTheilsUStatisticMeanResultDescription, new DoubleValue()));
      Add(new Result(TestTheilsUStatisticMeanResultName, TestTheilsUStatisticMeanResultDescription, new DoubleValue()));
    }

    protected override void RecalculateResults() {
      base.RecalculateResults();
      CalculateTimeSeriesResults();
      CalculateTimeSeriesResults(ProblemData.TrainingHorizon, ProblemData.TestHorizon);
    }

    protected void CalculateTimeSeriesResults() {
      OnlineCalculatorError errorState;
      double trainingMean = ProblemData.TrainingIndices.Any() ? ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TrainingIndices).Average() : double.NaN;
      var meanModel = new ConstantModel(trainingMean, ProblemData.TargetVariable);

      double alpha, beta;
      IEnumerable<double> trainingStartValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TrainingIndices.Select(r => r - 1).Where(r => r > 0)).ToList();
      OnlineLinearScalingParameterCalculator.Calculate(ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TrainingIndices.Where(x => x > 0)), trainingStartValues, out alpha, out beta, out errorState);
      var AR1model = new TimeSeriesPrognosisAutoRegressiveModel(ProblemData.TargetVariable, new double[] { beta }, alpha);


      #region Calculate training quality measures
      if (ProblemData.TrainingIndices.Any()) {
        IEnumerable<double> trainingTargetValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TrainingIndices).ToList();
        IEnumerable<double> trainingEstimatedValues = EstimatedTrainingValues.ToList();
        IEnumerable<double> trainingMeanModelPredictions = meanModel.GetEstimatedValues(ProblemData.Dataset, ProblemData.TrainingIndices).ToList();
        IEnumerable<double> trainingAR1ModelPredictions = AR1model.GetEstimatedValues(ProblemData.Dataset, ProblemData.TrainingIndices).ToList();

        TrainingDirectionalSymmetry = OnlineDirectionalSymmetryCalculator.Calculate(trainingTargetValues.First(), trainingTargetValues, trainingEstimatedValues, out errorState);
        TrainingDirectionalSymmetry = errorState == OnlineCalculatorError.None ? TrainingDirectionalSymmetry : 0.0;
        TrainingWeightedDirectionalSymmetry = OnlineWeightedDirectionalSymmetryCalculator.Calculate(trainingTargetValues.First(), trainingTargetValues, trainingEstimatedValues, out errorState);
        TrainingWeightedDirectionalSymmetry = errorState == OnlineCalculatorError.None ? TrainingWeightedDirectionalSymmetry : 0.0;
        TrainingTheilsUStatisticAR1 = OnlineTheilsUStatisticCalculator.Calculate(trainingTargetValues.First(), trainingTargetValues, trainingAR1ModelPredictions, trainingEstimatedValues, out errorState);
        TrainingTheilsUStatisticAR1 = errorState == OnlineCalculatorError.None ? TrainingTheilsUStatisticAR1 : double.PositiveInfinity;
        TrainingTheilsUStatisticMean = OnlineTheilsUStatisticCalculator.Calculate(trainingTargetValues.First(), trainingTargetValues, trainingMeanModelPredictions, trainingEstimatedValues, out errorState);
        TrainingTheilsUStatisticMean = errorState == OnlineCalculatorError.None ? TrainingTheilsUStatisticMean : double.PositiveInfinity;
      }
      #endregion

      #region Calculate test quality measures
      if (ProblemData.TestIndices.Any()) {
        IEnumerable<double> testTargetValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TestIndices).ToList();
        IEnumerable<double> testEstimatedValues = EstimatedTestValues.ToList();
        IEnumerable<double> testMeanModelPredictions = meanModel.GetEstimatedValues(ProblemData.Dataset, ProblemData.TestIndices).ToList();
        IEnumerable<double> testAR1ModelPredictions = AR1model.GetEstimatedValues(ProblemData.Dataset, ProblemData.TestIndices).ToList();

        TestDirectionalSymmetry = OnlineDirectionalSymmetryCalculator.Calculate(testTargetValues.First(), testTargetValues, testEstimatedValues, out errorState);
        TestDirectionalSymmetry = errorState == OnlineCalculatorError.None ? TestDirectionalSymmetry : 0.0;
        TestWeightedDirectionalSymmetry = OnlineWeightedDirectionalSymmetryCalculator.Calculate(testTargetValues.First(), testTargetValues, testEstimatedValues, out errorState);
        TestWeightedDirectionalSymmetry = errorState == OnlineCalculatorError.None ? TestWeightedDirectionalSymmetry : 0.0;
        TestTheilsUStatisticAR1 = OnlineTheilsUStatisticCalculator.Calculate(testTargetValues.First(), testTargetValues, testAR1ModelPredictions, testEstimatedValues, out errorState);
        TestTheilsUStatisticAR1 = errorState == OnlineCalculatorError.None ? TestTheilsUStatisticAR1 : double.PositiveInfinity;
        TestTheilsUStatisticMean = OnlineTheilsUStatisticCalculator.Calculate(testTargetValues.First(), testTargetValues, testMeanModelPredictions, testEstimatedValues, out errorState);
        TestTheilsUStatisticMean = errorState == OnlineCalculatorError.None ? TestTheilsUStatisticMean : double.PositiveInfinity;
      }
      #endregion
    }

    protected void CalculateTimeSeriesResults(int trainingHorizon, int testHorizon) {
      TimeSeriesPrognosisResults = new TimeSeriesPrognosisResults(trainingHorizon, testHorizon, this);
    }
  }
}
