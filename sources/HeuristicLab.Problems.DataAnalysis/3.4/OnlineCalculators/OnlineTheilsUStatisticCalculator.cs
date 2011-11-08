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
  public class OnlineTheilsUStatisticCalculator : IOnlineCalculator {
    private OnlineMeanAndVarianceCalculator squaredErrorMeanCalculator;
    private OnlineMeanAndVarianceCalculator unbiasedEstimatorMeanCalculator;
    private double prevOriginal;
    private int n;

    public double TheilsUStatistic {
      get {
        return Math.Sqrt(squaredErrorMeanCalculator.Mean) / Math.Sqrt(unbiasedEstimatorMeanCalculator.Mean);
      }
    }

    private OnlineCalculatorError errorState;
    public OnlineCalculatorError ErrorState {
      get { return errorState | squaredErrorMeanCalculator.MeanErrorState | unbiasedEstimatorMeanCalculator.MeanErrorState; }
    }

    public OnlineTheilsUStatisticCalculator() {
      squaredErrorMeanCalculator = new OnlineMeanAndVarianceCalculator();
      unbiasedEstimatorMeanCalculator = new OnlineMeanAndVarianceCalculator();
      Reset();
    }

    #region IOnlineEvaluator Members
    public double Value {
      get { return TheilsUStatistic; }
    }

    public void Add(double original, double estimated) {
      if (double.IsNaN(estimated) || double.IsInfinity(estimated) || double.IsNaN(original) || double.IsInfinity(original) || (errorState & OnlineCalculatorError.InvalidValueAdded) > 0) {
        errorState = errorState | OnlineCalculatorError.InvalidValueAdded;
      } else if (n == 0) {
        prevOriginal = original;
        n++;
      } else {
        // error of predicted change
        double errorEstimatedChange = (estimated - original);
        squaredErrorMeanCalculator.Add(errorEstimatedChange * errorEstimatedChange);

        double errorNoChange = (original - prevOriginal);
        unbiasedEstimatorMeanCalculator.Add(errorNoChange * errorNoChange);
        errorState = errorState & (~OnlineCalculatorError.InsufficientElementsAdded);        // n >= 1
        prevOriginal = original;
        n++;
      }
    }


    public void Reset() {
      prevOriginal = double.NaN;
      n = 0;
      squaredErrorMeanCalculator.Reset();
      unbiasedEstimatorMeanCalculator.Reset();
      errorState = OnlineCalculatorError.InsufficientElementsAdded;
    }

    #endregion

    public static double Calculate(IEnumerable<double> originalValues, IEnumerable<double> estimatedValues, out OnlineCalculatorError errorState) {
      IEnumerator<double> originalValuesEnumerator = originalValues.GetEnumerator();
      IEnumerator<double> estimatedValuesEnumerator = estimatedValues.GetEnumerator();
      OnlineTheilsUStatisticCalculator calculator = new OnlineTheilsUStatisticCalculator();

      // add first element of time series as a reference point
      originalValuesEnumerator.MoveNext();
      estimatedValuesEnumerator.MoveNext();
      calculator.Add(originalValuesEnumerator.Current, estimatedValuesEnumerator.Current);

      // always move forward both enumerators (do not use short-circuit evaluation!)
      while (originalValuesEnumerator.MoveNext() & estimatedValuesEnumerator.MoveNext()) {
        double estimated = estimatedValuesEnumerator.Current;
        double original = originalValuesEnumerator.Current;
        calculator.Add(original, estimated);
        if (calculator.ErrorState != OnlineCalculatorError.None) break;
      }

      // check if both enumerators are at the end to make sure both enumerations have the same length
      if (calculator.ErrorState == OnlineCalculatorError.None &&
          (estimatedValuesEnumerator.MoveNext() || originalValuesEnumerator.MoveNext())) {
        throw new ArgumentException("Number of elements in originalValues and estimatedValues enumerations doesn't match.");
      } else {
        errorState = calculator.ErrorState;
        return calculator.TheilsUStatistic;
      }
    }
  }
}
