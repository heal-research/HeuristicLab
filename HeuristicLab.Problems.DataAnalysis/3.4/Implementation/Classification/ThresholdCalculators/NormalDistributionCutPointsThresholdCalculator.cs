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
      thresholdList.Insert(0, double.NegativeInfinity);
      thresholdList.Add(double.PositiveInfinity);


      // find the most likely class for the points between thresholds m
      List<double> filteredThresholds = new List<double>();
      List<double> filteredClassValues = new List<double>();
      for (int i = 0; i < thresholdList.Count - 1; i++) {
        // determine class with maximal density mass between the thresholds
        double maxDensity = DensityMass(thresholdList[i], thresholdList[i + 1], classMean[originalClasses[0]], classStdDev[originalClasses[0]]);
        double maxDensityClassValue = originalClasses[0];
        foreach (var classValue in originalClasses.Skip(1)) {
          double density = DensityMass(thresholdList[i], thresholdList[i + 1], classMean[classValue], classStdDev[classValue]);
          if (density > maxDensity) {
            maxDensity = density;
            maxDensityClassValue = classValue;
          }
        }
        if (maxDensity > double.NegativeInfinity &&
          (filteredClassValues.Count == 0 || !maxDensityClassValue.IsAlmost(filteredClassValues.Last()))) {
          filteredThresholds.Add(thresholdList[i]);
          filteredClassValues.Add(maxDensityClassValue);
        }
      }

      if (filteredThresholds.Count == 0 || !double.IsNegativeInfinity(filteredThresholds.First())) {
        // this happens if there are no thresholds (distributions for all classes are exactly the same)
        // or when the CDF up to the first threshold is zero
        // -> all samples should be classified as the class with the most observations
        // group observations by target class and select the class with largest count 
        double mostFrequentClass = targetClassValues.GroupBy(c => c)
                              .OrderBy(g => g.Count())
                              .Last().Key;
        filteredThresholds.Insert(0, double.NegativeInfinity);
        filteredClassValues.Insert(0, mostFrequentClass);
      }

      thresholds = filteredThresholds.ToArray();
      classValues = filteredClassValues.ToArray();
    }

    private static double sqr2 = Math.Sqrt(2.0);
    // returns the density function of the standard normal distribution at x
    private static double NormalCDF(double x) {
      return 0.5 * (1 + alglib.errorfunction(x / sqr2));
    }

    // approximation of the log of the normal cummulative distribution from the lightspeed toolbox by Tom Minka
    // http://research.microsoft.com/en-us/um/people/minka/software/lightspeed/
    private static double[] c = new double[] { -1, 5 / 2.0, -37 / 3.0, 353 / 4.0, -4081 / 5.0, 55205 / 6.0, -854197 / 7.0 };
    private static double LogNormalCDF(double x) {
      if (x >= -6.5)
        // calculate the log directly if x is large enough
        return Math.Log(NormalCDF(x));
      else {
        double z = Math.Pow(x, -2);
        // asymptotic series for logcdf
        double y = z * (c[0] + z * (c[1] + z * (c[2] + z * (c[3] + z * (c[4] + z * (c[5] + z * c[6]))))));
        return y - 0.5 * Math.Log(2 * Math.PI) - 0.5 * x * x - Math.Log(-x);
      }
    }

    // determines the value NormalCDF(mu,sigma, upper)  - NormalCDF(mu, sigma, lower)
    // = the integral of the PDF of N(mu, sigma) in the range [lower, upper]
    private static double DensityMass(double lower, double upper, double mu, double sigma) {
      if (sigma.IsAlmost(0.0)) {
        if (lower < mu && mu < upper) return 0.0; // all mass is between lower and upper
        else return double.NegativeInfinity; // no mass is between lower and upper
      }

      if (lower > mu) {
        return DensityMass(-upper, -lower, -mu, sigma);
      }

      upper = (upper - mu) / sigma;
      lower = (lower - mu) / sigma;
      if (double.IsNegativeInfinity(lower)) return LogNormalCDF(upper);

      return LogNormalCDF(upper) + Math.Log(1 - Math.Exp(LogNormalCDF(lower) - LogNormalCDF(upper)));
    }

    // Calculates the points x1 and x2 where the distributions N(m1, s1) == N(m2,s2). 
    // In the general case there should be two cut points. If either s1 or s2 is 0 then x1==x2. 
    // If both s1 and s2 are zero than there are no cut points but we should return something reasonable (e.g. (m1 + m2) / 2) then.
    private static void CalculateCutPoints(double m1, double s1, double m2, double s2, out double x1, out double x2) {
      if (s1.IsAlmost(s2)) {
        if (m1.IsAlmost(m2)) {
          x1 = double.NegativeInfinity;
          x2 = double.NegativeInfinity;
        } else {
          // s1==s2 and m1 != m2
          // return something reasonable. cut point should be half way between m1 and m2
          x1 = (m1 + m2) / 2;
          x2 = double.NegativeInfinity;
        }
      } else if (s1.IsAlmost(0.0)) {
        // when s1 is 0.0 the cut points are exactly at m1 ...
        x1 = m1;
        x2 = m1;
      } else if (s2.IsAlmost(0.0)) {
        // ... same for s2
        x1 = m2;
        x2 = m2;
      } else {
        if (s2 < s1) {
          // make sure s2 is the larger std.dev.
          CalculateCutPoints(m2, s2, m1, s1, out x1, out x2);
        } else {
          // general case
          // calculate the solutions x1, x2 where N(m1,s1) == N(m2,s2)
          double g = Math.Sqrt(2 * s2 * s2 * Math.Log(s2 / s1) - 2 * s1 * s1 * Math.Log(s2 / s1) - 2 * m1 * m2 + m1 * m1 + m2 * m2);
          double s = (s1 * s1 - s2 * s2);
          x1 = (m2 * s1 * s1 - m1 * s2 * s2 + s1 * s2 * g) / s;
          x2 = -(m1 * s2 * s2 - m2 * s1 * s1 + s1 * s2 * g) / s;
        }
      }
    }
  }
}
