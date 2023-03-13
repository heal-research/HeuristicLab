#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.DataAnalysis {
  public static class RocAucCalculator {
    public static double CalculateRocAuc(IReadOnlyList<double> targetClassValues, IReadOnlyList<double> estimatedValues, string className, IReadOnlyList<double> classValues, IReadOnlyList<string> classNames, int slices = 100) {
      double minThreshold = estimatedValues.Min();
      double maxThreshold = estimatedValues.Max();
      double thresholdIncrement = (maxThreshold - minThreshold) / slices;
      minThreshold -= thresholdIncrement;
      maxThreshold += thresholdIncrement;
      
      double classValue = classValues[classNames.IndexOf(className)];
      var rocCurve = CalculateRocCurve(targetClassValues, estimatedValues, classValue, classValues, minThreshold, maxThreshold, thresholdIncrement);
      
      double auc = CalculateAreaUnderCurve(rocCurve);
      return auc;
    }

    public static Dictionary<string, IReadOnlyList<RocPoint>> CalculateRocCurves(IReadOnlyList<double> targetClassValues, IReadOnlyList<double> estimatedValues, IReadOnlyList<double> classValues, IReadOnlyList<string> classNames,  int slices = 100) {
       double minThreshold = estimatedValues.Min();
       double maxThreshold = estimatedValues.Max();
       double thresholdIncrement = (maxThreshold - minThreshold) / slices;
       minThreshold -= thresholdIncrement;
       maxThreshold += thresholdIncrement;

      var rocCurves = new Dictionary<string, IReadOnlyList<RocPoint>>();

      for (int i = 0; i < classValues.Count; i++) {
        double classValue = classValues[i];
        string className = classNames[i];
        
        var rocPoints = CalculateRocCurve(targetClassValues, estimatedValues, classValue, classValues, minThreshold, maxThreshold, thresholdIncrement);
        
        rocCurves[className] = rocPoints;
      }

      return rocCurves;
    }

    public static IReadOnlyList<RocPoint> CalculateRocCurve(IReadOnlyList<double> targetClassValues, IReadOnlyList<double> estimatedValues, double classValue, IReadOnlyList<double> classValues, double minThreshold, double maxThreshold, double thresholdIncrement) {
      var rocPoints = new List<RocPoint>();
      
      int positives = targetClassValues.Count(c => c.IsAlmost(classValue));
      int negatives = targetClassValues.Count - positives;

      for (double lowerThreshold = minThreshold; lowerThreshold < maxThreshold; lowerThreshold += thresholdIncrement) {
        for (double upperThreshold = lowerThreshold + thresholdIncrement; upperThreshold < maxThreshold; upperThreshold += thresholdIncrement) {
          //only adapt lower threshold for binary classification problems and upper class prediction              
          if (classValues.Count == 2 && classValue.IsAlmost(classValues[1])) upperThreshold = double.PositiveInfinity;
          
          int truePositives = 0;
          int falsePositives = 0;

          for (int row = 0; row < estimatedValues.Count; row++) {
            if (lowerThreshold < estimatedValues[row] && estimatedValues[row] < upperThreshold) {
              if (targetClassValues[row].IsAlmost(classValue)) truePositives++;
              else falsePositives++;
            }
          }

          double truePositiveRate = (double)truePositives / positives;
          double falsePositiveRate = (double)falsePositives / negatives;

          var rocPoint = new RocPoint(truePositiveRate, falsePositiveRate, lowerThreshold, upperThreshold);
          if (!rocPoints.Any(x => x.TruePositiveRate >= rocPoint.TruePositiveRate && x.FalsePositiveRate <= rocPoint.FalsePositiveRate)) {
            rocPoints.RemoveAll(x => x.TruePositiveRate <= rocPoint.TruePositiveRate && x.FalsePositiveRate >= rocPoint.FalsePositiveRate);
            rocPoints.Add(rocPoint);
          }

          if (thresholdIncrement.IsAlmost(0.0)) break;
        }

        //only adapt upper threshold for binary classification problems and upper class prediction              
        if (classValues.Count == 2 && classValue.IsAlmost(classValues[0])) lowerThreshold = double.PositiveInfinity;
        
        if (thresholdIncrement.IsAlmost(0.0)) break;
      }

      return rocPoints.OrderBy(x => x.FalsePositiveRate).ToList();
    }

    public static double CalculateAreaUnderCurve(IReadOnlyList<RocPoint> rocPoints) {
      if (rocPoints.Count < 1) return 0.0; //throw new ArgumentException("Could not calculate area under curve if less than 1 data points were given.");

      double auc = 0.0;
      for (int i = 1; i < rocPoints.Count; i++) {
        double width = rocPoints[i].FalsePositiveRate - rocPoints[i - 1].FalsePositiveRate;
        double y1 = rocPoints[i - 1].TruePositiveRate;
        double y2 = rocPoints[i].TruePositiveRate;

        auc += (y1 + y2) * width / 2;
      }
      
      if (rocPoints[0].FalsePositiveRate > 0.0) {
        var lowestPoint = rocPoints[0];
        auc += lowestPoint.TruePositiveRate * lowestPoint.FalsePositiveRate / 2;
      }
      if (rocPoints[rocPoints.Count - 1].FalsePositiveRate < 1.0) {
        var highestPoint = rocPoints[rocPoints.Count - 1];
        auc += (highestPoint.TruePositiveRate + 1.0) * (1.0 - highestPoint.FalsePositiveRate) / 2 ;
      }

      return auc;
    }
    
    public class RocPoint {
      public RocPoint(double truePositiveRate, double falsePositiveRate, double lowerThreshold, double upperThreshold) {
        TruePositiveRate = truePositiveRate;
        FalsePositiveRate = falsePositiveRate;
        LowerThreshold = lowerThreshold;
        UpperThreshold = upperThreshold;

      }
      public double TruePositiveRate { get; }
      public double FalsePositiveRate { get; }
      public double LowerThreshold { get; }
      public double UpperThreshold { get; }
    }
  }
}
