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
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableClass]
  public class ClassificationPerformanceMeasuresResultCollection : ResultCollection {
    #region result names
    protected const string ClassificationPositiveClassNameResultName = "Classification positive class";
    protected const string TrainingTruePositiveRateResultName = "True positive rate (training)";
    protected const string TrainingTrueNegativeRateResultName = "True negative rate (training)";
    protected const string TrainingPositivePredictiveValueResultName = "Positive predictive value (training)";
    protected const string TrainingNegativePredictiveValueResultName = "Negative predictive value (training)";
    protected const string TrainingFalsePositiveRateResultName = "False positive rate (training)";
    protected const string TrainingFalseDiscoveryRateResultName = "False discovery rate (training)";
    protected const string TrainingF1ScoreResultName = "F1 score (training)";
    protected const string TrainingMatthewsCorrelationResultName = "Matthews Correlation (training)";
    protected const string TestTruePositiveRateResultName = "True positive rate (test)";
    protected const string TestTrueNegativeRateResultName = "True negative rate (test)";
    protected const string TestPositivePredictiveValueResultName = "Positive predictive value (test)";
    protected const string TestNegativePredictiveValueResultName = "Negative predictive value (test)";
    protected const string TestFalsePositiveRateResultName = "False positive rate (test)";
    protected const string TestFalseDiscoveryRateResultName = "False discovery rate (test)";
    protected const string TestF1ScoreResultName = "F1 score (test)";
    protected const string TestMatthewsCorrelationResultName = "Matthews Correlation (test)";
    #endregion

    public ClassificationPerformanceMeasuresResultCollection()
      : base() {
      AddMeasures();
    }
    [StorableConstructor]
    protected ClassificationPerformanceMeasuresResultCollection(bool deserializing)
      : base(deserializing) {
    }

    protected ClassificationPerformanceMeasuresResultCollection(ClassificationPerformanceMeasuresResultCollection original, Cloner cloner)
      : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ClassificationPerformanceMeasuresResultCollection(this, cloner);
    }

    #region result properties
    public string ClassificationPositiveClassName {
      get { return ((StringValue)this[ClassificationPositiveClassNameResultName].Value).Value; }
      set { ((StringValue)this[ClassificationPositiveClassNameResultName].Value).Value = value; }
    }
    public double TrainingTruePositiveRate {
      get { return ((DoubleValue)this[TrainingTruePositiveRateResultName].Value).Value; }
      set { ((DoubleValue)this[TrainingTruePositiveRateResultName].Value).Value = value; }
    }
    public double TrainingTrueNegativeRate {
      get { return ((DoubleValue)this[TrainingTrueNegativeRateResultName].Value).Value; }
      set { ((DoubleValue)this[TrainingTrueNegativeRateResultName].Value).Value = value; }
    }
    public double TrainingPositivePredictiveValue {
      get { return ((DoubleValue)this[TrainingPositivePredictiveValueResultName].Value).Value; }
      set { ((DoubleValue)this[TrainingPositivePredictiveValueResultName].Value).Value = value; }
    }
    public double TrainingNegativePredictiveValue {
      get { return ((DoubleValue)this[TrainingNegativePredictiveValueResultName].Value).Value; }
      set { ((DoubleValue)this[TrainingNegativePredictiveValueResultName].Value).Value = value; }
    }
    public double TrainingFalsePositiveRate {
      get { return ((DoubleValue)this[TrainingFalsePositiveRateResultName].Value).Value; }
      set { ((DoubleValue)this[TrainingFalsePositiveRateResultName].Value).Value = value; }
    }
    public double TrainingFalseDiscoveryRate {
      get { return ((DoubleValue)this[TrainingFalseDiscoveryRateResultName].Value).Value; }
      set { ((DoubleValue)this[TrainingFalseDiscoveryRateResultName].Value).Value = value; }
    }
    public double TrainingF1Score {
      get { return ((DoubleValue)this[TrainingF1ScoreResultName].Value).Value; }
      set { ((DoubleValue)this[TrainingF1ScoreResultName].Value).Value = value; }
    }
    public double TrainingMatthewsCorrelation {
      get { return ((DoubleValue)this[TrainingMatthewsCorrelationResultName].Value).Value; }
      set { ((DoubleValue)this[TrainingMatthewsCorrelationResultName].Value).Value = value; }
    }
    public double TestTruePositiveRate {
      get { return ((DoubleValue)this[TestTruePositiveRateResultName].Value).Value; }
      set { ((DoubleValue)this[TestTruePositiveRateResultName].Value).Value = value; }
    }
    public double TestTrueNegativeRate {
      get { return ((DoubleValue)this[TestTrueNegativeRateResultName].Value).Value; }
      set { ((DoubleValue)this[TestTrueNegativeRateResultName].Value).Value = value; }
    }
    public double TestPositivePredictiveValue {
      get { return ((DoubleValue)this[TestPositivePredictiveValueResultName].Value).Value; }
      set { ((DoubleValue)this[TestPositivePredictiveValueResultName].Value).Value = value; }
    }
    public double TestNegativePredictiveValue {
      get { return ((DoubleValue)this[TestNegativePredictiveValueResultName].Value).Value; }
      set { ((DoubleValue)this[TestNegativePredictiveValueResultName].Value).Value = value; }
    }
    public double TestFalsePositiveRate {
      get { return ((DoubleValue)this[TestFalsePositiveRateResultName].Value).Value; }
      set { ((DoubleValue)this[TestFalsePositiveRateResultName].Value).Value = value; }
    }
    public double TestFalseDiscoveryRate {
      get { return ((DoubleValue)this[TestFalseDiscoveryRateResultName].Value).Value; }
      set { ((DoubleValue)this[TestFalseDiscoveryRateResultName].Value).Value = value; }
    }
    public double TestF1Score {
      get { return ((DoubleValue)this[TestF1ScoreResultName].Value).Value; }
      set { ((DoubleValue)this[TestF1ScoreResultName].Value).Value = value; }
    }
    public double TestMatthewsCorrelation {
      get { return ((DoubleValue)this[TestMatthewsCorrelationResultName].Value).Value; }
      set { ((DoubleValue)this[TestMatthewsCorrelationResultName].Value).Value = value; }
    }
    #endregion

    protected void AddMeasures() {
      Add(new Result(ClassificationPositiveClassNameResultName, "The positive class which is used for the performance measure calculations.", new StringValue()));
      Add(new Result(TrainingTruePositiveRateResultName, "Sensitivity/True positive rate of the model on the training partition\n(TP/(TP+FN)).", new PercentValue()));
      Add(new Result(TrainingTrueNegativeRateResultName, "Specificity/True negative rate of the model on the training partition\n(TN/(FP+TN)).", new PercentValue()));
      Add(new Result(TrainingPositivePredictiveValueResultName, "Precision/Positive predictive value of the model on the training partition\n(TP/(TP+FP)).", new PercentValue()));
      Add(new Result(TrainingNegativePredictiveValueResultName, "Negative predictive value of the model on the training partition\n(TN/(TN+FN)).", new PercentValue()));
      Add(new Result(TrainingFalsePositiveRateResultName, "The false positive rate is the complement of the true negative rate of the model on the training partition.", new PercentValue()));
      Add(new Result(TrainingFalseDiscoveryRateResultName, "The false discovery rate is the complement of the positive predictive value of the model on the training partition.", new PercentValue()));
      Add(new Result(TrainingF1ScoreResultName, "The F1 score of the model on the training partition.", new DoubleValue()));
      Add(new Result(TrainingMatthewsCorrelationResultName, "The Matthews correlation value of the model on the training partition.", new DoubleValue()));
      Add(new Result(TestTruePositiveRateResultName, "Sensitivity/True positive rate of the model on the test partition\n(TP/(TP+FN)).", new PercentValue()));
      Add(new Result(TestTrueNegativeRateResultName, "Specificity/True negative rate of the model on the test partition\n(TN/(FP+TN)).", new PercentValue()));
      Add(new Result(TestPositivePredictiveValueResultName, "Precision/Positive predictive value of the model on the test partition\n(TP/(TP+FP)).", new PercentValue()));
      Add(new Result(TestNegativePredictiveValueResultName, "Negative predictive value of the model on the test partition\n(TN/(TN+FN)).", new PercentValue()));
      Add(new Result(TestFalsePositiveRateResultName, "The false positive rate is the complement of the true negative rate of the model on the test partition.", new PercentValue()));
      Add(new Result(TestFalseDiscoveryRateResultName, "The false discovery rate is the complement of the positive predictive value of the model on the test partition.", new PercentValue()));
      Add(new Result(TestF1ScoreResultName, "The F1 score of the model on the test partition.", new DoubleValue()));
      Add(new Result(TestMatthewsCorrelationResultName, "The Matthews correlation value of the model on the test partition.", new DoubleValue()));
      TrainingTruePositiveRate = double.NaN;
      TrainingTrueNegativeRate = double.NaN;
      TrainingPositivePredictiveValue = double.NaN;
      TrainingNegativePredictiveValue = double.NaN;
      TrainingFalsePositiveRate = double.NaN;
      TrainingFalseDiscoveryRate = double.NaN;
      TrainingF1Score = double.NaN;
      TrainingMatthewsCorrelation = double.NaN;
      TestTruePositiveRate = double.NaN;
      TestTrueNegativeRate = double.NaN;
      TestPositivePredictiveValue = double.NaN;
      TestNegativePredictiveValue = double.NaN;
      TestFalsePositiveRate = double.NaN;
      TestFalseDiscoveryRate = double.NaN;
      TestF1Score = double.NaN;
      TestMatthewsCorrelation = double.NaN;
    }

    public void SetTrainingResults(ClassificationPerformanceMeasuresCalculator trainingPerformanceCalculator) {
      if (!string.IsNullOrWhiteSpace(ClassificationPositiveClassName)
              && !ClassificationPositiveClassName.Equals(trainingPerformanceCalculator.PositiveClassName))
        throw new ArgumentException("Classification positive class of the training data doesn't match with the data of test partition.");
      ClassificationPositiveClassName = trainingPerformanceCalculator.PositiveClassName;
      TrainingTruePositiveRate = trainingPerformanceCalculator.TruePositiveRate;
      TrainingTrueNegativeRate = trainingPerformanceCalculator.TrueNegativeRate;
      TrainingPositivePredictiveValue = trainingPerformanceCalculator.PositivePredictiveValue;
      TrainingNegativePredictiveValue = trainingPerformanceCalculator.NegativePredictiveValue;
      TrainingFalsePositiveRate = trainingPerformanceCalculator.FalsePositiveRate;
      TrainingFalseDiscoveryRate = trainingPerformanceCalculator.FalseDiscoveryRate;
    }

    public void SetTestResults(ClassificationPerformanceMeasuresCalculator testPerformanceCalculator) {
      if (!string.IsNullOrWhiteSpace(ClassificationPositiveClassName)
                && !ClassificationPositiveClassName.Equals(testPerformanceCalculator.PositiveClassName))
        throw new ArgumentException("Classification positive class of the test data doesn't match with the data of training partition.");
      ClassificationPositiveClassName = testPerformanceCalculator.PositiveClassName;
      TestTruePositiveRate = testPerformanceCalculator.TruePositiveRate;
      TestTrueNegativeRate = testPerformanceCalculator.TrueNegativeRate;
      TestPositivePredictiveValue = testPerformanceCalculator.PositivePredictiveValue;
      TestNegativePredictiveValue = testPerformanceCalculator.NegativePredictiveValue;
      TestFalsePositiveRate = testPerformanceCalculator.FalsePositiveRate;
      TestFalseDiscoveryRate = testPerformanceCalculator.FalseDiscoveryRate;
    }
  }
}
