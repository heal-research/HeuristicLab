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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  /// <summary>
  /// Represents a threshold calculator that maximizes the weighted accuracy of the classifcation model.
  /// </summary>
  [StorableClass]
  [Item("AccuracyMaximizationThresholdCalculator", "Represents a threshold calculator that maximizes the weighted accuracy of the classifcation model.")]
  public class AccuracyMaximizationThresholdCalculator : ThresholdCalculator {

    [StorableConstructor]
    protected AccuracyMaximizationThresholdCalculator(bool deserializing) : base(deserializing) { }
    protected AccuracyMaximizationThresholdCalculator(AccuracyMaximizationThresholdCalculator original, Cloner cloner)
      : base(original, cloner) {
    }
    public AccuracyMaximizationThresholdCalculator()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AccuracyMaximizationThresholdCalculator(this, cloner);
    }

    public override void Calculate(IClassificationProblemData problemData, IEnumerable<double> estimatedValues, IEnumerable<double> targetClassValues, out double[] classValues, out double[] thresholds) {
      AccuracyMaximizationThresholdCalculator.CalculateThresholds(problemData, estimatedValues, targetClassValues, out classValues, out thresholds);
    }

    public static void CalculateThresholds(IClassificationProblemData problemData, IEnumerable<double> estimatedValues, IEnumerable<double> targetClassValues, out double[] classValues, out double[] thresholds) {
      int slices = 100;
      List<double> estimatedValuesList = estimatedValues.ToList();
      double maxEstimatedValue = estimatedValuesList.Max();
      double minEstimatedValue = estimatedValuesList.Min();
      double thresholdIncrement = (maxEstimatedValue - minEstimatedValue) / slices;
      var estimatedAndTargetValuePairs =
        estimatedValuesList.Zip(targetClassValues, (x, y) => new { EstimatedValue = x, TargetClassValue = y })
        .OrderBy(x => x.EstimatedValue)
        .ToList();

      classValues = problemData.ClassValues.OrderBy(x => x).ToArray();
      int nClasses = classValues.Length;
      thresholds = new double[nClasses];
      thresholds[0] = double.NegativeInfinity;
      // thresholds[thresholds.Length - 1] = double.PositiveInfinity;

      // incrementally calculate accuracy of all possible thresholds
      int[,] confusionMatrix = new int[nClasses, nClasses];

      for (int i = 1; i < thresholds.Length; i++) {
        double lowerThreshold = thresholds[i - 1];
        double actualThreshold = Math.Max(lowerThreshold, minEstimatedValue);
        double lowestBestThreshold = double.NaN;
        double highestBestThreshold = double.NaN;
        double bestClassificationScore = double.PositiveInfinity;
        bool seriesOfEqualClassificationScores = false;

        while (actualThreshold < maxEstimatedValue) {
          double classificationScore = 0.0;

          foreach (var pair in estimatedAndTargetValuePairs) {
            //all positives
            if (pair.TargetClassValue.IsAlmost(classValues[i - 1])) {
              if (pair.EstimatedValue > lowerThreshold && pair.EstimatedValue < actualThreshold)
                //true positive
                classificationScore += problemData.GetClassificationPenalty(classValues[i - 1], classValues[i - 1]);
              else
                //false negative
                classificationScore += problemData.GetClassificationPenalty(classValues[i], classValues[i - 1]);
            }
              //all negatives
            else {
              if (pair.EstimatedValue > lowerThreshold && pair.EstimatedValue < actualThreshold)
                //false positive
                classificationScore += problemData.GetClassificationPenalty(classValues[i - 1], classValues[i]);
              else
                //true negative, consider only upper class
                classificationScore += problemData.GetClassificationPenalty(classValues[i], classValues[i]);
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
        double falseNegativePenalty = problemData.GetClassificationPenalty(classValues[i], classValues[i - 1]);
        double falsePositivePenalty = problemData.GetClassificationPenalty(classValues[i - 1], classValues[i]);
        thresholds[i] = (lowestBestThreshold * falsePositivePenalty + highestBestThreshold * falseNegativePenalty) / (falseNegativePenalty + falsePositivePenalty);
      }
    }
  }
}
