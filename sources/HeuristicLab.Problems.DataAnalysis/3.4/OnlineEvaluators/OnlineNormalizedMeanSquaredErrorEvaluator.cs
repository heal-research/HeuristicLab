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
  public class OnlineNormalizedMeanSquaredErrorEvaluator : IOnlineEvaluator {
    private OnlineMeanAndVarianceCalculator meanSquaredErrorCalculator;
    private OnlineMeanAndVarianceCalculator originalVarianceCalculator;

    public double NormalizedMeanSquaredError {
      get {
        double var = originalVarianceCalculator.Variance;
        double m = meanSquaredErrorCalculator.Mean;
        return var > 0 ? m / var : 0.0;
      }
    }

    public OnlineNormalizedMeanSquaredErrorEvaluator() {
      meanSquaredErrorCalculator = new OnlineMeanAndVarianceCalculator();
      originalVarianceCalculator = new OnlineMeanAndVarianceCalculator();
      Reset();
    }

    #region IOnlineEvaluator Members
    public double Value {
      get { return NormalizedMeanSquaredError; }
    }

    public void Reset() {
      meanSquaredErrorCalculator.Reset();
      originalVarianceCalculator.Reset();
    }

    public void Add(double original, double estimated) {
      // no need to check for validity of values explicitly as it is checked in the meanAndVariance calculator anyway
      double error = estimated - original;
      meanSquaredErrorCalculator.Add(error * error);
      originalVarianceCalculator.Add(original);
    }
    #endregion

    public static double Calculate(IEnumerable<double> first, IEnumerable<double> second) {
      IEnumerator<double> firstEnumerator = first.GetEnumerator();
      IEnumerator<double> secondEnumerator = second.GetEnumerator();
      OnlineNormalizedMeanSquaredErrorEvaluator normalizedMSEEvaluator = new OnlineNormalizedMeanSquaredErrorEvaluator();

      // always move forward both enumerators (do not use short-circuit evaluation!)
      while (firstEnumerator.MoveNext() & secondEnumerator.MoveNext()) {
        double estimated = secondEnumerator.Current;
        double original = firstEnumerator.Current;
        normalizedMSEEvaluator.Add(original, estimated);
      }

      // check if both enumerators are at the end to make sure both enumerations have the same length
      if (secondEnumerator.MoveNext() || firstEnumerator.MoveNext()) {
        throw new ArgumentException("Number of elements in first and second enumeration doesn't match.");
      } else {
        return normalizedMSEEvaluator.NormalizedMeanSquaredError;
      }
    }
  }
}
