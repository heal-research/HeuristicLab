#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class OnlineMeanErrorCalculator : IOnlineCalculator {

    private readonly OnlineMeanAndVarianceCalculator meanAndVarianceCalculator;
    public double MeanError {
      get { return meanAndVarianceCalculator.Mean; }
    }

    public OnlineMeanErrorCalculator() {
      meanAndVarianceCalculator = new OnlineMeanAndVarianceCalculator();
      Reset();
    }

    #region IOnlineCalculator Members
    public OnlineCalculatorError ErrorState {
      get { return meanAndVarianceCalculator.MeanErrorState; }
    }
    public double Value {
      get { return MeanError; }
    }
    public void Reset() {
      meanAndVarianceCalculator.Reset();
    }

    public void Add(double original, double estimated) {
      meanAndVarianceCalculator.Add(estimated - original);
    }
    #endregion

    public static double Calculate(IEnumerable<double> originalValues, IEnumerable<double> estimatedValues, out OnlineCalculatorError errorState) {
      IEnumerator<double> originalEnumerator = originalValues.GetEnumerator();
      IEnumerator<double> estimatedEnumerator = estimatedValues.GetEnumerator();
      OnlineMeanErrorCalculator meCalculator = new OnlineMeanErrorCalculator();

      // always move forward both enumerators (do not use short-circuit evaluation!)
      while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
        double original = originalEnumerator.Current;
        double estimated = estimatedEnumerator.Current;
        meCalculator.Add(original, estimated);
        if (meCalculator.ErrorState != OnlineCalculatorError.None) break;
      }

      // check if both enumerators are at the end to make sure both enumerations have the same length
      if (meCalculator.ErrorState == OnlineCalculatorError.None &&
         (estimatedEnumerator.MoveNext() || originalEnumerator.MoveNext())) {
        throw new ArgumentException("Number of elements in originalValues and estimatedValues enumerations doesn't match.");
      } else {
        errorState = meCalculator.ErrorState;
        return meCalculator.MeanError;
      }
    }
  }
}
