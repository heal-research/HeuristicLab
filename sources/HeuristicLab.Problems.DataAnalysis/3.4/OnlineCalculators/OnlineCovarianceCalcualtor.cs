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

namespace HeuristicLab.Problems.DataAnalysis {
  public class OnlineCovarianceCalculator : IOnlineCalculator {

    private double xMean, yMean, Cn;
    private int n;
    public double Covariance {
      get {
        return n > 0 ? Cn / n : 0.0;
      }
    }

    public OnlineCovarianceCalculator() {
      Reset();
    }

    #region IOnlineCalculator Members
    private OnlineCalculatorError errorState;
    public OnlineCalculatorError ErrorState {
      get { return errorState; }
    }
    public double Value {
      get { return Covariance; }
    }
    public void Reset() {
      n = 0;
      Cn = 0.0;
      xMean = 0.0;
      yMean = 0.0;
      errorState = OnlineCalculatorError.InsufficientElementsAdded;
    }

    public void Add(double x, double y) {
      if (double.IsNaN(y) || double.IsInfinity(y) || double.IsNaN(x) || double.IsInfinity(x) || (errorState & OnlineCalculatorError.InvalidValueAdded) > 0) {
        errorState = errorState | OnlineCalculatorError.InvalidValueAdded;
      } else {
        n++;
        errorState = errorState & (~OnlineCalculatorError.InsufficientElementsAdded);        // n >= 1

        // online calculation of tMean
        xMean = xMean + (x - xMean) / n;
        double delta = y - yMean; // delta = (y - yMean(n-1))
        yMean = yMean + delta / n;

        // online calculation of covariance
        Cn = Cn + delta * (x - xMean); // C(n) = C(n-1) + (y - yMean(n-1)) (t - tMean(n))       
      }
    }
    #endregion

    public static double Calculate(IEnumerable<double> first, IEnumerable<double> second, out OnlineCalculatorError errorState) {
      IEnumerator<double> firstEnumerator = first.GetEnumerator();
      IEnumerator<double> secondEnumerator = second.GetEnumerator();
      OnlineCovarianceCalculator covarianceCalculator = new OnlineCovarianceCalculator();

      // always move forward both enumerators (do not use short-circuit evaluation!)
      while (firstEnumerator.MoveNext() & secondEnumerator.MoveNext()) {
        double x = secondEnumerator.Current;
        double y = firstEnumerator.Current;
        covarianceCalculator.Add(x, y);
        if (covarianceCalculator.ErrorState != OnlineCalculatorError.None) break;
      }

      // check if both enumerators are at the end to make sure both enumerations have the same length
      if (covarianceCalculator.ErrorState == OnlineCalculatorError.None &&
          (secondEnumerator.MoveNext() || firstEnumerator.MoveNext())) {
        throw new ArgumentException("Number of elements in first and second enumeration doesn't match.");
      } else {
        errorState = covarianceCalculator.ErrorState;
        return covarianceCalculator.Covariance;
      }
    }
  }
}
