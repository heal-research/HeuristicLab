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
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HEAL.Attic;
using HeuristicLab.Problems.DataAnalysis.OnlineCalculators;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableType("60599497-EAF0-4DB0-B2E4-D58F34458D8F")]
  public abstract class ClassificationSolutionBase : DataAnalysisSolution, IClassificationSolution {
    private const string TrainingAccuracyResultName = "Accuracy (training)";
    private const string TestAccuracyResultName = "Accuracy (test)";
    private const string TrainingNormalizedGiniCoefficientResultName = "Normalized Gini Coefficient (training)";
    private const string TestNormalizedGiniCoefficientResultName = "Normalized Gini Coefficient (test)";
    private const string ClassificationPerformanceMeasuresResultName = "Classification Performance Measures";

    public new IClassificationModel Model {
      get { return (IClassificationModel)base.Model; }
      protected set { base.Model = value; }
    }

    public new IClassificationProblemData ProblemData {
      get { return (IClassificationProblemData)base.ProblemData; }
      set {
        if (value == null) throw new ArgumentNullException("The problemData must not be null.");
        string errorMessage = string.Empty;
        if (!Model.IsProblemDataCompatible(value, out errorMessage)) throw new ArgumentException(errorMessage);

        base.ProblemData = value;
      }
    }

    #region Results
    public double TrainingAccuracy {
      get { return ((DoubleValue)this[TrainingAccuracyResultName].Value).Value; }
      private set { ((DoubleValue)this[TrainingAccuracyResultName].Value).Value = value; }
    }
    public double TestAccuracy {
      get { return ((DoubleValue)this[TestAccuracyResultName].Value).Value; }
      private set { ((DoubleValue)this[TestAccuracyResultName].Value).Value = value; }
    }
    public double TrainingNormalizedGiniCoefficient {
      get { return ((DoubleValue)this[TrainingNormalizedGiniCoefficientResultName].Value).Value; }
      protected set { ((DoubleValue)this[TrainingNormalizedGiniCoefficientResultName].Value).Value = value; }
    }
    public double TestNormalizedGiniCoefficient {
      get { return ((DoubleValue)this[TestNormalizedGiniCoefficientResultName].Value).Value; }
      protected set { ((DoubleValue)this[TestNormalizedGiniCoefficientResultName].Value).Value = value; }
    }
    public ClassificationPerformanceMeasuresResultCollection ClassificationPerformanceMeasures {
      get { return ((ClassificationPerformanceMeasuresResultCollection)this[ClassificationPerformanceMeasuresResultName].Value); }
      protected set { (this[ClassificationPerformanceMeasuresResultName].Value) = value; }
    }
    #endregion

    [StorableConstructor]
    protected ClassificationSolutionBase(StorableConstructorFlag _) : base(_) { }
    protected ClassificationSolutionBase(ClassificationSolutionBase original, Cloner cloner)
      : base(original, cloner) {
    }
    protected ClassificationSolutionBase(IClassificationModel model, IClassificationProblemData problemData)
      : base(model, problemData) {
      Add(new Result(TrainingAccuracyResultName, "Accuracy of the model on the training partition (percentage of correctly classified instances).", new PercentValue()));
      Add(new Result(TestAccuracyResultName, "Accuracy of the model on the test partition (percentage of correctly classified instances).", new PercentValue()));
      Add(new Result(TrainingNormalizedGiniCoefficientResultName, "Normalized Gini coefficient of the model on the training partition.", new DoubleValue()));
      Add(new Result(TestNormalizedGiniCoefficientResultName, "Normalized Gini coefficient of the model on the test partition.", new DoubleValue()));
      Add(new Result(ClassificationPerformanceMeasuresResultName, @"Classification performance measures.\n
                              In a multiclass classification all misclassifications of the negative class will be treated as true negatives except on positive class estimations.",
                            new ClassificationPerformanceMeasuresResultCollection()));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (string.IsNullOrEmpty(Model.TargetVariable))
        Model.TargetVariable = this.ProblemData.TargetVariable;

      if (!this.ContainsKey(TrainingNormalizedGiniCoefficientResultName))
        Add(new Result(TrainingNormalizedGiniCoefficientResultName, "Normalized Gini coefficient of the model on the training partition.", new DoubleValue()));
      if (!this.ContainsKey(TestNormalizedGiniCoefficientResultName))
        Add(new Result(TestNormalizedGiniCoefficientResultName, "Normalized Gini coefficient of the model on the test partition.", new DoubleValue()));
      if (!this.ContainsKey(ClassificationPerformanceMeasuresResultName)) {
        Add(new Result(ClassificationPerformanceMeasuresResultName, @"Classification performance measures.\n
                              In a multiclass classification all misclassifications of the negative class will be treated as true negatives except on positive class estimations.",
                              new ClassificationPerformanceMeasuresResultCollection()));
        CalculateClassificationResults();
      }
    }

    protected void CalculateClassificationResults() {
      double[] estimatedTrainingClassValues = EstimatedTrainingClassValues.ToArray(); // cache values
      double[] originalTrainingClassValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TrainingIndices).ToArray();

      double[] estimatedTestClassValues = EstimatedTestClassValues.ToArray(); // cache values
      double[] originalTestClassValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TestIndices).ToArray();

      var positiveClassName = ProblemData.PositiveClass;
      double positiveClassValue = ProblemData.GetClassValue(positiveClassName);
      ClassificationPerformanceMeasuresCalculator trainingPerformanceCalculator = new ClassificationPerformanceMeasuresCalculator(positiveClassName, positiveClassValue);
      ClassificationPerformanceMeasuresCalculator testPerformanceCalculator = new ClassificationPerformanceMeasuresCalculator(positiveClassName, positiveClassValue);

      OnlineCalculatorError errorState;
      double trainingAccuracy = OnlineAccuracyCalculator.Calculate(originalTrainingClassValues, estimatedTrainingClassValues, out errorState);
      if (errorState != OnlineCalculatorError.None) trainingAccuracy = double.NaN;
      double testAccuracy = OnlineAccuracyCalculator.Calculate(originalTestClassValues, estimatedTestClassValues, out errorState);
      if (errorState != OnlineCalculatorError.None) testAccuracy = double.NaN;

      TrainingAccuracy = trainingAccuracy;
      TestAccuracy = testAccuracy;

      double trainingNormalizedGini = NormalizedGiniCalculator.Calculate(originalTrainingClassValues, estimatedTrainingClassValues, out errorState);
      if (errorState != OnlineCalculatorError.None) trainingNormalizedGini = double.NaN;
      double testNormalizedGini = NormalizedGiniCalculator.Calculate(originalTestClassValues, estimatedTestClassValues, out errorState);
      if (errorState != OnlineCalculatorError.None) testNormalizedGini = double.NaN;

      TrainingNormalizedGiniCoefficient = trainingNormalizedGini;
      TestNormalizedGiniCoefficient = testNormalizedGini;

      ClassificationPerformanceMeasures.Reset();

      trainingPerformanceCalculator.Calculate(originalTrainingClassValues, estimatedTrainingClassValues);
      if (trainingPerformanceCalculator.ErrorState == OnlineCalculatorError.None)
        ClassificationPerformanceMeasures.SetTrainingResults(trainingPerformanceCalculator);

      testPerformanceCalculator.Calculate(originalTestClassValues, estimatedTestClassValues);
      if (testPerformanceCalculator.ErrorState == OnlineCalculatorError.None)
        ClassificationPerformanceMeasures.SetTestResults(testPerformanceCalculator);

      if (ProblemData.Classes == 2) {
        var f1Training = FOneScoreCalculator.Calculate(originalTrainingClassValues, estimatedTrainingClassValues, out errorState);
        if (errorState == OnlineCalculatorError.None) ClassificationPerformanceMeasures.TrainingF1Score = f1Training;
        var f1Test = FOneScoreCalculator.Calculate(originalTestClassValues, estimatedTestClassValues, out errorState);
        if (errorState == OnlineCalculatorError.None) ClassificationPerformanceMeasures.TestF1Score = f1Test;
      }

      var mccTraining = MatthewsCorrelationCoefficientCalculator.Calculate(originalTrainingClassValues, estimatedTrainingClassValues, out errorState);
      if (errorState == OnlineCalculatorError.None) ClassificationPerformanceMeasures.TrainingMatthewsCorrelation = mccTraining;
      var mccTest = MatthewsCorrelationCoefficientCalculator.Calculate(originalTestClassValues, estimatedTestClassValues, out errorState);
      if (errorState == OnlineCalculatorError.None) ClassificationPerformanceMeasures.TestMatthewsCorrelation = mccTest;
    }

    public abstract IEnumerable<double> EstimatedClassValues { get; }
    public abstract IEnumerable<double> EstimatedTrainingClassValues { get; }
    public abstract IEnumerable<double> EstimatedTestClassValues { get; }

    public abstract IEnumerable<double> GetEstimatedClassValues(IEnumerable<int> rows);

    protected override void RecalculateResults() {
      CalculateClassificationResults();
    }
  }
}
