#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;

namespace HeuristicLab.Problems.DataAnalysis {
  public class OnlineMeanSquaredErrorCalculator : DeepCloneable, IOnlineCalculator {

    private double sse;
    private int n;
    public double MeanSquaredError {
      get {
        return n > 0 ? sse / n : 0.0;
      }
    }

    public OnlineMeanSquaredErrorCalculator() {
      Reset();
    }

    protected OnlineMeanSquaredErrorCalculator(OnlineMeanSquaredErrorCalculator original, Cloner cloner)
      : base(original, cloner) {
      sse = original.sse;
      n = original.n;
      errorState = original.errorState;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new OnlineMeanSquaredErrorCalculator(this, cloner);
    }

    #region IOnlineCalculator Members
    private OnlineCalculatorError errorState;
    public OnlineCalculatorError ErrorState {
      get { return errorState; }
    }
    public double Value {
      get { return MeanSquaredError; }
    }
    public void Reset() {
      n = 0;
      sse = 0.0;
      errorState = OnlineCalculatorError.InsufficientElementsAdded;
    }

    public void Add(double original, double estimated) {
      if (double.IsNaN(estimated) || double.IsInfinity(estimated) ||
          double.IsNaN(original) || double.IsInfinity(original) || (errorState & OnlineCalculatorError.InvalidValueAdded) > 0) {
        errorState = errorState | OnlineCalculatorError.InvalidValueAdded;
      } else {
        double error = estimated - original;
        sse += error * error;
        n++;
        errorState = errorState & (~OnlineCalculatorError.InsufficientElementsAdded);        // n >= 1
      }
    }
    #endregion

    public static double Calculate(IEnumerable<double> originalValues, IEnumerable<double> estimatedValues, out OnlineCalculatorError errorState) {
      IEnumerator<double> originalEnumerator = originalValues.GetEnumerator();
      IEnumerator<double> estimatedEnumerator = estimatedValues.GetEnumerator();
      OnlineMeanSquaredErrorCalculator mseCalculator = new OnlineMeanSquaredErrorCalculator();

      // always move forward both enumerators (do not use short-circuit evaluation!)
      while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
        double original = originalEnumerator.Current;
        double estimated = estimatedEnumerator.Current;
        mseCalculator.Add(original, estimated);
        if (mseCalculator.ErrorState != OnlineCalculatorError.None) break;
      }

      // check if both enumerators are at the end to make sure both enumerations have the same length
      if (mseCalculator.ErrorState == OnlineCalculatorError.None &&
         (estimatedEnumerator.MoveNext() || originalEnumerator.MoveNext())) {
        throw new ArgumentException("Number of elements in originalValues and estimatedValues enumerations doesn't match.");
      } else {
        errorState = mseCalculator.ErrorState;
        return mseCalculator.MeanSquaredError;
      }
    }
  }
}
