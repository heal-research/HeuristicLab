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
  public class OnlineMeanSquaredErrorEvaluator : IOnlineEvaluator {

    private double sse;
    private int n;
    public double MeanSquaredError {
      get {
        return n > 0 ? sse / n : 0.0;
      }
    }

    public OnlineMeanSquaredErrorEvaluator() {
      Reset();
    }

    #region IOnlineEvaluator Members
    private OnlineEvaluatorError errorState;
    public OnlineEvaluatorError ErrorState {
      get { return errorState; }
    }
    public double Value {
      get { return MeanSquaredError; }
    }
    public void Reset() {
      n = 0;
      sse = 0.0;
      errorState = OnlineEvaluatorError.InsufficientElementsAdded;
    }

    public void Add(double original, double estimated) {
      if (double.IsNaN(estimated) || double.IsInfinity(estimated) ||
          double.IsNaN(original) || double.IsInfinity(original) || (errorState & OnlineEvaluatorError.InvalidValueAdded) > 0) {
        errorState = errorState | OnlineEvaluatorError.InvalidValueAdded;
      } else {
        double error = estimated - original;
        sse += error * error;
        n++;
        errorState = errorState & (~OnlineEvaluatorError.InsufficientElementsAdded);        // n >= 1
      }
    }
    #endregion

    public static double Calculate(IEnumerable<double> first, IEnumerable<double> second, out OnlineEvaluatorError errorState) {
      IEnumerator<double> firstEnumerator = first.GetEnumerator();
      IEnumerator<double> secondEnumerator = second.GetEnumerator();
      OnlineMeanSquaredErrorEvaluator mseEvaluator = new OnlineMeanSquaredErrorEvaluator();

      // always move forward both enumerators (do not use short-circuit evaluation!)
      while (firstEnumerator.MoveNext() & secondEnumerator.MoveNext()) {
        double estimated = secondEnumerator.Current;
        double original = firstEnumerator.Current;
        mseEvaluator.Add(original, estimated);
      }

      // check if both enumerators are at the end to make sure both enumerations have the same length
      if (secondEnumerator.MoveNext() || firstEnumerator.MoveNext()) {
        throw new ArgumentException("Number of elements in first and second enumeration doesn't match.");
      } else {
        errorState = mseEvaluator.ErrorState;
        return mseEvaluator.MeanSquaredError;
      }
    }
  }
}
