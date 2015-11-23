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

namespace HeuristicLab.Problems.DataAnalysis {
  public class OnlineMaxAbsoluteErrorCalculator : IOnlineCalculator {

    private double mae;
    private int n;
    public double MaxAbsoluteError {
      get {
        return n > 0 ? mae : 0.0;
      }
    }

    public OnlineMaxAbsoluteErrorCalculator() {
      Reset();
    }

    #region IOnlineCalculator Members
    private OnlineCalculatorError errorState;
    public OnlineCalculatorError ErrorState {
      get { return errorState; }
    }
    public double Value {
      get { return MaxAbsoluteError; }
    }
    public void Reset() {
      n = 0;
      mae = double.MinValue;
      errorState = OnlineCalculatorError.InsufficientElementsAdded;
    }

    public void Add(double original, double estimated) {
      if (double.IsNaN(estimated) || double.IsInfinity(estimated) ||
          double.IsNaN(original) || double.IsInfinity(original) || (errorState & OnlineCalculatorError.InvalidValueAdded) > 0) {
        errorState = errorState | OnlineCalculatorError.InvalidValueAdded;
      } else {
        double error = Math.Abs(estimated - original);
        if (error > mae)
          mae = error;
        n++;
        errorState = errorState & (~OnlineCalculatorError.InsufficientElementsAdded);        // n >= 1
      }
    }
    #endregion

    public static double Calculate(IEnumerable<double> originalValues, IEnumerable<double> estimatedValues, out OnlineCalculatorError errorState) {
      IEnumerator<double> originalEnumerator = originalValues.GetEnumerator();
      IEnumerator<double> estimatedEnumerator = estimatedValues.GetEnumerator();
      OnlineMaxAbsoluteErrorCalculator maeCalculator = new OnlineMaxAbsoluteErrorCalculator();

      // always move forward both enumerators (do not use short-circuit evaluation!)
      while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
        double original = originalEnumerator.Current;
        double estimated = estimatedEnumerator.Current;
        maeCalculator.Add(original, estimated);
        if (maeCalculator.ErrorState != OnlineCalculatorError.None) break;
      }

      // check if both enumerators are at the end to make sure both enumerations have the same length
      if (maeCalculator.ErrorState == OnlineCalculatorError.None &&
         (estimatedEnumerator.MoveNext() || originalEnumerator.MoveNext())) {
        throw new ArgumentException("Number of elements in originalValues and estimatedValues enumerations doesn't match.");
      } else {
        errorState = maeCalculator.ErrorState;
        return maeCalculator.MaxAbsoluteError;
      }
    }
  }
}
