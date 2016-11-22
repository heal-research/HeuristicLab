#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class OnlinePearsonsRCalculator : DeepCloneable, IOnlineCalculator {
    private OnlineCovarianceCalculator covCalculator = new OnlineCovarianceCalculator();
    private OnlineMeanAndVarianceCalculator sxCalculator = new OnlineMeanAndVarianceCalculator();
    private OnlineMeanAndVarianceCalculator syCalculator = new OnlineMeanAndVarianceCalculator();

    public double R {
      get {
        double xVar = sxCalculator.PopulationVariance;
        double yVar = syCalculator.PopulationVariance;
        if (xVar.IsAlmost(0.0) || yVar.IsAlmost(0.0)) {
          return 0.0;
        } else {
          var r = covCalculator.Covariance / (Math.Sqrt(xVar) * Math.Sqrt(yVar));
          if (r < -1.0) r = -1.0;
          else if (r > 1.0) r = 1.0;
          return r;
        }
      }
    }

    public OnlinePearsonsRCalculator() { }

    protected OnlinePearsonsRCalculator(OnlinePearsonsRCalculator other, Cloner cloner) {
      covCalculator = (OnlineCovarianceCalculator)other.covCalculator.Clone(cloner);
      sxCalculator = (OnlineMeanAndVarianceCalculator)other.sxCalculator.Clone(cloner);
      syCalculator = (OnlineMeanAndVarianceCalculator)other.syCalculator.Clone(cloner);
    }

    #region IOnlineCalculator Members
    public OnlineCalculatorError ErrorState {
      get { return covCalculator.ErrorState | sxCalculator.PopulationVarianceErrorState | syCalculator.PopulationVarianceErrorState; }
    }
    public double Value {
      get { return R; }
    }
    public void Reset() {
      covCalculator.Reset();
      sxCalculator.Reset();
      syCalculator.Reset();
    }

    public void Add(double x, double y) {
      // no need to check validity of values explicitly here as it is checked in all three evaluators 
      covCalculator.Add(x, y);
      sxCalculator.Add(x);
      syCalculator.Add(y);
    }

    #endregion

    public static double Calculate(IEnumerable<double> first, IEnumerable<double> second, out OnlineCalculatorError errorState) {
      IEnumerator<double> firstEnumerator = first.GetEnumerator();
      IEnumerator<double> secondEnumerator = second.GetEnumerator();
      var calculator = new OnlinePearsonsRCalculator();

      // always move forward both enumerators (do not use short-circuit evaluation!)
      while (firstEnumerator.MoveNext() & secondEnumerator.MoveNext()) {
        double original = firstEnumerator.Current;
        double estimated = secondEnumerator.Current;
        calculator.Add(original, estimated);
        if (calculator.ErrorState != OnlineCalculatorError.None) break;
      }

      // check if both enumerators are at the end to make sure both enumerations have the same length
      if (calculator.ErrorState == OnlineCalculatorError.None &&
           (secondEnumerator.MoveNext() || firstEnumerator.MoveNext())) {
        throw new ArgumentException("Number of elements in first and second enumeration doesn't match.");
      } else {
        errorState = calculator.ErrorState;
        return calculator.R;
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new OnlinePearsonsRCalculator(this, cloner);
    }
  }
}
