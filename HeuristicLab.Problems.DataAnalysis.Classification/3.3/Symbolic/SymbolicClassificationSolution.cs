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
using HeuristicLab.Problems.DataAnalysis.Regression.Symbolic;

namespace HeuristicLab.Problems.DataAnalysis.Classification {
  /// <summary>
  /// Represents a solution for a symbolic classification problem which can be visualized in the GUI.
  /// </summary>
  [Item("SymbolicClassificationSolution", "Represents a solution for a symbolic classification problem which can be visualized in the GUI.")]
  [StorableClass]
  public class SymbolicClassificationSolution : SymbolicRegressionSolution, IClassificationSolution {
    public new ClassificationProblemData ProblemData {
      get { return (ClassificationProblemData)base.ProblemData; }
      set { base.ProblemData = value; }
    }

    #region properties
    private List<double> optimalThresholds;
    private List<double> actualThresholds;
    public IEnumerable<double> Thresholds {
      get {
        if (actualThresholds == null) RecalculateEstimatedValues();
        return actualThresholds;
      }
      set {
        if (actualThresholds != null && actualThresholds.SequenceEqual(value))
          return;
        actualThresholds = new List<double>(value);
        OnThresholdsChanged();
      }
    }

    public IEnumerable<double> EstimatedClassValues {
      get { return GetEstimatedClassValues(Enumerable.Range(0, ProblemData.Dataset.Rows)); }
    }

    public IEnumerable<double> EstimatedTrainingClassValues {
      get { return GetEstimatedClassValues(ProblemData.TrainingIndizes); }
    }

    public IEnumerable<double> EstimatedTestClassValues {
      get { return GetEstimatedClassValues(ProblemData.TestIndizes); }
    }

    [StorableConstructor]
    protected SymbolicClassificationSolution(bool deserializing) : base(deserializing) { }
    protected SymbolicClassificationSolution(SymbolicClassificationSolution original, Cloner cloner) : base(original, cloner) { }
    public SymbolicClassificationSolution(ClassificationProblemData problemData, SymbolicRegressionModel model, double lowerEstimationLimit, double upperEstimationLimit)
      : base(problemData, model, lowerEstimationLimit, upperEstimationLimit) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicClassificationSolution(this, cloner);
    }

    protected override void RecalculateEstimatedValues() {
      estimatedValues =
          (from x in Model.GetEstimatedValues(ProblemData, 0, ProblemData.Dataset.Rows)
           let boundedX = Math.Min(UpperEstimationLimit, Math.Max(LowerEstimationLimit, x))
           select double.IsNaN(boundedX) ? UpperEstimationLimit : boundedX).ToList();
      RecalculateClassIntermediates();
      OnEstimatedValuesChanged();
    }

    private void RecalculateClassIntermediates() {
      int slices = 100;

      List<int> classInstances = (from classValue in ProblemData.Dataset.GetVariableValues(ProblemData.TargetVariable.Value)
                                  group classValue by classValue into grouping
                                  select grouping.Count()).ToList();

      List<KeyValuePair<double, double>> estimatedTargetValues =
         (from row in ProblemData.TrainingIndizes
          select new KeyValuePair<double, double>(
            estimatedValues[row],
            ProblemData.Dataset[ProblemData.TargetVariable.Value, row])).ToList();

      List<double> originalClasses = ProblemData.SortedClassValues.ToList();
      double[] thresholds = new double[ProblemData.NumberOfClasses + 1];
      thresholds[0] = double.NegativeInfinity;
      thresholds[thresholds.Length - 1] = double.PositiveInfinity;

      for (int i = 1; i < thresholds.Length - 1; i++) {
        double lowerThreshold = thresholds[i - 1];
        double actualThreshold = originalClasses[i - 1];
        double thresholdIncrement = (originalClasses[i] - originalClasses[i - 1]) / slices;

        double lowestBestThreshold = double.NaN;
        double highestBestThreshold = double.NaN;
        double bestClassificationScore = double.PositiveInfinity;
        bool seriesOfEqualClassificationScores = false;

        while (actualThreshold < originalClasses[i]) {
          double classificationScore = 0.0;

          foreach (KeyValuePair<double, double> estimatedTarget in estimatedTargetValues) {
            //all positives
            if (estimatedTarget.Value.IsAlmost(originalClasses[i - 1])) {
              if (estimatedTarget.Key > lowerThreshold && estimatedTarget.Key < actualThreshold)
                //true positive
                classificationScore += ProblemData.MisclassificationMatrix[i - 1, i - 1];
              else
                //false negative
                classificationScore += ProblemData.MisclassificationMatrix[i, i - 1];
            }
              //all negatives
            else {
              if (estimatedTarget.Key > lowerThreshold && estimatedTarget.Key < actualThreshold)
                //false positive
                classificationScore += ProblemData.MisclassificationMatrix[i - 1, i];
              else
                //true negative, consider only upper class
                classificationScore += ProblemData.MisclassificationMatrix[i, i];
            }
          }

          //new best classification score found
          if (classificationScore < bestClassificationScore) {
            bestClassificationScore = classificationScore;
            lowestBestThreshold = actualThreshold;
            highestBestThreshold = actualThreshold;
            seriesOfEqualClassificationScores = true;
          }
            //equal classification scores => if seriesOfEqualClassifcationScores == true update highest threshold
          else if (Math.Abs(classificationScore - bestClassificationScore) < double.Epsilon && seriesOfEqualClassificationScores)
            highestBestThreshold = actualThreshold;
          //worse classificatoin score found reset seriesOfEqualClassifcationScores
          else seriesOfEqualClassificationScores = false;

          actualThreshold += thresholdIncrement;
        }
        //scale lowest thresholds and highest found optimal threshold according to the misclassification matrix
        double falseNegativePenalty = ProblemData.MisclassificationMatrix[i, i - 1];
        double falsePositivePenalty = ProblemData.MisclassificationMatrix[i - 1, i];
        thresholds[i] = (lowestBestThreshold * falsePositivePenalty + highestBestThreshold * falseNegativePenalty) / (falseNegativePenalty + falsePositivePenalty);
      }
      this.optimalThresholds = new List<double>(thresholds);
      this.actualThresholds = optimalThresholds;
    }

    public IEnumerable<double> GetEstimatedClassValues(IEnumerable<int> rows) {
      double[] classValues = ProblemData.SortedClassValues.ToArray();
      if (estimatedValues == null)
        RecalculateEstimatedValues();
      foreach (int row in rows) {
        double value = estimatedValues[row];
        int classIndex = 0;
        while (value > actualThresholds[classIndex + 1])
          classIndex++;
        yield return classValues[classIndex];
      }
    }
    #endregion

    public event EventHandler ThresholdsChanged;
    private void OnThresholdsChanged() {
      var handler = ThresholdsChanged;
      if (handler != null)
        ThresholdsChanged(this, EventArgs.Empty);
    }
  }
}
