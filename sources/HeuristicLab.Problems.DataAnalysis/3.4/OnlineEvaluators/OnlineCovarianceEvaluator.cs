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
  public class OnlineCovarianceEvaluator : IOnlineEvaluator {

    private double originalMean, estimatedMean, Cn;
    private int n;
    public double Covariance {
      get {
        return n > 0 ? Cn / n : 0.0;
      }
    }

    public OnlineCovarianceEvaluator() {
      Reset();
    }

    #region IOnlineEvaluator Members
    public double Value {
      get { return Covariance; }
    }
    public void Reset() {
      n = 0;
      Cn = 0.0;
      originalMean = 0.0;
      estimatedMean = 0.0;
    }

    public void Add(double original, double estimated) {
      if (double.IsNaN(estimated) || double.IsInfinity(estimated) ||
          double.IsNaN(original) || double.IsInfinity(original) ||
         double.IsNaN(Cn)) {
        Cn = double.NaN;
      } else {
        n++;
        // online calculation of tMean
        originalMean = originalMean + (original - originalMean) / n;
        double delta = estimated - estimatedMean; // delta = (y - yMean(n-1))
        estimatedMean = estimatedMean + delta / n;

        // online calculation of covariance
        Cn = Cn + delta * (original - originalMean); // C(n) = C(n-1) + (y - yMean(n-1)) (t - tMean(n))       
      }
    }
    #endregion

    public static double Calculate(IEnumerable<double> first, IEnumerable<double> second) {
      IEnumerator<double> firstEnumerator = first.GetEnumerator();
      IEnumerator<double> secondEnumerator = second.GetEnumerator();
      OnlineCovarianceEvaluator covarianceEvaluator = new OnlineCovarianceEvaluator();

      // always move forward both enumerators (do not use short-circuit evaluation!)
      while (firstEnumerator.MoveNext() & secondEnumerator.MoveNext()) {
        double estimated = secondEnumerator.Current;
        double original = firstEnumerator.Current;
        covarianceEvaluator.Add(original, estimated);
      }

      // check if both enumerators are at the end to make sure both enumerations have the same length
      if (secondEnumerator.MoveNext() || firstEnumerator.MoveNext()) {
        throw new ArgumentException("Number of elements in first and second enumeration doesn't match.");
      } else {
        return covarianceEvaluator.Covariance;
      }
    }
  }
}
