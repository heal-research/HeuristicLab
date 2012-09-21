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
      var estimatedTargetValues = Enumerable.Zip(estimatedValues, targetClassValues, (e, t) => new { EstimatedValue = e, TargetValue = t }).ToList();
      double estimatedValuesRange = estimatedValues.Range();

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

          // if the two cut points are too close (for instance because the stdDev=0)
          // then move them by 0.1% of the range of estimated values
          if (x1.IsAlmost(x2)) {
            x1 -= 0.001 * estimatedValuesRange;
            x2 += 0.001 * estimatedValuesRange;
          }
          if (!double.IsInfinity(x1) && !thresholdList.Any(x => x.IsAlmost(x1))) thresholdList.Add(x1);
          if (!double.IsInfinity(x2) && !thresholdList.Any(x => x.IsAlmost(x2))) thresholdList.Add(x2);
        }
      }
      thresholdList.Sort();

      // add small value and large value for the calculation of most influential class in each thresholded section
      thresholdList.Insert(0, estimatedValues.Min() - 1);
      thresholdList.Add(estimatedValues.Max() + 1);

      // determine class values for each partition separated by a threshold by calculating the density of all class distributions
      // all points in the partition are classified as the class with the maximal density in the parition
      List<double> classValuesList = new List<double>();
      if (thresholdList.Count == 2) {
        // this happens if there are no thresholds (distributions for all classes are exactly the same)
        // -> all samples should be classified as the first class
        classValuesList.Add(originalClasses[0]);
      } else {
        // at least one reasonable threshold ...
        // find the most likely class for the points between thresholds m
        for (int i = 0; i < thresholdList.Count - 1; i++) {

          // determine class with maximal density mass between the thresholds
          double maxDensity = LogNormalDensityMass(thresholdList[i], thresholdList[i + 1], classMean[originalClasses[0]], classStdDev[originalClasses[0]]);
          double maxDensityClassValue = originalClasses[0];
          foreach (var classValue in originalClasses.Skip(1)) {
            double density = LogNormalDensityMass(thresholdList[i], thresholdList[i + 1], classMean[classValue], classStdDev[classValue]);
            if (density > maxDensity) {
              maxDensity = density;
              maxDensityClassValue = classValue;
            }
          }
          classValuesList.Add(maxDensityClassValue);
        }
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
      filteredThresholds.Add(double.NegativeInfinity); // the smallest possible threshold for the first class
      filteredClassValues.Add(classValuesList[0]);
      // do not include the last threshold which was just needed for the previous step
      for (int i = 0; i < classValuesList.Count - 1; i++) {
        if (!classValuesList[i].IsAlmost(classValuesList[i + 1])) {
          filteredThresholds.Add(thresholdList[i + 1]);
          filteredClassValues.Add(classValuesList[i + 1]);
        }
      }
      thresholds = filteredThresholds.ToArray();
      classValues = filteredClassValues.ToArray();
    }

    private static double LogNormalDensityMass(double lower, double upper, double mu, double sigma) {
      if (sigma.IsAlmost(0.0)) {
        if (lower < mu && mu < upper) return double.PositiveInfinity; // log(1)
        else return double.NegativeInfinity; // log(0)
      }

      Func<double, double> f = (x) =>
        x * -0.5 * Math.Log(2.0 * Math.PI * sigma * sigma) - Math.Pow(x - mu, 3) / (3 * 2.0 * sigma * sigma);

      if (double.IsNegativeInfinity(lower)) return f(upper);
      else return f(upper) - f(lower);
    }

    private static void CalculateCutPoints(double m1, double s1, double m2, double s2, out double x1, out double x2) {
      if (s1.IsAlmost(s2)) {
        if (m1.IsAlmost(m2)) {
          x1 = double.NegativeInfinity;
          x2 = double.NegativeInfinity;
        } else {
          x1 = (m1 + m2) / 2;
          x2 = double.NegativeInfinity;
        }
      } else if (s1.IsAlmost(0.0)) {
        x1 = m1;
        x2 = m1;
      } else if (s2.IsAlmost(0.0)) {
        x1 = m2;
        x2 = m2;
      } else {
        if (s2 < s1) {
          // make sure s2 is the larger std.dev.
          CalculateCutPoints(m2, s2, m1, s1, out x1, out x2);
        } else {
          double a = (s1 + s2) * (s1 - s2);
          double g = Math.Sqrt(s1 * s1 * s2 * s2 * ((m1 - m2) * (m1 - m2) + 2.0 * (s1 * s1 + s2 * s2) * Math.Log(s2 / s1)));
          double m1s2 = m1 * s2 * s2;
          double m2s1 = m2 * s1 * s1;
          x1 = -(-m2s1 + m1s2 + g) / a;
          x2 = (m2s1 - m1s2 + g) / a;
        }
      }
    }
  }
}
