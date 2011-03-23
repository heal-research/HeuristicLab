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
using HeuristicLab.Common;

namespace HeuristicLab.Problems.DataAnalysis {
  public class OnlinePearsonsRSquaredEvaluator : IOnlineEvaluator {
    private OnlineCovarianceEvaluator covEvaluator = new OnlineCovarianceEvaluator();
    private OnlineMeanAndVarianceCalculator sxEvaluator = new OnlineMeanAndVarianceCalculator();
    private OnlineMeanAndVarianceCalculator syEvaluator = new OnlineMeanAndVarianceCalculator();

    public double RSquared {
      get {
        double xVar = sxEvaluator.PopulationVariance;
        double yVar = syEvaluator.PopulationVariance;
        if (xVar.IsAlmost(0.0) || yVar.IsAlmost(0.0)) {
          return 0.0;
        } else {
          double r = covEvaluator.Covariance / (Math.Sqrt(xVar) * Math.Sqrt(yVar));
          return r * r;
        }
      }
    }

    public OnlinePearsonsRSquaredEvaluator() { }

    #region IOnlineEvaluator Members
    public double Value {
      get { return RSquared; }
    }
    public void Reset() {
      covEvaluator.Reset();
      sxEvaluator.Reset();
      syEvaluator.Reset();
    }

    public void Add(double x, double y) {
      // no need to check validity of values explicitly here as it is checked in all three evaluators 
      covEvaluator.Add(x, y);
      sxEvaluator.Add(x);
      syEvaluator.Add(y);
    }

    #endregion

    public static double Calculate(IEnumerable<double> first, IEnumerable<double> second) {
      IEnumerator<double> firstEnumerator = first.GetEnumerator();
      IEnumerator<double> secondEnumerator = second.GetEnumerator();
      OnlinePearsonsRSquaredEvaluator rSquaredEvaluator = new OnlinePearsonsRSquaredEvaluator();

      // always move forward both enumerators (do not use short-circuit evaluation!)
      while (firstEnumerator.MoveNext() & secondEnumerator.MoveNext()) {
        double estimated = secondEnumerator.Current;
        double original = firstEnumerator.Current;
        rSquaredEvaluator.Add(original, estimated);
      }

      // check if both enumerators are at the end to make sure both enumerations have the same length
      if (secondEnumerator.MoveNext() || firstEnumerator.MoveNext()) {
        throw new ArgumentException("Number of elements in first and second enumeration doesn't match.");
      } else {
        return rSquaredEvaluator.RSquared;
      }
    }
  }
}
