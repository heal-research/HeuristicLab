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


namespace HeuristicLab.Problems.DataAnalysis {
  public class OnlineWeightedDirectionalSymmetryCalculator : IOnlineCalculator {
    private double prevEstimated;
    private double prevOriginal;
    private int n;
    private double correctSum;
    private double incorrectSum;

    public double WeightedDirectionalSymmetry {
      get {
        if (n <= 1) return 0.0;
        return incorrectSum / correctSum;
      }
    }

    public OnlineWeightedDirectionalSymmetryCalculator() {
      Reset();
    }

    public double Value {
      get { return WeightedDirectionalSymmetry; }
    }

    private OnlineCalculatorError errorState;
    public OnlineCalculatorError ErrorState {
      get { return errorState; }
    }

    public void Add(double original, double estimated) {
      if (double.IsNaN(estimated) || double.IsInfinity(estimated) || double.IsNaN(original) || double.IsInfinity(original) || (errorState & OnlineCalculatorError.InvalidValueAdded) > 0) {
        errorState = errorState | OnlineCalculatorError.InvalidValueAdded;
      } else if (n == 0) {
        prevOriginal = original;
        prevEstimated = estimated;
        n++;
      } else {
        double err = Math.Abs(original - estimated);
        if ((original - prevOriginal) * (estimated - prevEstimated) >= 0.0) {
          correctSum += err;
        } else {
          incorrectSum += err;
        }
        n++;
        errorState = errorState & (~OnlineCalculatorError.InsufficientElementsAdded);        // n >= 1
        prevOriginal = original;
        prevEstimated = estimated;
      }
    }

    public void Reset() {
      n = 0;
      correctSum = 0;
      incorrectSum = 0;
      prevOriginal = double.NaN;
      prevEstimated = double.NaN;
      errorState = OnlineCalculatorError.InsufficientElementsAdded;
    }


    public static double Calculate(IEnumerable<double> originalValues, IEnumerable<double> estimatedValues, out OnlineCalculatorError errorState) {
      IEnumerator<double> originalEnumerator = originalValues.GetEnumerator();
      IEnumerator<double> estimatedEnumerator = estimatedValues.GetEnumerator();
      OnlineWeightedDirectionalSymmetryCalculator dsCalculator = new OnlineWeightedDirectionalSymmetryCalculator();

      // add first element of time series as a reference point
      originalEnumerator.MoveNext();
      estimatedEnumerator.MoveNext();
      dsCalculator.Add(originalEnumerator.Current, estimatedEnumerator.Current);

      // always move forward both enumerators (do not use short-circuit evaluation!)
      while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
        double original = originalEnumerator.Current;
        double estimated = estimatedEnumerator.Current;
        dsCalculator.Add(original, estimated);
        if (dsCalculator.ErrorState != OnlineCalculatorError.None) break;
      }

      // check if both enumerators are at the end to make sure both enumerations have the same length
      if (dsCalculator.ErrorState == OnlineCalculatorError.None &&
          (originalEnumerator.MoveNext() || estimatedEnumerator.MoveNext())) {
        throw new ArgumentException("Number of elements in originalValues and estimatedValues enumerations doesn't match.");
      } else {
        errorState = dsCalculator.ErrorState;
        return dsCalculator.WeightedDirectionalSymmetry;
      }
    }
  }
}
