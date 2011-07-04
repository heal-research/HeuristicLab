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

namespace HeuristicLab.Problems.DataAnalysis {
  /// <summary>
  /// Represents a threshold calculator that calculates thresholds as the cutting points between the estimated class distributions (assuming normally distributed class values).
  /// </summary>
  [StorableClass]
  [Item("NormalDistributionCutPointsThresholdCalculator", "Represents a threshold calculator that calculates thresholds as the cutting points between the estimated class distributions (assuming normally distributed class values).")]
  public class NormalDistributionCutPointsThresholdCalculator : ThresholdCalculator {

    [StorableConstructor]
    protected NormalDistributionCutPointsThresholdCalculator(bool deserializing) : base(deserializing) { }
    protected NormalDistributionCutPointsThresholdCalculator(NormalDistributionCutPointsThresholdCalculator original, Cloner cloner)
      : base(original, cloner) {
    }
    public NormalDistributionCutPointsThresholdCalculator()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NormalDistributionCutPointsThresholdCalculator(this, cloner);
    }

    public override void Calculate(IClassificationProblemData problemData, IEnumerable<double> estimatedValues, IEnumerable<double> targetClassValues, out double[] classValues, out double[] thresholds) {
      NormalDistributionCutPointsThresholdCalculator.CalculateThresholds(problemData, estimatedValues, targetClassValues, out classValues, out thresholds);
    }

    public static void CalculateThresholds(IClassificationProblemData problemData, IEnumerable<double> estimatedValues, IEnumerable<double> targetClassValues, out double[] classValues, out double[] thresholds) {
      double maxEstimatedValue = estimatedValues.Max();
      double minEstimatedValue = estimatedValues.Min();
      var estimatedTargetValues = Enumerable.Zip(estimatedValues, targetClassValues, (e, t) => new { EstimatedValue = e, TargetValue = t }).ToList();

      Dictionary<double, double> classMean = new Dictionary<double, double>();
      Dictionary<double, double> classStdDev = new Dictionary<double, double>();
      // calculate moments per class
      foreach (var group in estimatedTargetValues.GroupBy(p => p.TargetValue)) {
        IEnumerable<double> estimatedClassValues = group.Select(x => x.EstimatedValue);
        double classValue = group.Key;
        double mean, variance;
        OnlineCalculatorError meanErrorState, varianceErrorState;
        OnlineMeanAndVarianceCalculator.Calculate(estimatedClassValues, out mean, out variance, out meanErrorState, out varianceErrorState);

        if (meanErrorState == OnlineCalculatorError.None && varianceErrorState == OnlineCalculatorError.None) {
          classMean[classValue] = mean;
          classStdDev[classValue] = Math.Sqrt(variance);
        }
      }
      double[] originalClasses = classMean.Keys.OrderBy(x => x).ToArray();
      int nClasses = originalClasses.Length;
      List<double> thresholdList = new List<double>();
      for (int i = 0; i < nClasses - 1; i++) {
        for (int j = i + 1; j < nClasses; j++) {
          double x1, x2;
          double class0 = originalClasses[i];
          double class1 = originalClasses[j];
          // calculate all thresholds
          CalculateCutPoints(classMean[class0], classStdDev[class0], classMean[class1], classStdDev[class1], out x1, out x2);
          if (!thresholdList.Any(x => x.IsAlmost(x1))) thresholdList.Add(x1);
          if (!thresholdList.Any(x => x.IsAlmost(x2))) thresholdList.Add(x2);
        }
      }
      thresholdList.Sort();
      thresholdList.Insert(0, double.NegativeInfinity);

      // determine class values for each partition separated by a threshold by calculating the density of all class distributions
      // all points in the partition are classified as the class with the maximal density in the parition
      List<double> classValuesList = new List<double>();
      for (int i = 0; i < thresholdList.Count; i++) {
        double m;
        if (double.IsNegativeInfinity(thresholdList[i])) {
          m = thresholdList[i + 1] - 1.0; // smaller than the smalles non-infinity threshold
        } else if (i == thresholdList.Count - 1) {
          // last threshold
          m = thresholdList[i] + 1.0; // larger than the last threshold
        } else {
          m = thresholdList[i] + (thresholdList[i + 1] - thresholdList[i]) / 2.0; // middle of partition
        }

        // determine class with maximal probability density in m
        double maxDensity = double.MinValue;
        double maxDensityClassValue = -1;
        foreach (var classValue in originalClasses) {
          double density = NormalDensity(m, classMean[classValue], classStdDev[classValue]);
          if (density > maxDensity) {
            maxDensity = density;
            maxDensityClassValue = classValue;
          }
        }
        classValuesList.Add(maxDensityClassValue);
      }

      // only keep thresholds at which the class changes 
      // class B overrides threshold s. So only thresholds r and t are relevant and have to be kept
      // 
      //      A    B  C
      //       /\  /\/\        
      //      / r\/ /\t\       
      //     /   /\/  \ \      
      //    /   / /\s  \ \     
      //  -/---/-/ -\---\-\----
      List<double> filteredThresholds = new List<double>();
      List<double> filteredClassValues = new List<double>();
      filteredThresholds.Add(thresholdList[0]);
      filteredClassValues.Add(classValuesList[0]);
      for (int i = 0; i < classValuesList.Count - 1; i++) {
        if (classValuesList[i] != classValuesList[i + 1]) {
          filteredThresholds.Add(thresholdList[i + 1]);
          filteredClassValues.Add(classValuesList[i + 1]);
        }
      }
      thresholds = filteredThresholds.ToArray();
      classValues = filteredClassValues.ToArray();
    }

    private static double NormalDensity(double x, double mu, double sigma) {
      if (sigma.IsAlmost(0.0)) {
        if (x.IsAlmost(mu)) return 1.0; else return 0.0;
      } else {
        return (1.0 / Math.Sqrt(2.0 * Math.PI * sigma * sigma)) * Math.Exp(-((x - mu) * (x - mu)) / (2.0 * sigma * sigma));
      }
    }

    private static void CalculateCutPoints(double m1, double s1, double m2, double s2, out double x1, out double x2) {
      double a = (s1 * s1 - s2 * s2);
      x1 = -(-m2 * s1 * s1 + m1 * s2 * s2 + Math.Sqrt(s1 * s1 * s2 * s2 * ((m1 - m2) * (m1 - m2) + 2.0 * (-s1 * s1 + s2 * s2) * Math.Log(s2 / s1)))) / a;
      x2 = (m2 * s1 * s1 - m1 * s2 * s2 + Math.Sqrt(s1 * s1 * s2 * s2 * ((m1 - m2) * (m1 - m2) + 2.0 * (-s1 * s1 + s2 * s2) * Math.Log(s2 / s1)))) / a;
    }
  }
}
