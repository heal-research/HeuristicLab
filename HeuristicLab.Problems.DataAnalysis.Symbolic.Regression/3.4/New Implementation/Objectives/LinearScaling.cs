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

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  public static class LinearScaling {
    [ThreadStatic]
    private static double[] cache;

    public static IEnumerable<double> ScaleEstimatedValues( IEnumerable<double> estimatedValues, IEnumerable<double> targetValues,
      double lowerEstimationLimit, double upperEstimationLimit, int maxRows) {

      if (cache == null || cache.Length < maxRows) {
        cache = new double[maxRows];
      }

      // calculate linear scaling
      int i = 0;
      var linearScalingCalculator = new OnlineLinearScalingParameterCalculator();
      var targetValuesEnumerator = targetValues.GetEnumerator();
      var estimatedValuesEnumerator = estimatedValues.GetEnumerator();
      while (targetValuesEnumerator.MoveNext() & estimatedValuesEnumerator.MoveNext()) {
        double target = targetValuesEnumerator.Current;
        double estimated = estimatedValuesEnumerator.Current;
        cache[i] = estimated;
        if (!double.IsNaN(estimated) && !double.IsInfinity(estimated))
          linearScalingCalculator.Add(estimated, target);
        i++;
      }
      if (linearScalingCalculator.ErrorState == OnlineCalculatorError.None && (targetValuesEnumerator.MoveNext() || estimatedValuesEnumerator.MoveNext()))
        throw new ArgumentException("Number of elements in target and estimated values enumeration do not match.");

      double alpha = linearScalingCalculator.Alpha;
      double beta = linearScalingCalculator.Beta;
      if (linearScalingCalculator.ErrorState != OnlineCalculatorError.None) {
        alpha = 0.0;
        beta = 1.0;
      }

      //calculate the quality by using the passed online calculator
      targetValuesEnumerator = targetValues.GetEnumerator();
      var scaledBoundedEstimatedValuesEnumerator = Enumerable.Range(0, i).Select(x => cache[x] * beta + alpha)
        .LimitToRange(lowerEstimationLimit, upperEstimationLimit);

      return scaledBoundedEstimatedValuesEnumerator;
    }
  }
}
