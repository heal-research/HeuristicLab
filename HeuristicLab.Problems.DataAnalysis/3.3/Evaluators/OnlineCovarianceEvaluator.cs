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

namespace HeuristicLab.Problems.DataAnalysis.Evaluators {
  public class OnlineCovarianceEvaluator : IOnlineEvaluator {

    private double originalMean, estimatedMean, Cn;
    private int n;
    public double Covariance {
      get {
        if (n < 1)
          throw new InvalidOperationException("No elements");
        else
          return Cn / n;
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
          double.IsNaN(original) || double.IsInfinity(original)) {
        throw new ArgumentException("Covariance is not defined for series containing NaN or infinity elements");
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
  }
}
